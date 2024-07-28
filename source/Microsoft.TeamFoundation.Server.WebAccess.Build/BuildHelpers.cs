// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildHelpers
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public static class BuildHelpers
  {
    public const int MaxBuildsPerDefinitionCount = 1000;
    public const int MaxDefinitionsCount = 10000;

    public static DateTime GetMinFinishTime(string dateFilter, DateTime now)
    {
      switch (dateFilter)
      {
        case "l24h":
          return now.AddHours(-24.0);
        case "l48h":
          return now.AddHours(-48.0);
        case "l7d":
          return now.AddDays(-7.0);
        case "l14d":
          return now.AddDays(-14.0);
        case "l28d":
          return now.AddDays(-28.0);
        case "any":
          return DateTime.MinValue;
        default:
          return new DateTime(now.Year, now.Month, now.Day);
      }
    }

    public static bool BuildFinished(this BuildDetail build) => build.FinishTime > DateTime.MinValue || build.Status == Microsoft.TeamFoundation.Build.Server.BuildStatus.Failed || build.Status == Microsoft.TeamFoundation.Build.Server.BuildStatus.PartiallySucceeded || build.Status == Microsoft.TeamFoundation.Build.Server.BuildStatus.Stopped || build.Status == Microsoft.TeamFoundation.Build.Server.BuildStatus.Succeeded;

    public static string BuildSummaryText(this BuildDetail build, TfsWebContext tfsWebContext)
    {
      string format;
      switch (build.Status)
      {
        case Microsoft.TeamFoundation.Build.Server.BuildStatus.PartiallySucceeded:
          format = BuildServerResources.BuildDefinitionTilePartiallySucceeded;
          break;
        case Microsoft.TeamFoundation.Build.Server.BuildStatus.Failed:
          format = BuildServerResources.BuildDefinitionTileFailed;
          break;
        default:
          format = BuildServerResources.BuildDefinitionTileCompleted;
          break;
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, (object) build.FinishTime.Friendly());
    }

    public static bool TryGetXamlBuildDefinition(
      IVssRequestContext tfsRequestContext,
      string definitionUri,
      out Microsoft.TeamFoundation.Build.Server.BuildDefinition definition)
    {
      definition = (Microsoft.TeamFoundation.Build.Server.BuildDefinition) null;
      try
      {
        tfsRequestContext.TraceEnter(515050, "WebAccess.Build", TfsTraceLayers.BusinessLogic, "TryGetBuildDefinition");
        ArgumentUtility.CheckForNull<string>(definitionUri, nameof (definitionUri));
        TeamFoundationBuildService service = tfsRequestContext.GetService<TeamFoundationBuildService>();
        tfsRequestContext.Trace(515053, TraceLevel.Info, "WebAccess.Build", TfsTraceLayers.BusinessLogic, "Getting build definition details.  definitionUri: {0}", (object) definitionUri);
        IVssRequestContext requestContext = tfsRequestContext;
        string[] uris = new string[1]{ definitionUri };
        Guid projectId = new Guid();
        BuildDefinitionQueryResult definitionQueryResult = service.QueryBuildDefinitionsByUri(requestContext, (IList<string>) uris, (IList<string>) null, QueryOptions.Definitions, projectId);
        definition = definitionQueryResult.Definitions.FirstOrDefault<Microsoft.TeamFoundation.Build.Server.BuildDefinition>();
      }
      catch (Exception ex)
      {
        tfsRequestContext.TraceException(515057, "WebAccess.Build", TfsTraceLayers.BusinessLogic, ex);
      }
      finally
      {
        tfsRequestContext.TraceLeave(515060, "WebAccess.Build", TfsTraceLayers.BusinessLogic, "TryGetBuildDefinition");
      }
      return definition != null;
    }

    public static bool TryGetBuildDefinition(
      IVssRequestContext tfsRequestContext,
      DefinitionReference definitionReference,
      out Microsoft.TeamFoundation.Build.WebApi.BuildDefinition definition)
    {
      definition = (Microsoft.TeamFoundation.Build.WebApi.BuildDefinition) null;
      try
      {
        tfsRequestContext.TraceEnter(515050, "WebAccess.Build", TfsTraceLayers.BusinessLogic, nameof (TryGetBuildDefinition));
        ArgumentUtility.CheckForNull<DefinitionReference>(definitionReference, nameof (definitionReference));
        IBuildDefinitionService service = tfsRequestContext.GetService<IBuildDefinitionService>();
        tfsRequestContext.Trace(515053, TraceLevel.Info, "WebAccess.Build", TfsTraceLayers.BusinessLogic, "Getting build definition details.  definitionUri: {0}", (object) definitionReference.Uri);
        definition = service.GetDefinition(tfsRequestContext, definitionReference.Project.Id, definitionReference.Id, definitionReference.Revision).ToWebApiBuildDefinition(tfsRequestContext, VssRestApiVersionsRegistry.GetLatestVersion());
      }
      catch (Exception ex)
      {
        tfsRequestContext.TraceException(515057, "WebAccess.Build", TfsTraceLayers.BusinessLogic, ex);
      }
      finally
      {
        tfsRequestContext.TraceLeave(515060, "WebAccess.Build", TfsTraceLayers.BusinessLogic, nameof (TryGetBuildDefinition));
      }
      return definition != null;
    }

    public static bool TryGetBuildDefinition(
      IVssRequestContext tfsRequestContext,
      int definitionId,
      Guid projectId,
      out Microsoft.TeamFoundation.Build.WebApi.BuildDefinition definition)
    {
      definition = (Microsoft.TeamFoundation.Build.WebApi.BuildDefinition) null;
      try
      {
        tfsRequestContext.TraceEnter(515050, "WebAccess.Build", TfsTraceLayers.BusinessLogic, nameof (TryGetBuildDefinition));
        ArgumentUtility.CheckForOutOfRange(definitionId, nameof (definitionId), 1);
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        IBuildDefinitionService service = tfsRequestContext.GetService<IBuildDefinitionService>();
        tfsRequestContext.Trace(515053, TraceLevel.Info, "WebAccess.Build", TfsTraceLayers.BusinessLogic, "Getting build definition details.  definitionId: {0}", (object) definitionId);
        definition = service.GetDefinition(tfsRequestContext, projectId, definitionId).ToWebApiBuildDefinition(tfsRequestContext, VssRestApiVersionsRegistry.GetLatestVersion());
      }
      catch (Exception ex)
      {
        tfsRequestContext.TraceException(515057, "WebAccess.Build", TfsTraceLayers.BusinessLogic, ex);
      }
      finally
      {
        tfsRequestContext.TraceLeave(515060, "WebAccess.Build", TfsTraceLayers.BusinessLogic, nameof (TryGetBuildDefinition));
      }
      return definition != null;
    }

    public static string ExtractBuildUriFromRequest(HttpRequestBase request)
    {
      ArgumentUtility.CheckForNull<HttpRequestBase>(request, nameof (request));
      string artifactUri = (string) null;
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        {
          "builduri",
          "{0}"
        },
        {
          "uri",
          "{0}"
        },
        {
          "id",
          "vstfs:///Build/Build/{0}"
        }
      };
      string str1 = (string) null;
      foreach (KeyValuePair<string, string> keyValuePair in dictionary)
      {
        string str2 = request.Params[keyValuePair.Key];
        if (!string.IsNullOrEmpty(str2))
        {
          str1 = keyValuePair.Key;
          artifactUri = string.Format((IFormatProvider) CultureInfo.InvariantCulture, keyValuePair.Value, (object) str2);
          break;
        }
      }
      if (string.IsNullOrEmpty(artifactUri))
        throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, BuildServerResources.SpecifyBuildDetailParameters, (object) string.Join(",", dictionary.Keys.ToArray<string>())), HttpStatusCode.BadRequest);
      return LinkingUtilities.IsUriWellFormed(artifactUri) ? artifactUri : throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, BuildServerResources.InvalidBuildParameterSpecified, (object) str1), HttpStatusCode.BadRequest);
    }

    public static string ExtractBuildParameterFromRequest(
      HttpRequestBase request,
      string parameterName,
      bool isRequired)
    {
      ArgumentUtility.CheckForNull<HttpRequestBase>(request, nameof (request));
      ArgumentUtility.CheckStringForNullOrEmpty(parameterName, nameof (parameterName));
      string empty = request.Params[parameterName];
      if (string.IsNullOrEmpty(empty))
      {
        if (isRequired)
          throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, BuildServerResources.SpecifyBuildDetailParameters, (object) string.Join(",", BuildHelpers.CompatUrlParameters.All)), HttpStatusCode.BadRequest);
        empty = string.Empty;
      }
      return empty;
    }

    public static Dictionary<string, string> ExtractOtherParametersFromRequest(
      HttpRequestBase request)
    {
      ArgumentUtility.CheckForNull<HttpRequestBase>(request, nameof (request));
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (string key in BuildHelpers.CompatUrlParameters.All)
        dictionary[key] = key;
      Dictionary<string, string> parametersFromRequest = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (string allKey in request.QueryString.AllKeys)
      {
        if (allKey != null && !dictionary.ContainsKey(allKey))
          parametersFromRequest[allKey] = request.QueryString[allKey];
      }
      return parametersFromRequest;
    }

    public static string GetEditorBaseUrl(
      IVssRequestContext requestContext,
      TfsWebContext webContext,
      bool isYamlEditor = false)
    {
      IContributionRoutingService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<IContributionRoutingService>();
      RouteValueDictionary levelRouteValues = NavigationHelpers.GetCurrentNavigationLevelRouteValues(webContext.NavigationContext);
      string str = isYamlEditor ? "ms.vss-build-web.ci-designer-hub" : "ms.vss-ciworkflow.build-ci-hub";
      IVssRequestContext requestContext1 = requestContext;
      string contributionId = str;
      RouteValueDictionary routeValues = levelRouteValues;
      return service.RouteUrl(requestContext1, contributionId, routeValues);
    }

    public static class CompatUrlParameters
    {
      public static readonly string Action = "action";
      public static readonly string BuildId = "buildid";
      public static readonly string BuildUri = "builduri";
      public static readonly string CollectionId = "pcguid";
      public static readonly string DefinitionId = "definitionid";
      public static readonly string DefinitionUri = "definitionuri";
      public static readonly string DefinitionTemplateId = "templateid";
      public static readonly string KindOfBuilds = "kindofbuilds";
      public static readonly string ProjectId = "projectId";
      public static readonly string ProjectName = "projectName";
      public static readonly string Sender = "sender";

      public static IEnumerable<string> All => (IEnumerable<string>) new string[11]
      {
        BuildHelpers.CompatUrlParameters.Action,
        BuildHelpers.CompatUrlParameters.BuildId,
        BuildHelpers.CompatUrlParameters.BuildUri,
        BuildHelpers.CompatUrlParameters.CollectionId,
        BuildHelpers.CompatUrlParameters.DefinitionId,
        BuildHelpers.CompatUrlParameters.DefinitionUri,
        BuildHelpers.CompatUrlParameters.DefinitionTemplateId,
        BuildHelpers.CompatUrlParameters.KindOfBuilds,
        BuildHelpers.CompatUrlParameters.ProjectId,
        BuildHelpers.CompatUrlParameters.ProjectName,
        BuildHelpers.CompatUrlParameters.Sender
      };
    }
  }
}
