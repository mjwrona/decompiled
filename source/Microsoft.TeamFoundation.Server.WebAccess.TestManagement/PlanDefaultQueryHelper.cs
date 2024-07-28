// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.PlanDefaultQueryHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Globalization;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class PlanDefaultQueryHelper
  {
    public static string GetDefaultTestPlanQuery(
      IVssRequestContext requestContext,
      Guid teamId,
      Guid projectGuid)
    {
      string areaPathFilter = PlanDefaultQueryHelper.GetAreaPathFilter(requestContext, teamId, projectGuid);
      if (string.IsNullOrWhiteSpace(areaPathFilter))
        return TestManagementConstants.Wiql_All_TestPlan;
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, TestManagementConstants.Wiql_And_Clause, (object) areaPathFilter);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) TestManagementConstants.Wiql_All_TestPlan, (object) str);
    }

    private static string GetAreaPathFilter(
      IVssRequestContext requestContext,
      Guid teamId,
      Guid projectGuid)
    {
      try
      {
        WebApiTeam teamInProject = requestContext.GetService<ITeamService>().GetTeamInProject(requestContext, projectGuid, teamId.ToString());
        if (teamInProject == null)
          return string.Empty;
        TestManagementRequestContext managementRequestContext = new TestManagementRequestContext(requestContext);
        ProjectProcessConfiguration processConfiguration = PlanDefaultQueryHelper.GetProjectProcessConfiguration(requestContext, projectGuid);
        if (processConfiguration == null)
          return string.Empty;
        ITeamFieldValue[] teamFieldValues = requestContext.GetService<ITeamConfigurationService>().GetTeamSettings(requestContext, teamInProject, true, false).TeamFieldConfig.TeamFieldValues;
        StringBuilder stringBuilder = new StringBuilder();
        bool flag = processConfiguration.IsTeamFieldAreaPath();
        string format = "{0} {1} '{2}'";
        string str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]", (object) processConfiguration.TeamField.Name);
        for (int index = 0; index < teamFieldValues.Length; ++index)
        {
          if (index > 0)
            stringBuilder.Append(" OR ");
          string str2 = !flag || !teamFieldValues[index].IncludeChildren ? "=" : "UNDER";
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, format, (object) str1, (object) str2, (object) WorkItemTrackingUtils.EscapeWiqlFieldValue(teamFieldValues[index].Value));
        }
        string empty = string.Empty;
        return teamFieldValues.Length <= 1 ? stringBuilder.ToString() : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "({0})", (object) stringBuilder.ToString());
      }
      catch (InvalidTeamSettingsException ex)
      {
        if (ex.InvalidFields.HasFlag((Enum) TeamSettingsFields.TeamField))
          throw new InvalidTeamSettingsException(TestManagementServerResources.AreaPathNotAssignedToTeamError, (Exception) ex);
        throw;
      }
    }

    private static ProjectProcessConfiguration GetProjectProcessConfiguration(
      IVssRequestContext requestContext,
      Guid projectGuid)
    {
      try
      {
        return requestContext.GetService<ProjectConfigurationService>().GetProcessSettings(requestContext, ProjectInfo.GetProjectUri(projectGuid), true);
      }
      catch (InvalidProjectSettingsException ex)
      {
        return (ProjectProcessConfiguration) null;
      }
    }
  }
}
