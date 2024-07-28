// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SessionsController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Session", ResourceVersion = 1)]
  public class SessionsController : TestManagementTeamController
  {
    private SessionHelper m_sessionHelper;

    [HttpPost]
    [ClientLocationId("1500b4b4-6c69-4ca6-9b18-35e9e97fe2ac")]
    [ClientExample("POST__test_session.json", null, null, null)]
    public TestSession CreateTestSession(TestSession testSession)
    {
      ArgumentUtility.CheckForNull<TestSession>(testSession, nameof (testSession), this.TfsRequestContext.ServiceName);
      ArgumentUtility.CheckForNull<ShallowReference>(testSession.Area, "testSession.Area", this.TfsRequestContext.ServiceName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(testSession.Title, "testSession.Title", this.TfsRequestContext.ServiceName);
      if (testSession.Source == TestSessionSource.Unknown)
        testSession.Source = this.GetDefaultTestSessionSource();
      LicenseCheckHelper.ValidateTestSessionSource(this.TfsRequestContext, testSession.Source);
      return this.SessionHelper.CreateSession(this.ProjectId.ToString(), testSession, this.GetDefaultTeamFieldValue());
    }

    [HttpPatch]
    [ClientLocationId("1500b4b4-6c69-4ca6-9b18-35e9e97fe2ac")]
    [ClientExample("PATCH__test_session.json", null, null, null)]
    public TestSession UpdateTestSession(TestSession testSession)
    {
      ArgumentUtility.CheckForNull<TestSession>(testSession, nameof (testSession), this.TfsRequestContext.ServiceName);
      ArgumentUtility.CheckGreaterThanZero((float) testSession.Id, "testSession.Id", this.TfsRequestContext.ServiceName);
      return this.SessionHelper.UpdateSession(this.ProjectId.ToString(), testSession);
    }

    [HttpGet]
    [ClientLocationId("1500b4b4-6c69-4ca6-9b18-35e9e97fe2ac")]
    [ClientExample("GET__test_session.json", null, null, null)]
    public IList<TestSession> GetTestSessions(
      int period = 7,
      bool allSessions = true,
      bool includeAllProperties = true,
      TestSessionSource source = TestSessionSource.SessionInsightsForAll,
      bool includeOnlyCompletedSessions = true)
    {
      LicenseCheckHelper.ValidateRightsToGetSession(this.TfsRequestContext, source);
      if (period < 1 || period > 90)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.PositiveArgument, (object) "Period should be positive integer, not greater than 90 days"), "Period").Expected(this.TfsRequestContext.ServiceName);
      List<int> sourceList = this.GetSourceList(source);
      if (sourceList.Count < 1)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFieldValue, (object) "TestSessionSource"), "Source").Expected(this.TfsRequestContext.ServiceName);
      return this.SessionHelper.GetTestSessions(this.ProjectId.ToString(), this.Team, period, allSessions, includeAllProperties, sourceList, this.GetStates(includeOnlyCompletedSessions));
    }

    private TestSessionSource GetDefaultTestSessionSource() => !CommonLicenseCheckHelper.IsStakeholder(this.TfsRequestContext) ? TestSessionSource.XTWeb : TestSessionSource.FeedbackWeb;

    private string GetDefaultTeamFieldValue()
    {
      if (this.Team != null)
      {
        ITeamSettings teamSettings = this.TfsRequestContext.GetService<ITeamConfigurationService>().GetTeamSettings(this.TfsRequestContext, this.Team, true, false);
        if (teamSettings != null && teamSettings.TeamFieldConfig != null)
        {
          ITeamFieldValue[] teamFieldValues = teamSettings.TeamFieldConfig.TeamFieldValues;
          int defaultValueIndex = teamSettings.TeamFieldConfig.DefaultValueIndex;
          if (teamFieldValues.Length > defaultValueIndex)
            return teamFieldValues[defaultValueIndex].Value;
        }
      }
      return string.Empty;
    }

    private List<int> GetSourceList(TestSessionSource source)
    {
      List<int> sourceList = new List<int>();
      switch (source)
      {
        case TestSessionSource.XTDesktop:
          sourceList.Add(1);
          break;
        case TestSessionSource.FeedbackDesktop:
          sourceList.Add(2);
          break;
        case TestSessionSource.XTWeb:
          sourceList.Add(3);
          break;
        case TestSessionSource.FeedbackWeb:
          sourceList.Add(4);
          break;
        case TestSessionSource.XTDesktop2:
          sourceList.Add(5);
          break;
        case TestSessionSource.SessionInsightsForAll:
          sourceList.Add(3);
          sourceList.Add(5);
          break;
      }
      return sourceList;
    }

    private List<int> GetStates(bool includeOnlyCompletedSessions)
    {
      List<int> states;
      if (includeOnlyCompletedSessions)
        states = new List<int>() { 4 };
      else
        states = ((IEnumerable<int>) Enum.GetValues(typeof (TestSessionState))).ToList<int>();
      return states;
    }

    private List<string> GetTeamFieldDetails(string projectId, ref bool IsTeamFieldAreaPath)
    {
      List<string> teamFieldDetails = new List<string>();
      if (this.Team == null)
        return teamFieldDetails;
      ProjectProcessConfiguration processConfiguration = this.GetProjectProcessConfiguration();
      if (processConfiguration != null)
        IsTeamFieldAreaPath = processConfiguration.IsTeamFieldAreaPath();
      ITeamFieldValue[] teamFieldValues = this.TfsRequestContext.GetService<ITeamConfigurationService>().GetTeamSettings(this.TfsRequestContext, this.Team, true, false).TeamFieldConfig.TeamFieldValues;
      for (int index = 0; index < teamFieldValues.Length; ++index)
      {
        string str = teamFieldValues[index].Value;
        if (IsTeamFieldAreaPath)
        {
          int startIndex = str.IndexOf('\\');
          str = startIndex == -1 ? projectId : projectId + str.Substring(startIndex);
          if (teamFieldValues[index].IncludeChildren)
            str += "%";
        }
        teamFieldDetails.Add(str);
      }
      return teamFieldDetails;
    }

    internal ProjectProcessConfiguration GetProjectProcessConfiguration()
    {
      try
      {
        ProjectConfigurationService service = this.TfsRequestContext.GetService<ProjectConfigurationService>();
        string projectUri1 = ProjectInfo.GetProjectUri(this.Team.ProjectId);
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        string projectUri2 = projectUri1;
        return service.GetProcessSettings(tfsRequestContext, projectUri2, true);
      }
      catch (Exception ex)
      {
        return (ProjectProcessConfiguration) null;
      }
    }

    internal SessionHelper SessionHelper
    {
      get
      {
        if (this.m_sessionHelper == null)
          this.m_sessionHelper = new SessionHelper(this.TestManagementRequestContext);
        return this.m_sessionHelper;
      }
    }
  }
}
