// Decompiled with JetBrains decompiler
// Type: Nest.ApplicationPrivilegesChecksDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class ApplicationPrivilegesChecksDescriptor : 
    DescriptorPromiseBase<ApplicationPrivilegesChecksDescriptor, List<IApplicationPrivilegesCheck>>
  {
    public ApplicationPrivilegesChecksDescriptor()
      : base(new List<IApplicationPrivilegesCheck>())
    {
    }

    public ApplicationPrivilegesChecksDescriptor Application(
      Func<ApplicationPrivilegesChecksDescriptor.ApplicationPrivilegesCheckDescriptor, IApplicationPrivilegesCheck> selector)
    {
      return this.Assign<Func<ApplicationPrivilegesChecksDescriptor.ApplicationPrivilegesCheckDescriptor, IApplicationPrivilegesCheck>>(selector, (Action<List<IApplicationPrivilegesCheck>, Func<ApplicationPrivilegesChecksDescriptor.ApplicationPrivilegesCheckDescriptor, IApplicationPrivilegesCheck>>) ((a, v) => a.Add(v.InvokeOrDefault<ApplicationPrivilegesChecksDescriptor.ApplicationPrivilegesCheckDescriptor, IApplicationPrivilegesCheck>(new ApplicationPrivilegesChecksDescriptor.ApplicationPrivilegesCheckDescriptor()))));
    }

    public class ApplicationPrivilegesCheckDescriptor : 
      DescriptorBase<ApplicationPrivilegesChecksDescriptor.ApplicationPrivilegesCheckDescriptor, IApplicationPrivilegesCheck>,
      IApplicationPrivilegesCheck
    {
      string IApplicationPrivilegesCheck.Name { get; set; }

      IEnumerable<string> IApplicationPrivilegesCheck.Privileges { get; set; }

      IEnumerable<string> IApplicationPrivilegesCheck.Resources { get; set; }

      public ApplicationPrivilegesChecksDescriptor.ApplicationPrivilegesCheckDescriptor Name(
        string name)
      {
        return this.Assign<string>(name, (Action<IApplicationPrivilegesCheck, string>) ((a, v) => a.Name = v));
      }

      public ApplicationPrivilegesChecksDescriptor.ApplicationPrivilegesCheckDescriptor Privileges(
        params string[] privileges)
      {
        return this.Assign<string[]>(privileges, (Action<IApplicationPrivilegesCheck, string[]>) ((a, v) => a.Privileges = (IEnumerable<string>) v));
      }

      public ApplicationPrivilegesChecksDescriptor.ApplicationPrivilegesCheckDescriptor Privileges(
        IEnumerable<string> privileges)
      {
        return this.Assign<IEnumerable<string>>(privileges, (Action<IApplicationPrivilegesCheck, IEnumerable<string>>) ((a, v) => a.Privileges = v));
      }

      public ApplicationPrivilegesChecksDescriptor.ApplicationPrivilegesCheckDescriptor Resources(
        params string[] resources)
      {
        return this.Assign<string[]>(resources, (Action<IApplicationPrivilegesCheck, string[]>) ((a, v) => a.Resources = (IEnumerable<string>) v));
      }

      public ApplicationPrivilegesChecksDescriptor.ApplicationPrivilegesCheckDescriptor Resources(
        IEnumerable<string> resources)
      {
        return this.Assign<IEnumerable<string>>(resources, (Action<IApplicationPrivilegesCheck, IEnumerable<string>>) ((a, v) => a.Resources = v));
      }
    }
  }
}
