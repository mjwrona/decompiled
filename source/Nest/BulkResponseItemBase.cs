// Decompiled with JetBrains decompiler
// Type: Nest.BulkResponseItemBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [JsonFormatter(typeof (BulkResponseItemFormatter))]
  public abstract class BulkResponseItemBase
  {
    [DataMember(Name = "error")]
    public Error Error { get; internal set; }

    [DataMember(Name = "_id")]
    public string Id { get; internal set; }

    [DataMember(Name = "_index")]
    public string Index { get; internal set; }

    public abstract string Operation { get; }

    [DataMember(Name = "_primary_term")]
    public long PrimaryTerm { get; internal set; }

    [DataMember(Name = "get")]
    internal LazyDocument Get { get; set; }

    public Nest.GetResponse<TDocument> GetResponse<TDocument>() where TDocument : class => this.Get?.AsUsingRequestResponseSerializer<Nest.GetResponse<TDocument>>();

    [DataMember(Name = "result")]
    public string Result { get; internal set; }

    [DataMember(Name = "_seq_no")]
    public long SequenceNumber { get; internal set; }

    [DataMember(Name = "_shards")]
    public ShardStatistics Shards { get; internal set; }

    [DataMember(Name = "status")]
    public int Status { get; internal set; }

    [DataMember(Name = "_type")]
    public string Type { get; internal set; }

    [DataMember(Name = "_version")]
    public long Version { get; internal set; }

    public bool IsValid
    {
      get
      {
        if (this.Error != null || this.Type.IsNullOrEmpty())
          return false;
        switch (this.Operation.ToLowerInvariant())
        {
          case "delete":
            return this.Status == 200 || this.Status == 404;
          case "update":
          case "index":
          case "create":
            return this.Status == 200 || this.Status == 201;
          default:
            return false;
        }
      }
    }

    public override string ToString() => string.Format("{0} returned {1} _index: {2} _type: {3} _id: {4} _version: {5} error: {6}", (object) this.Operation, (object) this.Status, (object) this.Index, (object) this.Type, (object) this.Id, (object) this.Version, (object) this.Error);
  }
}
