// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.LocationServerMapCache
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public static class LocationServerMapCache
  {
    private static ReaderWriterLockSlim s_accessLock = new ReaderWriterLockSlim();
    private static Dictionary<string, Guid> s_serverMappings = new Dictionary<string, Guid>((IEqualityComparer<string>) VssStringComparer.ServerUrl);
    private static string s_filePath;
    private static FileSystemWatcher s_fileWatcher;
    private static bool s_cacheFreshLocally = false;
    private static bool s_cacheUnavailable = false;
    private static object s_cacheMutex = new object();
    private static readonly string s_fileName = "LocationServerMap.xml";
    private static readonly string s_documentXmlText = "LocationServerMappings";
    private static readonly string s_mappingXmlText = "ServerMapping";
    private static readonly string s_locationAttribute = "location";
    private static readonly string s_guidAttribute = "guid";

    public static string ReadServerLocation(Guid serverId)
    {
      try
      {
        LocationServerMapCache.EnsureCacheLoaded();
        LocationServerMapCache.s_accessLock.EnterReadLock();
        foreach (KeyValuePair<string, Guid> serverMapping in LocationServerMapCache.s_serverMappings)
        {
          if (object.Equals((object) serverId, (object) serverMapping.Value))
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

    public static Guid ReadServerGuid(string location)
    {
      try
      {
        LocationServerMapCache.EnsureCacheLoaded();
        LocationServerMapCache.s_accessLock.EnterReadLock();
        Guid guid;
        return !LocationServerMapCache.s_serverMappings.TryGetValue(location, out guid) ? Guid.Empty : guid;
      }
      finally
      {
        if (LocationServerMapCache.s_accessLock.IsReadLockHeld)
          LocationServerMapCache.s_accessLock.ExitReadLock();
      }
    }

    public static bool EnsureServerMappingExists(string location, Guid serverId)
    {
      try
      {
        LocationServerMapCache.EnsureCacheLoaded();
        LocationServerMapCache.s_accessLock.EnterWriteLock();
        bool isNew = true;
        Guid guid;
        if (LocationServerMapCache.s_serverMappings.TryGetValue(location, out guid))
        {
          if (guid.Equals(serverId))
            return false;
          isNew = false;
        }
        LocationServerMapCache.s_serverMappings[location] = serverId;
        LocationServerMapCache.s_accessLock.ExitWriteLock();
        return LocationServerMapCache.TryWriteMappingToDisk(location, serverId, isNew);
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
        XmlDocument xmlDocument = TFCommonUtil.OpenXmlFile(out file, LocationServerMapCache.FilePath, FileShare.Read, false);
        if (xmlDocument != null)
        {
          foreach (XmlNode childNode in xmlDocument.ChildNodes[0].ChildNodes)
          {
            string innerText = childNode.Attributes[LocationServerMapCache.s_locationAttribute].InnerText;
            Guid guid = XmlConvert.ToGuid(childNode.Attributes[LocationServerMapCache.s_guidAttribute].InnerText);
            LocationServerMapCache.s_serverMappings[innerText] = guid;
          }
        }
        if (LocationServerMapCache.s_fileWatcher != null)
          return;
        LocationServerMapCache.s_fileWatcher = new FileSystemWatcher(TfsConnection.ClientCacheDirectory, LocationServerMapCache.s_fileName);
        LocationServerMapCache.s_fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
        LocationServerMapCache.s_fileWatcher.Changed += new FileSystemEventHandler(LocationServerMapCache.s_fileWatcher_Changed);
      }
      catch (Exception ex)
      {
        LocationServerMapCache.s_cacheUnavailable = true;
        TeamFoundationTrace.Warning("Unable to read LocationServerMap.xml because: '{0}'", (object) ex.Message);
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

    private static bool TryWriteMappingToDisk(string location, Guid serverGuid, bool isNew)
    {
      if (LocationServerMapCache.s_cacheUnavailable)
        return false;
      FileStream file = (FileStream) null;
      try
      {
        XmlDocument xmlDocument = TFCommonUtil.OpenXmlFile(out file, LocationServerMapCache.FilePath, FileShare.None, true);
        lock (LocationServerMapCache.s_cacheMutex)
        {
          if (xmlDocument == null)
          {
            xmlDocument = new XmlDocument();
            XmlNode node = xmlDocument.CreateNode(XmlNodeType.Element, LocationServerMapCache.s_documentXmlText, (string) null);
            xmlDocument.AppendChild(node);
            LocationServerMapCache.AddMappingNode(node, location, serverGuid);
          }
          else
          {
            XmlNode childNode1 = xmlDocument.ChildNodes[0];
            if (isNew)
            {
              LocationServerMapCache.AddMappingNode(childNode1, location, serverGuid);
            }
            else
            {
              foreach (XmlNode childNode2 in childNode1.ChildNodes)
              {
                if (VssStringComparer.ServerUrl.Equals(childNode2.Attributes[LocationServerMapCache.s_locationAttribute].InnerText, location))
                  childNode2.Attributes[LocationServerMapCache.s_guidAttribute].InnerText = XmlConvert.ToString(serverGuid);
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
        TeamFoundationTrace.Warning("Unable to write to LocationServerMap.xml because: '{0}'", (object) ex.Message);
        return false;
      }
      finally
      {
        file?.Close();
      }
    }

    private static void AddMappingNode(XmlNode parentNode, string location, Guid guid)
    {
      XmlNode node = parentNode.OwnerDocument.CreateNode(XmlNodeType.Element, LocationServerMapCache.s_mappingXmlText, (string) null);
      parentNode.AppendChild(node);
      TFCommonUtil.AddXmlAttribute(node, LocationServerMapCache.s_locationAttribute, location);
      TFCommonUtil.AddXmlAttribute(node, LocationServerMapCache.s_guidAttribute, XmlConvert.ToString(guid));
    }

    private static string FilePath
    {
      get
      {
        if (LocationServerMapCache.s_filePath == null)
          LocationServerMapCache.s_filePath = Path.Combine(TfsConnection.ClientCacheDirectory, LocationServerMapCache.s_fileName);
        return LocationServerMapCache.s_filePath;
      }
    }
  }
}
