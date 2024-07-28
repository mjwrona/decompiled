// Decompiled with JetBrains decompiler
// Type: Nest.ApiKeyPrivilegesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class ApiKeyPrivilegesDescriptor : 
    DescriptorPromiseBase<ApiKeyPrivilegesDescriptor, List<IApiKeyPrivileges>>
  {
    public ApiKeyPrivilegesDescriptor()
      : base(new List<IApiKeyPrivileges>())
    {
    }

    public ApiKeyPrivilegesDescriptor Index(
      Func<ApiKeyPrivilegesDescriptor.ApiKeyPrivilegeDescriptor, IApiKeyPrivileges> selector)
    {
      return this.Assign<Func<ApiKeyPrivilegesDescriptor.ApiKeyPrivilegeDescriptor, IApiKeyPrivileges>>(selector, (Action<List<IApiKeyPrivileges>, Func<ApiKeyPrivilegesDescriptor.ApiKeyPrivilegeDescriptor, IApiKeyPrivileges>>) ((a, v) => a.Add(v.InvokeOrDefault<ApiKeyPrivilegesDescriptor.ApiKeyPrivilegeDescriptor, IApiKeyPrivileges>(new ApiKeyPrivilegesDescriptor.ApiKeyPrivilegeDescriptor()))));
    }

    public class ApiKeyPrivilegeDescriptor : 
      DescriptorBase<ApiKeyPrivilegesDescriptor.ApiKeyPrivilegeDescriptor, IApiKeyPrivileges>,
      IApiKeyPrivileges
    {
      IEnumerable<string> IApiKeyPrivileges.Names { get; set; }

      IEnumerable<string> IApiKeyPrivileges.Privileges { get; set; }

      public ApiKeyPrivilegesDescriptor.ApiKeyPrivilegeDescriptor Privileges(
        params string[] privileges)
      {
        return this.Assign<string[]>(privileges, (Action<IApiKeyPrivileges, string[]>) ((a, v) => a.Privileges = (IEnumerable<string>) v));
      }

      public ApiKeyPrivilegesDescriptor.ApiKeyPrivilegeDescriptor Privileges(
        IEnumerable<string> privileges)
      {
        return this.Assign<IEnumerable<string>>(privileges, (Action<IApiKeyPrivileges, IEnumerable<string>>) ((a, v) => a.Privileges = v));
      }

      public ApiKeyPrivilegesDescriptor.ApiKeyPrivilegeDescriptor Names(params string[] resources) => this.Assign<string[]>(resources, (Action<IApiKeyPrivileges, string[]>) ((a, v) => a.Names = (IEnumerable<string>) v));

      public ApiKeyPrivilegesDescriptor.ApiKeyPrivilegeDescriptor Names(
        IEnumerable<string> resources)
      {
        return this.Assign<IEnumerable<string>>(resources, (Action<IApiKeyPrivileges, IEnumerable<string>>) ((a, v) => a.Names = v));
      }
    }
  }
}
