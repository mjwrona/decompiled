// Decompiled with JetBrains decompiler
// Type: Nest.ApplicationPrivilegesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class ApplicationPrivilegesDescriptor : 
    DescriptorBase<ApplicationPrivilegesDescriptor, IApplicationPrivileges>,
    IApplicationPrivileges
  {
    string IApplicationPrivileges.Application { get; set; }

    IEnumerable<string> IApplicationPrivileges.Privileges { get; set; }

    IEnumerable<string> IApplicationPrivileges.Resources { get; set; }

    public ApplicationPrivilegesDescriptor Application(string application) => this.Assign<string>(application, (Action<IApplicationPrivileges, string>) ((a, v) => a.Application = v));

    public ApplicationPrivilegesDescriptor Privileges(params string[] privileges) => this.Assign<string[]>(privileges, (Action<IApplicationPrivileges, string[]>) ((a, v) => a.Privileges = (IEnumerable<string>) v));

    public ApplicationPrivilegesDescriptor Privileges(IEnumerable<string> privileges) => this.Assign<IEnumerable<string>>(privileges, (Action<IApplicationPrivileges, IEnumerable<string>>) ((a, v) => a.Privileges = v));

    public ApplicationPrivilegesDescriptor Resources(params string[] resources) => this.Assign<string[]>(resources, (Action<IApplicationPrivileges, string[]>) ((a, v) => a.Resources = (IEnumerable<string>) v));

    public ApplicationPrivilegesDescriptor Resources(IEnumerable<string> resources) => this.Assign<IEnumerable<string>>(resources, (Action<IApplicationPrivileges, IEnumerable<string>>) ((a, v) => a.Resources = v));
  }
}
