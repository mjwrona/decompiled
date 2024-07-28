// Decompiled with JetBrains decompiler
// Type: Nest.SimpleQueryStringFlags
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  [Flags]
  public enum SimpleQueryStringFlags
  {
    [EnumMember(Value = "NONE")] None = 1,
    [EnumMember(Value = "AND")] And = 2,
    [EnumMember(Value = "OR")] Or = 4,
    [EnumMember(Value = "NOT")] Not = 8,
    [EnumMember(Value = "PREFIX")] Prefix = 16, // 0x00000010
    [EnumMember(Value = "PHRASE")] Phrase = 32, // 0x00000020
    [EnumMember(Value = "PRECEDENCE")] Precedence = 64, // 0x00000040
    [EnumMember(Value = "ESCAPE")] Escape = 128, // 0x00000080
    [EnumMember(Value = "WHITESPACE")] Whitespace = 256, // 0x00000100
    [EnumMember(Value = "FUZZY")] Fuzzy = 512, // 0x00000200
    [EnumMember(Value = "NEAR")] Near = 1024, // 0x00000400
    [EnumMember(Value = "SLOP")] Slop = 2048, // 0x00000800
    [EnumMember(Value = "ALL")] All = 4096, // 0x00001000
  }
}
