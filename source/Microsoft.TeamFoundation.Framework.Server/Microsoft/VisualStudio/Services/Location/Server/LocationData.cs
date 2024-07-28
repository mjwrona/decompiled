// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.LocationData
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  internal class LocationData
  {
    public static readonly LocationData Null = new LocationData(new Dictionary<string, AccessMapping>(), (AccessMapping) null, (AccessMapping) null, (AccessMapping) null, (IEnumerable<ServiceDefinition>) null, (string) null, DateTime.MinValue, -1L, Guid.Empty, Guid.Empty, Guid.Empty);

    [JsonConstructor]
    public LocationData(
      Dictionary<string, AccessMapping> accessMappings,
      AccessMapping defaultAccessMapping,
      AccessMapping publicAccessMapping,
      AccessMapping serverAccessMapping,
      Dictionary<string, Dictionary<Guid, ServiceDefinition>> serviceDefinitions,
      string webApplicationRelativeDirectory,
      DateTime cacheExpirationDate,
      long lastChangeId,
      Guid hostId,
      Guid instanceType,
      Guid deploymentId)
    {
      this.AccessMappings = accessMappings;
      this.DefaultAccessMapping = defaultAccessMapping;
      this.PublicAccessMapping = publicAccessMapping;
      this.ServerAccessMapping = serverAccessMapping;
      this.WebAppRelativeDirectory = webApplicationRelativeDirectory;
      this.ServiceDefinitions = new Dictionary<string, Dictionary<Guid, ServiceDefinition>>((IDictionary<string, Dictionary<Guid, ServiceDefinition>>) serviceDefinitions, (IEqualityComparer<string>) VssStringComparer.ServiceType);
      this.CacheExpirationDate = cacheExpirationDate;
      this.LastChangeId = lastChangeId;
      this.HostId = hostId;
      this.InstanceType = instanceType;
      this.DeploymentId = deploymentId;
    }

    public LocationData(
      Dictionary<string, AccessMapping> accessMappings,
      AccessMapping defaultAccessMapping,
      AccessMapping publicAccessMapping,
      AccessMapping serverAccessMapping,
      IEnumerable<ServiceDefinition> serviceDefinitions,
      string webApplicationRelativeDirectory,
      DateTime cacheExpirationDate,
      long lastChangeId,
      Guid hostId,
      Guid instanceType,
      Guid deploymentId)
      : this(accessMappings, defaultAccessMapping, publicAccessMapping, serverAccessMapping, new Dictionary<string, Dictionary<Guid, ServiceDefinition>>((IEqualityComparer<string>) VssStringComparer.ServiceType), webApplicationRelativeDirectory, cacheExpirationDate, lastChangeId, hostId, instanceType, deploymentId)
    {
      if (serviceDefinitions == null)
        return;
      this.AddServiceDefinitions(serviceDefinitions);
    }

    public Dictionary<string, AccessMapping> AccessMappings { get; private set; }

    public AccessMapping DefaultAccessMapping { get; private set; }

    public AccessMapping PublicAccessMapping { get; private set; }

    public AccessMapping ServerAccessMapping { get; private set; }

    public string WebAppRelativeDirectory { get; private set; }

    public Dictionary<string, Dictionary<Guid, ServiceDefinition>> ServiceDefinitions { get; private set; }

    public DateTime CacheExpirationDate { get; internal set; }

    public long LastChangeId { get; private set; }

    public Guid HostId { get; private set; }

    public Guid InstanceType { get; private set; }

    public Guid DeploymentId { get; private set; }

    public void AddServiceDefinitions(IEnumerable<ServiceDefinition> serviceDefinitions)
    {
      foreach (ServiceDefinition serviceDefinition in serviceDefinitions)
        this.AddServiceDefinition(serviceDefinition);
    }

    public void AddServiceDefinition(ServiceDefinition serviceDefinition)
    {
      Dictionary<Guid, ServiceDefinition> dictionary;
      if (!this.ServiceDefinitions.TryGetValue(serviceDefinition.ServiceType, out dictionary))
      {
        dictionary = new Dictionary<Guid, ServiceDefinition>();
        this.ServiceDefinitions[serviceDefinition.ServiceType] = dictionary;
      }
      dictionary[serviceDefinition.Identifier] = serviceDefinition;
    }

    public IEnumerable<ServiceDefinition> GetAllServiceDefinitions() => this.ServiceDefinitions.Values.SelectMany<Dictionary<Guid, ServiceDefinition>, ServiceDefinition>((Func<Dictionary<Guid, ServiceDefinition>, IEnumerable<ServiceDefinition>>) (x => (IEnumerable<ServiceDefinition>) x.Values));

    public ServiceDefinition FindServiceDefinition(string serviceType, Guid serviceIdentifier)
    {
      ServiceDefinition serviceDefinition = (ServiceDefinition) null;
      Dictionary<Guid, ServiceDefinition> dictionary;
      if (this.ServiceDefinitions.TryGetValue(serviceType, out dictionary))
        dictionary.TryGetValue(serviceIdentifier, out serviceDefinition);
      return serviceDefinition;
    }

    public ServiceDefinition FindServiceDefinitionByTypeTool(string serviceType, string toolId)
    {
      Dictionary<Guid, ServiceDefinition> dictionary = (Dictionary<Guid, ServiceDefinition>) null;
      List<ServiceDefinition> serviceDefinitionList = new List<ServiceDefinition>();
      if (!this.ServiceDefinitions.TryGetValue(serviceType, out dictionary))
        return (ServiceDefinition) null;
      foreach (ServiceDefinition serviceDefinition in dictionary.Values)
      {
        if (VssStringComparer.ToolId.Equals(serviceDefinition.ToolId, toolId))
          serviceDefinitionList.Add(serviceDefinition);
      }
      return serviceDefinitionList.Count == 1 ? serviceDefinitionList[0] : throw new TeamFoundationLocationServiceException(TFCommonResources.InvalidFindServiceByTypeAndToolId((object) serviceType, (object) toolId, (object) serviceDefinitionList.Count));
    }

    public IEnumerable<ServiceDefinition> FindServiceDefinitionsByTool(string toolId)
    {
      bool flag = string.IsNullOrEmpty(toolId);
      List<ServiceDefinition> definitionsByTool = new List<ServiceDefinition>();
      foreach (ServiceDefinition serviceDefinition in this.GetAllServiceDefinitions())
      {
        if (!string.IsNullOrEmpty(serviceDefinition.ToolId) && (flag || VssStringComparer.ToolId.Equals(serviceDefinition.ToolId, toolId)))
          definitionsByTool.Add(serviceDefinition);
      }
      return (IEnumerable<ServiceDefinition>) definitionsByTool;
    }

    public IEnumerable<ServiceDefinition> FindServiceDefinitionsByType(string serviceType)
    {
      List<ServiceDefinition> definitionsByType = new List<ServiceDefinition>();
      if (string.IsNullOrEmpty(serviceType))
      {
        foreach (ServiceDefinition serviceDefinition in this.GetAllServiceDefinitions())
          definitionsByType.Add(serviceDefinition);
      }
      else
      {
        Dictionary<Guid, ServiceDefinition> dictionary;
        if (this.ServiceDefinitions.TryGetValue(serviceType, out dictionary))
        {
          foreach (ServiceDefinition serviceDefinition in dictionary.Values)
            definitionsByType.Add(serviceDefinition);
        }
      }
      return (IEnumerable<ServiceDefinition>) definitionsByType;
    }

    public override string ToString() => "[accessMappings:{" + string.Join(",", ((IEnumerable<AccessMapping>) this.AccessMappings?.Values ?? Enumerable.Empty<AccessMapping>()).Select<AccessMapping, string>((Func<AccessMapping, string>) (am => LocationData.ToString(am)))) + "}, serviceDefinitions={" + string.Join(",", this.GetAllServiceDefinitions().Select<ServiceDefinition, string>((Func<ServiceDefinition, string>) (sd => LocationData.ToString(sd)))) + "}]";

    private static string ToString(AccessMapping accessMapping) => "[" + accessMapping?.Moniker + ":" + accessMapping?.AccessPoint + "]";

    private static string ToString(ServiceDefinition serviceDefinition) => string.Format("[{0}:{1}:{2}]", (object) serviceDefinition?.DisplayName, (object) serviceDefinition?.ServiceType, (object) serviceDefinition?.Identifier);
  }
}
