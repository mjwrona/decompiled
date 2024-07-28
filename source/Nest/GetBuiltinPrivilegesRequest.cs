// Decompiled with JetBrains decompiler
// Type: Nest.GetBuiltinPrivilegesRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SecurityApi;

namespace Nest
{
  public class GetBuiltinPrivilegesRequest : 
    PlainRequestBase<GetBuiltinPrivilegesRequestParameters>,
    IGetBuiltinPrivilegesRequest,
    IRequest<GetBuiltinPrivilegesRequestParameters>,
    IRequest
  {
    protected IGetBuiltinPrivilegesRequest Self => (IGetBuiltinPrivilegesRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityGetBuiltinPrivileges;
  }
}
