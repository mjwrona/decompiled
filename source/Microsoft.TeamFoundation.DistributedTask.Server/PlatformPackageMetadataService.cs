// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.PlatformPackageMetadataService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class PlatformPackageMetadataService : 
    IDistributedTaskPackageMetadataService,
    IVssFrameworkService,
    IPackageMetadataService
  {
    private static string s_offlineAgentsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), TaskAgentConstants.OfflineAgentsDirectory);
    private ConcurrentDictionary<Tuple<string, string>, PackageVersion> m_latestPackageVersions;
    private ConcurrentDictionary<string, Dictionary<string, string>> m_latestPackageDownloadUrls;
    private const int DefaultTop = 10;
    private const string ProviderKey = "Provider";
    private const string c_windowsPlatform = "Microsoft Windows";
    private const string c_linuxPlatform = "Linux";
    private const string c_darwinPlatform = "Darwin";
    private const string c_agentPackagePrefix = "vsts-agent-";
    private const string c_pipelinesAgentPackagePrefix = "pipelines-agent-";
    private static PackageVersion s_netcore20AgentCutOffVersion = new PackageVersion("2.125.0");

    public PackageMetadata GetPackage(
      IVssRequestContext requestContext,
      string packageType,
      string platform,
      string version)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(packageType, nameof (packageType), "DistributedTask");
      ArgumentUtility.CheckStringForNullOrEmpty(platform, nameof (platform), "DistributedTask");
      ArgumentUtility.CheckStringForNullOrEmpty(version, nameof (version), "DistributedTask");
      PackageVersion version1 = new PackageVersion(version);
      using (requestContext.TraceScope("PackageService", nameof (GetPackage)))
      {
        PackageData packageData = (PackageData) null;
        if (string.Equals(packageType, TaskAgentConstants.AgentPackageType, StringComparison.OrdinalIgnoreCase) && version1.CompareTo(PlatformPackageMetadataService.s_netcore20AgentCutOffVersion) >= 0)
        {
          if (string.Compare(platform, TaskAgentConstants.CoreV1WindowsPlatformName, StringComparison.OrdinalIgnoreCase) == 0)
            platform = TaskAgentConstants.CoreV2WindowsPlatformName;
          else if (string.Compare(platform, TaskAgentConstants.CoreV1Rhel7PlatformName, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(platform, TaskAgentConstants.CoreV1Ubuntu14PlatformName, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(platform, TaskAgentConstants.CoreV1Ubuntu16PlatformName, StringComparison.OrdinalIgnoreCase) == 0)
            platform = TaskAgentConstants.CoreV2LinuxPlatformName;
          else if (string.Compare(platform, TaskAgentConstants.CoreV1OSXPlatformName, StringComparison.OrdinalIgnoreCase) == 0)
            platform = TaskAgentConstants.CoreV2OSXPlatformName;
        }
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IList<PackageMetadata> localPackages = this.GetLocalPackages(vssRequestContext, packageType, platform, version1);
        PackageMetadata package = localPackages != null ? localPackages.FirstOrDefault<PackageMetadata>() : (PackageMetadata) null;
        if (package != null)
          return package;
        using (PackageMetadataComponent component = vssRequestContext.CreateComponent<PackageMetadataComponent>())
          packageData = component.GetPackage(packageType, platform, version1);
        return this.GetPackage(requestContext, packageData);
      }
    }

    public IList<PackageMetadata> GetPackages(
      IVssRequestContext requestContext,
      string packageType,
      string platform = null,
      int? top = null)
    {
      return this.GetPackagesInternal(requestContext, packageType, platform, top, true);
    }

    private IList<PackageMetadata> GetPackagesInternal(
      IVssRequestContext requestContext,
      string packageType,
      string platform = null,
      int? top = null,
      bool autoRedirectLegacyPlatform = false)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(packageType, nameof (packageType), "DistributedTask");
      using (requestContext.TraceScope("PackageService", nameof (GetPackagesInternal)))
      {
        List<PackageData> source1 = new List<PackageData>();
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        string platform1 = string.Empty;
        if (autoRedirectLegacyPlatform)
        {
          switch (platform?.ToLowerInvariant())
          {
            case "win7-x64":
              platform1 = "win-x64";
              break;
            case "osx.10.11-x64":
              platform1 = "osx-x64";
              break;
            case "rhel.7.2-x64":
            case "ubuntu.14.04-x64":
            case "ubuntu.16.04-x64":
              platform1 = "linux-x64";
              break;
          }
        }
        if (!string.IsNullOrEmpty(platform1))
        {
          List<PackageData> collection1 = (List<PackageData>) null;
          List<PackageData> collection2 = (List<PackageData>) null;
          using (PackageMetadataComponent component = vssRequestContext.CreateComponent<PackageMetadataComponent>())
            collection1 = component.GetPackages(packageType, platform1, top ?? 10);
          if (collection1.Count < (top ?? 10))
          {
            using (PackageMetadataComponent component = vssRequestContext.CreateComponent<PackageMetadataComponent>())
              collection2 = component.GetPackages(packageType, platform, (top ?? 10) - collection1.Count);
          }
          if (collection1 != null)
            source1.AddRange((IEnumerable<PackageData>) collection1);
          if (collection2 != null)
            source1.AddRange((IEnumerable<PackageData>) collection2);
        }
        else
        {
          using (PackageMetadataComponent component = vssRequestContext.CreateComponent<PackageMetadataComponent>())
            source1 = component.GetPackages(packageType, platform, top ?? 10);
        }
        List<PackageMetadata> list = source1.Select<PackageData, PackageMetadata>((Func<PackageData, PackageMetadata>) (p => this.GetPackage(requestContext, p))).ToList<PackageMetadata>();
        IList<PackageMetadata> other = string.IsNullOrEmpty(platform1) ? this.GetLocalPackages(vssRequestContext, packageType, platform) : this.GetLocalPackages(vssRequestContext, packageType, platform1);
        if (other == null || other.Count <= 0)
          return (IList<PackageMetadata>) list;
        HashSet<PackageMetadata> source2 = new HashSet<PackageMetadata>((IEqualityComparer<PackageMetadata>) new PackageMetadataByVersionComparer());
        List<PackageMetadata> packagesInternal = new List<PackageMetadata>();
        source2.UnionWith((IEnumerable<PackageMetadata>) other);
        source2.UnionWith((IEnumerable<PackageMetadata>) list);
        foreach (IEnumerable<PackageMetadata> collection in source2.GroupBy<PackageMetadata, string>((Func<PackageMetadata, string>) (package => package.Platform)).Select<IGrouping<string, PackageMetadata>, IEnumerable<PackageMetadata>>((Func<IGrouping<string, PackageMetadata>, IEnumerable<PackageMetadata>>) (packageGroups => packageGroups.OrderByDescending<PackageMetadata, PackageVersion>((Func<PackageMetadata, PackageVersion>) (x => x.Version)).Take<PackageMetadata>(top ?? 10))))
          packagesInternal.AddRange(collection);
        return (IList<PackageMetadata>) packagesInternal;
      }
    }

    public Dictionary<string, string> GetLatestPackageDownloadUrls(
      IVssRequestContext requestContext,
      string packageType)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return this.m_latestPackageDownloadUrls.GetOrAdd(packageType, (Func<string, Dictionary<string, string>>) (pt =>
        {
          IList<PackageMetadata> packages = this.GetPackages(requestContext, pt, (string) null, new int?(1));
          if (packages.Count == 0)
            return (Dictionary<string, string>) null;
          Dictionary<string, string> packageDownloadUrls = new Dictionary<string, string>();
          foreach (PackageMetadata packageMetadata in (IEnumerable<PackageMetadata>) packages)
            packageDownloadUrls[packageMetadata.Platform] = packageMetadata.DownloadUrl;
          return packageDownloadUrls;
        }));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IPackageMetadataService>().GetLatestPackageDownloadUrls(vssRequestContext, packageType);
    }

    public PackageVersion GetLatestPackageVersion(
      IVssRequestContext requestContext,
      string packageType,
      string platform)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(packageType, nameof (packageType), "DistributedTask");
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          return this.m_latestPackageVersions.GetOrAdd(new Tuple<string, string>(packageType, platform), (Func<Tuple<string, string>, PackageVersion>) (k =>
          {
            IList<PackageMetadata> packages = this.GetPackages(requestContext, k.Item1, k.Item2, new int?(1));
            return packages.Count == 0 ? (PackageVersion) null : packages[0].Version;
          }));
        IList<PackageMetadata> packages1 = this.GetPackages(requestContext, packageType, platform, new int?());
        return packages1.Count == 0 ? (PackageVersion) null : packages1[0].Version;
      }
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IPackageMetadataService>().GetLatestPackageVersion(vssRequestContext, packageType, platform);
    }

    public PackageMetadata GetLatestCompatiblePackage(
      IVssRequestContext requestContext,
      string packageType,
      TaskAgent taskAgent)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(packageType, nameof (packageType), "DistributedTask");
      string osDescription = taskAgent.OSDescription;
      using (requestContext.TraceScope("PackageService", nameof (GetLatestCompatiblePackage)))
      {
        if (!string.Equals(packageType, TaskAgentConstants.AgentPackageType, StringComparison.OrdinalIgnoreCase))
          throw new ArgumentOutOfRangeException(nameof (packageType));
        if (string.IsNullOrEmpty(osDescription))
          return this.GetPackages(requestContext, TaskAgentConstants.AgentPackageType, TaskAgentConstants.CoreV1WindowsPlatformName, new int?(1)).SingleOrDefault<PackageMetadata>();
        bool autoRedirectLegacyPlatform = true;
        IList<string> stringList = (IList<string>) new List<string>()
        {
          TaskAgentConstants.CoreV1WindowsPlatformName
        };
        if (osDescription.StartsWith("Microsoft Windows", StringComparison.OrdinalIgnoreCase))
          stringList.Add(TaskAgentConstants.CoreV2WindowsPlatformName);
        else if (osDescription.StartsWith("Linux", StringComparison.OrdinalIgnoreCase))
          stringList.Add(TaskAgentConstants.CoreV2LinuxPlatformName);
        else if (osDescription.StartsWith("Darwin", StringComparison.OrdinalIgnoreCase))
        {
          autoRedirectLegacyPlatform = false;
          string str = osDescription.Remove(0, "Darwin".Length).TrimStart(' ');
          if (str.IndexOf(' ') > 0)
          {
            Version result;
            if (Version.TryParse(str.Remove(str.IndexOf(' ')), out result))
            {
              if (result >= new Version("16.0.0"))
                stringList.Add(TaskAgentConstants.CoreV2OSXPlatformName);
            }
            else
              requestContext.TraceWarning(10015130, "PackageService", "'" + osDescription + "' is not an expected Darwin OS information.");
          }
          else
            requestContext.TraceWarning(10015130, "PackageService", "'" + osDescription + "' is not an expected Darwin OS information.");
        }
        else
          requestContext.TraceWarning(10015130, "PackageService", "'" + osDescription + "' is not an expected OS information.");
        PackageMetadata compatiblePackage1 = (PackageMetadata) null;
        PackageVersion v3 = new PackageVersion("3.0.0");
        PackageVersion packageVersion = new PackageVersion(taskAgent.Version);
        foreach (string platform in (IEnumerable<string>) stringList)
        {
          if (requestContext.IsFeatureEnabled("DistributedTask.Agent.ForceUpdateToLatest2Version") && packageVersion.CompareTo(v3) == -1)
          {
            PackageMetadata compatiblePackage2 = this.GetPackagesInternal(requestContext, TaskAgentConstants.AgentPackageType, platform, new int?(30), autoRedirectLegacyPlatform).FirstOrDefault<PackageMetadata>((Func<PackageMetadata, bool>) (x => x.Version.CompareTo(v3) == -1));
            if (compatiblePackage2 != null && packageVersion.CompareTo(compatiblePackage2.Version) == -1)
              return compatiblePackage2;
          }
          PackageMetadata packageMetadata = this.GetPackagesInternal(requestContext, TaskAgentConstants.AgentPackageType, platform, new int?(1), autoRedirectLegacyPlatform).SingleOrDefault<PackageMetadata>();
          if (packageMetadata != null && (compatiblePackage1 == null || packageMetadata.Version.CompareTo(compatiblePackage1.Version) > 0))
            compatiblePackage1 = packageMetadata;
        }
        return compatiblePackage1;
      }
    }

    internal PackageMetadata AddPackage(
      IVssRequestContext requestContext,
      string packageType,
      string platform,
      PackageVersion version,
      IDictionary<string, string> data)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(packageType, nameof (packageType));
      ArgumentUtility.CheckStringForNullOrEmpty(platform, nameof (platform));
      ArgumentUtility.CheckForNull<PackageVersion>(version, nameof (version));
      using (requestContext.TraceScope("PackageService", nameof (AddPackage)))
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
        PackageData packageData = (PackageData) null;
        using (PackageMetadataComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<PackageMetadataComponent>())
          packageData = component.AddPackage(packageType, platform, version, data);
        this.InvalidatePackageVersionCache(requestContext, SqlNotificationEventIds.PackagesChanged, "");
        return this.GetPackage(requestContext, packageData);
      }
    }

    internal void DeletePackage(
      IVssRequestContext requestContext,
      string packageType,
      string platform,
      PackageVersion version)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(packageType, nameof (packageType));
      ArgumentUtility.CheckStringForNullOrEmpty(platform, nameof (platform));
      ArgumentUtility.CheckForNull<PackageVersion>(version, nameof (version));
      using (requestContext.TraceScope("PackageService", nameof (DeletePackage)))
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
        using (PackageMetadataComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<PackageMetadataComponent>())
          component.DeletePackage(packageType, platform, version);
        this.InvalidatePackageVersionCache(requestContext, SqlNotificationEventIds.PackagesChanged, "");
      }
    }

    internal IList<PackageMetadata> GetLocalPackages(
      IVssRequestContext requestContext,
      string packageType,
      string platform = null,
      PackageVersion version = null)
    {
      try
      {
        if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          return (IList<PackageMetadata>) null;
        string empty = string.Empty;
        string str1;
        if (string.Equals(packageType, TaskAgentConstants.AgentPackageType, StringComparison.OrdinalIgnoreCase))
        {
          str1 = "vsts-agent-";
        }
        else
        {
          if (!string.Equals(packageType, TaskAgentConstants.PipelinesAgentPackageType, StringComparison.OrdinalIgnoreCase))
            return (IList<PackageMetadata>) null;
          str1 = "pipelines-agent-";
        }
        string str2 = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Service/DistributedTask/Settings/PackageLocation", true, PlatformPackageMetadataService.s_offlineAgentsDirectory);
        if (!Directory.Exists(str2))
          return (IList<PackageMetadata>) null;
        if (!string.IsNullOrEmpty(platform) && version != null)
        {
          string path1 = Path.Combine(str2, string.Format("{0}{1}-{2}.zip", (object) str1, (object) platform, (object) version));
          string path2 = Path.Combine(str2, string.Format("{0}{1}-{2}.tar.gz", (object) str1, (object) platform, (object) version));
          string str3 = (string) null;
          if (File.Exists(path1))
            str3 = Path.GetFileName(path1);
          else if (File.Exists(path2))
            str3 = Path.GetFileName(path2);
          else
            requestContext.Trace(10015162, TraceLevel.Info, "DistributedTask", "PackageService", string.Format("Local agent package for {0} ({1}) not found, checked location: '{2}', '{3}'.", (object) platform, (object) version, (object) path1, (object) path2));
          if (string.IsNullOrEmpty(str3))
            return (IList<PackageMetadata>) null;
          string absoluteUri = requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "distributedtask", TaskResourceIds.AgentDownload, (object) new Dictionary<string, object>()
          {
            {
              "package",
              (object) str3
            }
          }, true).AbsoluteUri;
          return (IList<PackageMetadata>) new PackageMetadata[1]
          {
            new PackageMetadata()
            {
              CreatedOn = DateTime.UtcNow,
              Filename = str3,
              Type = packageType,
              Platform = platform,
              Version = version,
              InfoUrl = "https://go.microsoft.com/fwlink/?LinkId=798199",
              DownloadUrl = absoluteUri
            }
          };
        }
        IList<PackageMetadata> source = (IList<PackageMetadata>) new List<PackageMetadata>();
        ILocationService service = requestContext.GetService<ILocationService>();
        foreach (FileInfo file in new DirectoryInfo(str2).GetFiles())
        {
          if (file.Name.StartsWith(str1) && (file.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) || file.Name.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase)))
          {
            int num = file.Name.LastIndexOf('-');
            if (num > str1.Length && num + 1 < file.Name.Length)
            {
              string str4 = file.Name.Substring(num + 1).ToLowerInvariant().Replace(".zip", "").Replace(".tar.gz", "");
              string str5 = file.Name.Substring(str1.Length, num - str1.Length);
              if (!string.IsNullOrEmpty(str5) && !string.IsNullOrEmpty(str4))
              {
                if (PackageVersion.TryParse(str4, out PackageVersion _))
                {
                  string absoluteUri = service.GetResourceUri(requestContext, "distributedtask", TaskResourceIds.AgentDownload, (object) new Dictionary<string, object>()
                  {
                    {
                      "package",
                      (object) file.Name
                    }
                  }, true).AbsoluteUri;
                  PackageMetadata packageMetadata = new PackageMetadata()
                  {
                    CreatedOn = DateTime.UtcNow,
                    Filename = file.Name,
                    Type = packageType,
                    Platform = str5,
                    Version = new PackageVersion(str4),
                    InfoUrl = "https://go.microsoft.com/fwlink/?LinkId=798199",
                    DownloadUrl = absoluteUri
                  };
                  source.Add(packageMetadata);
                }
                else
                  requestContext.Trace(10015162, TraceLevel.Info, "DistributedTask", "PackageService", "'" + file.FullName + "' is not an agent package file with valid naming format, expect: 'vsts-agent-<package_platform>-<package_version>.zip/tar.gz'.");
              }
              else
                requestContext.Trace(10015162, TraceLevel.Info, "DistributedTask", "PackageService", "'" + file.FullName + "' is not an agent package file with valid naming format, expect: 'vsts-agent-<package_platform>-<package_version>.zip/tar.gz'.");
            }
            else
              requestContext.Trace(10015162, TraceLevel.Info, "DistributedTask", "PackageService", "'" + file.FullName + "' is not an agent package file with valid naming format, expect: 'vsts-agent-<package_platform>-<package_version>.zip/tar.gz'.");
          }
          else
            requestContext.Trace(10015162, TraceLevel.Verbose, "DistributedTask", "PackageService", "'" + file.FullName + "' is not an agent package file.");
        }
        if (!string.IsNullOrEmpty(platform))
          return (IList<PackageMetadata>) source.Where<PackageMetadata>((Func<PackageMetadata, bool>) (x => string.Equals(x.Platform, platform, StringComparison.OrdinalIgnoreCase))).ToList<PackageMetadata>();
        return version != null ? (IList<PackageMetadata>) source.Where<PackageMetadata>((Func<PackageMetadata, bool>) (x => x.Version.Equals(version))).ToList<PackageMetadata>() : source;
      }
      catch (Exception ex)
      {
        requestContext.TraceError("DistributedTask", "PackageService", (object) ex);
        return (IList<PackageMetadata>) null;
      }
    }

    private PackageMetadata GetPackage(IVssRequestContext requestContext, PackageData packageData)
    {
      if (packageData == null)
        return (PackageMetadata) null;
      PackageMetadata package = packageData.Package;
      this.GetPackageProvider(requestContext, packageData.Data).EnsurePackageProperties(requestContext, package, packageData.Data);
      return package;
    }

    private IPackageMetadataProvider GetPackageProvider(
      IVssRequestContext requestContext,
      IDictionary<string, string> data)
    {
      IPackageMetadataProvider packageProvider = (IPackageMetadataProvider) null;
      string providerKey;
      if (data != null && data.TryGetValue("Provider", out providerKey))
      {
        using (IDisposableReadOnlyList<IPackageMetadataProvider> extensions = requestContext.GetExtensions<IPackageMetadataProvider>((Func<IPackageMetadataProvider, bool>) (pp => pp.Key.Equals(providerKey, StringComparison.OrdinalIgnoreCase))))
          packageProvider = extensions.FirstOrDefault<IPackageMetadataProvider>();
      }
      if (packageProvider == null)
        packageProvider = (IPackageMetadataProvider) new DefaultPackageMetadataProvider();
      return packageProvider;
    }

    private void InvalidatePackageVersionCache(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (this.m_latestPackageVersions != null)
        this.m_latestPackageVersions.Clear();
      if (this.m_latestPackageDownloadUrls == null)
        return;
      this.m_latestPackageDownloadUrls.Clear();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "DistributedTask", SqlNotificationEventIds.PackagesChanged, new SqlNotificationCallback(this.InvalidatePackageVersionCache), false);
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      this.m_latestPackageVersions = new ConcurrentDictionary<Tuple<string, string>, PackageVersion>();
      this.m_latestPackageDownloadUrls = new ConcurrentDictionary<string, Dictionary<string, string>>();
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "DistributedTask", SqlNotificationEventIds.PackagesChanged, new SqlNotificationCallback(this.InvalidatePackageVersionCache), true);
    }
  }
}
