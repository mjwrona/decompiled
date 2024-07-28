// Decompiled with JetBrains decompiler
// Type: Nest.PhoneticLanguage
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [StringEnum]
  public enum PhoneticLanguage
  {
    [EnumMember(Value = "any")] Any,
    [EnumMember(Value = "comomon")] Comomon,
    [EnumMember(Value = "cyrillic")] Cyrillic,
    [EnumMember(Value = "english")] English,
    [EnumMember(Value = "french")] French,
    [EnumMember(Value = "german")] German,
    [EnumMember(Value = "hebrew")] Hebrew,
    [EnumMember(Value = "hungarian")] Hungarian,
    [EnumMember(Value = "polish")] Polish,
    [EnumMember(Value = "romanian")] Romanian,
    [EnumMember(Value = "russian")] Russian,
    [EnumMember(Value = "spanish")] Spanish,
  }
}
