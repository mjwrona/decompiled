// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildLogsController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.Xaml;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "logs", ResourceVersion = 2)]
  public class BuildLogsController : BuildApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (List<BuildLog>), null, null)]
    [ClientResponseType(typeof (Stream), "GetBuildLogsZip", "application/zip")]
    public HttpResponseMessage GetBuildLogs(int buildId, Microsoft.TeamFoundation.Build.WebApi.DefinitionType? type = null)
    {
      RequestMediaType requestMediaType = MediaTypeFormatUtility.GetPrioritizedAcceptHeaders(this.Request, new List<RequestMediaType>()
      {
        RequestMediaType.Json,
        RequestMediaType.Zip
      }).FirstOrDefault<RequestMediaType>();
      BuildData build = (BuildData) null;
      Microsoft.TeamFoundation.Build.WebApi.DefinitionType? nullable = type;
      if (((int) nullable ?? 2) == 2)
        build = this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId, includeDeleted: true);
      if (build == null)
      {
        nullable = type;
        if (((int) nullable ?? 1) == 1)
        {
          if (requestMediaType != RequestMediaType.Zip)
            return this.Request.CreateResponse<IEnumerable<BuildLog>>(HttpStatusCode.OK, this.TfsRequestContext.GetService<IXamlBuildProvider>().GetBuildLogsMetadata(this.TfsRequestContext, this.ProjectInfo, buildId));
          PushStreamContent buildLogsZip = this.TfsRequestContext.GetService<IXamlBuildProvider>().GetBuildLogsZip(this.TfsRequestContext, this.ProjectInfo, buildId);
          if (buildLogsZip == null)
            return this.Request.CreateResponse(HttpStatusCode.NotFound);
          this.TfsRequestContext.RequestTimeout = TimeSpan.FromHours(1.0);
          HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
          response.Content = (HttpContent) buildLogsZip;
          response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(string.Format("logs_{0}.zip", (object) buildId));
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_TotalDownloads").Increment();
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_DownloadSec").Increment();
          this.TfsRequestContext.UpdateTimeToFirstPage();
          return response;
        }
      }
      if (build == null)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      Guid planId;
      if (!build.Deleted)
      {
        planId = build.OrchestrationPlan.PlanId;
      }
      else
      {
        Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference orchestrationPlanReference = build.Plans.Select<Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference, Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference>((Func<Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference, Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference>) (x => x)).Where<Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference>((Func<Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference, bool>) (x =>
        {
          int? orchestrationType = x.OrchestrationType;
          int num = 2;
          return orchestrationType.GetValueOrDefault() == num & orchestrationType.HasValue;
        })).FirstOrDefault<Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference>();
        if (orchestrationPlanReference == null)
          return this.Request.CreateResponse(HttpStatusCode.OK);
        planId = orchestrationPlanReference.PlanId;
      }
      TaskOrchestrationPlan plan = this.TfsRequestContext.GetService<IBuildOrchestrator>().GetPlan(this.TfsRequestContext, build.ProjectId, planId);
      if (plan == null)
        return this.Request.CreateResponse(HttpStatusCode.OK);
      if (requestMediaType != RequestMediaType.Zip)
        return this.Request.CreateResponse<List<BuildLog>>(HttpStatusCode.OK, this.GetLogsMetadata(plan, buildId).ToList<BuildLog>());
      this.TfsRequestContext.RequestTimeout = TimeSpan.FromHours(1.0);
      return this.GetLogsZip(plan, build);
    }

    [HttpGet]
    [ClientResponseType(typeof (Stream), "GetBuildLog", "application/octet-stream")]
    public virtual HttpResponseMessage GetBuildLog(
      int buildId,
      int logId,
      long? startLine = null,
      long? endLine = null,
      Microsoft.TeamFoundation.Build.WebApi.DefinitionType? type = null)
    {
      return this.GetBuildLog(buildId, logId, startLine, endLine, type, RequestMediaType.Json);
    }

    protected HttpResponseMessage GetBuildLog(
      int buildId,
      int logId,
      long? startLine,
      long? endLine,
      Microsoft.TeamFoundation.Build.WebApi.DefinitionType? type,
      RequestMediaType formatRequested)
    {
      BuildData build = (BuildData) null;
      if (((int) type ?? 2) == 2)
        build = this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId, includeDeleted: true);
      if (build == null && ((int) type ?? 1) == 1)
      {
        StreamContent buildLog = this.TfsRequestContext.GetService<IXamlBuildProvider>().GetBuildLog(this.TfsRequestContext, this.ProjectInfo, buildId, logId);
        if (buildLog == null)
          throw new BuildLogNotFoundException(Resources.BuildLogNotFound((object) buildId));
        HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
        response.Content = (HttpContent) buildLog;
        this.TfsRequestContext.UpdateTimeToFirstPage();
        return response;
      }
      if (build == null)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      Guid? nullable = new Guid?();
      if (build.Deleted)
      {
        IBuildOrchestrator service = this.TfsRequestContext.GetService<IBuildOrchestrator>();
        Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference orchestrationPlanReference = build.Plans.Select<Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference, Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference>((Func<Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference, Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference>) (x => x)).Where<Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference>((Func<Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference, bool>) (x =>
        {
          int? orchestrationType = x.OrchestrationType;
          int num = 2;
          return orchestrationType.GetValueOrDefault() == num & orchestrationType.HasValue;
        })).FirstOrDefault<Microsoft.TeamFoundation.Build2.Server.TaskOrchestrationPlanReference>();
        if (orchestrationPlanReference != null)
        {
          TaskOrchestrationPlan plan = service.GetPlan(this.TfsRequestContext, build.ProjectId, orchestrationPlanReference.PlanId);
          if (plan != null)
            nullable = new Guid?(plan.PlanId);
        }
      }
      else if (build.OrchestrationPlan != null)
        nullable = new Guid?(build.OrchestrationPlan.PlanId);
      return !nullable.HasValue ? this.Request.CreateResponse(HttpStatusCode.OK) : this.GetLogLines(build, nullable.Value, logId, startLine, endLine, formatRequested);
    }

    private IEnumerable<BuildLog> GetLogsMetadata(TaskOrchestrationPlan plan, int buildId) => this.TfsRequestContext.GetService<IBuildOrchestrator>().GetLogs(this.TfsRequestContext, plan.ScopeIdentifier, plan.PlanId).Select<TaskLog, BuildLog>((Func<TaskLog, BuildLog>) (log => this.ConvertToBuildLog(log, buildId)));

    private BuildLog ConvertToBuildLog(TaskLog log, int buildId)
    {
      IBuildRouteService service = this.TfsRequestContext.GetService<IBuildRouteService>();
      BuildLog buildLog = new BuildLog();
      buildLog.Id = log.Id;
      buildLog.CreatedOn = new DateTime?(log.CreatedOn);
      buildLog.LastChangedOn = new DateTime?(log.LastChangedOn);
      buildLog.LineCount = log.LineCount;
      buildLog.Type = "Container";
      buildLog.Url = service.GetBuildLogRestUrl(this.TfsRequestContext, this.ProjectId, buildId, log.Id);
      return buildLog;
    }

    private HttpResponseMessage GetLogsZip(TaskOrchestrationPlan plan, BuildData build)
    {
      IBuildOrchestrator orchestrator = this.TfsRequestContext.GetService<IBuildOrchestrator>();
      Dictionary<int, TaskLog> logs = orchestrator.GetLogs(this.TfsRequestContext, plan.ScopeIdentifier, plan.PlanId).ToDictionary<TaskLog, int>((Func<TaskLog, int>) (log => log.Id));
      Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline = orchestrator.GetTimeline(this.TfsRequestContext, plan.ScopeIdentifier, plan.PlanId, plan.Timeline.Id, includeRecords: true);
      Dictionary<Guid, string> jobFolders = new Dictionary<Guid, string>();
      foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord timelineRecord in timeline.Records.Where<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (tr => string.Equals(tr.RecordType, "Job", StringComparison.OrdinalIgnoreCase))))
      {
        string str = FileSpec.RemoveInvalidFileNameChars(timelineRecord.Name);
        if (!str.EndsWith("/", StringComparison.Ordinal) && !str.EndsWith("\\", StringComparison.Ordinal))
          str += "/";
        jobFolders[timelineRecord.Id] = str;
      }
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new PushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
      {
        try
        {
          using (SmartPushStreamContentStream streamContentStream = new SmartPushStreamContentStream(stream))
          {
            using (ZipArchive zipArchive = new ZipArchive((Stream) streamContentStream, ZipArchiveMode.Create))
            {
              foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord record in timeline.Records)
              {
                Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord timelineRecord = record;
                if (string.Equals(timelineRecord.RecordType, "Job", StringComparison.OrdinalIgnoreCase))
                  zipArchive.CreateEntry(jobFolders[timelineRecord.Id]);
                string str = string.Empty;
                string empty1 = string.Empty;
                Guid? parentId = timelineRecord.ParentId;
                if (parentId.HasValue)
                {
                  Dictionary<Guid, string> dictionary = jobFolders;
                  parentId = timelineRecord.ParentId;
                  Guid key = parentId.Value;
                  ref string local = ref empty1;
                  dictionary.TryGetValue(key, out local);
                }
                TaskLog taskLog;
                if (timelineRecord.Log != null && logs.TryGetValue(timelineRecord.Log.Id, out taskLog) && taskLog.LineCount > 0L)
                {
                  string empty2 = string.Empty;
                  string entryName = !timelineRecord.Order.HasValue ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.txt", (object) FileSpec.RemoveInvalidFileNameChars(timelineRecord.Name)) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}.txt", (object) timelineRecord.Order, (object) FileSpec.RemoveInvalidFileNameChars(timelineRecord.Name));
                  str = entryName;
                  if (!string.IsNullOrEmpty(empty1))
                    entryName = empty1 + entryName;
                  ZipArchiveEntry entry = zipArchive.CreateEntry(entryName);
                  long startLine = 0;
                  long lineCount = taskLog.LineCount;
                  using (BuildLogLinesResult logLines = orchestrator.GetLogLines(this.TfsRequestContext, plan.ScopeIdentifier, plan.PlanId, build.ToSecuredObject(), taskLog.Id, startLine, lineCount))
                  {
                    using (Stream stream1 = entry.Open())
                    {
                      using (StreamWriter streamWriter = new StreamWriter(stream1))
                      {
                        foreach (string line in logLines.Lines)
                          streamWriter.WriteLine(line);
                      }
                    }
                  }
                }
                IEnumerable<TaskAttachment> source1 = orchestrator.GetAttachments(this.TfsRequestContext, plan.ScopeIdentifier, plan.PlanId, CoreAttachmentType.FileAttachment).Where<TaskAttachment>((Func<TaskAttachment, bool>) (a => a.RecordId == timelineRecord.Id));
                IEnumerable<string> source2 = timelineRecord.Order.HasValue ? source1.Select<TaskAttachment, string>((Func<TaskAttachment, string>) (a => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) timelineRecord.Order.Value, (object) a.Name))) : source1.Select<TaskAttachment, string>((Func<TaskAttachment, string>) (a => a.Name));
                foreach (TaskAttachment attachment1 in source1)
                {
                  string attachmentName = timelineRecord.Order.HasValue ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) timelineRecord.Order.Value, (object) attachment1.Name) : attachment1.Name;
                  List<string> filesToCompare = new List<string>();
                  filesToCompare.Add(str);
                  filesToCompare.AddRange(source2.Where<string>((Func<string, bool>) (a => string.Compare(a, attachmentName, StringComparison.OrdinalIgnoreCase) != 0)));
                  attachmentName = BuildLogsController.GetUniqueFileName(attachmentName, (IEnumerable<string>) filesToCompare);
                  if (!string.IsNullOrEmpty(empty1))
                    attachmentName = empty1 + attachmentName;
                  using (Stream destination = zipArchive.CreateEntry(attachmentName).Open())
                  {
                    using (Stream attachment2 = orchestrator.GetAttachment(this.TfsRequestContext, plan.ScopeIdentifier, plan.PlanId, attachment1))
                    {
                      if (attachment2 != null)
                      {
                        attachment2.CopyTo(destination);
                        destination.Flush();
                      }
                    }
                  }
                }
              }
            }
          }
        }
        finally
        {
          stream?.Dispose();
        }
      }), new MediaTypeHeaderValue("application/zip"));
      response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(string.Format("logs_{0}.zip", (object) build.Id));
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_TotalDownloads").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_DownloadSec").Increment();
      this.TfsRequestContext.UpdateTimeToFirstPage();
      return response;
    }

    private static bool FileNameExists(string targetFileName, IEnumerable<string> fileNames)
    {
      foreach (string fileName in fileNames)
      {
        if (BuildLogsController.FileNameMatches(targetFileName, fileName))
          return true;
      }
      return false;
    }

    private static bool FileNameMatches(string fileName, string fileNameToCompare) => string.Compare(fileName, fileNameToCompare, StringComparison.OrdinalIgnoreCase) == 0;

    private static string GetUniqueFileName(string fileName, IEnumerable<string> filesToCompare)
    {
      string withoutExtension = Path.GetFileNameWithoutExtension(fileName);
      string extension = Path.GetExtension(fileName);
      foreach (string fileNameToCompare in filesToCompare)
      {
        if (BuildLogsController.FileNameMatches(fileName, fileNameToCompare))
        {
          int num = 0;
          while (true)
          {
            if (num != 0)
              goto label_6;
label_4:
            fileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1}){2}", (object) withoutExtension, (object) ++num, (object) extension);
            continue;
label_6:
            if (BuildLogsController.FileNameExists(fileName, filesToCompare))
              goto label_4;
            else
              goto label_11;
          }
        }
      }
