// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemFieldAllowedValuesController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [ValidateModel]
  [ControllerApiVersion(7.1)]
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workItemFieldAllowedValues", ResourceVersion = 1)]
  public class WorkItemFieldAllowedValuesController : WorkItemTrackingApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (WorkItemFieldAllowedValues), null, null)]
    [ClientInternalUseOnly(false)]
    public HttpResponseMessage GetWorkItemFieldAllowedValues([FromUri(Name = "name")] string name)
    {
      if (string.IsNullOrEmpty(name))
        throw new VssPropertyValidationException(nameof (name), ResourceStrings.NullOrEmptyParameter((object) nameof (name)));
      IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType> workItemTypes = this.WitRequestContext.RequestContext.GetService<IWorkItemTypeService>().GetWorkItemTypes(this.WitRequestContext.RequestContext, this.ProjectId);
      IWorkItemTrackingProcessService service = this.WitRequestContext.RequestContext.GetService<IWorkItemTrackingProcessService>();
      ProcessDescriptor processDescriptor = (ProcessDescriptor) null;
      if (this.ProjectId != Guid.Empty)
        service.TryGetProjectProcessDescriptor(this.WitRequestContext.RequestContext, this.ProjectId, out processDescriptor);
      string[] array = workItemTypes != null ? workItemTypes.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType, string>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType, string>) (wType => wType.Name)).ToArray<string>() : (string[]) null;
      FieldEntry fieldByNameOrId = this.WitRequestContext.FieldDictionary.GetFieldByNameOrId(name);
      string[] strArray = WorkItemTypeFieldInstanceFactory.GeFieldAllowedValues(this.WitRequestContext, fieldByNameOrId, array, processDescriptor, this.ProjectId, false);
      return this.Request.CreateResponse<WorkItemFieldAllowedValues>(HttpStatusCode.OK, new WorkItemFieldAllowedValues()
      {
        FieldName = fieldByNameOrId.Name,
        AllowedValues = strArray
      });
    }
  }
}
