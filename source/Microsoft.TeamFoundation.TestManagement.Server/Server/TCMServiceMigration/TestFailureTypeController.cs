// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration.TestFailureTypeController
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
  [VersionedApiControllerCustomName(Area = "TCMServiceMigration", ResourceName = "testfailuretype", ResourceVersion = 1)]
  public class TestFailureTypeController : TestManagementController
  {
    private TestFailureTypeHelper m_testFailureTypeHelper;

    [HttpGet]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestFailureType>), null, null)]
    [ClientLocationId("F9CEEE62-C8BE-4C16-84F2-710929DF32D2")]
    public HttpResponseMessage GetTestFailureTypes() => this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.TestFailureType>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestFailureType>) this.TestFailureTypeHelper.QueryTestFailureTypes(this.ProjectInfo.Name));

    internal TestFailureTypeHelper TestFailureTypeHelper
    {
      get
      {
        if (this.m_testFailureTypeHelper == null)
          this.m_testFailureTypeHelper = new TestFailureTypeHelper(this.TestManagementRequestContext);
        return this.m_testFailureTypeHelper;
      }
      set => this.m_testFailureTypeHelper = value;
    }
  }
}
