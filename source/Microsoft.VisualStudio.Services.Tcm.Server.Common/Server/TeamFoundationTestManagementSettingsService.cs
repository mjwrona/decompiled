// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestManagementSettingsService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TeamFoundationTestManagementSettingsService : 
    TeamFoundationTestManagementService,
    ITeamFoundationTestManagementSettingsService,
    IVssFrameworkService
  {
    public TeamFoundationTestManagementSettingsService()
    {
    }

    public TeamFoundationTestManagementSettingsService(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public IList<TestSettings2> GetTestSettings(
      TestManagementRequestContext requestContext,
      string projectName,
      ref int top,
      int continuationTokenId)
    {
      GuidAndString projectId;
      this.ValidateGetSettingsParams(requestContext, projectName, top, continuationTokenId, out projectId);
      top = this.GetBatchSize(requestContext.RequestContext, top);
      if (!requestContext.SecurityManager.HasViewTestResultsPermission(requestContext, projectId.String))
        return (IList<TestSettings2>) new List<TestSettings2>();
      List<TestSettings> testSettings;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(requestContext))
        testSettings = managementDatabase.GetTestSettings(projectId.GuidId, top, continuationTokenId);
      return new DataContractConverter(requestContext).ConvertTestSettingsListToDataContract((IList<TestSettings>) testSettings);
    }

    public TestSettings GetTestSettingsById(
      TestManagementRequestContext requestContext,
      int testSettingsId,
      TeamProjectReference teamProject)
    {
      ArgumentUtility.CheckForNull<TeamProjectReference>(teamProject, nameof (teamProject));
      TestSettings testSettingsById = TestSettings.QueryById(requestContext, testSettingsId, teamProject.Name);
      if (testSettingsById == null)
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException notFoundException = new Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestSettingsNotFound, (object) testSettingsId));
        requestContext.TraceError("BusinessLayer", "Error occurred in TeamFoundationTestManagementSettingsService.GetTestSettingsById. Error = {0}, testSettingsId = {1}, ProjectName = {2} ", (object) notFoundException.Message, (object) testSettingsId, (object) teamProject.Name);
        throw notFoundException;
      }
      requestContext.TraceInfo("BusinessLayer", "TeamFoundationTestManagementSettingsService.GetTestSettingsById returned succesfully. TestSettingsId = {0}. ProjectName = {1} ", (object) testSettingsId, (object) teamProject.Name);
      return testSettingsById;
    }

    public int CreateTestSettings(
      TestManagementRequestContext requestContext,
      TestSettings testSettings,
      TeamProjectReference teamProject)
    {
      ArgumentUtility.CheckForNull<TeamProjectReference>(teamProject, nameof (teamProject));
      ArgumentUtility.CheckForNull<TestSettings>(testSettings, nameof (testSettings), requestContext.RequestContext.ServiceName);
      UpdatedProperties updatedProperties = testSettings.Create(requestContext, teamProject.Name);
      requestContext.TraceInfo("BusinessLayer", "TeamFoundationTestManagementSettingsService.CreateTestSettings:: Created test settings with id = {0} successfully. ProjectName = {1} ", (object) updatedProperties.Id, (object) teamProject.Name);
      return updatedProperties.Id;
    }

    public void DeleteTestSettings(
      TestManagementRequestContext requestContext,
      int testSettingsId,
      TeamProjectReference teamProject)
    {
      ArgumentUtility.CheckForNull<TeamProjectReference>(teamProject, nameof (teamProject), requestContext.RequestContext.ServiceName);
      TestSettings.Delete(requestContext, testSettingsId, teamProject.Name);
      requestContext.TraceInfo("BusinessLayer", "TeamFoundationTestManagementSettingsService.DeleteTestSettings:: Deleted test settings with id = {0} successfully. ProjectName = {1} ", (object) testSettingsId, (object) teamProject.Name);
    }

    private void ValidateGetSettingsParams(
      TestManagementRequestContext requestContext,
      string projectName,
      int top,
      int continuationTokenId,
      out GuidAndString projectId)
    {
      ArgumentUtility.CheckForNull<string>(projectName, nameof (projectName));
      ArgumentUtility.CheckForNonPositiveInt(top, nameof (top));
      ArgumentUtility.CheckForNonnegativeInt(continuationTokenId, nameof (continuationTokenId));
      if (top > 1000)
        throw new ArgumentOutOfRangeException(nameof (top));
      projectId = Validator.CheckAndGetProjectFromName(requestContext, projectName);
    }

    private int GetBatchSize(IVssRequestContext requestContext, int top) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/Settings/TestSettingsFetchBatchSize", top);
  }
}
