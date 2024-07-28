// Decompiled with JetBrains decompiler
// Type: Nest.PutPrivilegesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using System;
using System.IO;

namespace Nest
{
  public class PutPrivilegesDescriptor : 
    RequestDescriptorBase<PutPrivilegesDescriptor, PutPrivilegesRequestParameters, IPutPrivilegesRequest>,
    IPutPrivilegesRequest,
    IRequest<PutPrivilegesRequestParameters>,
    IRequest,
    IProxyRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityPutPrivileges;

    public PutPrivilegesDescriptor Refresh(Elasticsearch.Net.Refresh? refresh) => this.Qs(nameof (refresh), (object) refresh);

    IAppPrivileges IPutPrivilegesRequest.Applications { get; set; }

    void IProxyRequest.WriteJson(
      IElasticsearchSerializer sourceSerializer,
      Stream stream,
      SerializationFormatting formatting)
    {
      sourceSerializer.Serialize<IAppPrivileges>(this.Self.Applications, stream, formatting);
    }

    public PutPrivilegesDescriptor Applications(
      Func<AppPrivilegesDescriptor, IPromise<IAppPrivileges>> selector)
    {
      return this.Assign<Func<AppPrivilegesDescriptor, IPromise<IAppPrivileges>>>(selector, (Action<IPutPrivilegesRequest, Func<AppPrivilegesDescriptor, IPromise<IAppPrivileges>>>) ((a, v) => a.Applications = v != null ? v(new AppPrivilegesDescriptor())?.Value : (IAppPrivileges) null));
    }
  }
}
