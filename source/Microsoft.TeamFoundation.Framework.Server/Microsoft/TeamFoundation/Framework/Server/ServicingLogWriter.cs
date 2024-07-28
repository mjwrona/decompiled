// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingLogWriter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class ServicingLogWriter
  {
    private const string c_stepSeparator = "+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++";
    private const string c_summarySeparator = "======================================================================================================";
    private static readonly char[] s_CRLF = new char[2]
    {
      '\r',
      '\n'
    };

    public static void WriteLogToStream(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid jobId,
      StreamWriter streamWriter)
    {
      ServicingJobDetail jobDetails;
      List<ServicingStepDetail> servicingDetails = requestContext.GetService<TeamFoundationServicingService>().GetServicingDetails(requestContext, hostId, jobId, ServicingStepDetailFilterOptions.AllStepDetails, out jobDetails);
      if (TeamFoundationServicingService.IsPseudoHostId(jobDetails.HostId))
        new DatabaseLogWriter(TeamFoundationServicingService.GetDatabaseId(jobDetails.HostId)).Write(jobDetails, servicingDetails, streamWriter);
      else
        new CollectionLogWriter().Write(jobDetails, servicingDetails, streamWriter);
    }

    public static List<ServicingLogEntry> GetStructuredLog(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid jobId)
    {
      ServicingJobDetail jobDetails;
      List<ServicingStepDetail> servicingDetails = requestContext.GetService<TeamFoundationServicingService>().GetServicingDetails(requestContext, hostId, jobId, ServicingStepDetailFilterOptions.AllStepDetails, out jobDetails);
      List<ServicingLogEntry> entryList = new List<ServicingLogEntry>();
      StructuredListLogEntryHandler handler = new StructuredListLogEntryHandler((IList<ServicingLogEntry>) entryList);
      if (TeamFoundationServicingService.IsPseudoHostId(jobDetails.HostId))
      {
        new DatabaseLogWriter(TeamFoundationServicingService.GetDatabaseId(jobDetails.HostId)).Write(jobDetails, servicingDetails, (IServicingLogEntryHandler) handler);
        return entryList;
      }
      new CollectionLogWriter().Write(jobDetails, servicingDetails, (IServicingLogEntryHandler) handler);
      return entryList;
    }

    public void Write(
      ServicingJobDetail servicingJobDetail,
      List<ServicingStepDetail> stepDetails,
      StreamWriter streamWriter)
    {
      if (stepDetails == null || servicingJobDetail.QueueTime == DateTime.MinValue)
        throw new InvalidOperationException();
      StreamWriterLogEntryHandler logHandler = new StreamWriterLogEntryHandler(streamWriter);
      this.Write(servicingJobDetail.OperationClass, servicingJobDetail.QueueTime, servicingJobDetail.StartTime, servicingJobDetail.EndTime, servicingJobDetail.Result, stepDetails, (IServicingLogEntryHandler) logHandler);
    }

    public void Write(
      ServicingJobDetail servicingJobDetail,
      List<ServicingStepDetail> stepDetails,
      IServicingLogEntryHandler handler)
    {
      if (stepDetails == null || servicingJobDetail.QueueTime == DateTime.MinValue)
        throw new InvalidOperationException();
      this.Write(servicingJobDetail.OperationClass, servicingJobDetail.QueueTime, servicingJobDetail.StartTime, servicingJobDetail.EndTime, servicingJobDetail.Result, stepDetails, handler);
    }

    public string Write(
      ServicingJobDetail servicingJobDetail,
      List<ServicingStepDetail> stepDetails,
      string fileName)
    {
      return this.Write(servicingJobDetail.OperationClass, servicingJobDetail.QueueTime, servicingJobDetail.StartTime, servicingJobDetail.EndTime, servicingJobDetail.Result, stepDetails, fileName);
    }

    public string Write(
      string operationClass,
      DateTime queueTime,
      DateTime startTime,
      DateTime endTime,
      ServicingJobResult jobResult,
      List<ServicingStepDetail> stepDetails,
      string fileName)
    {
      if (stepDetails == null || queueTime == DateTime.MinValue)
        throw new InvalidOperationException();
      using (StreamWriter text = File.CreateText(fileName))
      {
        StreamWriterLogEntryHandler logHandler = new StreamWriterLogEntryHandler(text);
        this.Write(operationClass, queueTime, startTime, endTime, jobResult, stepDetails, (IServicingLogEntryHandler) logHandler);
        text.Flush();
      }
      return fileName;
    }

    public void Write(
      string operationClass,
      DateTime queueTime,
      DateTime startTime,
      DateTime endTime,
      ServicingJobResult jobResult,
      List<ServicingStepDetail> stepDetails,
      IServicingLogEntryHandler logHandler)
    {
      this.WriteHeader(operationClass, queueTime, startTime, endTime, jobResult, logHandler);
      List<ServicingLogWriter.StepExecutionTime> executionTimes = new List<ServicingLogWriter.StepExecutionTime>();
      string a1 = (string) null;
      string a2 = (string) null;
      ServicingStepDetail servicingStepDetail1 = (ServicingStepDetail) null;
      ServicingStepDetail servicingStepDetail2 = (ServicingStepDetail) null;
      for (int index = 0; index < stepDetails.Count; ++index)
      {
        ServicingStepDetail stepDetail = stepDetails[index];
        if (!string.Equals(a2, stepDetail.ServicingStepId, StringComparison.Ordinal))
          servicingStepDetail1 = stepDetail;
        ServicingStepStateChange servicingStepStateChange1 = stepDetail as ServicingStepStateChange;
        ServicingStepLogEntry servicingStepLogEntry = stepDetail as ServicingStepLogEntry;
        if (servicingStepStateChange1 != null)
        {
          if (!string.Equals(a1, stepDetail.ServicingStepGroupId, StringComparison.Ordinal))
            ServicingLogWriter.WriteToLog(logHandler, ServicingLogEntryType.Info, stepDetail.DetailTime, "{0}\r\n++ Executing - Operation: {1}, Group: {2}", "+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++", stepDetail.ServicingOperation, stepDetail.ServicingStepGroupId);
          switch (servicingStepStateChange1.StepState)
          {
            case ServicingStepState.Executing:
              ServicingLogWriter.WriteToLog(logHandler, ServicingLogEntryType.Info, stepDetail.DetailTime, "{0}\r\nExecuting step: {1}", "+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++", stepDetail.ServicingStepId);
              break;
            case ServicingStepState.Failed:
              ServicingLogWriter.WriteToLog(logHandler, ServicingLogEntryType.Error, stepDetail.DetailTime, "Step failed: {0}. Execution time: {1}.", stepDetail.ServicingStepId, ServicingLogWriter.FormatExecutionTime(stepDetail.DetailTime - servicingStepDetail1.DetailTime));
              ServicingLogWriter.StepExecutionTime stepExecutionTime1 = new ServicingLogWriter.StepExecutionTime(stepDetail.ServicingStepId, stepDetail.ServicingStepGroupId, stepDetail.ServicingOperation, stepDetail.DetailTime - servicingStepDetail1.DetailTime);
              executionTimes.Add(stepExecutionTime1);
              break;
            case ServicingStepState.Skipped:
              if (!(servicingStepDetail2 is ServicingStepStateChange servicingStepStateChange2) || servicingStepStateChange2.StepState != ServicingStepState.Skipped)
                ServicingLogWriter.WriteToLog(logHandler, ServicingLogEntryType.Info, stepDetail.DetailTime, "+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
              ServicingLogWriter.WriteToLog(logHandler, ServicingLogEntryType.Info, stepDetail.DetailTime, "Step skipped: {0}.", stepDetail.ServicingStepId);
              break;
            case ServicingStepState.Passed:
              ServicingLogWriter.WriteToLog(logHandler, ServicingLogEntryType.Info, stepDetail.DetailTime, "Step passed: {0}. Execution time: {1}.", stepDetail.ServicingStepId, ServicingLogWriter.FormatExecutionTime(stepDetail.DetailTime - servicingStepDetail1.DetailTime));
              ServicingLogWriter.StepExecutionTime stepExecutionTime2 = new ServicingLogWriter.StepExecutionTime(stepDetail.ServicingStepId, stepDetail.ServicingStepGroupId, stepDetail.ServicingOperation, stepDetail.DetailTime - servicingStepDetail1.DetailTime);
              executionTimes.Add(stepExecutionTime2);
              break;
            case ServicingStepState.PassedWithWarnings:
              ServicingLogWriter.WriteToLog(logHandler, ServicingLogEntryType.Warning, stepDetail.DetailTime, "Step passed (with warnings): {0}. Execution time: {1}.", stepDetail.ServicingStepId, ServicingLogWriter.FormatExecutionTime(stepDetail.DetailTime - servicingStepDetail1.DetailTime));
              ServicingLogWriter.StepExecutionTime stepExecutionTime3 = new ServicingLogWriter.StepExecutionTime(stepDetail.ServicingStepId, stepDetail.ServicingStepGroupId, stepDetail.ServicingOperation, stepDetail.DetailTime - servicingStepDetail1.DetailTime);
              executionTimes.Add(stepExecutionTime3);
              break;
            case ServicingStepState.PassedWithSkipChildren:
              ServicingLogWriter.WriteToLog(logHandler, ServicingLogEntryType.Info, stepDetail.DetailTime, "Step passed (with skip children): {0}. Execution time: {1}.", stepDetail.ServicingStepId, ServicingLogWriter.FormatExecutionTime(stepDetail.DetailTime - servicingStepDetail1.DetailTime));
              ServicingLogWriter.StepExecutionTime stepExecutionTime4 = new ServicingLogWriter.StepExecutionTime(stepDetail.ServicingStepId, stepDetail.ServicingStepGroupId, stepDetail.ServicingOperation, stepDetail.DetailTime - servicingStepDetail1.DetailTime);
              executionTimes.Add(stepExecutionTime4);
              break;
          }
        }
        else if (servicingStepLogEntry != null)
        {
          string text;
          switch (servicingStepLogEntry.EntryKind)
          {
            case ServicingStepLogEntryKind.Informational:
              text = servicingStepLogEntry.Message;
              break;
            case ServicingStepLogEntryKind.Warning:
              text = "[Warning] " + servicingStepLogEntry.Message;
              break;
            case ServicingStepLogEntryKind.Error:
              text = "[Error] " + servicingStepLogEntry.Message;
              break;
            case ServicingStepLogEntryKind.StepDuration:
              text = "[StepDuration] " + servicingStepLogEntry.Message;
              break;
            case ServicingStepLogEntryKind.GroupDuration:
              text = "[GroupDuration] " + servicingStepLogEntry.Message;
              break;
            case ServicingStepLogEntryKind.OperationDuration:
              text = "[OperationDuration] " + servicingStepLogEntry.Message;
              break;
            case ServicingStepLogEntryKind.SleepDuration:
              text = "[SleepDuration] " + servicingStepLogEntry.Message;
              break;
            default:
              text = servicingStepLogEntry.Message;
              break;
          }
          ServicingLogWriter.WriteToLog(logHandler, ServicingLogEntry.FromEntryKind(servicingStepLogEntry.EntryKind), "  ", stepDetail.DetailTime, text);
        }
        servicingStepDetail2 = stepDetail;
        a2 = stepDetail.ServicingStepId;
        a1 = stepDetail.ServicingStepGroupId;
      }
      ServicingLogWriter.WriteSummary(executionTimes, logHandler);
    }

    protected abstract void WriteHeader(
      string operationClass,
      DateTime queueTime,
      DateTime startTime,
      DateTime endTime,
      ServicingJobResult jobResult,
      IServicingLogEntryHandler logHandler);

    private static void WriteSummary(
      List<ServicingLogWriter.StepExecutionTime> executionTimes,
      IServicingLogEntryHandler logHandler)
    {
      if (!executionTimes.Any<ServicingLogWriter.StepExecutionTime>())
        return;
      logHandler.HandleEntry(ServicingLogWriter.StringToInfo(""));
      logHandler.HandleEntry(ServicingLogWriter.StringToInfo("======================================================================================================"));
      if (executionTimes.Count > 40)
        logHandler.HandleEntry(new ServicingLogEntry(ServicingLogEntryType.Info, " Step execution times in descending order (top 40 steps)"));
      else
        logHandler.HandleEntry(new ServicingLogEntry(ServicingLogEntryType.Info, " Step execution times in descending order"));
      logHandler.HandleEntry(ServicingLogWriter.StringToInfo("======================================================================================================"));
      executionTimes.Sort((Comparison<ServicingLogWriter.StepExecutionTime>) ((t1, t2) => t2.ExecutionTime.CompareTo(t1.ExecutionTime)));
      List<ServicingLogWriter.StepExecutionTime> list1 = executionTimes.Take<ServicingLogWriter.StepExecutionTime>(40).ToList<ServicingLogWriter.StepExecutionTime>();
      int num = list1.Max<ServicingLogWriter.StepExecutionTime>((Func<ServicingLogWriter.StepExecutionTime, int>) (executionTime => Math.Min(executionTime.Step.Length, 60) + executionTime.Group.Length + executionTime.Operation.Length + 5));
      string format1 = "{0,-" + num.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "} - {1}";
      foreach (ServicingLogWriter.StepExecutionTime stepExecutionTime in list1)
      {
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} ({1}, {2})", stepExecutionTime.Step.Length < 60 ? (object) stepExecutionTime.Step : (object) (stepExecutionTime.Step.Substring(0, 57) + "..."), (object) stepExecutionTime.Group, (object) stepExecutionTime.Operation);
        logHandler.HandleEntry(ServicingLogWriter.FormatInfo(format1, (object) str, (object) ServicingLogWriter.FormatExecutionTime(stepExecutionTime.ExecutionTime)));
      }
      IOrderedEnumerable<\u003C\u003Ef__AnonymousType119<string, TimeSpan>> source = executionTimes.GroupBy<ServicingLogWriter.StepExecutionTime, string>((Func<ServicingLogWriter.StepExecutionTime, string>) (et => et.Group + " (" + et.Operation + ")")).Select(group => new
      {
        Group = group.Key,
        ExecutionTime = TimeSpan.FromMilliseconds(group.Sum<ServicingLogWriter.StepExecutionTime>((Func<ServicingLogWriter.StepExecutionTime, double>) (s => s.ExecutionTime.TotalMilliseconds)))
      }).OrderByDescending(g => g.ExecutionTime);
      logHandler.HandleEntry(ServicingLogWriter.StringToInfo(""));
      logHandler.HandleEntry(ServicingLogWriter.StringToInfo("======================================================================================================"));
      if (source.Count() > 40)
        logHandler.HandleEntry(new ServicingLogEntry(ServicingLogEntryType.Info, " Execution times by group in descending order (top 40 step groups)"));
      else
        logHandler.HandleEntry(new ServicingLogEntry(ServicingLogEntryType.Info, " Execution times by group in descending order"));
      logHandler.HandleEntry(ServicingLogWriter.StringToInfo("======================================================================================================"));
      List<\u003C\u003Ef__AnonymousType119<string, TimeSpan>> list2 = source.Take(40).ToList();
      string format2 = "{0,-" + num.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "} - {1}";
      foreach (var data in list2)
      {
        string message = string.Format(format2, (object) data.Group, (object) ServicingLogWriter.FormatExecutionTime(data.ExecutionTime));
        logHandler.HandleEntry(new ServicingLogEntry(ServicingLogEntryType.Info, message));
      }
    }

    private static void WriteToLog(
      IServicingLogEntryHandler handler,
      ServicingLogEntryType logType,
      DateTime detailTime,
      string format,
      params string[] args)
    {
      ServicingLogWriter.WriteToLog(handler, logType, "", detailTime, format, args);
    }

    private static void WriteToLog(
      IServicingLogEntryHandler handler,
      ServicingLogEntryType logType,
      string indent,
      DateTime detailTime,
      string format,
      params string[] args)
    {
      string text = string.Format((IFormatProvider) CultureInfo.CurrentUICulture, format, (object[]) args);
      ServicingLogWriter.WriteToLog(handler, logType, indent, detailTime, text);
    }

    private static void WriteToLog(
      IServicingLogEntryHandler handler,
      ServicingLogEntryType logType,
      string indent,
      DateTime detailTime,
      string text)
    {
      string str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0:HH\\:mm\\:ss\\.fff}] {1}", (object) detailTime.ToLocalTime(), (object) indent);
      foreach (string str2 in text.Split(ServicingLogWriter.s_CRLF, StringSplitOptions.RemoveEmptyEntries))
        handler.HandleEntry(new ServicingLogEntry(logType, str1 + str2));
    }

    protected static ServicingLogEntry StringToInfo(string s) => new ServicingLogEntry(ServicingLogEntryType.Info, s);

    protected static ServicingLogEntry FormatInfo(string format, params object[] args) => new ServicingLogEntry(ServicingLogEntryType.Info, string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args));

    private static string FormatExecutionTime(TimeSpan timeSpan)
    {
      string str;
      if (timeSpan.TotalMilliseconds < 1.0)
        str = "1 millisecond";
      else if (timeSpan.TotalMilliseconds < 2000.0)
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} milliseconds", (object) (int) timeSpan.TotalMilliseconds);
      else if (timeSpan.TotalSeconds < 120.0)
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} seconds", (object) (int) timeSpan.TotalSeconds);
      else if (timeSpan.TotalMinutes < 60.0)
      {
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} minutes and {1} {2}", (object) (int) timeSpan.TotalMinutes, (object) timeSpan.Seconds, timeSpan.Seconds == 1 ? (object) "second" : (object) "seconds");
      }
      else
      {
        int totalHours = (int) timeSpan.TotalHours;
        int minutes = timeSpan.Minutes;
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1} and {2} {3}", (object) totalHours, totalHours > 1 ? (object) "hours" : (object) "hour", (object) minutes, minutes != 1 ? (object) "minutes" : (object) "minute");
      }
      return str;
    }

    [DebuggerDisplay("Step: {0}, Group: {1}, Operation: {2}, Execution Time: {3}")]
    private sealed class StepExecutionTime
    {
      public StepExecutionTime(
        string step,
        string group,
        string operation,
        TimeSpan executionTime)
      {
        this.Step = step;
        this.Group = group;
        this.Operation = operation;
        this.ExecutionTime = executionTime;
      }

      public string Step { get; private set; }

      public string Group { get; private set; }

      public string Operation { get; private set; }

      public TimeSpan ExecutionTime { get; private set; }
    }
  }
}
