// Decompiled with JetBrains decompiler
// Type: Nest.SnowballLanguage
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [StringEnum]
  public enum SnowballLanguage
  {
    [EnumMember(Value = "Armenian")] Armenian,
    [EnumMember(Value = "Basque")] Basque,
    [EnumMember(Value = "Catalan")] Catalan,
    [EnumMember(Value = "Danish")] Danish,
    [EnumMember(Value = "Dutch")] Dutch,
    [EnumMember(Value = "English")] English,
    [EnumMember(Value = "Finnish")] Finnish,
    [EnumMember(Value = "French")] French,
    [EnumMember(Value = "German")] German,
    [EnumMember(Value = "German2")] German2,
    [EnumMember(Value = "Hungarian")] Hungarian,
    [EnumMember(Value = "Italian")] Italian,
    [EnumMember(Value = "Kp")] Kp,
    [EnumMember(Value = "Lovins")] Lovins,
    [EnumMember(Value = "Norwegian")] Norwegian,
    [EnumMember(Value = "Porter")] Porter,
    [EnumMember(Value = "Portuguese")] Portuguese,
    [EnumMember(Value = "Romanian")] Romanian,
    [EnumMember(Value = "Russian")] Russian,
    [EnumMember(Value = "Spanish")] Spanish,
    [EnumMember(Value = "Swedish")] Swedish,
    [EnumMember(Value = "Turkish")] Turkish,
  }
}
