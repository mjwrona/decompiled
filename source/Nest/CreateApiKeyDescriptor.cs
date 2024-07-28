// Decompiled with JetBrains decompiler
// Type: Nest.CreateApiKeyDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SecurityApi;
using System;

namespace Nest
{
  public class CreateApiKeyDescriptor : 
    RequestDescriptorBase<CreateApiKeyDescriptor, CreateApiKeyRequestParameters, ICreateApiKeyRequest>,
    ICreateApiKeyRequest,
    IRequest<CreateApiKeyRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityCreateApiKey;

    public CreateApiKeyDescriptor Refresh(Elasticsearch.Net.Refresh? refresh) => this.Qs(nameof (refresh), (object) refresh);

    Time ICreateApiKeyRequest.Expiration { get; set; }

    string ICreateApiKeyRequest.Name { get; set; }

    IApiKeyRoles ICreateApiKeyRequest.Roles { get; set; } = (IApiKeyRoles) new ApiKeyRoles();

    public CreateApiKeyDescriptor Expiration(Time expiration) => this.Assign<Time>(expiration, (Action<ICreateApiKeyRequest, Time>) ((a, v) => a.Expiration = v));

    public CreateApiKeyDescriptor Name(string name) => this.Assign<string>(name, (Action<ICreateApiKeyRequest, string>) ((a, v) => a.Name = v));

    public CreateApiKeyDescriptor Roles(
      Func<ApiKeyRolesDescriptor, IPromise<IApiKeyRoles>> selector)
    {
      return this.Assign<Func<ApiKeyRolesDescriptor, IPromise<IApiKeyRoles>>>(selector, (Action<ICreateApiKeyRequest, Func<ApiKeyRolesDescriptor, IPromise<IApiKeyRoles>>>) ((a, v) => a.Roles = v.InvokeOrDefault<ApiKeyRolesDescriptor, IPromise<IApiKeyRoles>>(new ApiKeyRolesDescriptor()).Value));
    }
  }
}
