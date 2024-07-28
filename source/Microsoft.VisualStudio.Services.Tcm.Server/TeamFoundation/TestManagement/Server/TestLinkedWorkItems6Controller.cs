// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestLinkedWorkItems6Controller
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(6.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "workitems", ResourceVersion = 2)]
  public class TestLinkedWorkItems6Controller : TcmTestLinkedWorkItemsController
  {
    [HttpGet]
    [ClientLocationId("3D032FD6-E7A0-468B-B105-75D206F99AAD")]
    public List<WorkItemReference> GetTestResultWorkItemsById(int runId, int testCaseResultId)
    {
      this.TestManagementRequestContext.SecurityManager.CheckViewTestResultsPermission(this.TestManagementRequestContext, this.ProjectInfo.Uri);
      return this.TestLinkedWorkItemsHelper.GetAssociatedWorkItemsForResult(new GuidAndString(this.ProjectInfo.Uri, this.ProjectInfo.Id), runId, testCaseResultId);
    }
  }
}
