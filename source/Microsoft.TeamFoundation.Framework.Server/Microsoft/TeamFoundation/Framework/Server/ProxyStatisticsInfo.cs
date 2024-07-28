// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ProxyStatisticsInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ProxyStatisticsInfo
  {
    private bool m_scanComplete;
    private long m_currentCacheSize;
    private long m_noOfRequests;
    private long m_overallCacheHits;
    private long m_noOfFilesInCache;
    private long m_overallCacheMisses;
    private float m_cacheHitsPercentage;
    private float m_cacheMissPercentage;
    private string m_serverId;
    private string m_serverUrl;

    [XmlAttribute]
    public long CurrentCacheSize
    {
      get => this.m_currentCacheSize;
      set => this.m_currentCacheSize = value;
    }

    [XmlAttribute]
    public long NoOfRequests
    {
      get => this.m_noOfRequests;
      set => this.m_noOfRequests = value;
    }

    [XmlAttribute]
    public long OverallCacheHits
    {
      get => this.m_overallCacheHits;
      set => this.m_overallCacheHits = value;
    }

    [XmlAttribute]
    public long NoOfFilesInCache
    {
      get => this.m_noOfFilesInCache;
      set => this.m_noOfFilesInCache = value;
    }

    [XmlAttribute]
    public long OverallCacheMisses
    {
      get => this.m_overallCacheMisses;
      set => this.m_overallCacheMisses = value;
    }

    [XmlAttribute]
    public float CacheHitsPercentage
    {
      get => this.m_cacheHitsPercentage;
      set => this.m_cacheHitsPercentage = value;
    }

    [XmlAttribute]
    public float CacheMissPercentage
    {
      get => this.m_cacheMissPercentage;
      set => this.m_cacheMissPercentage = value;
    }

    [XmlAttribute]
    public string ServerId
    {
      get => this.m_serverId;
      set => this.m_serverId = value;
    }

    [XmlAttribute]
    public string ServerUrl
    {
      get => this.m_serverUrl;
      set => this.m_serverUrl = value;
    }

    [XmlAttribute]
    public bool ScanComplete
    {
      get => this.m_scanComplete;
      set => this.m_scanComplete = value;
    }
  }
}
