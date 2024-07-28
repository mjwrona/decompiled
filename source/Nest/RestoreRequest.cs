// Decompiled with JetBrains decompiler
// Type: Nest.RestoreRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SnapshotApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class RestoreRequest : 
    PlainRequestBase<RestoreRequestParameters>,
    IRestoreRequest,
    IRequest<RestoreRequestParameters>,
    IRequest
  {
    public List<string> IgnoreIndexSettings { get; set; }

    public bool? IgnoreUnavailable { get; set; }

    public bool? IncludeAliases { get; set; }

    public bool? IncludeGlobalState { get; set; }

    public IUpdateIndexSettingsRequest IndexSettings { get; set; }

    public Indices Indices { get; set; }

    public bool? Partial { get; set; }

    public string RenamePattern { get; set; }

    public string RenameReplacement { get; set; }

    protected IRestoreRequest Self => (IRestoreRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotRestore;

    public RestoreRequest(Name repository, Name snapshot)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (repository), (IUrlParameter) repository).Required(nameof (snapshot), (IUrlParameter) snapshot)))
    {
    }

    [SerializationConstructor]
    protected RestoreRequest()
    {
    }

    [IgnoreDataMember]
    Name IRestoreRequest.RepositoryName => this.Self.RouteValues.Get<Name>("repository");

    [IgnoreDataMember]
    Name IRestoreRequest.Snapshot => this.Self.RouteValues.Get<Name>("snapshot");

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }

    public bool? WaitForCompletion
    {
      get => this.Q<bool?>("wait_for_completion");
      set => this.Q("wait_for_completion", (object) value);
    }
  }
}
