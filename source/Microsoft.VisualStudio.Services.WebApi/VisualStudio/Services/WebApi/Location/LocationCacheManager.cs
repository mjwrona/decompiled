// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Location.LocationCacheManager
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;

namespace Microsoft.VisualStudio.Services.WebApi.Location
{
  internal class LocationCacheManager
  {
    private Dictionary<string, Dictionary<Guid, ServiceDefinition>> m_services;
    private HashSet<string> m_cachedMisses;
    private Dictionary<string, AccessMapping> m_accessMappings;
    private int m_lastChangeId;
    private DateTime m_cacheExpirationDate;
    private ReaderWriterLockSlim m_accessLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
    private string m_webApplicationRelativeDirectory;
    private static object s_cacheMutex = new object();
    private bool m_cacheLocallyFresh;
    private bool m_cacheAvailable;
    private FileSystemWatcher m_fileSystemWatcher;
    private Uri m_connectionBaseUrl;
    private AccessMapping m_clientAccessMapping;
    private AccessMapping m_defaultAccessMapping;
    private static readonly string s_cacheFileName = "LocationServiceData.config";
    private string m_cacheFilePath;
    private LocationXmlOperator m_locationXmlOperator;
    private const string s_docStartElement = "LocationServiceConfiguration";

    public LocationCacheManager(Guid serverGuid, Guid serviceOwner, Uri connectionBaseUrl)
    {
      this.m_cacheAvailable = !serverGuid.Equals(Guid.Empty);
      this.m_lastChangeId = -1;
      this.m_cacheExpirationDate = DateTime.MinValue;
      this.ClientCacheTimeToLive = VssClientSettings.ClientCacheTimeToLive;
      this.m_cacheFilePath = !(serviceOwner == Guid.Empty) ? Path.Combine(Path.Combine(Path.Combine(VssClientSettings.ClientCacheDirectory, serverGuid.ToString()), serviceOwner.ToString()), LocationCacheManager.s_cacheFileName) : Path.Combine(Path.Combine(VssClientSettings.ClientCacheDirectory, serverGuid.ToString()), LocationCacheManager.s_cacheFileName);
      this.m_cacheLocallyFresh = false;
      this.m_accessMappings = new Dictionary<string, AccessMapping>((IEqualityComparer<string>) VssStringComparer.AccessMappingMoniker);
      this.m_services = new Dictionary<string, Dictionary<Guid, ServiceDefinition>>((IEqualityComparer<string>) VssStringComparer.ServiceType);
      this.m_cachedMisses = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.ServiceType);
      this.m_connectionBaseUrl = connectionBaseUrl;
      this.m_locationXmlOperator = new LocationXmlOperator(true);
    }

    public bool LocalCacheAvailable
    {
      get
      {
        this.EnsureDiskCacheLoaded();
        return this.m_cacheAvailable;
      }
    }

    internal bool CacheDataExpired => this.m_cacheAvailable && this.m_cacheLocallyFresh && DateTime.UtcNow >= this.m_cacheExpirationDate;

    public AccessMapping ClientAccessMapping
    {
      get
      {
        this.m_accessLock.EnterReadLock();
        try
        {
          return !this.CacheDataExpired ? this.m_clientAccessMapping : (AccessMapping) null;
        }
        finally
        {
          this.m_accessLock.ExitReadLock();
        }
      }
    }

    public AccessMapping DefaultAccessMapping
    {
      get
      {
        this.m_accessLock.EnterReadLock();
        try
        {
          return !this.CacheDataExpired ? this.m_defaultAccessMapping : (AccessMapping) null;
        }
        finally
        {
          this.m_accessLock.ExitReadLock();
        }
      }
    }

    public string WebApplicationRelativeDirectory
    {
      get => this.m_webApplicationRelativeDirectory;
      set => this.m_webApplicationRelativeDirectory = string.IsNullOrEmpty(value) ? this.m_webApplicationRelativeDirectory : value;
    }

    public void ClearIfCacheNotFresh(int serverLastChangeId)
    {
      if (serverLastChangeId == this.m_lastChangeId)
        return;
      this.m_accessLock.EnterWriteLock();
      try
      {
        if (serverLastChangeId == this.m_lastChangeId)
          return;
        this.m_accessMappings.Clear();
        this.m_services.Clear();
        this.m_cachedMisses.Clear();
        this.m_lastChangeId = -1;
        this.m_cacheExpirationDate = DateTime.MinValue;
      }
      finally
      {
        this.m_accessLock.ExitWriteLock();
      }
    }

