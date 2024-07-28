// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Configuration.IMonitoringAccount
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client.Configuration
{
  public interface IMonitoringAccount
  {
    string Name { get; }

    string DisplayName { get; set; }

    string Description { get; set; }

    string HomeStampHostName { get; }

    string ServiceTreeId { get; set; }

    string MetricsBillingSubscriptionId { get; set; }

    IEnumerable<IPermissionV2> Permissions { get; }

    DateTime LastUpdatedTimeUtc { get; }

    string LastUpdatedBy { get; }

    uint Version { get; }

    bool? ReplicateToLxCloud { get; }

    IEnumerable<string> MirrorMonitoringAccountList { get; }

    IDictionary<string, IVirtualSyncOptions> TargetAccountSyncOptions { get; }

    void AddPermission(IPermissionV2 permission);

    void RemovePermission(IPermissionV2 permission);

    void AddMirrorMonitoringAccount(string monitoringAccountName);

    void RemoveMirrorMonitoringAccount(string monitoringAccountName);

    void AddOrUpdateTargetAccounts(
      IDictionary<string, IVirtualSyncOptions> targetAccountsSyncOptions);

    void RemoveTargetAccounts(params string[] targetAccountsNamesToRemove);

    void TagAsLxAccount();

    void UnTagLxAccount();
  }
}
