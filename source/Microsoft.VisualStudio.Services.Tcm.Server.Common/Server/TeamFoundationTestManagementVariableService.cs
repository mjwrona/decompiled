// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestManagementVariableService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TeamFoundationTestManagementVariableService : 
    TeamFoundationTestManagementService,
    ITeamFoundationTestManagementVariableService,
    IVssFrameworkService
  {
    public TeamFoundationTestManagementVariableService()
    {
    }

    public TeamFoundationTestManagementVariableService(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public TestVariable GetTestVariableById(
      TestManagementRequestContext requestContext,
      int testVariableId,
      string projectName)
    {
      return this.ExecuteAction<TestVariable>(requestContext.RequestContext, "TeamFoundationTestManagementVariableService.GetTestVariableById", (Func<TestVariable>) (() =>
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), requestContext.RequestContext.ServiceName);
        TestVariable testVariableById = TestVariable.QueryById(requestContext, testVariableId, projectName);
        if (testVariableById == null)
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException notFoundException = new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestVariableNotFound, (object) testVariableId));
          requestContext.RequestContext.Trace(1015067, TraceLevel.Error, "TestManagement", "BusinessLayer", "Error occurred in TeamFoundationTestManagementVariableService.GetTestVariableById. Error = {0}, testVariableId = {1}, ProjectName = {2} ", (object) notFoundException.Message, (object) testVariableId, (object) projectName);
          throw notFoundException;
        }
        requestContext.RequestContext.Trace(1015067, TraceLevel.Info, "TestManagement", "BusinessLayer", "TeamFoundationTestManagementVariableService.GetTestVariableById returned succesfully. testVariableId = {0}. ProjectName = {1} ", (object) testVariableId, (object) projectName);
        return testVariableById;
      }), 1015067, "TestManagement", "BusinessLayer");
    }

    public IEnumerable<TestVariable> GetTestVariables(
      TestManagementRequestContext requestContext,
      string projectName,
      int skip,
      int topToFetch,
      int watermark)
    {
      return (IEnumerable<TestVariable>) this.ExecuteAction<List<TestVariable>>(requestContext.RequestContext, "TeamFoundationTestManagementVariableService.GetTestVariables", (Func<List<TestVariable>>) (() =>
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), requestContext.RequestContext.ServiceName);
        return TestVariable.QueryTestVariables(requestContext, projectName, skip, topToFetch, watermark);
      }), 1015067, "TestManagement", "BusinessLayer");
    }

    public TestVariable CreateTestVariable(
      TestManagementRequestContext requestContext,
      TestVariable testVariable,
      string projectName)
    {
      return this.ExecuteAction<TestVariable>(requestContext.RequestContext, "TeamFoundationTestManagementVariableService.CreateTestVariable", (Func<TestVariable>) (() =>
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), requestContext.RequestContext.ServiceName);
        ArgumentUtility.CheckForNull<TestVariable>(testVariable, nameof (testVariable), requestContext.RequestContext.ServiceName);
        ArgumentUtility.CheckStringForNullOrWhiteSpace(testVariable.Name, "name", requestContext.RequestContext.ServiceName);
        int testVariableId = testVariable.Create(requestContext, projectName);
        requestContext.RequestContext.Trace(1015067, TraceLevel.Info, "TestManagement", "BusinessLayer", "TeamFoundationTestManagementVariableService.CreateTestVariable:: Created test variable with id = {0} successfully. ProjectName = {1} ", (object) testVariableId, (object) projectName);
        return this.GetTestVariableById(requestContext, testVariableId, projectName);
      }), 1015067, "TestManagement", "BusinessLayer");
    }

    public void DeleteTestVariable(
      TestManagementRequestContext requestContext,
      int testVariableId,
      string projectName)
    {
      this.ExecuteAction(requestContext.RequestContext, "TeamFoundationTestManagementVariableService.DeleteTestVariable", (Action) (() =>
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), requestContext.RequestContext.ServiceName);
        TestVariable.Delete(requestContext, testVariableId, projectName);
        requestContext.RequestContext.Trace(1015067, TraceLevel.Info, "TestManagement", "BusinessLayer", "TeamFoundationTestManagementVariableService.DeleteTestVariable:: Deleted test variable with id = {0} successfully. ProjectName = {1} ", (object) testVariableId, (object) projectName);
      }), 1015067, "TestManagement", "BusinessLayer");
    }

    public TestVariable UpdateTestVariable(
      TestManagementRequestContext requestContext,
      int testVariableId,
      TestVariable updatedTestVariable,
      string projectName)
    {
      return this.ExecuteAction<TestVariable>(requestContext.RequestContext, "TeamFoundationTestManagementVariableService.UpdateTestVariable", (Func<TestVariable>) (() =>
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), requestContext.RequestContext.ServiceName);
        ArgumentUtility.CheckForNull<TestVariable>(updatedTestVariable, nameof (updatedTestVariable), requestContext.RequestContext.ServiceName);
        if (this.UpdateTestVariableProperties(this.GetTestVariableById(requestContext, testVariableId, projectName), updatedTestVariable).Update(requestContext, projectName) == -1)
          throw new TestObjectUpdatedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ServerResources.TestObjectUpdatedError, (object) testVariableId));
        requestContext.RequestContext.Trace(1015067, TraceLevel.Info, "TestManagement", "BusinessLayer", "TeamFoundationTestManagementVariableService.UpdateTestVariable:: Updated test variable with id = {0} successfully. ProjectName = {1} ", (object) testVariableId, (object) projectName);
        return this.GetTestVariableById(requestContext, testVariableId, projectName);
      }), 1015067, "TestManagement");
    }

    private TestVariable UpdateTestVariableProperties(
      TestVariable testVariable,
      TestVariable updatedTestVariable)
    {
      if (!string.IsNullOrEmpty(updatedTestVariable.Name))
        testVariable.Name = updatedTestVariable.Name;
      if (!string.IsNullOrEmpty(updatedTestVariable.Description))
        testVariable.Description = updatedTestVariable.Description;
      if (updatedTestVariable.Values != null)
      {
        testVariable.Values.RemoveRange(0, testVariable.Values.Count);
        testVariable.Values.AddRange((IEnumerable<string>) updatedTestVariable.Values);
      }
      return testVariable;
    }
  }
}
