// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Diagnostics.DiagnosticExportResult
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Diagnostics, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6A4F2FF9-BE93-434B-9864-FE0D09D21D75
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Diagnostics.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Diagnostics
{
  public class DiagnosticExportResult : FileResult
  {
    private int s_bufferSize = 32768;

    public DiagnosticExportResult(
      IVssRequestContext tfsRequestContext,
      Guid instanceId,
      string userName)
      : base("text/csv")
    {
      this.UserName = userName;
      this.InstanceId = instanceId;
      this.RequestContext = tfsRequestContext;
      this.FileDownloadName = "ActivityLogEntries.csv";
    }

    public IVssRequestContext RequestContext { get; private set; }

    public string UserName { get; private set; }

    public Guid InstanceId { get; private set; }

    protected override void WriteFile(HttpResponseBase response)
    {
      if (response == null)
        return;
      TeamFoundationHostManagementService service1 = this.RequestContext.Elevate().To(TeamFoundationHostType.Deployment).GetService<TeamFoundationHostManagementService>();
      List<KeyValuePair<ActivityLogColumns, SortOrder>> sortColumns = new List<KeyValuePair<ActivityLogColumns, SortOrder>>();
      sortColumns.Add(new KeyValuePair<ActivityLogColumns, SortOrder>(ActivityLogColumns.CommandId, SortOrder.Descending));
      IVssRequestContext requestContext = this.RequestContext.Elevate();
      Guid instanceId = this.InstanceId;
      using (IVssRequestContext vssRequestContext = service1.BeginRequest(requestContext, instanceId, RequestContextType.UserContext, true, true))
      {
        TeamFoundationActivityLogService service2 = vssRequestContext.GetService<TeamFoundationActivityLogService>();
        IEnumerable<int> source = service2.QueryActivityLogIds(vssRequestContext, this.UserName, 1000000, sortColumns);
        IEnumerable<ActivityLogEntry> activityLogEntries = service2.QueryActivityLogEntries(vssRequestContext, source.ToArray<int>());
        using (Stream stream = (Stream) File.Create(Path.Combine(FileSpec.GetTempDirectory(), Guid.NewGuid().ToString("N")), this.s_bufferSize, FileOptions.DeleteOnClose))
        {
          using (StreamWriter streamWriter = new StreamWriter(stream))
          {
            streamWriter.WriteLine("Id,Application,Command,Status,StartTime,ExecutionTime,IdentityName,IpAddress,UniqueIdentifier,UserAgent,CommandIdentifier,ExecutionCount,AuthenticationType,ResponseCode,TimeToFirstPage");
            foreach (ActivityLogEntry activityLogEntry in activityLogEntries)
              streamWriter.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}", (object) activityLogEntry.CommandId, (object) this.EscapeForCsv(activityLogEntry.Application), (object) this.EscapeForCsv(activityLogEntry.Command), (object) activityLogEntry.Status, (object) activityLogEntry.StartTime.ToString("dd/MM/yyyy HH:mm:ss.f"), (object) activityLogEntry.ExecutionTime, (object) this.EscapeForCsv(activityLogEntry.IdentityName), (object) this.EscapeForCsv(activityLogEntry.IpAddress), (object) activityLogEntry.UniqueIdentifier, (object) this.EscapeForCsv(activityLogEntry.UserAgent), (object) this.EscapeForCsv(activityLogEntry.CommandIdentifier), (object) activityLogEntry.ExecutionCount, (object) this.EscapeForCsv(activityLogEntry.AuthenticationType), (object) activityLogEntry.ResponseCode, (object) activityLogEntry.TimeToFirstPage);
            streamWriter.Flush();
            stream.Seek(0L, SeekOrigin.Begin);
            response.AppendHeader("Content-Length", stream.Length.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            stream.CopyTo(response.OutputStream);
          }
        }
      }
    }

    private string EscapeForCsv(string val) => string.Format("\"{0}\"", (object) val.Replace("\"", "\"\""));
  }
}
