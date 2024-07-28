// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.Configuration;
using Microsoft.TeamFoundation.Server.WebAccess.Controllers;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Resources;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class BuildAreaRegistration : AreaRegistration
  {
    public override string AreaName => "Build";

    private static bool ShouldRegisterHub(TfsWebContext tfsWebContext) => false;

    public override void RegisterArea(AreaRegistrationContext context)
    {
      ScriptRegistration.RegisterBundledArea(this.AreaName, (Func<ResourceManager>) (() => BuildResources.ResourceManager), "TFS");
      BuiltinPluginManager.RegisterPluginBase("TFS.Build", "Build/Scripts/");
      CompatibilityController.RegisterCompatRouter(new CompatibilityController.RouteBackwardCompatibileUrl(this.RouteCompatibleUrl));
    }

    public string RouteCompatibleUrl(
      CompatibilityController controller,
      string page,
      TfsLocator locator,
      ProjectInfo projectInfo)
    {
      TeamFoundationHostManagementService service = controller.TfsRequestContext.GetService<TeamFoundationHostManagementService>();
      IVssRequestContext vssRequestContext = controller.TfsRequestContext.To(TeamFoundationHostType.Application);
      IVssRequestContext requestContext = vssRequestContext;
      Guid collectionGuid = locator.CollectionGuid;
      IdentityDescriptor userContext = vssRequestContext.UserContext;
      using (IVssRequestContext collectionRequestCtx = service.BeginUserRequest(requestContext, collectionGuid, userContext, false))
      {
        if (page.EndsWith("build.aspx", StringComparison.OrdinalIgnoreCase))
        {
          string parameterFromRequest1 = BuildHelpers.ExtractBuildParameterFromRequest(controller.Request, BuildHelpers.CompatUrlParameters.BuildId, false);
          string parameterFromRequest2 = BuildHelpers.ExtractBuildParameterFromRequest(controller.Request, BuildHelpers.CompatUrlParameters.BuildUri, false);
          string parameterFromRequest3 = BuildHelpers.ExtractBuildParameterFromRequest(controller.Request, BuildHelpers.CompatUrlParameters.DefinitionId, false);
          string parameterFromRequest4 = BuildHelpers.ExtractBuildParameterFromRequest(controller.Request, BuildHelpers.CompatUrlParameters.DefinitionUri, false);
          string parameterFromRequest5 = BuildHelpers.ExtractBuildParameterFromRequest(controller.Request, BuildHelpers.CompatUrlParameters.Action, false);
          string parameterFromRequest6 = BuildHelpers.ExtractBuildParameterFromRequest(controller.Request, BuildHelpers.CompatUrlParameters.Sender, false);
          RouteValueDictionary routeValues = new RouteValueDictionary();
          foreach (KeyValuePair<string, string> keyValuePair in BuildHelpers.ExtractOtherParametersFromRequest(controller.Request))
            routeValues[keyValuePair.Key] = (object) keyValuePair.Value;
          if (!string.IsNullOrEmpty(parameterFromRequest1))
            return this.RouteCompatibleBuildAction(controller, collectionRequestCtx, routeValues, parameterFromRequest1, parameterFromRequest5, parameterFromRequest6);
          if (!string.IsNullOrEmpty(parameterFromRequest2))
          {
            string toolSpecificId = LinkingUtilities.DecodeUri(parameterFromRequest2).ToolSpecificId;
            return this.RouteCompatibleBuildAction(controller, collectionRequestCtx, routeValues, toolSpecificId, parameterFromRequest5, parameterFromRequest6);
          }
          if (!string.IsNullOrEmpty(parameterFromRequest3))
            return this.RouteCompatibleDefinitionAction(controller, collectionRequestCtx, routeValues, parameterFromRequest3, parameterFromRequest5, parameterFromRequest6);
          if (!string.IsNullOrEmpty(parameterFromRequest4))
          {
            string toolSpecificId = LinkingUtilities.DecodeUri(parameterFromRequest4).ToolSpecificId;
            return this.RouteCompatibleDefinitionAction(controller, collectionRequestCtx, routeValues, toolSpecificId, parameterFromRequest5, parameterFromRequest6);
          }
          string actionName = "index";
          string controllerName = "build";
          string parameterFromRequest7 = BuildHelpers.ExtractBuildParameterFromRequest(controller.Request, BuildHelpers.CompatUrlParameters.KindOfBuilds, false);
          routeValues["_a"] = !string.IsNullOrEmpty(parameterFromRequest7) ? (object) parameterFromRequest7 : (object) "completed";
          return controller.Url.Action(actionName, controllerName, routeValues);
        }
      }
      return (string) null;
    }

    private string RouteCompatibleDefinitionAction(
      CompatibilityController controller,
      IVssRequestContext collectionRequestCtx,
      RouteValueDictionary routeValues,
      string definitionId,
      string action,
      string sender)
    {
      string actionName = "index";
      string controllerName = "build";
      string parameterFromRequest1 = BuildHelpers.ExtractBuildParameterFromRequest(controller.Request, BuildHelpers.CompatUrlParameters.ProjectName, false);
      string parameterFromRequest2 = BuildHelpers.ExtractBuildParameterFromRequest(controller.Request, BuildHelpers.CompatUrlParameters.DefinitionTemplateId, false);
      string parameterFromRequest3 = BuildHelpers.ExtractBuildParameterFromRequest(controller.Request, BuildHelpers.CompatUrlParameters.KindOfBuilds, false);
      List<string> values = new List<string>();
      string str1 = LinkingUtilities.EncodeUri(new ArtifactId("Build", "Definition", definitionId));
      TeamFoundationBuildService service = collectionRequestCtx.GetService<TeamFoundationBuildService>();
      IVssRequestContext requestContext = collectionRequestCtx;
      List<string> uris = new List<string>();
      uris.Add(str1);
      Guid projectId = new Guid();
      if (service.QueryBuildDefinitionsByUri(requestContext, (IList<string>) uris, (IList<string>) null, QueryOptions.None, projectId).Definitions.FirstOrDefault<Microsoft.TeamFoundation.Build.Server.BuildDefinition>() != null)
        action = "viewxamlbuilds";
      if (action.Equals("new", StringComparison.OrdinalIgnoreCase))
      {
        if (!string.IsNullOrEmpty(parameterFromRequest2))
        {
          values.Add("templateId=" + parameterFromRequest2);
          values.Add("_a=simple-process");
        }
        else
          values.Add("_a=new");
      }
      else if (action.Equals("open", StringComparison.OrdinalIgnoreCase))
      {
        actionName = "definitionEditor";
        values.Add("definitionId=" + definitionId);
        values.Add("_a=simple-process");
      }
      else if (action.Equals("queuebuild", StringComparison.OrdinalIgnoreCase))
      {
        values.Add("definitionId=" + definitionId);
        string str2 = !string.IsNullOrEmpty(parameterFromRequest3) ? parameterFromRequest3 : "queuebuild";
        values.Add("_a=" + str2);
      }
      else if (action.Equals("viewbuilds", StringComparison.OrdinalIgnoreCase))
      {
        if (definitionId != "0")
          values.Add("definitionId=" + definitionId);
      }
      else if (action.Equals("viewxamlbuilds", StringComparison.OrdinalIgnoreCase))
      {
        actionName = "xaml";
        if (definitionId == "0")
        {
          values.Add("definitionType=1");
          string str3 = !string.IsNullOrEmpty(parameterFromRequest3) ? parameterFromRequest3 : "completed";
          values.Add("_a=" + str3);
        }
        else
        {
          routeValues[nameof (definitionId)] = (object) definitionId;
          routeValues["definitionUri"] = (object) str1;
          routeValues["_a"] = (object) parameterFromRequest3;
        }
      }
      routeValues["routeArea"] = (object) "";
      routeValues["serviceHost"] = (object) collectionRequestCtx.ServiceHost;
      routeValues["project"] = (object) parameterFromRequest1;
      routeValues["team"] = (object) "";
      string str4 = controller.Url.Action(actionName, controllerName, routeValues);
      if (values.Count > 0)
        str4 = str4 + "?" + string.Join("&", (IEnumerable<string>) values);
      return str4;
    }

    private string RouteCompatibleBuildAction(
      CompatibilityController controller,
      IVssRequestContext collectionRequestCtx,
      RouteValueDictionary routeValues,
      string buildIdString,
      string action,
      string sender)
    {
      string controllerName = "build";
      int result;
      if (!int.TryParse(buildIdString, out result))
        throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, BuildServerResources.InvalidBuildParameterSpecified, (object) "builduri"), HttpStatusCode.BadRequest);
      BuildData buildById = collectionRequestCtx.GetService<IBuildServiceInternal>().GetBuildById(collectionRequestCtx, result, includeDeleted: true);
      string projectName;
      string actionName;
      if (buildById != null)
      {
        if (!collectionRequestCtx.GetService<IProjectService>().TryGetProjectName(collectionRequestCtx, buildById.ProjectId, out projectName))
          projectName = string.Empty;
        actionName = "index";
        routeValues["buildId"] = (object) result;
      }
      else
      {
        string str = LinkingUtilities.EncodeUri(new ArtifactId("Build", "Build", result.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        TeamFoundationBuildService service = collectionRequestCtx.GetService<TeamFoundationBuildService>();
        IVssRequestContext requestContext = collectionRequestCtx;
        List<string> uris = new List<string>();
        uris.Add(str);
        Guid projectId = new Guid();
        using (TeamFoundationDataReader foundationDataReader = service.QueryBuildsByUri(requestContext, (IList<string>) uris, (IList<string>) null, QueryOptions.None, Microsoft.TeamFoundation.Build.Server.QueryDeletedOption.ExcludeDeleted, projectId, false))
        {
          projectName = (foundationDataReader.Current<BuildQueryResult>().Builds.FirstOrDefault<BuildDetail>() ?? throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, BuildResources.BuildNotFound, (object) str), HttpStatusCode.NotFound)).TeamProject;
          actionName = "index";
          routeValues["buildId"] = (object) result;
        }
      }
      routeValues["routeArea"] = (object) "";
      routeValues["serviceHost"] = (object) collectionRequestCtx.ServiceHost;
      routeValues["project"] = (object) projectName;
      routeValues["team"] = (object) "";
      return controller.Url.Action(actionName, controllerName, routeValues);
    }
  }
}
