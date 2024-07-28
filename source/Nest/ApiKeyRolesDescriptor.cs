// Decompiled with JetBrains decompiler
// Type: Nest.ApiKeyRolesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ApiKeyRolesDescriptor : 
    IsADictionaryDescriptorBase<ApiKeyRolesDescriptor, ApiKeyRoles, string, IApiKeyRole>
  {
    public ApiKeyRolesDescriptor()
      : base(new ApiKeyRoles())
    {
    }

    public ApiKeyRolesDescriptor Role(string name, Func<ApiKeyRoleDescriptor, IApiKeyRole> selector) => this.Assign<Func<ApiKeyRoleDescriptor, IApiKeyRole>>(selector, (Action<ApiKeyRoles, Func<ApiKeyRoleDescriptor, IApiKeyRole>>) ((a, v) => a.Add(name, v.InvokeOrDefault<ApiKeyRoleDescriptor, IApiKeyRole>(new ApiKeyRoleDescriptor()))));
  }
}
