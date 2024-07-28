// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings.RegistryWildcardListOrgLevelPackagingSettingDefinition`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings
{
  public class RegistryWildcardListOrgLevelPackagingSettingDefinition<TValue>
  {
    public RegistryWildcardListOrgLevelPackagingSettingDefinition(
      RegistryQuery deploymentOrOrgRegistryQuery)
    {
      this.DeploymentOrOrgRegistryQuery = deploymentOrOrgRegistryQuery;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public RegistryQuery DeploymentOrOrgRegistryQuery { get; }

    public IOrgLevelPackagingSetting<IReadOnlyCollection<TValue>> Bootstrap(
      IVssRequestContext requestContext)
    {
      return (IOrgLevelPackagingSetting<IReadOnlyCollection<TValue>>) new RegistryWildcardListOrgLevelPackagingSetting<TValue>(this, (IRegistryService) requestContext.GetRegistryFacade(), (IDeploymentLevelRegistryService) new DeploymentLevelRegistryServiceFacade(requestContext));
    }
  }
}
