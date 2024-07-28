// Decompiled with JetBrains decompiler
// Type: Nest.BulkAliasRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.IndicesApi;
using System.Collections.Generic;

namespace Nest
{
  public class BulkAliasRequest : 
    PlainRequestBase<BulkAliasRequestParameters>,
    IBulkAliasRequest,
    IRequest<BulkAliasRequestParameters>,
    IRequest
  {
    public IList<IAliasAction> Actions { get; set; }

    protected IBulkAliasRequest Self => (IBulkAliasRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesBulkAlias;

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
  }
}
