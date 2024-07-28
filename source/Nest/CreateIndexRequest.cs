// Decompiled with JetBrains decompiler
// Type: Nest.CreateIndexRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class CreateIndexRequest : 
    PlainRequestBase<CreateIndexRequestParameters>,
    ICreateIndexRequest,
    IIndexState,
    IRequest<CreateIndexRequestParameters>,
    IRequest
  {
    private static readonly string[] ReadOnlySettings = new string[4]
    {
      "index.creation_date",
      "index.uuid",
      "index.version.created",
      "index.provided_name"
    };

    public CreateIndexRequest(IndexName index, IIndexState state)
      : this(index)
    {
      this.Settings = state.Settings;
      this.Mappings = state.Mappings;
      CreateIndexRequest.RemoveReadOnlySettings(this.Settings);
    }

    public IAliases Aliases { get; set; }

    public ITypeMapping Mappings { get; set; }

    public IIndexSettings Settings { get; set; }

    internal static void RemoveReadOnlySettings(IIndexSettings settings)
    {
      if (settings == null)
        return;
      foreach (string readOnlySetting in CreateIndexRequest.ReadOnlySettings)
      {
        if (settings.ContainsKey(readOnlySetting))
          ((IDictionary<string, object>) settings).Remove(readOnlySetting);
      }
    }

    protected ICreateIndexRequest Self => (ICreateIndexRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesCreate;

    public CreateIndexRequest(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected CreateIndexRequest()
    {
    }

    [IgnoreDataMember]
    IndexName ICreateIndexRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public bool? IncludeTypeName
    {
      get => this.Q<bool?>("include_type_name");
      set => this.Q("include_type_name", (object) value);
    }

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }

    public string WaitForActiveShards
    {
      get => this.Q<string>("wait_for_active_shards");
      set => this.Q("wait_for_active_shards", (object) value);
    }
  }
}
