// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.AzureBlobGeoRedundancyManagementService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  public class AzureBlobGeoRedundancyManagementService : 
    IAzureBlobGeoRedundancyManagementService,
    IVssFrameworkService
  {
    private const string c_area = "AzureBlobGeoRedundancy";
    private const string c_layer = "AzureBlobGeoRedundancyManagementService";

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public string GeoRedundancyRemoteBlobProvider
    {
      get
      {
        Type type = typeof (AzureBlobGeoRedundancyProvider);
        return type.FullName + "," + type.Assembly.GetName().Name;
      }
    }

    public IEnumerable<GeoRedundantStorageAccountSettings> GetGeoRedundantStorageAccounts(
      IVssRequestContext requestContext)
    {
      List<GeoRedundantStorageAccountSettings> redundantStorageAccounts = new List<GeoRedundantStorageAccountSettings>();
      PrefixAzureBlobGeoRedundancyExtension redundancyExtension1 = new PrefixAzureBlobGeoRedundancyExtension("ConfigurationSecrets", "FileServiceStorageAccount", "FileServiceOmegaStorageAccount");
      redundantStorageAccounts.AddRange(redundancyExtension1.GetGeoRedundantStorageAccounts(requestContext));
      using (IDisposableReadOnlyList<IAzureBlobGeoRedundancyExtension> extensions = requestContext.GetExtensions<IAzureBlobGeoRedundancyExtension>())
      {
        foreach (IAzureBlobGeoRedundancyExtension redundancyExtension2 in (IEnumerable<IAzureBlobGeoRedundancyExtension>) extensions)
        {
          try
          {
            redundantStorageAccounts.AddRange(redundancyExtension2.GetGeoRedundantStorageAccounts(requestContext));
          }
          catch (Exception ex)
          {
            requestContext.Trace(15309000, TraceLevel.Error, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyManagementService), string.Format("Error while retrieving geo-redundancy storage accounts. Extension: {0}, Exception: {1}", (object) redundancyExtension2.GetType(), (object) ex));
          }
        }
      }
      return (IEnumerable<GeoRedundantStorageAccountSettings>) redundantStorageAccounts;
    }

    public async Task<bool> IsQueueEmptyAsync(IVssRequestContext requestContext, CloudQueue queue)
    {
      if (await queue.ExistsAsync(requestContext.CancellationToken))
      {
        if (await queue.PeekMessageAsync(requestContext.CancellationToken) != null)
          return false;
        await queue.FetchAttributesAsync(requestContext.CancellationToken);
        int? approximateMessageCount = queue.ApproximateMessageCount;
        int num = 0;
        if (approximateMessageCount.GetValueOrDefault() > num & approximateMessageCount.HasValue)
          return false;
      }
      return true;
    }

    public virtual async Task<bool> AreGeoReplicationQueuesEmptyAsync(
      IVssRequestContext requestContext,
      CloudStorageAccount account)
    {
      CloudQueueClient queueClient = account.CreateCloudQueueClient();
      int numberOfQueues = requestContext.GetService<IAzureBlobGeoRedundancyService>().GetNumberOfQueues(requestContext);
      for (int i = 0; i < numberOfQueues; ++i)
      {
        requestContext.CancellationToken.ThrowIfCancellationRequested();
        CloudQueue queueReference = queueClient.GetQueueReference(AzureBlobGeoRedundancyUtils.GetQueueName(i));
        if (!await this.IsQueueEmptyAsync(requestContext, queueReference))
          return false;
      }
      return true;
    }

    public async Task DelayUntilQueuesDrainAsync(
      IVssRequestContext requestContext,
      IEnumerable<GeoRedundantStorageAccountSettings> accounts,
      TimeSpan pollingFrequency,
      TimeSpan? minimumWaitTime,
      ITFLogger logger)
    {
      IEnumerable<CloudStorageAccount> configuredAccounts = accounts.Select<GeoRedundantStorageAccountSettings, CloudStorageAccount>((Func<GeoRedundantStorageAccountSettings, CloudStorageAccount>) (a => CloudStorageAccount.Parse(AzureBlobGeoRedundancyUtils.GetStorageAccountConnectionString(requestContext, a.DrawerName, a.PrimaryLookupKey))));
      if (!minimumWaitTime.HasValue)
        minimumWaitTime = new TimeSpan?(requestContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(requestContext, (RegistryQuery) AzureBlobGeoRedundancyCatchupJob.s_maxVisibilityTimeoutRegistryPath, AzureBlobGeoRedundancyCatchupJob.s_defaultMaxVisibilityTimeout));
      logger.Info("Waiting for all storage account queues to drain...");
      DateTime? queuesDrainedTime = new DateTime?();
      bool validatedQueueDrained = false;
      while (!validatedQueueDrained)
      {
        requestContext.CancellationToken.ThrowIfCancellationRequested();
        if (queuesDrainedTime.HasValue)
        {
          if (await this.AreQueuesEmptyAsync(requestContext, configuredAccounts))
          {
            TimeSpan timeSpan1 = DateTime.Now - queuesDrainedTime.Value;
            TimeSpan? nullable = minimumWaitTime;
            if ((nullable.HasValue ? (timeSpan1 < nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
            {
              ITFLogger tfLogger = logger;
              object[] objArray = new object[1];
              nullable = minimumWaitTime;
              TimeSpan timeSpan2 = DateTime.Now - queuesDrainedTime.Value;
              objArray[0] = (object) (nullable.HasValue ? new TimeSpan?(nullable.GetValueOrDefault() - timeSpan2) : new TimeSpan?());
              tfLogger.Info("All queues are empty. Waiting for {0} to ensure there are no invisible messages...", objArray);
            }
            else
            {
              logger.Info("All queues are confirmed to be empty.");
              validatedQueueDrained = true;
              continue;
            }
          }
          else
          {
            logger.Info("Detected a non-empty queue! Will check again in {0}...", (object) pollingFrequency);
            queuesDrainedTime = new DateTime?();
          }
        }
        else if (await this.AreQueuesEmptyAsync(requestContext, configuredAccounts))
        {
          logger.Info("All queues are empty. Waiting for {0} to ensure there are no invisible messages...", (object) minimumWaitTime);
          queuesDrainedTime = new DateTime?(DateTime.Now);
        }
        else
          logger.Info("One or more queues are not empty. Will check again in {0}...", (object) pollingFrequency);
        await Task.Delay(pollingFrequency, requestContext.CancellationToken);
      }
      configuredAccounts = (IEnumerable<CloudStorageAccount>) null;
    }

    public Guid StartCatchupJob(IVssRequestContext requestContext, int jobIndex)
    {
      Guid catchupJobId = AzureBlobGeoRedundancyUtils.IndexToCatchupJobId(jobIndex);
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(catchupJobId, "Azure Blob Geo Redundancy Catchup Job", typeof (AzureBlobGeoRedundancyCatchupJob).FullName, (XmlNode) null);
      int num1 = 60;
      int num2 = catchupJobId.GetHashCode() % num1;
      foundationJobDefinition.Schedule.Add(new TeamFoundationJobSchedule()
      {
        ScheduledTime = DateTime.Parse("2018-10-24T00:00:00.0000000Z").AddSeconds((double) num2),
        Interval = num1
      });
      requestContext.GetService<ITeamFoundationJobService>().UpdateJobDefinitions(requestContext, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
      {
        foundationJobDefinition
      });
      return catchupJobId;
    }

    public Guid StartSeedingJob(
      IVssRequestContext requestContext,
      GeoRedundantStorageAccountSettings account,
      bool continueOnError = true,
      bool overwriteExisting = false)
    {
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new AzureBlobGeoRedundancySeedingJobData()
      {
        DrawerName = account.DrawerName,
        PrimaryLookupKey = account.PrimaryLookupKey,
        SecondaryLookupKey = account.SecondaryLookupKey,
        ContinueOnError = continueOnError,
        OverwriteExisting = overwriteExisting
      });
      return requestContext.GetService<ITeamFoundationJobService>().QueueOneTimeJob(requestContext, "Azure Blob Geo Redundancy Seeding Job - " + account.PrimaryLookupKey, typeof (AzureBlobGeoRedundancySeedingJob).FullName, xml, false);
    }

    private async Task<bool> AreQueuesEmptyAsync(
      IVssRequestContext requestContext,
      IEnumerable<CloudStorageAccount> accounts)
    {
      foreach (CloudStorageAccount account in accounts)
      {
        if (!await this.AreGeoReplicationQueuesEmptyAsync(requestContext, account))
          return false;
      }
      return true;
    }
  }
}
