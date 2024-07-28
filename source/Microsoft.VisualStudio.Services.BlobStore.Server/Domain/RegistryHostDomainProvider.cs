// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Domain.RegistryHostDomainProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common.MultiDomainExceptions;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Domain
{
  public class RegistryHostDomainProvider : 
    IAdminHostDomainProvider,
    IHostDomainProvider,
    IDisposable
  {
    private const string LegacyDomain = "LegacyDomain";
    private const string NewDomain = "NewDomain";
    private const string Domain2 = "Domain2";
    private const string DomainId = "DomainId";
    private const string Shards = "Shards";
    private const string Region = "Region";
    private const string RedundancyType = "RedundancyType";
    internal const char DefaultShardListSeparator = ',';
    private static readonly string DefaultRegistryBasePath = "/Configuration/BlobStore/MultiDomain/HostDomain";
    private static readonly string DefaultLegacyDomainRegistryBasePath = RegistryHostDomainProvider.DefaultRegistryBasePath + "/LegacyDomain";
    private static readonly string DefaultNewDomainRegistryBasePath = RegistryHostDomainProvider.DefaultRegistryBasePath + "/NewDomain";
    private static readonly string DefaultDomain2RegistryBasePath = RegistryHostDomainProvider.DefaultRegistryBasePath + "/Domain2";
    internal static readonly string LegacyDomainIdRegistryPath = RegistryHostDomainProvider.DefaultLegacyDomainRegistryBasePath + "/DomainId";
    internal static readonly string NewDomainIdRegistryPath = RegistryHostDomainProvider.DefaultNewDomainRegistryBasePath + "/DomainId";
    internal static readonly string Domain2DomainIdRegistryPath = RegistryHostDomainProvider.DefaultDomain2RegistryBasePath + "/DomainId";
    internal static readonly string LegacyDomainShardsRegistryPath = RegistryHostDomainProvider.DefaultLegacyDomainRegistryBasePath + "/Shards";
    internal static readonly string NewDomainShardsRegistryPath = RegistryHostDomainProvider.DefaultNewDomainRegistryBasePath + "/Shards";
    internal static readonly string Domain2ShardsRegistryPath = RegistryHostDomainProvider.DefaultDomain2RegistryBasePath + "/Shards";
    internal static readonly string LegacyDomainRegionRegistryPath = RegistryHostDomainProvider.DefaultLegacyDomainRegistryBasePath + "/Region";
    internal static readonly string NewDomainRegionRegistryPath = RegistryHostDomainProvider.DefaultNewDomainRegistryBasePath + "/Region";
    internal static readonly string Domain2RegionRegistryPath = RegistryHostDomainProvider.DefaultDomain2RegistryBasePath + "/Region";
    internal static readonly string LegacyDomainRedundancyTypeRegistryPath = RegistryHostDomainProvider.DefaultLegacyDomainRegistryBasePath + "/RedundancyType";
    internal static readonly string NewDomainRedundancyTypeRegistryPath = RegistryHostDomainProvider.DefaultNewDomainRegistryBasePath + "/RedundancyType";
    internal static readonly string Domain2RedundancyTypeRegistryPath = RegistryHostDomainProvider.DefaultDomain2RegistryBasePath + "/RedundancyType";
    internal static readonly string InstallFrameworkHostingAzureRegionRegistryPath = "/Configuration/Service/AzureRegion";
    private const string NotFound = "NotFound";
    private static readonly string TraceArea = "Blobstore";
    private static readonly string TraceLayer = nameof (RegistryHostDomainProvider);
    private static readonly string LegacyDomainIdString = WellKnownDomainIds.DefaultDomainId.Serialize();
    private static readonly string LegacyDomainRedundancyType = "RA_GRS";
    private bool disposedValue;

    public Task<bool> InitializeAsync(IVssRequestContext requestContext) => Task.FromResult<bool>(true);

    public Task<IMultiDomainInfo> GetDefaultDomainAsync(IVssRequestContext requestContext) => this.GetDomainAsync(requestContext, WellKnownDomainIds.DefaultDomainId);

    public async Task<IMultiDomainInfo> GetDomainAsync(
      IVssRequestContext requestContext,
      IDomainId domainId)
    {
      PhysicalDomainInfo physicalDomainAsync = await this.GetPhysicalDomainAsync(requestContext, domainId);
      return physicalDomainAsync == null ? (IMultiDomainInfo) await this.GetProjectDomainAsync(requestContext, domainId) : (IMultiDomainInfo) physicalDomainAsync;
    }

    public async Task<IEnumerable<IMultiDomainInfo>> GetDomainsAsync(
      IVssRequestContext requestContext)
    {
      List<IMultiDomainInfo> domains = new List<IMultiDomainInfo>();
      List<IMultiDomainInfo> multiDomainInfoList = domains;
      multiDomainInfoList.AddRange((IEnumerable<IMultiDomainInfo>) await this.GetPhysicalDomainsForAdminAsync(requestContext));
      multiDomainInfoList = (List<IMultiDomainInfo>) null;
      multiDomainInfoList = domains;
      multiDomainInfoList.AddRange((IEnumerable<IMultiDomainInfo>) await this.GetProjectDomainsForAdminAsync(requestContext));
      multiDomainInfoList = (List<IMultiDomainInfo>) null;
      IEnumerable<IMultiDomainInfo> domainsAsync = (IEnumerable<IMultiDomainInfo>) domains;
      domains = (List<IMultiDomainInfo>) null;
      return domainsAsync;
    }

    public async Task<IEnumerable<PhysicalDomainInfo>> GetPhysicalDomainsForAdminAsync(
      IVssRequestContext requestContext)
    {
      IVssRequestContext deploymentRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService deploymentRegistryService = deploymentRequestContext.GetService<IVssRegistryService>();
      PhysicalDomainInfo legacyDomain = await this.GetPhysicalDomainInfo(deploymentRegistryService, deploymentRequestContext, RegistryHostDomainProvider.DomainType.LegacyDomain);
      PhysicalDomainInfo newDomain = await this.GetPhysicalDomainInfo(deploymentRegistryService, deploymentRequestContext, RegistryHostDomainProvider.DomainType.NewDomain);
      PhysicalDomainInfo physicalDomainInfo = await this.GetPhysicalDomainInfo(deploymentRegistryService, deploymentRequestContext, RegistryHostDomainProvider.DomainType.Domain2);
      List<PhysicalDomainInfo> physicalDomainInfoList = new List<PhysicalDomainInfo>()
      {
        legacyDomain
      };
      if (newDomain != null)
        physicalDomainInfoList.Add(newDomain);
      if (physicalDomainInfo != null)
        physicalDomainInfoList.Add(physicalDomainInfo);
      IEnumerable<PhysicalDomainInfo> domainsForAdminAsync = (IEnumerable<PhysicalDomainInfo>) physicalDomainInfoList;
      deploymentRequestContext = (IVssRequestContext) null;
      deploymentRegistryService = (IVssRegistryService) null;
      legacyDomain = (PhysicalDomainInfo) null;
      newDomain = (PhysicalDomainInfo) null;
      return domainsForAdminAsync;
    }

    public async Task<IEnumerable<ProjectDomainInfo>> GetProjectDomainsForAdminAsync(
      IVssRequestContext requestContext)
    {
      List<ProjectDomainInfo> projectDomains = new List<ProjectDomainInfo>();
      if (requestContext.IsFeatureEnabled("Blobstore.Features.ProjectDomains"))
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        string domainPath = RegistryHostDomainProvider.GetDomainPath("*");
        IVssRequestContext requestContext1 = requestContext;
        // ISSUE: explicit reference operation
        ref RegistryQuery local = @(RegistryQuery) domainPath;
        foreach (RegistryItem entry in service.Read(requestContext1, in local))
        {
          List<ProjectDomainInfo> projectDomainInfoList = projectDomains;
          projectDomainInfoList.Add(await this.ConvertEntryToProjectDomainInfoAsync(requestContext, entry));
          projectDomainInfoList = (List<ProjectDomainInfo>) null;
        }
      }
      IEnumerable<ProjectDomainInfo> domainsForAdminAsync = (IEnumerable<ProjectDomainInfo>) projectDomains;
      projectDomains = (List<ProjectDomainInfo>) null;
      return domainsForAdminAsync;
    }

    private async Task<ProjectDomainInfo> ConvertEntryToProjectDomainInfoAsync(
      IVssRequestContext requestContext,
      RegistryItem entry)
    {
      int num = entry.Path.LastIndexOf('/');
      if (num < 0)
        throw new IndexOutOfRangeException("The domain information does not contain an ID at the correct Path.");
      ProjectDomainId domainId = (ProjectDomainId) DomainIdFactory.Create(entry.Path.Substring(num + 1));
      string containerName = entry.Value;
      return await this.GetProjectDomainInfoAsync(requestContext, domainId, containerName);
    }

    private async Task<ProjectDomainInfo> GetProjectDomainAsync(
      IVssRequestContext requestContext,
      IDomainId domainId)
    {
      if (!requestContext.IsFeatureEnabled("Blobstore.Features.ProjectDomains"))
        return (ProjectDomainInfo) null;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string domainPath = RegistryHostDomainProvider.GetDomainPath(domainId.Serialize());
      IVssRequestContext requestContext1 = requestContext;
      // ISSUE: explicit reference operation
      ref RegistryQuery local = @(RegistryQuery) domainPath;
      IEnumerable<RegistryItem> source = service.Read(requestContext1, in local);
      if (source.Count<RegistryItem>() > 1)
        throw new DuplicateDomainDataException(Microsoft.VisualStudio.Services.BlobStore.Server.Resources.DuplicateDomainIdError((object) domainId.Serialize()));
      return source.Count<RegistryItem>() == 0 ? (ProjectDomainInfo) null : await this.ConvertEntryToProjectDomainInfoAsync(requestContext, source.First<RegistryItem>());
    }

    private async Task<ProjectDomainInfo> GetProjectDomainInfoAsync(
      IVssRequestContext requestContext,
      ProjectDomainId domainId,
      string containerName)
    {
      if (domainId.ProjectId != Guid.Empty)
        return new ProjectDomainInfo((IDomainId) domainId, await this.GetPhysicalDomainAsync(requestContext, (IDomainId) domainId.PhysicalDomainId) ?? throw new InvalidDomainIdException(Microsoft.VisualStudio.Services.BlobStore.Server.Resources.InvalidProjectDomainIdError((object) domainId.Serialize())), RegistryHostDomainProvider.GetAssociatedProject(requestContext, domainId), containerName);
      requestContext.TraceAlways(5701700, TraceLevel.Error, RegistryHostDomainProvider.TraceArea, RegistryHostDomainProvider.TraceLayer, Microsoft.VisualStudio.Services.BlobStore.Server.Resources.InvalidProjectIdError((object) domainId.ProjectId));
      return await Task.FromResult<ProjectDomainInfo>((ProjectDomainInfo) null);
    }

    private static ProjectDomainInfo.AssociatedProject GetAssociatedProject(
      IVssRequestContext requestContext,
      ProjectDomainId domainId)
    {
      ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, domainId.ProjectId);
      if (project == null)
        return (ProjectDomainInfo.AssociatedProject) null;
      return new ProjectDomainInfo.AssociatedProject()
      {
        Id = domainId.ProjectId,
        Name = project.Name
      };
    }

    private async Task<PhysicalDomainInfo> GetPhysicalDomainAsync(
      IVssRequestContext requestContext,
      IDomainId domainId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      string str1 = service.GetValue(vssRequestContext, (RegistryQuery) RegistryHostDomainProvider.LegacyDomainIdRegistryPath, (string) null) ?? WellKnownDomainIds.DefaultDomainId.Serialize();
      string str2 = service.GetValue(vssRequestContext, (RegistryQuery) RegistryHostDomainProvider.NewDomainIdRegistryPath, (string) null);
      string str3 = service.GetValue(vssRequestContext, (RegistryQuery) RegistryHostDomainProvider.Domain2DomainIdRegistryPath, (string) null);
      if (domainId.Equals(DomainIdFactory.Create(str1)))
        return await this.GetPhysicalDomainInfo(service, vssRequestContext, RegistryHostDomainProvider.DomainType.LegacyDomain);
      if (str2 != null && domainId.Equals(DomainIdFactory.Create(str2)))
        return await this.GetPhysicalDomainInfo(service, vssRequestContext, RegistryHostDomainProvider.DomainType.NewDomain);
      return str3 != null && domainId.Equals(DomainIdFactory.Create(str3)) ? await this.GetPhysicalDomainInfo(service, vssRequestContext, RegistryHostDomainProvider.DomainType.Domain2) : (PhysicalDomainInfo) null;
    }

    private Task<PhysicalDomainInfo> GetPhysicalDomainInfo(
      IVssRegistryService registryService,
      IVssRequestContext requestContext,
      RegistryHostDomainProvider.DomainType domainType)
    {
      string str1 = registryService.GetValue(requestContext, (RegistryQuery) RegistryHostDomainProvider.InstallFrameworkHostingAzureRegionRegistryPath, false, (string) null);
      string legacyDomainIdString;
      string domainRedundancyType;
      string str2;
      string str3;
      string domainIdRegistryPath;
      string typeRegistryPath;
      string regionRegistryPath;
      string shardsRegistryPath;
      switch (domainType)
      {
        case RegistryHostDomainProvider.DomainType.LegacyDomain:
          legacyDomainIdString = RegistryHostDomainProvider.LegacyDomainIdString;
          domainRedundancyType = RegistryHostDomainProvider.LegacyDomainRedundancyType;
          str2 = str1;
          str3 = string.Join(",", StorageAccountConfigurationFacade.ReadAllStorageAccounts(requestContext.GetElevatedDeploymentRequestContext()).Select<StrongBoxConnectionString, string>((Func<StrongBoxConnectionString, string>) (s => StorageAccountUtilities.GetAccountInfo(s.ConnectionString).Name)));
          if (!requestContext.ServiceHost.IsProduction)
          {
            if (string.IsNullOrEmpty(str3))
              str3 = "blob1";
            legacyDomainIdString = registryService.GetValue(requestContext, (RegistryQuery) RegistryHostDomainProvider.LegacyDomainIdRegistryPath, legacyDomainIdString);
            domainRedundancyType = registryService.GetValue(requestContext, (RegistryQuery) RegistryHostDomainProvider.LegacyDomainRedundancyTypeRegistryPath, domainRedundancyType);
            str2 = registryService.GetValue(requestContext, (RegistryQuery) RegistryHostDomainProvider.LegacyDomainRegionRegistryPath, str2);
            str3 = registryService.GetValue(requestContext, (RegistryQuery) RegistryHostDomainProvider.LegacyDomainShardsRegistryPath, str3);
          }
          if (string.IsNullOrWhiteSpace(str3))
            throw new InvalidDomainShardListException("Shard list for legacy domain was missing from StrongBox");
          goto label_18;
        case RegistryHostDomainProvider.DomainType.NewDomain:
          domainIdRegistryPath = RegistryHostDomainProvider.NewDomainIdRegistryPath;
          typeRegistryPath = RegistryHostDomainProvider.NewDomainRedundancyTypeRegistryPath;
          regionRegistryPath = RegistryHostDomainProvider.NewDomainRegionRegistryPath;
          shardsRegistryPath = RegistryHostDomainProvider.NewDomainShardsRegistryPath;
          break;
        case RegistryHostDomainProvider.DomainType.Domain2:
          domainIdRegistryPath = RegistryHostDomainProvider.Domain2DomainIdRegistryPath;
          typeRegistryPath = RegistryHostDomainProvider.Domain2RedundancyTypeRegistryPath;
          regionRegistryPath = RegistryHostDomainProvider.Domain2RegionRegistryPath;
          shardsRegistryPath = RegistryHostDomainProvider.Domain2ShardsRegistryPath;
          break;
        default:
          throw new ArgumentException(string.Format("Invalid domain type {0}", (object) domainType));
      }
      legacyDomainIdString = registryService.GetValue(requestContext, (RegistryQuery) domainIdRegistryPath, "NotFound");
      if (legacyDomainIdString == "NotFound")
      {
        requestContext.TraceAlways(5701700, TraceLevel.Error, RegistryHostDomainProvider.TraceArea, RegistryHostDomainProvider.TraceLayer, "New domain could not be loaded because " + domainIdRegistryPath + " was missing from the registry.");
        return Task.FromResult<PhysicalDomainInfo>((PhysicalDomainInfo) null);
      }
      domainRedundancyType = registryService.GetValue(requestContext, (RegistryQuery) typeRegistryPath, "NotFound");
      if (domainRedundancyType == "NotFound")
      {
        requestContext.TraceAlways(5701700, TraceLevel.Error, RegistryHostDomainProvider.TraceArea, RegistryHostDomainProvider.TraceLayer, "New domain could not be loaded because " + typeRegistryPath + " was missing from the registry.");
        return Task.FromResult<PhysicalDomainInfo>((PhysicalDomainInfo) null);
      }
      str2 = registryService.GetValue(requestContext, (RegistryQuery) regionRegistryPath, "NotFound");
      if (str2 == "NotFound")
      {
        requestContext.TraceAlways(5701700, TraceLevel.Error, RegistryHostDomainProvider.TraceArea, RegistryHostDomainProvider.TraceLayer, "New domain could not be loaded because " + regionRegistryPath + " was missing from the registry. The deployment region is: " + str1);
        return Task.FromResult<PhysicalDomainInfo>((PhysicalDomainInfo) null);
      }
      str3 = registryService.GetValue(requestContext, (RegistryQuery) shardsRegistryPath, "NotFound");
      if (str3 == "NotFound")
      {
        requestContext.TraceAlways(5701700, TraceLevel.Error, RegistryHostDomainProvider.TraceArea, RegistryHostDomainProvider.TraceLayer, "New domain could not be loaded because " + shardsRegistryPath + " was missing from the registry.");
        return Task.FromResult<PhysicalDomainInfo>((PhysicalDomainInfo) null);
      }
label_18:
      bool isDefault = domainType == RegistryHostDomainProvider.DomainType.LegacyDomain;
      PhysicalDomainInfo domainInfo;
      Exception parseException;
      if (this.TryCreateAdminMultiDomainInfo(legacyDomainIdString, isDefault, domainRedundancyType, str2, str3, out domainInfo, out parseException))
        return Task.FromResult<PhysicalDomainInfo>(domainInfo);
      if (isDefault)
        parseException.ReThrow();
      else
        requestContext.TraceException(5701700, TraceLevel.Error, RegistryHostDomainProvider.TraceArea, RegistryHostDomainProvider.TraceLayer, parseException);
      return Task.FromResult<PhysicalDomainInfo>((PhysicalDomainInfo) null);
    }

    private bool TryCreateAdminMultiDomainInfo(
      string domainIdText,
      bool isDefault,
      string redundancyType,
      string region,
      string shardsList,
      out PhysicalDomainInfo domainInfo,
      out Exception parseException)
    {
      domainInfo = (PhysicalDomainInfo) null;
      parseException = (Exception) null;
      IDomainId domainId;
      try
      {
        domainId = DomainIdFactory.Create(domainIdText);
      }
      catch (Exception ex)
      {
        parseException = ex;
        return false;
      }
      HashSet<string> shards = new HashSet<string>(((IEnumerable<string>) shardsList.Split(',')).Select<string, string>((Func<string, string>) (p => p.Trim())));
      try
      {
        domainInfo = new PhysicalDomainInfo(domainId, isDefault, region, redundancyType, shards);
      }
      catch (Exception ex)
      {
        parseException = ex;
        return false;
      }
      return true;
    }

    public async Task<IMultiDomainInfo> CreateProjectDomainsForAdminAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      ByteDomainId physicalDomainId,
      bool isDelete,
      bool forceDelete)
    {
      PhysicalDomainInfo physicalDomainAsync = await this.GetPhysicalDomainAsync(requestContext, (IDomainId) physicalDomainId);
      if (physicalDomainAsync == null)
        throw new InvalidDomainIdException(Microsoft.VisualStudio.Services.BlobStore.Server.Resources.InvalidDomainIdError((object) physicalDomainId));
      ProjectDomainId projectDomainId = new ProjectDomainId(projectId, physicalDomainId);
      ProjectDomainInfo.AssociatedProject associatedProject = RegistryHostDomainProvider.GetAssociatedProject(requestContext, projectDomainId);
      string partitionKey = RegistryHostDomainProvider.GetPartitionKey();
      if (associatedProject == null)
        throw new InvalidProjectDomainIdException(Microsoft.VisualStudio.Services.BlobStore.Server.Resources.InvalidProjectIdError((object) projectId));
      ProjectDomainInfo domainsForAdminAsync = new ProjectDomainInfo((IDomainId) projectDomainId, physicalDomainAsync, associatedProject, partitionKey);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string domainPath1 = RegistryHostDomainProvider.GetDomainPath(projectDomainId.Serialize());
      if (service.Read(requestContext, (RegistryQuery) domainPath1).Count<RegistryItem>() >= 1)
        throw new ProjectDomainIdAlreadyExistsException(Microsoft.VisualStudio.Services.BlobStore.Server.Resources.ProjectDomainIdAlreadyExistsError((object) projectId, (object) physicalDomainId));
      string domainPath2 = RegistryHostDomainProvider.GetDomainPath(projectDomainId.Serialize());
      if (this.TryRegistrySetValue(service, requestContext, domainPath2, partitionKey, isDelete, forceDelete))
        return (IMultiDomainInfo) domainsForAdminAsync;
      throw new ProjectDomainCreationException(Microsoft.VisualStudio.Services.BlobStore.Server.Resources.CreateProjectDomainError((object) projectId, (object) physicalDomainId));
    }

    private static string GetDomainPath(string domainId) => "/Configuration/BlobStore/ProjectDomains/" + domainId;

    private static string GetPartitionKey() => "pdcontainer" + Guid.NewGuid().ConvertToAzureCompatibleString();

    private bool TryRegistrySetValue(
      IVssRegistryService registry,
      IVssRequestContext requestContext,
      string path,
      string value,
      bool isDelete,
      bool forceDelete)
    {
      string b = registry.GetValue(requestContext, (RegistryQuery) path, false, (string) null);
      if (b != null)
      {
        if (isDelete)
        {
          if (string.Equals(value, b, StringComparison.OrdinalIgnoreCase))
          {
            RegistryHostDomainProvider.TraceAlwaysOnRegistrySetValue(requestContext, TraceLevel.Info, "Per -isDelete, deleting registry path: " + path + " ...");
            registry.DeleteEntries(requestContext, path);
            return true;
          }
          if (forceDelete)
          {
            RegistryHostDomainProvider.TraceAlwaysOnRegistrySetValue(requestContext, TraceLevel.Info, "Per -isDelete and forceDelete, deleting registry path: " + path + " ...");
            registry.DeleteEntries(requestContext, path);
            return true;
          }
          RegistryHostDomainProvider.TraceAlwaysOnRegistrySetValue(requestContext, TraceLevel.Warning, "Aborting delete because registry contained a different value than expected at path " + path + ": Expected=" + value + " Actual=" + b);
          return false;
        }
        if (forceDelete)
        {
          RegistryHostDomainProvider.TraceAlwaysOnRegistrySetValue(requestContext, TraceLevel.Info, "Per -forceDelete, overwriting existing registry entry: Path=" + path + ", ExistingValue=" + b + ", NewValue=" + value);
          registry.SetValue<string>(requestContext, path, value);
          return true;
        }
        RegistryHostDomainProvider.TraceAlwaysOnRegistrySetValue(requestContext, TraceLevel.Warning, "Aborting set because registry contains an existing value at " + path + ": Existing=" + b);
        return false;
      }
      if (isDelete)
      {
        RegistryHostDomainProvider.TraceAlwaysOnRegistrySetValue(requestContext, TraceLevel.Warning, "Aborting delete because registry didn't contain an existing value at path " + path + ".");
        return false;
      }
      registry.SetValue<string>(requestContext, path, value);
      RegistryHostDomainProvider.TraceAlwaysOnRegistrySetValue(requestContext, TraceLevel.Info, "Project domain created successfully at path " + path + ".");
      return true;
    }

    private static void TraceAlwaysOnRegistrySetValue(
      IVssRequestContext requestContext,
      TraceLevel traceLevel,
      string TraceMessage)
    {
      requestContext.TraceAlways(5701701, traceLevel, RegistryHostDomainProvider.TraceArea, RegistryHostDomainProvider.TraceLayer, TraceMessage);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      int num = disposing ? 1 : 0;
      this.disposedValue = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private enum DomainType
    {
      LegacyDomain,
      NewDomain,
      Domain2,
    }
  }
}
