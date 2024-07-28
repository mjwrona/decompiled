// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Location.LocationServerMapCache
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;

namespace Microsoft.VisualStudio.Services.WebApi.Location
{
  internal static class LocationServerMapCache
  {
    private static ReaderWriterLockSlim s_accessLock = new ReaderWriterLockSlim();
    private static Dictionary<string, ServerMapData> s_serverMappings = new Dictionary<string, ServerMapData>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static string s_filePath;
    private static FileSystemWatcher s_fileWatcher;
    private static bool s_cacheFreshLocally = false;
    private static bool s_cacheUnavailable = false;
    private static readonly string s_fileName = "LocationServerMap.xml";
    private static readonly string s_documentXmlText = "LocationServerMappings";
    private static readonly string s_mappingXmlText = "ServerMapping";
    private static readonly string s_locationAttribute = "location";
    private static readonly string s_guidAttribute = "guid";
    private static readonly string s_ownerAttribute = "owner";
    private static object s_cacheMutex = new object();

    public static string ReadServerLocation(Guid serverId, Guid serviceOwner)
    {
      try
      {
        LocationServerMapCache.EnsureCacheLoaded();
        LocationServerMapCache.s_accessLock.EnterReadLock();
        foreach (KeyValuePair<string, ServerMapData> serverMapping in LocationServerMapCache.s_serverMappings)
        {
          if (object.Equals((object) serverId, (object) serverMapping.Value.ServerId) && object.Equals((object) serviceOwner, (object) serverMapping.Value.ServiceOwner))
            return serverMapping.Key;
        }
        return (string) null;
      }
      finally
      {
        if (LocationServerMapCache.s_accessLock.IsReadLockHeld)
          LocationServerMapCache.s_accessLock.ExitReadLock();
      }
    }

    public static ServerMapData ReadServerData(string location)
    {
      try
      {
        LocationServerMapCache.EnsureCacheLoaded();
        LocationServerMapCache.s_accessLock.EnterReadLock();
        ServerMapData serverMapData;
        return !LocationServerMapCache.s_serverMappings.TryGetValue(location, out serverMapData) ? new ServerMapData() : serverMapData;
      }
      finally
      {
        if (LocationServerMapCache.s_accessLock.IsReadLockHeld)
          LocationServerMapCache.s_accessLock.ExitReadLock();
      }
    }

    public static bool EnsureServerMappingExists(string location, Guid serverId, Guid serviceOwner)
    {
      try
      {
        LocationServerMapCache.EnsureCacheLoaded();
        LocationServerMapCache.s_accessLock.EnterWriteLock();
        bool isNew = true;
        ServerMapData serverMapData;
        if (LocationServerMapCache.s_serverMappings.TryGetValue(location, out serverMapData))
        {
          if (serverMapData.ServerId.Equals(serverId) && serverMapData.ServiceOwner.Equals(serviceOwner))
            return false;
          isNew = false;
        }
        LocationServerMapCache.s_serverMappings[location] = new ServerMapData(serverId, serviceOwner);
        LocationServerMapCache.s_accessLock.ExitWriteLock();
        return LocationServerMapCache.TryWriteMappingToDisk(location, serverId, serviceOwner, isNew);
      }
      finally
      {
        if (LocationServerMapCache.s_accessLock.IsWriteLockHeld)
          LocationServerMapCache.s_accessLock.ExitWriteLock();
      }
    }

    private static void EnsureCacheLoaded()
    {
      if (LocationServerMapCache.s_cacheFreshLocally || LocationServerMapCache.s_cacheUnavailable)
        return;
      FileStream file = (FileStream) null;
      try
      {
        LocationServerMapCache.s_accessLock.EnterWriteLock();
        if (LocationServerMapCache.s_cacheFreshLocally)
          return;
        XmlDocument xmlDocument = XmlUtility.OpenXmlFile(out file, LocationServerMapCache.FilePath, FileShare.Read, false);
        if (xmlDocument != null)
        {
          foreach (XmlNode childNode in xmlDocument.ChildNodes[0].ChildNodes)
          {
            string innerText = childNode.Attributes[LocationServerMapCache.s_locationAttribute].InnerText;
            Guid guid = XmlConvert.ToGuid(childNode.Attributes[LocationServerMapCache.s_guidAttribute].InnerText);
            Guid serviceOwner = Guid.Empty;
            if (childNode.Attributes[LocationServerMapCache.s_ownerAttribute] != null)
              serviceOwner = XmlConvert.ToGuid(childNode.Attributes[LocationServerMapCache.s_ownerAttribute].InnerText);
            if (Guid.Empty == serviceOwner)
              serviceOwner = ServiceInstanceTypes.TFSOnPremises;
            LocationServerMapCache.s_serverMappings[innerText] = new ServerMapData(guid, serviceOwner);
          }
        }
        if (LocationServerMapCache.s_fileWatcher != null)
          return;
        string clientCacheDirectory = VssClientSettings.ClientCacheDirectory;
        if (!Directory.Exists(clientCacheDirectory))
          Directory.CreateDirectory(clientCacheDirectory);
        LocationServerMapCache.s_fileWatcher = new FileSystemWatcher(clientCacheDirectory, LocationServerMapCache.s_fileName);
        LocationServerMapCache.s_fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
        LocationServerMapCache.s_fileWatcher.Changed += new FileSystemEventHandler(LocationServerMapCache.s_fileWatcher_Changed);
      }
      catch (Exception ex)
      {
        LocationServerMapCache.s_cacheUnavailable = true;
      }
      finally
      {
        LocationServerMapCache.s_cacheFreshLocally = true;
        file?.Close();
        if (LocationServerMapCache.s_accessLock.IsWriteLockHeld)
          LocationServerMapCache.s_accessLock.ExitWriteLock();
      }
    }

