// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ClientToolReleaseQueryService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class ClientToolReleaseQueryService : IClientToolReleaseQueryService, IVssFrameworkService
  {
    protected virtual IClientToolStorageProvider StorageProvider { get; set; }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.Initialize(systemRequestContext);
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged), true, new RegistryQuery(ClientToolReleaseConstants.Registry.SettingsWildcard));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged));

    private void OnSettingsChanged(
      IVssRequestContext systemRequestContext,
      RegistryEntryCollection changedEntries)
    {
      this.Initialize(systemRequestContext);
    }

    private void Initialize(IVssRequestContext systemRequestContext)
    {
      string storageAccountName = systemRequestContext.GetService<IVssRegistryService>().GetValue(systemRequestContext, (RegistryQuery) ClientToolReleaseConstants.Registry.StorageAccountKey, true, (string) null);
      this.StorageProvider = (IClientToolStorageProvider) new AFDClientToolProvider(systemRequestContext, storageAccountName);
    }

    public ClientToolReleaseInfo GetRelease(
      IVssRequestContext requestContext,
      ClientTool toolName,
      RuntimeIdentifier runtimeId,
      EdgeCache edgeCache,
      string version = null)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      RegistryQuery registryQuery;
      if (version == null)
      {
        string currentPath = ClientToolReleaseRegistryUtil.GetCurrentPath(toolName, runtimeId);
        IVssRegistryService registryService = service;
        IVssRequestContext requestContext1 = requestContext;
        registryQuery = (RegistryQuery) currentPath;
        ref RegistryQuery local = ref registryQuery;
        version = registryService.GetValue(requestContext1, in local, true);
        if (version == null)
          return (ClientToolReleaseInfo) null;
      }
      string versionPath = ClientToolReleaseRegistryUtil.GetVersionPath(toolName, runtimeId, version);
      IVssRegistryService registryService1 = service;
      IVssRequestContext requestContext2 = requestContext;
      registryQuery = (RegistryQuery) versionPath;
      ref RegistryQuery local1 = ref registryQuery;
      string str = registryService1.GetValue(requestContext2, in local1, true);
      if (str == null)
        return (ClientToolReleaseInfo) null;
      PreauthenticatedUri? uri = this.StorageProvider.GenerateUri(requestContext, str, toolName, edgeCache);
      return !uri.HasValue ? (ClientToolReleaseInfo) null : new ClientToolReleaseInfo(toolName, runtimeId, version, str, uri.Value.NotNullUri);
    }
  }
}
