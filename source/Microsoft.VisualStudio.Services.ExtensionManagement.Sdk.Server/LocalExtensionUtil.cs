// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.LocalExtensionUtil
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  public static class LocalExtensionUtil
  {
    private const string c_localExtensionInstalledRegistryPathFormat = "/Configuration/LocalContributions/{0}/{1}/Installed";
    private const string c_localExtensionVersionRegistryPathFormat = "/Configuration/LocalContributions/{0}/{1}/Version";
    private const string c_localExtensionSupportedHostsRegistryPathFormat = "/Configuration/LocalContributions/{0}/{1}/SupportedHosts";
    private static readonly string s_area = nameof (LocalExtensionUtil);
    private static readonly string s_layer = "Extensions";

    public static bool InstallVSIX(
      IVssRequestContext requestContext,
      string vsixPath,
      string supportedHostsStr,
      bool isCdnEnabled,
      string cdnContainerName,
      string cdnStorageConnectionString,
      ITFLogger logger)
    {
      logger = logger ?? (ITFLogger) new NullLogger();
      TeamFoundationHostType result;
      if (!Enum.TryParse<TeamFoundationHostType>(supportedHostsStr, out result))
      {
        logger.Error("InstallVSIX - supported hosts is invalid '{0}'", (object) supportedHostsStr);
        throw new LocalExtensionInstallException(string.Format("InstallVSIX - supported hosts is invalid '{0}'", (object) supportedHostsStr));
      }
      return LocalExtensionUtil.InstallVSIX(requestContext, vsixPath, result, isCdnEnabled, cdnContainerName, cdnStorageConnectionString, logger);
    }

    public static bool InstallVSIX(
      IVssRequestContext requestContext,
      string vsixPath,
      TeamFoundationHostType supportedHosts,
      bool isCdnEnabled,
      string cdnContainerName,
      string cdnStorageConnectionString,
      ITFLogger logger)
    {
      logger = logger ?? (ITFLogger) new NullLogger();
      requestContext.CheckDeploymentRequestContext();
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        logger.Info("CDN Enabled                 : {0}", (object) isCdnEnabled);
        logger.Info("CDN Container Name          : {0}", (object) cdnContainerName);
      }
      if (!File.Exists(vsixPath))
      {
        logger.Error("InstallVSIX - file not found '{0}'", (object) vsixPath);
        throw new LocalExtensionInstallException(string.Format("InstallVSIX - file not found '{0}'", (object) vsixPath));
      }
      byte[] packageBytes = File.ReadAllBytes(vsixPath);
      return LocalExtensionUtil.InstallVSIX(requestContext, false, packageBytes, supportedHosts, isCdnEnabled, cdnContainerName, cdnStorageConnectionString, logger);
    }

    public static bool InstallVSIX(
      IVssRequestContext requestContext,
      bool fallbackOnly,
      byte[] packageBytes,
      TeamFoundationHostType supportedHosts,
      bool isCdnEnabled,
      string cdnContainerName,
      string cdnStorageConnectionString,
      ITFLogger logger)
    {
      int errorCount = 0;
      logger = logger ?? (ITFLogger) new NullLogger();
      requestContext.CheckDeploymentRequestContext();
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        logger.Info("CDN Enabled                 : {0}", (object) isCdnEnabled);
        logger.Info("CDN Container Name          : {0}", (object) cdnContainerName);
      }
      PackageDetails packageDetails = (PackageDetails) null;
      new ExtensionPackage().ExtensionManifest = Convert.ToBase64String(packageBytes);
      using (MemoryStream packageStream = new MemoryStream(packageBytes))
        packageDetails = VSIXPackage.Parse((Stream) packageStream);
      if (packageDetails?.Manifest?.Metadata == null || string.IsNullOrEmpty(packageDetails.Manifest.Metadata.Identity?.Version))
        throw new LocalExtensionInstallException(string.Format("InstallVSIX step requires a manifest, metadata, and version."));
      string publisherName = packageDetails.Manifest.Metadata.Identity.PublisherName;
      string extensionName = packageDetails.Manifest.Metadata.Identity.ExtensionName;
      string version = packageDetails.Manifest.Metadata.Identity.Version;
      logger.Info("Installing local extension: {0}.{1} {2}", (object) publisherName, (object) extensionName, (object) version);
      if (!GalleryUtil.InstallationTargetsHasVSTS((IEnumerable<InstallationTarget>) packageDetails.Manifest.Installation))
        throw new LocalExtensionInstallException(string.Format("InstallVSIX step requires extension to target VSTS. Publisher: {0}  Extension: {1}", (object) publisherName, (object) extensionName));
      if (!packageDetails.Manifest.Metadata.Flags.HasFlag((Enum) PublishedExtensionFlags.BuiltIn))
        throw new LocalExtensionInstallException(string.Format("InstallVSIX step requires extensions to be BuiltIn. Publisher: {0}  Extension: {1}", (object) publisherName, (object) extensionName));
      if (!packageDetails.Manifest.Metadata.Identity.PublisherName.Equals("ms", StringComparison.OrdinalIgnoreCase))
        throw new LocalExtensionInstallException(string.Format("InstallVSIX step will only publish to ms publisher. Publisher: {0}  Extension: {1}", (object) publisherName, (object) extensionName));
      IVssRegistryService service1 = requestContext.GetService<IVssRegistryService>();
      string registryRoot = string.Format("/Configuration/LocalContributions/{0}/{1}/", (object) publisherName, (object) extensionName);
      string registryPath1 = registryRoot + "Version";
      string registryPath2 = registryRoot + "SupportedHosts";
      string registryPath3 = fallbackOnly ? registryRoot + "Fallback" : registryRoot + "Installed";
      IVssRegistryService registryService1 = service1;
      IVssRequestContext requestContext1 = requestContext;
      RegistryQuery registryQuery = (RegistryQuery) registryPath1;
      ref RegistryQuery local1 = ref registryQuery;
      string empty = string.Empty;
      string str1 = registryService1.GetValue<string>(requestContext1, in local1, empty);
      IVssRegistryService registryService2 = service1;
      IVssRequestContext requestContext2 = requestContext;
      registryQuery = (RegistryQuery) registryPath3;
      ref RegistryQuery local2 = ref registryQuery;
      bool flag1 = registryService2.GetValue<bool>(requestContext2, in local2, false);
      IVssRegistryService registryService3 = service1;
      IVssRequestContext requestContext3 = requestContext;
      registryQuery = (RegistryQuery) registryPath2;
      ref RegistryQuery local3 = ref registryQuery;
      TeamFoundationHostType foundationHostType = registryService3.GetValue<TeamFoundationHostType>(requestContext3, in local3, TeamFoundationHostType.Unknown);
      if (flag1 && !string.IsNullOrEmpty(str1) && str1.Equals(version, StringComparison.OrdinalIgnoreCase) && foundationHostType == supportedHosts)
      {
        logger.Info("Current version has already been published.  Skipping this extension.");
        return false;
      }
      ConcurrentBag<int> fileIdsToDelete = new ConcurrentBag<int>();
      ConcurrentDictionary<string, int> previousVersionAssets = LocalExtensionUtil.FindCurrentVersionAssets(requestContext, registryRoot);
      ConcurrentQueue<Tuple<ManifestFile, Stream>> queue = new ConcurrentQueue<Tuple<ManifestFile, Stream>>();
      ConcurrentBag<RegistryEntry> registryUpdates = new ConcurrentBag<RegistryEntry>();
      registryUpdates.Add(RegistryEntry.Create<string>(registryPath1, version));
      registryUpdates.Add(RegistryEntry.Create<int>(registryPath2, (int) supportedHosts));
      using (MemoryStream packageStream = new MemoryStream(packageBytes))
      {
        List<Task> taskList = new List<Task>();
        IVssDeploymentServiceHost deploymentHost = requestContext.ServiceHost.DeploymentServiceHost;
        AutoResetEvent dataReadyToProcessEvent = new AutoResetEvent(false);
        ManualResetEvent vsixParsingCompleteEvent = new ManualResetEvent(false);
        for (int index = 0; index < 10; ++index)
          taskList.Add(Task.Factory.StartNew((Action) (() =>
          {
            try
            {
              using (IVssRequestContext servicingContext = deploymentHost.CreateServicingContext())
              {
                IDisposableReadOnlyList<ILocalExtensionStoragePlugin> extensions = servicingContext.GetExtensions<ILocalExtensionStoragePlugin>(ExtensionLifetime.Service);
                ITeamFoundationFileService service2 = servicingContext.GetService<ITeamFoundationFileService>();
label_2:
                int num1;
                Tuple<ManifestFile, Stream> result;
                do
                {
                  num1 = WaitHandle.WaitAny(new WaitHandle[2]
                  {
                    (WaitHandle) vsixParsingCompleteEvent,
                    (WaitHandle) dataReadyToProcessEvent
                  });
                  if (queue.TryDequeue(out result))
                    goto label_4;
                }
                while (num1 != 0);
                return;
label_4:
                ManifestFile manifestFile = result.Item1;
                Stream stream = result.Item2;
                try
                {
                  if (!manifestFile.AssetType.Equals("Microsoft.VisualStudio.Services.VSIXPackage", StringComparison.OrdinalIgnoreCase))
                  {
                    string str2 = !string.IsNullOrEmpty(manifestFile.Language) ? manifestFile.Language : "_";
                    string str3 = string.Format("/Configuration/LocalContributions/{0}/{1}/Assets/{2}/{3}/", (object) publisherName, (object) extensionName, (object) str2, (object) manifestFile.AssetType);
                    string str4 = str3 + "FileId";
                    string registryPath4 = str3 + "Addressable";
                    string registryPath5 = str3 + "ContentType";
                    logger.Info("Processing file: {0}", (object) manifestFile.FullPath);
                    bool flag2 = false;
                    byte[] numArray = (byte[]) null;
                    int fileId1;
                    if (previousVersionAssets.TryGetValue(str4, out fileId1))
                    {
                      FileStatistics fileStatistics = service2.GetFileStatistics(servicingContext, (long) fileId1);
                      if (fileStatistics != null && fileStatistics.HashValue != null && fileStatistics.HashValue.Length != 0)
                      {
                        numArray = MD5Util.CalculateMD5(stream, true);
                        if (numArray != null && ArrayUtil.Equals(fileStatistics.HashValue, numArray))
                        {
                          logger.Info("File has not changed.  Skipping update: {0}", (object) manifestFile.FullPath);
                          previousVersionAssets.TryRemove(str4, out int _);
                          flag2 = true;
                        }
                      }
                    }
                    if (!flag2)
                    {
                      int fileId2 = 0;
                      if (numArray == null)
                        numArray = MD5Util.CalculateMD5(stream, true);
                      try
                      {
                        service2.UploadFile(servicingContext, ref fileId2, stream, numArray, stream.Length, stream.Length, 0L, CompressionType.None, OwnerId.Generic, Guid.Empty, "", false, requestContext.ExecutionEnvironment.IsCloudDeployment);
                      }
                      catch (Exception ex)
                      {
                        logger.Error(string.Format("Failed to upload {0}. Exception: {1}", (object) manifestFile.FullPath, (object) ex));
                        throw;
                      }
                      registryUpdates.Add(RegistryEntry.Create<int>(str4, fileId2));
                      registryUpdates.Add(RegistryEntry.Create<bool>(registryPath4, manifestFile.Addressable));
                      registryUpdates.Add(RegistryEntry.Create<string>(registryPath5, manifestFile.ContentType));
                      int num2 = -1;
                      if (previousVersionAssets.TryGetValue(str4, out num2))
                        fileIdsToDelete.Add(num2);
                      previousVersionAssets.TryRemove(str4, out int _);
                    }
                    if (extensions != null)
                    {
                      string str5 = manifestFile.FullPath;
                      if (str5.StartsWith("/"))
                        str5 = str5.Substring(1);
                      using (IEnumerator<ILocalExtensionStoragePlugin> enumerator = extensions.GetEnumerator())
                      {
                        while (enumerator.MoveNext())
                        {
                          ILocalExtensionStoragePlugin current = enumerator.Current;
                          stream.Seek(0L, SeekOrigin.Begin);
                          IVssRequestContext requestContext4 = servicingContext;
                          ITFLogger logger1 = logger;
                          string fullPath = str5;
                          string publisherName1 = publisherName;
                          string extensionName1 = extensionName;
                          string version1 = version;
                          string language = str2;
                          Stream asset = stream;
                          int num3 = isCdnEnabled ? 1 : 0;
                          string cdnContainerName1 = cdnContainerName;
                          string cdnStorageConnectionString1 = cdnStorageConnectionString;
                          current.StoreAsset(requestContext4, logger1, fullPath, publisherName1, extensionName1, version1, language, asset, num3 != 0, cdnContainerName1, cdnStorageConnectionString1);
                        }
                        goto label_2;
                      }
                    }
                    else
                      goto label_2;
                  }
                  else
                    goto label_2;
                }
                finally
                {
                  stream?.Dispose();
                }
              }
            }
            catch (Exception ex)
            {
              logger.Error(ex);
              Interlocked.Increment(ref errorCount);
            }
          })));
        VSIXPackage.Parse((Stream) packageStream, (Func<ManifestFile, Stream, bool>) ((manifestFile, fileStream) =>
        {
          MemoryStream destination = new MemoryStream();
          fileStream.CopyTo((Stream) destination);
          destination.Seek(0L, SeekOrigin.Begin);
          queue.Enqueue(new Tuple<ManifestFile, Stream>(manifestFile, (Stream) destination));
          dataReadyToProcessEvent.Set();
          return false;
        }));
        vsixParsingCompleteEvent.Set();
        if (Task.WaitAll(taskList.ToArray(), TimeSpan.FromMinutes(30.0)))
        {
          if (errorCount == 0)
            goto label_31;
        }
        logger.Error("Failed to upload vsix files to storage.");
        while (queue.Count > 0)
        {
          Tuple<ManifestFile, Stream> result;
          if (queue.TryDequeue(out result))
            result.Item2?.Dispose();
          else
            break;
        }
      }
