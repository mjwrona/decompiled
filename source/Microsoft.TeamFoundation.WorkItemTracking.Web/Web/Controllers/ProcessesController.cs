// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessesController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "processes", ResourceName = "processes", ResourceVersion = 1)]
  public class ProcessesController : ProcessControllerBase
  {
    public ProcessesController() => this.SendCustomizationType = false;

    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessModel>), null, null)]
    [TraceFilter(5922000, 5922010)]
    [ClientExample("GET__processes.json", "Get the list of processes", null, null)]
    public HttpResponseMessage GetProcesses([FromUri(Name = "$expand")] GetProcessExpandLevel expand = GetProcessExpandLevel.None) => this.GetProcessesUtil(expand);

    [HttpGet]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessModel), null, null)]
    [TraceFilter(5922010, 5922020)]
    [ClientExample("GET__process.json", "Get the specific process definition", null, null)]
    public HttpResponseMessage GetProcessById(Guid processTypeId, [FromUri(Name = "$expand")] GetProcessExpandLevel expand = GetProcessExpandLevel.None) => this.GetProcessByIdUtil(processTypeId, expand);

    [HttpPost]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessModel), null, null)]
    [TraceFilter(5922020, 5922030)]
    [FeatureEnabled("WebAccess.Process.Hierarchy")]
    [ClientExample("POST__create_process.json", "Create the process", null, null)]
    public HttpResponseMessage CreateProcess([FromBody] CreateProcessModel createRequest)
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
    [ClientResponseType(typeof (Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessModel), null, null)]
    [TraceFilter(5922040, 5922050)]
    [ClientExample("PATCH__update_process.json", "Update the process", null, null)]
    public HttpResponseMessage UpdateProcess(Guid processTypeId, [FromBody] UpdateProcessModel updateRequest)
    {
      ProcessDescriptor processDescriptor;
      this.TfsRequestContext.GetService<IWorkItemTrackingProcessService>().TryGetProjectProcessDescriptor(this.TfsRequestContext, this.ProjectId, out processDescriptor);
      string str = string.IsNullOrEmpty(processDescriptor?.Name) ? string.Empty : processDescriptor.Name;
      string enumerable = string.Empty;
      if (!string.IsNullOrEmpty(updateRequest.Name))
        enumerable = updateRequest.Name;
      HttpResponseMessage httpResponseMessage = this.UpdateProcessUtil(processTypeId, updateRequest);
      if (enumerable.IsNullOrEmpty<char>())
      {
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        string actionId = ProcessAuditConstants.GetActionId("Process", "EditWithoutNewInformation");
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("OldProcessName", (object) str);
        Guid targetHostId = new Guid();
        Guid projectId = new Guid();
        tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
        return httpResponseMessage;
      }
      IVssRequestContext tfsRequestContext1 = this.TfsRequestContext;
      string actionId1 = ProcessAuditConstants.GetActionId("Process", "Edit");
      Dictionary<string, object> data1 = new Dictionary<string, object>();
      data1.Add("OldProcessName", (object) str);
      data1.Add("NewProcessInformation", (object) enumerable);
      Guid targetHostId1 = new Guid();
      Guid projectId1 = new Guid();
      tfsRequestContext1.LogAuditEvent(actionId1, data1, targetHostId1, projectId1);
      return httpResponseMessage;
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [TraceFilter(5922030, 5922040)]
    [ClientExample("DELETE__process.json", "Delete the process", null, null)]
    public HttpResponseMessage DeleteProcess(Guid processTypeId) => this.DeleteProcessUtil(processTypeId);
  }
}
