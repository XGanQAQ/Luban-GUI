#!/usr/bin/env python3
"""
递归抓取 Luban 文档并保存为 Markdown。

默认通过 r.jina.ai 将网页转换为 Markdown，避免自行做 HTML->Markdown 解析。
示例:
    python tools/crawl_luban_docs.py \
      --seed https://www.datable.cn/docs/basic \
      --seed https://www.datable.cn/docs/beginner \
      --out-dir docs/lubanDocs_crawled
"""

from __future__ import annotations

import argparse
import collections
import html
import html.parser
import pathlib
import re
import sys
import time
import urllib.error
import urllib.parse
import urllib.request
from typing import Iterable


MARKDOWN_LINK_RE = re.compile(r"\[[^\]]+\]\(([^)]+)\)")
SOURCE_LINE_RE = re.compile(r"^>\s*Source:\s*(https?://\S+)\s*$", re.IGNORECASE)
HTML_HREF_RE = re.compile(r"href\s*=\s*[\"']([^\"']+)[\"']", re.IGNORECASE)
HTML_TITLE_RE = re.compile(r"<title[^>]*>(.*?)</title>", re.IGNORECASE | re.DOTALL)
SKIP_EXTENSIONS = {
    ".png",
    ".jpg",
    ".jpeg",
    ".gif",
    ".svg",
    ".webp",
    ".ico",
    ".css",
    ".js",
    ".zip",
    ".pdf",
    ".xml",
    ".json",
}


class _SimpleHtmlTextExtractor(html.parser.HTMLParser):
    BLOCK_TAGS = {
        "p",
        "div",
        "section",
        "article",
        "header",
        "footer",
        "main",
        "aside",
        "br",
        "li",
        "ul",
        "ol",
        "h1",
        "h2",
        "h3",
        "h4",
        "h5",
        "h6",
        "pre",
        "code",
        "blockquote",
        "tr",
        "table",
    }
    IGNORE_TAGS = {"script", "style", "noscript"}

    def __init__(self) -> None:
        super().__init__()
        self.parts: list[str] = []
        self.ignore_depth = 0

    def handle_starttag(self, tag: str, attrs) -> None:  # type: ignore[override]
        t = tag.lower()
        if t in self.IGNORE_TAGS:
            self.ignore_depth += 1
            return
        if t in self.BLOCK_TAGS:
            self.parts.append("\n")

    def handle_endtag(self, tag: str) -> None:  # type: ignore[override]
        t = tag.lower()
        if t in self.IGNORE_TAGS:
            self.ignore_depth = max(0, self.ignore_depth - 1)
            return
        if t in self.BLOCK_TAGS:
            self.parts.append("\n")

    def handle_data(self, data: str) -> None:  # type: ignore[override]
        if self.ignore_depth > 0:
            return
        text = data.strip()
        if text:
            self.parts.append(text)

    def get_text(self) -> str:
        text = " ".join(self.parts)
        text = re.sub(r"\n\s*\n+", "\n\n", text)
        text = re.sub(r"[ \t]+", " ", text)
        return text.strip()


class CrawlConfig:
    def __init__(
        self,
        out_dir: pathlib.Path,
        domain: str,
        scope_prefix: str,
        max_pages: int,
        delay_sec: float,
        overwrite: bool,
        timeout_sec: int,
        index_filenames: bool,
    ) -> None:
        self.out_dir = out_dir
        self.domain = domain.lower()
        self.scope_prefix = scope_prefix if scope_prefix.startswith("/") else f"/{scope_prefix}"
        self.max_pages = max_pages
        self.delay_sec = delay_sec
        self.overwrite = overwrite
        self.timeout_sec = timeout_sec
        self.index_filenames = index_filenames


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description="抓取 datable/luban docs 到本地 Markdown")
    parser.add_argument(
        "--seed",
        action="append",
        default=[],
        help="起始 URL，可重复传入多个",
    )
    parser.add_argument(
        "--seed-file",
        default="",
        help="包含种子 URL 的文本文件（每行一个）",
    )
    parser.add_argument(
        "--from-existing-sources",
        default="",
        help="从已有 Markdown 的 '> Source: ...' 行提取种子 URL（目录路径）",
    )
    parser.add_argument(
        "--out-dir",
        default="docs/lubanDocs_crawled",
        help="输出目录",
    )
    parser.add_argument(
        "--domain",
        default="www.datable.cn",
        help="限制抓取域名",
    )
    parser.add_argument(
        "--scope-prefix",
        default="/docs",
        help="限制抓取路径前缀",
    )
    parser.add_argument(
        "--max-pages",
        type=int,
        default=300,
        help="最多抓取页面数",
    )
    parser.add_argument(
        "--delay-sec",
        type=float,
        default=0.2,
        help="每次请求后延迟秒数",
    )
    parser.add_argument(
        "--timeout-sec",
        type=int,
        default=20,
        help="HTTP 请求超时秒数",
    )
    parser.add_argument(
        "--overwrite",
        action="store_true",
        help="允许覆盖已存在文件",
    )
    parser.add_argument(
        "--index-filenames",
        action="store_true",
        help="使用旧命名方式，将目录页保存为 index.md（默认关闭）",
    )
    return parser