    public void RemoveServices(IEnumerable<ServiceDefinition> serviceDefinitions, int lastChangeId)
    {
      this.EnsureDiskCacheLoaded();
      this.m_accessLock.EnterWriteLock();
      try
      {
        foreach (ServiceDefinition serviceDefinition in serviceDefinitions)
        {
          Dictionary<Guid, ServiceDefinition> dictionary = (Dictionary<Guid, ServiceDefinition>) null;
          if (this.m_services.TryGetValue(serviceDefinition.ServiceType, out dictionary) && dictionary.Remove(serviceDefinition.Identifier) && dictionary.Count == 0)
            this.m_services.Remove(serviceDefinition.ServiceType);
        }
        this.SetLastChangeId(lastChangeId, false);
        this.WriteCacheToDisk();
      }
      finally
      {
        this.m_accessLock.ExitWriteLock();
      }
    }

    public AccessMapping GetAccessMapping(string moniker)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(moniker, nameof (moniker));
      this.EnsureDiskCacheLoaded();
      this.m_accessLock.EnterReadLock();
      try
      {
        if (this.CacheDataExpired)
          return (AccessMapping) null;
        AccessMapping accessMapping;
        this.m_accessMappings.TryGetValue(moniker, out accessMapping);
        return accessMapping;
      }
      finally
      {
        this.m_accessLock.ExitReadLock();
      }
    }

    public bool TryFindService(
      string serviceType,
      Guid serviceIdentifier,
      out ServiceDefinition serviceDefinition)
    {
      this.EnsureDiskCacheLoaded();
      this.m_accessLock.EnterReadLock();
      try
      {
        Dictionary<Guid, ServiceDefinition> dictionary = (Dictionary<Guid, ServiceDefinition>) null;
        serviceDefinition = (ServiceDefinition) null;
        return !this.CacheDataExpired && (this.m_services.TryGetValue(serviceType, out dictionary) && dictionary.TryGetValue(serviceIdentifier, out serviceDefinition) || this.m_cachedMisses.Contains(LocationCacheManager.BuildCacheMissString(serviceType, serviceIdentifier)));
      }
      finally
      {
        this.m_accessLock.ExitReadLock();
      }
    }

    public IEnumerable<ServiceDefinition> FindServices(string serviceType)
    {
      this.EnsureDiskCacheLoaded();
      this.m_accessLock.EnterReadLock();
      try
      {
        if (this.CacheDataExpired || this.m_services.Count == 0)
          return (IEnumerable<ServiceDefinition>) null;
        IEnumerable<Dictionary<Guid, ServiceDefinition>> dictionaries;
        if (string.IsNullOrEmpty(serviceType))
        {
          dictionaries = (IEnumerable<Dictionary<Guid, ServiceDefinition>>) this.m_services.Values;
        }
        else
        {
          Dictionary<Guid, ServiceDefinition> dictionary = (Dictionary<Guid, ServiceDefinition>) null;
          if (!this.m_services.TryGetValue(serviceType, out dictionary))
            return (IEnumerable<ServiceDefinition>) null;
          dictionaries = (IEnumerable<Dictionary<Guid, ServiceDefinition>>) new Dictionary<Guid, ServiceDefinition>[1]
          {
            dictionary
          };
        }
        List<ServiceDefinition> services = new List<ServiceDefinition>();
        foreach (Dictionary<Guid, ServiceDefinition> dictionary in dictionaries)
        {
          foreach (ServiceDefinition serviceDefinition in dictionary.Values)
            services.Add(serviceDefinition.Clone());
        }
        return (IEnumerable<ServiceDefinition>) services;
      }
      finally
      {
        this.m_accessLock.ExitReadLock();
      }
    }

    public void LoadServicesData(LocationServiceData locationServiceData, bool allServicesIncluded)
    {
      this.m_accessLock.EnterWriteLock();
      try
      {
        if (!locationServiceData.ClientCacheFresh && locationServiceData.LastChangeId != this.m_lastChangeId)
        {
          this.m_accessMappings = new Dictionary<string, AccessMapping>((IEqualityComparer<string>) VssStringComparer.AccessMappingMoniker);
          this.m_services = new Dictionary<string, Dictionary<Guid, ServiceDefinition>>((IEqualityComparer<string>) VssStringComparer.ServiceType);
          this.m_cachedMisses = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.ServiceType);
          this.m_lastChangeId = -1;
          this.m_cacheExpirationDate = DateTime.MinValue;
        }
        else
          this.EnsureDiskCacheLoadedHelper();
        this.SetLastChangeId(locationServiceData.LastChangeId, allServicesIncluded);
        this.m_cacheExpirationDate = DateTime.UtcNow.AddSeconds(this.ClientCacheTimeToLive.HasValue ? (double) this.ClientCacheTimeToLive.Value : (double) locationServiceData.ClientCacheTimeToLive);
        ICollection<AccessMapping> accessMappings = locationServiceData.AccessMappings;
        if (accessMappings != null && accessMappings.Count > 0)
        {
          foreach (AccessMapping accessMapping1 in (IEnumerable<AccessMapping>) accessMappings)
          {
            if (accessMapping1.VirtualDirectory == null && !string.IsNullOrEmpty(this.WebApplicationRelativeDirectory))
            {
              string main = accessMapping1.AccessPoint.TrimEnd('/');
              string pattern = this.WebApplicationRelativeDirectory.TrimEnd('/');
              if (VssStringComparer.ServerUrl.EndsWith(main, pattern))
                accessMapping1.AccessPoint = main.Substring(0, main.Length - pattern.Length);
            }
            AccessMapping accessMapping2;
            if (this.m_accessMappings.TryGetValue(accessMapping1.Moniker, out accessMapping2))
            {
              accessMapping2.DisplayName = accessMapping1.DisplayName;
              accessMapping2.AccessPoint = accessMapping1.AccessPoint;
              accessMapping2.VirtualDirectory = accessMapping1.VirtualDirectory;
            }
            else
            {
              accessMapping2 = accessMapping1;
              this.m_accessMappings[accessMapping1.Moniker] = accessMapping1;
            }
          }
          this.DetermineClientAndDefaultZones(locationServiceData.DefaultAccessMappingMoniker);
        }
        if (locationServiceData.ServiceDefinitions != null)
        {
          foreach (ServiceDefinition serviceDefinition in (IEnumerable<ServiceDefinition>) locationServiceData.ServiceDefinitions)
          {
            Dictionary<Guid, ServiceDefinition> dictionary = (Dictionary<Guid, ServiceDefinition>) null;
            if (!this.m_services.TryGetValue(serviceDefinition.ServiceType, out dictionary))
            {
              dictionary = new Dictionary<Guid, ServiceDefinition>();
              this.m_services[serviceDefinition.ServiceType] = dictionary;
            }
            dictionary[serviceDefinition.Identifier] = serviceDefinition;
          }
        }
        this.m_cacheAvailable = true;
        this.WriteCacheToDisk();
      }
      finally
      {
        this.m_accessLock.ExitWriteLock();
      }
    }

    private void DetermineClientAndDefaultZones(string defaultAccessMappingMoniker)
    {
      this.m_defaultAccessMapping = (AccessMapping) null;
      this.m_clientAccessMapping = (AccessMapping) null;
      string str1;
      if (this.WebApplicationRelativeDirectory == null)
        str1 = string.Empty;
      else
        str1 = this.WebApplicationRelativeDirectory.TrimEnd('/');
      string str2 = str1;
      foreach (AccessMapping accessMapping in this.m_accessMappings.Values)
      {
        if (VssStringComparer.ServerUrl.StartsWith(this.m_connectionBaseUrl.ToString(), accessMapping.AccessPoint.TrimEnd('/')) && (accessMapping.VirtualDirectory == null || VssStringComparer.UrlPath.Equals(accessMapping.VirtualDirectory, str2)))
          this.m_clientAccessMapping = accessMapping;
      }
      this.m_defaultAccessMapping = this.m_accessMappings[defaultAccessMappingMoniker];
      if (this.m_clientAccessMapping != null)
        return;
      string main = this.m_connectionBaseUrl.ToString().TrimEnd('/');
      string str3 = string.Empty;
      if (!string.IsNullOrEmpty(this.WebApplicationRelativeDirectory) && VssStringComparer.ServerUrl.EndsWith(main, str2))
      {
        main = main.Substring(0, main.Length - str2.Length);
        str3 = str2;
      }
      this.m_clientAccessMapping = new AccessMapping()
      {
        Moniker = main,
        DisplayName = main,
        AccessPoint = main,
        VirtualDirectory = str3
      };
    }

    public IEnumerable<AccessMapping> AccessMappings
    {
      get
      {
        this.EnsureDiskCacheLoaded();
        this.m_accessLock.EnterReadLock();
        try
        {
          List<AccessMapping> accessMappings = new List<AccessMapping>();
          if (!this.CacheDataExpired)
          {
            foreach (AccessMapping accessMapping in this.m_accessMappings.Values)
              accessMappings.Add(accessMapping);
          }
          return (IEnumerable<AccessMapping>) accessMappings;
        }
        finally
        {
          this.m_accessLock.ExitReadLock();
        }
      }
    }

    public void RemoveAccessMapping(string moniker)
    {
      this.EnsureDiskCacheLoaded();
      this.m_accessLock.EnterWriteLock();
      try
      {
        this.m_accessMappings.Remove(moniker);
        foreach (Dictionary<Guid, ServiceDefinition> dictionary in this.m_services.Values)
        {
          foreach (ServiceDefinition serviceDefinition in dictionary.Values)
          {
            for (int index = 0; index < serviceDefinition.LocationMappings.Count; ++index)
            {
              if (VssStringComparer.AccessMappingMoniker.Equals(moniker, serviceDefinition.LocationMappings[index].AccessMappingMoniker))
              {
                serviceDefinition.LocationMappings.RemoveAt(index);
                break;
              }
            }
          }
        }
        this.WriteCacheToDisk();
      }
      finally
      {
        this.m_accessLock.ExitWriteLock();
      }
    }

    public void AddCachedMiss(string serviceType, Guid serviceIdentifier, int missedLastChangeId)
    {
      if (missedLastChangeId < 0)
        return;
      this.EnsureDiskCacheLoaded();
      this.m_accessLock.EnterWriteLock();
      try
      {
        if (missedLastChangeId != this.m_lastChangeId || !this.m_cachedMisses.Add(LocationCacheManager.BuildCacheMissString(serviceType, serviceIdentifier)))
          return;
        this.WriteCacheToDisk();
      }
      finally
      {
        this.m_accessLock.ExitWriteLock();
      }
    }

    public int GetLastChangeId()
    {
      this.EnsureDiskCacheLoaded();
      this.m_accessLock.EnterReadLock();
      try
      {
        return this.m_lastChangeId;
      }
      finally
      {
        this.m_accessLock.ExitReadLock();
      }
    }

    internal DateTime GetCacheExpirationDate()
    {
      this.EnsureDiskCacheLoaded();
      this.m_accessLock.EnterReadLock();
      try
      {
        return this.m_cacheExpirationDate;
      }
      finally
      {
        this.m_accessLock.ExitReadLock();
      }
    }

    private void SetLastChangeId(int lastChangeId, bool allServicesUpdated)
    {
      if (!(this.m_lastChangeId != -1 | allServicesUpdated))
        return;
      this.m_lastChangeId = lastChangeId;
    }

    private static string BuildCacheMissString(string serviceType, Guid serviceIdentifier) => serviceType + "_" + serviceIdentifier.ToString();

    internal void EnsureDiskCacheLoaded()
    {
      if (this.m_cacheLocallyFresh || !this.m_cacheAvailable)
        return;
      this.m_accessLock.EnterWriteLock();
      try
      {
        this.EnsureDiskCacheLoadedHelper();
      }
      finally
      {
        this.m_accessLock.ExitWriteLock();
      }
    }

    private void EnsureDiskCacheLoadedHelper()
    {
      FileStream file = (FileStream) null;
      try
      {
        if (this.m_cacheLocallyFresh || !this.m_cacheAvailable)
          return;
        XmlDocument document = XmlUtility.OpenXmlFile(out file, this.m_cacheFilePath, FileShare.Read, false);
        if (document != null)
        {
          this.m_accessMappings = new Dictionary<string, AccessMapping>((IEqualityComparer<string>) VssStringComparer.AccessMappingMoniker);
          this.m_services = new Dictionary<string, Dictionary<Guid, ServiceDefinition>>((IEqualityComparer<string>) VssStringComparer.ServiceType);
          this.m_cachedMisses = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.ServiceType);
          this.m_lastChangeId = this.m_locationXmlOperator.ReadLastChangeId(document);
          this.m_cacheExpirationDate = this.m_locationXmlOperator.ReadCacheExpirationDate(document);
          string defaultAccessMappingMoniker = this.m_locationXmlOperator.ReadDefaultAccessMappingMoniker(document);
          this.m_webApplicationRelativeDirectory = this.m_locationXmlOperator.ReadVirtualDirectory(document);
          List<AccessMapping> accessMappingList = this.m_locationXmlOperator.ReadAccessMappings(document);
          foreach (AccessMapping accessMapping in accessMappingList)
            this.m_accessMappings[accessMapping.Moniker] = accessMapping;
          if (accessMappingList.Count > 0)
          {
            this.DetermineClientAndDefaultZones(defaultAccessMappingMoniker);
            foreach (ServiceDefinition readService in this.m_locationXmlOperator.ReadServices(document, this.m_accessMappings))
            {
              Dictionary<Guid, ServiceDefinition> dictionary;
              if (!this.m_services.TryGetValue(readService.ServiceType, out dictionary))
              {
                dictionary = new Dictionary<Guid, ServiceDefinition>();
                this.m_services.Add(readService.ServiceType, dictionary);
              }
              dictionary[readService.Identifier] = readService;
            }
            foreach (string readCachedMiss in this.m_locationXmlOperator.ReadCachedMisses(document))
              this.m_cachedMisses.Add(readCachedMiss);
          }
          else
          {
            this.m_cacheAvailable = false;
            this.m_lastChangeId = -1;
            return;
          }
        }
        if (this.m_fileSystemWatcher != null)
          return;
        this.m_fileSystemWatcher = new FileSystemWatcher(Path.GetDirectoryName(this.m_cacheFilePath), LocationCacheManager.s_cacheFileName);
        this.m_fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
        this.m_fileSystemWatcher.Changed += new FileSystemEventHandler(this.m_fileSystemWatcher_Changed);
      }
      catch (Exception ex)
      {
        this.m_cacheAvailable = false;
        this.m_lastChangeId = -1;
      }
      finally
      {
        this.m_cacheLocallyFresh = true;
        file?.Dispose();
      }
    }

    private void m_fileSystemWatcher_Changed(object sender, FileSystemEventArgs e) => this.m_cacheLocallyFresh = false;

    private void WriteCacheToDisk()
    {
      if (!this.m_cacheAvailable)
        return;
      try
      {
        using (FileStream outStream = XmlUtility.OpenFile(this.m_cacheFilePath, FileShare.None, true))
        {
          XmlDocument xmlDocument = new XmlDocument();
          XmlNode node = xmlDocument.CreateNode(XmlNodeType.Element, "LocationServiceConfiguration", (string) null);
          xmlDocument.AppendChild(node);
          this.m_locationXmlOperator.WriteLastChangeId(node, this.m_lastChangeId);
          this.m_locationXmlOperator.WriteCacheExpirationDate(node, this.m_cacheExpirationDate);
          this.m_locationXmlOperator.WriteDefaultAccessMappingMoniker(node, this.m_defaultAccessMapping.Moniker);
          this.m_locationXmlOperator.WriteVirtualDirectory(node, this.m_webApplicationRelativeDirectory);
          this.m_locationXmlOperator.WriteAccessMappings(node, (IEnumerable<AccessMapping>) this.m_accessMappings.Values);
          List<ServiceDefinition> serviceDefinitionList = new List<ServiceDefinition>();
          foreach (Dictionary<Guid, ServiceDefinition> dictionary in this.m_services.Values)
            serviceDefinitionList.AddRange((IEnumerable<ServiceDefinition>) dictionary.Values);
          this.m_locationXmlOperator.WriteServices(node, (IEnumerable<ServiceDefinition>) serviceDefinitionList);
          this.m_locationXmlOperator.WriteCachedMisses(node, (IEnumerable<string>) this.m_cachedMisses);
          outStream.SetLength(0L);
          outStream.Position = 0L;
          xmlDocument.Save((Stream) outStream);
        }
      }
      catch (Exception ex)
      {
        this.m_cacheAvailable = false;
      }
    }

    internal int? ClientCacheTimeToLive { get; set; }
  }
}
