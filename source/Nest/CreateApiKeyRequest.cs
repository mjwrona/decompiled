// Decompiled with JetBrains decompiler
// Type: Nest.CreateApiKeyRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SecurityApi;

namespace Nest
{
  public class CreateApiKeyRequest : 
    PlainRequestBase<CreateApiKeyRequestParameters>,
    ICreateApiKeyRequest,
    IRequest<CreateApiKeyRequestParameters>,
    IRequest
  {
    protected ICreateApiKeyRequest Self => (ICreateApiKeyRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityCreateApiKey;

    public Elasticsearch.Net.Refresh? Refresh
    {
      get => this.Q<Elasticsearch.Net.Refresh?>("refresh");
      set => this.Q("refresh", (object) value);
    }

    public Time Expiration { get; set; }

    public string Name { get; set; }

    public IApiKeyRoles Roles { get; set; } = (IApiKeyRoles) new ApiKeyRoles();
  }
}
