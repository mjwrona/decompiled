// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Platform.Abstraction.RegistryServiceExtensions
// Assembly: Microsoft.TeamFoundation.CodeSense.Platform.Abstraction, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7627AC5C-7AFD-416A-A79B-D03A392C9E3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Platform.Abstraction.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.CodeSense.Platform.Abstraction
{
  public static class RegistryServiceExtensions
  {
    private const string KeepupDelayRegistryKey = "/Service/CodeSense/Settings/KeepupDelay";
    private const int KeepupDelayDefault = 5;
    private const string WorkItemAssociationDelayRegistryKey = "/Service/CodeSense/Settings/WorkItemAssociationDelay";
    private const int WorkItemAssociationDelayDefault = 600;
    private const string CatchupRuntimeRegistryKey = "/Service/CodeSense/Settings/CatchupRuntime";
    private static readonly TimeSpan CatchupRuntimeDefault = TimeSpan.FromMinutes(30.0);
    private const string FLICatchupRuntimeRegistryKey = "/Service/CodeSense/Settings/FLICatchupRuntime";
    private static readonly TimeSpan FLICatchupRuntimeDefault = TimeSpan.FromMinutes(30.0);
    private const string MopupRuntimeRegistryKey = "/Service/CodeSense/Settings/MopupRuntime";
    private static readonly TimeSpan MopupRuntimeDefault = TimeSpan.FromMinutes(30.0);
    private const string StorageGrowthLimitRegistryKey = "/Service/CodeSense/Settings/StorageGrowthLimitRegistryKey";
    private const long StorageGrowthLimitDefault = 2;
    private const string StorageGrowthGounterUpdateThresholdRegistryKey = "/Service/CodeSense/Settings/StorageGrowthCounterUpdateThreshold";
    private const int StorageGrowthCounterUpdateThresholdDefaultValue = 10485760;
    private const string CompressionFactorRegistryKey = "/Service/CodeSense/Settings/CompressionFactor";
    private const double CompressionFactorDefault = 0.17;
    private const string SliceBufferSizeRegistryKey = "/Service/CodeSense/Settings/SliceBufferSize";
    private const int SliceBufferSizeDefault = 200;
    private const string CatchupBatchSize = "/Service/CodeSense/Settings/CatchupBatchSize";
    private const int CatchupBatchSizeDefault = 1;
    private const string FLICatchupBatchSize = "/Service/CodeSense/Settings/FLICatchupBatchSize";
    private const int FLICatchupBatchSizeDefault = 1;
    private const string KeepupBatchSize = "/Service/CodeSense/Settings/KeepupBatchSize";
    private const int KeepupBatchSizeDefault = 1;
    private const string MopupBatchSize = "/Service/CodeSense/Settings/MopupBatchSize";
    private const int MopupBatchSizeDefault = 1;
    private const string AggregatorBatchSize = "/Service/CodeSense/Settings/AggregatorBatchSize";
    public const int AggregatorBatchSizeDefault = 100;
    private const string AggregatorRequeueThreshold = "/Service/CodeSense/Settings/AggregatorRequeueThreshold";
    private const int AggregatorRequeueThresholdDefault = 2;
    private const string ChangesetChangesBatchSize = "/Service/CodeSense/Settings/ChangesetChangesBatchSize";
    private const int ChangesetChangesBatchSizeDefault = 1000;
    private const string DownloadFilesBatchSize = "/Service/CodeSense/Settings/DownloadFilesBatchSize";
    private const int DownloadFilesBatchSizeDefault = 4;
    private const string CIMetricPublishLevel = "/Service/CodeSense/Settings/CIMetricPublishLevel";
    private const int CIMetricPublishLevelDefault = 1;
    private const string QueuedJobsDormancyThresholdDays = "/Service/CodeSense/Settings/QueuedJobsDormancyThreshold";
    private const int QueuedJobsDormancyThresholdDaysDefault = 10;
    private const string QueuedCleanupJobDelayIntervalInMinutes = "/Service/CodeSense/Settings/QueuedCleanupJobDelayIntervalInMinutes";
    private const int QueuedCleanupJobDelayIntervalInMinutesDefault = 0;
    private const string RetentionPeriodRegistryKey = "/Service/CodeSense/Settings/RetentionPeriod";
    private const int RetentionPeriodDefault = 24;
    private const string FileServiceRetentionPeriodInDays = "/Service/FileService/SqlRetentionPeriod";
    private const int FileServiceRetentionPeriodInDaysDefault = 1;
    private const string TransientErrorsRetryIntervalInSeconds = "/Service/CodeSense/Settings/TransientErrorRetryInterval";
    private const int TransientErrorsRetryIntervalInSecondsDefault = 10;
    private const string TransientErrorsRetryCount = "/Service/CodeSense/Settings/TransientErrorRetryCount";
    private const int TransientErrorsRetryCountDefault = 2;
    private const string QueueCatchupJobUntilCompletion = "/Service/CodeSense/Settings/QueueCatchupJobUntilCompletion";
    private const bool QueueCatchupJobUntilCompletionDefault = false;
    private const string QueueFrameworkCleanupJobFromCodeLens = "/Service/CodeSense/Settings/QueueFrameworkCleanupJobFromCodeLens";
    private const bool QueueFrameworkCleanupJobFromCodeLensDefault = false;
    private const string TimeElapsedThresholdForTraceWatchRegistryKey = "/Service/CodeSense/Settings/TimeElapsedThresholdForTraceWatch";
    private const double TimeElapsedThresholdForTraceWatchDefaultInMilliseconds = 5000.0;
    private const string LayerCakeSamplingProbabilityKey = "/Service/CodeSense/Settings/LayerCakeSamplingProbability";
    private const int LayerCakeSamplingProbabilityInverseDefault = 1000;
    private const string LayerCakeSamplingFeatureEnabledKey = "/Service/CodeSense/Settings/IsLayerCakeSamplingFeatureEnabled";
    private const bool LayerCakeSamplingFeatureEnabledDefault = false;
    private const string TimeElapsedThresholdForLayerCakeCIKey = "/Service/CodeSense/Settings/TimeElapsedThresholdForLayerCakeCIInMilliseconds";
    private const double TimeElapsedThresholdForLayerCakeCIInMillisecondsDefault = 0.0;
    private const string AggregateFileSizeThresholdInMBKey = "/Service/CodeSense/Settings/AggregateFileSizeThresholdInMB";
    private const int AggregateFileSizeThresholdInMBDefault = 300;
    private const string CleanupMonitorJobThresholdInHoursKey = "/Service/CodeSense/Settings/CleanupMonitorJobThresholdInHours";
    private const int CleanupMonitorJobThresholdDefault = 168;
    private const string ChangesetProcessingTimeThresholdInMinutes = "/Service/CodeSense/Settings/ChangesetProcessingTimeThreshold";
    private const int ChangesetProcessingTimeThresholdInMinutesDefault = 15;
    private const string ChangesetCreationThresholdInDays = "/Service/CodeSense/Settings/ChangesetCreationTimeThreshold";
    private const int ChangesetCreationThresholdInDaysDefault = 7;
    private const string ExcludedFileExtensionsRegistryKey = "/Service/CodeSense/Settings/ExcludedFileExtensions";
    private const string ExcludedFileExtensionsDefault = ".3gp,.7z,.accdb,.aiff,.ape,.avi,.bin,.blk,.blob,.bmp,.cache,.cer,.chm,.cspkg,.dbmdl,.dll,.dmg,.doc,.docx,.dotx,.edmx,.eot,.exe,.flac,.flv,.gif,.gz,.gzip,.ico,.ide,.iso,.jar,.jpeg,.jpg,.la,.lbr,.ldf,.lex,.lib,.lzip,.m3u,.m4a,.mdf,.mkv,.mov,.mp2,.mp3,.mp4,.mpeg,.mpg,.mpp,.msi,.msp,.msu,.nrg,.nupkg,.otf,.pac,.pdb,.pdf,.pfx,.png,.ppsx,.ppt,.pptx,.psd,.rar,.rm,.rtf,.snk,.so,.suo,.sys,.tgz,.ttf,.uha,.uml,.vhd,.wav,.wav,.winmd,.wmv,.woff,.xls,.xlsx,.xsn,.zip";
    private const string FLICatchupEndMarkRegistryKey = "/Service/CodeSense/Settings/PatchupEndMark";
    private const string FLICatchupStartMarkRegistryKey = "/Service/CodeSense/Settings/PatchupStartMark";

    public static double GetTimeElapsedThresholdForLayerCakeCIInMilliseconds(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<double>(requestContext, "/Service/CodeSense/Settings/TimeElapsedThresholdForLayerCakeCIInMilliseconds", true);
    }

    public static bool IsLayerCakeSamplingFeatureEnabled(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<bool>(requestContext, "/Service/CodeSense/Settings/IsLayerCakeSamplingFeatureEnabled", true, false);
    }

    public static int GetLayerCakeSamplingProbabilityInverse(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/LayerCakeSamplingProbability", true, 1000);
    }

    public static double GetTimeElapsedThresholdForTraceWatch(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      int tracepoint)
    {
      double orDefault = registryService.GetOrDefault<double>(requestContext, "/Service/CodeSense/Settings/TimeElapsedThresholdForTraceWatch", true, 5000.0);
      return registryService.GetOrDefault<double>(requestContext, "/Service/CodeSense/Settings/TimeElapsedThresholdForTraceWatch/" + tracepoint.ToString(), true, orDefault);
    }

    public static long StorageGrowthCounterUpdateThreshold(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return (long) registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/StorageGrowthCounterUpdateThreshold", true, 10485760);
    }

    public static double GetCompressionFactor(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<double>(requestContext, "/Service/CodeSense/Settings/CompressionFactor", true, 0.17);
    }

    public static long GetStorageGrowthLimit(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<long>(requestContext, "/Service/CodeSense/Settings/StorageGrowthLimitRegistryKey", true, 2L);
    }

    public static HashSet<string> GetExcludedFileExtensions(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return new HashSet<string>((IEnumerable<string>) registryService.GetOrDefault<string>(requestContext, "/Service/CodeSense/Settings/ExcludedFileExtensions", true, ".3gp,.7z,.accdb,.aiff,.ape,.avi,.bin,.blk,.blob,.bmp,.cache,.cer,.chm,.cspkg,.dbmdl,.dll,.dmg,.doc,.docx,.dotx,.edmx,.eot,.exe,.flac,.flv,.gif,.gz,.gzip,.ico,.ide,.iso,.jar,.jpeg,.jpg,.la,.lbr,.ldf,.lex,.lib,.lzip,.m3u,.m4a,.mdf,.mkv,.mov,.mp2,.mp3,.mp4,.mpeg,.mpg,.mpp,.msi,.msp,.msu,.nrg,.nupkg,.otf,.pac,.pdb,.pdf,.pfx,.png,.ppsx,.ppt,.pptx,.psd,.rar,.rm,.rtf,.snk,.so,.suo,.sys,.tgz,.ttf,.uha,.uml,.vhd,.wav,.wav,.winmd,.wmv,.woff,.xls,.xlsx,.xsn,.zip").Split(','));
    }

    public static int GetKeepupDelay(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/KeepupDelay", true, 5);
    }

    public static int GetFLICatchupEndMark(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/PatchupEndMark");
    }

    public static void SetFLICatchupEndMark(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      int changesetId)
    {
      registryService.SetValue<int>(requestContext, "/Service/CodeSense/Settings/PatchupEndMark", changesetId);
    }

    public static void SetFLICatchupStartMark(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      int changesetId)
    {
      registryService.SetValue<int>(requestContext, "/Service/CodeSense/Settings/PatchupStartMark", changesetId);
    }

    public static int GetFLICatchupStartMark(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/PatchupStartMark");
    }

    public static int GetWorkItemAssociationDelay(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/WorkItemAssociationDelay", true, 600);
    }

    public static TimeSpan GetCatchupRuntime(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<TimeSpan>(requestContext, "/Service/CodeSense/Settings/CatchupRuntime", true, requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? RegistryServiceExtensions.CatchupRuntimeDefault : TimeSpan.FromMinutes(15.0));
    }

    public static TimeSpan GetFLICatchupRuntime(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<TimeSpan>(requestContext, "/Service/CodeSense/Settings/FLICatchupRuntime", true, RegistryServiceExtensions.FLICatchupRuntimeDefault);
    }

    public static TimeSpan GetMopupRuntime(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<TimeSpan>(requestContext, "/Service/CodeSense/Settings/MopupRuntime", true, RegistryServiceExtensions.MopupRuntimeDefault);
    }

    public static int GetRetentionPeriod(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/RetentionPeriod", true, requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? 12 : 24);
    }

    public static int GetSliceBufferSize(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/SliceBufferSize", true, 200);
    }

    public static int GetCatchupBatchSize(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/CatchupBatchSize", true, 1);
    }

    public static int GetFLICatchupBatchSize(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/CatchupBatchSize", true, 1);
    }

    public static int GetKeepupBatchSize(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/KeepupBatchSize", true, 1);
    }

    public static int GetMopupBatchSize(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/MopupBatchSize", true, 1);
    }

    public static int GetAggregatorRequeueThreshold(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/AggregatorRequeueThreshold", true, 2);
    }

    public static int GetAggregatorBatchSize(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/AggregatorBatchSize", true, 100);
    }

    public static int GetChangesetChangesBatchSize(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/ChangesetChangesBatchSize", true, 1000);
    }

    public static int GetDownloadFilesBatchSize(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/DownloadFilesBatchSize", true, 4);
    }

    public static int GetCIMetricPublishLevel(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/CIMetricPublishLevel", true, 1);
    }

    public static int GetQueuedJobsDormancyThresholdDays(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/QueuedJobsDormancyThreshold", true, 10);
    }

    public static int GetQueuedCleanupJobDelayIntervalInMinutes(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/QueuedCleanupJobDelayIntervalInMinutes", true);
    }

    public static int GetFileServiceRetentionPeriodInDays(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/FileService/SqlRetentionPeriod", true, 1);
    }

    public static bool GetQueueCatchupJobUntilCompletion(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<bool>(requestContext, "/Service/CodeSense/Settings/QueueCatchupJobUntilCompletion", true, false);
    }

    public static bool GetQueueFrameworkCleanupJobFromCodeLens(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<bool>(requestContext, "/Service/CodeSense/Settings/QueueFrameworkCleanupJobFromCodeLens", true, false);
    }

    public static int GetChangesetProcessingTimeThresholdInMinutes(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/ChangesetProcessingTimeThreshold", true, 15);
    }

    public static int GetChangesetCreationThresholdInDays(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/ChangesetCreationTimeThreshold", true, 7);
    }

    public static int GetTransientErrorsRetryIntervalInSeconds(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/TransientErrorRetryInterval", true, 10);
    }

    public static int GetTransientErrorsRetryCount(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/TransientErrorRetryCount", true, 2);
    }

    public static int GetAggregateFileSizeThresholdInMB(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/AggregateFileSizeThresholdInMB", true, 300);
    }

    public static int GetCleanupMonitorJobThresholdInHours(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetOrDefault<int>(requestContext, "/Service/CodeSense/Settings/CleanupMonitorJobThresholdInHours", true, 168);
    }

    public static T GetOrDefault<T>(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      string registryKey,
      T defaultValue = null)
    {
      using (requestContext.AcquireExemptionLock())
      {
        try
        {
          return registryService.GetValue<T>(requestContext, (RegistryQuery) registryKey, true, defaultValue);
        }
        catch (FormatException ex)
        {
          TeamFoundationEventLog.Default.LogException(requestContext, string.Format("Registry setting {0} was not in the correct format. Using default value of {1}.", (object) registryKey, (object) defaultValue), (Exception) ex);
          return defaultValue;
        }
      }
    }

    public static T GetOrDefault<T>(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      string registryKey,
      bool fallThru,
      T defaultValue = null)
    {
      using (requestContext.AcquireExemptionLock())
      {
        try
        {
          return registryService.GetValue<T>(requestContext, (RegistryQuery) registryKey, fallThru, defaultValue);
        }
        catch (FormatException ex)
        {
          TeamFoundationEventLog.Default.LogException(requestContext, string.Format("Registry setting {0} was not in the correct format. Using default value of {1}.", (object) registryKey, (object) defaultValue), (Exception) ex);
          return defaultValue;
        }
      }
    }
  }
}