    private static void s_fileWatcher_Changed(object sender, FileSystemEventArgs e) => LocationServerMapCache.s_cacheFreshLocally = false;

    private static bool TryWriteMappingToDisk(
      string location,
      Guid serverGuid,
      Guid serviceOwner,
      bool isNew)
    {
      if (LocationServerMapCache.s_cacheUnavailable)
        return false;
      FileStream file = (FileStream) null;
      try
      {
        XmlDocument xmlDocument = XmlUtility.OpenXmlFile(out file, LocationServerMapCache.FilePath, FileShare.None, true);
        lock (LocationServerMapCache.s_cacheMutex)
        {
          if (xmlDocument == null)
          {
            xmlDocument = new XmlDocument();
            XmlNode node = xmlDocument.CreateNode(XmlNodeType.Element, LocationServerMapCache.s_documentXmlText, (string) null);
            xmlDocument.AppendChild(node);
            LocationServerMapCache.AddMappingNode(node, location, serverGuid, serviceOwner);
          }
          else
          {
            XmlNode childNode1 = xmlDocument.ChildNodes[0];
            if (isNew)
            {
              LocationServerMapCache.AddMappingNode(childNode1, location, serverGuid, serviceOwner);
            }
            else
            {
              foreach (XmlNode childNode2 in childNode1.ChildNodes)
              {
                if (StringComparer.OrdinalIgnoreCase.Equals(childNode2.Attributes[LocationServerMapCache.s_locationAttribute].InnerText, location))
                {
                  childNode2.Attributes[LocationServerMapCache.s_guidAttribute].InnerText = XmlConvert.ToString(serverGuid);
                  if (ServiceInstanceTypes.TFSOnPremises == serviceOwner)
                    serviceOwner = Guid.Empty;
                  XmlAttribute attribute = xmlDocument.CreateAttribute(LocationServerMapCache.s_ownerAttribute);
                  attribute.InnerText = XmlConvert.ToString(serviceOwner);
                  childNode2.Attributes.Append(attribute);
                }
              }
            }
          }
          file.SetLength(0L);
          file.Position = 0L;
          xmlDocument.Save((Stream) file);
          return true;
        }
      }
      catch (Exception ex)
      {
        LocationServerMapCache.s_cacheUnavailable = true;
        return false;
      }
      finally
      {
        file?.Close();
      }
    }

    private static void AddMappingNode(XmlNode parentNode, string location, Guid guid, Guid owner)
    {
      XmlNode node = parentNode.OwnerDocument.CreateNode(XmlNodeType.Element, LocationServerMapCache.s_mappingXmlText, (string) null);
      parentNode.AppendChild(node);
      XmlUtility.AddXmlAttribute(node, LocationServerMapCache.s_locationAttribute, location);
      XmlUtility.AddXmlAttribute(node, LocationServerMapCache.s_guidAttribute, XmlConvert.ToString(guid));
      if (ServiceInstanceTypes.TFSOnPremises == owner)
        owner = Guid.Empty;
      if (!(owner != Guid.Empty))
        return;
      XmlUtility.AddXmlAttribute(node, LocationServerMapCache.s_ownerAttribute, XmlConvert.ToString(owner));
    }

    private static string FilePath
    {
      get
      {
        if (LocationServerMapCache.s_filePath == null)
          LocationServerMapCache.s_filePath = Path.Combine(VssClientSettings.ClientCacheDirectory, LocationServerMapCache.s_fileName);
        return LocationServerMapCache.s_filePath;
      }
    }
  }
}
