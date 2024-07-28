// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessControllerBase
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common.ExpandResults;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  public abstract class ProcessControllerBase : WorkItemTrackingApiController
  {
    protected const int TraceRange = 5922000;

    protected bool SendCustomizationType { set; get; }

    [ClientIgnore]
    internal HttpResponseMessage GetProcessesUtil([FromUri(Name = "$expand")] GetProcessExpandLevel expand = GetProcessExpandLevel.None)
    {
      IReadOnlyCollection<ProcessDescriptor> processDescriptors = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptors(this.TfsRequestContext);
      GetProcessesExpandResult expandResult = new GetProcessesExpandResult(this.TfsRequestContext, expand);
      return this.SendCustomizationType ? this.Request.CreateResponse<IEnumerable<ProcessInfo>>(HttpStatusCode.OK, processDescriptors.Select<ProcessDescriptor, ProcessInfo>((Func<ProcessDescriptor, ProcessInfo>) (x => ProcessModelFactory.CreateProcessInfo(this.TfsRequestContext, x.TypeId, expandResult)))) : this.Request.CreateResponse<IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessModel>>(HttpStatusCode.OK, processDescriptors.Select<ProcessDescriptor, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessModel>((Func<ProcessDescriptor, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessModel>) (x => ProcessModelFactory.Create(this.TfsRequestContext, x.TypeId, expandResult))));
    }

    [ClientIgnore]
    internal HttpResponseMessage GetProcessByIdUtil(
      Guid processTypeId,
      [FromUri(Name = "$expand")] GetProcessExpandLevel expand = GetProcessExpandLevel.None)
    {
      if (processTypeId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processTypeId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processTypeId)));
      GetProcessesExpandResult expandResult = new GetProcessesExpandResult(this.TfsRequestContext, expand, new Guid?(processTypeId));
      return this.SendCustomizationType ? this.Request.CreateResponse<ProcessInfo>(HttpStatusCode.OK, ProcessModelFactory.CreateProcessInfo(this.TfsRequestContext, processTypeId, expandResult)) : this.Request.CreateResponse<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessModel>(HttpStatusCode.OK, ProcessModelFactory.Create(this.TfsRequestContext, processTypeId, expandResult));
    }

    [ClientIgnore]
    internal HttpResponseMessage DeleteProcessUtil(Guid processTypeId)
    {
      if (processTypeId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processTypeId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processTypeId)));
      ITeamFoundationProcessService service = this.TfsRequestContext.GetService<ITeamFoundationProcessService>();
      ProcessDescriptor descriptor;
      bool processDescriptor = this.TfsRequestContext.GetService<ITeamFoundationProcessService>().TryGetProcessDescriptor(this.TfsRequestContext, processTypeId, out descriptor);
      try
      {
        service.DeleteProcess(this.TfsRequestContext, processTypeId);
      }
      catch (ProcessNotFoundByTypeIdException ex)
      {
        return this.Request.CreateResponse(HttpStatusCode.NoContent);
      }
      if (processDescriptor)
      {
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        string actionId = ProcessAuditConstants.GetActionId("Process", "Delete");
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("ProcessName", (object) descriptor?.Name);
        Guid targetHostId = new Guid();
        Guid projectId = new Guid();
        tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      }
      return this.Request.CreateResponse(HttpStatusCode.Accepted);
    }

    [ClientIgnore]
    internal HttpResponseMessage CreateProcessUtil([FromBody] CreateProcessModel createRequest)
    {
      if (createRequest == null)
        throw new VssPropertyValidationException(nameof (createRequest), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (createRequest)));
      if (string.IsNullOrWhiteSpace(createRequest.Name))
        throw new VssPropertyValidationException("Name", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) "Name"));
      if (createRequest.ParentProcessTypeId == Guid.Empty)
        throw new VssPropertyValidationException("ParentProcessTypeId", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) "ParentProcessTypeId"));
      ProcessDescriptor inheritedProcess = this.TfsRequestContext.GetService<IWorkItemTrackingProcessService>().CreateInheritedProcess(this.TfsRequestContext, createRequest.Name, createRequest.ReferenceName, createRequest.Description, createRequest.ParentProcessTypeId);
      GetProcessesExpandResult expandResult = new GetProcessesExpandResult(this.TfsRequestContext, GetProcessExpandLevel.None, new Guid?(inheritedProcess.TypeId));
      return this.SendCustomizationType ? this.Request.CreateResponse<ProcessInfo>(HttpStatusCode.Created, ProcessModelFactory.CreateProcessInfo(this.TfsRequestContext, inheritedProcess.TypeId, expandResult)) : this.Request.CreateResponse<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessModel>(HttpStatusCode.Created, ProcessModelFactory.Create(this.TfsRequestContext, inheritedProcess.TypeId, expandResult));
    }

    [ClientIgnore]
    internal HttpResponseMessage UpdateProcessUtil(
      Guid processTypeId,
      [FromBody] UpdateProcessModel updateRequest)
    {
      if (processTypeId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processTypeId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (processTypeId)));
      if (updateRequest == null)
        throw new VssPropertyValidationException(nameof (updateRequest), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (updateRequest)));
      ITeamFoundationProcessService service1 = this.TfsRequestContext.GetService<ITeamFoundationProcessService>();
      IWorkItemTrackingProcessService service2 = this.TfsRequestContext.GetService<IWorkItemTrackingProcessService>();
      bool flag1 = false;
      if (updateRequest.IsDefault.HasValue)
      {
        bool? isDefault = updateRequest.IsDefault;
        bool flag2 = true;
        if (isDefault.GetValueOrDefault() == flag2 & isDefault.HasValue)
        {
          service1.SetProcessAsDefault(this.TfsRequestContext, processTypeId);
          flag1 = true;
        }
      }
      bool? isEnabled = updateRequest.IsEnabled;
      if (isEnabled.HasValue)
      {
        IWorkItemTrackingProcessService trackingProcessService = service2;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        Guid processTypeId1 = processTypeId;
        isEnabled = updateRequest.IsEnabled;
        int num = isEnabled.Value ? 1 : 0;
        trackingProcessService.EnableDisableProcess(tfsRequestContext, processTypeId1, num != 0);
        flag1 = true;
      }
      try
      {
        service2.UpdateProcessNameAndDescription(this.TfsRequestContext, processTypeId, updateRequest.Name, updateRequest.Description);
      }
      catch (ProcessInvalidInheritedProcessUpdateInputException ex)
      {
        if (!flag1)
          throw;
      }
      GetProcessesExpandResult expandResult = new GetProcessesExpandResult(this.TfsRequestContext, GetProcessExpandLevel.None, new Guid?(processTypeId));
      return this.SendCustomizationType ? this.Request.CreateResponse<ProcessInfo>(HttpStatusCode.OK, ProcessModelFactory.CreateProcessInfo(this.TfsRequestContext, processTypeId, expandResult)) : this.Request.CreateResponse<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessModel>(HttpStatusCode.OK, ProcessModelFactory.Create(this.TfsRequestContext, processTypeId, expandResult));
    }
  }
}
