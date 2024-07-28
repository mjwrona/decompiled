// Decompiled with JetBrains decompiler
// Type: Nest.ApiKeyRoleDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class ApiKeyRoleDescriptor : DescriptorBase<ApiKeyRoleDescriptor, IApiKeyRole>, IApiKeyRole
  {
    IEnumerable<string> IApiKeyRole.Cluster { get; set; }

    IEnumerable<IApiKeyPrivileges> IApiKeyRole.Index { get; set; }

    public ApiKeyRoleDescriptor Cluster(params string[] cluster) => this.Assign<string[]>(cluster, (Action<IApiKeyRole, string[]>) ((a, v) => a.Cluster = (IEnumerable<string>) v));

    public ApiKeyRoleDescriptor Cluster(IEnumerable<string> cluster) => this.Assign<IEnumerable<string>>(cluster, (Action<IApiKeyRole, IEnumerable<string>>) ((a, v) => a.Cluster = v));

    public ApiKeyRoleDescriptor Indices(
      Func<ApiKeyPrivilegesDescriptor, IPromise<List<IApiKeyPrivileges>>> selector)
    {
      return this.Assign<Func<ApiKeyPrivilegesDescriptor, IPromise<List<IApiKeyPrivileges>>>>(selector, (Action<IApiKeyRole, Func<ApiKeyPrivilegesDescriptor, IPromise<List<IApiKeyPrivileges>>>>) ((a, v) => a.Index = v != null ? (IEnumerable<IApiKeyPrivileges>) v(new ApiKeyPrivilegesDescriptor())?.Value : (IEnumerable<IApiKeyPrivileges>) null));
    }
  }
}
