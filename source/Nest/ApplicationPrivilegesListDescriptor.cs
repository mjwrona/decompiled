// Decompiled with JetBrains decompiler
// Type: Nest.ApplicationPrivilegesListDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class ApplicationPrivilegesListDescriptor : 
    DescriptorPromiseBase<ApplicationPrivilegesListDescriptor, IList<IApplicationPrivileges>>
  {
    public ApplicationPrivilegesListDescriptor()
      : base((IList<IApplicationPrivileges>) new List<IApplicationPrivileges>())
    {
    }

    public ApplicationPrivilegesListDescriptor Add(
      Func<ApplicationPrivilegesDescriptor, IApplicationPrivileges> selector)
    {
      return this.Assign<Func<ApplicationPrivilegesDescriptor, IApplicationPrivileges>>(selector, (Action<IList<IApplicationPrivileges>, Func<ApplicationPrivilegesDescriptor, IApplicationPrivileges>>) ((a, v) => a.AddIfNotNull<IApplicationPrivileges>(v != null ? v(new ApplicationPrivilegesDescriptor()) : (IApplicationPrivileges) null)));
    }
  }
}
