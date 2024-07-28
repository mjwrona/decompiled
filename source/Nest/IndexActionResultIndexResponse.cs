// Decompiled with JetBrains decompiler
// Type: Nest.IndexActionResultIndexResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class IndexActionResultIndexResponse
  {
    [DataMember(Name = "created")]
    public bool? Created { get; set; }

    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "index")]
    public IndexName Index { get; set; }

    [DataMember(Name = "result")]
    public Result Result { get; set; }

    [DataMember(Name = "version")]
    public int Version { get; set; }
  }
}
