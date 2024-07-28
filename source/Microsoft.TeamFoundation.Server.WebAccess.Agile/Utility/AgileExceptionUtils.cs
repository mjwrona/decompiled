// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility.AgileExceptionUtils
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility
{
  internal static class AgileExceptionUtils
  {
    internal static SettingsErrorModel HandleAgileException(
      TfsWebContext tfsWebContext,
      SettingsException exception,
      UrlHelper urlHelper)
    {
      ArgumentUtility.CheckForNull<TfsWebContext>(tfsWebContext, nameof (tfsWebContext));
      ArgumentUtility.CheckForNull<SettingsException>(exception, nameof (exception));
      ArgumentUtility.CheckForNull<UrlHelper>(urlHelper, nameof (urlHelper));
      SettingsErrorModel settingsErrorModel = new SettingsErrorModel();
      switch (exception)
      {
        case ProjectSettingsException _:
          tfsWebContext.TfsRequestContext.TraceException(290233, TraceLevel.Warning, "Agile", TfsTraceLayers.BusinessLogic, (Exception) exception);
          if (!tfsWebContext.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
          {
            switch (exception)
            {
              case MissingProjectSettingsException _:
                RouteValueDictionary routeValueDictionary = new RouteValueDictionary()
                {
                  ["team"] = (object) string.Empty,
                  ["routeArea"] = (object) "Admin"
                };
                settingsErrorModel.ErrorText = WACommonResources.FeatureEnablementSettings_Error_Missing_Admin;
                break;
              case InvalidProjectSettingsException _:
                settingsErrorModel.ErrorText = WACommonResources.FeatureEnablementSettings_Error_Invalid_Admin;
                settingsErrorModel.ExceptionMessages = (IEnumerable<string>) ((InvalidProjectSettingsException) exception).GetErrors().ToArray<string>();
                settingsErrorModel.SetLink(WACommonResources.FeatureEnablementSettings_Error_Link_Invalid_Admin, WACommonResources.FeatureEnablementSettings_Error_LinkText_Invalid_Admin);
                break;
            }
          }
          else
          {
            switch (exception)
            {
              case MissingProjectSettingsException _:
                settingsErrorModel.ErrorText = WACommonResources.FeatureEnablementSettings_Error_Missing;
                break;
              case InvalidProjectSettingsException _:
                settingsErrorModel.ErrorText = WACommonResources.FeatureEnablementSettings_Error_Invalid;
                settingsErrorModel.ExceptionMessages = (IEnumerable<string>) ((InvalidProjectSettingsException) exception).GetErrors().ToArray<string>();
                break;
            }
          }
          break;
        case InvalidTeamSettingsException _:
          tfsWebContext.TfsRequestContext.TraceException(290232, TraceLevel.Warning, "Agile", TfsTraceLayers.BusinessLogic, (Exception) exception);
          TeamSettingsFields invalidFields = ((InvalidTeamSettingsException) exception).InvalidFields;
          IWebTeamContext webTeamContext = tfsWebContext.TfsRequestContext.GetWebTeamContext();
          string name = webTeamContext.Team != null ? webTeamContext.Team.Name : "";
          if (invalidFields.HasFlag((Enum) TeamSettingsFields.BacklogIteration))
          {
            settingsErrorModel.ErrorText = AgileServerResources.AgileSettings_ErrorMissingBacklogIteration;
            settingsErrorModel.SetLink(urlHelper.Action("index", "AdminWork", (object) new
            {
              team = name,
              routeArea = "Admin",
              _a = "iterations"
            }), AgileServerResources.AgileSettings_ErrorMissingBacklogIterationLinkText);
            break;
          }
          if (invalidFields.HasFlag((Enum) TeamSettingsFields.TeamField))
          {
            settingsErrorModel.ErrorText = AgileServerResources.AgileSettings_ErrorMissingWorkItemSettings;
            settingsErrorModel.SetLink(urlHelper.Action("index", "AdminWork", (object) new
            {
              team = name,
              routeArea = "Admin",
              _a = "areas"
            }), AgileServerResources.AgileSettings_ErrorMissingWorkItemSettingsLinkText);
            break;
          }
          if (!invalidFields.HasFlag((Enum) TeamSettingsFields.TeamIteration))
            return (SettingsErrorModel) null;
          settingsErrorModel.ErrorText = AgileServerResources.AgileSettings_ErrorMissingSprintIterations;
          settingsErrorModel.SetLink(urlHelper.Action("index", "AdminWork", (object) new
          {
            team = name,
            routeArea = "Admin",
            _a = "iterations"
          }), AgileServerResources.AgileSettings_ErrorMissingSprintIterationsLinkText);
          break;
        default:
          tfsWebContext.TfsRequestContext.TraceException(599999, "Agile", TfsTraceLayers.BusinessLogic, (Exception) exception);
          return (SettingsErrorModel) null;
      }
      return settingsErrorModel;
    }
  }
}
