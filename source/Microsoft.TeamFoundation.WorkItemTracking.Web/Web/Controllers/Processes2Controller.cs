// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Processes2Controller
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "processes", ResourceName = "processes", ResourceVersion = 2)]
  [ControllerApiVersion(5.0)]
  public class Processes2Controller : ProcessControllerBase
  {
    public Processes2Controller() => this.SendCustomizationType = true;

    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<ProcessInfo>), null, null)]
    [TraceFilter(5922000, 5922010)]
    [ClientExample("GET__processes.json", "Get the list of processes", null, null)]
    public HttpResponseMessage GetListOfProcesses([FromUri(Name = "$expand")] GetProcessExpandLevel expand = GetProcessExpandLevel.None) => this.GetProcessesUtil(expand);

    [HttpGet]
    [ClientResponseType(typeof (ProcessInfo), null, null)]
    [TraceFilter(5922010, 5922020)]
    [ClientExample("GET__process.json", "Get the specific process", null, null)]
    public HttpResponseMessage GetProcessByItsId(Guid processTypeId, [FromUri(Name = "$expand")] GetProcessExpandLevel expand = GetProcessExpandLevel.None) => this.GetProcessByIdUtil(processTypeId, expand);

    [HttpPost]
    [ClientResponseType(typeof (ProcessInfo), null, null)]
    [TraceFilter(5922020, 5922030)]
    [FeatureEnabled("WebAccess.Process.Hierarchy")]
    [ClientExample("POST__create_process.json", "Create the process", null, null)]
    public HttpResponseMessage CreateNewProcess([FromBody] CreateProcessModel createRequest)
    {
      HttpResponseMessage processUtil = this.CreateProcessUtil(createRequest);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("Process", "Create");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("ProcessName", (object) createRequest.Name);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return processUtil;
    }

    [HttpPatch]
    [ClientResponseType(typeof (ProcessInfo), null, null)]
    [TraceFilter(5922040, 5922050)]
    [ClientExample("PATCH__edit_process.json", "Edit the process", null, null)]
    public HttpResponseMessage EditProcess(Guid processTypeId, [FromBody] UpdateProcessModel updateRequest)
    {
      ProcessDescriptor descriptor;
      int num = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().TryGetProcessDescriptor(this.TfsRequestContext, processTypeId, out descriptor) ? 1 : 0;
      HttpResponseMessage httpResponseMessage = this.UpdateProcessUtil(processTypeId, updateRequest);
      if (num != 0)
      {
        string name1 = descriptor.Name == null ? "" : descriptor.Name;
        string name2 = updateRequest.Name == null ? "" : updateRequest.Name;
        if (name2.IsNullOrEmpty<char>())
        {
          IVssRequestContext tfsRequestContext = this.TfsRequestContext;
          string actionId = ProcessAuditConstants.GetActionId("Process", "EditWithoutNewInformation");
          Dictionary<string, object> data = new Dictionary<string, object>();
          data.Add("OldProcessName", (object) name1);
          Guid targetHostId = new Guid();
          Guid projectId = new Guid();
          tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
        }
        else
        {
          IVssRequestContext tfsRequestContext = this.TfsRequestContext;
          string actionId = ProcessAuditConstants.GetActionId("Process", "Edit");
          Dictionary<string, object> data = new Dictionary<string, object>();
          data.Add("OldProcessName", (object) name1);
          data.Add("NewProcessInformation", (object) name2);
          Guid targetHostId = new Guid();
          Guid projectId = new Guid();
          tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
        }
      }
      return httpResponseMessage;
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [TraceFilter(5922030, 5922040)]
    [ClientExample("DELETE__process.json", "Delete the process", null, null)]
    public HttpResponseMessage DeleteProcessById(Guid processTypeId) => this.DeleteProcessUtil(processTypeId);
  }
}
