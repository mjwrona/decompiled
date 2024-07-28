// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.ResourceAreas2Controller
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Client;
using Microsoft.VisualStudio.Services.NameResolution;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  [ControllerApiVersion(3.3)]
  [VersionedApiControllerCustomName(Area = "Location", ResourceName = "ResourceAreas")]
  [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
  public class ResourceAreas2Controller : TfsApiController
  {
    private const string c_resourceAreaForwardingFF = "VisualStudio.Services.Location.ForwardResourceAreaRequestsToHostLevel";
    private const string c_area = "LocationService";
    private const string c_layer = "ResourceAreasController";

    [HttpGet]
    [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    public async Task<IEnumerable<ResourceAreaInfo>> GetResourceAreas(
      string enterpriseName = null,
      string organizationName = null)
    {
      ResourceAreas2Controller areas2Controller = this;
      string queryStringValue = areas2Controller.Request.GetQueryStringValue("accountName");
      if (((IEnumerable<string>) ResourceAreas2Controller.MakeArray<string>(enterpriseName, organizationName, queryStringValue)).Count<string>((Func<string, bool>) (x => x != null)) > 1)
        throw new ArgumentException(FrameworkResources.MultipleParametersProvidedError3((object) nameof (enterpriseName), (object) nameof (organizationName), (object) "accountName"));
      if (enterpriseName == null && organizationName == null && queryStringValue == null)
      {
        Dictionary<Guid, string> urlCache = new Dictionary<Guid, string>();
        if (ResourceAreas2Controller.DoNotUseInheritedData(areas2Controller.TfsRequestContext))
          return (IEnumerable<ResourceAreaInfo>) areas2Controller.TfsRequestContext.GetService<IResourceAreaService>().GetResourceAreas(areas2Controller.TfsRequestContext).Select<ResourceArea, ResourceAreaInfo>((Func<ResourceArea, ResourceAreaInfo>) (ra => ra.ToResourceAreaInfo(this.TfsRequestContext, (IDictionary<Guid, string>) urlCache))).Where<ResourceAreaInfo>((Func<ResourceAreaInfo, bool>) (ra => ra.LocationUrl != null)).ToList<ResourceAreaInfo>();
        IVssRequestContext vssRequestContext = areas2Controller.TfsRequestContext.To(TeamFoundationHostType.Deployment);
        return (IEnumerable<ResourceAreaInfo>) vssRequestContext.GetService<IResourceAreaService>().GetInheritedResourceAreas(vssRequestContext, areas2Controller.TfsRequestContext.ServiceHost.HostType).Select<ResourceArea, ResourceAreaInfo>((Func<ResourceArea, ResourceAreaInfo>) (ra => ra.ToResourceAreaInfo(this.TfsRequestContext, (IDictionary<Guid, string>) urlCache))).Where<ResourceAreaInfo>((Func<ResourceAreaInfo, bool>) (ra => ra.LocationUrl != null)).ToList<ResourceAreaInfo>();
      }
      areas2Controller.TfsRequestContext.CheckDeploymentRequestContext();
      NameResolutionEntry entry = areas2Controller.ResolveHost(areas2Controller.TfsRequestContext, enterpriseName, queryStringValue ?? organizationName);
      return await areas2Controller.GetResourceAreasByHost(entry);
    }

    [HttpGet]
    [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    public async Task<ResourceAreaInfo> GetResourceArea(
      Guid areaId,
      string enterpriseName = null,
      string organizationName = null)
    {
      ResourceAreas2Controller areas2Controller = this;
      ArgumentUtility.CheckForEmptyGuid(areaId, nameof (areaId));
      string queryStringValue = areas2Controller.Request.GetQueryStringValue("accountName");
      if (((IEnumerable<string>) ResourceAreas2Controller.MakeArray<string>(enterpriseName, organizationName, queryStringValue)).Count<string>((Func<string, bool>) (x => x != null)) > 1)
        throw new ArgumentException(FrameworkResources.MultipleParametersProvidedError3((object) nameof (enterpriseName), (object) nameof (organizationName), (object) "accountName"));
      if (enterpriseName == null && organizationName == null && queryStringValue == null)
      {
        if (ResourceAreas2Controller.DoNotUseInheritedData(areas2Controller.TfsRequestContext))
        {
          ResourceAreaInfo resourceAreaInfo = areas2Controller.TfsRequestContext.GetService<IResourceAreaService>().GetResourceArea(areas2Controller.TfsRequestContext, areaId).ToResourceAreaInfo(areas2Controller.TfsRequestContext);
          return resourceAreaInfo == null || resourceAreaInfo.LocationUrl == null ? (ResourceAreaInfo) null : resourceAreaInfo;
        }
        IVssRequestContext vssRequestContext = areas2Controller.TfsRequestContext.To(TeamFoundationHostType.Deployment);
        ResourceAreaInfo resourceAreaInfo1 = vssRequestContext.GetService<IResourceAreaService>().GetInheritedResourceArea(vssRequestContext, areaId, areas2Controller.TfsRequestContext.ServiceHost.HostType).ToResourceAreaInfo(areas2Controller.TfsRequestContext);
        return resourceAreaInfo1 == null || resourceAreaInfo1.LocationUrl == null ? (ResourceAreaInfo) null : resourceAreaInfo1;
      }
      areas2Controller.TfsRequestContext.CheckDeploymentRequestContext();
      NameResolutionEntry entry = areas2Controller.ResolveHost(areas2Controller.TfsRequestContext, enterpriseName, queryStringValue ?? organizationName);
      return await areas2Controller.GetResourceAreaByHost(areaId, entry);
    }

    [HttpGet]
    [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    public async Task<IEnumerable<ResourceAreaInfo>> GetResourceAreasByHost(Guid hostId)
    {
      ResourceAreas2Controller areas2Controller = this;
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      areas2Controller.TfsRequestContext.CheckDeploymentRequestContext();
      NameResolutionEntry entry = areas2Controller.ResolveHost(areas2Controller.TfsRequestContext, hostId);
      return await areas2Controller.GetResourceAreasByHost(entry);
    }

    [HttpGet]
    [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    public async Task<ResourceAreaInfo> GetResourceAreaByHost(Guid areaId, Guid hostId)
    {
      ResourceAreas2Controller areas2Controller = this;
      ArgumentUtility.CheckForEmptyGuid(areaId, nameof (areaId));
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      areas2Controller.TfsRequestContext.CheckDeploymentRequestContext();
      NameResolutionEntry entry = areas2Controller.ResolveHost(areas2Controller.TfsRequestContext, hostId);
      return await areas2Controller.GetResourceAreaByHost(areaId, entry);
    }

    private async Task<IEnumerable<ResourceAreaInfo>> GetResourceAreasByHost(
      NameResolutionEntry entry)
    {
      ResourceAreas2Controller areas2Controller = this;
      ArgumentUtility.CheckForNull<NameResolutionEntry>(entry, nameof (entry));
      areas2Controller.TfsRequestContext.CheckDeploymentRequestContext();
      if (ResourceAreas2Controller.DoNotUseInheritedData(areas2Controller.TfsRequestContext))
        return await HttpClientHelper.CreateSpsClient<LocationHttpClient>(areas2Controller.TfsRequestContext.Elevate(), entry.Value).GetResourceAreasAsync();
      Dictionary<Guid, string> urlCache = new Dictionary<Guid, string>();
      return (IEnumerable<ResourceAreaInfo>) areas2Controller.TfsRequestContext.GetService<IResourceAreaService>().GetInheritedResourceAreas(areas2Controller.TfsRequestContext, entry.GetHostType()).Select<ResourceArea, ResourceAreaInfo>((Func<ResourceArea, ResourceAreaInfo>) (ra => ra.ToResourceAreaInfo(this.TfsRequestContext, entry, (IDictionary<Guid, string>) urlCache))).Where<ResourceAreaInfo>((Func<ResourceAreaInfo, bool>) (ra => ra.LocationUrl != null)).ToList<ResourceAreaInfo>();
    }

    private async Task<ResourceAreaInfo> GetResourceAreaByHost(
      Guid areaId,
      NameResolutionEntry entry)
    {
      ResourceAreas2Controller areas2Controller = this;
      ArgumentUtility.CheckForEmptyGuid(areaId, nameof (areaId));
      ArgumentUtility.CheckForNull<NameResolutionEntry>(entry, nameof (entry));
      areas2Controller.TfsRequestContext.CheckDeploymentRequestContext();
      if (ResourceAreas2Controller.DoNotUseInheritedData(areas2Controller.TfsRequestContext))
        return await HttpClientHelper.CreateSpsClient<LocationHttpClient>(areas2Controller.TfsRequestContext.Elevate(), entry.Value).GetResourceAreaAsync(areaId);
      ResourceAreaInfo resourceAreaInfo = areas2Controller.TfsRequestContext.GetService<IResourceAreaService>().GetInheritedResourceArea(areas2Controller.TfsRequestContext, areaId, entry.GetHostType()).ToResourceAreaInfo(areas2Controller.TfsRequestContext, entry);
      return resourceAreaInfo == null || resourceAreaInfo.LocationUrl == null ? (ResourceAreaInfo) null : resourceAreaInfo;
    }

    private NameResolutionEntry ResolveHost(
      IVssRequestContext requestContext,
      string organizationName,
      string collectionName)
    {
      NameResolutionEntry nameResolutionEntry = collectionName == null ? HostNameResolver.ResolveOrganizationServiceHost(requestContext, organizationName) : HostNameResolver.ResolveCollectionServiceHost(requestContext, collectionName);
      if (nameResolutionEntry == null)
      {
        requestContext.Trace(71550340, TraceLevel.Error, "LocationService", "ResourceAreasController", string.Format("Failed to resolve host, organization={0}, collection={1}", (object) 0, (object) 1), (object) (organizationName ?? "<null>"), (object) (collectionName ?? "<null>"));
        throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.NotFound, FrameworkResources.HostDoesNotExistException((object) (collectionName ?? organizationName))));
      }
      return nameResolutionEntry;
    }

    private NameResolutionEntry ResolveHost(IVssRequestContext requestContext, Guid hostId)
    {
      NameResolutionEntry nameResolutionEntry = HostNameResolver.ResolveServiceHost(requestContext, hostId);
      if (nameResolutionEntry != null)
        return nameResolutionEntry;
      requestContext.Trace(71550340, TraceLevel.Error, "LocationService", "ResourceAreasController", string.Format("Failed to resolve host, hostId={0}", (object) 0), (object) hostId);
      throw new HttpResponseException(this.Request.CreateErrorResponse(HttpStatusCode.NotFound, FrameworkResources.HostDoesNotExistException((object) hostId.ToString())));
    }

    private static T[] MakeArray<T>(params T[] args) => args ?? Array.Empty<T>();

    private static bool DoNotUseInheritedData(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsOnPremisesDeployment || requestContext.IsFeatureEnabled("VisualStudio.Services.Location.ForwardResourceAreaRequestsToHostLevel");
  }
}
