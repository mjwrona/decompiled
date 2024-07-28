// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestConfigurationsHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestConfigurationsHelper : RestApiHelper
  {
    public TestConfigurationsHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration> GetTestConfigurations(
      string projectName,
      int skip = 0,
      int top = 2147483647,
      bool includeAllProperties = false)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), this.RequestContext.ServiceName);
      this.RequestContext.Trace(1015068, TraceLevel.Info, "TestManagement", "RestLayer", "TestConfigurationsHelper.GetTestConfigurations projectName = {0}, skip = {1}, top = {2}", (object) projectName, (object) skip, (object) top);
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>>("TestConfigurationsHelper.GetTestConfigurations", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>>) (() =>
      {
        ShallowReference projectRepresentation = this.ProjectServiceHelper.GetProjectRepresentation(projectName);
        IEnumerable<TestConfiguration> list = (IEnumerable<TestConfiguration>) this.TestManagementConfigurationService.GetTestConfigurations(this.TestManagementRequestContext, projectName).Skip<TestConfiguration>(skip).Take<TestConfiguration>(top).ToList<TestConfiguration>();
        Dictionary<Guid, IdentityRef> identityMap = this.GetIdentityMap(list);
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration> testConfigurations = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>(list.Count<TestConfiguration>());
        foreach (TestConfiguration testConfiguration in list)
          testConfigurations.Add(this.ConvertTestConfigurationToDataContract(testConfiguration, projectRepresentation, identityMap, includeAllProperties));
        return testConfigurations;
      }), 1015068, "TestManagement");
    }

    public virtual List<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration> GetTestConfigurationsWithPaging(
      string projectName,
      int skip = 0,
      int top = 0,
      int watermark = 0,
      bool includeAllProperties = false)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), this.RequestContext.ServiceName);
      this.RequestContext.Trace(1015068, TraceLevel.Info, "TestManagement", "RestLayer", "TestConfigurationsHelper.GetTestConfigurationsWithPaging projectName = {0}, skip = {1}, top = {2}", (object) projectName, (object) skip, (object) top);
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>>("TestConfigurationsHelper.GetTestConfigurationsWithPaging", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>>) (() =>
      {
        ShallowReference projectRepresentation = this.ProjectServiceHelper.GetProjectRepresentation(projectName);
        IEnumerable<TestConfiguration> configurationsWithPaging1 = this.TestManagementConfigurationService.GetTestConfigurationsWithPaging(this.TestManagementRequestContext, projectName, skip, top, watermark);
        Dictionary<Guid, IdentityRef> identityMap = this.GetIdentityMap(configurationsWithPaging1);
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration> configurationsWithPaging2 = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>(configurationsWithPaging1.Count<TestConfiguration>());
        foreach (TestConfiguration testConfiguration in configurationsWithPaging1)
          configurationsWithPaging2.Add(this.ConvertTestConfigurationToDataContract(testConfiguration, projectRepresentation, identityMap, includeAllProperties));
        return configurationsWithPaging2;
      }), 1015068, "TestManagement");
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration GetTestConfigurationById(
      string projectName,
      int testConfigurationId)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), this.RequestContext.ServiceName);
      this.RequestContext.Trace(1015068, TraceLevel.Info, "TestManagement", "RestLayer", "TestConfigurationsHelper.GetTestConfigurationById projectName = {0} testConfigurationId = {1}", (object) projectName, (object) testConfigurationId);
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>("TestConfigurationsHelper.GetTestConfigurationById", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>) (() =>
      {
        ShallowReference projectRepresentation = this.ProjectServiceHelper.GetProjectRepresentation(projectName);
        return this.ConvertTestConfigurationToDataContract(this.TestManagementConfigurationService.GetTestConfigurationById(this.TestManagementRequestContext, testConfigurationId, projectName), projectRepresentation);
      }), 1015068, "TestManagement");
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration CreateTestConfiguration(
      string projectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration testConfigurationDataContract)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), this.RequestContext.ServiceName);
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>(testConfigurationDataContract, "testConfiguration", this.RequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(testConfigurationDataContract.Name, "name", this.RequestContext.ServiceName);
      this.RequestContext.Trace(1015068, TraceLevel.Info, "TestManagement", "RestLayer", "TestConfigurationsHelper.CreateTestConfiguration testConfigurationName = {0}, projectName = {1}", (object) testConfigurationDataContract.Name, (object) projectName);
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>("TestConfigurationsHelper.CreateTestConfiguration", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>) (() =>
      {
        ShallowReference projectRepresentation = this.ProjectServiceHelper.GetProjectRepresentation(projectName);
        return this.ConvertTestConfigurationToDataContract(this.TestManagementConfigurationService.CreateTestConfiguration(this.TestManagementRequestContext, this.ConvertDataContractToTestConfiguration(testConfigurationDataContract, projectName, false), projectName), projectRepresentation);
      }), 1015068, "TestManagement");
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration UpdateTestConfiguration(
      string projectName,
      int testConfigurationId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration testConfigurationDataContract)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), this.RequestContext.ServiceName);
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>(testConfigurationDataContract, "testConfiguration", this.RequestContext.ServiceName);
      this.RequestContext.Trace(1015068, TraceLevel.Info, "TestManagement", "RestLayer", "TestConfigurationsHelper.UpdateTestConfiguration testConfigurationId= {0}, projectName = {1}", (object) testConfigurationId, (object) projectName, (object) testConfigurationDataContract.Description);
      return this.ExecuteAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>("TestConfigurationsHelper.UpdateTestConfiguration", (Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration>) (() =>
      {
        ShallowReference projectRepresentation = this.ProjectServiceHelper.GetProjectRepresentation(projectName);
        return this.ConvertTestConfigurationToDataContract(this.TestManagementConfigurationService.UpdateTestConfiguration(this.TestManagementRequestContext, testConfigurationId, this.ConvertDataContractToTestConfiguration(testConfigurationDataContract, projectName, true), projectName), projectRepresentation);
      }), 1015068, "TestManagement");
    }

    public int DeleteTestConfiguration(string projectName, int testConfigurationId)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), this.RequestContext.ServiceName);
      this.RequestContext.Trace(1015068, TraceLevel.Info, "TestManagement", "RestLayer", "TestConfigurationsHelper.DeleteTestConfiguration testConfigurationName = {0}, projectName = {1}", (object) testConfigurationId, (object) projectName);
      return this.ExecuteAction<int>("TestConfigurationsHelper.DeleteTestConfiguration", (Func<int>) (() =>
      {
        this.ProjectServiceHelper.GetProjectRepresentation(projectName);
        this.TestManagementConfigurationService.DeleteTestConfiguration(this.TestManagementRequestContext, testConfigurationId, projectName);
        return testConfigurationId;
      }), 1015068, "TestManagement");
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration ConvertTestConfigurationToDataContract(
      TestConfiguration testConfiguration,
      ShallowReference projectReference,
      Dictionary<Guid, IdentityRef> identityRefMap = null,
      bool includeAllProperties = true)
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration dataContract = new Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration();
      dataContract.Id = testConfiguration.Id;
      dataContract.Name = testConfiguration.Name;
      dataContract.Description = testConfiguration.Description;
      dataContract.IsDefault = testConfiguration.IsDefault;
      dataContract.State = (TestConfigurationState) testConfiguration.State;
      dataContract.Url = UrlBuildHelper.GetResourceUrl(this.RequestContext, ServiceInstanceTypes.TFS, "Test", TestManagementResourceIds.TestConfiguration, (object) new
      {
        testConfigurationId = testConfiguration.Id,
        project = projectReference.Name
      });
      if (testConfiguration.IsDefault && testConfiguration.State.Equals((object) TestConfigurationState.Inactive))
        throw new ArgumentException(ServerResources.InactivePlanCannotBeMarkedAsDefault).Expected(this.RequestContext.ServiceName);
      if (includeAllProperties)
      {
        dataContract.Project = projectReference;
        dataContract.Revision = testConfiguration.Revision;
        dataContract.LastUpdatedDate = testConfiguration.LastUpdated;
        dataContract.Area = new ShallowReference()
        {
          Id = testConfiguration.AreaId.ToString(),
          Name = testConfiguration.AreaPath
        };
        if (identityRefMap == null)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = this.ReadIdentityByAccountId(testConfiguration.LastUpdatedBy);
          if (identity != null)
            dataContract.LastUpdatedBy = this.CreateTeamFoundationIdentityReference(identity);
        }
        else if (testConfiguration.LastUpdatedBy != Guid.Empty && identityRefMap.ContainsKey(testConfiguration.LastUpdatedBy))
          dataContract.LastUpdatedBy = identityRefMap[testConfiguration.LastUpdatedBy];
        dataContract.Values = this.ConvertToClientTestVariables((IList<NameValuePair>) testConfiguration.Values);
      }
      return dataContract;
    }

    private TestConfiguration ConvertDataContractToTestConfiguration(
      Microsoft.TeamFoundation.TestManagement.WebApi.TestConfiguration testConfigurationDataContract,
      string projectName,
      bool isUpdate)
    {
      TestConfiguration testConfiguration = new TestConfiguration();
      testConfiguration.Name = testConfigurationDataContract.Name;
      testConfiguration.Description = testConfigurationDataContract.Description;
      testConfiguration.State = (byte) testConfigurationDataContract.State;
      testConfiguration.IsDefault = testConfigurationDataContract.IsDefault;
      testConfiguration.Revision = testConfigurationDataContract.Revision;
      if (testConfigurationDataContract.Area != null && !string.IsNullOrEmpty(testConfigurationDataContract.Area.Name))
        testConfiguration.AreaPath = testConfigurationDataContract.Area.Name;
      else if (!isUpdate)
      {
        ProjectInfo projectFromName = this.TestManagementRequestContext.ProjectServiceHelper.GetProjectFromName(projectName);
        testConfiguration.AreaPath = this.GetAreaPathForProject(projectFromName.Uri);
      }
      if (testConfigurationDataContract.Values != null)
      {
        testConfiguration.Values.RemoveRange(0, testConfiguration.Values.Count);
        testConfiguration.Values.AddRange((IEnumerable<NameValuePair>) this.ConvertToServerTestVariables(testConfigurationDataContract.Values, projectName));
      }
      return testConfiguration;
    }

    private string GetAreaPathForProject(string projectUri) => this.ExecuteAction<string>("TestConfigurationsHelper.GetAreaPathForProject", (Func<string>) (() =>
    {
      TcmCommonStructureNodeInfo rootNode = this.TestManagementRequestContext.CSSHelper.GetRootNode(projectUri, "ProjectModelHierarchy");
      return rootNode == null ? string.Empty : rootNode.Path;
    }), 1015050, "TestManagement");

    private IList<NameValuePair> ConvertToServerTestVariables(
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair> clientValues,
      string projectName)
    {
      List<NameValuePair> serverTestVariables = new List<NameValuePair>();
      List<TestVariable> source = TestVariable.QueryTestVariables(this.TestManagementRequestContext, projectName);
      if (clientValues != null)
      {
        if (clientValues.Where<Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair, bool>) (tv => string.IsNullOrWhiteSpace(tv.Name))).Count<Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair>() > 1)
          throw new InvalidPropertyException("TestVariableValue", ServerResources.InvalidPropertyMessage);
        IEnumerable<string> strings = clientValues.GroupBy<Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair, string>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair, string>) (tv => tv.Name), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Where<IGrouping<string, Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair>>((Func<IGrouping<string, Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair>, bool>) (grp => grp.Count<Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair>() > 1)).Select<IGrouping<string, Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair>, string>((Func<IGrouping<string, Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair>, string>) (grp => grp.Key));
        if (strings.Count<string>() > 0)
          throw new InvalidPropertyException("TestVariableValue", string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.DuplicateTestVariable, (object) string.Join(ServerResources.VariableSeparator, strings)));
        foreach (Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair clientValue in (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair>) clientValues)
        {
          Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair keyValue = clientValue;
          TestVariable testVariable = source.AsQueryable<TestVariable>().Where<TestVariable>((Expression<Func<TestVariable, bool>>) (tv => tv.Name.Equals(keyValue.Name, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<TestVariable>();
          if (testVariable != null && testVariable.Values.Contains<string>(keyValue.Value, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          {
            serverTestVariables.Add(new NameValuePair(keyValue.Name, keyValue.Value));
          }
          else
          {
            if (testVariable == null)
              throw new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestVariableNotFound, (object) keyValue.Name));
            throw new InvalidPropertyException("TestVariableValue", ServerResources.InvalidPropertyMessage);
          }
        }
      }
      return (IList<NameValuePair>) serverTestVariables;
    }

    private IList<Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair> ConvertToClientTestVariables(
      IList<NameValuePair> serverValues)
    {
      List<Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair> clientTestVariables = new List<Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair>();
      if (serverValues != null)
      {
        foreach (NameValuePair serverValue in (IEnumerable<NameValuePair>) serverValues)
          clientTestVariables.Add(new Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair(serverValue.Name, serverValue.Value));
      }
      return (IList<Microsoft.TeamFoundation.TestManagement.WebApi.NameValuePair>) clientTestVariables;
    }

    private Dictionary<Guid, IdentityRef> GetIdentityMap(
      IEnumerable<TestConfiguration> testConfigurations)
    {
      List<Guid> source = new List<Guid>();
      foreach (TestConfiguration testConfiguration in testConfigurations)
      {
        if (testConfiguration.LastUpdatedBy != Guid.Empty)
          source.Add(testConfiguration.LastUpdatedBy);
      }
      List<Guid> list = source.Distinct<Guid>().ToList<Guid>();
      Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = this.ReadIdentityByAccounts(list.ToArray());
      Dictionary<Guid, IdentityRef> identityMap = new Dictionary<Guid, IdentityRef>();
      if (identityArray != null)
      {
        int index = 0;
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identityArray)
        {
          if (identity != null)
            identityMap[list[index]] = this.CreateTeamFoundationIdentityReference(identity);
          ++index;
        }
      }
      return identityMap;
    }
  }
}
