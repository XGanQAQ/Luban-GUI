# 文档抓取脚本

## 目的

将 Luban 在线文档批量抓取到本地 Markdown，避免手工复制和 AI 上下文限制。

## 脚本

- `tools/crawl_luban_docs.py`：主脚本（Python，零第三方依赖）
- `tools/crawl_luban_docs.ps1`：Windows 便捷启动脚本

## 快速使用（PowerShell）

```powershell
./tools/crawl_luban_docs.ps1 -OutDir docs/lubanDocs_crawled -MaxPages 500 -Overwrite
```

## 直接使用 Python

```powershell
python tools/crawl_luban_docs.py `
  --seed https://www.datable.cn/docs/basic `
  --seed https://www.datable.cn/docs/beginner `
  --seed https://www.datable.cn/docs/manual `
  --from-existing-sources docs/lubanDocs `
  --out-dir docs/lubanDocs_crawled `
  --max-pages 500 `
  --overwrite
```

## 参数说明

- `--seed`：起始 URL，可重复传入多个
- `--seed-file`：每行一个 URL 的种子文件
- `--from-existing-sources`：从已有 Markdown 中提取 `> Source: ...` 作为种子
- `--domain`：限制抓取域名（默认 `www.datable.cn`）
- `--scope-prefix`：限制路径前缀（默认 `/docs`）
- `--max-pages`：最多抓取页数
- `--overwrite`：覆盖已存在文件

## 实现说明

- 使用 `https://r.jina.ai/<原始URL>` 把网页转换为 Markdown。
- 自动提取 Markdown 链接并递归抓取。
- 仅抓取指定域名和路径前缀，避免跑偏到站外。
- 输出路径会根据 URL 自动映射为本地 `.md` 文件。
