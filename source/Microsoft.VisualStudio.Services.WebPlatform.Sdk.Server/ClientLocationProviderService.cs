// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ClientLocationProviderService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class ClientLocationProviderService : IClientLocationProviderService, IVssFrameworkService
  {
    private const string c_area = "ClientLocationProviderService";
    private const string c_layer = "IVssFrameworkService";
    private const string c_sharedDataKey = "_locations";
    private const string c_resourceAreasDataKey = "_resourceAreas";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void AddResourceAreaLocation(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      Guid resourceAreaId,
      TeamFoundationHostType? hostType = null)
    {
      Guid serviceInstanceType = Guid.Empty;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        try
        {
          ResourceArea resourceArea = requestContext.GetService<IResourceAreaService>().GetResourceArea(requestContext, resourceAreaId);
          if (resourceArea != null)
            serviceInstanceType = resourceArea.ParentService;
        }
        catch (Exception ex)
        {
          requestContext.Trace(10013765, TraceLevel.Error, nameof (ClientLocationProviderService), "IVssFrameworkService", "Failed finding resource area for {0}", (object) resourceAreaId);
          requestContext.TraceException(10013770, nameof (ClientLocationProviderService), "IVssFrameworkService", ex);
        }
      }
      else
        serviceInstanceType = ServiceInstanceTypes.TFSOnPremises;
      object obj;
      WebSdkMetadataDictionary<Guid, Guid> metadataDictionary;
      if (sharedData.TryGetValue("_resourceAreas", out obj) && obj is WebSdkMetadataDictionary<Guid, Guid>)
      {
        metadataDictionary = obj as WebSdkMetadataDictionary<Guid, Guid>;
      }
      else
      {
        metadataDictionary = new WebSdkMetadataDictionary<Guid, Guid>();
        sharedData.Add("_resourceAreas", (object) metadataDictionary);
      }
      metadataDictionary[resourceAreaId] = serviceInstanceType;
      this.AddLocation(requestContext, sharedData, serviceInstanceType, hostType);
    }

    public void AddLocation(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      Guid serviceInstanceType,
      TeamFoundationHostType? hostType = null)
    {
      if (!hostType.HasValue)
        hostType = new TeamFoundationHostType?(requestContext.IntendedHostType());
      if (hostType.Value > requestContext.ServiceHost.HostType)
        return;
      if (!requestContext.ServiceHost.Is(hostType.Value))
        requestContext = requestContext.To(hostType.Value);
      string str = (string) null;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        try
        {
          str = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, serviceInstanceType, AccessMappingConstants.ClientAccessMappingMoniker);
        }
        catch (Exception ex)
        {
          requestContext.Trace(10013755, TraceLevel.Error, nameof (ClientLocationProviderService), "IVssFrameworkService", "Failed finding location for  {0}", (object) serviceInstanceType);
          requestContext.TraceException(10013760, nameof (ClientLocationProviderService), "IVssFrameworkService", ex);
        }
      }
      else
      {
        str = requestContext.VirtualPath();
        serviceInstanceType = ServiceInstanceTypes.TFSOnPremises;
      }
      if (str != null)
      {
        object obj;
        WebSdkMetadataDictionary<Guid, Dictionary<TeamFoundationHostType, string>> metadataDictionary;
        if (sharedData.TryGetValue("_locations", out obj) && obj is WebSdkMetadataDictionary<Guid, Dictionary<TeamFoundationHostType, string>>)
        {
          metadataDictionary = obj as WebSdkMetadataDictionary<Guid, Dictionary<TeamFoundationHostType, string>>;
        }
        else
        {
          metadataDictionary = new WebSdkMetadataDictionary<Guid, Dictionary<TeamFoundationHostType, string>>();
          sharedData.Add("_locations", (object) metadataDictionary);
        }
        Dictionary<TeamFoundationHostType, string> dictionary;
        if (!metadataDictionary.TryGetValue(serviceInstanceType, out dictionary))
        {
          dictionary = new Dictionary<TeamFoundationHostType, string>();
          metadataDictionary.Add(serviceInstanceType, dictionary);
        }
        dictionary[hostType.Value] = str;
      }
      else
        requestContext.Trace(10013576, TraceLevel.Warning, nameof (ClientLocationProviderService), "IVssFrameworkService", "No service location information found for {0}", (object) serviceInstanceType);
    }
  }
}
