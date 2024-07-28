// Decompiled with JetBrains decompiler
// Type: Nest.InvalidateApiKeyRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SecurityApi;

namespace Nest
{
  public class InvalidateApiKeyRequest : 
    PlainRequestBase<InvalidateApiKeyRequestParameters>,
    IInvalidateApiKeyRequest,
    IRequest<InvalidateApiKeyRequestParameters>,
    IRequest
  {
    protected IInvalidateApiKeyRequest Self => (IInvalidateApiKeyRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityInvalidateApiKey;

    public string Id { get; set; }

    public string Name { get; set; }

    public string RealmName { get; set; }

    public string Username { get; set; }

    public bool? Owner { get; set; }
  }
}
