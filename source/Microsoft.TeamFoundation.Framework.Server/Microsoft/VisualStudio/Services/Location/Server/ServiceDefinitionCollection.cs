// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.ServiceDefinitionCollection
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  public class ServiceDefinitionCollection
  {
    private bool m_addCollectionToApplication;
    private Dictionary<ServiceDefinition, ServiceDefinition> m_allDefinitions = new Dictionary<ServiceDefinition, ServiceDefinition>((IEqualityComparer<ServiceDefinition>) ServiceDefinitionComparer.Instance);

    public ServiceDefinitionCollection(bool addCollectionToApplication = false) => this.m_addCollectionToApplication = addCollectionToApplication;

    public ServiceDefinitionCollection(
      ServiceDefinitionCollection other,
      bool addCollectionToApplication = false)
    {
      foreach (ServiceDefinition definition in other.GetDefinitions())
        this.m_allDefinitions.Add(definition, definition);
      this.m_addCollectionToApplication = addCollectionToApplication;
    }

    public void RegisterDefinition(
      TeamFoundationHostType hostType,
      string serviceType,
      string identifier,
      string displayName,
      string relativePath,
      string toolId,
      string description = null,
      RelativeToSetting relativeToSetting = RelativeToSetting.Context)
    {
      this.RegisterDefinition(hostType, serviceType, new Guid(identifier), displayName, relativePath, toolId, description, relativeToSetting);
    }

    public void RegisterDefinition(
      TeamFoundationHostType hostType,
      string serviceType,
      Guid identifier,
      string displayName,
      string relativePath,
      string toolId,
      string description = null,
      RelativeToSetting relativeToSetting = RelativeToSetting.Context,
      List<LocationMapping> locationMappings = null)
    {
      hostType = this.NormalizeHostType(hostType);
      ServiceDefinition definition = new ServiceDefinition(serviceType, identifier, displayName, relativePath, relativeToSetting, description, toolId, locationMappings)
      {
        InheritLevel = (InheritLevel) hostType
      };
      this.RegisterDefinition(hostType, definition, false);
    }

    public void RegisterDefinition(
      TeamFoundationHostType hostType,
      ServiceDefinition definition,
      bool skipUniqueCheck)
    {
      hostType = this.NormalizeHostType(hostType);
      definition.InheritLevel |= (InheritLevel) hostType;
      ServiceDefinition serviceDefinition;
      if (this.m_allDefinitions.TryGetValue(definition, out serviceDefinition))
      {
        if (!skipUniqueCheck)
          throw new ArgumentException(string.Format("Definition with type {0} and identifier {1} already exists", (object) definition.ServiceType, (object) definition.Identifier));
        serviceDefinition.InheritLevel |= definition.InheritLevel;
      }
      else
        this.m_allDefinitions.Add(definition, definition);
    }

    public IEnumerable<ServiceDefinition> GetDefinitions() => (IEnumerable<ServiceDefinition>) this.m_allDefinitions.Values;

    private TeamFoundationHostType NormalizeHostType(TeamFoundationHostType hostType)
    {
      if (this.m_addCollectionToApplication && hostType.HasFlag((Enum) TeamFoundationHostType.Application))
        hostType |= TeamFoundationHostType.ProjectCollection;
      return hostType;
    }
  }
}
