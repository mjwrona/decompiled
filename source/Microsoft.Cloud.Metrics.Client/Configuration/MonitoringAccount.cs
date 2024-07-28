// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Configuration.MonitoringAccount
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Cloud.Metrics.Client.Configuration
{
  public sealed class MonitoringAccount : IMonitoringAccount
  {
    private readonly IList<IPermissionV2> permissions;
    private readonly IList<string> mirrorMonitoringAccountList;
    private readonly IDictionary<string, IVirtualSyncOptions> targetAccountSyncOptions;

    public MonitoringAccount(
      string name,
      string description,
      IEnumerable<IPermissionV2> permissionsV2,
      string serviceTreeId = null,
      string metricsBillingSubscriptionId = null)
      : this(name, (string) null, description, new DateTime(), permissionsV2, (string) null, 1U, new bool?(), (IEnumerable<string>) null, (IDictionary<string, IVirtualSyncOptions>) null, (string) null, serviceTreeId, metricsBillingSubscriptionId)
    {
    }

    [JsonConstructor]
    internal MonitoringAccount(
      string name,
      string displayName,
      string description,
      DateTime lastUpdatedTimeUtc,
      IEnumerable<IPermissionV2> permissionsV2,
      string lastUpdatedBy,
      uint version,
      bool? replicateToLxCloud,
      IEnumerable<string> mirrorMonitoringAccountList,
      IDictionary<string, IVirtualSyncOptions> targetAccountSyncOptions,
      string homeStampHostName,
      string serviceTreeId,
      string metricsBillingSubscriptionId)
    {
      if (permissionsV2 == null)
        throw new ArgumentNullException(nameof (permissionsV2));
      this.Name = name;
      this.DisplayName = displayName;
      this.Description = description;
      this.LastUpdatedTimeUtc = lastUpdatedTimeUtc;
      this.LastUpdatedBy = lastUpdatedBy;
      this.Version = version;
      this.permissions = (IList<IPermissionV2>) permissionsV2.ToList<IPermissionV2>();
      this.HomeStampHostName = homeStampHostName;
      this.ReplicateToLxCloud = replicateToLxCloud;
      this.mirrorMonitoringAccountList = (IList<string>) ((mirrorMonitoringAccountList != null ? mirrorMonitoringAccountList.ToList<string>() : (List<string>) null) ?? new List<string>());
      this.targetAccountSyncOptions = targetAccountSyncOptions == null || targetAccountSyncOptions.Count <= 0 ? (IDictionary<string, IVirtualSyncOptions>) new Dictionary<string, IVirtualSyncOptions>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IDictionary<string, IVirtualSyncOptions>) new Dictionary<string, IVirtualSyncOptions>(targetAccountSyncOptions, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.ServiceTreeId = serviceTreeId == null || Guid.TryParse(serviceTreeId, out Guid _) ? serviceTreeId : throw new ArgumentException("Expected GUID for the ServiceTreeId:" + serviceTreeId + " in monitoring account: " + name);
      if (metricsBillingSubscriptionId != null)
      {
        if (serviceTreeId == null)
          throw new ArgumentException("ServiceTreeId must be specified when MetricsBillingSubscriptionId is specified for monitoring account: " + name);
        if (!Guid.TryParse(metricsBillingSubscriptionId, out Guid _))
          throw new ArgumentException("Expected GUID for the MetricsBillingSubscriptionId:" + metricsBillingSubscriptionId + " in monitoring account: " + name);
      }
      this.MetricsBillingSubscriptionId = metricsBillingSubscriptionId;
    }

    public string Name { get; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public string HomeStampHostName { get; }

    public string ServiceTreeId { get; set; }

    public string MetricsBillingSubscriptionId { get; set; }

    [JsonProperty(PropertyName = "PermissionsV2")]
    public IEnumerable<IPermissionV2> Permissions => (IEnumerable<IPermissionV2>) this.permissions;

    public DateTime LastUpdatedTimeUtc { get; }

    public string LastUpdatedBy { get; }

    public uint Version { get; }

    public bool? ReplicateToLxCloud { get; private set; }

    public IEnumerable<string> MirrorMonitoringAccountList => (IEnumerable<string>) this.mirrorMonitoringAccountList;

    public IDictionary<string, IVirtualSyncOptions> TargetAccountSyncOptions => this.targetAccountSyncOptions;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    internal TimeSpan MaxMetricAge { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    internal bool? PreferNewPreaggregateOnMetricsStore { get; set; }

    public void AddPermission(IPermissionV2 permission) => this.permissions.Add(permission);

    public void RemovePermission(IPermissionV2 permission) => this.permissions.Remove(permission);

    public void AddMirrorMonitoringAccount(string monitoringAccountName) => this.mirrorMonitoringAccountList.Add(monitoringAccountName);

    public void RemoveMirrorMonitoringAccount(string monitoringAccountName) => this.mirrorMonitoringAccountList.Remove(monitoringAccountName);

    public void AddOrUpdateTargetAccounts(
      IDictionary<string, IVirtualSyncOptions> targetAccountsSyncOptionsToAddUpdate)
    {
      foreach (KeyValuePair<string, IVirtualSyncOptions> keyValuePair in (IEnumerable<KeyValuePair<string, IVirtualSyncOptions>>) targetAccountsSyncOptionsToAddUpdate)
      {
        if (string.Equals(keyValuePair.Key, this.Name, StringComparison.OrdinalIgnoreCase))
          throw new Exception("Method AddOrUpdateTargetAccounts only applies on source account and cannot add current account itself.");
        if (this.targetAccountSyncOptions.ContainsKey(keyValuePair.Key))
          this.targetAccountSyncOptions[keyValuePair.Key] = keyValuePair.Value;
        else
          this.targetAccountSyncOptions.Add(keyValuePair.Key, keyValuePair.Value);
      }
    }

    public void RemoveTargetAccounts(params string[] targetAccountsNamesToRemove)
    {
      if (this.targetAccountSyncOptions.Count == 0)
        throw new Exception("Method RemoveTargetAccounts only applies on source account.");
      foreach (string key in targetAccountsNamesToRemove)
        this.targetAccountSyncOptions.Remove(key);
    }

    public void TagAsLxAccount() => this.ReplicateToLxCloud = new bool?(true);

    public void UnTagLxAccount() => this.ReplicateToLxCloud = new bool?(false);
  }
}
