// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Rest.DataContractHelpers
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FileContainer;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;

namespace Microsoft.TeamFoundation.Build.Server.Rest
{
  public static class DataContractHelpers
  {
    public static TValue GetValueFromMap<TKey, TValue>(
      TKey key,
      IDictionary<TKey, TValue> dictionary,
      Func<TKey, TValue> valueFactory)
    {
      TValue valueFromMap = default (TValue);
      if (dictionary == null)
        valueFromMap = valueFactory(key);
      else if (!dictionary.TryGetValue(key, out valueFromMap))
      {
        valueFromMap = valueFactory(key);
        dictionary.Add(key, valueFromMap);
      }
      return valueFromMap;
    }

    public static DateTime EnsureUtc(this DateTime value) => value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(value, DateTimeKind.Utc) : value;

    public static void WriteDictionaries(
      BuildQueryResult firstResult,
      Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> urisToControllers,
      Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition> urisToDefinitions,
      Dictionary<string, List<QueuedBuild>> urisToRequests)
    {
      foreach (Microsoft.TeamFoundation.Build.Server.BuildController controller in firstResult.Controllers)
        urisToControllers.Add(controller.Uri, controller);
      foreach (Microsoft.TeamFoundation.Build.Server.BuildDefinition definition in firstResult.Definitions)
        urisToDefinitions.Add(definition.Uri, definition);
      foreach (QueuedBuild queuedBuild in firstResult.QueuedBuilds)
        DataContractHelpers.WriteRequestsToDictionary(queuedBuild, urisToRequests);
    }

    public static void WriteDictionaries(
      Microsoft.TeamFoundation.Build.Server.Events.BuildCompletedEvent buildCompletedEvent,
      Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> urisToControllers,
      Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition> urisToDefinitions,
      Dictionary<string, List<QueuedBuild>> urisToRequests)
    {
      if (buildCompletedEvent.Controller != null)
        urisToControllers.Add(buildCompletedEvent.Controller.Uri, buildCompletedEvent.Controller);
      if (buildCompletedEvent.Definition != null)
        urisToDefinitions.Add(buildCompletedEvent.Definition.Uri, buildCompletedEvent.Definition);
      foreach (QueuedBuild request in buildCompletedEvent.Requests)
        DataContractHelpers.WriteRequestsToDictionary(request, urisToRequests);
    }

    private static void WriteRequestsToDictionary(
      QueuedBuild queuedBuild,
      Dictionary<string, List<QueuedBuild>> urisToRequests)
    {
      foreach (string buildUri in queuedBuild.BuildUris)
      {
        List<QueuedBuild> queuedBuildList = new List<QueuedBuild>();
        if (urisToRequests.TryGetValue(buildUri, out queuedBuildList))
        {
          queuedBuildList.Add(queuedBuild);
        }
        else
        {
          queuedBuildList = new List<QueuedBuild>();
          queuedBuildList.Add(queuedBuild);
          urisToRequests.Add(buildUri, queuedBuildList);
        }
      }
    }

    public static object ConvertDetailToDataContractObject(
      IVssRequestContext tfsRequestContext,
      BuildDetail buildDetail,
      Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> urisToControllers,
      Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition> urisToDefinitions,
      Dictionary<string, List<QueuedBuild>> urisToRequests,
      Dictionary<string, Guid> idCache,
      Guid projectId)
    {
      return (object) DataContractHelpers.ConvertDetailToDataContract(tfsRequestContext, buildDetail, urisToControllers, urisToDefinitions, urisToRequests, idCache, projectId);
    }

