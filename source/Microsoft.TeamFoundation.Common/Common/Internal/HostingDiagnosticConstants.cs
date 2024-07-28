// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.HostingDiagnosticConstants
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class HostingDiagnosticConstants
  {
    public const string MdsMonitoringAgentFeatureName = "VisualStudio.Diagnostics.MdsMonitoringAgent";
    public const string AllIdentifiersFromBinariesFeatureName = "VisualStudio.Diagnostics.AllIdentifiersFromBinaries";
    public const string DiagnosticRegistryPath = "/Diagnostics/Hosting/";
    public const string ConfigurationChangePollInterval = "ConfigurationChangePollInterval";
    public const string OverallQuotaInMB = "OverallQuotaInMB";
    public const string TfsDiagnosticEnabled = "TfsDiagnosticEnabled";
    public const string DiagnosticLogTransferInterval = "DiagnosticLogTransferInterval";
    public const string DiagnosticLogFilterLevel = "DiagnosticLogFilterLevel";
    public const string TraceLogTransferInterval = "TraceLogTransferInterval";
    public const string TraceLogFilterLevel = "TraceLogFilterLevel";
    public const string DirectoriesTransferInterval = "DirectoriesTransferInterval";
    public const string FullCrashDumps = "FullCrashDumps";
    public const string PerformanceCounterTransferInterval = "PerformanceCounterTransferInterval";
    public const string PerformanceCounterSampleRate = "PerformanceCounterSampleRate";
    public const string PerformanceCounterDefaultCountersEnabled = "PerformanceCounterDefaultCountersEnabled";
    public const string PerformanceCounterPath = "/Diagnostics/Hosting/Performance/";
    public const string Counter = "Counter";
    public const string CounterSampleRate = "SampleRate";
    public const string EventLogTransferInterval = "EventLogTransferInterval";
    public const string EventLogFilterLevel = "EventLogFilterLevel";
    public const string EventLogPath = "/Diagnostics/Hosting/EventLog/";
    public const string EventLogDefaultDataSourcesEnabled = "EventLogDefaultDataSourcesEnabled";
    public const string EventLogVsoDataSourcesEnabled = "EventLogVsoDataSourcesEnabled";
    public const string DataSource = "DataSource";
    public const string FilterLevel = "FilterLevel";
    public const string MdsUseDevStorageRegistryPath = "/Diagnostics/Hosting/MdsDiagnostics/UseDevStorage";
    public const string MdsStorageAccountNameRegistryPath = "/Diagnostics/Hosting/MdsDiagnostics/StorageAccountName";
    public const string MdsStorageKeyEncryptedRegistryPath = "/Diagnostics/Hosting/MdsDiagnostics/StorageKeyEncrypted";
    public const string MdsStorageKeyEncryptThumbprintRegistryPath = "/Diagnostics/Hosting/MdsDiagnostics/StorageKeyEncryptThumbprint";
    public const string MdsTenantNameRegistryPath = "/Diagnostics/Hosting/MdsDiagnostics/TenantName";
    public const string MdsRoleEnvironmentNameRegistryPath = "/Diagnostics/Hosting/MdsDiagnostics/RoleEnvironment";
    public const string MdsAzureSecurityPackFlighting = "/Diagnostics/Hosting/MdsDiagnostics/ASPFlighting";
    public const string MdsAzureSecurityPackDisabledFeatures = "/Diagnostics/Hosting/MdsDiagnostics/ASPDisabledFeatures";
    public const string MdsConfigVersionRegistryPathRoot = "/Diagnostics/Hosting/MdsDiagnostics/MdsConfigVersionFull";
  }
}
