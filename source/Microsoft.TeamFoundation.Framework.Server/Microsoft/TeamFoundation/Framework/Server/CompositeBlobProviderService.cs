// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CompositeBlobProviderService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class CompositeBlobProviderService : ICompositeBlobProviderService, IVssFrameworkService
  {
    private ConcurrentDictionary<SecondaryBlobProvidersChanged, SecondaryBlobProvidersChanged> m_notifications;
    private const string c_area = "CompositeBlobProviderService";
    private const string c_layer = "BlobStorage";

    public void ServiceStart(IVssRequestContext systemContext)
    {
      systemContext.CheckDeploymentRequestContext();
      this.m_notifications = new ConcurrentDictionary<SecondaryBlobProvidersChanged, SecondaryBlobProvidersChanged>((IEqualityComparer<SecondaryBlobProvidersChanged>) DelegateComparer.Instance);
      systemContext.GetService<IVssRegistryService>().RegisterNotification(systemContext, new RegistrySettingsChangedCallback(this.OnOnlineBlobCopyEnabledChanged), true, FrameworkServerConstants.OnlineBlobCopyEnabled);
      systemContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(systemContext, new StrongBoxItemChangedCallback(this.OnStrongBoxSecondaryStorageItemChanged), FrameworkServerConstants.HostMigrationSecretsDrawerName, (IEnumerable<string>) new string[1]
      {
        "*"
      });
    }

    public void ServiceEnd(IVssRequestContext systemContext)
    {
      systemContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(systemContext, new StrongBoxItemChangedCallback(this.OnStrongBoxSecondaryStorageItemChanged));
      systemContext.GetService<IVssRegistryService>().UnregisterNotification(systemContext, new RegistrySettingsChangedCallback(this.OnOnlineBlobCopyEnabledChanged));
    }

    public IBlobProvider CreateCompositeBlobProvider(
      IVssRequestContext systemContext,
      IBlobProvider provider)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(systemContext, nameof (systemContext));
      ArgumentUtility.CheckForNull<IBlobProvider>(provider, nameof (provider));
      systemContext.CheckDeploymentRequestContext();
      systemContext.CheckSystemRequestContext();
      ICompositeBlobProviderFactory extension = systemContext.GetExtension<ICompositeBlobProviderFactory>();
      if (extension != null)
      {
        SecondaryBlobProviderCacheService service = systemContext.GetService<SecondaryBlobProviderCacheService>();
        provider = extension.CreateCompositeBlobProvider(provider, (ISecondaryBlobProviderCache) service);
        systemContext.Trace(14600, TraceLevel.Info, nameof (CompositeBlobProviderService), "BlobStorage", "Configured a composite provider");
      }
      else
        systemContext.Trace(14601, TraceLevel.Error, nameof (CompositeBlobProviderService), "BlobStorage", "Found no plugins implementing {0}", (object) "ICompositeBlobProviderFactory");
      return provider;
    }

    public bool HasSecondaryProviders(IVssRequestContext systemContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(systemContext, nameof (systemContext));
      systemContext.CheckDeploymentRequestContext();
      systemContext.CheckSystemRequestContext();
      if (!systemContext.GetService<IVssRegistryService>().GetValue<bool>(systemContext, (RegistryQuery) FrameworkServerConstants.OnlineBlobCopyEnabled, false))
        return false;
      ITeamFoundationStrongBoxService service = systemContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(systemContext, FrameworkServerConstants.HostMigrationSecretsDrawerName, false);
      return drawerId != Guid.Empty && !service.IsDrawerEmpty(systemContext, drawerId);
    }

    public void RegisterNotification(
      IVssRequestContext systemContext,
      SecondaryBlobProvidersChanged callback)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(systemContext, nameof (systemContext));
      ArgumentUtility.CheckForNull<SecondaryBlobProvidersChanged>(callback, nameof (callback));
      this.m_notifications.TryAdd(callback, callback);
    }

    public void UnregisterNotification(
      IVssRequestContext systemContext,
      SecondaryBlobProvidersChanged callback)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(systemContext, nameof (systemContext));
      ArgumentUtility.CheckForNull<SecondaryBlobProvidersChanged>(callback, nameof (callback));
      this.m_notifications.TryRemove(callback, out SecondaryBlobProvidersChanged _);
    }

    private void RemoveCachedBlobProviders(
      IVssRequestContext systemContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      SecondaryBlobProviderCacheService service = systemContext.GetService<SecondaryBlobProviderCacheService>();
      foreach (StrongBoxItemName itemName in itemNames)
      {
        Guid hostId;
        Uri containerUri;
        if (HostMigrationStrongBoxUtil.GetHostIdFromLookupKey(itemName.LookupKey, out hostId) && HostMigrationStrongBoxUtil.GetContainerUriFromLookupKey(itemName.LookupKey, out containerUri))
        {
          string containerId = containerUri.AbsolutePath;
          if (containerId.StartsWith("/", StringComparison.Ordinal))
            containerId = containerId.Substring(1);
          bool flag = service.Remove(systemContext, BlobProviderKey.Create(hostId, containerId));
          systemContext.Trace(14609, TraceLevel.Info, nameof (CompositeBlobProviderService), "BlobStorage", "Removing provider from cache (if any).  hostId={0}, containerId={1}, result={2}.", (object) hostId, (object) containerId, (object) flag);
        }
      }
    }

    private void OnStrongBoxSecondaryStorageItemChanged(
      IVssRequestContext systemContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      systemContext.TraceAlways(14602, TraceLevel.Info, nameof (CompositeBlobProviderService), "BlobStorage", "Strong box drawer for host migration credentials was modified.  Removing cached blob providers and invoking registered callback methods.");
      this.RemoveCachedBlobProviders(systemContext, itemNames);
      foreach (SecondaryBlobProvidersChanged key in (IEnumerable<SecondaryBlobProvidersChanged>) this.m_notifications.Keys)
        key(systemContext);
    }

    private void OnOnlineBlobCopyEnabledChanged(
      IVssRequestContext systemContext,
      RegistryEntryCollection changedEntries)
    {
      systemContext.TraceAlways(14618, TraceLevel.Info, nameof (CompositeBlobProviderService), "BlobStorage", FrameworkServerConstants.OnlineBlobCopyEnabled + " setting changed, invoking registered callback methods.");
      foreach (SecondaryBlobProvidersChanged key in (IEnumerable<SecondaryBlobProvidersChanged>) this.m_notifications.Keys)
        key(systemContext);
    }
  }
}