label_11:
      return fileName;
    }

    private HttpResponseMessage GetLogLines(
      BuildData build,
      Guid planId,
      int logId,
      long? startLine,
      long? endLine,
      RequestMediaType formatRequested)
    {
      long startLine1 = 0;
      long maxValue = long.MaxValue;
      if (startLine.HasValue && startLine.Value > 0L)
        startLine1 = startLine.Value;
      if (endLine.HasValue)
        maxValue = endLine.Value;
      BuildLogLinesResult logResult = this.TfsRequestContext.GetService<IBuildOrchestrator>().GetLogLines(this.TfsRequestContext, this.ProjectId, planId, build.ToSecuredObject(), logId, startLine1, maxValue);
      this.AddDisposableResource((IDisposable) logResult);
      long totalLines = logResult.TotalLines;
      long startLine2 = logResult.StartLine;
      long endLine1 = logResult.EndLine;
      HttpResponseMessage response;
      if (formatRequested != RequestMediaType.Json)
      {
        response = this.Request.CreateResponse(HttpStatusCode.OK);
        response.Content = (HttpContent) new PushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
        {
          try
          {
            using (StreamWriter streamWriter = new StreamWriter(stream))
            {
              foreach (string line in logResult.Lines)
                streamWriter.WriteLine(line);
            }
          }
          finally
          {
            stream?.Dispose();
          }
        }), new MediaTypeHeaderValue("text/plain"));
      }
      else
        response = this.Request.CreateResponse<List<string>>(HttpStatusCode.OK, logResult.Lines.ToList<string>());
      if (totalLines > 0L && startLine2 <= endLine1)
        response.Content.Headers.ContentRange = new ContentRangeHeaderValue(startLine2, endLine1, totalLines)
        {
          Unit = "lines"
        };
      this.TfsRequestContext.UpdateTimeToFirstPage();
      return response;
    }
  }
}
