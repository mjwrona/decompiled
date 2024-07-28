// Decompiled with JetBrains decompiler
// Type: Nest.GetApiKeyDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SecurityApi;

namespace Nest
{
  public class GetApiKeyDescriptor : 
    RequestDescriptorBase<GetApiKeyDescriptor, GetApiKeyRequestParameters, IGetApiKeyRequest>,
    IGetApiKeyRequest,
    IRequest<GetApiKeyRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityGetApiKey;

    public GetApiKeyDescriptor Id(string id) => this.Qs(nameof (id), (object) id);

    public GetApiKeyDescriptor Name(string name) => this.Qs(nameof (name), (object) name);

    public GetApiKeyDescriptor Owner(bool? owner = true) => this.Qs(nameof (owner), (object) owner);

    public GetApiKeyDescriptor RealmName(string realmname) => this.Qs("realm_name", (object) realmname);

    public GetApiKeyDescriptor Username(string username) => this.Qs(nameof (username), (object) username);
  }
}