label_31:
      if (errorCount == 0)
      {
        foreach (string key in (IEnumerable<string>) previousVersionAssets.Keys)
        {
          fileIdsToDelete.Add(previousVersionAssets[key]);
          if (key.EndsWith("FileId"))
          {
            string str6 = key.Substring(0, key.Length - "FileId".Length);
            registryUpdates.Add(new RegistryEntry(key, (string) null));
            registryUpdates.Add(new RegistryEntry(str6 + "Addressable", (string) null));
            registryUpdates.Add(new RegistryEntry(str6 + "ContentType", (string) null));
          }
        }
        registryUpdates.Add(RegistryEntry.Create<bool>(registryPath3, true));
        service1.WriteEntries(requestContext, (IEnumerable<RegistryEntry>) registryUpdates);
        if (fileIdsToDelete.Count > 0)
        {
          try
          {
            requestContext.GetService<ITeamFoundationFileService>().DeleteFiles(requestContext, (IEnumerable<int>) fileIdsToDelete);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(10013664, LocalExtensionUtil.s_area, LocalExtensionUtil.s_layer, ex);
          }
        }
      }
      return errorCount == 0;
    }

    public static void Uninstall(
      IVssRequestContext requestContext,
      string publisher,
      string extensionName)
    {
      requestContext.CheckDeploymentRequestContext();
      string registryRoot = !string.IsNullOrEmpty(publisher) && !string.IsNullOrEmpty(extensionName) ? string.Format("/Configuration/LocalContributions/{0}/{1}/", (object) publisher, (object) extensionName) : throw new LocalExtensionInstallException(string.Format("RemoveInstalled requres a Publisher({0}) and ExtensionName({1})", (object) publisher, (object) extensionName));
      ConcurrentDictionary<string, int> currentVersionAssets = LocalExtensionUtil.FindCurrentVersionAssets(requestContext, registryRoot);
      if (currentVersionAssets.Keys.Count > 0)
      {
        try
        {
          requestContext.GetService<ITeamFoundationFileService>().DeleteFiles(requestContext, (IEnumerable<int>) currentVersionAssets.Values);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10013664, LocalExtensionUtil.s_area, LocalExtensionUtil.s_layer, ex);
        }
      }
      requestContext.GetService<IVssRegistryService>().DeleteEntries(requestContext, registryRoot + "**");
    }

    public static string AssetTypeFromPath(string path) => LocalExtensionUtil.AssetTypeFromPath(path.Split(new char[1]
    {
      '/'
    }, StringSplitOptions.RemoveEmptyEntries));

    public static string AssetTypeFromPath(string[] pathParts)
    {
      string str = pathParts.Length >= 7 ? pathParts[pathParts.Length - 1] : throw new ArgumentException(nameof (pathParts));
      int num;
      if (str == "Addressable" || str == "FileId")
      {
        if (pathParts.Length < 8)
          throw new ArgumentException(nameof (pathParts));
        num = pathParts.Length - 2;
      }
      else
        num = pathParts.Length - 1;
      StringBuilder stringBuilder = new StringBuilder(pathParts[6]);
      for (int index = 7; index <= num; ++index)
      {
        stringBuilder.Append("/");
        stringBuilder.Append(pathParts[index]);
      }
      return stringBuilder.ToString();
    }

    public static string ReadLocalExtensionVersion(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = context.GetService<IVssRegistryService>();
      string str = string.Format("/Configuration/LocalContributions/{0}/{1}/Version", (object) publisherName, (object) extensionName);
      IVssRequestContext requestContext1 = context;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) str;
      string empty = string.Empty;
      return service.GetValue<string>(requestContext1, in local, empty);
    }

    public static void ReadLocalExtensionValuesFromRegistry(
      IVssRequestContext deploymentContext,
      string publisherName,
      string extensionName,
      out string currentVersion,
      out bool versionInstalled,
      out int supportedHosts)
    {
      IVssRegistryService service = deploymentContext.GetService<IVssRegistryService>();
      currentVersion = service.GetValue<string>(deploymentContext, (RegistryQuery) string.Format("/Configuration/LocalContributions/{0}/{1}/Version", (object) publisherName, (object) extensionName), string.Empty);
      versionInstalled = service.GetValue<bool>(deploymentContext, (RegistryQuery) string.Format("/Configuration/LocalContributions/{0}/{1}/Installed", (object) publisherName, (object) extensionName), false);
      supportedHosts = service.GetValue<int>(deploymentContext, (RegistryQuery) string.Format("/Configuration/LocalContributions/{0}/{1}/SupportedHosts", (object) publisherName, (object) extensionName), -1);
    }

    private static ConcurrentDictionary<string, int> FindCurrentVersionAssets(
      IVssRequestContext requestContext,
      string registryRoot)
    {
      ConcurrentDictionary<string, int> currentVersionAssets = new ConcurrentDictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      requestContext.GetService<ITeamFoundationFileService>();
      string registryPathPattern = string.Format("{0}{1}/**", (object) registryRoot, (object) "Assets");
      IVssRequestContext requestContext1 = requestContext;
      RegistryQuery query = new RegistryQuery(registryPathPattern);
      foreach (RegistryEntry readEntry in service.ReadEntries(requestContext1, query))
      {
        if (readEntry.Name.Equals("FileId", StringComparison.OrdinalIgnoreCase))
        {
          int num = readEntry.GetValue<int>();
          if (num != 0)
            currentVersionAssets.TryAdd(readEntry.Path, num);
        }
      }
      return currentVersionAssets;
    }
  }
}
