// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestVariablesHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestVariablesHelper : RestApiHelper
  {
    public TestVariablesHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable> GetTestVariables(
      string projectName,
      int skip = 0,
      int top = 2147483647)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), this.RequestContext.ServiceName);
      this.RequestContext.Trace(1015067, TraceLevel.Info, "TestManagement", "RestLayer", "TestVariablesHelper.GetTestVariables projectName = {0}, skip = {1}, top = {2}", (object) projectName, (object) skip, (object) top);
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable>>("TestVariablesHelper.GetTestVariables", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable>>) (() =>
      {
        ShallowReference projectRepresentation = this.ProjectServiceHelper.GetProjectRepresentation(projectName);
        List<TestVariable> list = this.TestManagementVariableService.GetTestVariables(this.TestManagementRequestContext, projectName, 0, int.MaxValue, 0).Skip<TestVariable>(skip).Take<TestVariable>(top).ToList<TestVariable>();
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable> testVariables = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable>(list.Count<TestVariable>());
        foreach (TestVariable testVariable in (IEnumerable<TestVariable>) list)
          testVariables.Add(this.ConvertTestVariableToDataContract(testVariable, projectName, projectRepresentation));
        return testVariables;
      }), 1015067, "TestManagement");
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable GetTestVariableById(
      string projectName,
      int testVariableId)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), this.RequestContext.ServiceName);
      this.RequestContext.Trace(1015067, TraceLevel.Info, "TestManagement", "RestLayer", "TestVariablesHelper.GetTestVariableById projectName = {0} testVariableId = {1}", (object) projectName, (object) testVariableId);
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable>("TestVariablesHelper.GetTestVariableById", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable>) (() => this.ConvertTestVariableToDataContract(this.TestManagementVariableService.GetTestVariableById(this.TestManagementRequestContext, testVariableId, projectName), projectName)), 1015067, "TestManagement");
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable CreateTestVariable(
      string projectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable testVariableDataContract)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable>(testVariableDataContract, "testVariable", this.RequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(testVariableDataContract.Name, "name", this.RequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), this.RequestContext.ServiceName);
      this.RequestContext.Trace(1015067, TraceLevel.Info, "TestManagement", "RestLayer", "TestVariablesHelper.CreateTestVariable testVariableName = {0}, projectName = {1}", (object) testVariableDataContract.Name, (object) projectName);
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable>("TestVariablesHelper.CreateTestVariable", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable>) (() => this.ConvertTestVariableToDataContract(this.TestManagementVariableService.CreateTestVariable(this.TestManagementRequestContext, this.ConvertDataContractToTestVariable(testVariableDataContract, projectName), projectName), projectName)), 1015067, "TestManagement");
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable UpdateTestVariable(
      string projectName,
      int testVariableId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable testVariableDataContract)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable>(testVariableDataContract, "testVariable", this.RequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), this.RequestContext.ServiceName);
      this.RequestContext.Trace(1015067, TraceLevel.Info, "TestManagement", "RestLayer", "TestVariablesHelper.UpdateTestVariable testVariableId = {0}, projectName = {1}", (object) testVariableId, (object) projectName);
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable>("TestVariablesHelper.PatchTestVariable", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable>) (() => this.ConvertTestVariableToDataContract(this.TestManagementVariableService.UpdateTestVariable(this.TestManagementRequestContext, testVariableId, this.ConvertDataContractToTestVariable(testVariableDataContract, projectName), projectName), projectName)), 1015067, "TestManagement");
    }

    public int DeleteTestVariable(string projectName, int testVariableId)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), this.RequestContext.ServiceName);
      this.RequestContext.Trace(1015067, TraceLevel.Info, "TestManagement", "RestLayer", "TestVariablesHelper.DeleteTestVariable testVariableId = {0}, projectName = {1}", (object) testVariableId, (object) projectName);
      return this.ExecuteAction<int>("TestVariablesHelper.DeleteTestVariable", (Func<int>) (() =>
      {
        this.TestManagementVariableService.DeleteTestVariable(this.TestManagementRequestContext, testVariableId, projectName);
        return testVariableId;
      }), 1015067, "TestManagement");
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable ConvertTestVariableToDataContract(
      TestVariable testVariable,
      string projectName,
      ShallowReference project = null)
    {
      ArgumentUtility.CheckForNull<TestVariable>(testVariable, nameof (testVariable), this.RequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), this.RequestContext.ServiceName);
      return new Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable()
      {
        Id = testVariable.Id,
        Name = testVariable.Name,
        Description = testVariable.Description,
        Project = project == null ? this.ProjectServiceHelper.GetProjectRepresentation(projectName) : project,
        Revision = testVariable.Revision,
        Values = testVariable.Values,
        Url = UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "Test", TestManagementResourceIds.TestVariable, (object) new
        {
          testVariableId = testVariable.Id,
          project = projectName
        })
      };
    }

    private TestVariable ConvertDataContractToTestVariable(
      Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable testVariableDataContract,
      string projectName)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.TestVariable>(testVariableDataContract, "testVariable", this.RequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), this.RequestContext.ServiceName);
      TestVariable testVariable = new TestVariable();
      testVariable.Id = testVariableDataContract.Id;
      testVariable.Name = testVariableDataContract.Name;
      if (testVariableDataContract.Values != null && testVariableDataContract.Values.Count > 0)
        testVariable.Values.AddRange((IEnumerable<string>) testVariableDataContract.Values);
      testVariable.Description = testVariableDataContract.Description;
      testVariable.Revision = testVariableDataContract.Revision;
      return testVariable;
    }
  }
}
