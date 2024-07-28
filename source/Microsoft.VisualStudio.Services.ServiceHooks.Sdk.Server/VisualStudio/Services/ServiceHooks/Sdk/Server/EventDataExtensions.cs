// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.EventDataExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  public static class EventDataExtensions
  {
    private static readonly string s_layer = typeof (EventDataExtensions).Name;
    private static readonly string s_area = typeof (EventDataExtensions).Namespace;

    public static List<VersionedResource> GetOtherResources(
      this SortedList<string, object> payloads,
      string currentVersion,
      object currentPayload)
    {
      List<VersionedResource> otherResources = new List<VersionedResource>();
      if (payloads.Count > 1)
      {
        Dictionary<object, string> dictionary = new Dictionary<object, string>();
        dictionary.Add(currentPayload, currentVersion);
        for (int index = 0; index < payloads.Count; ++index)
        {
          string key = payloads.Keys[index];
          if (!key.Equals(currentVersion, StringComparison.Ordinal))
          {
            object payload = payloads[key];
            if (dictionary.ContainsKey(payload))
            {
              otherResources.Add(new VersionedResource()
              {
                ResourceVersion = key,
                CompatibleWith = dictionary[payload]
              });
            }
            else
            {
              otherResources.Add(new VersionedResource()
              {
                ResourceVersion = key,
                Resource = payload
              });
              dictionary.Add(payload, key);
            }
          }
        }
      }
      return otherResources;
    }

    public static Dictionary<string, ResourceContainer> GetResourceContainers(
      IVssRequestContext requestContext)
    {
      return EventDataExtensions.GetResourceContainers(requestContext, Guid.Empty);
    }

    public static Dictionary<string, ResourceContainer> GetResourceContainers(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      Dictionary<string, ResourceContainer> resourceContainers = new Dictionary<string, ResourceContainer>();
      IVssServiceHost collectionServiceHost = requestContext.ServiceHost.CollectionServiceHost;
      string str = (string) null;
      if (collectionServiceHost != null)
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.ProjectCollection);
        ILocationService service = vssRequestContext.GetService<ILocationService>();
        ResourceContainer resourceContainer = new ResourceContainer()
        {
          Id = collectionServiceHost.InstanceId
        };
        if (service != null)
        {
          str = service.GetLocationServiceUrl(vssRequestContext, Guid.Empty, AccessMappingConstants.PublicAccessMappingMoniker);
          resourceContainer.BaseUrl = str;
        }
        resourceContainers.Add("collection", resourceContainer);
      }
      IVssServiceHost organizationServiceHost = requestContext.ServiceHost.OrganizationServiceHost;
      if (organizationServiceHost != null)
      {
        if (organizationServiceHost.IsOnly(TeamFoundationHostType.Application))
        {
          requestContext.To(TeamFoundationHostType.Application).GetService<ILocationService>();
          ResourceContainer resourceContainer = new ResourceContainer()
          {
            Id = organizationServiceHost.InstanceId,
            BaseUrl = str
          };
          resourceContainers.Add("account", resourceContainer);
        }
        else
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          ILocationService service = vssRequestContext.GetService<ILocationService>();
          ResourceContainer resourceContainer = new ResourceContainer()
          {
            Id = organizationServiceHost.InstanceId
          };
          if (service != null)
            resourceContainer.BaseUrl = service.GetLocationServiceUrl(vssRequestContext, Guid.Empty, AccessMappingConstants.PublicAccessMappingMoniker);
          resourceContainers.Add("server", resourceContainer);
        }
      }
      if (projectId != Guid.Empty)
      {
        ResourceContainer resourceContainer = new ResourceContainer()
        {
          Id = projectId,
          BaseUrl = str
        };
        resourceContainers.Add("project", resourceContainer);
      }
      if (resourceContainers.Count != 0)
        return resourceContainers;
      requestContext.Trace(0, TraceLevel.Error, EventDataExtensions.s_area, EventDataExtensions.s_layer, "Service Hook payload generation is returning null from GetResourceContainers");
      return (Dictionary<string, ResourceContainer>) null;
    }
  }
}
