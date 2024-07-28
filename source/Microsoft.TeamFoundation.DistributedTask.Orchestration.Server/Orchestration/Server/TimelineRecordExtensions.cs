// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TimelineRecordExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public static class TimelineRecordExtensions
  {
    private const string VMImage = "vmImage";

    internal static void AddIssue(this TimelineRecord record, IssueType issueType, string message)
    {
      if (record == null)
        return;
      if (issueType == IssueType.Error)
        record.ErrorCount = new int?(record.ErrorCount.GetValueOrDefault() + 1);
      else
        record.WarningCount = new int?(record.WarningCount.GetValueOrDefault() + 1);
      record.Issues.Add(new Issue()
      {
        Message = message,
        Type = issueType
      });
    }

    internal static void UpdateLocations(
      this TimelineRecord record,
      IVssRequestContext requestContext,
      Guid planId,
      Guid timelineId)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      record.Location = service.GetResourceUri(requestContext, "distributedtask", TaskResourceIds.TimelineRecords_Compat, (object) new
      {
        planId = planId,
        timelineId = timelineId,
        recordId = record.Id
      });
      if (record.Details != null)
        record.Details.Location = service.GetResourceUri(requestContext, "distributedtask", TaskResourceIds.Timelines_Compat, (object) new
        {
          planId = planId,
          timelineId = timelineId
        });
      if (record.Log == null)
        return;
      record.Log.Location = service.GetResourceUri(requestContext, "distributedtask", TaskResourceIds.Logs_Compat, (object) new
      {
        planId = planId,
        logId = record.Log.Id
      });
    }

    public static string GetImageName(this TimelineRecord record)
    {
      JObject agentSpecification = record.AgentSpecification;
      if (agentSpecification == null)
        return (string) null;
      JToken jtoken = agentSpecification.GetValue("vmImage", StringComparison.OrdinalIgnoreCase);
      return jtoken == null ? (string) null : jtoken.Value<string>();
    }
  }
}
