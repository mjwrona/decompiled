// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Routes.BuildRouteService
// Assembly: Microsoft.TeamFoundation.Build2.Routes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3759BAC-2581-46BE-B787-E219FAA96ED4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Routes.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FileContainer;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Routes
{
  public sealed class BuildRouteService : BaseRouteService, IBuildRouteService, IVssFrameworkService
  {
    public string GetBuildWebUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      bool isXAML = false)
    {
      Dictionary<string, object> routeValues = new Dictionary<string, object>()
      {
        {
          nameof (buildId),
          (object) buildId
        }
      };
      return this.ConstructWebLink(requestContext, projectId, isXAML ? "build" : "build/results", isXAML ? "xaml" : (string) null, (IDictionary<string, object>) routeValues);
    }

    public string GetDefinitionWebUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId)
    {
      return this.GetPipelineWebUrl(requestContext, projectId, definitionId);
    }

    public string GetDefinitionDesignerUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId)
    {
      Dictionary<string, object> routeValues = new Dictionary<string, object>()
      {
        {
          "id",
          (object) definitionId
        },
        {
          "_a",
          (object) "edit-build-definition"
        }
      };
      return this.ConstructWebLink(requestContext, projectId, "build", "designer", (IDictionary<string, object>) routeValues);
    }

    public string GetNewDefinitionDesignerUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      string selectedRepositoryType,
      string connectionId = null,
      IEnumerable<DefinitionTriggerType> triggers = null,
      bool useNewDesigner = false,
      string requestSource = null,
      string repositoryId = null,
      string telemetrySession = null)
    {
      Dictionary<string, object> routeValues = new Dictionary<string, object>(4);
      if (triggers != null && triggers.Any<DefinitionTriggerType>())
      {
        string str = string.Join(",", triggers.Select<DefinitionTriggerType, string>((Func<DefinitionTriggerType, string>) (t => t.ToString())));
        routeValues.Add(nameof (triggers), (object) str);
      }
      if (!string.IsNullOrEmpty(connectionId))
        routeValues.Add(nameof (connectionId), (object) connectionId);
      if (!string.IsNullOrEmpty(repositoryId))
        routeValues.Add(nameof (repositoryId), (object) repositoryId);
      if (!string.IsNullOrEmpty(telemetrySession))
        routeValues.Add(nameof (telemetrySession), (object) telemetrySession);
      if (useNewDesigner)
      {
        routeValues.Add("sourceProvider", (object) selectedRepositoryType);
        if (requestSource != null)
          routeValues.Add("entryPoint", (object) requestSource);
        return this.ConstructWebLink(requestContext, projectId, "apps/hub", "ms.vss-build-web.ci-designer-hub", (IDictionary<string, object>) routeValues);
      }
      routeValues.Add("_a", (object) "build-definition-getting-started");
      routeValues.Add("repositoryType", (object) selectedRepositoryType);
      return this.ConstructWebLink(requestContext, projectId, "build", "designer", (IDictionary<string, object>) routeValues);
    }

    public string GetEditPipelineDesignerUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      string pipelineId,
      string nonce = null)
    {
      Dictionary<string, object> routeValues = new Dictionary<string, object>(2);
      if (!string.IsNullOrEmpty(pipelineId))
        routeValues.Add(nameof (pipelineId), (object) pipelineId);
      if (!string.IsNullOrEmpty(nonce))
        routeValues.Add(nameof (nonce), (object) nonce);
      return this.ConstructWebLink(requestContext, projectId, "apps/hub", "ms.vss-build-web.ci-designer-hub", (IDictionary<string, object>) routeValues);
    }

    public string GetArtifactWebUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      string type,
      string data)
    {
      Dictionary<string, object> routeValues = new Dictionary<string, object>()
      {
        {
          nameof (type),
          (object) type
        },
        {
          "resource",
          (object) data
        }
      };
      return this.ConstructWebLink(requestContext, projectId, "build", "artifact", (IDictionary<string, object>) routeValues);
    }

    public string GetDefinitionRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      int? definitionRevision)
    {
      return this.GetResourceUrl(requestContext, BuildResourceIds.Definitions, projectId, (object) new
      {
        definitionId = definitionId
      }, (Func<Uri, string>) (uri => !definitionRevision.HasValue ? uri.AbsoluteUri : uri.AppendQuery("revision", definitionRevision.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture)).AbsoluteUri));
    }

    public string GetDefinitionBadgeUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId)
    {
      return this.GetResourceUrl(requestContext, "build", BuildResourceIds.BuildDefinitionBadge, (object) new
      {
        project = projectId,
        definitionId = definitionId
      });
    }

    public string GetBuildRestUrl(IVssRequestContext requestContext, Guid projectId, int buildId) => this.GetResourceUrl(requestContext, BuildResourceIds.Builds, projectId, (object) new
    {
      buildId = buildId
    });

    public string GetBuildAttachmentRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      Guid timelineId,
      Guid timelineRecordId,
      string type,
      string name)
    {
      return this.GetResourceUrl(requestContext, BuildResourceIds.Attachment, projectId, (object) new
      {
        buildId = buildId,
        timelineId = timelineId,
        recordId = timelineRecordId,
        type = type,
        name = name
      });
    }

    public string GetBuildLogsRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId)
    {
      return this.GetResourceUrl(requestContext, BuildResourceIds.BuildLogs, projectId, (object) new
      {
        buildId = buildId
      });
    }

    public string GetBuildLogRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      int logId)
    {
      return this.GetResourceUrl(requestContext, BuildResourceIds.BuildLogs, projectId, (object) new
      {
        buildId = buildId,
        logId = logId
      });
    }

    public string GetBuildSourcesRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId)
    {
      return this.GetResourceUrl(requestContext, BuildResourceIds.Sources, projectId, (object) new
      {
        buildId = buildId
      });
    }

    public string GetQueueRestUrl(IVssRequestContext requestContext, int queueId) => this.GetResourceUrl(requestContext, "build", BuildResourceIds.Queues, (object) new
    {
      controllerId = queueId
    });

    public string GetTimelineRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      Guid? timelineId)
    {
      return this.GetResourceUrl(requestContext, BuildResourceIds.Timelines, projectId, (object) new
      {
        buildId = buildId,
        timelineId = timelineId
      });
    }

    public string GetArtifactRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      string artifactName,
      ApiResourceVersion resourceVersion)
    {
      return this.GetResourceUrl(requestContext, BuildResourceIds.Artifacts, projectId, (object) new
      {
        buildId = buildId
      }, (Func<Uri, string>) (uri =>
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (artifactName), artifactName);
        collection.Add("api-version", resourceVersion.ToString());
        List<KeyValuePair<string, string>> queryValues = collection;
        return uri.AppendQuery((IEnumerable<KeyValuePair<string, string>>) queryValues).AbsoluteUri;
      }));
    }

    public string GetBranchBadgeRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      string repoType,
      string branchName,
      string repoId)
    {
      return this.GetResourceUrl(requestContext, BuildResourceIds.BuildBadge, projectId, (object) new
      {
        repoType = repoType
      }, (Func<Uri, string>) (uri =>
      {
        if (!string.IsNullOrEmpty(branchName))
          uri = uri.AppendQuery(nameof (branchName), branchName);
        if (!string.IsNullOrEmpty(repoId))
          uri = uri.AppendQuery(nameof (repoId), repoId);
        return uri.AbsoluteUri;
      }));
    }

    public string GetTfvcItemRestUrl(IVssRequestContext requestContext, string path) => this.GetResourceUrl(requestContext, "tfvc", TfvcConstants.TfvcItemsLocationId, (object) new
    {
      path = path
    });

    public string GetFileContainerItemRestUrl(
      IVssRequestContext requestContext,
      string container,
      string itemPath)
    {
      return this.GetResourceUrl(requestContext, "Container", FileContainerResourceIds.BrowseFileContainer, (object) new
      {
        container = container,
        ItemPath = itemPath
      });
    }

    public string GetStatusBadgeUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      string branchName = null)
    {
      return this.GetResourceUrl(requestContext, BuildResourceIds.StatusBadgeLocationId, projectId, (object) new
      {
        definition = definitionId.ToString()
      }, (Func<Uri, string>) (uri =>
      {
        if (!string.IsNullOrWhiteSpace(branchName))
          uri = uri.AppendQuery(nameof (branchName), branchName);
        return uri.AbsoluteUri;
      }));
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
