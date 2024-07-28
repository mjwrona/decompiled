// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.BlobEdgeCachingService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Threading;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public abstract class BlobEdgeCachingService : IBlobEdgeCachingService, IVssFrameworkService
  {
    internal static readonly TimeSpan AzureFrontDoorSasUriExpiryBuffer = new TimeSpan(0, 5, 0);
    private BlobEdgeCachingService.ServiceSettings m_settings;
    private RegistryQuery m_settingRegistryQuery;
    private readonly ITimeProvider timeProvider;

    public abstract string RootRegistryPath { get; }

    internal string UrlSigningKeyIdRegistryPath => this.RootRegistryPath + "/UrlSigningKeyId";

    internal string HostSuffixRegistryPath => this.RootRegistryPath + "/HostSuffix";

    public abstract string UrlSigningKeySettingName { get; }

    public BlobEdgeCachingService()
      : this((ITimeProvider) new DefaultTimeProvider())
    {
    }

    public BlobEdgeCachingService(ITimeProvider timeProvider) => this.timeProvider = timeProvider;

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      this.m_settingRegistryQuery = BlobEdgeCachingService.GetSettingsRegistryQuery(this.RootRegistryPath);
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChange), true, in this.m_settingRegistryQuery);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(vssRequestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxChange), "ConfigurationSecrets", (IEnumerable<string>) new string[1]
      {
        this.UrlSigningKeySettingName
      });
      Interlocked.CompareExchange<BlobEdgeCachingService.ServiceSettings>(ref this.m_settings, new BlobEdgeCachingService.ServiceSettings(requestContext, this.m_settingRegistryQuery, this.UrlSigningKeySettingName), (BlobEdgeCachingService.ServiceSettings) null);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChange));
      requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(requestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxChange));
    }

    public Uri GetEdgeUri(Uri uri, DateTime expiry) => new EdgeCacheUrlBuilder(this.m_settings.HostSuffix, (IUrlSigner) new AzureFrontDoorUrlSigner(this.m_settings.UrlSigningKey, this.m_settings.UrlSigningKeyId)).Create(uri, expiry);

    public Uri GetEdgeUri(Uri uri)
    {
      DateTime expiry = this.timeProvider.Now + TimeSpan.FromMinutes((double) this.m_settings.DefaultUrlValidityDurationInMinutes);
      return this.GetEdgeUri(uri, expiry);
    }

    public Uri GetEdgeUri(Uri uri, TimeSpan duration)
    {
      DateTime expiry = this.timeProvider.Now + duration;
      return this.GetEdgeUri(uri, expiry);
    }

    public bool UserIsExcluded(IVssRequestContext requestContext)
    {
      if (!((IEnumerable<Guid>) this.m_settings.ExcludedVSIDs).Any<Guid>())
        return false;
      Guid userId = requestContext.GetUserId();
      requestContext.Trace(5700001, TraceLevel.Verbose, "BlobStore", "EdgeCaching", userId.ToString());
      return ((IEnumerable<Guid>) this.m_settings.ExcludedVSIDs).Contains<Guid>(userId);
    }

    private void OnRegistryChange(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadServiceSettings(requestContext);
    }

    private void OnStrongBoxChange(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      this.LoadServiceSettings(requestContext);
    }

    private void LoadServiceSettings(IVssRequestContext requestContext) => Volatile.Write<BlobEdgeCachingService.ServiceSettings>(ref this.m_settings, new BlobEdgeCachingService.ServiceSettings(requestContext, this.m_settingRegistryQuery, this.UrlSigningKeySettingName));

    private static RegistryQuery GetSettingsRegistryQuery(string rootRegistryPath)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(rootRegistryPath, nameof (rootRegistryPath));
      string str = "/*";
      if (!rootRegistryPath.EndsWith(str))
        rootRegistryPath += str;
      return new RegistryQuery(rootRegistryPath);
    }

    private class ServiceSettings
    {
      private const long DefaultDurationInMinutes = 240;
      private const string DefaultHostSuffix = "vsblob.vsassets.io";
      private static readonly string[] HostSuffixWhitelist = new string[4]
      {
        "vsblob.vsassets.io",
        "blob.core.windows.net",
        "vsblob.tfsallin.net",
        EdgeCacheUrlBuilder.BlobStorageEmulatorHostPort
      };
      public readonly string UrlSigningKeyId;
      public readonly SecureString UrlSigningKey;
      public readonly string HostSuffix;
      public readonly long DefaultUrlValidityDurationInMinutes;
      public readonly Guid[] ExcludedVSIDs;

      public ServiceSettings(
        IVssRequestContext requestContext,
        RegistryQuery settingsQuery,
        string urlSigningKeySettingName)
      {
        RegistryEntryCollection registryEntryCollection = !string.IsNullOrWhiteSpace(settingsQuery.Pattern) ? requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, in settingsQuery) : throw new ArgumentException("The settings registry query must be a wildcard query", nameof (settingsQuery));
        this.HostSuffix = registryEntryCollection.GetValueFromPath<string>(nameof (HostSuffix), "vsblob.vsassets.io");
        this.ValidateHostSuffix(this.HostSuffix);
        this.DefaultUrlValidityDurationInMinutes = registryEntryCollection.GetValueFromPath<long>(nameof (DefaultUrlValidityDurationInMinutes), 240L);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
        Guid drawerId = service.UnlockDrawer(vssRequestContext, "ConfigurationSecrets", true);
        this.UrlSigningKeyId = service.GetItemInfo(vssRequestContext, drawerId, urlSigningKeySettingName).CredentialName;
        this.UrlSigningKey = service.GetSecureString(vssRequestContext, drawerId, urlSigningKeySettingName);
        this.ExcludedVSIDs = ((IEnumerable<string>) registryEntryCollection.GetValueFromPath<string>(nameof (ExcludedVSIDs), "").Split(new char[1]
        {
          ';'
        }, StringSplitOptions.RemoveEmptyEntries)).Select<string, Guid>((Func<string, Guid>) (s => Guid.Parse(s))).ToArray<Guid>();
      }

      private void ValidateHostSuffix(string hostSuffix)
      {
        for (int index = 0; index < BlobEdgeCachingService.ServiceSettings.HostSuffixWhitelist.Length; ++index)
        {
          if (hostSuffix.Equals(BlobEdgeCachingService.ServiceSettings.HostSuffixWhitelist[index], StringComparison.InvariantCultureIgnoreCase))
            return;
        }
        throw new ArgumentOutOfRangeException(nameof (hostSuffix), (object) hostSuffix, "Host suffix was not in the acceptable whitelist");
      }
    }
  }
}
