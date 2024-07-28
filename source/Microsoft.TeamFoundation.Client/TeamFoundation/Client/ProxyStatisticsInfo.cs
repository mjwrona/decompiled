// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ProxyStatisticsInfo
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Client
{
  public sealed class ProxyStatisticsInfo
  {
    private float m_cacheHitsPercentage;
    private float m_cacheMissPercentage;
    private long m_currentCacheSize;
    private long m_noOfFilesInCache;
    private long m_noOfRequests;
    private long m_overallCacheHits;
    private long m_overallCacheMisses;
    private bool m_scanComplete;
    private string m_serverId;
    private string m_serverUrl;

    private ProxyStatisticsInfo()
    {
    }

    public float CacheHitsPercentage
    {
      get => this.m_cacheHitsPercentage;
      set => this.m_cacheHitsPercentage = value;
    }

    public float CacheMissPercentage
    {
      get => this.m_cacheMissPercentage;
      set => this.m_cacheMissPercentage = value;
    }

    public long CurrentCacheSize
    {
      get => this.m_currentCacheSize;
      set => this.m_currentCacheSize = value;
    }

    public long NoOfFilesInCache
    {
      get => this.m_noOfFilesInCache;
      set => this.m_noOfFilesInCache = value;
    }

    public long NoOfRequests
    {
      get => this.m_noOfRequests;
      set => this.m_noOfRequests = value;
    }

    public long OverallCacheHits
    {
      get => this.m_overallCacheHits;
      set => this.m_overallCacheHits = value;
    }

    public long OverallCacheMisses
    {
      get => this.m_overallCacheMisses;
      set => this.m_overallCacheMisses = value;
    }

    public bool ScanComplete
    {
      get => this.m_scanComplete;
      set => this.m_scanComplete = value;
    }

    public string ServerId
    {
      get => this.m_serverId;
      set => this.m_serverId = value;
    }

    public string ServerUrl
    {
      get => this.m_serverUrl;
      set => this.m_serverUrl = value;
    }

    internal static ProxyStatisticsInfo FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      ProxyStatisticsInfo proxyStatisticsInfo = new ProxyStatisticsInfo();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          string name = reader.Name;
          if (name != null)
          {
            switch (name.Length)
            {
              case 8:
                if (name == "ServerId")
                {
                  proxyStatisticsInfo.m_serverId = XmlUtility.StringFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 9:
                if (name == "ServerUrl")
                {
                  proxyStatisticsInfo.m_serverUrl = XmlUtility.StringFromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 12:
                switch (name[0])
                {
                  case 'N':
                    if (name == "NoOfRequests")
                    {
                      proxyStatisticsInfo.m_noOfRequests = XmlUtility.Int64FromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'S':
                    if (name == "ScanComplete")
                    {
                      proxyStatisticsInfo.m_scanComplete = XmlUtility.BooleanFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 16:
                switch (name[0])
                {
                  case 'C':
                    if (name == "CurrentCacheSize")
                    {
                      proxyStatisticsInfo.m_currentCacheSize = XmlUtility.Int64FromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'N':
                    if (name == "NoOfFilesInCache")
                    {
                      proxyStatisticsInfo.m_noOfFilesInCache = XmlUtility.Int64FromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'O':
                    if (name == "OverallCacheHits")
                    {
                      proxyStatisticsInfo.m_overallCacheHits = XmlUtility.Int64FromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 18:
                if (name == "OverallCacheMisses")
                {
                  proxyStatisticsInfo.m_overallCacheMisses = XmlUtility.Int64FromXmlAttribute(reader);
                  continue;
                }
                continue;
              case 19:
                switch (name[5])
                {
                  case 'H':
                    if (name == "CacheHitsPercentage")
                    {
                      proxyStatisticsInfo.m_cacheHitsPercentage = XmlUtility.SingleFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  case 'M':
                    if (name == "CacheMissPercentage")
                    {
                      proxyStatisticsInfo.m_cacheMissPercentage = XmlUtility.SingleFromXmlAttribute(reader);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              default:
                continue;
            }
          }
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          string name = reader.Name;
          reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return proxyStatisticsInfo;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ProxyStatisticsInfo instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  CacheHitsPercentage: " + this.m_cacheHitsPercentage.ToString());
      stringBuilder.AppendLine("  CacheMissPercentage: " + this.m_cacheMissPercentage.ToString());
      stringBuilder.AppendLine("  CurrentCacheSize: " + this.m_currentCacheSize.ToString());
      stringBuilder.AppendLine("  NoOfFilesInCache: " + this.m_noOfFilesInCache.ToString());
      stringBuilder.AppendLine("  NoOfRequests: " + this.m_noOfRequests.ToString());
      stringBuilder.AppendLine("  OverallCacheHits: " + this.m_overallCacheHits.ToString());
      stringBuilder.AppendLine("  OverallCacheMisses: " + this.m_overallCacheMisses.ToString());
      stringBuilder.AppendLine("  ScanComplete: " + this.m_scanComplete.ToString());
      stringBuilder.AppendLine("  ServerId: " + this.m_serverId);
      stringBuilder.AppendLine("  ServerUrl: " + this.m_serverUrl);
      return stringBuilder.ToString();
    }

    internal void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if ((double) this.m_cacheHitsPercentage != 0.0)
        XmlUtility.ToXmlAttribute(writer, "CacheHitsPercentage", this.m_cacheHitsPercentage);
      if ((double) this.m_cacheMissPercentage != 0.0)
        XmlUtility.ToXmlAttribute(writer, "CacheMissPercentage", this.m_cacheMissPercentage);
      if (this.m_currentCacheSize != 0L)
        XmlUtility.ToXmlAttribute(writer, "CurrentCacheSize", this.m_currentCacheSize);
      if (this.m_noOfFilesInCache != 0L)
        XmlUtility.ToXmlAttribute(writer, "NoOfFilesInCache", this.m_noOfFilesInCache);
      if (this.m_noOfRequests != 0L)
        XmlUtility.ToXmlAttribute(writer, "NoOfRequests", this.m_noOfRequests);
      if (this.m_overallCacheHits != 0L)
        XmlUtility.ToXmlAttribute(writer, "OverallCacheHits", this.m_overallCacheHits);
      if (this.m_overallCacheMisses != 0L)
        XmlUtility.ToXmlAttribute(writer, "OverallCacheMisses", this.m_overallCacheMisses);
      if (this.m_scanComplete)
        XmlUtility.ToXmlAttribute(writer, "ScanComplete", this.m_scanComplete);
      if (this.m_serverId != null)
        XmlUtility.ToXmlAttribute(writer, "ServerId", this.m_serverId);
      if (this.m_serverUrl != null)
        XmlUtility.ToXmlAttribute(writer, "ServerUrl", this.m_serverUrl);
      writer.WriteEndElement();
    }

    internal static void ToXml(XmlWriter writer, string element, ProxyStatisticsInfo obj) => obj.ToXml(writer, element);
  }
}
