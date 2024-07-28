// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ProxyConfiguration
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ProxyConfiguration : IProxyConfigurationInternal, IProxyConfiguration, IDisposable
  {
    private const string c_area = "FileCacheService";
    private const string c_layer = "ProxyConfiguration";
    private IProxyRegistration m_registrationPlugin;
    private ReaderWriterLock m_configLock = new ReaderWriterLock();
    private IVssDeploymentServiceHost m_deploymentServiceHost;
    private Dictionary<string, string> m_repositories = new Dictionary<string, string>((IEqualityComparer<string>) VssStringComparer.Guid);
    private bool m_remoteConfiguration;
    private List<Uri> m_serverUris = new List<Uri>();
    private const string c_registrySettingsPath = "/Service/VersionControl/Settings/";
    private const string c_registrationPlugin = "Microsoft.TeamFoundation.Client.PlugIns.Core.Proxy.ResolveCollection, Microsoft.TeamFoundation.Client.PlugIns.Core";
    private const int c_defaultCacheDirectoryDepthLimit = 16;

    internal ProxyConfiguration(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      this.m_deploymentServiceHost = requestContext.ServiceHost.DeploymentServiceHost;
      this.LoadRegistrationPlugin(requestContext);
      this.LoadConfiguration();
    }

    private void LoadRegistrationPlugin(IVssRequestContext requestContext)
    {
      try
      {
        IProxyRegistration extension = requestContext.GetExtension<IProxyRegistration>((Func<IProxyRegistration, bool>) (x => x.GetType().AssemblyQualifiedName.StartsWith("Microsoft.TeamFoundation.Client.PlugIns.Core.Proxy.ResolveCollection, Microsoft.TeamFoundation.Client.PlugIns.Core", StringComparison.OrdinalIgnoreCase)));
        if (extension != null)
        {
          string assemblyQualifiedName = extension.GetType().AssemblyQualifiedName;
          requestContext.Trace(13380, TraceLevel.Info, "FileCacheService", nameof (ProxyConfiguration), "Found plugin {0}", (object) assemblyQualifiedName);
          this.m_registrationPlugin = extension;
        }
        else
          requestContext.Trace(13382, TraceLevel.Info, "FileCacheService", nameof (ProxyConfiguration), "Found no plugins implementing IProxyRegistration");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(13384, "FileCacheService", nameof (ProxyConfiguration), ex);
      }
    }

    public void Dispose()
    {
      this.m_deploymentServiceHost = (IVssDeploymentServiceHost) null;
      GC.SuppressFinalize((object) this);
    }

    public bool IsRemoteConfiguration => this.m_remoteConfiguration;

    public bool IsValid => this.CacheRoot != null;

    public string CacheRoot { get; private set; }

    public int CacheDirectoryDepthLimit { get; private set; }

    public int VCCacheEnabledState { get; private set; }

    public int GitCacheEnabledState { get; private set; }

    public int CacheLimit { get; private set; }

    public double CacheLimitPercent { get; private set; }

    public int CacheDeletionPercent { get; private set; }

    public int ReaderChunkSize { get; private set; }

    public int WriterChunkSize { get; private set; }

    public int StatisticsPersistTime { get; private set; }

    public int ApplicationTierReadTimeout { get; private set; }

    public int DeletionAgeThreshold { get; private set; }

    internal ICollection<Uri> ServerUris => (ICollection<Uri>) this.m_serverUris;

    internal int Crc { get; private set; }

    string IProxyConfigurationInternal.GetRepositoryLocation(string serverId)
    {
      try
      {
        this.m_configLock.AcquireReaderLock(-1);
        string repositoryLocation;
        if (this.m_repositories.TryGetValue(serverId, out repositoryLocation))
          return repositoryLocation;
      }
      finally
      {
        if (this.m_configLock.IsReaderLockHeld)
          this.m_configLock.ReleaseReaderLock();
      }
      if (!this.IsRemoteConfiguration)
      {
        try
        {
          using (IVssRequestContext systemContext = this.m_deploymentServiceHost.CreateSystemContext())
          {
            TeamFoundationServiceHostProperties serviceHostProperties = systemContext.GetService<TeamFoundationHostManagementService>().QueryServiceHostProperties(systemContext, new Guid(serverId));
            if (serviceHostProperties != null)
            {
              string repositoryLocation = !serviceHostProperties.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment) ? "~/" + serviceHostProperties.Name + "/" : (string) null;
              try
              {
                this.m_configLock.AcquireWriterLock(-1);
                this.m_repositories[serverId] = repositoryLocation;
              }
              finally
              {
                if (this.m_configLock.IsWriterLockHeld)
                  this.m_configLock.ReleaseWriterLock();
              }
              return repositoryLocation;
            }
          }
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(13386, "FileCacheService", nameof (ProxyConfiguration), ex);
        }
      }
      return string.Empty;
    }

    public void RegisterRepository(string respositoryId, string serverLocation)
    {
      try
      {
        this.m_configLock.AcquireReaderLock((int) TimeSpan.FromMinutes(5.0).TotalMilliseconds);
        if (this.m_repositories.ContainsKey(respositoryId))
          return;
      }
      finally
      {
        if (this.m_configLock.IsReaderLockHeld)
          this.m_configLock.ReleaseReaderLock();
      }
      try
      {
        this.m_configLock.AcquireWriterLock((int) TimeSpan.FromMinutes(5.0).TotalMilliseconds);
        this.m_repositories[respositoryId] = serverLocation;
      }
      finally
      {
        if (this.m_configLock.IsWriterLockHeld)
          this.m_configLock.ReleaseWriterLock();
      }
    }

    public void UnRegisterRepository(string respositoryId)
    {
      try
      {
        this.m_configLock.AcquireReaderLock((int) TimeSpan.FromMinutes(5.0).TotalMilliseconds);
        if (!this.m_repositories.ContainsKey(respositoryId))
          return;
      }
      finally
      {
        if (this.m_configLock.IsReaderLockHeld)
          this.m_configLock.ReleaseReaderLock();
      }
      try
      {
        this.m_configLock.AcquireWriterLock((int) TimeSpan.FromMinutes(5.0).TotalMilliseconds);
        this.m_repositories.Remove(respositoryId);
      }
      finally
      {
        if (this.m_configLock.IsWriterLockHeld)
          this.m_configLock.ReleaseWriterLock();
      }
    }

    private void LoadConfiguration()
    {
      using (IVssRequestContext systemContext = this.m_deploymentServiceHost.CreateSystemContext())
      {
        if (object.Equals((object) systemContext.ServiceHost.InstanceId, (object) ProxyApplicationSettings.ProxyDeploymentId))
        {
          try
          {
            FileInfo fileInfo = new FileInfo(Path.Combine(systemContext.ServiceHost.PhysicalDirectory, "VersionControlProxy\\proxy.config"));
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            using (Stream input = (Stream) fileInfo.OpenRead())
            {
              using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("ProxyConfig.xsd"))
              {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Prohibit;
                settings.XmlResolver = (XmlResolver) null;
                XPathNavigator navigator1 = new XPathDocument(XmlReader.Create(input, settings)).CreateNavigator();
                XmlSchema schema = XmlSchema.Read(XmlReader.Create(manifestResourceStream, settings), (ValidationEventHandler) null);
                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add(schema);
                navigator1.CheckValidity(schemas, new ValidationEventHandler(this.ValidationHandler));
                string str;
                if (!ConfigUtil.TryReadNode(navigator1, ProxyConfiguration.RootQuery("CacheRoot"), out str))
                  throw new ProxyException(FrameworkResources.InvalidCacheRoot());
                this.CacheRoot = str;
                this.CacheDirectoryDepthLimit = (int) ConfigUtil.ReadLong(navigator1, ProxyConfiguration.RootQuery("CacheDirectoryDepthLimit"), 16L, 0L, (long) int.MaxValue);
                this.VCCacheEnabledState = 2;
                this.GitCacheEnabledState = 2;
                string node1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) "CacheLimitPolicy", (object) "FixedSizeBasedPolicy");
                this.CacheLimit = (int) ConfigUtil.ReadLong(navigator1, ProxyConfiguration.RootQuery(node1), 0L, 0L, (long) int.MaxValue);
                this.StatisticsPersistTime = (int) ConfigUtil.ReadLong(navigator1, ProxyConfiguration.RootQuery("StatisticsPersistTime"), 1L, 1L, 24L);
                this.DeletionAgeThreshold = (int) ConfigUtil.ReadLong(navigator1, ProxyConfiguration.RootQuery("DeletionAgeThreshold"), 0L, 1L, (long) int.MaxValue);
                string node2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) "CacheLimitPolicy", (object) "PercentageBasedPolicy");
                this.CacheLimitPercent = ConfigUtil.ReadDouble(navigator1, ProxyConfiguration.RootQuery(node2), 75.0, 0.001, 100.0);
                this.CacheDeletionPercent = (int) ConfigUtil.ReadLong(navigator1, ProxyConfiguration.RootQuery("CacheDeletionPercent"), 10L, 1L, 100L);
                this.ReaderChunkSize = (int) ConfigUtil.ReadLong(navigator1, ProxyConfiguration.RootQuery("ReaderChunkSize"), 65536L, 4096L, 1048576L);
                this.WriterChunkSize = (int) ConfigUtil.ReadLong(navigator1, ProxyConfiguration.RootQuery("WriterChunkSize"), 65536L, 4096L, 1048576L);
                this.Crc = (int) ConfigUtil.ReadLong(navigator1, ProxyConfiguration.RootQuery("Crc"), 100L, 0L, 100L);
                this.ApplicationTierReadTimeout = (int) ConfigUtil.ReadLong(navigator1, ProxyConfiguration.RootQuery("ApplicationTierReadTimeout"), 60000L, 5000L, 86400000L);
                string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/{1}/{2}", (object) nameof (ProxyConfiguration), (object) "Servers", (object) "Server");
                XPathExpression expr = navigator1.Compile(xpath);
                XPathNodeIterator xpathNodeIterator = navigator1.Select(expr);
                if (xpathNodeIterator != null)
                {
                  foreach (XPathNavigator navigator2 in xpathNodeIterator)
                  {
                    string uriString;
                    if (!ConfigUtil.TryReadNode(navigator2, "Uri", out uriString))
                      throw new ProxyException(FrameworkResources.ConfigMissingServerUriNode());
                    Uri result;
                    if (!Uri.TryCreate(uriString, UriKind.Absolute, out result))
                      throw new ProxyException(FrameworkResources.ConfigInvalidUriFormat((object) uriString));
                    this.m_serverUris.Add(result);
                    if (this.m_registrationPlugin != null)
                    {
                      try
                      {
                        string collectionId = this.m_registrationPlugin.GetCollectionId(result);
                        if (!string.IsNullOrEmpty(collectionId))
                          this.m_repositories[collectionId] = result.ToString();
                      }
                      catch (Exception ex)
                      {
                        systemContext.TraceException(13207, "FileCacheService", nameof (ProxyConfiguration), ex);
                        string message = FrameworkResources.ErrorLoadingCollection((object) result.ToString(), (object) ex.Message);
                        TeamFoundationEventLog.Default.LogException(message, ex, TeamFoundationEventId.DefaultExceptionEventId, EventLogEntryType.Error);
                      }
                    }
                  }
                }
                else
                  TeamFoundationEventLog.Default.Log(FrameworkResources.ConfigFileHasNoServers(), TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
              }
            }
            this.m_remoteConfiguration = true;
          }
          catch (Exception ex)
          {
            systemContext.TraceException(13208, "FileCacheService", nameof (ProxyConfiguration), ex);
            TeamFoundationEventLog.Default.LogException(FrameworkResources.ErrorProcessingProxyConfig(), ex, TeamFoundationEventId.ConfigurationException, EventLogEntryType.Error);
            throw;
          }
        }
        else
        {
          RegistryEntryCollection registryEntryCollection = systemContext.GetService<ICachedRegistryService>().ReadEntries(systemContext, (RegistryQuery) "/Service/VersionControl/Settings/*");
          this.VCCacheEnabledState = registryEntryCollection.GetValueFromPath<int>("/Service/VersionControl/Settings/CacheEnabledState", 2);
          this.VCCacheEnabledState = registryEntryCollection.GetValueFromPath<int>("/Service/VersionControl/Settings/VCCacheEnabledState", this.VCCacheEnabledState);
          this.GitCacheEnabledState = registryEntryCollection.GetValueFromPath<int>("/Service/VersionControl/Settings/GitCacheEnabledState", 2);
          this.CacheDirectoryDepthLimit = registryEntryCollection.GetValueFromPath<int>("/Service/VersionControl/Settings/CacheDirectoryDepthLimit", 16);
          this.CacheLimit = registryEntryCollection.GetValueFromPath<int>("/Service/VersionControl/Settings/FixedSizeBasedPolicy", 0);
          this.CacheLimitPercent = registryEntryCollection.GetValueFromPath<double>("/Service/VersionControl/Settings/PercentageBasedPolicy", 0.0);
          this.CacheDeletionPercent = registryEntryCollection.GetValueFromPath<int>("/Service/VersionControl/Settings/CacheDeletionPercent", 0);
          this.StatisticsPersistTime = registryEntryCollection.GetValueFromPath<int>("/Service/VersionControl/Settings/StatisticsPersistTime", 0);
          this.DeletionAgeThreshold = registryEntryCollection.GetValueFromPath<int>("/Service/VersionControl/Settings/DeletionAgeThreshold", 0);
          this.Crc = registryEntryCollection.GetValueFromPath<int>("/Service/VersionControl/Settings/Crc", 100);
          this.Crc = Math.Max(Math.Min(100, this.Crc), 0);
          int result1;
          if ((int.TryParse(ConfigurationManager.AppSettings["VCCacheEnabledState"], out result1) || int.TryParse(ConfigurationManager.AppSettings["CacheEnabledState"], out result1)) && result1 >= 0 && result1 <= 2)
            this.VCCacheEnabledState = result1;
          int result2;
          if (int.TryParse(ConfigurationManager.AppSettings["GitCacheEnabledState"], out result2) && result2 >= 0 && result2 <= 2)
            this.GitCacheEnabledState = result2;
          int result3;
          if (int.TryParse(ConfigurationManager.AppSettings["CacheDirectoryDepthLimit"], out result3))
            this.CacheDirectoryDepthLimit = result3;
          int result4;
          if (int.TryParse(ConfigurationManager.AppSettings["FixedSizeBasedPolicy"], out result4))
            this.CacheLimit = result4;
          if (int.TryParse(ConfigurationManager.AppSettings["CacheDeletionPercent"], out result4))
            this.CacheDeletionPercent = result4;
          if (int.TryParse(ConfigurationManager.AppSettings["StatisticsPersistTime"], out result4))
            this.StatisticsPersistTime = result4;
          if (int.TryParse(ConfigurationManager.AppSettings["DeletionAgeThreshold"], out result4))
            this.DeletionAgeThreshold = result4;
          double result5;
          if (double.TryParse(ConfigurationManager.AppSettings["PercentageBasedPolicy"], out result5))
            this.CacheLimitPercent = result5;
          this.CacheRoot = ConfigurationManager.AppSettings["CacheRoot"];
          this.m_remoteConfiguration = false;
        }
        if (this.CacheDirectoryDepthLimit < 2)
          this.CacheDirectoryDepthLimit = 2;
        if (this.CacheLimit == 0 && this.CacheLimitPercent == 0.0)
          this.CacheLimitPercent = 75.0;
        if (this.CacheLimit != 0 && this.CacheLimitPercent != 0.0)
          this.CacheLimitPercent = 0.0;
        if (this.CacheDeletionPercent == 0)
          this.CacheDeletionPercent = 20;
        if (this.StatisticsPersistTime < 1)
          this.StatisticsPersistTime = 1;
        if (this.DeletionAgeThreshold <= 0)
          this.DeletionAgeThreshold = 30;
        if (this.DeletionAgeThreshold > 20000)
          this.DeletionAgeThreshold = 20000;
        if (string.IsNullOrEmpty(this.CacheRoot))
        {
          this.CacheRoot = systemContext.ServiceHost.DeploymentServiceHost.ServiceHostInternal().DataDirectory;
          if (systemContext.ExecutionEnvironment.IsCloudDeployment)
            systemContext.Trace(13215, TraceLevel.Error, "FileCacheService", nameof (ProxyConfiguration), "The CacheRoot shouldn't be null. Falling back to " + this.CacheRoot + ".");
        }
        try
        {
          if (Path.IsPathRooted(this.CacheRoot))
          {
            new DirectoryInfo(this.CacheRoot).Create();
            this.CacheRoot = Path.Combine(this.CacheRoot, "Proxy");
            new DirectoryInfo(this.CacheRoot).Create();
          }
          else
          {
            systemContext.Trace(13211, TraceLevel.Error, "FileCacheService", nameof (ProxyConfiguration), "FileCacheRoot {0} is not a rooted path", (object) this.CacheRoot);
            TeamFoundationEventLog.Default.Log(FrameworkResources.FileCacheRootNotAbsolute(), TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
            this.CacheRoot = (string) null;
          }
        }
        catch (Exception ex)
        {
          systemContext.TraceException(13213, "FileCacheService", nameof (ProxyConfiguration), ex);
          TeamFoundationEventLog.Default.LogException(FrameworkResources.InvalidCacheRoot(), ex);
          this.CacheRoot = (string) null;
        }
      }
    }

    private static string RootQuery(string node) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/{1}", (object) nameof (ProxyConfiguration), (object) node);

    private void ValidationHandler(object sender, ValidationEventArgs args)
    {
      if (args.Severity == XmlSeverityType.Error)
        throw args.Exception;
    }
  }
}
