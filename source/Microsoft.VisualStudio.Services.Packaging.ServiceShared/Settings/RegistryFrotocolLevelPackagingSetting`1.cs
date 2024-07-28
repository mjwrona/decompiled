// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings.RegistryFrotocolLevelPackagingSetting`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings
{
  public class RegistryFrotocolLevelPackagingSetting<TValue> : IFrotocolLevelPackagingSetting<TValue>
  {
    private readonly RegistryFrotocolLevelPackagingSettingDefinition<TValue> definition;
    private readonly IRegistryService registryService;

    public RegistryFrotocolLevelPackagingSetting(
      RegistryFrotocolLevelPackagingSettingDefinition<TValue> definition,
      IRegistryService registryService)
    {
      this.definition = definition;
      this.registryService = registryService;
    }

    public TValue Get(IFeedRequest feedRequest)
    {
      string str1 = this.registryService.GetValue<string>(this.definition.OrgProtocolAndFeedRegistryQueryGenerator(feedRequest.Protocol, feedRequest.Feed.GetIdentity()), (string) null, false);
      if (str1 != null)
        return RegistryUtility.FromString<TValue>(str1);
      string str2 = this.registryService.GetValue<string>(this.definition.DeploymentOrOrgAndProtocolRegistryQueryGenerator(feedRequest.Protocol), (string) null);
      if (str2 != null)
        return RegistryUtility.FromString<TValue>(str2);
      RegistryQuery? levelRegistryPath = this.definition.OrgLevelRegistryPath;
      return levelRegistryPath.HasValue ? this.registryService.GetValue<TValue>(levelRegistryPath.Value, this.definition.DefaultValue) : this.definition.DefaultValue;
    }
  }
}
