// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices.ProblemPackagesRecorder
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices
{
  public class ProblemPackagesRecorder : IProblemPackagesRecorder
  {
    private readonly ITimeProvider timeProvider;
    private readonly IOrgLevelPackagingSetting<TimeSpan> intervalSetting;
    private readonly ICommitLogWriter<ICommitLogEntry> commitLogWriter;
    private readonly IProblemPackagesRecordingThrottler throttler;
    private readonly ITracerService tracerService;
    private readonly IFeedJobQueuer cpjQueuer;

    public ProblemPackagesRecorder(
      ITimeProvider timeProvider,
      IOrgLevelPackagingSetting<TimeSpan> intervalSetting,
      ICommitLogWriter<ICommitLogEntry> commitLogWriter,
      IProblemPackagesRecordingThrottler throttler,
      ITracerService tracerService,
      IFeedJobQueuer cpjQueuer)
    {
      this.timeProvider = timeProvider;
      this.intervalSetting = intervalSetting;
      this.commitLogWriter = commitLogWriter;
      this.throttler = throttler;
      this.tracerService = tracerService;
      this.cpjQueuer = cpjQueuer;
    }

    public async Task RecordProblemPackageAsync(
      IPackageRequest packageRequest,
      UpstreamSourceInfo upstreamSource,
      TerrapinIngestionValidationResult status)
    {
      ProblemPackagesRecorder sendInTheThisObject = this;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (RecordProblemPackageAsync)))
      {
        ProblemPackageRecordingThrottleKey key = new ProblemPackageRecordingThrottleKey(packageRequest.Feed.Id, packageRequest.Protocol, packageRequest.PackageId.Name, packageRequest.PackageId.Version, sendInTheThisObject.intervalSetting.Get(), sendInTheThisObject.timeProvider.Now, upstreamSource);
        if (sendInTheThisObject.throttler.OkToRecord(key))
        {
          ICommitLogEntry commitLogEntry = await sendInTheThisObject.commitLogWriter.AppendEntryAsync(packageRequest.Feed, (ICommitOperationData) new AddProblemPackageOperationData(packageRequest.PackageId, upstreamSource, (IEnumerable<TerrapinIngestionValidationReason>) status.Reasons));
          Guid guid = await sendInTheThisObject.cpjQueuer.QueueJob(packageRequest.Feed, packageRequest.Protocol, JobPriorityLevel.Normal);
        }
      }
    }
  }
}
