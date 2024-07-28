// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.IAzureBlobGeoRedundancyManagementService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  [DefaultServiceImplementation(typeof (AzureBlobGeoRedundancyManagementService))]
  public interface IAzureBlobGeoRedundancyManagementService : IVssFrameworkService
  {
    string GeoRedundancyRemoteBlobProvider { get; }

    IEnumerable<GeoRedundantStorageAccountSettings> GetGeoRedundantStorageAccounts(
      IVssRequestContext requestContext);

    Task<bool> IsQueueEmptyAsync(IVssRequestContext requestContext, CloudQueue queue);

    Task<bool> AreGeoReplicationQueuesEmptyAsync(
      IVssRequestContext requestContext,
      CloudStorageAccount account);

    Task DelayUntilQueuesDrainAsync(
      IVssRequestContext requestContext,
      IEnumerable<GeoRedundantStorageAccountSettings> accounts,
      TimeSpan pollingFrequency,
      TimeSpan? minimumWaitTime,
      ITFLogger logger);

    Guid StartCatchupJob(IVssRequestContext requestContext, int jobIndex);

    Guid StartSeedingJob(
      IVssRequestContext requestContext,
      GeoRedundantStorageAccountSettings account,
      bool continueOnError = true,
      bool overwriteExisting = false);
  }
}
