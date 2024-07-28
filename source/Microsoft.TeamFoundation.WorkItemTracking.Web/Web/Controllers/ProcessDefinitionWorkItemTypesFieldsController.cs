// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessDefinitionWorkItemTypesFieldsController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [FeatureEnabled("WebAccess.Process.Hierarchy")]
  [VersionedApiControllerCustomName(Area = "processDefinitions", ResourceName = "workItemTypesFields", ResourceVersion = 1)]
  [ControllerApiVersion(2.1)]
  public class ProcessDefinitionWorkItemTypesFieldsController : 
    ProcessDefinitionWorkItemTypesFieldsControllerBase
  {
    [HttpPost]
    [ClientResponseType(typeof (WorkItemTypeFieldModel), null, null)]
    [ClientLocationId("976713B4-A62E-499E-94DC-EEB869EA9126")]
    public HttpResponseMessage AddFieldToWorkItemType(
      Guid processId,
      string witRefNameForFields,
      [FromBody] WorkItemTypeFieldModel field)
    {
      return this.ProcessAddFieldToWorkItemType(processId, witRefNameForFields, (object) field);
    }

    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<WorkItemTypeFieldModel>), null, null)]
    [ClientLocationId("976713B4-A62E-499E-94DC-EEB869EA9126")]
    public virtual HttpResponseMessage GetWorkItemTypeFields(
      Guid processId,
      string witRefNameForFields)
    {
      return this.ProcessGetWorkItemTypeFields(processId, witRefNameForFields, false);
    }

    [HttpGet]
    [ClientResponseType(typeof (WorkItemTypeFieldModel), null, null)]
    [ClientLocationId("976713B4-A62E-499E-94DC-EEB869EA9126")]
    public virtual HttpResponseMessage GetWorkItemTypeField(
      Guid processId,
      string witRefNameForFields,
      string fieldRefName)
    {
      return this.ProcessGetWorkItemTypeField(processId, witRefNameForFields, fieldRefName, false);
    }
  }
}
