// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.ReleaseLogsProcessor
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors
{
  public class ReleaseLogsProcessor
  {
    private const int MaxFilePathLength = 210;
    private readonly Func<IVssRequestContext, Guid, DeployPhaseTypes, IDistributedTaskOrchestrator> getDistributedTaskOrchestrator;

    public ReleaseLogsProcessor()
      : this(ReleaseLogsProcessor.\u003C\u003EO.\u003C0\u003E__CreateDistributedTaskOrchestrator ?? (ReleaseLogsProcessor.\u003C\u003EO.\u003C0\u003E__CreateDistributedTaskOrchestrator = new Func<IVssRequestContext, Guid, DeployPhaseTypes, IDistributedTaskOrchestrator>(DistributedTaskOrchestrator.CreateDistributedTaskOrchestrator)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    protected ReleaseLogsProcessor(
      Func<IVssRequestContext, Guid, DeployPhaseTypes, IDistributedTaskOrchestrator> getDistributedTaskOrchestratorFunc)
    {
      this.getDistributedTaskOrchestrator = getDistributedTaskOrchestratorFunc;
    }

    public static void DownloadLog(
      IVssRequestContext context,
      Guid planId,
      int logId,
      StreamWriter streamWriter,
      IDistributedTaskOrchestrator orchestrator)
    {
      ReleaseLogsProcessor.DownloadLog(context, planId, logId, new long?(), new long?(), streamWriter, orchestrator);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    public static void DownloadLog(
      IVssRequestContext context,
      Guid planId,
      int logId,
      long? startLine,
      long? endLine,
      StreamWriter streamWriter,
      IDistributedTaskOrchestrator orchestrator)
    {
      context.TraceEnter(1900046, "ReleaseManagementService", "Service", "ReleaseLogsProcessor::DownloadLog");
      long stream = ReleaseLogsProcessor.WriteLogToStream(orchestrator, planId, logId, startLine, endLine, streamWriter);
      context.Trace(1900046, TraceLevel.Info, "ReleaseManagementService", "Service", "ReleaseLogsProcessor - DownloadLog - Leave - Total Lines: {0}", (object) stream);
      if (stream != 0L)
        return;
      context.Trace(1976458, TraceLevel.Info, "ReleaseManagementService", "Service", "No log found for Id : {0}", (object) logId);
    }

    public static string GetFixedLengthFilePath(string filePath)
    {
      if (filePath == null)
        throw new ArgumentNullException(nameof (filePath));
      if (filePath.Length <= 210)
        return filePath;
      string fixedLengthFilePath = string.Empty;
      string[] strArray = filePath.Split('/');
      string name1 = strArray[strArray.Length - 1];
      int maxLength = (210 - (strArray.Length - 1)) / strArray.Length;
      foreach (string name2 in strArray)
      {
        string str = name2.Equals(name1) ? ReleaseLogsProcessor.GetFixedLengthFileName(name1, maxLength) : ReleaseLogsProcessor.GetFixedLengthFolderName(name2, maxLength);
        fixedLengthFilePath = !string.IsNullOrEmpty(fixedLengthFilePath) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) fixedLengthFilePath, (object) str) : str;
      }
      return fixedLengthFilePath;
    }

    public void DownloadLog(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      DeployPhaseTypes phaseType,
      int logId,
      long? startLine,
      long? endLine,
      StreamWriter streamWriter)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (streamWriter == null)
        throw new ArgumentNullException(nameof (streamWriter));
      IDistributedTaskOrchestrator orchestrator = this.getDistributedTaskOrchestrator(requestContext, projectId, phaseType);
      ReleaseLogsProcessor.DownloadLog(requestContext, planId, logId, startLine, endLine, streamWriter, orchestrator);
    }

    private static long WriteLogToStream(
      IDistributedTaskOrchestrator distributedTaskService,
      Guid planId,
      int logId,
      StreamWriter streamWriter)
    {
      return ReleaseLogsProcessor.WriteLogToStream(distributedTaskService, planId, logId, new long?(), new long?(), streamWriter);
    }

    private static long WriteLogToStream(
      IDistributedTaskOrchestrator distributedTaskService,
      Guid planId,
      int logId,
      long? startLine,
      long? endLine,
      StreamWriter streamWriter)
    {
      long totalLines;
      using (TeamFoundationDataReader logAllLines = distributedTaskService.GetLogAllLines(planId, logId, startLine.GetValueOrDefault(0L), endLine.GetValueOrDefault(long.MinValue), out totalLines))
      {
        foreach (string current in logAllLines.CurrentEnumerable<string>())
          streamWriter.WriteLine(current);
      }
      return totalLines;
    }

    public void DownloadLogs(
      IVssRequestContext requestContext,
      Guid projectId,
      ReleaseLogContainers logContainers,
      ZipArchive zipArchive)
    {
      requestContext.TraceEnter(1900046, "ReleaseManagementService", "Service", "ReleaseLogsProcessor::DownloadLogs");
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (zipArchive == null)
        throw new ArgumentNullException(nameof (zipArchive));
      if (logContainers == null)
        logContainers = new ReleaseLogContainers();
      if (logContainers.DeployPhases == null)
        logContainers.DeployPhases = (IList<ReleaseDeployPhaseRef>) new List<ReleaseDeployPhaseRef>();
      ReleaseLogsProcessor.GetGreenlightingLogs(requestContext, projectId, logContainers, zipArchive);
      if (logContainers.DeployPhases.Any<ReleaseDeployPhaseRef>())
      {
        foreach (KeyValuePair<Guid, ReleaseLogsProcessor.LogStructureData> keyValuePair in ReleaseLogsProcessor.GetLogFolderStructure((IEnumerable<ReleaseDeployPhaseRef>) logContainers.DeployPhases))
        {
          Guid key = keyValuePair.Key;
          ReleaseLogsProcessor.LogStructureData logStructureData = keyValuePair.Value;
          string folderStructure = logStructureData.FolderStructure;
          Func<IVssRequestContext, Guid, DeployPhaseTypes, IDistributedTaskOrchestrator> taskOrchestrator = this.getDistributedTaskOrchestrator;
          IVssRequestContext vssRequestContext = requestContext;
          Guid guid = projectId;
          logStructureData = keyValuePair.Value;
          int phaseType = (int) logStructureData.PhaseType;
          IDistributedTaskOrchestrator orchestrator = taskOrchestrator(vssRequestContext, guid, (DeployPhaseTypes) phaseType);
          logStructureData = keyValuePair.Value;
          if (logStructureData.PhaseType == DeployPhaseTypes.DeploymentGates)
            ReleaseLogsProcessor.PrepareGatesZip(zipArchive, orchestrator, key, folderStructure);
          else
            ReleaseLogsProcessor.PrepareZip(zipArchive, orchestrator, key, folderStructure);
        }
      }
      else
      {
        Dictionary<Guid, string> logFolderStructure = ReleaseLogsProcessor.GetLogFolderStructure(logContainers.DeploySteps);
        PipelineOrchestrator orchestrator = new PipelineOrchestrator(requestContext, projectId);
        foreach (KeyValuePair<Guid, string> keyValuePair in logFolderStructure)
        {
          Guid key = keyValuePair.Key;
          string folderPath = keyValuePair.Value;
          ReleaseLogsProcessor.PrepareZip(zipArchive, (IDistributedTaskOrchestrator) orchestrator, key, folderPath);
        }
      }
      requestContext.TraceLeave(1900046, "ReleaseManagementService", "Service", "ReleaseLogsProcessor::DownloadLogs");
    }

    private static void PrepareGatesZip(
      ZipArchive zipArchive,
      IDistributedTaskOrchestrator orchestrator,
      Guid planId,
      string folderPath)
    {
      ReleaseLogsProcessor.PrepareZip(zipArchive, orchestrator, planId, folderPath, (Func<Guid, IEnumerable<TimelineRecord>>) (runPlanId => (IEnumerable<TimelineRecord>) ReleaseLogsProcessor.GetGateTimelineRecords(orchestrator as GreenlightingOrchestrator, runPlanId)), (Func<IEnumerable<TimelineRecord>, Dictionary<Guid, string>>) (timelineRecords => (Dictionary<Guid, string>) null), false);
    }

    private static void PrepareZip(
      ZipArchive zipArchive,
      IDistributedTaskOrchestrator orchestrator,
      Guid planId,
      string folderPath,
      Func<Guid, IEnumerable<TimelineRecord>> getTimelineRecords = null,
      Func<IEnumerable<TimelineRecord>, Dictionary<Guid, string>> getJobIdNameMap = null,
      bool includeTaskAttachments = true)
    {
      if (getTimelineRecords == null)
        getTimelineRecords = new Func<Guid, IEnumerable<TimelineRecord>>(orchestrator.GetTimelineRecords);
      if (getJobIdNameMap == null)
        getJobIdNameMap = new Func<IEnumerable<TimelineRecord>, Dictionary<Guid, string>>(orchestrator.GetJobIdNameMap);
      List<TimelineRecord> list1 = getTimelineRecords(planId).ToList<TimelineRecord>();
      Dictionary<Guid, string> jobIdNameMap = getJobIdNameMap((IEnumerable<TimelineRecord>) list1);
      IDictionary<int, string> logIdsWithNames = orchestrator.GetLogIdsWithNames(planId, (IEnumerable<TimelineRecord>) list1, jobIdNameMap);
      foreach (KeyValuePair<int, string> keyValuePair in (IEnumerable<KeyValuePair<int, string>>) logIdsWithNames)
      {
        string str = keyValuePair.Value;
        string entryName = ReleaseLogsProcessor.SanitizeFilePath(ReleaseLogsProcessor.GetFixedLengthFilePath(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) folderPath, (object) str)));
        using (StreamWriter streamWriter = new StreamWriter(zipArchive.CreateEntry(entryName).Open()))
          ReleaseLogsProcessor.WriteLogToStream(orchestrator, planId, keyValuePair.Key, streamWriter);
      }
      if (!includeTaskAttachments)
        return;
      List<TaskAttachment> list2 = orchestrator.GetAttachments(planId, CoreAttachmentType.FileAttachment).ToList<TaskAttachment>();
      foreach (TaskAttachment taskAttachment in list2)
      {
        string attachmentFileName = DistributedTaskOrchestratorExtensions.GetAttachmentFileName(taskAttachment, (IEnumerable<TimelineRecord>) list1, (IEnumerable<TaskAttachment>) list2, logIdsWithNames, jobIdNameMap);
        string entryName = ReleaseLogsProcessor.SanitizeFilePath(ReleaseLogsProcessor.GetFixedLengthFilePath(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) folderPath, (object) attachmentFileName)));
        using (Stream destination = zipArchive.CreateEntry(entryName).Open())
        {
          using (Stream attachment = orchestrator.GetAttachment(planId, taskAttachment))
          {
            if (attachment != null)
            {
              attachment.CopyTo(destination);
              destination.Flush();
            }
          }
        }
      }
    }

    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Justification = "filePath need not be localized")]
    private static string SanitizeFilePath(string filePath)
    {
      filePath = new Regex(string.Format("[{0}]", (object) Regex.Escape(new string(Path.GetInvalidPathChars())))).Replace(filePath, string.Empty);
      return filePath;
    }

    private static string GetFixedLengthFolderName(string name, int maxLength) => name.Length > maxLength ? name.Substring(0, maxLength) : name;

    private static string GetFixedLengthFileName(string name, int maxLength)
    {
      if (name.Length <= maxLength)
        return name;
      int num = name.LastIndexOf('.');
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) (num == -1 ? name : name.Substring(0, num)).Substring(0, maxLength - 4), num == -1 ? (object) string.Empty : (object) name.Substring(num, name.Length - num).Substring(0, 4));
    }

    private static void GetGreenlightingLogs(
      IVssRequestContext requestContext,
      Guid projectId,
      ReleaseLogContainers logContainers,
      ZipArchive zipArchive)
    {
      if (logContainers == null || logContainers.Gates.IsNullOrEmpty<DeploymentGateRef>())
        return;
      GreenlightingOrchestrator greenlightingOrchestrator = new GreenlightingOrchestrator(requestContext, projectId);
      foreach (DeploymentGateRef gate in (IEnumerable<DeploymentGateRef>) logContainers.Gates)
      {
        Guid? runPlanId = gate.RunPlanId;
        if (runPlanId.HasValue)
        {
          string folderStructure = ReleaseLogsProcessor.GetFolderStructure(gate.EnvironmentName, gate.EnvironmentRank, gate.Attempt, gate.GateType.ToString());
          ZipArchive zipArchive1 = zipArchive;
          GreenlightingOrchestrator orchestrator = greenlightingOrchestrator;
          runPlanId = gate.RunPlanId;
          Guid planId = runPlanId.Value;
          string folderPath = folderStructure;
          ReleaseLogsProcessor.PrepareGatesZip(zipArchive1, (IDistributedTaskOrchestrator) orchestrator, planId, folderPath);
        }
      }
    }

    private static IList<TimelineRecord> GetGateTimelineRecords(
      GreenlightingOrchestrator orchestrator,
      Guid runPlanId)
    {
      List<TimelineRecord> list1 = orchestrator.GetTimelineRecords(runPlanId).ToList<TimelineRecord>().Select<TimelineRecord, TimelineRecord>((Func<TimelineRecord, TimelineRecord>) (t => t.Clone())).ToList<TimelineRecord>();
      List<TimelineRecord> list2 = list1.Where<TimelineRecord>((Func<TimelineRecord, bool>) (t => t.RecordType == "Job")).ToList<TimelineRecord>();
      foreach (TimelineRecord timelineRecord in list2)
        list1.Remove(timelineRecord);
      List<TimelineRecord> gateTimelineRecords = new List<TimelineRecord>();
      foreach (TimelineRecord timelineRecord1 in list2)
      {
        TimelineRecord job = timelineRecord1;
        foreach (TimelineRecord timelineRecord2 in list1.Where<TimelineRecord>((Func<TimelineRecord, bool>) (t =>
        {
          Guid? parentId = t.ParentId;
          Guid id = job.Id;
          if (!parentId.HasValue)
            return false;
          return !parentId.HasValue || parentId.GetValueOrDefault() == id;
        })).ToList<TimelineRecord>())
        {
          timelineRecord2.Order = job.Order;
          gateTimelineRecords.Add(timelineRecord2);
          list1.Remove(timelineRecord2);
        }
      }
      return (IList<TimelineRecord>) gateTimelineRecords;
    }

    private static Dictionary<Guid, string> GetLogFolderStructure(
      IList<ReleaseEnvironmentStep> deploySteps)
    {
      Dictionary<Guid, string> logFolderStructure = new Dictionary<Guid, string>();
      foreach (ReleaseEnvironmentStep deployStep in (IEnumerable<ReleaseEnvironmentStep>) deploySteps)
      {
        if (deployStep.RunPlanId.HasValue)
        {
          string folderStructure = ReleaseLogsProcessor.GetFolderStructure(deployStep);
          logFolderStructure[deployStep.RunPlanId.Value] = folderStructure;
        }
      }
      return logFolderStructure;
    }

    private static Dictionary<Guid, ReleaseLogsProcessor.LogStructureData> GetLogFolderStructure(
      IEnumerable<ReleaseDeployPhaseRef> phaseRefs)
    {
      Dictionary<Guid, ReleaseLogsProcessor.LogStructureData> logFolderStructure = new Dictionary<Guid, ReleaseLogsProcessor.LogStructureData>();
      foreach (ReleaseDeployPhaseRef phaseRef in phaseRefs)
      {
        ReleaseLogsProcessor.LogStructureData logStructureData = new ReleaseLogsProcessor.LogStructureData()
        {
          FolderStructure = ReleaseLogsProcessor.GetFolderStructure(phaseRef),
          PhaseType = phaseRef.PhaseType
        };
        logFolderStructure[phaseRef.PlanId] = logStructureData;
      }
      return logFolderStructure;
    }

    private static string GetFolderStructure(ReleaseDeployPhaseRef releaseDeployPhaseRef)
    {
      string logFolderName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}", (object) releaseDeployPhaseRef.PhaseRank, (object) releaseDeployPhaseRef.PhaseName);
      return ReleaseLogsProcessor.GetFolderStructure(releaseDeployPhaseRef.EnvironmentName, releaseDeployPhaseRef.EnvironmentRank, releaseDeployPhaseRef.Attempt, logFolderName);
    }

    private static string GetFolderStructure(ReleaseEnvironmentStep step) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/Attempt{1}", (object) step.ReleaseEnvironmentName, (object) step.TrialNumber);

    private static string GetFolderStructure(
      string environmentName,
      int environmentRank,
      int attempt,
      string logFolderName)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}/Attempt{2}/{3}", (object) environmentRank, (object) environmentName, (object) attempt, (object) logFolderName);
    }

    private struct LogStructureData
    {
      public DeployPhaseTypes PhaseType { get; set; }

      public string FolderStructure { get; set; }
    }
  }
}
