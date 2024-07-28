// Decompiled with JetBrains decompiler
// Type: Nest.CatAliasesRecord
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class CatAliasesRecord : ICatRecord
  {
    [DataMember(Name = "alias")]
    public string Alias { get; set; }

    [DataMember(Name = "filter")]
    public string Filter { get; set; }

    [DataMember(Name = "index")]
    public string Index { get; set; }

    [DataMember(Name = "indexRouting")]
    public string IndexRouting { get; set; }

    [DataMember(Name = "searchRouting")]
    public string SearchRouting { get; set; }
  }
}
