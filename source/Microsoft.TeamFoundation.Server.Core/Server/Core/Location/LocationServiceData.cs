// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Location.LocationServiceData
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core.Location
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class LocationServiceData
  {
    private List<ServiceDefinition> m_serviceDefinitions;
    private List<AccessMapping> m_accessMappings;

    private LocationServiceData()
    {
    }

    internal LocationServiceData(
      long lastChangeId,
      string defaultAccessMappingMoniker,
      bool clientCacheFresh,
      int clientCacheTimeToLive)
      : this(lastChangeId, (IEnumerable<ServiceDefinition>) null, (IEnumerable<AccessMapping>) null, defaultAccessMappingMoniker, clientCacheFresh, clientCacheTimeToLive, false)
    {
    }

    internal LocationServiceData(
      long lastChangeId,
      IEnumerable<ServiceDefinition> serviceDefinitions,
      IEnumerable<AccessMapping> accessMappings,
      string defaultAccessMappingMoniker,
      bool clientCacheFresh,
      int clientCacheTimeToLive,
      bool ensureTrailingSlash)
    {
      this.LastChangeId = (int) lastChangeId;
      if (accessMappings != null)
      {
        this.m_accessMappings = new List<AccessMapping>(accessMappings);
        if (ensureTrailingSlash)
        {
          foreach (AccessMapping accessMapping in this.m_accessMappings)
            accessMapping.AccessPoint = VirtualPathUtility.AppendTrailingSlash(accessMapping.AccessPoint);
        }
      }
      if (serviceDefinitions != null)
        this.m_serviceDefinitions = new List<ServiceDefinition>(serviceDefinitions);
      this.ClientCacheFresh = clientCacheFresh;
      this.ClientCacheTimeToLive = clientCacheTimeToLive;
      this.DefaultAccessMappingMoniker = defaultAccessMappingMoniker;
      this.AccessPointsDoNotIncludeWebAppRelativeDirectory = true;
    }

    public List<ServiceDefinition> ServiceDefinitions => this.m_serviceDefinitions;

    public List<AccessMapping> AccessMappings => this.m_accessMappings;

    [XmlAttribute]
    public string DefaultAccessMappingMoniker { get; set; }

    [XmlAttribute]
    public int LastChangeId { get; set; }

    [XmlAttribute]
    public bool ClientCacheFresh { get; set; }

    [XmlAttribute]
    [DefaultValue(3600)]
    public int ClientCacheTimeToLive { get; set; }

    [XmlAttribute]
    public bool AccessPointsDoNotIncludeWebAppRelativeDirectory { get; set; }
  }
}
