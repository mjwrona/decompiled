// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildDataExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
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

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  public static class BuildDataExtensions
  {
    internal static readonly int MaxFolderLength = 90;
    internal static readonly int MaxFileNameWithoutExtLength = 60;
    internal static readonly int MaxFileExtLength = 30;
    private const string DiagnosticLogsFolderName = "Agent Diagnostic Logs/";
    private const string InitLogTxtFileName = "initializeLog.txt";
    private const string PipelineYamlFileName = "azure-pipelines-expanded.yaml";
    private const string ZipArchiveEntryNameFormat = "logs_{0}.zip";
    private const string TopOrderFilenameFormat = "{0}.txt";
    private const string SecondOrderFilenameFormat = "{0}_{1}.txt";

    public static IEnumerable<BuildLog> GetLogsMetadata(
      this BuildData build,
      TaskOrchestrationPlan plan,
      Guid projectId,
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<IBuildOrchestrator>().GetLogs(requestContext, plan.ScopeIdentifier, plan.PlanId).Select<TaskLog, BuildLog>((Func<TaskLog, BuildLog>) (log => build.ConvertToBuildLog(log, projectId, requestContext)));
    }

    public static HttpResponseMessage GetLogsZip(
      this BuildData build,
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      HttpRequestMessage request)
    {
      IBuildOrchestrator service = requestContext.GetService<IBuildOrchestrator>();
      Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline = service.GetTimeline(requestContext, plan.ScopeIdentifier, plan.PlanId, plan.Timeline.Id, includeRecords: true);
      if (requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, "Build2.UseImprovedLogsZipFetch"))
      {
        try
        {
          return build.ImprovedGetLogsZip(requestContext, plan, request, timeline);
        }
        catch (Exception ex)
        {
          TracingExtensions.TraceError(requestContext, nameof (BuildDataExtensions), "Failed to use Improved zip logs fetching, falling back to original fetching: {0}", (object) ex);
        }
      }
      IEnumerable<TaskLog> logs = service.GetLogs(requestContext, plan.ScopeIdentifier, plan.PlanId);
      return build.OriginalGetLogsZip(requestContext, plan, request, logs, timeline);
    }

    private static HttpResponseMessage OriginalGetLogsZip(
      this BuildData build,
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      HttpRequestMessage request,
      IEnumerable<TaskLog> taskLogs,
      Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline)
    {
      Dictionary<Guid, string> jobFolders = new Dictionary<Guid, string>();
      Dictionary<string, HashSet<string>> uniquePaths = new Dictionary<string, HashSet<string>>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
      foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord timelineRecord in timeline.Records.Where<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (tr => string.Equals(tr.RecordType, "Job", StringComparison.OrdinalIgnoreCase))))
      {
        string uniqueFileName = BuildDataExtensions.GetUniqueFileName(string.Empty, timelineRecord.Name, true, uniquePaths);
        jobFolders[timelineRecord.Id] = uniqueFileName + "/";
      }
      IBuildOrchestrator orchestrator = requestContext.GetService<IBuildOrchestrator>();
      Dictionary<int, TaskLog> logMap = taskLogs.ToDictionary<TaskLog, int>((Func<TaskLog, int>) (log => log.Id));
      HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new VssServerPushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
      {
        try
        {
          requestContext.UpdateTimeToFirstPage();
          using (SmartPushStreamContentStream streamContentStream = new SmartPushStreamContentStream(stream))
          {
            using (ZipArchive archive = new ZipArchive((Stream) streamContentStream, ZipArchiveMode.Create))
            {
              foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord record in timeline.Records)
              {
                Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord timelineRecord = record;
                if (string.Equals(timelineRecord.RecordType, "Job", StringComparison.OrdinalIgnoreCase))
                  archive.CreateEntry(jobFolders[timelineRecord.Id]);
                string empty;
                if (!timelineRecord.ParentId.HasValue || !jobFolders.TryGetValue(timelineRecord.ParentId.Value, out empty))
                  empty = string.Empty;
                TaskLog log;
                if (timelineRecord.Log != null && logMap.TryGetValue(timelineRecord.Log.Id, out log) && log.LineCount > 0L)
                {
                  string str = !timelineRecord.Order.HasValue ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.txt", (object) timelineRecord.Name) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}.txt", (object) timelineRecord.Order, (object) timelineRecord.Name);
                  string uniqueFileName = BuildDataExtensions.GetUniqueFileName(empty, str, false, uniquePaths);
                  AddTaskLogToArchive(Path.Combine(empty, uniqueFileName), log);
                }
                IEnumerable<TaskAttachment> source1 = orchestrator.GetAttachments(requestContext, plan.ScopeIdentifier, plan.PlanId, CoreAttachmentType.FileAttachment).Where<TaskAttachment>((Func<TaskAttachment, bool>) (a => a.RecordId == timelineRecord.Id));
                if (!timelineRecord.Order.HasValue)
                  source1.Select<TaskAttachment, string>((Func<TaskAttachment, string>) (a => a.Name));
                else
                  source1.Select<TaskAttachment, string>((Func<TaskAttachment, string>) (a => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) timelineRecord.Order.Value, (object) a.Name)));
                foreach (TaskAttachment attachment1 in source1)
                {
                  int? order = timelineRecord.Order;
                  string str1;
                  if (!order.HasValue)
                  {
                    str1 = attachment1.Name;
                  }
                  else
                  {
                    CultureInfo invariantCulture = CultureInfo.InvariantCulture;
                    order = timelineRecord.Order;
                    // ISSUE: variable of a boxed type
                    __Boxed<int> local = (ValueType) order.Value;
                    string name = attachment1.Name;
                    str1 = string.Format((IFormatProvider) invariantCulture, "{0}_{1}", (object) local, (object) name);
                  }
                  string str2 = str1;
                  string uniqueFileName = BuildDataExtensions.GetUniqueFileName(empty, str2, false, uniquePaths);
                  string entryName = empty + uniqueFileName;
                  using (Stream destination = archive.CreateEntry(entryName).Open())
                  {
                    using (Stream attachment2 = orchestrator.GetAttachment(requestContext, plan.ScopeIdentifier, plan.PlanId, attachment1))
                    {
                      if (attachment2 != null)
                      {
                        attachment2.CopyTo(destination);
                        destination.Flush();
                      }
                    }
                  }
                }
                if (requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, "WebAccess.Build.DiagnosticLog"))
                {
                  archive.CreateEntry("Agent Diagnostic Logs/");
                  IEnumerable<TaskAttachment> source2 = orchestrator.GetAttachments(requestContext, plan.ScopeIdentifier, plan.PlanId, CoreAttachmentType.DiagnosticLog).Where<TaskAttachment>((Func<TaskAttachment, bool>) (a => a.RecordId == timelineRecord.Id));
                  foreach (TaskAttachment attachment3 in source2.Where<TaskAttachment>((Func<TaskAttachment, bool>) (f => f.IsDiagnosticLogMetadataFile())))
                  {
                    string end;
                    using (StreamReader streamReader = new StreamReader(orchestrator.GetAttachment(requestContext, plan.ScopeIdentifier, plan.PlanId, attachment3)))
                      end = streamReader.ReadToEnd();
                    DiagnosticLogMetadata metadata = JsonUtilities.Deserialize<DiagnosticLogMetadata>(end);
                    TaskAttachment attachment4 = source2.Where<TaskAttachment>((Func<TaskAttachment, bool>) (d => d.Name.Equals(metadata.FileName))).FirstOrDefault<TaskAttachment>();
                    if (attachment4 != null)
                    {
                      using (Stream destination = archive.CreateEntry("Agent Diagnostic Logs/" + attachment4.Name).Open())
                      {
                        using (Stream attachment5 = orchestrator.GetAttachment(requestContext, plan.ScopeIdentifier, plan.PlanId, attachment4))
                        {
                          if (attachment5 != null)
                          {
                            attachment5.CopyTo(destination);
                            destination.Flush();
                          }
                        }
                      }
                    }
                  }
                }
              }
              TaskLog log1;
              if (plan.InitializationLog != null && logMap.TryGetValue(plan.InitializationLog.Id, out log1))
                AddTaskLogToArchive("initializeLog.txt", log1);
              TaskLog log2;
              if (plan.ExpandedYaml == null || !logMap.TryGetValue(plan.ExpandedYaml.Id, out log2))
                return;
              AddTaskLogToArchive("azure-pipelines-expanded.yaml", log2);

              void AddTaskLogToArchive(string path, TaskLog log)
              {
                ZipArchiveEntry entry = archive.CreateEntry(path);
                using (BuildLogLinesResult logLines = orchestrator.GetLogLines(requestContext, plan.ScopeIdentifier, plan.PlanId, build.ToSecuredObject(), log.Id, 0L, log.LineCount))
                {
                  using (Stream stream = entry.Open())
                  {
                    using (StreamWriter streamWriter = new StreamWriter(stream))
                    {
                      foreach (string line in logLines.Lines)
                        streamWriter.WriteLine(line);
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
      }), new MediaTypeHeaderValue("application/zip"), (object) build.ToSecuredObject());
      response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(string.Format("logs_{0}.zip", (object) build.Id));
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_TotalDownloads").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_DownloadSec").Increment();
      return response;
    }

    private static HttpResponseMessage ImprovedGetLogsZip(
      this BuildData build,
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      HttpRequestMessage request,
      Microsoft.TeamFoundation.DistributedTask.WebApi.Timeline timeline)
    {
      Dictionary<Guid, string> jobFolders = new Dictionary<Guid, string>();
      Dictionary<string, HashSet<string>> uniquePaths = new Dictionary<string, HashSet<string>>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
      foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord timelineRecord in timeline.Records.Where<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (tr => string.Equals(tr.RecordType, "Job", StringComparison.OrdinalIgnoreCase))))
      {
        string uniqueFileName = BuildDataExtensions.GetUniqueFileName(string.Empty, timelineRecord.Name, true, uniquePaths);
        jobFolders[timelineRecord.Id] = uniqueFileName + "/";
      }
      IBuildOrchestrator orchestrator = requestContext.GetService<IBuildOrchestrator>();
      Dictionary<Guid, List<TaskAttachment>> fileAttachmentsMap = orchestrator.GetAttachments(requestContext, plan.ScopeIdentifier, plan.PlanId, CoreAttachmentType.FileAttachment).GroupBy<TaskAttachment, Guid>((Func<TaskAttachment, Guid>) (a => a.RecordId)).ToDictionary<IGrouping<Guid, TaskAttachment>, Guid, List<TaskAttachment>>((Func<IGrouping<Guid, TaskAttachment>, Guid>) (i => i.Key), (Func<IGrouping<Guid, TaskAttachment>, List<TaskAttachment>>) (i => i.ToList<TaskAttachment>()));
      bool diagLogsEnabled = requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, "WebAccess.Build.DiagnosticLog");
      Dictionary<Guid, List<TaskAttachment>> diagLogsMap = (Dictionary<Guid, List<TaskAttachment>>) null;
      if (diagLogsEnabled)
        diagLogsMap = orchestrator.GetAttachments(requestContext, plan.ScopeIdentifier, plan.PlanId, CoreAttachmentType.DiagnosticLog).GroupBy<TaskAttachment, Guid>((Func<TaskAttachment, Guid>) (a => a.RecordId)).ToDictionary<IGrouping<Guid, TaskAttachment>, Guid, List<TaskAttachment>>((Func<IGrouping<Guid, TaskAttachment>, Guid>) (i => i.Key), (Func<IGrouping<Guid, TaskAttachment>, List<TaskAttachment>>) (i => i.ToList<TaskAttachment>()));
      HashSet<int> hashSet = timeline.Records.Where<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, bool>) (i => i.Log != null)).Select<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, int>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord, int>) (i => i.Log.Id)).ToHashSet<int>();
      if (plan.InitializationLog != null)
        hashSet.Add(plan.InitializationLog.Id);
      if (plan.ExpandedYaml != null)
        hashSet.Add(plan.ExpandedYaml.Id);
      Dictionary<int, BuildLogLinesResult> logResultsMap = orchestrator.GetLogLinesBatch(requestContext, plan.ScopeIdentifier, plan.PlanId, (ISet<int>) hashSet);
      HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new VssServerPushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
      {
        try
        {
          requestContext.UpdateTimeToFirstPage();
          using (SmartPushStreamContentStream streamContentStream = new SmartPushStreamContentStream(stream))
          {
            using (ZipArchive archive = new ZipArchive((Stream) streamContentStream, ZipArchiveMode.Create))
            {
              if (diagLogsEnabled)
                archive.CreateEntry("Agent Diagnostic Logs/");
              foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord record in timeline.Records)
              {
                if (string.Equals(record.RecordType, "Job", StringComparison.OrdinalIgnoreCase))
                  archive.CreateEntry(jobFolders[record.Id]);
                Guid? parentId = record.ParentId;
                string empty;
                if (parentId.HasValue)
                {
                  Dictionary<Guid, string> dictionary = jobFolders;
                  parentId = record.ParentId;
                  Guid key = parentId.Value;
                  ref string local = ref empty;
                  if (dictionary.TryGetValue(key, out local))
                    goto label_11;
                }
                empty = string.Empty;
label_11:
                BuildLogLinesResult logLinesResult;
                if (record.Log != null && logResultsMap.TryGetValue(record.Log.Id, out logLinesResult))
                {
                  string str = record.Order.HasValue ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}.txt", (object) record.Order, (object) record.Name) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.txt", (object) record.Name);
                  string uniqueFileName = BuildDataExtensions.GetUniqueFileName(empty, str, false, uniquePaths);
                  string fileName = Path.Combine(empty, uniqueFileName);
                  BuildDataExtensions.WriteFileToArchive(archive, fileName, logLinesResult);
                }
                List<TaskAttachment> attachments;
                if (fileAttachmentsMap.TryGetValue(record.Id, out attachments))
                  BuildDataExtensions.AddAttachmentsToArchive(requestContext, archive, orchestrator, plan, record.Order, empty, uniquePaths, attachments);
                List<TaskAttachment> diagnosticFileAttachments;
                if (diagLogsEnabled && diagLogsMap.TryGetValue(record.Id, out diagnosticFileAttachments))
                  BuildDataExtensions.AddDiagnosticLogsAttachmentsToArchive(requestContext, archive, orchestrator, plan, "Agent Diagnostic Logs/", diagnosticFileAttachments);
              }
              BuildLogLinesResult logLinesResult1;
              if (plan.InitializationLog != null && logResultsMap.TryGetValue(plan.InitializationLog.Id, out logLinesResult1))
                BuildDataExtensions.WriteFileToArchive(archive, "initializeLog.txt", logLinesResult1);
              BuildLogLinesResult logLinesResult2;
              if (plan.ExpandedYaml == null || !logResultsMap.TryGetValue(plan.ExpandedYaml.Id, out logLinesResult2))
                return;
              BuildDataExtensions.WriteFileToArchive(archive, "azure-pipelines-expanded.yaml", logLinesResult2);
            }
          }
        }
        finally
        {
          stream?.Dispose();
        }
      }), new MediaTypeHeaderValue("application/zip"), (object) build.ToSecuredObject());
      response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(string.Format("logs_{0}.zip", (object) build.Id));
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_TotalDownloads").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_DownloadSec").Increment();
      return response;
    }

    private static void WriteFileToArchive(
      ZipArchive archive,
      string fileName,
      BuildLogLinesResult logLinesResult)
    {
      ZipArchiveEntry entry = archive.CreateEntry(fileName);
      using (BuildLogLinesResult buildLogLinesResult = logLinesResult)
      {
        using (Stream stream = entry.Open())
        {
          using (StreamWriter streamWriter = new StreamWriter(stream))
          {
            foreach (string line in buildLogLinesResult.Lines)
              streamWriter.WriteLine(line);
          }
        }
      }
    }

    private static void AddAttachmentsToArchive(
      IVssRequestContext requestContext,
      ZipArchive archive,
      IBuildOrchestrator orchestrator,
      TaskOrchestrationPlan plan,
      int? timelineRecordOrder,
      string folderName,
      Dictionary<string, HashSet<string>> uniquePaths,
      List<TaskAttachment> attachments)
    {
      foreach (TaskAttachment attachment1 in attachments)
      {
        using (Stream attachment2 = orchestrator.GetAttachment(requestContext, plan.ScopeIdentifier, plan.PlanId, attachment1))
        {
          if (attachment2 != null)
          {
            string str = timelineRecordOrder.HasValue ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) timelineRecordOrder.Value, (object) attachment1.Name) : attachment1.Name;
            string uniqueFileName = BuildDataExtensions.GetUniqueFileName(folderName, str, false, uniquePaths);
            string entryName = folderName + uniqueFileName;
            using (Stream destination = archive.CreateEntry(entryName).Open())
              attachment2.CopyTo(destination);
          }
        }
      }
    }

    private static void AddDiagnosticLogsAttachmentsToArchive(
      IVssRequestContext requestContext,
      ZipArchive archive,
      IBuildOrchestrator orchestrator,
      TaskOrchestrationPlan plan,
      string diagnosticLogsFolderName,
      List<TaskAttachment> diagnosticFileAttachments)
    {
      foreach (TaskAttachment attachment1 in diagnosticFileAttachments.Where<TaskAttachment>((Func<TaskAttachment, bool>) (f => f.IsDiagnosticLogMetadataFile())))
      {
        string end;
        using (Stream attachment2 = orchestrator.GetAttachment(requestContext, plan.ScopeIdentifier, plan.PlanId, attachment1))
        {
          if (attachment2 != null)
          {
            using (StreamReader streamReader = new StreamReader(attachment2))
              end = streamReader.ReadToEnd();
          }
          else
            continue;
        }
        DiagnosticLogMetadata metadata = JsonUtilities.Deserialize<DiagnosticLogMetadata>(end);
        TaskAttachment attachment3 = diagnosticFileAttachments.FirstOrDefault<TaskAttachment>((Func<TaskAttachment, bool>) (d => d.Name.Equals(metadata.FileName)));
        if (attachment3 != null)
        {
          ZipArchiveEntry entry = archive.CreateEntry(diagnosticLogsFolderName + attachment3.Name);
          using (Stream attachment4 = orchestrator.GetAttachment(requestContext, plan.ScopeIdentifier, plan.PlanId, attachment3))
          {
            if (attachment4 != null)
            {
              using (Stream destination = entry.Open())
                attachment4.CopyTo(destination);
            }
          }
        }
      }
    }

    public static HttpResponseMessage GetLogZip(
      this BuildData build,
      IVssRequestContext requestContext,
      string fileName,
      IEnumerable<string> lines,
      HttpRequestMessage request)
    {
      HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new VssServerPushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
      {
        using (stream)
        {
          requestContext.UpdateTimeToFirstPage();
          using (SmartPushStreamContentStream streamContentStream = new SmartPushStreamContentStream(stream))
          {
            using (ZipArchive zipArchive = new ZipArchive((Stream) streamContentStream, ZipArchiveMode.Create))
            {
              using (Stream stream1 = zipArchive.CreateEntry(string.Format("{0}.txt", (object) fileName)).Open())
              {
                using (StreamWriter streamWriter = new StreamWriter(stream1))
                {
                  foreach (string line in lines)
                    streamWriter.WriteLine(line);
                }
              }
            }
          }
        }
      }), new MediaTypeHeaderValue("application/zip"), (object) build.ToSecuredObject());
      response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(string.Format("log_{0}_{1}.zip", (object) fileName, (object) build.Id));
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_TotalDownloads").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_DownloadSec").Increment();
      return response;
    }

    private static BuildLog ConvertToBuildLog(
      this BuildData build,
      TaskLog log,
      Guid projectId,
      IVssRequestContext requestContext)
    {
      IBuildRouteService service = requestContext.GetService<IBuildRouteService>();
      BuildLog buildLog = new BuildLog(build.ToSecuredObject());
      buildLog.Id = log.Id;
      buildLog.CreatedOn = new DateTime?(log.CreatedOn);
      buildLog.LastChangedOn = new DateTime?(log.LastChangedOn);
      buildLog.LineCount = log.LineCount;
      buildLog.Type = "Container";
      buildLog.Url = service.GetBuildLogRestUrl(requestContext, projectId, build.Id, log.Id);
      return buildLog;
    }

    internal static string GetUniqueFileName(
      string parent,
      string item,
      bool isFolder,
      Dictionary<string, HashSet<string>> uniquePaths)
    {
      string str1 = FileSpec.RemoveInvalidFileNameChars(item);
      string str2;
      string str3;
      if (isFolder)
      {
        str2 = TruncateAfter(str1, BuildDataExtensions.MaxFolderLength);
        str3 = string.Empty;
      }
      else
      {
        str2 = TruncateAfter(Path.GetFileNameWithoutExtension(str1), BuildDataExtensions.MaxFileNameWithoutExtLength);
        str3 = TruncateAfter(Path.GetExtension(str1), BuildDataExtensions.MaxFileExtLength);
      }
      string uniqueFileName = str2 + str3;
      HashSet<string> stringSet;
      if (!uniquePaths.TryGetValue(parent ?? string.Empty, out stringSet))
      {
        stringSet = new HashSet<string>(uniquePaths.Comparer);
        uniquePaths[parent ?? string.Empty] = stringSet;
      }
      int num = 0;
      while (!stringSet.Add(uniqueFileName))
        uniqueFileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1}){2}", (object) str2, (object) ++num, (object) str3);
      return uniqueFileName;

      static string TruncateAfter(string str, int max) => (str.Length > max ? str.Substring(0, max) : str).Trim();
    }
  }
}
