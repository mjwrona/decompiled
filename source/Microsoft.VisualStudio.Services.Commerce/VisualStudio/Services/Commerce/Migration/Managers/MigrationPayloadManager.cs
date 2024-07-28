// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Migration.Managers.MigrationPayloadManager
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts;
using Microsoft.VisualStudio.Services.Commerce.Migration.Extensions;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce.Migration.Managers
{
  internal static class MigrationPayloadManager
  {
    public static DataMigrationRequest BuildMigrationRequest(
      Microsoft.VisualStudio.Services.Commerce.AzureResourceAccount resourceAccount,
      IEnumerable<Microsoft.VisualStudio.Services.Commerce.SubscriptionResourceUsage> resourceUsages,
      Microsoft.VisualStudio.Services.Commerce.AzureSubscriptionInternal azureSubscription,
      Guid hostId)
    {
      DataMigrationRequest migrationRequest1 = new DataMigrationRequest();
      migrationRequest1.HostId = hostId;
      DataMigrationRequest migrationRequest2 = migrationRequest1;
      Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.AzureResourceAccount[] azureResourceAccountArray;
      if (resourceAccount == null)
        azureResourceAccountArray = (Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.AzureResourceAccount[]) null;
      else
        azureResourceAccountArray = new Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.AzureResourceAccount[1]
        {
          resourceAccount.ToMigrationResourceAccount()
        };
      migrationRequest2.AzureResourceAccounts = (IEnumerable<Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.AzureResourceAccount>) azureResourceAccountArray;
      migrationRequest1.AzureSubscription = azureSubscription.ToMigrationSubscriptionInternal();
      migrationRequest1.ResourceUsages = resourceUsages.ToResourceUsages();
      return migrationRequest1;
    }

    public static DualWriteSRURequest BuildDualWriteSRURequest(
      IEnumerable<Microsoft.VisualStudio.Services.Commerce.SubscriptionResourceUsage> resourceUsages,
      Guid hostId)
    {
      return new DualWriteSRURequest()
      {
        HostId = hostId,
        ResourceUsages = resourceUsages.ToResourceUsages()
      };
    }

    public static DualWriteARARequest BuildDualWriteARARequest(
      Microsoft.VisualStudio.Services.Commerce.AzureResourceAccount resourceAccount,
      Guid hostId,
      bool isLinkOperation)
    {
      DualWriteARARequest dualWriteAraRequest1 = new DualWriteARARequest();
      dualWriteAraRequest1.HostId = hostId;
      DualWriteARARequest dualWriteAraRequest2 = dualWriteAraRequest1;
      Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.AzureResourceAccount[] azureResourceAccountArray;
      if (resourceAccount == null)
        azureResourceAccountArray = (Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.AzureResourceAccount[]) null;
      else
        azureResourceAccountArray = new Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.AzureResourceAccount[1]
        {
          resourceAccount.ToMigrationResourceAccount()
        };
      dualWriteAraRequest2.AzureResourceAccounts = (IEnumerable<Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.AzureResourceAccount>) azureResourceAccountArray;
      dualWriteAraRequest1.IsLinkOperation = isLinkOperation;
      return dualWriteAraRequest1;
    }

    public static DualWriteSubscriptionRequest BuildDualWriteSubscriptionRequest(
      Microsoft.VisualStudio.Services.Commerce.AzureSubscriptionInternal azureSubscription,
      Guid subscriptionId)
    {
      return new DualWriteSubscriptionRequest()
      {
        SubscriptionId = subscriptionId,
        AzureSubscription = azureSubscription.ToMigrationSubscriptionInternal()
      };
    }

    public static StaleOrganizationRequest BuildStaleOrganizationRequest(
      Guid orgId,
      IEnumerable<int> meterIds)
    {
      return new StaleOrganizationRequest()
      {
        OrganizationId = orgId,
        CommerceMeterIds = meterIds
      };
    }
  }
}
