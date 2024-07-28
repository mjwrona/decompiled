// Decompiled with JetBrains decompiler
// Type: Nest.GrantApiKeyDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SecurityApi;
using System;

namespace Nest
{
  public class GrantApiKeyDescriptor : 
    RequestDescriptorBase<GrantApiKeyDescriptor, GrantApiKeyRequestParameters, IGrantApiKeyRequest>,
    IGrantApiKeyRequest,
    IRequest<GrantApiKeyRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityGrantApiKey;

    public GrantApiKeyDescriptor Refresh(Elasticsearch.Net.Refresh? refresh) => this.Qs(nameof (refresh), (object) refresh);

    string IGrantApiKeyRequest.AccessToken { get; set; }

    Nest.GrantType? IGrantApiKeyRequest.GrantType { get; set; } = new Nest.GrantType?(Nest.GrantType.AccessToken);

    string IGrantApiKeyRequest.Password { get; set; }

    string IGrantApiKeyRequest.Username { get; set; }

    IApiKey IGrantApiKeyRequest.ApiKey { get; set; }

    public GrantApiKeyDescriptor AccessToken(string accessToken) => this.Assign<string>(accessToken, (Action<IGrantApiKeyRequest, string>) ((a, v) => a.AccessToken = v));

    public GrantApiKeyDescriptor GrantType(Nest.GrantType? type) => this.Assign<Nest.GrantType?>(type, (Action<IGrantApiKeyRequest, Nest.GrantType?>) ((a, v) => a.GrantType = v));

    public GrantApiKeyDescriptor Password(string password) => this.Assign<string>(password, (Action<IGrantApiKeyRequest, string>) ((a, v) => a.Password = v));

    public GrantApiKeyDescriptor Username(string username) => this.Assign<string>(username, (Action<IGrantApiKeyRequest, string>) ((a, v) => a.Username = v));

    public GrantApiKeyDescriptor ApiKey(Func<ApiKeyDescriptor, IApiKey> selector) => this.Assign<Func<ApiKeyDescriptor, IApiKey>>(selector, (Action<IGrantApiKeyRequest, Func<ApiKeyDescriptor, IApiKey>>) ((a, v) => a.ApiKey = v != null ? v(new ApiKeyDescriptor()) : (IApiKey) null));
  }
}
