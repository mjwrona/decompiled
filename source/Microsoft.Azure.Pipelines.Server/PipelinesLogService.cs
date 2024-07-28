// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.PipelinesLogService
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.Azure.Pipelines.Server.Migration;
using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Pipelines.Server
{
  public class PipelinesLogService : IPipelinesLogService, IVssFrameworkService
  {
    private const string c_layer = "PipelinesLogService";
    private const string c_taskHubName = "Build";
    internal static readonly int MaxFolderLength = 90;
    internal static readonly int MaxFileNameWithoutExtLength = 60;
    internal static readonly int MaxFileExtLength = 30;

    public TaskLog GetLog(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId,
      int logId)
    {
      using (requestContext.TraceScope(nameof (PipelinesLogService), nameof (GetLog)))
      {
        Microsoft.Azure.Pipelines.Server.ObjectModel.Run run = requestContext.GetService<IExternalRunsService>().GetRun(requestContext, projectId, pipelineId, runId);
        if (run == null)
          throw new RunNotFoundException(PipelinesServerResources.RunNotFound((object) runId)).Expected("Pipelines");
        if (!run.PlanId.HasValue)
        {
          requestContext.TraceInfo(nameof (PipelinesLogService), "No planId for run {0}", (object) runId);
          throw new LogNotFoundException(PipelinesServerResources.LogNotFound((object) logId));
        }
        Guid planId = run.PlanId.Value;
        return this.GetTaskHub(requestContext).GetLog(requestContext, projectId, planId, logId) ?? throw new LogNotFoundException(PipelinesServerResources.LogNotFound((object) logId)).Expected("Pipelines");
      }
    }

    public Action<Stream> GetLogContent(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId,
      int logId,
      long startLine = 0,
      long endLine = 9223372036854775807)
    {
      using (requestContext.TraceScope(nameof (PipelinesLogService), nameof (GetLogContent)))
      {
        Microsoft.Azure.Pipelines.Server.ObjectModel.Run run = requestContext.GetService<IExternalRunsService>().GetRun(requestContext, projectId, pipelineId, runId);
        if (run == null)
          throw new RunNotFoundException(PipelinesServerResources.RunNotFound((object) runId)).Expected("Pipelines");
        if (!run.PlanId.HasValue)
        {
          requestContext.TraceInfo(nameof (PipelinesLogService), "No planId for run {0}", (object) runId);
          throw new LogNotFoundException(PipelinesServerResources.LogNotFound((object) logId));
        }
        Guid planId = run.PlanId.Value;
        requestContext.AddDisposableResource((IDisposable) (this.GetLogLines(requestContext, projectId, planId, run.ToSecuredObject(), logId, startLine, endLine) ?? throw new LogNotFoundException(PipelinesServerResources.LogNotFound((object) logId)).Expected("Pipelines")));
        return new Action<Stream>(OnStreamAvailable);
      }
      PipelinesLogService.PipelinesLogLinesResult logResult;

      void OnStreamAvailable(Stream stream)
      {
        using (stream)
        {
          using (StreamWriter streamWriter = new StreamWriter(stream))
          {
            foreach (string line in logResult.Lines)
              streamWriter.WriteLine(line);
          }
        }
      }
    }

    public IEnumerable<TaskLog> GetLogs(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId)
    {
      using (requestContext.TraceScope(nameof (PipelinesLogService), nameof (GetLogs)))
      {
        Microsoft.Azure.Pipelines.Server.ObjectModel.Run run = requestContext.GetService<IExternalRunsService>().GetRun(requestContext, projectId, pipelineId, runId);
        if (run == null)
          throw new RunNotFoundException(PipelinesServerResources.RunNotFound((object) runId)).Expected("Pipelines");
        if (!run.PlanId.HasValue)
        {
          requestContext.TraceInfo(nameof (PipelinesLogService), "No planId for run {0}", (object) runId);
          return (IEnumerable<TaskLog>) Array.Empty<TaskLog>();
        }
        Guid planId = run.PlanId.Value;
        return this.GetTaskHub(requestContext).GetLogs(requestContext, projectId, planId);
      }
    }

    public Action<Stream> GetZippedLogContents(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId)
    {
      using (requestContext.TraceScope(nameof (PipelinesLogService), nameof (GetZippedLogContents)))
      {
        Microsoft.Azure.Pipelines.Server.ObjectModel.Run run = requestContext.GetService<IExternalRunsService>().GetRun(requestContext, projectId, pipelineId, runId);
        Guid? nullable = run != null ? run.PlanId : throw new RunNotFoundException(PipelinesServerResources.RunNotFound((object) runId)).Expected("Pipelines");
        if (!nullable.HasValue)
        {
          requestContext.TraceError(nameof (PipelinesLogService), "No planId for run {0}", (object) runId);
          throw new LogNotFoundException(PipelinesServerResources.LogsNotFound((object) runId));
        }
        nullable = run.PlanId;
        Guid planId = nullable.Value;
        TaskHub taskHub = this.GetTaskHub(requestContext);
        TaskOrchestrationPlan plan = taskHub.GetPlan(requestContext, projectId, planId);
        if (plan == null)
        {
          requestContext.TraceError(nameof (PipelinesLogService), "No plan found for run {0}, planId {1}", (object) runId, (object) planId);
          throw new LogNotFoundException(PipelinesServerResources.LogsNotFound((object) runId));
        }
        if (plan.Timeline == null)
        {
          requestContext.TraceError(nameof (PipelinesLogService), "No timelineId for run {0}, planId {1}", (object) runId, (object) planId);
          throw new LogNotFoundException(PipelinesServerResources.LogsNotFound((object) runId));
        }
        Timeline timeline = taskHub.GetTimeline(requestContext, projectId, planId, plan.Timeline.Id, includeRecords: true);
        if (timeline == null)
        {
          requestContext.TraceError(nameof (PipelinesLogService), "No timeline found for run {0}, planId {1}, timelineId {2}", (object) runId, (object) planId, (object) plan.Timeline.Id);
          throw new LogNotFoundException(PipelinesServerResources.LogsNotFound((object) runId));
        }
        IEnumerable<TaskLog> logs = taskHub.GetLogs(requestContext, projectId, planId);
        if (logs == null)
        {
          requestContext.TraceError(nameof (PipelinesLogService), "No logs found for run {0}, planId {1}", (object) runId, (object) planId);
          throw new LogNotFoundException(PipelinesServerResources.LogsNotFound((object) runId));
        }
        Dictionary<int, TaskLog> logMap = logs.ToDictionary<TaskLog, int>((Func<TaskLog, int>) (log => log.Id));
        Dictionary<Guid, string> jobFolders = new Dictionary<Guid, string>();
        Dictionary<string, HashSet<string>> uniquePaths = new Dictionary<string, HashSet<string>>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
        foreach (TimelineRecord timelineRecord in timeline.Records.Where<TimelineRecord>((Func<TimelineRecord, bool>) (tr => string.Equals(tr.RecordType, "Job", StringComparison.OrdinalIgnoreCase))))
        {
          string uniqueFileName = PipelinesLogService.GetUniqueFileName(string.Empty, timelineRecord.Name, true, uniquePaths);
          jobFolders[timelineRecord.Id] = uniqueFileName + "/";
        }
        return new Action<Stream>(OnStreamAvailable);

        void AddTaskLogToArchive(ZipArchive archive, string path, TaskLog log)
        {
          ZipArchiveEntry entry = archive.CreateEntry(path);
          using (PipelinesLogService.PipelinesLogLinesResult logLines = this.GetLogLines(requestContext, plan.ScopeIdentifier, plan.PlanId, run.ToSecuredObject(), log.Id, 0L, log.LineCount))
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

        void OnStreamAvailable(Stream stream)
        {
          using (stream)
          {
            using (SmartPushStreamContentStream streamContentStream = new SmartPushStreamContentStream(stream))
            {
              using (ZipArchive archive = new ZipArchive((Stream) streamContentStream, ZipArchiveMode.Create))
              {
                foreach (TimelineRecord record in timeline.Records)
                {
                  TimelineRecord timelineRecord = record;
                  if (string.Equals(timelineRecord.RecordType, "Job", StringComparison.OrdinalIgnoreCase))
                    archive.CreateEntry(jobFolders[timelineRecord.Id]);
                  Guid? nullable = timelineRecord.ParentId;
                  string empty;
                  if (nullable.HasValue)
                  {
                    Dictionary<Guid, string> dictionary = jobFolders;
                    nullable = timelineRecord.ParentId;
                    Guid key = nullable.Value;
                    ref string local = ref empty;
                    if (dictionary.TryGetValue(key, out local))
                      goto label_10;
                  }
                  empty = string.Empty;
label_10:
                  TaskLog log;
                  int? order;
                  if (timelineRecord.Log != null && logMap.TryGetValue(timelineRecord.Log.Id, out log) && log.LineCount > 0L)
                  {
                    order = timelineRecord.Order;
                    string str;
                    if (order.HasValue)
                      str = FormattableString.Invariant(FormattableStringFactory.Create("{0}_{1}.txt", (object) timelineRecord.Order, (object) timelineRecord.Name));
                    else
                      str = FormattableString.Invariant(FormattableStringFactory.Create("{0}.txt", (object) timelineRecord.Name));
                    string uniqueFileName = PipelinesLogService.GetUniqueFileName(empty, str, false, uniquePaths);
                    string path = Path.Combine(empty, uniqueFileName);
                    AddTaskLogToArchive(archive, path, log);
                  }
                  TaskHub hub = taskHub;
                  IVssRequestContext requestContext = requestContext;
                  Guid scopeIdentifier = plan.ScopeIdentifier;
                  Guid planId = plan.PlanId;
                  string fileAttachment = CoreAttachmentType.FileAttachment;
                  nullable = new Guid?();
                  Guid? timelineId = nullable;
                  nullable = new Guid?();
                  Guid? recordId = nullable;
                  List<TaskAttachment> list = hub.GetAttachments(requestContext, scopeIdentifier, planId, fileAttachment, timelineId, recordId).Where<TaskAttachment>((Func<TaskAttachment, bool>) (a => a.RecordId == timelineRecord.Id)).ToList<TaskAttachment>();
                  order = timelineRecord.Order;
                  if (!order.HasValue)
                    list.Select<TaskAttachment, string>((Func<TaskAttachment, string>) (a => a.Name));
                  else
                    list.Select<TaskAttachment, string>((Func<TaskAttachment, string>) (a => FormattableString.Invariant(FormattableStringFactory.Create("{0}_{1}", (object) timelineRecord.Order.Value, (object) a.Name))));
                  foreach (TaskAttachment attachment1 in list)
                  {
                    order = timelineRecord.Order;
                    string str1;
                    if (!order.HasValue)
                    {
                      str1 = attachment1.Name;
                    }
                    else
                    {
                      object[] objArray = new object[2];
                      order = timelineRecord.Order;
                      objArray[0] = (object) order.Value;
                      objArray[1] = (object) attachment1.Name;
                      str1 = FormattableString.Invariant(FormattableStringFactory.Create("{0}_{1}", objArray));
                    }
                    string str2 = str1;
                    string uniqueFileName = PipelinesLogService.GetUniqueFileName(empty, str2, false, uniquePaths);
                    string entryName = empty + uniqueFileName;
                    using (Stream destination = archive.CreateEntry(entryName).Open())
                    {
                      using (Stream attachment2 = taskHub.GetAttachment(requestContext, plan.ScopeIdentifier, plan.PlanId, attachment1))
                      {
                        if (attachment2 != null)
                        {
                          attachment2.CopyTo(destination);
                          destination.Flush();
                        }
                      }
                    }
                  }
                  string entryName1 = "Agent Diagnostic Logs/";
                  archive.CreateEntry(entryName1);
                  IEnumerable<TaskAttachment> source = taskHub.GetAttachments(requestContext, plan.ScopeIdentifier, plan.PlanId, CoreAttachmentType.DiagnosticLog).Where<TaskAttachment>((Func<TaskAttachment, bool>) (a => a.RecordId == timelineRecord.Id));
                  foreach (TaskAttachment attachment3 in source.Where<TaskAttachment>((Func<TaskAttachment, bool>) (f => f.IsDiagnosticLogMetadataFile())))
                  {
                    string end;
                    using (StreamReader streamReader = new StreamReader(taskHub.GetAttachment(requestContext, plan.ScopeIdentifier, plan.PlanId, attachment3)))
                      end = streamReader.ReadToEnd();
                    DiagnosticLogMetadata metadata = JsonUtilities.Deserialize<DiagnosticLogMetadata>(end);
                    TaskAttachment attachment4 = source.Where<TaskAttachment>((Func<TaskAttachment, bool>) (d => d.Name.Equals(metadata.FileName))).FirstOrDefault<TaskAttachment>();
                    if (attachment4 != null)
                    {
                      using (Stream destination = archive.CreateEntry(entryName1 + attachment4.Name).Open())
                      {
                        using (Stream attachment5 = taskHub.GetAttachment(requestContext, plan.ScopeIdentifier, plan.PlanId, attachment4))
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
                TaskLog log1;
                if (plan.InitializationLog != null && logMap.TryGetValue(plan.InitializationLog.Id, out log1))
                  AddTaskLogToArchive(archive, "InitializePipeline.txt", log1);
                TaskLog log2;
                if (plan.ExpandedYaml == null || !logMap.TryGetValue(plan.ExpandedYaml.Id, out log2))
                  return;
                AddTaskLogToArchive(archive, "azure-pipelines-expanded.yaml", log2);
              }
            }
          }
        }
      }
    }

    private TaskHub GetTaskHub(IVssRequestContext requestContext) => requestContext.GetService<IDistributedTaskHubService>().GetTaskHub(requestContext, "Build");

    private PipelinesLogService.PipelinesLogLinesResult GetLogLines(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      ISecuredObject securedObject,
      int logId,
      long startLine,
      long endLine)
    {
      using (requestContext.TraceScope(nameof (PipelinesLogService), nameof (GetLogLines)))
      {
        TaskHub taskHub = this.GetTaskHub(requestContext);
        long totalLines;
        TeamFoundationDataReader logLines = taskHub.GetLogLines(requestContext, projectId, planId, logId, securedObject, ref startLine, ref endLine, out totalLines);
        if (totalLines == 0L && taskHub.GetLog(requestContext, projectId, planId, logId) == null)
        {
          logLines.Dispose();
          return (PipelinesLogService.PipelinesLogLinesResult) null;
        }
        return new PipelinesLogService.PipelinesLogLinesResult(logLines)
        {
          StartLine = startLine,
          EndLine = endLine,
          TotalLines = totalLines
        };
      }
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
        str2 = TruncateAfter(str1, PipelinesLogService.MaxFolderLength);
        str3 = string.Empty;
      }
      else
      {
        str2 = TruncateAfter(Path.GetFileNameWithoutExtension(str1), PipelinesLogService.MaxFileNameWithoutExtLength);
        str3 = TruncateAfter(Path.GetExtension(str1), PipelinesLogService.MaxFileExtLength);
      }
      string uniqueFileName = str2 + str3;
      HashSet<string> stringSet;
      if (!uniquePaths.TryGetValue(parent ?? string.Empty, out stringSet))
      {
        stringSet = new HashSet<string>(uniquePaths.Comparer);
        uniquePaths[parent ?? string.Empty] = stringSet;
      }
      int num = 0;
      object[] objArray;
      for (; !stringSet.Add(uniqueFileName); uniqueFileName = FormattableString.Invariant(FormattableStringFactory.Create("{0} ({1}){2}", objArray)))
        objArray = new object[3]
        {
          (object) str2,
          (object) ++num,
          (object) str3
        };
      return uniqueFileName;

      static string TruncateAfter(string str, int max) => (str.Length > max ? str.Substring(0, max) : str).Trim();
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    private class PipelinesLogLinesResult : IDisposable
    {
      private TeamFoundationDataReader m_reader;
      private bool m_disposed;

      public PipelinesLogLinesResult(TeamFoundationDataReader reader) => this.m_reader = reader;

      public IEnumerable<string> Lines
      {
        get
        {
          if (this.m_disposed)
            throw new ObjectDisposedException(nameof (PipelinesLogLinesResult));
          return this.m_reader.CurrentEnumerable<string>();
        }
      }

      public long StartLine { get; set; }

      public long EndLine { get; set; }

      public long TotalLines { get; set; }

      protected virtual void Dispose(bool disposing)
      {
        if (this.m_disposed)
          return;
        if (disposing && this.m_reader != null)
        {
          this.m_reader.Dispose();
          this.m_reader = (TeamFoundationDataReader) null;
        }
        this.m_disposed = true;
      }

      public void Dispose() => this.Dispose(true);
    }
  }
}
