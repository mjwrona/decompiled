// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestVariablesController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Variables", ResourceVersion = 1)]
  public class TestVariablesController : TestManagementController
  {
    private TestVariablesHelper m_testVariablesHelper;

    [HttpGet]
    [ClientLocationId("be3fcb2b-995b-47bf-90e5-ca3cf9980912")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientExample("GET__test_variables__variableId_.json", null, null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable GetTestVariableById(
      int testVariableId)
    {
      return this.TestVariablesHelper.GetTestVariableById(this.ProjectInfo.Name, testVariableId);
    }

    [HttpGet]
    [ClientLocationId("be3fcb2b-995b-47bf-90e5-ca3cf9980912")]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable>), null, null)]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientExample("GET__test_variables.json", "Get a list of test variables", null, null)]
    [ClientExample("GET__test_variables__top-2.json", "A page at a time", null, null)]
    public HttpResponseMessage GetTestVariables([FromUri(Name = "$skip")] int skip = 0, [FromUri(Name = "$top")] int top = 2147483647) => this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable>) this.TestVariablesHelper.GetTestVariables(this.ProjectInfo.Name, skip, top));

    [HttpPost]
    [ClientLocationId("be3fcb2b-995b-47bf-90e5-ca3cf9980912")]
    [ClientExample("POST__test_variables.json", null, null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable CreateTestVariable(
      Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable testVariable)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable>(testVariable, "TestVariable", this.TfsRequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(testVariable.Name, "name", this.TfsRequestContext.ServiceName);
      return this.TestVariablesHelper.CreateTestVariable(this.ProjectInfo.Name, testVariable);
    }

    [HttpPatch]
    [ClientLocationId("be3fcb2b-995b-47bf-90e5-ca3cf9980912")]
    [ClientExample("PATCH__test_variables__variableId_.json", null, null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable UpdateTestVariable(
      int testVariableId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable testVariable)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable>(testVariable, "TestVariable", this.TfsRequestContext.ServiceName);
      return this.TestVariablesHelper.UpdateTestVariable(this.ProjectInfo.Name, testVariableId, testVariable);
    }

    [HttpDelete]
    [ClientLocationId("be3fcb2b-995b-47bf-90e5-ca3cf9980912")]
    [ClientExample("DELETE__test_variables__variableId_.json", null, null, null)]
    public void DeleteTestVariable(int testVariableId)
    {
      LicenseCheckHelper.ValidateIfUserHasTestManagementAdvancedAccess(this.TfsRequestContext);
      this.TestVariablesHelper.DeleteTestVariable(this.ProjectInfo.Name, testVariableId);
    }

    private TestVariablesHelper TestVariablesHelper
    {
      get
      {
        if (this.m_testVariablesHelper == null)
          this.m_testVariablesHelper = new TestVariablesHelper(this.TestManagementRequestContext);
        return this.m_testVariablesHelper;
      }
    }
  }
}
