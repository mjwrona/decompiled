// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.ClientTraceHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public static class ClientTraceHelper
  {
    private const string c_area = "TaskHub";
    private const string TaskIdKeyName = "TaskId";
    private const string c_euiiMask = "****";
    private const string _credsMask = "$1***";
    private static readonly IEnumerable<string> WellKnownEuiiPatterns = (IEnumerable<string>) new ReadOnlyCollection<string>((IList<string>) new List<string>()
    {
      "\\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\b",
      "[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}",
      "\\b(?:[0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}\\b",
      "\\b((?:[0-9A-Fa-f]{1,4}(?::[0-9A-Fa-f]{1,4})*)?)::((?:[0-9A-Fa-f]{1,4}(?::[0-9A-Fa-f]{1,4})*)?)\\b",
      "\\b((?:[0-9A-Fa-f]{1,4}:){6,6})(25[0-5]|2[0-4]\\d|[0-1]?\\d?\\d)(\\.(25[0-5]|2[0-4]\\d|[0-1]?\\d?\\d)){3}\\b",
      "\\b((?:[0-9A-Fa-f]{1,4}(?::[0-9A-Fa-f]{1,4})*)?) ::((?:[0-9A-Fa-f]{1,4}:)*)(25[0-5]|2[0-4]\\d|[0-1]?\\d?\\d)(\\.(25[0-5]|2[0-4]\\d|[0-1]?\\d?\\d)){3}\\b"
    });
    private static readonly IEnumerable<string> WellKnownCredentialsPatterns = (IEnumerable<string>) new ReadOnlyCollection<string>((IList<string>) new List<string>()
    {
      "(&sig=)[A-Za-z0-9]+",
      "(Bearer )[^\\]\\\\]+",
      "\\b([a-zA-Z0-9_=]+)\\.([a-zA-Z0-9_=]+)\\.([a-zA-Z0-9_\\-\\+\\/=]*)"
    });

    internal static void PublishTaskHubTimelineRecordIssues(
      IVssRequestContext requestContext,
      Guid planId,
      Guid jobId,
      Timeline timeline)
    {
      using (new MethodScope(requestContext, "ClientTrace", nameof (PublishTaskHubTimelineRecordIssues)))
      {
        try
        {
          ClientTraceService service = requestContext.GetService<ClientTraceService>();
          if (!service.IsTracingEnabled(requestContext))
            return;
          foreach (TimelineRecord timelineRecord in timeline.Records.Where<TimelineRecord>((Func<TimelineRecord, bool>) (x =>
          {
            Guid? parentId = x.ParentId;
            Guid guid = jobId;
            return (parentId.HasValue ? (parentId.HasValue ? (parentId.GetValueOrDefault() == guid ? 1 : 0) : 1) : 0) != 0 && string.Equals(x.RecordType, "Task", StringComparison.OrdinalIgnoreCase);
          })))
          {
            ClientTraceData properties = new ClientTraceData();
            properties.Add("JobId", (object) jobId);
            properties.Add("TimelineResult", (object) timelineRecord.Result);
            properties.Add("PlanId", (object) planId);
            properties.Add("TimelineRecordId", (object) timelineRecord.Id);
            DateTime? nullable = timelineRecord.StartTime;
            if (nullable.HasValue)
            {
              nullable = timelineRecord.FinishTime;
              if (nullable.HasValue)
              {
                nullable = timelineRecord.FinishTime;
                DateTime? startTime = timelineRecord.StartTime;
                TimeSpan valueOrDefault = (nullable.HasValue & startTime.HasValue ? new TimeSpan?(nullable.GetValueOrDefault() - startTime.GetValueOrDefault()) : new TimeSpan?()).GetValueOrDefault();
                properties.Add("TimelineDuration", (object) valueOrDefault.TotalSeconds);
              }
            }
            string key = "code";
            List<Dictionary<string, string>> dictionaryList1 = new List<Dictionary<string, string>>();
            foreach (Issue issue in timelineRecord.Issues.Where<Issue>((Func<Issue, bool>) (q => q.Type == IssueType.Error)))
            {
              string str1;
              issue.Data.TryGetValue(key, out str1);
              dictionaryList1.Add(new Dictionary<string, string>()
              {
                {
                  "ErrorCode",
                  str1
                },
                {
                  "ErrorMessage",
                  ClientTraceHelper.MaskSensitiveData(issue.Message)
                }
              });
              string str2;
              if (issue.Data.TryGetValue("TaskId", out str2))
                properties.Add("TaskId", (object) str2);
            }
            properties.Add("Errors", (object) dictionaryList1);
            List<Dictionary<string, string>> dictionaryList2 = new List<Dictionary<string, string>>();
            foreach (Issue issue in timelineRecord.Issues.Where<Issue>((Func<Issue, bool>) (q => q.Type == IssueType.Warning)))
            {
              string str3;
              issue.Data.TryGetValue(key, out str3);
              dictionaryList2.Add(new Dictionary<string, string>()
              {
                {
                  "WarningCode",
                  str3
                },
                {
                  "WarningMessage",
                  ClientTraceHelper.MaskSensitiveData(issue.Message)
                }
              });
              string str4;
              if (issue.Data.TryGetValue("TaskId", out str4))
                properties.Add("TaskId", (object) str4);
            }
            properties.Add("Warnings", (object) dictionaryList2);
            service.Publish(requestContext, "TaskHub", "JobCompleted_TimelineInstanceIssues", properties);
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException("ClientTrace", ex);
        }
      }
    }

    private static string MaskSensitiveData(string message)
    {
      try
      {
        foreach (string knownEuiiPattern in ClientTraceHelper.WellKnownEuiiPatterns)
          message = Regex.Replace(message, knownEuiiPattern, "****", RegexOptions.Compiled, TimeSpan.FromSeconds(3.0));
        foreach (string credentialsPattern in ClientTraceHelper.WellKnownCredentialsPatterns)
          message = Regex.Replace(message, credentialsPattern, "$1***", RegexOptions.Compiled, TimeSpan.FromSeconds(3.0));
        return message;
      }
      catch (RegexMatchTimeoutException ex)
      {
        return string.Empty;
      }
    }
  }
}
