// Decompiled with JetBrains decompiler
// Type: Nest.AppPrivilegesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class AppPrivilegesDescriptor : 
    IsADictionaryDescriptorBase<AppPrivilegesDescriptor, IAppPrivileges, string, IPrivileges>
  {
    public AppPrivilegesDescriptor()
      : base((IAppPrivileges) new AppPrivileges())
    {
    }

    public AppPrivilegesDescriptor Application(string applicationName, IPrivileges privileges) => this.Assign(applicationName, privileges);

    public AppPrivilegesDescriptor Application(
      string applicationName,
      Func<PrivilegesDescriptor, IPromise<IPrivileges>> selector)
    {
      return this.Assign(applicationName, selector != null ? selector(new PrivilegesDescriptor())?.Value : (IPrivileges) null);
    }
  }
}
