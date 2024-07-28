// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.LocationCacheManager
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class LocationCacheManager
  {
    private Dictionary<string, Dictionary<Guid, ServiceDefinition>> m_services;
    private HashSet<string> m_cachedMisses;
    private Dictionary<string, AccessMapping> m_accessMappings;
    private int m_lastChangeId;
    private DateTime m_cacheExpirationDate;
    private ReaderWriterLock m_accessLock;
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

    public LocationCacheManager(Guid serverGuid, Uri connectionBaseUrl)
    {
      this.m_cacheAvailable = !serverGuid.Equals(Guid.Empty);
      this.m_accessLock = new ReaderWriterLock();
      this.m_lastChangeId = -1;
      this.m_cacheExpirationDate = DateTime.MinValue;
      this.ClientCacheTimeToLive = TfsConnection.ClientCacheTimeToLive;
      this.m_cacheFilePath = Path.Combine(Path.Combine(TfsConnection.ClientCacheDirectory, serverGuid.ToString()), LocationCacheManager.s_cacheFileName);
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
        try
        {
          this.m_accessLock.AcquireReaderLock(-1);
          return this.m_clientAccessMapping;
        }
        finally
        {
          if (this.m_accessLock.IsReaderLockHeld || this.m_accessLock.IsWriterLockHeld)
            this.m_accessLock.ReleaseReaderLock();
        }
      }
    }

    public AccessMapping DefaultAccessMapping
    {
      get
      {
        try
        {
          this.m_accessLock.AcquireReaderLock(-1);
          return this.m_defaultAccessMapping;
        }
        finally
        {
          if (this.m_accessLock.IsReaderLockHeld || this.m_accessLock.IsWriterLockHeld)
            this.m_accessLock.ReleaseReaderLock();
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
      try
      {
        this.m_accessLock.AcquireWriterLock(-1);
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
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }

    public void RemoveServices(IEnumerable<ServiceDefinition> serviceDefinitions, int lastChangeId)
    {
      try
      {
        this.EnsureDiskCacheLoaded();
        this.m_accessLock.AcquireWriterLock(-1);
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
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }

    public AccessMapping GetAccessMapping(string moniker)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(moniker, nameof (moniker));
      try
      {
        this.EnsureDiskCacheLoaded();
        this.m_accessLock.AcquireReaderLock(-1);
        if (this.CacheDataExpired)
          return (AccessMapping) null;
        AccessMapping accessMapping;
        this.m_accessMappings.TryGetValue(moniker, out accessMapping);
        return accessMapping;
      }
      finally
      {
        if (this.m_accessLock.IsReaderLockHeld)
          this.m_accessLock.ReleaseReaderLock();
      }
    }

    public bool TryFindService(
      string serviceType,
      Guid serviceIdentifier,
      bool ignoreCacheExpiration,
      out ServiceDefinition serviceDefinition)
    {
      try
      {
        this.EnsureDiskCacheLoaded();
        this.m_accessLock.AcquireReaderLock(-1);
        Dictionary<Guid, ServiceDefinition> dictionary = (Dictionary<Guid, ServiceDefinition>) null;
        serviceDefinition = (ServiceDefinition) null;
        return (ignoreCacheExpiration || !this.CacheDataExpired) && (this.m_services.TryGetValue(serviceType, out dictionary) && dictionary.TryGetValue(serviceIdentifier, out serviceDefinition) || this.m_cachedMisses.Contains(this.BuildCacheMissString(serviceType, serviceIdentifier)));
      }
      finally
      {
        if (this.m_accessLock.IsReaderLockHeld)
          this.m_accessLock.ReleaseReaderLock();
      }
    }

    public IEnumerable<ServiceDefinition> FindServices(string serviceType)
    {
      try
      {
        this.EnsureDiskCacheLoaded();
        this.m_accessLock.AcquireReaderLock(-1);
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
        if (this.m_accessLock.IsReaderLockHeld)
          this.m_accessLock.ReleaseReaderLock();
      }
    }

    public IEnumerable<ServiceDefinition> FindServicesByToolId(string toolId)
    {
      try
      {
        this.EnsureDiskCacheLoaded();
        this.m_accessLock.AcquireReaderLock(-1);
        if (this.CacheDataExpired)
          return (IEnumerable<ServiceDefinition>) null;
        bool flag = string.IsNullOrEmpty(toolId);
        List<ServiceDefinition> serviceDefinitionList = new List<ServiceDefinition>();
        foreach (Dictionary<Guid, ServiceDefinition> dictionary in this.m_services.Values)
        {
          foreach (ServiceDefinition serviceDefinition in dictionary.Values)
          {
            if (flag || VssStringComparer.ToolId.Equals(serviceDefinition.ToolType, toolId))
              serviceDefinitionList.Add(serviceDefinition.Clone());
          }
        }
        return serviceDefinitionList.Count == 0 ? (IEnumerable<ServiceDefinition>) null : (IEnumerable<ServiceDefinition>) serviceDefinitionList;
      }
      finally
      {
        if (this.m_accessLock.IsReaderLockHeld)
          this.m_accessLock.ReleaseReaderLock();
      }
    }

    public void LoadServicesData(LocationServiceData locationServiceData, bool allServicesIncluded)
    {
      try
      {
        this.m_accessLock.AcquireWriterLock(-1);
        if (!locationServiceData.ClientCacheFresh && locationServiceData.LastChangeId != this.m_lastChangeId)
        {
          this.m_accessMappings = new Dictionary<string, AccessMapping>((IEqualityComparer<string>) VssStringComparer.AccessMappingMoniker);
          this.m_services = new Dictionary<string, Dictionary<Guid, ServiceDefinition>>((IEqualityComparer<string>) VssStringComparer.ServiceType);
          this.m_cachedMisses = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.ServiceType);
          this.m_lastChangeId = -1;
          this.m_cacheExpirationDate = DateTime.MinValue;
        }
        else
          this.EnsureDiskCacheLoaded();
        this.SetLastChangeId(locationServiceData.LastChangeId, allServicesIncluded);
        int? clientCacheTimeToLive1 = this.ClientCacheTimeToLive;
        int clientCacheTimeToLive2;
        if (!clientCacheTimeToLive1.HasValue)
        {
          clientCacheTimeToLive2 = locationServiceData.ClientCacheTimeToLive;
        }
        else
        {
          clientCacheTimeToLive1 = this.ClientCacheTimeToLive;
          clientCacheTimeToLive2 = clientCacheTimeToLive1.Value;
        }
        this.m_cacheExpirationDate = DateTime.UtcNow.AddSeconds((double) clientCacheTimeToLive2);
        AccessMapping[] accessMappings = locationServiceData.AccessMappings;
        if (accessMappings != null && accessMappings.Length != 0)
        {
          foreach (AccessMapping accessMapping1 in accessMappings)
          {
            if (accessMapping1.VirtualDirectory == null && !locationServiceData.AccessPointsDoNotIncludeWebAppRelativeDirectory && !string.IsNullOrEmpty(this.WebApplicationRelativeDirectory))
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
        this.m_connectionBaseUrl.GetLeftPart(UriPartial.Authority);
        string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}://{1}", (object) this.m_connectionBaseUrl.Scheme, (object) this.m_connectionBaseUrl.Host);
        if (locationServiceData.ServiceDefinitions != null)
        {
          foreach (ServiceDefinition serviceDefinition in locationServiceData.ServiceDefinitions)
          {
            serviceDefinition.ReactToWebServiceDeserialization(this.m_accessMappings);
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
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
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
      string str3 = this.m_connectionBaseUrl.ToString().TrimEnd('/');
      string virtualDirectory = string.Empty;
      if (!string.IsNullOrEmpty(this.WebApplicationRelativeDirectory) && VssStringComparer.ServerUrl.EndsWith(str3, str2))
      {
        str3 = str3.Substring(0, str3.Length - str2.Length);
        virtualDirectory = str2;
      }
      this.m_clientAccessMapping = new AccessMapping(str3, str3, str3, virtualDirectory);
    }

    public IEnumerable<AccessMapping> AccessMappings
    {
      get
      {
        try
        {
          this.EnsureDiskCacheLoaded();
          this.m_accessLock.AcquireReaderLock(-1);
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
          if (this.m_accessLock.IsReaderLockHeld)
            this.m_accessLock.ReleaseReaderLock();
        }
      }
    }

    public void RemoveAccessMapping(string moniker)
    {
      try
      {
        this.EnsureDiskCacheLoaded();
        this.m_accessLock.AcquireWriterLock(-1);
        this.m_accessMappings.Remove(moniker);
        foreach (Dictionary<Guid, ServiceDefinition> dictionary in this.m_services.Values)
        {
          foreach (ServiceDefinition serviceDefinition in dictionary.Values)
          {
            for (int index = 0; index < serviceDefinition.Mappings.Count; ++index)
            {
              if (VssStringComparer.AccessMappingMoniker.Equals(moniker, serviceDefinition.Mappings[index].AccessMappingMoniker))
              {
                serviceDefinition.Mappings.RemoveAt(index);
                break;
              }
            }
          }
        }
        this.WriteCacheToDisk();
      }
      finally
      {
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }

    public void AddCachedMiss(string serviceType, Guid serviceIdentifier, int missedLastChangeId)
    {
      if (missedLastChangeId < 0)
        return;
      this.EnsureDiskCacheLoaded();
      try
      {
        this.m_accessLock.AcquireWriterLock(-1);
        if (missedLastChangeId != this.m_lastChangeId || !this.m_cachedMisses.Add(this.BuildCacheMissString(serviceType, serviceIdentifier)))
          return;
        this.WriteCacheToDisk();
      }
      finally
      {
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }

    public int GetLastChangeId()
    {
      this.EnsureDiskCacheLoaded();
      try
      {
        this.m_accessLock.AcquireReaderLock(-1);
        return this.m_lastChangeId;
      }
      finally
      {
        if (this.m_accessLock.IsReaderLockHeld)
          this.m_accessLock.ReleaseReaderLock();
      }
    }

    internal DateTime GetCacheExpirationDate()
    {
      this.EnsureDiskCacheLoaded();
      try
      {
        this.m_accessLock.AcquireReaderLock(-1);
        return this.m_cacheExpirationDate;
      }
      finally
      {
        if (this.m_accessLock.IsReaderLockHeld)
          this.m_accessLock.ReleaseReaderLock();
      }
    }

    private void SetLastChangeId(int lastChangeId, bool allServicesUpdated)
    {
      if (!(this.m_lastChangeId != -1 | allServicesUpdated))
        return;
      this.m_lastChangeId = lastChangeId;
    }

    private string BuildCacheMissString(string serviceType, Guid serviceIdentifier) => serviceType + "_" + serviceIdentifier.ToString();

    internal void EnsureDiskCacheLoaded()
    {
      if (this.m_cacheLocallyFresh || !this.m_cacheAvailable)
        return;
      FileStream file = (FileStream) null;
      try
      {
        this.m_accessLock.AcquireWriterLock(-1);
        if (this.m_cacheLocallyFresh)
          return;
        XmlDocument document = TFCommonUtil.OpenXmlFile(out file, this.m_cacheFilePath, FileShare.None, true);
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
            TeamFoundationTrace.Warning("LocationServerMap.xml did not contain any access mappings");
            return;
          }
        }
        if (this.m_fileSystemWatcher != null)
          return;
        this.m_fileSystemWatcher = new FileSystemWatcher(FileSpec.GetDirectoryName(this.m_cacheFilePath), LocationCacheManager.s_cacheFileName);
        this.m_fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
        this.m_fileSystemWatcher.Changed += new FileSystemEventHandler(this.m_fileSystemWatcher_Changed);
      }
      catch (Exception ex)
      {
        this.m_cacheAvailable = false;
        this.m_lastChangeId = -1;
        TeamFoundationTrace.Warning("Unable to read LocationServerMap.xml because: '{0}'", (object) ex.Message);
      }
      finally
      {
        this.m_cacheLocallyFresh = true;
        file?.Close();
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }

    private void m_fileSystemWatcher_Changed(object sender, FileSystemEventArgs e) => this.m_cacheLocallyFresh = false;

    public void WriteCacheToDisk()
    {
      if (!this.m_cacheAvailable)
        return;
      try
      {
        using (FileStream outStream = TFCommonUtil.OpenFile(this.m_cacheFilePath, FileShare.None, true))
        {
          XmlDocument xmlDocument = new XmlDocument();
          XmlNode node = xmlDocument.CreateNode(XmlNodeType.Element, "LocationServiceConfiguration", (string) null);
          xmlDocument.AppendChild(node);
          this.m_locationXmlOperator.WriteLastChangeId(node, this.m_lastChangeId);
          this.m_locationXmlOperator.WriteCacheExpirationDate(node, this.m_cacheExpirationDate);
          this.m_locationXmlOperator.WriteDefaultAccessMappingMoniker(node, this.m_defaultAccessMapping.Moniker);
          this.m_locationXmlOperator.WriteVirtualDirectory(node, this.m_webApplicationRelativeDirectory);
          this.m_locationXmlOperator.WriteAccessMappings(node, (IEnumerable<AccessMapping>) this.m_accessMappings.Values);
          List<ServiceDefinition> serviceDefintions = new List<ServiceDefinition>();
          foreach (Dictionary<Guid, ServiceDefinition> dictionary in this.m_services.Values)
            serviceDefintions.AddRange((IEnumerable<ServiceDefinition>) dictionary.Values);
          this.m_locationXmlOperator.WriteServices(node, (IEnumerable<ServiceDefinition>) serviceDefintions);
          this.m_locationXmlOperator.WriteCachedMisses(node, (IEnumerable<string>) this.m_cachedMisses);
          outStream.SetLength(0L);
          outStream.Position = 0L;
          xmlDocument.Save((Stream) outStream);
        }
      }
      catch (Exception ex)
      {
        this.m_cacheAvailable = false;
        TeamFoundationTrace.Warning("Unable to read LocationServerMap.xml because: '{0}'", (object) ex.Message);
      }
    }

    internal int? ClientCacheTimeToLive { get; set; }
  }
}