    public static Microsoft.TeamFoundation.Build.WebApi.Build ConvertDetailToDataContract(
      IVssRequestContext tfsRequestContext,
      BuildDetail buildDetail,
      Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> urisToControllers,
      Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition> urisToDefinitions,
      Dictionary<string, List<QueuedBuild>> urisToRequests,
      Dictionary<string, Guid> idCache,
      Guid projectId)
    {
      if (buildDetail == null)
        return (Microsoft.TeamFoundation.Build.WebApi.Build) null;
      int num1 = int.Parse(LinkingUtilities.DecodeUri(buildDetail.Uri).ToolSpecificId);
      Microsoft.TeamFoundation.Build.WebApi.Build build1 = new Microsoft.TeamFoundation.Build.WebApi.Build();
      build1.Id = num1;
      build1.Uri = buildDetail.Uri;
      build1.BuildNumber = buildDetail.BuildNumber;
      build1.RetainIndefinitely = new bool?(buildDetail.KeepForever);
      build1.Reason = (Microsoft.TeamFoundation.Build.WebApi.BuildReason) buildDetail.Reason;
      build1.Status = (Microsoft.TeamFoundation.Build.WebApi.BuildStatus) buildDetail.Status;
      build1.Quality = buildDetail.Quality;
      build1.StartTime = buildDetail.StartTime;
      build1.FinishTime = buildDetail.FinishTime;
      build1.DropLocation = buildDetail.DropLocation;
      build1.SourceGetVersion = buildDetail.SourceGetVersion;
      build1.HasDiagnostics = buildDetail.ContainerId.HasValue;
      build1.Url = DataContractHelpers.GetResourceUrl(tfsRequestContext, BuildResourceIds.Builds, projectId, (object) new
      {
        buildId = num1
      });
      DataContractHelpers.UpdateDropLocationReferences(tfsRequestContext, buildDetail, build1);
      List<QueuedBuild> queuedBuildList = new List<QueuedBuild>();
      if (urisToRequests.TryGetValue(buildDetail.Uri, out queuedBuildList))
      {
        List<RequestReference> requestReferenceList = new List<RequestReference>();
        foreach (QueuedBuild queuedBuild in queuedBuildList)
        {
          ArtifactId artifactId = new ArtifactId("Build", "Request", queuedBuild.Id.ToString());
          RequestReference requestReference1 = new RequestReference();
          requestReference1.Id = queuedBuild.Id;
          requestReference1.Url = DataContractHelpers.GetResourceUrl(tfsRequestContext, BuildResourceIds.Requests, projectId, (object) new
          {
            requestId = long.Parse(artifactId.ToolSpecificId)
          });
          IdentityRef identityRef;
          if (queuedBuild.RequestedForIdentity == null)
            identityRef = new IdentityRef()
            {
              DisplayName = queuedBuild.RequestedForDisplayName,
              UniqueName = queuedBuild.RequestedFor,
              Id = DataContractHelpers.ParseOwnerId(tfsRequestContext, queuedBuild.RequestedFor).ToString()
            };
          else
            identityRef = queuedBuild.RequestedForIdentity.ToIdentityRef(tfsRequestContext);
          requestReference1.RequestedFor = identityRef;
          RequestReference requestReference2 = requestReference1;
          requestReferenceList.Add(requestReference2);
        }
        build1.Requests = requestReferenceList;
      }
      Microsoft.TeamFoundation.Build.Server.BuildController buildController = new Microsoft.TeamFoundation.Build.Server.BuildController();
      if (buildDetail.BuildControllerUri != null && urisToControllers.TryGetValue(buildDetail.BuildControllerUri, out buildController))
      {
        int num2 = int.Parse(LinkingUtilities.DecodeUri(buildController.Uri).ToolSpecificId);
        Microsoft.TeamFoundation.Build.WebApi.Build build2 = build1;
        QueueReference queueReference = new QueueReference();
        queueReference.Id = num2;
        queueReference.Name = buildController.Name;
        queueReference.Url = DataContractHelpers.GetResourceUrl(tfsRequestContext, BuildResourceIds.Queues, Guid.Empty, (object) new
        {
          controllerId = num2
        });
        queueReference.QueueType = QueueType.BuildController;
        build2.Queue = queueReference;
      }
      else
        build1.Queue = (QueueReference) null;
      Microsoft.TeamFoundation.Build.Server.BuildDefinition buildDefinition = new Microsoft.TeamFoundation.Build.Server.BuildDefinition();
      if (urisToDefinitions.TryGetValue(buildDetail.BuildDefinitionUri, out buildDefinition))
      {
        Microsoft.TeamFoundation.Build.WebApi.Build build3 = build1;
        DefinitionReference definitionReference = new DefinitionReference();
        definitionReference.Id = buildDefinition.Id;
        definitionReference.Name = buildDefinition.Name;
        definitionReference.Url = DataContractHelpers.GetResourceUrl(tfsRequestContext, BuildResourceIds.Definitions, projectId, (object) new
        {
          definitionId = int.Parse(LinkingUtilities.DecodeUri(buildDefinition.Uri).ToolSpecificId)
        });
        definitionReference.DefinitionType = DefinitionType.Xaml;
        build3.Definition = (ShallowReference) definitionReference;
      }
      else
        build1.Definition = (ShallowReference) null;
      Guid valueFromMap = DataContractHelpers.GetValueFromMap<string, Guid>(buildDetail.LastChangedBy, (IDictionary<string, Guid>) idCache, (Func<string, Guid>) (key => DataContractHelpers.ParseOwnerId(tfsRequestContext, key)));
      build1.LastChangedBy = new IdentityRef()
      {
        DisplayName = buildDetail.LastChangedByDisplayName,
        UniqueName = buildDetail.LastChangedBy,
        Id = valueFromMap.ToString()
      };
      if (valueFromMap != Guid.Empty)
      {
        build1.LastChangedBy.Url = IdentityHelper.GetIdentityResourceUriString(tfsRequestContext, valueFromMap);
        build1.LastChangedBy.ImageUrl = IdentityHelper.GetImageResourceUrl(tfsRequestContext, valueFromMap);
      }
      return build1;
    }

