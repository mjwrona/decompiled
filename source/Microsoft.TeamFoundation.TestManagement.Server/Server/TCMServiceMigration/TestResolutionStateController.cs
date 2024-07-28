// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration.TestResolutionStateController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration
{
  [ClientInternalUseOnly(false)]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "TCMServiceMigration", ResourceName = "testresolutionstate", ResourceVersion = 1)]
  public class TestResolutionStateController : TestManagementController
  {
    private TestResolutionStateHelper m_testResolutionStateHelper;

    [HttpGet]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestResolutionState>), null, null)]
    [ClientLocationId("D1D88A69-25F9-4A42-A537-C605E0077CE8")]
    public HttpResponseMessage GetTestResolutionStates() => this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.TestResolutionState>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestResolutionState>) this.TestResolutionStateHelper.QueryTestResolutionStates(this.ProjectInfo.Name));

    internal TestResolutionStateHelper TestResolutionStateHelper
    {
      get
      {
        if (this.m_testResolutionStateHelper == null)
          this.m_testResolutionStateHelper = new TestResolutionStateHelper(this.TestManagementRequestContext);
        return this.m_testResolutionStateHelper;
      }
      set => this.m_testResolutionStateHelper = value;
    }
  }
}
