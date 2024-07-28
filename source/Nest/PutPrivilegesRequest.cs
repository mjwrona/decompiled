// Decompiled with JetBrains decompiler
// Type: Nest.PutPrivilegesRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using System.IO;

namespace Nest
{
  public class PutPrivilegesRequest : 
    PlainRequestBase<PutPrivilegesRequestParameters>,
    IPutPrivilegesRequest,
    IRequest<PutPrivilegesRequestParameters>,
    IRequest,
    IProxyRequest
  {
    protected IPutPrivilegesRequest Self => (IPutPrivilegesRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityPutPrivileges;

    public Elasticsearch.Net.Refresh? Refresh
    {
      get => this.Q<Elasticsearch.Net.Refresh?>("refresh");
      set => this.Q("refresh", (object) value);
    }

    public IAppPrivileges Applications { get; set; }

    void IProxyRequest.WriteJson(
      IElasticsearchSerializer sourceSerializer,
      Stream stream,
      SerializationFormatting formatting)
    {
      sourceSerializer.Serialize<IAppPrivileges>(this.Self.Applications, stream, formatting);
    }
  }
}
