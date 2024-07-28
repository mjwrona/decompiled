// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityImage.IdentityImageCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Profile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.IdentityImage
{
  public class IdentityImageCacheService : 
    VssMemoryCacheService<Guid, ImageData>,
    IIdentityImageCache
  {
    private IVssMemoryCacheGrouping<Guid, ImageData, string> m_cacheIdentityIdGrouping;
    private const string RegistryRoot = "/WebAccess/IdentityImageCacheService";
    private const int s_defaultExpiryIntervalInMinutes = 30;
    private static readonly TimeSpan s_cacheCleanupInterval = new TimeSpan(0, 20, 0);
    private const int MaxCacheValues = 200000;
    private const string c_area = "Profile";
    private const string c_layer = "IdentityImageCache";

    public IdentityImageCacheService()
      : this(1000, TimeSpan.FromSeconds(10.0))
    {
    }

    public IdentityImageCacheService(int cleanupBatchSize, TimeSpan cleanupTimeout)
      : base((IEqualityComparer<Guid>) EqualityComparer<Guid>.Default, new MemoryCacheConfiguration<Guid, ImageData>().WithCleanupInterval(IdentityImageCacheService.s_cacheCleanupInterval))
    {
      this.ExpiryInterval.Value = TimeSpan.FromMinutes(30.0);
    }

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      base.ServiceStart(requestContext);
      this.m_cacheIdentityIdGrouping = VssMemoryCacheGroupingFactory.Create<Guid, ImageData, string>(requestContext, this.MemoryCache, (Func<Guid, ImageData, IEnumerable<string>>) ((key, value) => (IEnumerable<string>) new string[1]
      {
        value.DependencyKey
      }), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), true, "/WebAccess/IdentityImageCacheService/...");
      requestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(requestContext, "Default", SqlNotificationEventClasses.ProfileChanged, new SqlNotificationCallback(this.OnProfileImageChanged), true);
      this.ReadRegistrySettings(requestContext);
    }

    protected override void ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
      requestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(requestContext, "Default", SqlNotificationEventClasses.ProfileChanged, new SqlNotificationCallback(this.OnProfileImageChanged), false);
      base.ServiceEnd(requestContext);
    }

    internal void OnProfileImageChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      ProfileMessage profileMessage = ProfileMessage.FromXml(eventData);
      if (!(Guid.Empty != profileMessage.IdentityId) || !string.IsNullOrEmpty(profileMessage.ContainerName) && !StringComparer.OrdinalIgnoreCase.Equals(profileMessage.ContainerName, "avatar"))
        return;
      this.Remove(requestContext, profileMessage.IdentityId);
      if (profileMessage.IdentityDescriptor == (IdentityDescriptor) null)
      {
        requestContext.Trace(364272026, TraceLevel.Error, "Profile", "IdentityImageCache", "IdentityDescriptor missing in the ProfileMessage for identity {0}", (object) profileMessage.IdentityId);
      }
      else
      {
        string dependencyCacheKey = IdentityImageCacheProvider.GetDependencyCacheKey(profileMessage.IdentityDescriptor.IdentityType, profileMessage.IdentityDescriptor.Identifier);
        this.Remove(requestContext, dependencyCacheKey);
      }
    }

    private void ReadRegistrySettings(IVssRequestContext requestContext)
    {
      CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
      RegistryEntry registryEntry1 = service.ReadEntries(requestContext, (RegistryQuery) "/WebAccess/IdentityImageCacheService/ExpiryIntervalInMinutes").FirstOrDefault<RegistryEntry>();
      int num1 = registryEntry1 != null ? registryEntry1.GetValue<int>() : 30;
      RegistryEntry registryEntry2 = service.ReadEntries(requestContext, (RegistryQuery) "/WebAccess/IdentityImageCacheService/ExpiryDelayInHours").FirstOrDefault<RegistryEntry>();
      int num2 = registryEntry2 != null ? registryEntry2.GetValue<int>() : 0;
      RegistryEntryCollection source = service.ReadEntries(requestContext, (RegistryQuery) "/WebAccess/IdentityImageCacheService/maxCacheValues");
      if (((source != null ? source.FirstOrDefault<RegistryEntry>()?.GetValue<int>() : new int?()) ?? 200000) != this.MaxCacheLength.Value)
        this.MaxCacheLength.Value = 200000;
      this.ExpiryInterval.Value = TimeSpan.FromMinutes((double) num1);
      if (num2 <= 0)
        return;
      this.ExpiryDelay.Value = DateTime.UtcNow + TimeSpan.FromHours((double) num2);
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.ReadRegistrySettings(requestContext);
    }

    public bool TryGetValue(
      IVssRequestContext requestContext,
      Guid identityId,
      out bool isContainer,
      out Guid imageId)
    {
      ImageData imageData = (ImageData) null;
      if (this.TryGetValue(requestContext, identityId, out imageData))
      {
        isContainer = imageData.IsContainer;
        imageId = imageData.ImageId;
        return true;
      }
      isContainer = false;
      imageId = Guid.Empty;
      return false;
    }

    public bool Remove(IVssRequestContext requestContext, string dependencyKey)
    {
      IEnumerable<Guid> keys;
      if (!this.m_cacheIdentityIdGrouping.TryGetKeys(dependencyKey, out keys) || keys == null)
        return false;
      foreach (Guid key in keys)
        this.Remove(requestContext, key);
      return true;
    }

    public void Add(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      Guid imageId,
      string dependencyKey)
    {
      ImageData imageData = new ImageData()
      {
        DependencyKey = dependencyKey,
        ImageId = imageId,
        IsContainer = identity.IsContainer
      };
      this.TryAdd(requestContext, identity.Id, imageData);
    }
  }
}
