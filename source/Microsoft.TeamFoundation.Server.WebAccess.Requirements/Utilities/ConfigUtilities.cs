// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Requirements.Utilities.ConfigUtilities
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Requirements, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6C113FD4-8DA1-49E9-A859-47B7ED9A5698
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Requirements.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Requirements.Utilities
{
  internal static class ConfigUtilities
  {
    private const string FeedbackRequestCategory = "Microsoft.FeedbackRequestCategory";
    private const string TypeFieldApplicationType = "ApplicationType";
    private const int WebAccessExceptionEaten = 599999;

    internal static object BuildFeedbackRequestConfiguration(
      IVssRequestContext tfsRequestContext,
      CommonStructureProjectInfo project,
      WebApiTeam team)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(tfsRequestContext, nameof (tfsRequestContext));
      ArgumentUtility.CheckForNull<CommonStructureProjectInfo>(project, nameof (project));
      WebAccessWorkItemService service1 = tfsRequestContext.GetService<WebAccessWorkItemService>();
      ProjectProcessConfiguration processSettings = ConfigUtilities.GetProcessSettings(tfsRequestContext, project);
      int num1 = !string.IsNullOrEmpty(team?.Name) ? 1 : 0;
      string str1 = (string) null;
      string str2 = (string) null;
      string str3 = (string) null;
      if (num1 != 0)
      {
        ITeamSettings teamSettings = (ITeamSettings) null;
        if (ConfigUtilities.TryGetTeamSettings(tfsRequestContext, team, out teamSettings))
        {
          if (TFStringComparer.WorkItemFieldReferenceName.Equals(CoreFieldReferenceNames.AreaPath, processSettings.TeamField.Name))
          {
            ITeamFieldSettings teamFieldConfig = teamSettings.TeamFieldConfig;
            str1 = teamFieldConfig.TeamFieldValues[teamFieldConfig.DefaultValueIndex].Value;
          }
          try
          {
            Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode backlogIterationNode = teamSettings.GetBacklogIterationNode(tfsRequestContext);
            if (backlogIterationNode != null)
              str3 = backlogIterationNode.GetPath(tfsRequestContext);
          }
          catch (Exception ex)
          {
            TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, nameof (BuildFeedbackRequestConfiguration), ex);
          }
          if (teamSettings.Iterations.Values.Count<ITeamIteration>() > 0)
          {
            try
            {
              Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode currentIterationNode = teamSettings.GetCurrentIterationNode(tfsRequestContext, project.GetId());
              if (currentIterationNode != null)
                str2 = currentIterationNode.GetPath(tfsRequestContext);
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, nameof (BuildFeedbackRequestConfiguration), ex);
            }
          }
        }
      }
      string str4 = service1.GetWorkItemNamesForCategories(tfsRequestContext, project.Name, (IEnumerable<string>) new string[1]
      {
        "Microsoft.FeedbackRequestCategory"
      }).FirstOrDefault<string>();
      if (string.IsNullOrEmpty(str4))
        throw new InvalidOperationException(FeedbackResources.FeedbackRequest_Error_NoFeedbackRequestWITD);
      TypeField var = ((IEnumerable<TypeField>) processSettings.TypeFields).Where<TypeField>((Func<TypeField, bool>) (t => t.Type == FieldTypeEnum.ApplicationType)).FirstOrDefault<TypeField>();
      ArgumentUtility.CheckForNull<TypeField>(var, "typeFieldApplicationType");
      TypeField startInformation = processSettings.ApplicationStartInformation;
      ArgumentUtility.CheckForNull<TypeField>(startInformation, "typeFieldApplicationStartInformation");
      TypeField launchInstructions = processSettings.ApplicationLaunchInstructions;
      ArgumentUtility.CheckForNull<TypeField>(launchInstructions, "typeFieldApplicationLaunchInstructions");
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (TypeFieldValue typeFieldValue in var.TypeFieldValues)
        dictionary.Add(typeFieldValue.Type, typeFieldValue.Value);
      TeamFoundationMailService service2 = tfsRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationMailService>();
      int num2 = TeamFoundationMailService.MailBodyEncoding.IsSingleByte ? 1 : 2;
      int num3 = (int) Math.Floor((double) (service2.MaxEmailBodySize / num2));
      return (object) new
      {
        feedbackRequestWorkItemTypeName = str4,
        applicationTypeFieldName = var.Name,
        applicationStartInformationFieldName = startInformation.Name,
        applicationLaunchInstructionsFieldName = launchInstructions.Name,
        applicationTypes = new
        {
          webApplication = dictionary["WebApp"],
          remoteMachine = dictionary["RemoteMachine"],
          clientApplication = dictionary["ClientApp"]
        },
        teamDefaultAreaPath = str1,
        teamCurrentIterationPath = str2,
        teamBacklogIterationPath = str3,
        maxCharsInEmailBody = num3,
        bytesPerChar = num2
      };
    }

    internal static ProjectProcessConfiguration GetProcessSettings(
      IVssRequestContext tfsRequestContext,
      CommonStructureProjectInfo project)
    {
      ProjectProcessConfiguration processConfiguration = tfsRequestContext.GetProjectProcessConfiguration(project.Uri, false);
      ProcessSettingsValidator.Validate(tfsRequestContext, processConfiguration, project.Uri, false);
      return processConfiguration;
    }

    internal static bool TryGetTeamSettings(
      IVssRequestContext tfsRequestContext,
      WebApiTeam team,
      out ITeamSettings teamSettings)
    {
      teamSettings = (ITeamSettings) null;
      try
      {
        ITeamConfigurationService service = tfsRequestContext.GetService<ITeamConfigurationService>();
        teamSettings = service.GetTeamSettings(tfsRequestContext, team, true, false);
        return true;
      }
      catch (SettingsException ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, nameof (TryGetTeamSettings), (Exception) ex);
        return false;
      }
    }
  }
}
