// Decompiled with JetBrains decompiler
// Type: Nest.ApiKeyDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ApiKeyDescriptor : DescriptorBase<ApiKeyDescriptor, IApiKey>, IApiKey
  {
    Time IApiKey.Expiration { get; set; }

    string IApiKey.Name { get; set; }

    IApiKeyRoles IApiKey.Roles { get; set; }

    public ApiKeyDescriptor Expiration(Time expiration) => this.Assign<Time>(expiration, (Action<IApiKey, Time>) ((a, v) => a.Expiration = v));

    public ApiKeyDescriptor Name(string name) => this.Assign<string>(name, (Action<IApiKey, string>) ((a, v) => a.Name = v));

    public ApiKeyDescriptor Roles(
      Func<ApiKeyRolesDescriptor, IPromise<IApiKeyRoles>> selector)
    {
      return this.Assign<Func<ApiKeyRolesDescriptor, IPromise<IApiKeyRoles>>>(selector, (Action<IApiKey, Func<ApiKeyRolesDescriptor, IPromise<IApiKeyRoles>>>) ((a, v) => a.Roles = v.InvokeOrDefault<ApiKeyRolesDescriptor, IPromise<IApiKeyRoles>>(new ApiKeyRolesDescriptor()).Value));
    }
  }
}
