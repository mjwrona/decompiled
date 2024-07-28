// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestLinkedWorkItemsQueryController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClientInternalUseOnly(false)]
  [ControllerApiVersion(3.0)]
  [DemandFeature("2DD84BB6-7821-4FDE-85BA-A6CC4AB1B7E9", true)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "LinkedWorkItemsQuery", ResourceVersion = 1)]
  public class TestLinkedWorkItemsQueryController : TestResultsControllerBase
  {
    private ITestManagementLinkedWorkItemService _linkedWorkItemService;
    private const int c_defaultWorkItemCount = 100;

    [HttpPost]
    [ClientLocationId("A4DCB25B-9878-49EA-ABFD-E440BD9B1DCD")]
    [FeatureEnabled("TestManagement.Server.WorkItemTestLinksNewApi")]
    public List<LinkedWorkItemsQueryResult> GetLinkedWorkItemsByQuery(
      LinkedWorkItemsQuery workItemQuery)
    {
      return this.TestManagementLinkedWorkItemService.GetLinkedWorkItemsByQuery(this.TestManagementRequestContext, this.ProjectInfo, workItemQuery);
    }

    internal ITestManagementLinkedWorkItemService TestManagementLinkedWorkItemService
    {
      get => this._linkedWorkItemService ?? this.TestManagementRequestContext.RequestContext.GetService<ITestManagementLinkedWorkItemService>();
      set => this._linkedWorkItemService = value;
    }
  }
}
