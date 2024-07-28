// Decompiled with JetBrains decompiler
// Type: Nest.PrivilegesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class PrivilegesDescriptor : 
    IsADictionaryDescriptorBase<PrivilegesDescriptor, IPrivileges, string, IPrivilegesActions>
  {
    public PrivilegesDescriptor()
      : base((IPrivileges) new Privileges())
    {
    }

    public PrivilegesDescriptor Privilege(string privilegesName, IPrivilegesActions actions) => this.Assign(privilegesName, actions);

    public PrivilegesDescriptor Privilege(
      string privilegesName,
      Func<PrivilegesActionsDescriptor, IPrivilegesActions> selector)
    {
      return this.Assign(privilegesName, selector != null ? selector(new PrivilegesActionsDescriptor()) : (IPrivilegesActions) null);
    }
  }
}
