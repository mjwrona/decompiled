// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.ServicingDataInstaller
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public sealed class ServicingDataInstaller
  {
    private static Regex s_unneededForHostedRegex = new Regex("(Dev1[124])");
    private static Regex s_integerRegex = new Regex("[0-9]+");
    private readonly string m_servicingFilesPath;
    private object m_cacheLock = new object();
    private readonly Dictionary<ServicingOperationTarget, ServicingOperationProvider> m_servicingOperationProviderCache;
    private readonly List<string> m_servicingDataAssemblyNames;
    private readonly List<ServicingOperationTarget> m_uploadTargets;
    private readonly AssemblyServicingResourceProvider m_servicingResourceProvider;
    private readonly ITFLogger m_log;

    public ServicingDataInstaller(
      string servicingFilesPath,
      bool hostedDeployment,
      ITFLogger logger)
      : this(servicingFilesPath, (ServicingOperationTarget[]) null, hostedDeployment, logger)
    {
    }

    public ServicingDataInstaller(
      string servicingFilesPath,
      ServicingOperationTarget[] uploadTargets,
      bool hostedDeployment,
      ITFLogger logger,
      params string[] servicingDataAssemblyNames)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(servicingFilesPath, nameof (servicingFilesPath));
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      this.m_servicingFilesPath = servicingFilesPath;
      this.m_uploadTargets = new List<ServicingOperationTarget>();
      this.m_log = logger;
      this.m_servicingDataAssemblyNames = new List<string>();
      this.HostedDeployment = hostedDeployment;
      this.m_servicingResourceProvider = new AssemblyServicingResourceProvider(servicingFilesPath, hostedDeployment);
      this.m_servicingOperationProviderCache = new Dictionary<ServicingOperationTarget, ServicingOperationProvider>();
      if (uploadTargets != null)
      {
        foreach (ServicingOperationTarget uploadTarget in uploadTargets)
        {
          if (!this.m_uploadTargets.Contains(uploadTarget))
          {
            logger.Info("Servicing will be uploaded for database target: {0}", (object) uploadTarget);
            this.m_uploadTargets.Add(uploadTarget);
          }
        }
      }
      else
        logger.Info("No servicing targets were provided to ServicingDataInstaller. No servicing operations will be uploaded. Resources will still be loaded from the Servicing Directory.");
      if (servicingDataAssemblyNames != null)
      {
        foreach (string dataAssemblyName in servicingDataAssemblyNames)
        {
          if (!string.IsNullOrEmpty(dataAssemblyName) && !this.m_servicingDataAssemblyNames.Contains(dataAssemblyName))
          {
            logger.Info("Servicing will be uploaded from data assembly: {0}", (object) dataAssemblyName);
            this.m_servicingDataAssemblyNames.Add(dataAssemblyName);
          }
        }
      }
      else
        logger.Info("No servicing data assembly was provided to ServicingDataInstaller. No servicing operations will be uploaded. Resources will still be loaded from the Servicing Directory.");
    }

    public bool HostedDeployment { get; private set; }

    public void UpdateServicingData(
      IVssRequestContext deploymentContext,
      TeamFoundationLock servicingLock = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(deploymentContext, nameof (deploymentContext));
      TeamFoundationLock teamFoundationLock = (TeamFoundationLock) null;
      if (servicingLock == null)
        teamFoundationLock = servicingLock = deploymentContext.GetService<TeamFoundationServicingService>().AcquireServicingLock(deploymentContext, TeamFoundationLockMode.Exclusive);
      using (teamFoundationLock)
      {
        this.UploadServicingOperations(deploymentContext, servicingLock);
        this.AddServicingResources(deploymentContext, servicingLock);
      }
    }

    public void UpdateProcessTemplates(IVssRequestContext deploymentContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(deploymentContext, nameof (deploymentContext));
      this.UploadProcessTemplates(deploymentContext);
    }

    public void UploadServicingOperations(
      IVssRequestContext deploymentContext,
      TeamFoundationLock servicingLock)
    {
      Dictionary<ServicingOperationTarget, ServicingOperation[]> servicingOperationDict = new Dictionary<ServicingOperationTarget, ServicingOperation[]>();
      foreach (ServicingOperationTarget uploadTarget in this.m_uploadTargets)
      {
        ServicingOperation[] servicingOperations = this.GetServicingOperationProvider(uploadTarget).GetServicingOperations();
        servicingOperationDict[uploadTarget] = servicingOperations;
      }
      deploymentContext.GetService<TeamFoundationServicingService>().UploadServicingOperations(deploymentContext, servicingOperationDict);
    }

    public void AddServicingResources(
      IVssRequestContext deploymentContext,
      TeamFoundationLock collectionServicingLock)
    {
      this.Log.Info("Adding servicing resources");
      ArgumentUtility.CheckForNull<IVssRequestContext>(deploymentContext, nameof (deploymentContext));
      this.Log.Info("Querying servicing resources that has to be uploaded to the deployment database");
      string[] source = this.m_servicingResourceProvider.ResourceNames;
      this.Log.Info("Found {0} servicing resources.", (object) source.Length);
      if (this.HostedDeployment)
      {
        source = ((IEnumerable<string>) source).Where<string>((Func<string, bool>) (name => this.IsNeededForHostedDeployment(name))).ToArray<string>();
        this.Log.Info("Optimized resources for hosted deploy to {0} servicing resources.", (object) source.Length);
      }
      deploymentContext.GetService<TeamFoundationServicingService>();
      TeamFoundationFileService service = deploymentContext.GetService<TeamFoundationFileService>();
      this.Log.Info("Querying servicing files stats");
      List<FileStatistics> servicingFileStats = service.QueryNamedFiles(deploymentContext, OwnerId.Servicing);
      this.Log.Info("Found {0} servicing files in the deployment database.", (object) servicingFileStats.Count);
      IVssDeploymentServiceHost deploymentHost = deploymentContext.ServiceHost.DeploymentServiceHost;
      ParallelOptions parallelOptions = new ParallelOptions()
      {
        MaxDegreeOfParallelism = 16
      };
      Parallel.ForEach<string, IVssRequestContext>(((IEnumerable<string>) source).Distinct<string>(), parallelOptions, (Func<IVssRequestContext>) (() => deploymentHost.CreateServicingContext()), (Func<string, ParallelLoopState, IVssRequestContext, IVssRequestContext>) ((name, loopState, requestContext) =>
      {
        using (Stream servicingResource = this.m_servicingResourceProvider.GetServicingResource(name))
        {
          FileStatistics existingFileStat = servicingFileStats.FirstOrDefault<FileStatistics>((Func<FileStatistics, bool>) (fstat => VssStringComparer.FilePath.Equals(fstat.FileName, name)));
          this.AddServicingResource(requestContext, name, servicingResource, existingFileStat);
        }
        return requestContext;
      }), (Action<IVssRequestContext>) (requestContext => requestContext.Dispose()));
      servicingFileStats = service.QueryNamedFiles(deploymentContext, OwnerId.Servicing);
      this.AddServicingResourcesFromDirectory(deploymentContext, servicingFileStats, Path.Combine(this.m_servicingFilesPath, "Partition\\ServicingFiles"), (Func<string, string>) (fileName => Path.GetFileName(fileName)));
      this.AddServicingResourcesFromDirectory(deploymentContext, servicingFileStats, Path.Combine(this.m_servicingFilesPath, "Application\\ServicingFiles"), (Func<string, string>) (fileName => Path.GetFileName(fileName)));
      this.AddServicingResourcesFromDirectory(deploymentContext, servicingFileStats, Path.Combine(this.m_servicingFilesPath, "Deployment\\ServicingFiles"), (Func<string, string>) (fileName => Path.GetFileName(fileName)));
      deploymentContext.GetService<CachedRegistryService>().SetValue<string>(deploymentContext, FrameworkServerConstants.ServicingResourceCookie, DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture));
    }

    private bool IsNeededForHostedDeployment(string resourceName)
    {
      int num = resourceName.IndexOf("Dev15M", StringComparison.OrdinalIgnoreCase);
      if (num >= 0)
      {
        Match match = ServicingDataInstaller.s_integerRegex.Match(resourceName, num + "Dev15M".Length);
        int result;
        if (match.Success && int.TryParse(match.Value, out result))
        {
          if (result >= 113)
            return true;
          if (resourceName.IndexOf("Notifications", StringComparison.OrdinalIgnoreCase) > 0)
            return result >= 98;
        }
      }
      return !ServicingDataInstaller.s_unneededForHostedRegex.IsMatch(resourceName);
    }

    private void UploadProcessTemplates(IVssRequestContext deploymentContext)
    {
      List<FileStatistics> servicingFileStats = deploymentContext.GetService<TeamFoundationFileService>().QueryNamedFiles(deploymentContext, OwnerId.Servicing);
      string directoryName = Path.GetDirectoryName(this.m_servicingFilesPath);
      this.Log.Info("Checking for Process templates to upload.");
      string fileName = Path.GetFileName(directoryName);
      if (string.Equals(fileName, "Deploy", StringComparison.OrdinalIgnoreCase))
      {
        CultureInfo culture = deploymentContext.ServiceHost.GetCulture(deploymentContext);
        this.Log.Info("Server culture: {0}", (object) culture.EnglishName);
        int lcid = culture.LCID;
        this.AddServicingResourcesFromDirectory(deploymentContext, servicingFileStats, Path.Combine(directoryName, "ProcessTemplateManagerFiles", lcid.ToString()), (Func<string, string>) (dataFile => dataFile.Substring(dataFile.IndexOf("\\Deploy\\", StringComparison.OrdinalIgnoreCase) + 1)));
      }
      else if (string.Equals(fileName, "Tfs", StringComparison.OrdinalIgnoreCase) && Directory.Exists(directoryName + "\\Deploy"))
        this.AddServicingResourcesFromDirectory(deploymentContext, servicingFileStats, Path.Combine(directoryName, "Deploy\\ProcessTemplateManagerFiles"), (Func<string, string>) (dataFile => dataFile.Substring(dataFile.IndexOf("\\Deploy\\", StringComparison.OrdinalIgnoreCase) + 1)));
      else if (string.Equals(fileName, "Boards", StringComparison.OrdinalIgnoreCase) && Directory.Exists(directoryName + "\\Deploy"))
        this.AddServicingResourcesFromDirectory(deploymentContext, servicingFileStats, Path.Combine(directoryName, "Deploy\\ProcessTemplateManagerFiles"), (Func<string, string>) (dataFile => dataFile.Substring(dataFile.IndexOf("\\Deploy\\", StringComparison.OrdinalIgnoreCase) + 1)));
      else
        this.Log.Info("No process templates to upload.");
    }

    private void AddServicingResourcesFromDirectory(
      IVssRequestContext deploymentContext,
      List<FileStatistics> servicingFileStats,
      string directoryPath,
      Func<string, string> getNameCallback)
    {
      this.Log.Info("Adding servicing resources from : {0} .", (object) directoryPath);
      if (Directory.Exists(directoryPath))
      {
        string[] files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
        this.Log.Info("Found {0} files", (object) files.Length);
        foreach (string path in files)
        {
          using (FileStream contentStream = new FileStream(path, FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
          {
            string name = getNameCallback(path);
            this.AddServicingResource(deploymentContext, name, (Stream) contentStream, servicingFileStats.FirstOrDefault<FileStatistics>((Func<FileStatistics, bool>) (fileStat => VssStringComparer.FilePath.Equals(fileStat.FileName, name))));
          }
        }
      }
      else
        this.Log.Info("{0} directory does not exist", (object) directoryPath);
    }

    private void AddServicingResource(
      IVssRequestContext deploymentContext,
      string name,
      Stream contentStream,
      FileStatistics existingFileStat)
    {
      this.Log.Info("AddServicingResource: {0}", (object) name);
      if (!MD5Util.CanCreateMD5Provider)
        this.Log.Warning("Cannot create MD5 provider on this machine.");
      Stream stream;
      if (!contentStream.CanSeek)
      {
        stream = (Stream) new MemoryStream();
        contentStream.CopyTo(stream);
        stream.Position = 0L;
      }
      else
        stream = contentStream;
      byte[] md5 = MD5Util.CalculateMD5(stream, true);
      if ((md5.Length == 0 || existingFileStat == null || existingFileStat.HashValue == null || md5.Length == 0 ? 0 : (((IEnumerable<byte>) md5).SequenceEqual<byte>((IEnumerable<byte>) existingFileStat.HashValue) ? 1 : 0)) != 0)
      {
        this.Log.Info("The servicing file with the same name and MD5 hash already exists for file: {0}. Skipping upload.", (object) name);
      }
      else
      {
        if (existingFileStat != null)
          deploymentContext.GetService<TeamFoundationFileService>().DeleteNamedFiles(deploymentContext, OwnerId.Servicing, (IEnumerable<string>) new string[1]
          {
            name
          });
        deploymentContext.GetService<TeamFoundationServicingService>().AddServicingResource(deploymentContext, name, md5, stream, stream.Length, stream.Length, 0L, CompressionType.None);
      }
    }

    private ServicingOperationProvider GetServicingOperationProvider(ServicingOperationTarget target)
    {
      ServicingOperationProvider operationProvider = (ServicingOperationProvider) null;
      if (!this.m_servicingOperationProviderCache.TryGetValue(target, out operationProvider))
      {
        lock (this.m_cacheLock)
        {
          if (!this.m_servicingOperationProviderCache.TryGetValue(target, out operationProvider))
          {
            operationProvider = new ServicingOperationProvider(this.m_servicingFilesPath, target, this.HostedDeployment, this.m_servicingDataAssemblyNames);
            this.m_servicingOperationProviderCache.Add(target, operationProvider);
          }
        }
      }
      return operationProvider;
    }

    private ITFLogger Log => this.m_log;
  }
}
