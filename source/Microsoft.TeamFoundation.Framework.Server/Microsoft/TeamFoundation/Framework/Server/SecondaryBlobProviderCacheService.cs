// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecondaryBlobProviderCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecondaryBlobProviderCacheService : 
    VssMemoryCacheService<BlobProviderKey, IBlobProvider>,
    ISecondaryBlobProviderCache,
    IVssFrameworkService
  {
    private string m_remoteBlobProviderType;
    private string m_remoteBlobProviderAssembly;
    private const string c_area = "SecondaryBlobProviderCacheService";
    private const string c_layer = "BlobStorage";
    private const int c_cleanupIntervalInMinutes = 30;
    private const int c_maxLength = 100000;

    public SecondaryBlobProviderCacheService()
      : base((IEqualityComparer<BlobProviderKey>) EqualityComparer<BlobProviderKey>.Default, new MemoryCacheConfiguration<BlobProviderKey, IBlobProvider>().WithCleanupInterval(TimeSpan.FromMinutes(30.0)).WithMaxElements(100000))
    {
    }

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      base.ServiceStart(requestContext);
      TeamFoundationFileService.TryGetBlobProviderInfo(requestContext, out this.m_remoteBlobProviderType, out this.m_remoteBlobProviderAssembly);
    }

    public bool TryGetProvider(
      IVssRequestContext requestContext,
      Guid hostId,
      string containerId,
      out IBlobProvider provider)
    {
      BlobProviderKey key = BlobProviderKey.Create(hostId, containerId);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      if (this.TryGetValue(vssRequestContext, key, out provider))
      {
        if (provider != null)
        {
          vssRequestContext.Trace(14603, TraceLevel.Info, nameof (SecondaryBlobProviderCacheService), "BlobStorage", "Found secondary provider for host={0}, container={1} (cached result)", (object) hostId, (object) containerId);
          return true;
        }
        vssRequestContext.Trace(14604, TraceLevel.Info, nameof (SecondaryBlobProviderCacheService), "BlobStorage", "No secondary provider for host={0}, container={1} (cached result)", (object) hostId, (object) containerId);
        return false;
      }
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(vssRequestContext, FrameworkServerConstants.HostMigrationSecretsDrawerName, false);
      if (drawerId != Guid.Empty)
      {
        foreach (StrongBoxItemInfo drawerContent in service.GetDrawerContents(vssRequestContext, drawerId))
        {
          Guid hostId1;
          Uri containerUri1;
          if (HostMigrationStrongBoxUtil.GetHostIdFromLookupKey(drawerContent.LookupKey, out hostId1) && object.Equals((object) hostId, (object) hostId1) && HostMigrationStrongBoxUtil.GetContainerUriFromLookupKey(drawerContent.LookupKey, out containerUri1))
          {
            string b = containerUri1.AbsolutePath;
            if (b.StartsWith("/", StringComparison.Ordinal))
              b = b.Substring(1);
            if (string.Equals(containerId, b, StringComparison.OrdinalIgnoreCase))
            {
              string storageCredentials = service.GetString(vssRequestContext, drawerContent);
              Uri containerUri2 = new Uri(containerUri1.GetLeftPart(UriPartial.Authority));
              provider = this.CreateProvider(requestContext, containerUri2, storageCredentials);
              this.TryAdd(vssRequestContext, key, provider);
              if (provider != null)
                vssRequestContext.TraceAlways(14605, TraceLevel.Info, nameof (SecondaryBlobProviderCacheService), "BlobStorage", "Created secondary provider for host={0}, container={1} at {2}", (object) hostId, (object) containerId, (object) containerUri2);
              return provider != null;
            }
          }
        }
      }
      vssRequestContext.Trace(14606, TraceLevel.Info, nameof (SecondaryBlobProviderCacheService), "BlobStorage", "No secondary provider for host={0}, container={1}", (object) hostId, (object) containerId);
      provider = (IBlobProvider) null;
      this.TryAdd(vssRequestContext, key, (IBlobProvider) null);
      return false;
    }

    public bool TryGetProvider(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid containerId,
      out IBlobProvider provider)
    {
      return this.TryGetProvider(requestContext, hostId, containerId.ToString("N"), out provider);
    }

    public bool TryGetProvider(
      IVssRequestContext requestContext,
      string containerId,
      out IBlobProvider provider)
    {
      return this.TryGetProvider(requestContext, requestContext.ServiceHost.InstanceId, containerId, out provider);
    }

    public bool TryGetProvider(
      IVssRequestContext requestContext,
      Guid containerId,
      out IBlobProvider provider)
    {
      return this.TryGetProvider(requestContext, requestContext.ServiceHost.InstanceId, containerId, out provider);
    }

    internal bool TryAddProvider(
      IVssRequestContext requestContext,
      Guid hostId,
      string containerId,
      IBlobProvider provider)
    {
      return this.TryAdd(requestContext, BlobProviderKey.Create(hostId, containerId), provider);
    }

    public virtual IBlobProvider CreateProvider(
      IVssRequestContext requestContext,
      Uri containerUri,
      string storageCredentials)
    {
      if (string.IsNullOrEmpty(this.m_remoteBlobProviderType))
      {
        requestContext.Trace(14607, TraceLevel.Error, nameof (SecondaryBlobProviderCacheService), "BlobStorage", "Unable to create secondary provider. FileServiceRemoteBlobProvider is not configured correctly");
        return (IBlobProvider) null;
      }
      provider = (IBlobProvider) null;
      Type type1 = Type.GetType(this.m_remoteBlobProviderType);
      if (type1 != (Type) null)
      {
        if (!(Activator.CreateInstance(type1) is IBlobProvider provider))
          requestContext.Trace(14449, TraceLevel.Warning, nameof (SecondaryBlobProviderCacheService), "BlobStorage", "Type '" + this.m_remoteBlobProviderType + "' does not implement IBlobProvider");
      }
      else
        requestContext.Trace(14448, TraceLevel.Warning, nameof (SecondaryBlobProviderCacheService), "BlobStorage", "Could not find type: " + this.m_remoteBlobProviderType);
      if (provider == null)
        provider = requestContext.GetExtension<IBlobProvider>((Func<IBlobProvider, bool>) (x =>
        {
          Type type2 = x.GetType();
          if (!type2.FullName.Equals(this.m_remoteBlobProviderType, StringComparison.Ordinal))
            return false;
          return string.IsNullOrEmpty(this.m_remoteBlobProviderAssembly) || this.m_remoteBlobProviderAssembly.Equals(type2.Assembly.GetName().Name, StringComparison.Ordinal);
        }));
      if (provider == null)
      {
        requestContext.Trace(14608, TraceLevel.Error, nameof (SecondaryBlobProviderCacheService), "BlobStorage", "Found no plugins implementing IBlobProvider when attempting to instantiate secondary providers");
        return (IBlobProvider) null;
      }
      provider.ServiceStart(requestContext, (IDictionary<string, string>) new Dictionary<string, string>()
      {
        ["BlobStorageUriOverride"] = containerUri.ToString(),
        ["BlobStorageCredentialsOverride"] = storageCredentials
      });
      return provider;
    }
  }
}
