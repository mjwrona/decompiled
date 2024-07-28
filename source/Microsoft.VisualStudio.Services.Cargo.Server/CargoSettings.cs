// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.CargoSettings
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using System;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server
{
  [ExcludeFromCodeCoverage]
  public static class CargoSettings
  {
    public const string EnabledFeatureFlagName = "Packaging.Cargo.Enabled";
    public static readonly IOrgLevelPackagingSettingDefinition<bool> EnabledFeatureFlag = (IOrgLevelPackagingSettingDefinition<bool>) new FeatureFlagPackagingSettingDefinition("Packaging.Cargo.Enabled", true);
    public const string ReadOnlyFeatureName = "Packaging.Cargo.ReadOnly";
    public static readonly IOrgLevelPackagingSettingDefinition<bool> ReadOnly = (IOrgLevelPackagingSettingDefinition<bool>) new FeatureFlagPackagingSettingDefinition("Packaging.Cargo.ReadOnly");
    public const string ChangeProcessingBypassFeatureName = "Packaging.Cargo.DisasterRecovery.ChangeProcessingBypass";
    public static readonly IOrgLevelPackagingSettingDefinition<bool> ChangeProcessingBypass = (IOrgLevelPackagingSettingDefinition<bool>) new FeatureFlagPackagingSettingDefinition("Packaging.Cargo.DisasterRecovery.ChangeProcessingBypass");
    public static readonly IOrgLevelPackagingSettingDefinition<bool> AllowNonAsciiNames = (IOrgLevelPackagingSettingDefinition<bool>) new FeatureFlagPackagingSettingDefinition("Packaging.Cargo.AllowNonAsciiCrateNames");
    public static readonly RegistryFrotocolLevelPackagingSettingDefinition<int> MaxIngestionManifestLengthSetting = new RegistryFrotocolLevelPackagingSettingDefinition<int>((Func<IProtocol, RegistryQuery>) (protocol => (RegistryQuery) ("/Configuration/Packaging/" + protocol.CorrectlyCasedName + "/Ingestion/MaxManifestSize")), (Func<IProtocol, FeedIdentity, RegistryQuery>) ((protocol, feed) => (RegistryQuery) string.Format("/Configuration/Packaging/{0}/Ingestion/MaxManifestSize/{1}", (object) protocol.CorrectlyCasedName, (object) feed.Id)), 384000);
    public static readonly FeatureFlagPackagingSettingDefinition EnforceConsistentIdentity = new FeatureFlagPackagingSettingDefinition("Packaging.Cargo.EnforceConsistentIdentity");
    public static readonly RegistryWildcardListOrgLevelPackagingSettingDefinition<string> AdditionalDashMappings = new RegistryWildcardListOrgLevelPackagingSettingDefinition<string>((RegistryQuery) "/Configuration/Packaging/Cargo/DashUnderscoreFixup/*");
  }
}
