// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Routes.PipelinesRouteService
// Assembly: Microsoft.TeamFoundation.Build2.Routes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3759BAC-2581-46BE-B787-E219FAA96ED4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Routes.dll

using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Routes
{
  public class PipelinesRouteService : IPipelinesRouteService, IVssFrameworkService
  {
    public string GetArtifactRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId,
      string artifactName,
      GetArtifactExpandOptions? expandOptions = null)
    {
      Dictionary<string, string> additionalQueryParams = new Dictionary<string, string>();
      if (!string.IsNullOrEmpty(artifactName))
        additionalQueryParams.Add(nameof (artifactName), artifactName);
      if (expandOptions.HasValue)
        additionalQueryParams.Add("$expands", expandOptions.ToString());
      return this.GetResourceUrl(requestContext, PipelinesResources.Artifacts.Id, projectId, (object) new
      {
        pipelineId = pipelineId,
        runId = runId
      }, additionalQueryParams: (IDictionary<string, string>) additionalQueryParams);
    }

    public string GetLogCollectionRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId,
      GetLogExpandOptions? expandOptions = null)
    {
      Dictionary<string, string> additionalQueryParams = new Dictionary<string, string>();
      if (expandOptions.HasValue)
        additionalQueryParams.Add("$expands", expandOptions.ToString());
      return this.GetResourceUrl(requestContext, PipelinesResources.Logs.Id, projectId, (object) new
      {
        pipelineId = pipelineId,
        runId = runId
      }, additionalQueryParams: (IDictionary<string, string>) additionalQueryParams);
    }

    public string GetLogRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId,
      int logId,
      GetLogExpandOptions? expandOptions = null)
    {
      Dictionary<string, string> additionalQueryParams = new Dictionary<string, string>();
      if (expandOptions.HasValue)
        additionalQueryParams.Add("$expands", expandOptions.ToString());
      return this.GetResourceUrl(requestContext, PipelinesResources.Logs.Id, projectId, (object) new
      {
        pipelineId = pipelineId,
        runId = runId,
        logId = logId
      }, additionalQueryParams: (IDictionary<string, string>) additionalQueryParams);
    }

    public string GetPipelineRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int? pipelineRevision)
    {
      return this.GetResourceUrl(requestContext, PipelinesResources.Pipelines.Id, projectId, (object) new
      {
        pipelineId = pipelineId
      }, (Func<Uri, string>) (uri => !pipelineRevision.HasValue ? uri.AbsoluteUri : uri.AppendQuery("revision", pipelineRevision.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture)).AbsoluteUri));
    }

    public string GetPipelineWebUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId)
    {
      Dictionary<string, object> routeValues = new Dictionary<string, object>()
      {
        {
          "definitionId",
          (object) pipelineId
        }
      };
      return this.ConstructWebLink(requestContext, projectId, "build", "definition", (IDictionary<string, object>) routeValues);
    }

    public string GetRunsRestUrl(IVssRequestContext requestContext, Guid projectId, int pipelineId) => this.GetResourceUrl(requestContext, PipelinesResources.Runs.Id, projectId, (object) new
    {
      pipelineId = pipelineId
    });

    public string GetRunRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId)
    {
      return this.GetResourceUrl(requestContext, PipelinesResources.Runs.Id, projectId, (object) new
      {
        pipelineId = pipelineId,
        runId = runId
      });
    }

    public string GetRunWebUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId)
    {
      Dictionary<string, object> routeValues = new Dictionary<string, object>()
      {
        {
          "buildId",
          (object) runId
        }
      };
      return this.ConstructWebLink(requestContext, projectId, "build", "results", (IDictionary<string, object>) routeValues);
    }

    public string GetSignalRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId)
    {
      return this.GetResourceUrl(requestContext, PipelinesResources.SignalR.Id, projectId, (object) new
      {
        pipelineId = pipelineId,
        runId = runId
      });
    }

    public string GetSignedLogContentRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId,
      int logId)
    {
      return this.GetResourceUrl(requestContext, PipelinesResources.SignedLogContent.Id, projectId, (object) new
      {
        pipelineId = pipelineId,
        runId = runId,
        logId = logId
      });
    }

    public string GetSignedLogsContentRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId)
    {
      return this.GetResourceUrl(requestContext, PipelinesResources.SignedLogContent.Id, projectId, (object) new
      {
        pipelineId = pipelineId,
        runId = runId
      });
    }

    public string GetSignedSignalRWebsocketRestUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int pipelineId,
      int runId)
    {
      return this.GetResourceUrl(requestContext, PipelinesResources.SignalRLive.Id, projectId, (object) new
      {
        pipelineId = pipelineId,
        runId = runId
      });
    }

    protected string ConstructWebLink(
      IVssRequestContext requestContext,
      Guid projectId,
      string controllerName,
      string actionName,
      IDictionary<string, object> routeValues)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      string str1;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        string accessMappingMoniker = AccessMappingConstants.ClientAccessMappingMoniker;
        str1 = service.GetLocationServiceUrl(requestContext, LocationServiceConstants.SelfReferenceIdentifier, accessMappingMoniker);
      }
      else
        str1 = service.LocationForAccessMapping(requestContext, "/", RelativeToSetting.Context, service.GetPublicAccessMapping(requestContext));
      if (!str1.EndsWith("/", StringComparison.OrdinalIgnoreCase))
        str1 += "/";
      List<string> values = new List<string>();
      values.Add(projectId.ToString("D"));
      values.Add("_" + controllerName);
      if (!string.IsNullOrEmpty(actionName))
        values.Add(actionName);
      object obj;
      if (routeValues.TryGetValue("parameters", out obj))
      {
        string[] strArray = (string[]) obj;
        if (((IEnumerable<string>) strArray).Any<string>())
          values.AddRange((IEnumerable<string>) strArray);
        routeValues.Remove("parameters");
      }
      string str2 = "";
      foreach (string key in (IEnumerable<string>) routeValues.Keys)
      {
        string str3;
        if (!(str2 != ""))
          str3 = key + "=" + routeValues[key]?.ToString();
        else
          str3 = str2 + "&" + key + "=" + routeValues[key]?.ToString();
        str2 = str3;
      }
      return str1 + string.Join("/", (IEnumerable<string>) values) + (str2 != "" ? "?" + str2 : "");
    }

    protected string GetResourceUrl(
      IVssRequestContext requestContext,
      Guid locationId,
      Guid projectId,
      object routeValues,
      Func<Uri, string> continuation = null,
      IDictionary<string, string> additionalQueryParams = null)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      Uri uri;
      try
      {
        uri = service.GetResourceUri(requestContext, this.ServiceType, locationId, projectId, routeValues);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "pipelines", nameof (PipelinesRouteService), ex);
        return string.Empty;
      }
      if (additionalQueryParams != null && additionalQueryParams.Any<KeyValuePair<string, string>>())
      {
        UriBuilder uriBuilder = new UriBuilder(uri);
        foreach (KeyValuePair<string, string> additionalQueryParam in (IEnumerable<KeyValuePair<string, string>>) additionalQueryParams)
          uriBuilder.AppendQuery(additionalQueryParam.Key, additionalQueryParam.Value);
        uri = uriBuilder.Uri;
      }
      return continuation == null ? uri.AbsoluteUri : continuation(uri);
    }

    protected virtual string ServiceType => "pipelines";

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
