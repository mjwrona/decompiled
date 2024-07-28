// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Hosting.AzureProviderSettingsService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Hosting
{
  internal class AzureProviderSettingsService : VssBaseService, IVssFrameworkService
  {
    private AzureProviderSettingsService m_deploymentService;
    private AzureProviderSettingsService.AzureProviderSettings m_blobProviderSettings;
    private static readonly RegistryQuery s_azureProviderSettingsRegistryQuery = new RegistryQuery("/Service/AzureProvider/Settings/*");
    private ILockName m_updateSettingsLock;
    private const string c_updateSettingsLockName = "azureProviderUpdateSettingsLock";
    private const string c_area = "FileService";
    private const string c_layer = "AzureProviderSettingsService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.Initialize(systemRequestContext);
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged), false, in AzureProviderSettingsService.s_azureProviderSettingsRegistryQuery);
      Interlocked.CompareExchange<AzureProviderSettingsService.AzureProviderSettings>(ref this.m_blobProviderSettings, new AzureProviderSettingsService.AzureProviderSettings(systemRequestContext), (AzureProviderSettingsService.AzureProviderSettings) null);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged));
    }

    private void Initialize(IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        this.m_deploymentService = this;
        this.m_updateSettingsLock = this.CreateLockName(requestContext, "azureProviderUpdateSettingsLock");
      }
      else
        this.m_deploymentService = requestContext.To(TeamFoundationHostType.Deployment).GetService<AzureProviderSettingsService>();
    }

    private void OnSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.TraceEnter(448263933, "FileService", nameof (AzureProviderSettingsService), nameof (OnSettingsChanged));
      try
      {
        this.m_blobProviderSettings.UpdateSettings(requestContext, this.m_updateSettingsLock);
      }
      finally
      {
        requestContext.TraceLeave(448263934, "FileService", nameof (AzureProviderSettingsService), nameof (OnSettingsChanged));
      }
    }

    internal AzureProviderSettingsService.AzureProviderSettings Settings => this.m_deploymentService.m_blobProviderSettings;

    internal class AzureProviderSettings : IAzureProviderSettings
    {
      public int Version;
      private static readonly TimeSpan s_downloadToStreamClientTimeoutFallThru = TimeSpan.FromSeconds(70.0);
      private static readonly TimeSpan s_enumerateBlobClientTimeoutFallThru = TimeSpan.FromSeconds(5.0);
      private static readonly TimeSpan s_fetchAttributesClientTimeoutFallThru = TimeSpan.FromSeconds(80.0);
      private static readonly TimeSpan s_getCloudBlobContainerClientTimeoutFallThru = TimeSpan.FromSeconds(5.0);
      private static readonly TimeSpan s_getStreamClientTimeoutFallThru = TimeSpan.FromSeconds(25.0);
      private static readonly TimeSpan s_deleteBlobClientTimeoutFallThru = TimeSpan.FromSeconds(15.0);
      private static readonly TimeSpan s_deleteContainerClientTimeoutFallThru = TimeSpan.FromSeconds(15.0);
      private static readonly TimeSpan s_putBlockClientTimeoutFallThru = TimeSpan.FromSeconds(45.0);
      private static readonly TimeSpan s_readBlobMetadataClientTimeoutFallThru = TimeSpan.FromSeconds(80.0);
      private static readonly TimeSpan s_readBlobPropertiesClientTimeoutFallThru = TimeSpan.FromSeconds(80.0);
      private static readonly TimeSpan s_renameBlobClientTimeoutFallThru = TimeSpan.FromSeconds(80.0);
      private static readonly TimeSpan s_writeBlobMetadataClientTimeoutFallThru = TimeSpan.FromSeconds(80.0);
      private static readonly TimeSpan s_blobExistsClientTimeoutFallThru = TimeSpan.FromSeconds(15.0);
      private static readonly TimeSpan s_getPageRangesClientTimeoutFallThru = TimeSpan.FromSeconds(80.0);
      private static readonly TimeSpan s_defaultBlobRequestClientTimeoutFallThru = TimeSpan.FromSeconds(3600.0);
      private static readonly TimeSpan s_defaultTableRequestClientTimeoutFallThru = TimeSpan.FromSeconds(3600.0);
      private static readonly TimeSpan s_defaultQueueRequestClientTimeoutFallThru = TimeSpan.FromSeconds(3600.0);
      private const int c_notificationThresholdFallThru = 5000;

      public AzureProviderSettings(IVssRequestContext requestContext)
      {
        requestContext.CheckDeploymentRequestContext();
        this.ApplySettingsFromRegistry(requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, AzureProviderSettingsService.s_azureProviderSettingsRegistryQuery));
        TeamFoundationTracingService.TraceRaw(1013183, TraceLevel.Info, "FileService", nameof (AzureProviderSettingsService), "AzureBlobProvider settings read; BlobTimeout {0}; TableTimeout {1}, QueueTimeout {2}, NotificationThreshold {3}", (object) this.DefaultBlobRequestClientTimeout, (object) this.DefaultTableRequestClientTimeout, (object) this.DefaultQueueRequestClientTimeout, (object) this.NotificationThreshold);
      }

      public void UpdateSettings(IVssRequestContext requestContext, ILockName lockName)
      {
        requestContext.CheckDeploymentRequestContext();
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        using (requestContext.Lock(lockName))
        {
          this.ApplySettingsFromRegistry(service.ReadEntries(requestContext, AzureProviderSettingsService.s_azureProviderSettingsRegistryQuery));
          TeamFoundationTracingService.TraceRaw(1013183, TraceLevel.Info, "FileService", nameof (AzureProviderSettingsService), "AzureBlobProvider settings read; BlobTimeout {0}; TableTimeout {1}, QueueTimeout {2}, NotificationThreshold {3}", (object) this.DefaultBlobRequestClientTimeout, (object) this.DefaultTableRequestClientTimeout, (object) this.DefaultQueueRequestClientTimeout, (object) this.NotificationThreshold);
        }
      }

      private void ApplySettingsFromRegistry(RegistryEntryCollection registryEntries)
      {
        this.DownloadToStreamClientTimeout = registryEntries.GetValueFromPath<TimeSpan>("DownloadToStreamClientTimeout", AzureProviderSettingsService.AzureProviderSettings.s_downloadToStreamClientTimeoutFallThru);
        this.EnumerateBlobsClientTimeout = registryEntries.GetValueFromPath<TimeSpan>("EnumerateBlobsClientTimeout", AzureProviderSettingsService.AzureProviderSettings.s_enumerateBlobClientTimeoutFallThru);
        this.FetchAttributesClientTimeout = registryEntries.GetValueFromPath<TimeSpan>("FetchAttributesClientTimeout", AzureProviderSettingsService.AzureProviderSettings.s_fetchAttributesClientTimeoutFallThru);
        this.GetCloudBlobContainerClientTimeout = registryEntries.GetValueFromPath<TimeSpan>("GetCloudBlobContainerClientTimeout", AzureProviderSettingsService.AzureProviderSettings.s_getCloudBlobContainerClientTimeoutFallThru);
        this.GetStreamClientTimeout = registryEntries.GetValueFromPath<TimeSpan>("GetStreamClientTimeout", AzureProviderSettingsService.AzureProviderSettings.s_getStreamClientTimeoutFallThru);
        this.DeleteBlobClientTimeout = registryEntries.GetValueFromPath<TimeSpan>("DeleteBlobClientTimeout", AzureProviderSettingsService.AzureProviderSettings.s_deleteBlobClientTimeoutFallThru);
        this.DeleteContainerClientTimeout = registryEntries.GetValueFromPath<TimeSpan>("DeleteContainerClientTimeout", AzureProviderSettingsService.AzureProviderSettings.s_deleteContainerClientTimeoutFallThru);
        this.PutBlockClientTimeout = registryEntries.GetValueFromPath<TimeSpan>("PutBlockClientTimeout", AzureProviderSettingsService.AzureProviderSettings.s_putBlockClientTimeoutFallThru);
        this.ReadBlobMetadataClientTimeout = registryEntries.GetValueFromPath<TimeSpan>("ReadBlobMetadataClientTimeout", AzureProviderSettingsService.AzureProviderSettings.s_readBlobMetadataClientTimeoutFallThru);
        this.ReadBlobPropertiesClientTimeout = registryEntries.GetValueFromPath<TimeSpan>("ReadBlobPropertiesClientTimeout", AzureProviderSettingsService.AzureProviderSettings.s_readBlobPropertiesClientTimeoutFallThru);
        this.RenameBlobClientTimeout = registryEntries.GetValueFromPath<TimeSpan>("RenameBlobClientTimeout", AzureProviderSettingsService.AzureProviderSettings.s_renameBlobClientTimeoutFallThru);
        this.WriteBlobMetadataClientTimeout = registryEntries.GetValueFromPath<TimeSpan>("WriteBlobMetadataClientTimeout", AzureProviderSettingsService.AzureProviderSettings.s_writeBlobMetadataClientTimeoutFallThru);
        this.BlobExistsClientTimeout = registryEntries.GetValueFromPath<TimeSpan>("BlobExistsClientTimeout", AzureProviderSettingsService.AzureProviderSettings.s_blobExistsClientTimeoutFallThru);
        this.GetPageRangesClientTimeout = registryEntries.GetValueFromPath<TimeSpan>("GetPageRangesClientTimeout", AzureProviderSettingsService.AzureProviderSettings.s_getPageRangesClientTimeoutFallThru);
        this.DefaultBlobRequestClientTimeout = registryEntries.GetValueFromPath<TimeSpan>("DefaultBlobRequestClientTimeout", AzureProviderSettingsService.AzureProviderSettings.s_defaultBlobRequestClientTimeoutFallThru);
        this.DefaultTableRequestClientTimeout = registryEntries.GetValueFromPath<TimeSpan>("DefaultTableRequestClientTimeout", AzureProviderSettingsService.AzureProviderSettings.s_defaultTableRequestClientTimeoutFallThru);
        this.DefaultQueueRequestClientTimeout = registryEntries.GetValueFromPath<TimeSpan>("DefaultQueueRequestClientTimeout", AzureProviderSettingsService.AzureProviderSettings.s_defaultQueueRequestClientTimeoutFallThru);
        this.NotificationThreshold = registryEntries.GetValueFromPath<int>("NotificationThreshold", 5000);
        Interlocked.Increment(ref this.Version);
      }

      public TimeSpan DownloadToStreamClientTimeout { get; private set; }

      public TimeSpan EnumerateBlobsClientTimeout { get; private set; }

      public TimeSpan FetchAttributesClientTimeout { get; private set; }

      public TimeSpan GetCloudBlobContainerClientTimeout { get; private set; }

      public TimeSpan GetStreamClientTimeout { get; private set; }

      public TimeSpan DeleteBlobClientTimeout { get; private set; }

      public TimeSpan DeleteContainerClientTimeout { get; private set; }

      public TimeSpan PutBlockClientTimeout { get; private set; }

      public TimeSpan ReadBlobMetadataClientTimeout { get; private set; }

      public TimeSpan ReadBlobPropertiesClientTimeout { get; private set; }

      public TimeSpan RenameBlobClientTimeout { get; private set; }

      public TimeSpan WriteBlobMetadataClientTimeout { get; private set; }

      public TimeSpan BlobExistsClientTimeout { get; private set; }

      public TimeSpan GetPageRangesClientTimeout { get; private set; }

      public TimeSpan DefaultBlobRequestClientTimeout { get; private set; }

      public TimeSpan DefaultTableRequestClientTimeout { get; private set; }

      public TimeSpan DefaultQueueRequestClientTimeout { get; private set; }

      public int NotificationThreshold { get; private set; }
    }
  }
}
