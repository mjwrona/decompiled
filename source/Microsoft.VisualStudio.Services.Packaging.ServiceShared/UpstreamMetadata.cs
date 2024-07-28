// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamMetadata
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public static class UpstreamMetadata
  {
    public static readonly TimeSpanFrotocolLevelPackagingSettingDefinition MetadataValidityPeriodWithEntries = new TimeSpanFrotocolLevelPackagingSettingDefinition((IFrotocolLevelPackagingSettingDefinition<string>) new RegistryFrotocolLevelPackagingSettingDefinition<string>((Func<IProtocol, RegistryQuery>) (protocol => (RegistryQuery) ("/Configuration/Packaging/" + protocol.CorrectlyCasedName + "/Upstreams/MetadataValidityPeriodMinutes")), (Func<IProtocol, FeedIdentity, RegistryQuery>) ((protocol, feed) => (RegistryQuery) string.Format("/Configuration/Packaging/{0}/Upstreams/MetadataValidityPeriodMinutes/{1}", (object) protocol.CorrectlyCasedName, (object) feed.Id)), (string) null), TimeSpan.FromHours(24.0), new TimeUnit?(TimeUnit.Minutes));
    public static readonly TimeSpanFrotocolLevelPackagingSettingDefinition MetadataValidityPeriodWithoutEntries = new TimeSpanFrotocolLevelPackagingSettingDefinition((IFrotocolLevelPackagingSettingDefinition<string>) new RegistryFrotocolLevelPackagingSettingDefinition<string>((Func<IProtocol, RegistryQuery>) (protocol => (RegistryQuery) ("/Configuration/Packaging/" + protocol.CorrectlyCasedName + "/Upstreams/MetadataValidityPeriodNoEntriesMinutes")), (Func<IProtocol, FeedIdentity, RegistryQuery>) ((protocol, feed) => (RegistryQuery) string.Format("/Configuration/Packaging/{0}/Upstreams/MetadataValidityPeriodNoEntriesMinutes/{1}", (object) protocol.CorrectlyCasedName, (object) feed.Id)), (string) null), TimeSpan.FromHours(1.0), new TimeUnit?(TimeUnit.Minutes));
    public static readonly TimeSpan ShouldForceSaveUpstreamDataIfOlderThanSpan = TimeSpan.FromHours(12.0);
    public static readonly FeatureFlagPackagingSettingDefinition AddToRefreshListOnlyIfAnyVersionsPresent = new FeatureFlagPackagingSettingDefinition("Packaging.AddToRefreshListOnlyIfAnyVersionsPresent");
  }
}
