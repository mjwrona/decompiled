// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.DistributedTaskOrchestratorExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  public static class DistributedTaskOrchestratorExtensions
  {
    public static IEnumerable<TimelineRecord> GetTaskTimelineRecords(
      this IDistributedTaskOrchestrator distributedTaskOrchestrator,
      Guid planId)
    {
      if (distributedTaskOrchestrator == null)
        throw new ArgumentNullException(nameof (distributedTaskOrchestrator));
      return distributedTaskOrchestrator.GetTimelineRecords(planId).Where<TimelineRecord>((Func<TimelineRecord, bool>) (record => record.ParentId.HasValue));
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Required to know how many lines are retrieved.")]
    public static TeamFoundationDataReader GetLogAllLines(
      this IDistributedTaskOrchestrator distributedTaskOrchestrator,
      Guid planId,
      int logId,
      long startLine,
      long endLine,
      out long totalLines)
    {
      if (distributedTaskOrchestrator == null)
        throw new ArgumentNullException(nameof (distributedTaskOrchestrator));
      try
      {
        return distributedTaskOrchestrator.GetLoglines(planId, logId, ref startLine, ref endLine, out totalLines);
      }
      catch (TaskOrchestrationPlanNotFoundException ex)
      {
        throw new InvalidOperationException(ex.Message);
      }
    }

    public static IDictionary<int, string> GetLogIdsWithNames(
      this IDistributedTaskOrchestrator distributedTaskOrchestrator,
      Guid planId,
      IEnumerable<TimelineRecord> timelineRecords,
      Dictionary<Guid, string> jobIdNameMap)
    {
      if (distributedTaskOrchestrator == null)
        throw new ArgumentNullException(nameof (distributedTaskOrchestrator));
      if (timelineRecords == null)
        throw new ArgumentNullException(nameof (timelineRecords));
      Dictionary<int, string> logIdsWithNames = new Dictionary<int, string>();
      foreach (TaskLogReference taskLogReference in distributedTaskOrchestrator.GetLogs(planId).Where<TaskLog>((Func<TaskLog, bool>) (log => log.LineCount > 0L)))
      {
        int id = taskLogReference.Id;
        string fileName;
        if (DistributedTaskOrchestratorExtensions.TryGetLogFileStructure(distributedTaskOrchestrator, jobIdNameMap, timelineRecords, id, out fileName))
          logIdsWithNames.Add(id, fileName);
      }
      return (IDictionary<int, string>) logIdsWithNames;
    }

    private static bool TryGetLogFileStructure(
      IDistributedTaskOrchestrator distributedTaskOrchestrator,
      Dictionary<Guid, string> jobNameMap,
      IEnumerable<TimelineRecord> timelineRecords,
      int logId,
      out string fileName)
    {
      TimelineRecord timelineRecord = timelineRecords.SingleOrDefault<TimelineRecord>((Func<TimelineRecord, bool>) (r => !string.IsNullOrWhiteSpace(r.Name) && r.Log != null && r.Log.Id == logId));
      string folderName = "Release";
      if (timelineRecord == null || jobNameMap != null && !DistributedTaskOrchestratorExtensions.TryGetFolderName(jobNameMap, timelineRecord, out folderName))
      {
        string str = FileSpec.RemoveInvalidFileNameChars(timelineRecord != null ? timelineRecord.Name : "Unknown");
        fileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}.log", (object) logId, (object) str);
        return true;
      }
      fileName = distributedTaskOrchestrator.GetLogRelativeFilePath(timelineRecord, folderName);
      return !string.IsNullOrWhiteSpace(fileName);
    }

    private static bool TryGetFolderName(
      Dictionary<Guid, string> jobNameMap,
      TimelineRecord timelineRecord,
      out string folderName)
    {
      folderName = (string) null;
      if (timelineRecord == null)
        return false;
      switch (timelineRecord.RecordType)
      {
        case "Job":
          return jobNameMap.TryGetValue(timelineRecord.Id, out folderName);
        case "Task":
          return timelineRecord.ParentId.HasValue && jobNameMap.TryGetValue(timelineRecord.ParentId.Value, out folderName);
        case "Phase":
          folderName = FileSpec.RemoveInvalidFileNameChars(timelineRecord.Name);
          return true;
        default:
          return false;
      }
    }

    public static string GetAttachmentFileName(
      TaskAttachment taskAttachment,
      IEnumerable<TimelineRecord> timelineRecords,
      IEnumerable<TaskAttachment> taskAttachments,
      IDictionary<int, string> logs,
      Dictionary<Guid, string> jobIdNameMap)
    {
      if (taskAttachment == null)
        throw new ArgumentNullException(nameof (taskAttachment));
      if (timelineRecords == null)
        throw new ArgumentNullException(nameof (timelineRecords));
      if (taskAttachments == null)
        throw new ArgumentNullException(nameof (taskAttachments));
      if (logs == null)
        throw new ArgumentNullException(nameof (logs));
      string jobName = (string) null;
      TimelineRecord timelineRecord = timelineRecords.SingleOrDefault<TimelineRecord>((Func<TimelineRecord, bool>) (r => r.Id == taskAttachment.RecordId));
      int? order;
      int num;
      if (timelineRecord != null)
      {
        order = timelineRecord.Order;
        if (order.HasValue)
        {
          num = DistributedTaskOrchestratorExtensions.TryGetFolderName(jobIdNameMap, timelineRecord, out jobName) ? 1 : 0;
          goto label_12;
        }
      }
      num = 0;
label_12:
      bool flag = num != 0;
      string str1;
      if (!flag)
      {
        str1 = taskAttachment.Name;
      }
      else
      {
        CultureInfo invariantCulture = CultureInfo.InvariantCulture;
        string str2 = jobName;
        order = timelineRecord.Order;
        // ISSUE: variable of a boxed type
        __Boxed<int> local = (ValueType) order.Value;
        string name = taskAttachment.Name;
        str1 = string.Format((IFormatProvider) invariantCulture, "{0}/{1}_{2}", (object) str2, (object) local, (object) name);
      }
      string fileName = str1;
      IEnumerable<string> source = flag ? taskAttachments.Select<TaskAttachment, string>((Func<TaskAttachment, string>) (a => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}_{2}", (object) jobName, (object) timelineRecord.Order.Value, (object) a.Name))) : taskAttachments.Select<TaskAttachment, string>((Func<TaskAttachment, string>) (a => a.Name));
      List<string> filesToCompare = new List<string>();
      filesToCompare.AddRange(logs.Select<KeyValuePair<int, string>, string>((Func<KeyValuePair<int, string>, string>) (l => l.Value)));
      filesToCompare.AddRange(source.Where<string>((Func<string, bool>) (a => !DistributedTaskOrchestratorExtensions.FileNameMatches(a, fileName))));
      return DistributedTaskOrchestratorExtensions.GetUniqueFilePath(fileName, (IEnumerable<string>) filesToCompare);
    }

    private static string GetUniqueFilePath(string fileName, IEnumerable<string> filesToCompare)
    {
      string directoryName = Path.GetDirectoryName(fileName);
      string withoutExtension = Path.GetFileNameWithoutExtension(fileName);
      string extension = Path.GetExtension(fileName);
      foreach (string fileNameToCompare in filesToCompare)
      {
        if (DistributedTaskOrchestratorExtensions.FileNameMatches(fileName, fileNameToCompare))
        {
          int num = 0;
          while (true)
          {
            do
            {
              if (num != 0)
                goto label_7;
label_4:
              fileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1}){2}", (object) withoutExtension, (object) ++num, (object) extension);
              continue;
label_7:
              if (DistributedTaskOrchestratorExtensions.FileNameExists(fileName, filesToCompare))
                goto label_4;
              else
                goto label_12;
            }
            while (directoryName == null);
            fileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) directoryName, (object) fileName);
          }
        }
      }
label_12:
      return fileName;
    }

    private static bool FileNameExists(string targetFile, IEnumerable<string> filesToCompare)
    {
      foreach (string fileNameToCompare in filesToCompare)
      {
        if (DistributedTaskOrchestratorExtensions.FileNameMatches(targetFile, fileNameToCompare))
          return true;
      }
      return false;
    }

    private static bool FileNameMatches(string fileName, string fileNameToCompare) => string.Compare(fileName, fileNameToCompare, StringComparison.OrdinalIgnoreCase) == 0;
  }
}
