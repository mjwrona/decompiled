// Decompiled with JetBrains decompiler
// Type: Nest.CatIndicesRecord
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class CatIndicesRecord : ICatRecord
  {
    [DataMember(Name = "docs.count")]
    public string DocsCount { get; set; }

    [DataMember(Name = "docs.deleted")]
    public string DocsDeleted { get; set; }

    [DataMember(Name = "health")]
    public string Health { get; set; }

    [DataMember(Name = "index")]
    public string Index { get; set; }

    [DataMember(Name = "uuid")]
    public string UUID { get; set; }

    [DataMember(Name = "pri")]
    public string Primary { get; set; }

    [DataMember(Name = "pri.store.size")]
    public string PrimaryStoreSize { get; set; }

    [DataMember(Name = "rep")]
    public string Replica { get; set; }

    [DataMember(Name = "status")]
    public string Status { get; set; }

    [DataMember(Name = "store.size")]
    public string StoreSize { get; set; }

    [DataMember(Name = "tm")]
    public string TotalMemory { get; set; }
  }
}
