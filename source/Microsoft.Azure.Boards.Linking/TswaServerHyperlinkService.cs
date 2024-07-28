// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Linking.TswaServerHyperlinkService
// Assembly: Microsoft.Azure.Boards.Linking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2FA874A3-91E6-4EEC-B5F5-3126D83824FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Linking.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Boards.Linking
{
  public class TswaServerHyperlinkService : 
    TswaHyperlinkBuilder,
    IVssFrameworkService,
    ITswaServerHyperlinkService
  {
    private Guid m_applicationInstanceId;
    private IVssDeploymentServiceHost m_deploymentServiceHost;
    private const string c_collectionGuidKey = "{projectCollectionGuid}";

    private TswaServerHyperlinkService()
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      this.CollectionId = systemRequestContext.ServiceHost.InstanceId;
      this.m_applicationInstanceId = systemRequestContext.To(TeamFoundationHostType.Application).ServiceHost.InstanceId;
      this.m_deploymentServiceHost = (IVssDeploymentServiceHost) systemRequestContext.To(TeamFoundationHostType.Deployment).ServiceHost;
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => this.m_deploymentServiceHost = (IVssDeploymentServiceHost) null;

    public Uri GetWorkItemEditorUrl(IVssRequestContext requestContext, int workItemId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForOutOfRange(workItemId, nameof (workItemId), 1);
      IVssRequestContext requestContext1 = requestContext;
      Dictionary<string, string> args = new Dictionary<string, string>();
      args[nameof (workItemId)] = workItemId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      Guid? nullable = new Guid?(WitConstants.WorkItemTrackingWebConstants.RestAreaGuid);
      Guid? remoteHostId = new Guid?();
      Guid? areaId = nullable;
      return this.FormatUrl(requestContext1, "OpenWorkItem", true, args: (IDictionary<string, string>) args, remoteHostId: remoteHostId, areaId: areaId);
    }

    public Uri GetWorkItemEditorUrl(
      IVssRequestContext requestContext,
      Uri projectUri,
      int workItemId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForOutOfRange(workItemId, nameof (workItemId), 1);
      if (!(projectUri != (Uri) null))
        return this.GetWorkItemEditorUrl(workItemId);
      Guid guid = new Guid(LinkingUtilities.DecodeUri(projectUri.AbsoluteUri).ToolSpecificId);
      IVssRequestContext requestContext1 = requestContext;
      Dictionary<string, string> args = new Dictionary<string, string>();
      args["pguid"] = this.CollectionId.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);
      args["projectId"] = guid.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);
      args["id"] = workItemId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      Guid? nullable = new Guid?(WitConstants.WorkItemTrackingWebConstants.RestAreaGuid);
      Guid? remoteHostId = new Guid?();
      Guid? areaId = nullable;
      return this.FormatUrl(requestContext1, "OpenWorkItemWithProjectContext", false, args: (IDictionary<string, string>) args, remoteHostId: remoteHostId, areaId: areaId);
    }

    public Uri GetWorkItemEditorUrl(
      IVssRequestContext requestContext,
      Guid projectId,
      int workItemId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForOutOfRange(workItemId, nameof (workItemId), 1);
      IVssRequestContext requestContext1 = requestContext;
      Dictionary<string, string> args = new Dictionary<string, string>();
      args[nameof (projectId)] = projectId.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);
      args["id"] = workItemId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      Guid? nullable = new Guid?(WitConstants.WorkItemTrackingWebConstants.RestAreaGuid);
      Guid? remoteHostId = new Guid?();
      Guid? areaId = nullable;
      return this.FormatUrl(requestContext1, "OpenWorkItemWithProjectContext", false, args: (IDictionary<string, string>) args, remoteHostId: remoteHostId, areaId: areaId);
    }

    public Uri GetWorkItemEditorUrl(
      IVssRequestContext requestContext,
      int workItemId,
      Guid remoteHostId,
      string remoteHostUrl,
      Guid? remoteProjectId = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForOutOfRange(workItemId, nameof (workItemId), 1);
      ArgumentUtility.CheckForEmptyGuid(remoteHostId, nameof (remoteHostId));
      ArgumentUtility.CheckStringForNullOrEmpty(remoteHostUrl, nameof (remoteHostUrl));
      Dictionary<string, string> dictionary = new Dictionary<string, string>()
      {
        [nameof (workItemId)] = workItemId.ToString((IFormatProvider) CultureInfo.InvariantCulture),
        ["pguid"] = remoteHostId.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture),
        ["id"] = workItemId.ToString((IFormatProvider) CultureInfo.InvariantCulture)
      };
      if (remoteProjectId.HasValue)
        dictionary["projectId"] = remoteProjectId.Value.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);
      IVssRequestContext requestContext1 = requestContext;
      Dictionary<string, string> args = new Dictionary<string, string>();
      args[nameof (workItemId)] = workItemId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      args["pguid"] = remoteHostId.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);
      args["id"] = workItemId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      Guid? remoteHostId1 = new Guid?(remoteHostId);
      string remoteHostUrl1 = remoteHostUrl;
      Guid? areaId = new Guid?(WitConstants.WorkItemTrackingWebConstants.RestAreaGuid);
      return this.FormatUrl(requestContext1, "OpenWorkItem", true, args: (IDictionary<string, string>) args, remoteHostId: remoteHostId1, remoteHostUrl: remoteHostUrl1, areaId: areaId);
    }

    public Uri GetWorkItemQueryResultsUrl(
      IVssRequestContext requestContext,
      Uri projectUri,
      Guid queryId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IVssRequestContext requestContext1 = requestContext;
      Guid? nullable = new Guid?(WitConstants.WorkItemTrackingWebConstants.RestAreaGuid);
      Guid? remoteHostId = new Guid?();
      Guid? areaId = nullable;
      return this.FormatUrl(requestContext1, "QueryResults", true, remoteHostId: remoteHostId, areaId: areaId).AppendQuery((IEnumerable<KeyValuePair<string, string>>) new Dictionary<string, string>()
      {
        ["pguid"] = this.GetProjectIdFromProjectUri(projectUri).ToString("D", (IFormatProvider) CultureInfo.InvariantCulture),
        ["qid"] = queryId.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture)
      });
    }

    public Uri GetChangesetDetailsUrl(IVssRequestContext requestContext, int changesetId)
    {
      ArgumentUtility.CheckForOutOfRange(changesetId, nameof (changesetId), 1);
      IVssRequestContext requestContext1 = requestContext;
      Dictionary<string, string> args = new Dictionary<string, string>();
      args[nameof (changesetId)] = changesetId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      Guid? remoteHostId = new Guid?();
      Guid? areaId = new Guid?();
      return this.FormatUrl(requestContext1, "ViewChangesetDetails", true, args: (IDictionary<string, string>) args, remoteHostId: remoteHostId, areaId: areaId);
    }

    public Uri GetDifferenceSourceControlItemsUrl(
      IVssRequestContext requestContext,
      string originalItemServerPath,
      int originalItemChangeset,
      string modifiedItemServerPath,
      int modifiedItemChangeset)
    {
      return this.GetDifferenceSourceControlItemsUrl(requestContext, originalItemServerPath, originalItemChangeset.ToString((IFormatProvider) CultureInfo.InvariantCulture), modifiedItemServerPath, modifiedItemChangeset.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    public Uri GetDifferenceSourceControlItemsUrl(
      IVssRequestContext requestContext,
      string originalItemServerPath,
      string originalItemVersionSpec,
      string modifiedItemServerPath,
      string modifiedItemVersionSpec)
    {
      this.ValidateServerItemPath(originalItemServerPath, nameof (originalItemServerPath));
      this.ValidateServerItemPath(modifiedItemServerPath, nameof (modifiedItemServerPath));
      IVssRequestContext requestContext1 = requestContext;
      Dictionary<string, string> args = new Dictionary<string, string>();
      args["originalItemPath"] = originalItemServerPath;
      args["originalItemChangeset"] = originalItemVersionSpec;
      args["modifiedItemPath"] = modifiedItemServerPath;
      args["modifiedItemChangeset"] = modifiedItemVersionSpec;
      Guid? remoteHostId = new Guid?();
      Guid? areaId = new Guid?();
      return this.FormatUrl(requestContext1, "DiffSourceControlItems", true, args: (IDictionary<string, string>) args, remoteHostId: remoteHostId, areaId: areaId);
    }

    public Uri GetSourceExplorerUrl(IVssRequestContext requestContext, string serverItemPath)
    {
      if (string.IsNullOrEmpty(serverItemPath))
        serverItemPath = "$/";
      IVssRequestContext requestContext1 = requestContext;
      Dictionary<string, string> args = new Dictionary<string, string>();
      args["sourceControlPath"] = serverItemPath;
      Guid? remoteHostId = new Guid?();
      Guid? areaId = new Guid?();
      return this.FormatUrl(requestContext1, "ExploreSourceControlPath", true, args: (IDictionary<string, string>) args, remoteHostId: remoteHostId, areaId: areaId);
    }

    public Uri GetViewSourceControlItemUrl(
      IVssRequestContext requestContext,
      string serverItemPath,
      int changesetId)
    {
      string str = changesetId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      this.ValidateServerItemPath(serverItemPath, nameof (serverItemPath));
      IVssRequestContext requestContext1 = requestContext;
      Dictionary<string, string> args = new Dictionary<string, string>();
      args["itemPath"] = serverItemPath;
      args["itemChangeset"] = str;
      Guid? remoteHostId = new Guid?();
      Guid? areaId = new Guid?();
      return this.FormatUrl(requestContext1, "ViewSourceControlItem", true, args: (IDictionary<string, string>) args, remoteHostId: remoteHostId, areaId: areaId);
    }

    public Uri GetShelvesetDetailsUrl(
      IVssRequestContext requestContext,
      string shelvesetName,
      string shelvesetOwner)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(shelvesetName, nameof (shelvesetName));
      ArgumentUtility.CheckStringForNullOrEmpty(shelvesetOwner, nameof (shelvesetOwner));
      IVssRequestContext requestContext1 = requestContext;
      Dictionary<string, string> args = new Dictionary<string, string>();
      args[nameof (shelvesetName)] = shelvesetName;
      args[nameof (shelvesetOwner)] = shelvesetOwner;
      Guid? remoteHostId = new Guid?();
      Guid? areaId = new Guid?();
      return this.FormatUrl(requestContext1, "ViewShelvesetDetails", true, args: (IDictionary<string, string>) args, remoteHostId: remoteHostId, areaId: areaId);
    }

    public Uri GetDifferenceSourceControlShelvedItemUrl(
      IVssRequestContext requestContext,
      string originalItemServerPath,
      int originalItemChangeset,
      string shelvedItemServerPath,
      string shelvesetName,
      string shelvesetOwner)
    {
      this.ValidateServerItemPath(originalItemServerPath, nameof (originalItemServerPath));
      this.ValidateServerItemPath(shelvedItemServerPath, nameof (shelvedItemServerPath));
      IVssRequestContext requestContext1 = requestContext;
      Dictionary<string, string> args = new Dictionary<string, string>();
      args["originalItemPath"] = originalItemServerPath;
      args[nameof (originalItemChangeset)] = originalItemChangeset.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      args["shelvedItemPath"] = shelvedItemServerPath;
      args[nameof (shelvesetName)] = shelvesetName;
      args[nameof (shelvesetOwner)] = shelvesetOwner;
      Guid? remoteHostId = new Guid?();
      Guid? areaId = new Guid?();
      return this.FormatUrl(requestContext1, "DiffSourceControlShelvedItem", true, args: (IDictionary<string, string>) args, remoteHostId: remoteHostId, areaId: areaId);
    }

    public Uri GetViewSourceControlShelvedItemUrl(
      IVssRequestContext requestContext,
      string serverItemPath,
      string shelvesetName,
      string shelvesetOwner)
    {
      ArgumentUtility.CheckForNull<string>(shelvesetName, nameof (shelvesetName));
      ArgumentUtility.CheckForNull<string>(shelvesetOwner, nameof (shelvesetOwner));
      this.ValidateServerItemPath(serverItemPath, nameof (serverItemPath));
      return new UriBuilder(UriUtility.Combine(this.GetLocation(requestContext, "TSWAHome", true, new Guid?()), "view.aspx", true))
      {
        Query = this.FormatQueryString("path", serverItemPath, "ss", shelvesetName + ";" + shelvesetOwner)
      }.Uri;
    }

    private string FormatQueryString(params string[] args)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("pcguid=");
      stringBuilder.Append(UriUtility.UrlEncode(this.CollectionId.ToString()));
      for (int index = 0; index < args.Length; index += 2)
      {
        string str1 = args[index];
        string str2 = args[index + 1];
        if (str1 != null)
        {
          if (stringBuilder.Length > 0)
            stringBuilder.Append('&');
          stringBuilder.Append(UriUtility.UrlEncode(str1));
        }
        if (str2 != null)
        {
          if (str1 != null)
            stringBuilder.Append('=');
          else if (stringBuilder.Length > 0)
            stringBuilder.Append('&');
          stringBuilder.Append(UriUtility.UrlEncode(str2));
        }
      }
      return stringBuilder.ToString();
    }

    private void ValidateServerItemPath(string serverItemPath, string parameterName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(serverItemPath, parameterName);
      if (!serverItemPath.StartsWith("$/", StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException(TFCommonResources.InvalidPathMissingRoot((object) serverItemPath), parameterName);
    }

    private Guid GetProjectIdFromProjectUri(Uri ProjectUri) => Guid.Parse(LinkingUtilities.DecodeUri(ProjectUri.AbsoluteUri).ToolSpecificId);

    private Uri FormatUrl(
      IVssRequestContext requestContext,
      string serviceType,
      bool isOrganization,
      bool isQueryParameter = false,
      IDictionary<string, string> args = null,
      Guid? remoteHostId = null,
      string remoteHostUrl = null,
      Guid? areaId = null)
    {
      string str1 = string.IsNullOrEmpty(remoteHostUrl) ? this.GetLocation(requestContext, serviceType, isOrganization, areaId) : this.GetRemoteLocation(requestContext, serviceType, remoteHostUrl);
      Guid guid = !remoteHostId.HasValue || !(remoteHostId.Value != Guid.Empty) ? this.CollectionId : remoteHostId.Value;
      StringBuilder stringBuilder = new StringBuilder(str1);
      if (str1.IndexOf("{projectCollectionGuid}", StringComparison.OrdinalIgnoreCase) >= 0)
        stringBuilder.Replace("{projectCollectionGuid}", guid.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture));
      if (args != null)
      {
        foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) args)
        {
          string key = keyValuePair.Key;
          string str2 = keyValuePair.Value;
          if (isQueryParameter)
            stringBuilder.Replace("{" + key + "}", Uri.EscapeDataString(str2));
          else
            stringBuilder.Replace("{" + key + "}", UriUtility.UrlEncode(str2));
        }
      }
      return new Uri(stringBuilder.ToString());
    }

    private string GetLocation(
      IVssRequestContext requestContext,
      string serviceType,
      bool isOrganization,
      Guid? areaId)
    {
      if (((!requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? 0 : (!requestContext.ServiceHost.Is(TeamFoundationHostType.Application) ? 1 : 0)) & (isOrganization ? 1 : 0)) != 0)
        requestContext = requestContext.To(TeamFoundationHostType.Application);
      ILocationService service = requestContext.GetService<ILocationService>();
      Guid serviceInterfaceGuid = this.MapServiceTypeToServiceInterfaceGuid(serviceType);
      if (areaId.HasValue && serviceInterfaceGuid != Guid.Empty)
      {
        ILocationDataProvider locationData = service.GetLocationData(requestContext, areaId.Value);
        ServiceDefinition serviceDefinition = locationData.FindServiceDefinition(requestContext, serviceType, serviceInterfaceGuid);
        return locationData.LocationForAccessMapping(requestContext, serviceType, serviceDefinition.Identifier, locationData.DetermineAccessMapping(requestContext));
      }
      ServiceDefinition serviceDefinition1 = this.GetServiceDefinition(requestContext, serviceType);
      return service.LocationForAccessMapping(requestContext, serviceType, serviceDefinition1.Identifier, service.DetermineAccessMapping(requestContext));
    }

    private Guid MapServiceTypeToServiceInterfaceGuid(string serviceType)
    {
      switch (serviceType)
      {
        case "OpenWorkItemWithProjectContext":
          return FrameworkServiceIdentifiers.OpenWorkItemWithProjectContext;
        case "OpenWorkItem":
          return Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaOpenWorkItem;
        case "QueryResults":
          return Microsoft.TeamFoundation.ServiceInterfaces.GuidTswaQueryResults;
        default:
          return Guid.Empty;
      }
    }

    private string GetRemoteLocation(
      IVssRequestContext requestContext,
      string serviceType,
      string remoteHostUrl)
    {
      ServiceDefinition serviceDefinition = this.GetServiceDefinition(requestContext, serviceType);
      return remoteHostUrl + serviceDefinition.RelativePath;
    }

    private ServiceDefinition GetServiceDefinition(
      IVssRequestContext requestContext,
      string serviceType)
    {
      IEnumerable<ServiceDefinition> serviceDefinitions = requestContext.GetService<ILocationService>().FindServiceDefinitions(requestContext, serviceType);
      return serviceDefinitions.FirstOrDefault<ServiceDefinition>((Func<ServiceDefinition, bool>) (sd => sd.ToolId == "TSWebAccess")) ?? serviceDefinitions.FirstOrDefault<ServiceDefinition>();
    }

    protected override Uri GetUrl(string serviceType, string accessMappingMoniker) => this.GetUrl(serviceType, accessMappingMoniker, false);

    protected override Uri GetUrl(
      string serviceType,
      string accessMappingMoniker,
      bool collectionLevel)
    {
      using (IVssRequestContext systemContext = this.m_deploymentServiceHost.CreateSystemContext())
      {
        Guid instanceId = this.m_applicationInstanceId;
        if (collectionLevel || systemContext.ExecutionEnvironment.IsHostedDeployment)
          instanceId = this.CollectionId;
        using (IVssRequestContext vssRequestContext = systemContext.GetService<TeamFoundationHostManagementService>().BeginRequest(systemContext, instanceId, RequestContextType.SystemContext, true, true))
        {
          ILocationService service = vssRequestContext.GetService<ILocationService>();
          IEnumerable<ServiceDefinition> serviceDefinitions = service.FindServiceDefinitions(vssRequestContext, serviceType);
          ServiceDefinition serviceDefinition = serviceDefinitions.FirstOrDefault<ServiceDefinition>((Func<ServiceDefinition, bool>) (sd => sd.ToolId == "TSWebAccess")) ?? serviceDefinitions.FirstOrDefault<ServiceDefinition>();
          return serviceDefinition != null ? new Uri(service.LocationForAccessMapping(vssRequestContext, serviceType, serviceDefinition.Identifier, service.DetermineAccessMapping(vssRequestContext))) : (Uri) null;
        }
      }
    }

    Uri ITswaServerHyperlinkService.GetHomeUrl() => this.GetHomeUrl();

    Uri ITswaServerHyperlinkService.GetViewBuildDetailsUrl(Uri buildUri, Guid projectId) => this.GetViewBuildDetailsUrl(buildUri, projectId);
  }
}
