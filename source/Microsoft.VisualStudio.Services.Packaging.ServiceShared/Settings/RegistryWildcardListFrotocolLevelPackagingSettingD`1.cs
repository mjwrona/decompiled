// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings.RegistryWildcardListFrotocolLevelPackagingSettingDefinition`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings
{
  public class RegistryWildcardListFrotocolLevelPackagingSettingDefinition<TValue> : 
    IFrotocolLevelPackagingSettingDefinition<IReadOnlyCollection<TValue>>
  {
    public Func<IProtocol, RegistryQuery> DeploymentOrOrgAndProtocolRegistryQueryGenerator { get; }

    public Func<IProtocol, FeedIdentity, RegistryQuery> OrgProtocolAndFeedRegistryQueryGenerator { get; }

    public RegistryWildcardListFrotocolLevelPackagingSettingDefinition(
      Func<IProtocol, RegistryQuery> deploymentOrOrgAndProtocolRegistryQueryGenerator,
      Func<IProtocol, FeedIdentity, RegistryQuery> orgProtocolAndFeedRegistryQueryGenerator)
    {
      this.DeploymentOrOrgAndProtocolRegistryQueryGenerator = deploymentOrOrgAndProtocolRegistryQueryGenerator;
      this.OrgProtocolAndFeedRegistryQueryGenerator = orgProtocolAndFeedRegistryQueryGenerator;
    }

    public IFrotocolLevelPackagingSetting<IReadOnlyCollection<TValue>> Bootstrap(
      IVssRequestContext requestContext)
    {
      return (IFrotocolLevelPackagingSetting<IReadOnlyCollection<TValue>>) new RegistryWildcardListFrotocolLevelPackagingSetting<TValue>(this, (IRegistryService) requestContext.GetRegistryFacade(), (IDeploymentLevelRegistryService) new DeploymentLevelRegistryServiceFacade(requestContext));
    }
  }
}
