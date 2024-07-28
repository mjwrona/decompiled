// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessDefinitionWorkItemTypesFields2Controller
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
  [ControllerApiVersion(5.0)]
  public class ProcessDefinitionWorkItemTypesFields2Controller : 
    ProcessDefinitionWorkItemTypesFieldsControllerBase
  {
    [HttpPost]
    [ClientResponseType(typeof (WorkItemTypeFieldModel2), null, null)]
    [ClientLocationId("976713B4-A62E-499E-94DC-EEB869EA9126")]
    public HttpResponseMessage AddFieldToWorkItemType(
      Guid processId,
      string witRefNameForFields,
      [FromBody] WorkItemTypeFieldModel2 field)
    {
      return this.ProcessAddFieldToWorkItemType(processId, witRefNameForFields, (object) field);
    }

    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<WorkItemTypeFieldModel2>), null, null)]
    [ClientLocationId("976713B4-A62E-499E-94DC-EEB869EA9126")]
    public HttpResponseMessage GetWorkItemTypeFields(Guid processId, string witRefNameForFields) => this.ProcessGetWorkItemTypeFields(processId, witRefNameForFields, true);

    [HttpGet]
    [ClientResponseType(typeof (WorkItemTypeFieldModel2), null, null)]
    [ClientLocationId("976713B4-A62E-499E-94DC-EEB869EA9126")]
    public HttpResponseMessage GetWorkItemTypeField(
      Guid processId,
      string witRefNameForFields,
      string fieldRefName)
    {
      return this.ProcessGetWorkItemTypeField(processId, witRefNameForFields, fieldRefName, true);
    }

    [HttpPatch]
    [ClientResponseType(typeof (WorkItemTypeFieldModel2), null, null)]
    [ClientLocationId("976713B4-A62E-499E-94DC-EEB869EA9126")]
    public HttpResponseMessage UpdateWorkItemTypeField(
      Guid processId,
      string witRefNameForFields,
      [FromBody] WorkItemTypeFieldModel2 field)
    {
      return this.ProcessUpdateWorkItemTypeField(processId, witRefNameForFields, (object) field);
    }
  }
}
