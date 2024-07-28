// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestManagementConfigurationService
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
  internal class TeamFoundationTestManagementConfigurationService : 
    TeamFoundationTestManagementService,
    ITeamFoundationTestManagementConfigurationService,
    IVssFrameworkService
  {
    public TeamFoundationTestManagementConfigurationService()
    {
    }

    public TeamFoundationTestManagementConfigurationService(
      TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public TestConfiguration GetTestConfigurationById(
      TestManagementRequestContext requestContext,
      int testConfigurationId,
      string projectName)
    {
      return this.ExecuteAction<TestConfiguration>(requestContext.RequestContext, "TeamFoundationTestManagementConfigurationService.GetTestConfigurationById", (Func<TestConfiguration>) (() =>
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), requestContext.RequestContext.ServiceName);
        TestConfiguration configurationById = TestConfiguration.QueryById(requestContext, testConfigurationId, projectName);
        if (configurationById == null)
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException notFoundException = new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestConfigurationNotFound, (object) testConfigurationId));
          requestContext.RequestContext.Trace(1015068, TraceLevel.Error, "TestManagement", "BusinessLayer", "Error occurred in TeamFoundationTestManagementConfigurationService.GetTestConfigurationById. Error = {0}, testConfigurationId = {1}, ProjectName = {2} ", (object) notFoundException.Message, (object) testConfigurationId, (object) projectName);
          throw notFoundException;
        }
        requestContext.RequestContext.Trace(1015068, TraceLevel.Info, "TestManagement", "BusinessLayer", "TeamFoundationTestManagementConfigurationService.GetTestConfigurationById returned succesfully. testConfigurationId = {0}. ProjectName = {1} ", (object) testConfigurationId, (object) projectName);
        return configurationById;
      }), 1015068, "TestManagement", "BusinessLayer");
    }

    public IEnumerable<TestConfiguration> GetTestConfigurations(
      TestManagementRequestContext requestContext,
      string projectName)
    {
      return (IEnumerable<TestConfiguration>) this.ExecuteAction<List<TestConfiguration>>(requestContext.RequestContext, "TeamFoundationTestManagementConfigurationService.GetTestConfigurations", (Func<List<TestConfiguration>>) (() =>
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), requestContext.RequestContext.ServiceName);
        return new TestConfigurationHelper().FetchConfigurations(requestContext, projectName);
      }), 1015068, "TestManagement", "BusinessLayer");
    }

    public IEnumerable<TestConfiguration> GetTestConfigurationsWithPaging(
      TestManagementRequestContext requestContext,
      string projectName,
      int skip,
      int top,
      int watermark)
    {
      return (IEnumerable<TestConfiguration>) this.ExecuteAction<List<TestConfiguration>>(requestContext.RequestContext, "TeamFoundationTestManagementConfigurationService.GetTestConfigurationsWithPaging", (Func<List<TestConfiguration>>) (() =>
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), requestContext.RequestContext.ServiceName);
        return TestConfiguration.QueryWithPaging(requestContext, projectName, skip, top, watermark);
      }), 1015068, "TestManagement", "BusinessLayer");
    }

    public TestConfiguration CreateTestConfiguration(
      TestManagementRequestContext requestContext,
      TestConfiguration testConfiguration,
      string projectName)
    {
      return this.ExecuteAction<TestConfiguration>(requestContext.RequestContext, "TeamFoundationTestManagementConfigurationService.CreateTestConfiguration", (Func<TestConfiguration>) (() =>
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), requestContext.RequestContext.ServiceName);
        ArgumentUtility.CheckForNull<TestConfiguration>(testConfiguration, nameof (testConfiguration), requestContext.RequestContext.ServiceName);
        ArgumentUtility.CheckStringForNullOrWhiteSpace(testConfiguration.Name, "name", requestContext.RequestContext.ServiceName);
        TestConfiguration testConfiguration1 = testConfiguration.Create(requestContext, projectName);
        requestContext.RequestContext.Trace(1015068, TraceLevel.Info, "TestManagement", "BusinessLayer", "TeamFoundationTestManagementConfigurationService.CreateTestConfiguration:: Created test configuration with id = {0} successfully. ProjectName = {1} ", (object) testConfiguration1.Id, (object) projectName);
        return this.GetTestConfigurationById(requestContext, testConfiguration1.Id, projectName);
      }), 1015068, "TestManagement", "BusinessLayer");
    }

    public void DeleteTestConfiguration(
      TestManagementRequestContext requestContext,
      int testConfigurationId,
      string projectName)
    {
      this.ExecuteAction(requestContext.RequestContext, "TeamFoundationTestManagementConfigurationService.DeleteTestConfiguration", (Action) (() =>
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), requestContext.RequestContext.ServiceName);
        TestConfiguration.Delete(requestContext, testConfigurationId, projectName);
        requestContext.RequestContext.Trace(1015068, TraceLevel.Info, "TestManagement", "BusinessLayer", "TeamFoundationTestManagementConfigurationService.DeleteTestConfiguration:: Deleted test configuration with id = {0} successfully. ProjectName = {1} ", (object) testConfigurationId, (object) projectName);
      }), 1015068, "TestManagement", "BusinessLayer");
    }

    public TestConfiguration UpdateTestConfiguration(
      TestManagementRequestContext requestContext,
      int testConfigurationId,
      TestConfiguration updatedTestConfiguration,
      string projectName)
    {
      return this.ExecuteAction<TestConfiguration>(requestContext.RequestContext, "TeamFoundationTestManagementConfigurationService.UpdateTestConfiguration", (Func<TestConfiguration>) (() =>
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), requestContext.RequestContext.ServiceName);
        ArgumentUtility.CheckForNull<TestConfiguration>(updatedTestConfiguration, nameof (updatedTestConfiguration), requestContext.RequestContext.ServiceName);
        if (this.UpdateTestConfigurationProperties(this.GetTestConfigurationById(requestContext, testConfigurationId, projectName), updatedTestConfiguration).Update(requestContext, projectName, true, updatedTestConfiguration.Values == null).Revision == -1)
          throw new TestObjectUpdatedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ServerResources.TestObjectUpdatedError, (object) testConfigurationId));
        requestContext.RequestContext.Trace(1015068, TraceLevel.Info, "TestManagement", "BusinessLayer", "TeamFoundationTestManagementConfigurationService.UpdateTestConfiguration:: Updated test configuration with id = {0} successfully. ProjectName = {1} ", (object) testConfigurationId, (object) projectName);
        return this.GetTestConfigurationById(requestContext, testConfigurationId, projectName);
      }), 1015068, "TestManagement");
    }

    public List<TestConfigurationRecord> QueryTestConfigurationsByChangedDate(
      TestManagementRequestContext requestContext,
      int projectId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate,
      TestArtifactSource dataSource)
    {
      string str = "TeamFoundationTestManagementConfigurationService.QueryTestConfigurationsByChangedDate";
      try
      {
        requestContext.RequestContext.TraceEnter(1015068, "TestManagement", "BusinessLayer", str);
        using (PerfManager.Measure(requestContext.RequestContext, "BusinessLayer", str, isTopLevelScenario: true))
        {
          using (TestManagementDatabase replicaAwareComponent = TestManagementDatabase.CreateReadReplicaAwareComponent(requestContext.RequestContext))
            return replicaAwareComponent.QueryTestConfigurationsByChangedDate(projectId, batchSize, fromDate, out toDate, dataSource);
        }
      }
      finally
      {
        requestContext.RequestContext.TraceLeave(1015068, "TestManagement", "BusinessLayer", str);
      }
    }

    private TestConfiguration UpdateTestConfigurationProperties(
      TestConfiguration testConfiguration,
      TestConfiguration updatedTestConfiguration)
    {
      if (!string.IsNullOrEmpty(updatedTestConfiguration.Name))
        testConfiguration.Name = updatedTestConfiguration.Name;
      if (!string.IsNullOrEmpty(updatedTestConfiguration.Description))
        testConfiguration.Description = updatedTestConfiguration.Description;
      testConfiguration.State = updatedTestConfiguration.State;
      testConfiguration.IsDefault = updatedTestConfiguration.IsDefault;
      if (updatedTestConfiguration.AreaPath != null)
        testConfiguration.AreaPath = updatedTestConfiguration.AreaPath;
      if (updatedTestConfiguration.Values != null)
      {
        testConfiguration.Values.RemoveRange(0, testConfiguration.Values.Count);
        testConfiguration.Values.AddRange((IEnumerable<NameValuePair>) updatedTestConfiguration.Values);
      }
      return testConfiguration;
    }
  }
}