    public static Guid ParseOwnerId(IVssRequestContext TfsRequestContext, string ownerId)
    {
      if (string.IsNullOrEmpty(ownerId))
        return Guid.Empty;
      TeamFoundationIdentity foundationIdentity = TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentity(TfsRequestContext, IdentitySearchFactor.AccountName, ownerId);
      return foundationIdentity == null ? Guid.Empty : foundationIdentity.TeamFoundationId;
    }

    public static string GetResourceUrl(
      IVssRequestContext tfsRequestContext,
      Guid locationId,
      Guid projectId,
      object routeValues)
    {
      return DataContractHelpers.GetResourceUrl(tfsRequestContext, "Build", locationId, projectId, routeValues);
    }

    private static string GetResourceUrl(
      IVssRequestContext tfsRequestContext,
      string serviceArea,
      Guid locationId,
      Guid projectId,
      object routeValues)
    {
      ILocationService service = tfsRequestContext.GetService<ILocationService>();
      try
      {
        return service.GetResourceUri(tfsRequestContext, serviceArea, locationId, projectId, routeValues).AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ResourceStrings.ResourceUrlCreationFailed(), ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }

    private static void UpdateDropLocationReferences(
      IVssRequestContext tfsRequestContext,
      BuildDetail buildDetail,
      Microsoft.TeamFoundation.Build.WebApi.Build build)
    {
      build.Drop = new DropLocationReference();
      build.Log = new LogLocationReference();
      DropLocationReference drop = build.Drop;
      LogLocationReference log = build.Log;
      tfsRequestContext.GetService<ILocationService>();
      string dropLocation = buildDetail.DropLocation;
      drop.Location = dropLocation;
      if (string.IsNullOrEmpty(dropLocation))
      {
        drop.Type = DropLocationReferenceType.Unknown;
        log.Type = DropLocationReferenceType.Unknown;
      }
      else if (dropLocation.StartsWith("$/", StringComparison.OrdinalIgnoreCase))
      {
        drop.Type = DropLocationReferenceType.VersionControl;
        drop.Url = DataContractHelpers.GetResourceUrl(tfsRequestContext, "tfvc", TfvcConstants.TfvcItemsLocationId, Guid.Empty, (object) new
        {
          path = dropLocation
        });
        if (string.IsNullOrEmpty(drop.Url))
          return;
        string buildNumber = buildDetail.BuildNumber;
        drop.DownloadUrl = DataContractHelpers.GetDownloadZipUrl(drop.Url, buildNumber, true);
        log.Type = DropLocationReferenceType.VersionControl;
        log.Url = DataContractHelpers.GetLogUrl(drop.Url);
        string downloadFileName = buildDetail.BuildNumber + "_logs";
        log.DownloadUrl = DataContractHelpers.GetDownloadZipUrl(log.Url, downloadFileName, true);
      }
      else if (dropLocation.StartsWith("#", StringComparison.OrdinalIgnoreCase))
      {
        string[] strArray = dropLocation.Split(new char[1]
        {
          '/'
        }, 3);
        drop.Type = DropLocationReferenceType.Container;
        drop.Url = DataContractHelpers.GetResourceUrl(tfsRequestContext, "Container", FileContainerResourceIds.FileContainer, Guid.Empty, (object) new
        {
          containerId = strArray[1],
          itemPath = strArray[2]
        });
        if (string.IsNullOrEmpty(drop.Url))
          return;
        string downloadFileName1 = string.Format("{0}_{1}", (object) buildDetail.BuildNumber, (object) strArray[2]);
        drop.DownloadUrl = DataContractHelpers.GetDownloadZipUrl(drop.Url, downloadFileName1, false);
        log.Type = DropLocationReferenceType.Container;
        log.Url = DataContractHelpers.GetResourceUrl(tfsRequestContext, "Container", FileContainerResourceIds.FileContainer, Guid.Empty, (object) new
        {
          containerId = strArray[1],
          itemPath = "logs"
        });
        string downloadFileName2 = buildDetail.BuildNumber + "_logs";
        log.DownloadUrl = DataContractHelpers.GetDownloadZipUrl(log.Url, downloadFileName2, false);
      }
      else
      {
        Uri result = (Uri) null;
        Uri.TryCreate(dropLocation, UriKind.RelativeOrAbsolute, out result);
        if (result != (Uri) null && (result.IsUnc || result.IsFile))
        {
          drop.Type = DropLocationReferenceType.LocalPath;
          log.Type = DropLocationReferenceType.LocalPath;
          if (result.IsUnc)
          {
            drop.Url = "file://///" + result.Authority + result.PathAndQuery;
            log.Url = DataContractHelpers.GetLogUrl(drop.Url);
          }
          else if (result.IsFile)
          {
            drop.Url = "file:///" + result.Authority + result.PathAndQuery;
            log.Url = DataContractHelpers.GetLogUrl(drop.Url);
          }
          drop.DownloadUrl = drop.Url;
          log.DownloadUrl = log.Url;
        }
        else
          drop.Type = DropLocationReferenceType.Unknown;
      }
    }

    private static string GetLogUrl(string url)
    {
      string[] strArray = url.Split(new char[1]{ '?' }, 2);
      string logUrl = strArray[0].TrimEnd('/') + "/logs";
      if (strArray.Length > 1)
        logUrl = logUrl + "?" + strArray[1];
      return logUrl;
    }

    private static string GetDownloadZipUrl(string url, string downloadFileName, bool isTfvc)
    {
      string str1 = url.IndexOf('?') > -1 ? "&" : "?";
      string str2 = isTfvc ? "fileName" : nameof (downloadFileName);
      return string.Format("{0}{1}api-version=1.0&$format=zip&{2}={3}", (object) url, (object) str1, (object) str2, (object) HttpUtility.HtmlEncode(downloadFileName));
    }
  }
}