def read_seed_file(seed_file: str) -> list[str]:
    if not seed_file:
        return []
    p = pathlib.Path(seed_file)
    if not p.exists():
        raise FileNotFoundError(f"seed 文件不存在: {p}")
    urls: list[str] = []
    for line in p.read_text(encoding="utf-8").splitlines():
        text = line.strip()
        if not text or text.startswith("#"):
            continue
        urls.append(text)
    return urls


def read_sources_from_markdown(root: str) -> list[str]:
    if not root:
        return []
    base = pathlib.Path(root)
    if not base.exists():
        raise FileNotFoundError(f"Markdown 目录不存在: {base}")

    urls: list[str] = []
    for md in base.rglob("*.md"):
        try:
            for line in md.read_text(encoding="utf-8", errors="ignore").splitlines():
                m = SOURCE_LINE_RE.match(line.strip())
                if m:
                    urls.append(m.group(1))
                    break
        except OSError:
            continue
    return urls


def normalize_seed_urls(urls: Iterable[str]) -> list[str]:
    normalized: list[str] = []
    seen: set[str] = set()
    for raw in urls:
        u = raw.strip()
        if not u:
            continue
        parsed = urllib.parse.urlparse(u)
        if not parsed.scheme:
            u = f"https://{u}"
            parsed = urllib.parse.urlparse(u)
        if parsed.scheme not in {"http", "https"}:
            continue
        canonical = parsed._replace(fragment="").geturl()
        if canonical not in seen:
            seen.add(canonical)
            normalized.append(canonical)
    return normalized


def is_in_scope(url: str, cfg: CrawlConfig) -> bool:
    parsed = urllib.parse.urlparse(url)
    if parsed.scheme not in {"http", "https"}:
        return False
    if parsed.netloc.lower() != cfg.domain:
        return False
    return parsed.path.startswith(cfg.scope_prefix)


def should_skip_url(url: str) -> bool:
    path = urllib.parse.urlparse(url).path.lower()
    suffix = pathlib.Path(path).suffix.lower()
    return suffix in SKIP_EXTENSIONS


def jina_url(url: str) -> str:
    # r.jina.ai 支持: https://r.jina.ai/http://example.com/page
    return f"https://r.jina.ai/{url}"


def http_get_text(url: str, timeout_sec: int) -> str:
    req = urllib.request.Request(
        url,
        headers={
            "User-Agent": "Mozilla/5.0 (compatible; luban-doc-crawler/1.0)",
            "Accept": "text/plain,text/markdown,text/html;q=0.9,*/*;q=0.8",
        },
    )
    with urllib.request.urlopen(req, timeout=timeout_sec) as resp:
        raw = resp.read()
        charset = resp.headers.get_content_charset() or "utf-8"
        return raw.decode(charset, errors="replace")


def extract_links_from_html(html_text: str, base_url: str, cfg: CrawlConfig) -> list[str]:
    result: list[str] = []
    seen: set[str] = set()
    for m in HTML_HREF_RE.finditer(html_text):
        href = m.group(1).strip()
        if not href or href.startswith("#"):
            continue
        absolute = urllib.parse.urljoin(base_url, href)
        parsed = urllib.parse.urlparse(absolute)
        if parsed.scheme not in {"http", "https"}:
            continue
        canonical = parsed._replace(fragment="").geturl()
        if not is_in_scope(canonical, cfg):
            continue
        if should_skip_url(canonical):
            continue
        if canonical not in seen:
            seen.add(canonical)
            result.append(canonical)
    return result


def html_to_markdown_like(html_text: str, source_url: str) -> str:
    title = ""
    tm = HTML_TITLE_RE.search(html_text)
    if tm:
        title = html.unescape(tm.group(1)).strip()

    parser = _SimpleHtmlTextExtractor()
    parser.feed(html_text)
    body = parser.get_text()

    if title:
        return f"# {title}\n\n{body}" if body else f"# {title}"
    if body:
        return body
    return f"(无法提取正文)\n\n原始地址: {source_url}"


