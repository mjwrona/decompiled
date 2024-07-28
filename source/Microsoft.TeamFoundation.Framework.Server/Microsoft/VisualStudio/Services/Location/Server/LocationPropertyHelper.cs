// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.LocationPropertyHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  internal static class LocationPropertyHelper
  {
    private static int m_RetryCount = 3;
    private static TimeSpan m_RetryDelay = TimeSpan.FromSeconds(10.0);

    private static string GetArtifactMoniker(ServiceDefinition serviceDefinition) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) serviceDefinition.ServiceType, (object) serviceDefinition.Identifier);

    private static ArtifactSpec MakeArtifactSpec(ServiceDefinition serviceDefinition)
    {
      string artifactMoniker = LocationPropertyHelper.GetArtifactMoniker(serviceDefinition);
      Guid location = ArtifactKinds.Location;
      string moniker = !string.IsNullOrEmpty(artifactMoniker) ? artifactMoniker : throw new ArgumentOutOfRangeException("location");
      return new ArtifactSpec(location, moniker, 0);
    }

    private static List<ArtifactSpec> MakeArtifactSpecs(
      IEnumerable<ServiceDefinition> serviceDefinitions)
    {
      List<ArtifactSpec> artifactSpecList = new List<ArtifactSpec>();
      foreach (ServiceDefinition serviceDefinition in serviceDefinitions)
        artifactSpecList.Add(LocationPropertyHelper.MakeArtifactSpec(serviceDefinition));
      return artifactSpecList;
    }

    private static List<ArtifactPropertyValue> MakeArtifactPropertyValues(
      ServiceDefinition serviceDefinition)
    {
      List<ArtifactPropertyValue> artifactPropertyValueList = new List<ArtifactPropertyValue>();
      List<PropertyValue> propertyValueList = new List<PropertyValue>();
      foreach (string key in serviceDefinition.Properties.Keys)
      {
        object obj;
        serviceDefinition.TryGetProperty(key, out obj);
        propertyValueList.Add(new PropertyValue(key, obj));
      }
      artifactPropertyValueList.Add(new ArtifactPropertyValue(LocationPropertyHelper.MakeArtifactSpec(serviceDefinition), (IEnumerable<PropertyValue>) propertyValueList));
      return artifactPropertyValueList;
    }

    internal static void FetchServiceDefinitionProperties(
      IVssRequestContext requestContext,
      List<ServiceDefinition> serviceDefinitions,
      IEnumerable<string> propertyNameFilters = null)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnsupportedHostTypeException(requestContext.ServiceHost.HostType);
      if (serviceDefinitions.Count <= 0)
        return;
      using (TeamFoundationDataReader properties = requestContext.GetService<TeamFoundationPropertyService>().GetProperties(requestContext, (IEnumerable<ArtifactSpec>) LocationPropertyHelper.MakeArtifactSpecs((IEnumerable<ServiceDefinition>) serviceDefinitions), propertyNameFilters))
      {
        IEnumerator<ServiceDefinition> enumerator = (IEnumerator<ServiceDefinition>) serviceDefinitions.GetEnumerator();
        bool flag = enumerator.MoveNext();
        ServiceDefinition current = enumerator.Current;
        foreach (ArtifactPropertyValue artifactPropertyValue in properties)
        {
          for (string moniker = artifactPropertyValue.Spec.Moniker; current == null || LocationPropertyHelper.GetArtifactMoniker(current) != moniker; current = enumerator.Current)
          {
            flag = enumerator.MoveNext();
            if (!flag)
              break;
          }
          if (!flag)
            break;
          foreach (PropertyValue propertyValue in artifactPropertyValue.PropertyValues)
            current.SetProperty(propertyValue.PropertyName, propertyValue.Value);
          current.ResetModifiedProperties();
        }
      }
    }

    internal static void FetchServiceDefinitionProperties(
      IVssRequestContext requestContext,
      ServiceDefinition serviceDefinition,
      IEnumerable<string> propertyNameFilters = null)
    {
      IVssRequestContext requestContext1 = requestContext;
      List<ServiceDefinition> serviceDefinitions = new List<ServiceDefinition>();
      serviceDefinitions.Add(serviceDefinition);
      IEnumerable<string> propertyNameFilters1 = propertyNameFilters;
      LocationPropertyHelper.FetchServiceDefinitionProperties(requestContext1, serviceDefinitions, propertyNameFilters1);
    }

    internal static bool UpdateServiceDefinitionProperties(
      IVssRequestContext requestContext,
      IEnumerable<ServiceDefinition> serviceDefinitions)
    {
      List<ArtifactPropertyValue> artifactPropertyValueList = new List<ArtifactPropertyValue>();
      foreach (ServiceDefinition serviceDefinition in serviceDefinitions)
      {
        if (serviceDefinition.HasModifiedProperties && serviceDefinition.Properties.Count > 0)
          artifactPropertyValueList.AddRange((IEnumerable<ArtifactPropertyValue>) LocationPropertyHelper.MakeArtifactPropertyValues(serviceDefinition));
      }
      if (artifactPropertyValueList.Count <= 0)
        return false;
      bool flag = requestContext.GetService<TeamFoundationPropertyService>().SetProperties(requestContext, (IEnumerable<ArtifactPropertyValue>) artifactPropertyValueList);
      foreach (ServiceDefinition serviceDefinition in serviceDefinitions)
        serviceDefinition.ResetModifiedProperties();
      return flag;
    }

    internal static PropertiesCollection LoadProperties(
      IVssRequestContext requestContext,
      string hostedServiceDnsName,
      bool syncExistingHostsOnly)
    {
      PropertiesCollection properties = new PropertiesCollection();
      properties.Add("Microsoft.TeamFoundation.Location.SyncExistingHostsOnly", (object) syncExistingHostsOnly);
      if (!string.IsNullOrEmpty(hostedServiceDnsName) && !requestContext.ExecutionEnvironment.IsDevFabricDeployment)
        new RetryManager(LocationPropertyHelper.m_RetryCount, LocationPropertyHelper.m_RetryDelay).Invoke((Action) (() =>
        {
          IPAddress ipAddress = ((IEnumerable<IPAddress>) Dns.GetHostAddresses(hostedServiceDnsName)).FirstOrDefault<IPAddress>((Func<IPAddress, bool>) (x => x.AddressFamily == AddressFamily.InterNetwork));
          if (ipAddress == null)
            return;
          properties.Add("Microsoft.TeamFoundation.Location.AzureVIP", (object) ipAddress.ToString());
        }));
      return properties;
    }
  }
}
