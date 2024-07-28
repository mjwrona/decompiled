// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings.RegistryFrotocolLevelPackagingSettingDefinition`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings
{
  public class RegistryFrotocolLevelPackagingSettingDefinition<TValue> : 
    IFrotocolLevelPackagingSettingDefinition<TValue>
  {
    public Func<IProtocol, RegistryQuery> DeploymentOrOrgAndProtocolRegistryQueryGenerator { get; }

    public Func<IProtocol, FeedIdentity, RegistryQuery> OrgProtocolAndFeedRegistryQueryGenerator { get; }

    public TValue DefaultValue { get; }

    public RegistryQuery? OrgLevelRegistryPath { get; }

    public RegistryFrotocolLevelPackagingSettingDefinition(
      Func<IProtocol, RegistryQuery> deploymentOrOrgAndProtocolRegistryQueryGenerator,
      Func<IProtocol, FeedIdentity, RegistryQuery> orgProtocolAndFeedRegistryQueryGenerator,
      TValue defaultValue,
      RegistryQuery? orgLevelRegistryPath = null)
    {
      this.DeploymentOrOrgAndProtocolRegistryQueryGenerator = deploymentOrOrgAndProtocolRegistryQueryGenerator;
      this.OrgProtocolAndFeedRegistryQueryGenerator = orgProtocolAndFeedRegistryQueryGenerator;
      this.DefaultValue = defaultValue;
      this.OrgLevelRegistryPath = orgLevelRegistryPath;
    }

    public IFrotocolLevelPackagingSetting<TValue> Bootstrap(IVssRequestContext requestContext) => (IFrotocolLevelPackagingSetting<TValue>) new RegistryFrotocolLevelPackagingSetting<TValue>(this, (IRegistryService) requestContext.GetRegistryFacade());
  }
}