def extract_links(markdown_text: str, base_url: str, cfg: CrawlConfig) -> list[str]:
    result: list[str] = []
    seen: set[str] = set()

    for match in MARKDOWN_LINK_RE.finditer(markdown_text):
        href = match.group(1).strip()
        if not href:
            continue
        if href.startswith("#"):
            continue

        absolute = urllib.parse.urljoin(base_url, href)
        parsed = urllib.parse.urlparse(absolute)
        if parsed.scheme not in {"http", "https"}:
            continue

        canonical = parsed._replace(fragment="").geturl()
        if not is_in_scope(canonical, cfg):
            continue
        if should_skip_url(canonical):
            continue

        if canonical not in seen:
            seen.add(canonical)
            result.append(canonical)
    return result


def url_to_output_path(url: str, cfg: CrawlConfig) -> pathlib.Path:
    parsed = urllib.parse.urlparse(url)
    rel = parsed.path

    # 去掉 scope 前缀后转路径。
    if rel.startswith(cfg.scope_prefix):
        rel = rel[len(cfg.scope_prefix) :]

    rel = rel.strip("/")
    if not rel:
        rel_path = pathlib.Path("index.md")
    else:
        p = pathlib.Path(rel)
        if p.suffix.lower() == ".md":
            rel_path = p
        elif p.suffix:
            rel_path = p.with_suffix(".md")
        else:
            if cfg.index_filenames:
                rel_path = p / "index.md"
            else:
                rel_path = p.with_suffix(".md")

    return cfg.out_dir / rel_path


def render_markdown(source_url: str, body: str) -> str:
    text = body.strip()
    if text.startswith("```"):
        # 某些场景下返回 fenced block，保留即可
        pass
    return f"> Source: {source_url}\n\n{text}\n"


def crawl(seed_urls: list[str], cfg: CrawlConfig) -> int:
    queue: collections.deque[str] = collections.deque(seed_urls)
    visited: set[str] = set()
    saved_count = 0

    cfg.out_dir.mkdir(parents=True, exist_ok=True)

    while queue and saved_count < cfg.max_pages:
        current = queue.popleft()
        if current in visited:
            continue
        visited.add(current)

        if not is_in_scope(current, cfg):
            continue
        if should_skip_url(current):
            continue

        out_file = url_to_output_path(current, cfg)
        if out_file.exists() and not cfg.overwrite:
            print(f"[skip] 已存在: {out_file}")
            continue

        converted = ""
        links: list[str] = []

        try:
            converted = http_get_text(jina_url(current), cfg.timeout_sec)
            links = extract_links(converted, current, cfg)
        except Exception as first_err:  # noqa: BLE001
            print(f"[warn] jina 转换失败，回退直连: {current} ({first_err})")
            try:
                html_text = http_get_text(current, cfg.timeout_sec)
            except urllib.error.HTTPError as e:
                print(f"[error] HTTP {e.code} {current}")
                continue
            except urllib.error.URLError as e:
                print(f"[error] 网络错误 {current}: {e}")
                continue
            except Exception as e:  # noqa: BLE001
                print(f"[error] 未知异常 {current}: {e}")
                continue

            converted = html_to_markdown_like(html_text, current)
            links = extract_links_from_html(html_text, current, cfg)

        for link in links:
            if link not in visited:
                queue.append(link)

        out_file.parent.mkdir(parents=True, exist_ok=True)
        out_file.write_text(render_markdown(current, converted), encoding="utf-8")

        saved_count += 1
        print(f"[ok] ({saved_count}) {current} -> {out_file}")

        if cfg.delay_sec > 0:
            time.sleep(cfg.delay_sec)

    return saved_count


def main() -> int:
    parser = build_parser()
    args = parser.parse_args()

    seeds = []
    seeds.extend(args.seed)
    seeds.extend(read_seed_file(args.seed_file))
    seeds.extend(read_sources_from_markdown(args.from_existing_sources))

    seed_urls = normalize_seed_urls(seeds)
    if not seed_urls:
        parser.error("没有可用种子 URL。请传入 --seed，或使用 --seed-file / --from-existing-sources")

    cfg = CrawlConfig(
        out_dir=pathlib.Path(args.out_dir),
        domain=args.domain,
        scope_prefix=args.scope_prefix,
        max_pages=args.max_pages,
        delay_sec=args.delay_sec,
        overwrite=args.overwrite,
        timeout_sec=args.timeout_sec,
        index_filenames=args.index_filenames,
    )

    print("=== 配置 ===")
    print(f"domain      : {cfg.domain}")
    print(f"scope_prefix: {cfg.scope_prefix}")
    print(f"out_dir     : {cfg.out_dir}")
    print(f"max_pages   : {cfg.max_pages}")
    print(f"seeds       : {len(seed_urls)}")
    print(f"index_files : {cfg.index_filenames}")

    count = crawl(seed_urls, cfg)
    print(f"完成，共保存 {count} 个页面。")
    return 0


if __name__ == "__main__":
    sys.exit(main())
