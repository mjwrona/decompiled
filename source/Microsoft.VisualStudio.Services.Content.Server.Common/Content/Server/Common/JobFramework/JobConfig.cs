// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework.JobConfig
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework
{
  public static class JobConfig
  {
    public const string RegistryEnabledStateKey = "/EnabledState";
    public const string RegistryEnableMultiDomainKey = "/EnableMultiDomain";
    public const string RegistryAutoJobFanoutTimestamp = "/AutoJobFanoutRecommendationTimestamp";
    public const string RegistryEnforcedJobExecutionTimeoutKey = "/EnforcedJobExecutionTimeout";
    public const string RegistryJobSchedulingIntervalKey = "/JobSchedulingIntervalInSeconds";
    public const string RegistryTotalPartitionsKey = "/TotalPartitions";
    public const string RegistryDomainKey = "/Domain/{0}";
    public const string RegistryDomainTotalPartitionsKey = "/Domain/{0}/TotalPartitions";
    public const string RegistryLastSucceededJobEndTimeKey = "/LastSucceedJobExecutionTime";
    public const string RegistryFreshJobResultWindowInSecondsKey = "/FreshJobResultWindowInSeconds";
    public const string RegistryDefaultMultiPartitionSizeKey = "/DefaultMultiPartitionSize";
    public const string RegistryMaxParallelismKey = "/MaxParallelism";
    public const string RegistryBlobIdPartitionSize = "/BlobIdPartitionSize";
    public const string RegistryAutoFanout = "/AutoFanout";
    public const int DefaultLookbackForFanoutEvaluation = 7;
    public static readonly TimeSpan DefaultTargetExecutionTimeThreshold = TimeSpan.FromHours(15.0);
    public const string RegistryCpuThresholdKey = "/CpuThreshold";
    public const int DefaultTotalBlobPartitions = 1;
    public const int DefaultJobExecutionTimeoutInSeconds = 84600;
    public const int DefaultSubPartitionJobExecutionTimeoutInSeconds = 86100;
    public const int DefaultJobSchedulingIntervalInSeconds = 60;
    public const int DefaultCpuThreshold = 70;
    public const int DefaultMultiPartitionSize = 0;
    public const int DefaultTotalPartitions = 1;
    public const int DefaultHostInactiveWindowInSeconds = 259200;
    public static readonly DateTime DefaultLastSucceededJobEndTime = DateTime.MinValue;
    public const int DefaultSucceededResultLookbackWindowInSeconds = 604800;
    public const string ArtifactServicesJobsContainer = "artifactservicesdataexport";
    public const int DefaultChildJobPollIntervalInSeconds = 60;
    public const int DefaultParentJobPollIntervalInSeconds = 900;
    public const string RegistryChildJobPollIntervalKey = "/ChildJobPollInterval";
    public const string RegistryParentJobPollInterval = "/ParentJobPollInterval";
    public const string RegistryChildJobIds = "/ChildJobIds";
    public const string RegistryParentJobStartTimeStamp = "/ParentJobStartTimeStamp";
    public const string RegistryParentJobRunId = "/ParentJobRunId";
    public const int DefaultReferenceAuditServiceBackOffDurationInSeconds = 60;
    public const int DefaultReferenceAuditServiceQueueMessageBatchSize = 10;
    public const int DefaultStatusTraceIntervalInSeconds = 300;
    public const int DefaultReferenceAuditJobParallelism = 8;
    public const string RegistryBackOffDurationInSeconds = "BackOffDurationInSeconds";
    public const string RegistryQueueMessageBatchSize = "QueueMessageBatchSize";
    public const string RegistryShouldQuitWhenQueueIsEmpty = "ShouldQuitWhenQueueIsEmpty";
    public const string RegistryStatusTraceIntervalInSeconds = "StatusTraceIntervalInSeconds";
    public const string RegistryMaxReferenceAuditJobParallelism = "MaxReferenceAuditJobParallelism";
  }
}
