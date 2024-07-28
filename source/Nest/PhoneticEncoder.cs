// Decompiled with JetBrains decompiler
// Type: Nest.PhoneticEncoder
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [StringEnum]
  public enum PhoneticEncoder
  {
    [EnumMember(Value = "metaphone")] Metaphone,
    [EnumMember(Value = "double_metaphone")] DoubleMetaphone,
    [EnumMember(Value = "soundex")] Soundex,
    [EnumMember(Value = "refined_soundex")] RefinedSoundex,
    [EnumMember(Value = "caverphone1")] Caverphone1,
    [EnumMember(Value = "caverphone2")] Caverphone2,
    [EnumMember(Value = "cologne")] Cologne,
    [EnumMember(Value = "nysiis")] Nysiis,
    [EnumMember(Value = "koelnerphonetik")] KoelnerPhonetik,
    [EnumMember(Value = "haasephonetik")] HaasePhonetik,
    [EnumMember(Value = "beider_morse")] Beidermorse,
    [EnumMember(Value = "daitch_mokotoff")] DaitchMokotoff,
  }
}
