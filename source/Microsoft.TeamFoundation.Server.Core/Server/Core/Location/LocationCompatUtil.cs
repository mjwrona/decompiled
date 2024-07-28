// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Location.LocationCompatUtil
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core.Location
{
  public static class LocationCompatUtil
  {
    public static IEnumerable<ServiceDefinition> Convert(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Location.ServiceDefinition> definitions)
    {
      if (definitions == null)
        return (IEnumerable<ServiceDefinition>) null;
      LocationServiceHelper.FixVirtualServiceDefinitions(requestContext, definitions);
      return (IEnumerable<ServiceDefinition>) definitions.Select<Microsoft.VisualStudio.Services.Location.ServiceDefinition, ServiceDefinition>((Func<Microsoft.VisualStudio.Services.Location.ServiceDefinition, ServiceDefinition>) (x => LocationCompatUtil.Convert(x))).ToList<ServiceDefinition>();
    }

    public static ServiceDefinition Convert(Microsoft.VisualStudio.Services.Location.ServiceDefinition definition)
    {
      if (definition == null)
        return (ServiceDefinition) null;
      ServiceDefinition serviceDefinition = new ServiceDefinition();
      serviceDefinition.ServiceType = definition.ServiceType;
      serviceDefinition.Identifier = definition.Identifier;
      serviceDefinition.DisplayName = definition.DisplayName;
      serviceDefinition.RelativePath = definition.RelativePath;
      serviceDefinition.RelativeToSetting = LocationCompatUtil.Convert(definition.RelativeToSetting);
      serviceDefinition.Description = definition.Description;
      serviceDefinition.ToolId = definition.ToolId;
      if (definition.LocationMappings != null)
        serviceDefinition.LocationMappings.AddRange((IEnumerable<LocationMapping>) LocationCompatUtil.Convert((IList<Microsoft.VisualStudio.Services.Location.LocationMapping>) definition.LocationMappings, definition));
      return serviceDefinition;
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.Location.ServiceDefinition> Convert(
      IEnumerable<ServiceDefinition> definitions)
    {
      return definitions == null ? (IEnumerable<Microsoft.VisualStudio.Services.Location.ServiceDefinition>) null : (IEnumerable<Microsoft.VisualStudio.Services.Location.ServiceDefinition>) definitions.Select<ServiceDefinition, Microsoft.VisualStudio.Services.Location.ServiceDefinition>((Func<ServiceDefinition, Microsoft.VisualStudio.Services.Location.ServiceDefinition>) (x => LocationCompatUtil.Convert(x))).ToList<Microsoft.VisualStudio.Services.Location.ServiceDefinition>();
    }

    public static Microsoft.VisualStudio.Services.Location.ServiceDefinition Convert(
      ServiceDefinition definition)
    {
      if (definition == null)
        return (Microsoft.VisualStudio.Services.Location.ServiceDefinition) null;
      Microsoft.VisualStudio.Services.Location.ServiceDefinition serviceDefinition = new Microsoft.VisualStudio.Services.Location.ServiceDefinition();
      serviceDefinition.ServiceType = definition.ServiceType;
      serviceDefinition.Identifier = definition.Identifier;
      serviceDefinition.DisplayName = definition.DisplayName;
      serviceDefinition.RelativePath = definition.RelativePath;
      serviceDefinition.RelativeToSetting = LocationCompatUtil.Convert(definition.RelativeToSetting);
      serviceDefinition.Description = definition.Description;
      serviceDefinition.ToolId = definition.ToolId;
      if (definition.LocationMappings != null)
      {
        foreach (Microsoft.VisualStudio.Services.Location.LocationMapping locationMapping in LocationCompatUtil.Convert((IList<LocationMapping>) definition.LocationMappings, definition))
          serviceDefinition.LocationMappings.Add(locationMapping);
      }
      return serviceDefinition;
    }

    private static Microsoft.TeamFoundation.Framework.Common.RelativeToSetting Convert(
      Microsoft.VisualStudio.Services.Location.RelativeToSetting setting)
    {
      switch (setting)
      {
        case Microsoft.VisualStudio.Services.Location.RelativeToSetting.Context:
          return Microsoft.TeamFoundation.Framework.Common.RelativeToSetting.Context;
        case Microsoft.VisualStudio.Services.Location.RelativeToSetting.WebApplication:
          return Microsoft.TeamFoundation.Framework.Common.RelativeToSetting.WebApplication;
        case Microsoft.VisualStudio.Services.Location.RelativeToSetting.FullyQualified:
          return Microsoft.TeamFoundation.Framework.Common.RelativeToSetting.FullyQualified;
        default:
          throw new ArgumentException("Invalid relative to setting");
      }
    }

    private static Microsoft.VisualStudio.Services.Location.RelativeToSetting Convert(
      Microsoft.TeamFoundation.Framework.Common.RelativeToSetting setting)
    {
      switch (setting)
      {
        case Microsoft.TeamFoundation.Framework.Common.RelativeToSetting.Context:
          return Microsoft.VisualStudio.Services.Location.RelativeToSetting.Context;
        case Microsoft.TeamFoundation.Framework.Common.RelativeToSetting.WebApplication:
          return Microsoft.VisualStudio.Services.Location.RelativeToSetting.WebApplication;
        case Microsoft.TeamFoundation.Framework.Common.RelativeToSetting.FullyQualified:
          return Microsoft.VisualStudio.Services.Location.RelativeToSetting.FullyQualified;
        default:
          throw new ArgumentException("Invalid relative to setting");
      }
    }

    public static IEnumerable<AccessMapping> Convert(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Location.AccessMapping> accessMappings)
    {
      return accessMappings == null ? (IEnumerable<AccessMapping>) null : (IEnumerable<AccessMapping>) accessMappings.Select<Microsoft.VisualStudio.Services.Location.AccessMapping, AccessMapping>((Func<Microsoft.VisualStudio.Services.Location.AccessMapping, AccessMapping>) (x => LocationCompatUtil.Convert(x))).ToList<AccessMapping>();
    }

    public static AccessMapping Convert(Microsoft.VisualStudio.Services.Location.AccessMapping accessMapping)
    {
      if (accessMapping == null)
        return (AccessMapping) null;
      return new AccessMapping()
      {
        AccessPoint = accessMapping.AccessPoint,
        DisplayName = accessMapping.DisplayName,
        Moniker = accessMapping.Moniker,
        VirtualDirectory = accessMapping.VirtualDirectory
      };
    }

    public static IEnumerable<Microsoft.VisualStudio.Services.Location.AccessMapping> Convert(
      IEnumerable<AccessMapping> accessMappings)
    {
      return accessMappings == null ? (IEnumerable<Microsoft.VisualStudio.Services.Location.AccessMapping>) null : (IEnumerable<Microsoft.VisualStudio.Services.Location.AccessMapping>) accessMappings.Select<AccessMapping, Microsoft.VisualStudio.Services.Location.AccessMapping>((Func<AccessMapping, Microsoft.VisualStudio.Services.Location.AccessMapping>) (x => LocationCompatUtil.Convert(x))).ToList<Microsoft.VisualStudio.Services.Location.AccessMapping>();
    }

    public static Microsoft.VisualStudio.Services.Location.AccessMapping Convert(
      AccessMapping accessMapping)
    {
      if (accessMapping == null)
        return (Microsoft.VisualStudio.Services.Location.AccessMapping) null;
      return new Microsoft.VisualStudio.Services.Location.AccessMapping()
      {
        AccessPoint = accessMapping.AccessPoint,
        DisplayName = accessMapping.DisplayName,
        Moniker = accessMapping.Moniker,
        VirtualDirectory = accessMapping.VirtualDirectory
      };
    }

    private static List<LocationMapping> Convert(
      IList<Microsoft.VisualStudio.Services.Location.LocationMapping> mappings,
      Microsoft.VisualStudio.Services.Location.ServiceDefinition definition)
    {
      if (mappings == null)
        return new List<LocationMapping>();
      List<LocationMapping> locationMappingList = new List<LocationMapping>(mappings.Count);
      foreach (Microsoft.VisualStudio.Services.Location.LocationMapping mapping in (IEnumerable<Microsoft.VisualStudio.Services.Location.LocationMapping>) mappings)
      {
        LocationMapping locationMapping = new LocationMapping(mapping.AccessMappingMoniker, mapping.Location);
        locationMappingList.Add(locationMapping);
      }
      return locationMappingList;
    }

    private static List<Microsoft.VisualStudio.Services.Location.LocationMapping> Convert(
      IList<LocationMapping> mappings,
      ServiceDefinition definition)
    {
      if (mappings == null)
        return new List<Microsoft.VisualStudio.Services.Location.LocationMapping>();
      List<Microsoft.VisualStudio.Services.Location.LocationMapping> locationMappingList = new List<Microsoft.VisualStudio.Services.Location.LocationMapping>(mappings.Count);
      foreach (LocationMapping mapping in (IEnumerable<LocationMapping>) mappings)
      {
        Microsoft.VisualStudio.Services.Location.LocationMapping locationMapping = new Microsoft.VisualStudio.Services.Location.LocationMapping()
        {
          AccessMappingMoniker = mapping.AccessMappingMoniker,
          Location = mapping.Location
        };
        locationMappingList.Add(locationMapping);
      }
      return locationMappingList;
    }
  }
}
