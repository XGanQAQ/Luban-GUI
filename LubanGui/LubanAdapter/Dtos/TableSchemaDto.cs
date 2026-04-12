using System.Collections.Generic;

namespace LubanGui.LubanAdapter.Dtos;

/// <summary>Luban 表格 Schema，对应 RawTable。</summary>
public record TableSchemaDto(
    string FullName,
    string Index,
    string ValueType,
    string Mode,
    string Comment,
    IReadOnlyList<string> InputFiles,
    string OutputFile,
    IReadOnlyList<string> Groups,
    bool ReadSchemaFromFile
);
