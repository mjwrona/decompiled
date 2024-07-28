// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings.RegistryWildcardListFrotocolLevelPackagingSetting`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings
{
  public class RegistryWildcardListFrotocolLevelPackagingSetting<TValue> : 
    IFrotocolLevelPackagingSetting<IReadOnlyCollection<TValue>>
  {
    private readonly RegistryWildcardListFrotocolLevelPackagingSettingDefinition<TValue> definition;
    private readonly IRegistryService registryService;
    private readonly IDeploymentLevelRegistryService deploymentRegistryService;

    public RegistryWildcardListFrotocolLevelPackagingSetting(
      RegistryWildcardListFrotocolLevelPackagingSettingDefinition<TValue> definition,
      IRegistryService registryService,
      IDeploymentLevelRegistryService deploymentRegistryService)
    {
      this.definition = definition;
      this.registryService = registryService;
      this.deploymentRegistryService = deploymentRegistryService;
    }

    public IReadOnlyCollection<TValue> Get(IFeedRequest feedRequest)
    {
      IEnumerable<RegistryItem> second1 = this.registryService.Read(this.definition.OrgProtocolAndFeedRegistryQueryGenerator(feedRequest.Protocol, feedRequest.Feed.GetIdentity()));
      RegistryQuery registryQuery = this.definition.DeploymentOrOrgAndProtocolRegistryQueryGenerator(feedRequest.Protocol);
      IEnumerable<RegistryItem> second2 = this.registryService.Read(registryQuery);
      return (IReadOnlyCollection<TValue>) this.deploymentRegistryService.Read(registryQuery).Concat<RegistryItem>(second2).Concat<RegistryItem>(second1).Select<RegistryItem, TValue>((Func<RegistryItem, TValue>) (x => RegistryUtility.FromString<TValue>(x.Value))).ToImmutableList<TValue>();
    }
  }
}
