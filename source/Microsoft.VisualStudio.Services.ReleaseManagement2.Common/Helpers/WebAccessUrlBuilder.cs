// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers.WebAccessUrlBuilder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers
{
  public static class WebAccessUrlBuilder
  {
    public const string Self = "self";
    public const string Web = "web";
    public const string Logs = "logs";
    private const string ReleasePipelineHubRoute = "_release";
    private const string BuildArea = "_build";
    private const string WorkItemArea = "_workitems";
    private const string TfsGitCommitArea = "_git";
    private const string TfsGitCommitPath = "commit";
    private const string TfsVersionControlCommitArea = "_versionControl";
    private const string TfsVersionControlCommitPath = "changeset";
    private const string TfsGit = "TfsGit";
    private const string TfsVersionControl = "TfsVersionControl";

    public static Uri GetCollectionUrl(IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      string uriString = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, ServiceInstanceTypes.TFS, AccessMappingConstants.ClientAccessMappingMoniker);
      if (uriString == null)
      {
        string absoluteUri = requestContext.RequestUri().AbsoluteUri;
        uriString = absoluteUri.Substring(0, absoluteUri.IndexOf(HttpRouteCollectionExtensions.DefaultRoutePrefix, StringComparison.Ordinal));
      }
      return new Uri(uriString);
    }

    [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", Justification = "It is of type string")]
    public static ReferenceLinks ReplaceReferenceLink(
      string uriKey,
      ReferenceLinks referenceLinks,
      string uri)
    {
      ReferenceLinks referenceLinks1 = new ReferenceLinks();
      if (referenceLinks == null)
        return referenceLinks1;
      foreach (KeyValuePair<string, object> link in (IEnumerable<KeyValuePair<string, object>>) referenceLinks.Links)
      {
        if (!link.Key.Equals(uriKey, StringComparison.OrdinalIgnoreCase))
        {
          if (link.Value is IList<ReferenceLink>)
          {
            if (link.Value is IList<ReferenceLink> referenceLinkList)
            {
              foreach (ReferenceLink referenceLink in (IEnumerable<ReferenceLink>) referenceLinkList)
                referenceLinks1.AddLink(link.Key, referenceLink.Href);
            }
          }
          else if (link.Value is ReferenceLink && link.Value is ReferenceLink referenceLink1)
            referenceLinks1.AddLink(link.Key, referenceLink1.Href);
        }
        else
          referenceLinks1.AddLink(uriKey, uri);
      }
      return referenceLinks1;
    }

    [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "//TODO: Yadu")]
    [SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Taken logic from pull request event")]
    public static string GetReleaseWebAccessUri(
      IVssRequestContext requestContext,
      string projectName,
      int releaseId)
    {
      Uri collectionUrl = WebAccessUrlBuilder.GetCollectionUrl(requestContext);
      Dictionary<string, object> parameterValues = new Dictionary<string, object>()
      {
        {
          nameof (releaseId),
          (object) releaseId
        },
        {
          "_a",
          (object) "release-summary"
        }
      };
      return new Uri(WebAccessUrlBuilder.GetBaseUri(collectionUrl), WebAccessUrlBuilder.GetReleasesHubRelativeUrl(projectName) + WebAccessUrlBuilder.GetQueryParameter(parameterValues)).AbsoluteUri;
    }

    [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Taken logic from pull request event")]
    [SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Taken logic from pull request event")]
    public static string GetReleaseLogsWebAccessUri(
      IVssRequestContext requestContext,
      string projectName,
      int releaseId)
    {
      return WebAccessUrlBuilder.GetReleaseLogsWebAccessUri(WebAccessUrlBuilder.GetCollectionUrl(requestContext), projectName, releaseId);
    }

    [SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Taken logic from pull request event")]
    public static string GetReleaseLogsWebAccessUri(
      Uri collectionUri,
      string projectName,
      int releaseId)
    {
      return WebAccessUrlBuilder.GetReleaseLogsWebAccessUriWithEnvironmentId(collectionUri, projectName, releaseId, -1);
    }

    [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Taken logic from pull request event")]
    public static string GetReleaseLogsWebAccessUriWithEnvironmentId(
      IVssRequestContext requestContext,
      string projectName,
      int releaseId,
      int releaseEnvironmentId)
    {
      return WebAccessUrlBuilder.GetReleaseLogsWebAccessUriWithEnvironmentId(WebAccessUrlBuilder.GetCollectionUrl(requestContext), projectName, releaseId, releaseEnvironmentId);
    }

    [SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Taken logic from pull request event")]
    public static string GetReleaseLogsWebAccessUriWithEnvironmentId(
      Uri collectionUri,
      string projectName,
      int releaseId,
      int releaseEnvironmentId)
    {
      Dictionary<string, object> parameterValues = new Dictionary<string, object>()
      {
        {
          nameof (releaseId),
          (object) releaseId
        },
        {
          "_a",
          (object) "release-logs"
        }
      };
      if (releaseEnvironmentId > 0)
      {
        parameterValues.Add("environmentId", (object) releaseEnvironmentId);
        parameterValues.Add(nameof (releaseEnvironmentId), (object) releaseEnvironmentId);
      }
      return new Uri(WebAccessUrlBuilder.GetBaseUri(collectionUri), WebAccessUrlBuilder.GetReleasesHubRelativeUrl(projectName) + WebAccessUrlBuilder.GetQueryParameter(parameterValues)).AbsoluteUri;
    }

    public static string GetReleaseDefinitionWebAccessUri(
      IVssRequestContext requestContext,
      string projectName,
      int definitionId)
    {
      Uri collectionUrl = WebAccessUrlBuilder.GetCollectionUrl(requestContext);
      Dictionary<string, object> parameterValues = new Dictionary<string, object>()
      {
        {
          nameof (definitionId),
          (object) definitionId
        }
      };
      return new Uri(WebAccessUrlBuilder.GetBaseUri(collectionUrl), WebAccessUrlBuilder.GetReleasesHubRelativeUrl(projectName) + WebAccessUrlBuilder.GetQueryParameter(parameterValues)).AbsoluteUri;
    }

    [SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Taken logic from pull request event")]
    public static string GetEnvironmentSummaryWebAccessUri(
      IVssRequestContext requestContext,
      string projectName,
      int definitionId,
      int environmentId)
    {
      Uri collectionUrl = WebAccessUrlBuilder.GetCollectionUrl(requestContext);
      Dictionary<string, object> parameterValues = new Dictionary<string, object>()
      {
        {
          "_a",
          (object) "environment-summary"
        },
        {
          nameof (definitionId),
          (object) definitionId
        },
        {
          "definitionEnvironmentId",
          (object) environmentId
        }
      };
      return new Uri(WebAccessUrlBuilder.GetBaseUri(collectionUrl), WebAccessUrlBuilder.GetReleasesHubRelativeUrl(projectName) + WebAccessUrlBuilder.GetQueryParameter(parameterValues)).AbsoluteUri;
    }

    [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Url is represented as string")]
    public static string GetReleaseEnvironmentRestUri(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId)
    {
      return WebAccessUrlBuilder.GetResourceUrl(requestContext, "Release", ReleaseManagementApiConstants.ReleaseEnvironmentsLocationGuid, projectId, (object) new
      {
        releaseId = releaseId,
        environmentId = releaseEnvironmentId
      });
    }

    [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Url is represented as string")]
    public static string GetReleaseApprovalRestUri(
      IVssRequestContext requestContext,
      Guid projectId,
      int approvalId)
    {
      return WebAccessUrlBuilder.GetResourceUrl(requestContext, "Release", ReleaseManagementApiConstants.ApprovalsLocationGuid, projectId, (object) new
      {
        approvalId = approvalId
      });
    }

    [SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Taken logic from pull request event")]
    public static string GetBuildDefinitionWebAccessUri(
      Uri collectionUri,
      string projectName,
      int buildDefinitionId)
    {
      Dictionary<string, object> parameterValues = new Dictionary<string, object>()
      {
        {
          "_a",
          (object) "simple-process"
        },
        {
          "definitionId",
          (object) buildDefinitionId
        }
      };
      return new Uri(WebAccessUrlBuilder.GetBaseUri(collectionUri), WebAccessUrlBuilder.GetRelativeUrl(projectName, "_build") + WebAccessUrlBuilder.GetQueryParameter(parameterValues)).AbsoluteUri;
    }

    [SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Taken logic from pull request event")]
    public static string GetBuildWebAccessUri(Uri collectionUri, string projectName, int buildId)
    {
      Dictionary<string, object> parameterValues = new Dictionary<string, object>()
      {
        {
          "_a",
          (object) "summary"
        },
        {
          nameof (buildId),
          (object) buildId
        }
      };
      return new Uri(WebAccessUrlBuilder.GetBaseUri(collectionUri), WebAccessUrlBuilder.GetRelativeUrl(projectName, "_build") + WebAccessUrlBuilder.GetQueryParameter(parameterValues)).AbsoluteUri;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "It is a validated at parent level")]
    [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Doing similar to Build's Helper")]
    public static string GetCommitWebAccessUri(
      IVssRequestContext requestContext,
      string projectName,
      string repositoryType,
      string repositoryId,
      string commitId)
    {
      if (repositoryType.Equals("TfsGit", StringComparison.OrdinalIgnoreCase))
        return WebAccessUrlBuilder.GetTfsGitCommitWebAccessUri(WebAccessUrlBuilder.GetCollectionUrl(requestContext), projectName, repositoryId, commitId);
      if (repositoryType.Equals("TfsVersionControl", StringComparison.OrdinalIgnoreCase))
        return WebAccessUrlBuilder.GetTfsVersionControlCommitWebAccessUri(WebAccessUrlBuilder.GetCollectionUrl(requestContext), projectName, commitId.Substring(1));
      requestContext.Trace(1972003, TraceLevel.Info, "ReleaseManagementService", "Events", "Approval Event fired with {0} repositorty type {1} repository Id for primary artifact", (object) repositoryType, (object) repositoryId);
      return string.Empty;
    }

    [SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Taken logic from pull request event")]
    public static string GetTfsGitCommitWebAccessUri(
      Uri collectionUri,
      string projectName,
      string repository,
      string commitId)
    {
      string relativeUri = WebAccessUrlBuilder.GetRelativeUrl(projectName, "_git") + "/" + repository + "/commit/" + commitId;
      return new Uri(WebAccessUrlBuilder.GetBaseUri(collectionUri), relativeUri).AbsoluteUri;
    }

    [SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Taken logic from pull request event")]
    public static string GetTfsVersionControlCommitWebAccessUri(
      Uri collectionUri,
      string projectName,
      string changesetId)
    {
      string relativeUri = WebAccessUrlBuilder.GetRelativeUrl(projectName, "_versionControl") + "/changeset/" + changesetId;
      return new Uri(WebAccessUrlBuilder.GetBaseUri(collectionUri), relativeUri).AbsoluteUri;
    }

    [SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Taken logic from pull request event")]
    public static string GetWorkItemWebAccessUri(Uri collectionUri, string projectName, int? id)
    {
      Dictionary<string, object> parameterValues = new Dictionary<string, object>()
      {
        {
          "_a",
          (object) "edit"
        },
        {
          nameof (id),
          (object) id
        }
      };
      return new Uri(WebAccessUrlBuilder.GetBaseUri(collectionUri), WebAccessUrlBuilder.GetRelativeUrl(projectName, "_workitems") + WebAccessUrlBuilder.GetQueryParameter(parameterValues)).AbsoluteUri;
    }

    [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Doing similar to Build's RestHelper")]
    public static string GetReleaseRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId)
    {
      return WebAccessUrlBuilder.GetResourceUrl(requestContext, "Release", ReleaseManagementApiConstants.ReleasesLocationGuid, projectId, (object) new
      {
        releaseId = releaseId
      });
    }

    [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Doing similar to Build's RestHelper")]
    public static string GetReleaseDefinitionRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId)
    {
      return WebAccessUrlBuilder.GetResourceUrl(requestContext, "Release", ReleaseManagementApiConstants.ReleaseDefinitionsLocationId, projectId, (object) new
      {
        definitionId = definitionId
      });
    }

    [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Doing similar to Build's RestHelper")]
    public static string GetReleaseDefinitionRevisionRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int revision)
    {
      return WebAccessUrlBuilder.GetResourceUrl(requestContext, "Release", ReleaseManagementApiConstants.ReleaseDefinitionsLocationId, projectId, (object) new
      {
        definitionId = definitionId,
        revision = revision
      });
    }

    [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Doing similar to Build's RestHelper")]
    public static string GetReleaseLogsContainerUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId)
    {
      return WebAccessUrlBuilder.GetResourceUrl(requestContext, "Release", ReleaseManagementApiConstants.ReleaseLogsLocationGuid, projectId, (object) new
      {
        releaseId = releaseId
      });
    }

    [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "string is the expected return type")]
    public static string GetTaskLogUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId,
      int attemptId,
      int releaseDeployPhaseId,
      ReleaseTask task)
    {
      if (task == null)
        throw new ArgumentNullException(nameof (task));
      return releaseDeployPhaseId != 0 ? WebAccessUrlBuilder.GetReleaseTaskLogUrl(requestContext, projectId, releaseId, environmentId, releaseDeployPhaseId, task.Id) : WebAccessUrlBuilder.GetPipelineTaskLogUrl(requestContext, projectId, releaseId, environmentId, attemptId, task.TimelineRecordId);
    }

    [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Doing similar to Build's RestHelper")]
    public static string GetReleaseTaskLogUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId,
      int releaseDeployPhaseId,
      int taskId)
    {
      return releaseId > 0 && environmentId > 0 && releaseDeployPhaseId > 0 && taskId > 0 ? WebAccessUrlBuilder.GetResourceUrl(requestContext, "Release", ReleaseManagementApiConstants.ReleaseTasksGroupTaskLogsLocationGuid, projectId, (object) new
      {
        releaseId = releaseId,
        environmentId = environmentId,
        releaseDeployPhaseId = releaseDeployPhaseId,
        taskId = taskId
      }) : string.Empty;
    }

    [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "string is the expected return type")]
    public static string GetPipelineTaskLogUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId,
      int attemptId,
      Guid timelineRecordId)
    {
      return !(projectId == Guid.Empty) && releaseId > 0 && environmentId > 0 && attemptId > 0 && !(timelineRecordId == Guid.Empty) ? WebAccessUrlBuilder.GetResourceUrl(requestContext, "Release", ReleaseManagementApiConstants.ReleaseTasksLocationGuid2, projectId, (object) new
      {
        releaseId = releaseId,
        environmentId = environmentId,
        attemptId = attemptId,
        timelineId = timelineRecordId
      }) : string.Empty;
    }

    [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Doing similar to Build's RestHelper")]
    public static string GetDeploymentGateLogUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId,
      int gateId,
      int taskId)
    {
      return !(projectId == Guid.Empty) && releaseId > 0 && environmentId > 0 && gateId > 0 && taskId > 0 ? WebAccessUrlBuilder.GetResourceUrl(requestContext, "Release", ReleaseManagementApiConstants.GreenlightingGateLogsLocationGuid, projectId, (object) new
      {
        releaseId = releaseId,
        environmentId = environmentId,
        gateId = gateId,
        taskId = taskId
      }) : string.Empty;
    }

    [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "Url is represented as string")]
    public static string GetDeploymentBadgeRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseDefinitionId,
      int environmentId)
    {
      return WebAccessUrlBuilder.GetResourceUrl(requestContext, "Release", ReleaseManagementApiConstants.DeploymentBadgeLocationGuid, projectId, (object) new
      {
        projectId = projectId,
        releaseDefinitionId = releaseDefinitionId,
        environmentId = environmentId
      });
    }

    private static Uri GetBaseUri(Uri uri)
    {
      string str = uri.ToString();
      return new Uri(str + (str.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? string.Empty : "/"));
    }

    private static string GetReleasesHubRelativeUrl(string projectName) => projectName + "/_release";

    private static string GetRelativeUrl(string projectName, string areaName) => projectName + "/" + areaName;

    private static string GetQueryParameter(Dictionary<string, object> parameterValues)
    {
      string queryParameter = string.Empty;
      foreach (string key in parameterValues.Keys)
      {
        string str;
        if (!string.IsNullOrEmpty(queryParameter))
          str = queryParameter + "&" + key + "=" + parameterValues[key]?.ToString();
        else
          str = "?" + key + "=" + parameterValues[key]?.ToString();
        queryParameter = str;
      }
      return queryParameter;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We want to handle any type of exception here")]
    private static string GetResourceUrl(
      IVssRequestContext requestContext,
      string serviceArea,
      Guid locationId,
      Guid projectId,
      object routeValues)
    {
      if (string.IsNullOrEmpty(serviceArea))
        throw new ArgumentNullException(nameof (serviceArea));
      ILocationService locationService = requestContext != null ? requestContext.GetService<ILocationService>() : throw new ArgumentNullException(nameof (requestContext));
      try
      {
        return locationService.GetResourceUri(requestContext, serviceArea, locationId, projectId, routeValues).AbsoluteUri;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1973128, TraceLevel.Error, "ReleaseManagementService", "Events", ex);
        return string.Empty;
      }
    }
  }
}
