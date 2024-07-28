// Decompiled with JetBrains decompiler
// Type: Nest.GrantApiKeyRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SecurityApi;

namespace Nest
{
  public class GrantApiKeyRequest : 
    PlainRequestBase<GrantApiKeyRequestParameters>,
    IGrantApiKeyRequest,
    IRequest<GrantApiKeyRequestParameters>,
    IRequest
  {
    protected IGrantApiKeyRequest Self => (IGrantApiKeyRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityGrantApiKey;

    public Elasticsearch.Net.Refresh? Refresh
    {
      get => this.Q<Elasticsearch.Net.Refresh?>("refresh");
      set => this.Q("refresh", (object) value);
    }

    public string AccessToken { get; set; }

    public Nest.GrantType? GrantType { get; set; }

    public string Password { get; set; }

    public string Username { get; set; }

    public IApiKey ApiKey { get; set; }
  }
}
