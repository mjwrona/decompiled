// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestHelperBase
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Server;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public abstract class TestHelperBase
  {
    protected TestHelperBase(TestManagerRequestContext testContext)
    {
      this.TestContext = testContext;
      this.TfsRequestContext = testContext.TfsRequestContext;
    }

    protected TestHelperBase(IVssRequestContext tfsManagementRequestContext) => this.TfsRequestContext = tfsManagementRequestContext;

    internal TestManagerRequestContext TestContext { get; private set; }

    internal IVssRequestContext TfsRequestContext { get; private set; }

    internal ProjectProcessConfiguration GetProjectProcessConfiguration()
    {
      try
      {
        return this.TfsRequestContext.GetService<IProjectConfigurationService>().GetProcessSettings(this.TfsRequestContext, ProjectInfo.GetProjectUri(this.TestContext.Team.ProjectId), true);
      }
      catch (InvalidProjectSettingsException ex)
      {
        return (ProjectProcessConfiguration) null;
      }
    }

    public static string GetTestPointOutcomeText(
      TestPointState state,
      TestOutcome outcome,
      TestResultState lastResultState)
    {
      switch (state)
      {
        case TestPointState.Ready:
          return TestManagementResources.TestPointState_Ready;
        case TestPointState.Completed:
          return outcome == TestOutcome.None ? TestManagementResources.TestPointState_Completed : TestHelperBase.GetStringFromOutcome(outcome);
        case TestPointState.NotReady:
          switch (outcome)
          {
            case TestOutcome.Unspecified:
            case TestOutcome.NotExecuted:
              return TestManagementResources.TestPointState_Ready;
            case TestOutcome.None:
              return TestManagementServerResources.TestPointState_NotReady;
            default:
              return TestHelperBase.GetStringFromOutcome(outcome);
          }
        case TestPointState.InProgress:
          return lastResultState == TestResultState.Paused || outcome == TestOutcome.Paused ? TestManagementResources.TestPointState_Paused : TestManagementResources.TestPointState_InProgress;
        default:
          return string.Empty;
      }
    }

    public static string GetTestPointOutcomeText(TestPoint tp)
    {
      int state = (int) tp.State;
      TestOutcome lastResultOutcome = (TestOutcome) tp.LastResultOutcome;
      TestResultState lastResultState1 = (TestResultState) tp.LastResultState;
      int outcome = (int) lastResultOutcome;
      int lastResultState2 = (int) lastResultState1;
      return TestHelperBase.GetTestPointOutcomeText((TestPointState) state, (TestOutcome) outcome, (TestResultState) lastResultState2);
    }

    public static WebApiTeam GetDefaultTeam(IVssRequestContext requestContext, Guid projectGuid) => requestContext.GetService<ITeamService>().GetDefaultTeam(requestContext, projectGuid);

    public static WebApiTeam GetTeamObject(
      IVssRequestContext requestContext,
      Guid projectGuid,
      string teamIdOrName)
    {
      return requestContext.GetService<ITeamService>().GetTeamInProject(requestContext, projectGuid, teamIdOrName);
    }

    private static string GetStringFromOutcome(TestOutcome outcome)
    {
      switch (outcome)
      {
        case TestOutcome.None:
          return TestManagementResources.TestOutcome_None;
        case TestOutcome.Passed:
          return TestManagementResources.TestOutcome_Passed;
        case TestOutcome.Failed:
          return TestManagementResources.TestOutcome_Failed;
        case TestOutcome.Inconclusive:
          return TestManagementResources.TestOutcome_Inconclusive;
        case TestOutcome.Timeout:
          return TestManagementResources.TestOutcome_Timeout;
        case TestOutcome.Aborted:
          return TestManagementResources.TestOutcome_Aborted;
        case TestOutcome.Blocked:
          return TestManagementResources.TestOutcome_Blocked;
        case TestOutcome.NotExecuted:
          return TestManagementResources.TestOutcome_NotExecuted;
        case TestOutcome.Warning:
          return TestManagementResources.TestOutcome_Warning;
        case TestOutcome.Error:
          return TestManagementResources.TestOutcome_Error;
        case TestOutcome.NotApplicable:
          return TestManagementResources.TestOutcome_NotApplicable;
        default:
          return TestManagementServerResources.TestOutcome_Unspecified;
      }
    }
  }
}
