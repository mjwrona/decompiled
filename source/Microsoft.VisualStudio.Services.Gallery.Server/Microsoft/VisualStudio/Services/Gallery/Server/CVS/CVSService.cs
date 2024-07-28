// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CVS.CVSService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.Ops.Cvs.Client.DataContracts;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server.CVS
{
  internal class CVSService : PackageVerificationService, ICVSService, IVssFrameworkService
  {
    private GalleryBlobProvider m_cvsBlobProvider;
    private ICVSClient m_cvsClient;
    private string m_cvsContainerName;
    private bool m_isCallbackEnabled;
    private long m_cvsRecheckTime = 1800;
    private long m_cvsTimeout = 86400;
    private long m_retryCount = 2;
    private long m_blockedDuration = 1296000;
    private long m_blockedNotifyWindow = 172800;
    private const string s_layer = "CVSService";

    public CVSService()
    {
    }

    public CVSService(ICVSClient cvsClient) => this.m_cvsClient = cvsClient;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(12061123, "gallery", nameof (CVSService), nameof (ServiceStart));
      if (systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback), false, "/Configuration/Service/Gallery/PackageVerification/**");
        systemRequestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.StrongboxValueChangeCallback), "ConfigurationSecrets", (IEnumerable<string>) new string[1]
        {
          "GalleryPVStorageConnectionString"
        });
        this.LoadCVSConfiguration(systemRequestContext);
        this.LoadBlobStorageBaseUrl(systemRequestContext, 12061123, nameof (CVSService));
        this.m_cvsBlobProvider = new GalleryBlobProvider(systemRequestContext, "ConfigurationSecrets", "GalleryPVStorageConnectionString");
        this.m_cvsClient = this.m_cvsClient != null ? this.m_cvsClient : (ICVSClient) new CVSClient(systemRequestContext, "ConfigurationSecrets", "CVSAuthCertificate", "ConfigurationSecrets", "CVSAPIMKey");
      }
      systemRequestContext.TraceLeave(12061123, "gallery", nameof (CVSService), nameof (ServiceStart));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(12061123, "gallery", nameof (CVSService), nameof (ServiceEnd));
      if (systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback));
        systemRequestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.StrongboxValueChangeCallback));
        if (this.m_cvsBlobProvider != null)
          this.m_cvsBlobProvider.Unload(systemRequestContext);
        if (this.m_cvsClient != null)
          this.m_cvsClient.Unload(systemRequestContext);
      }
      systemRequestContext.TraceLeave(12061123, "gallery", nameof (CVSService), nameof (ServiceEnd));
    }

    public ScanItem SubmitStreamForScan(
      IVssRequestContext requestContext,
      Guid scanId,
      Stream stream,
      string itemDescription,
      ContentType cvsContentType,
      string externalId,
      IdentityPuid publisherPuid,
      DateTime? updateTime,
      bool isSynchronous = false)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSService), nameof (SubmitStreamForScan));
      ScanItem scanItem = (ScanItem) null;
      try
      {
        if (stream != null)
        {
          if (scanId != Guid.Empty)
          {
            if (this.m_cvsClient != null)
            {
              Guid itemId = Guid.NewGuid();
              if (!string.IsNullOrWhiteSpace(this.PrepareStreamForScan(requestContext, scanId, itemId, stream)))
              {
                scanItem = this.SubmitItemForScan(requestContext, scanId, itemId, itemDescription, cvsContentType, externalId, publisherPuid, updateTime, isSynchronous);
                if (scanItem != null)
                {
                  using (ScanItemComponent component = requestContext.CreateComponent<ScanItemComponent>())
                  {
                    if (component is ScanItemComponent1 scanItemComponent1)
                      scanItemComponent1.CreateScanItem(requestContext, scanItem);
                  }
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061123, "gallery", nameof (CVSService), ex);
        throw;
      }
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSService), nameof (SubmitStreamForScan));
      return scanItem;
    }

    public CVSScanResponse GetItemScanStatus(IVssRequestContext requestContext, string jobId)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSService), nameof (GetItemScanStatus));
      CVSScanResponse itemScanStatus = (CVSScanResponse) null;
      try
      {
        if (this.m_cvsClient != null)
          itemScanStatus = this.m_cvsClient.GetScanStatus(requestContext, jobId);
      }
      catch (Exception ex)
      {
        string format = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GetItemScanStatus jobId= {0} ErrorMessage= {1}", (object) jobId, (object) ex.Message);
        requestContext.TraceAlways(12061123, TraceLevel.Error, "gallery", nameof (CVSService), format);
        throw;
      }
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSService), nameof (GetItemScanStatus));
      return itemScanStatus;
    }

    public ScanItem UpdateItemScanStatus(
      IVssRequestContext requestContext,
      Guid scanId,
      Guid itemId,
      CVSScanResponse response)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSService), nameof (UpdateItemScanStatus));
      if (scanId.Equals(Guid.Empty))
        throw new ArgumentException(nameof (scanId));
      if (itemId.Equals(Guid.Empty))
        throw new ArgumentException(nameof (itemId));
      if (response == null)
        throw new ArgumentNullException(nameof (response));
      ScanItem scanItem = new ScanItem()
      {
        ScanId = scanId,
        ItemId = itemId,
        JobId = response.JobId,
        ValidationStatus = response.ValidationStatus
      };
      if (!response.Violations.IsNullOrEmpty<string>())
        scanItem.ResultMessage = string.Join(Environment.NewLine, response.Violations);
      if (!response.Faults.IsNullOrEmpty<string>() || !response.FailureLogs.IsNullOrEmpty<char>())
      {
        string faultDetails = (string) null;
        if (!response.Faults.IsNullOrEmpty<string>())
          faultDetails = string.Join(Environment.NewLine, response.Faults);
        this.LogFaultClientTraceLog(requestContext, scanItem, faultDetails, response.FailureLogs);
      }
      using (ScanItemComponent component = requestContext.CreateComponent<ScanItemComponent>())
      {
        if (component is ScanItemComponent1 scanItemComponent1)
          scanItem = scanItemComponent1.UpdateScanItem(requestContext, scanItem);
      }
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSService), nameof (UpdateItemScanStatus));
      return scanItem;
    }

    public IEnumerable<ScanItem> GetAllScanItems(IVssRequestContext requestContext, Guid scanId)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSService), nameof (GetAllScanItems));
      IEnumerable<ScanItem> allScanItems = (IEnumerable<ScanItem>) null;
      using (ScanItemComponent component = requestContext.CreateComponent<ScanItemComponent>())
      {
        if (component is ScanItemComponent1 scanItemComponent1)
          allScanItems = scanItemComponent1.GetAllScanItems(requestContext, scanId);
      }
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSService), nameof (GetAllScanItems));
      return allScanItems;
    }

    public void DeleteScanItemsByScanId(IVssRequestContext requestContext, Guid scanId)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSService), nameof (DeleteScanItemsByScanId));
      IEnumerable<ScanItem> allScanItems = this.GetAllScanItems(requestContext, scanId);
      if (!allScanItems.IsNullOrEmpty<ScanItem>())
      {
        foreach (ScanItem scanItem in allScanItems)
        {
          if (scanItem.ContentType != 2)
            this.CleanupBlobstore(requestContext, scanId, scanItem.ItemId);
        }
        using (ScanItemComponent component = requestContext.CreateComponent<ScanItemComponent>())
        {
          if (component is ScanItemComponent1 scanItemComponent1)
            scanItemComponent1.DeleteScanItemsByScanId(requestContext, scanId);
        }
      }
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSService), nameof (DeleteScanItemsByScanId));
    }

    public IEnumerable<ScanViolationItem> GetScanViolations(
      IVssRequestContext requestContext,
      IEnumerable<Guid> extensionIds = null)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSService), nameof (GetScanViolations));
      IEnumerable<ScanViolationItem> scanViolations = (IEnumerable<ScanViolationItem>) null;
      if (extensionIds == null)
        extensionIds = (IEnumerable<Guid>) new List<Guid>();
      using (ScanItemComponent component = requestContext.CreateComponent<ScanItemComponent>())
      {
        if (component is ScanItemComponent2 scanItemComponent2)
          scanViolations = scanItemComponent2.GetScanViolationItems(extensionIds);
      }
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSService), nameof (GetScanViolations));
      return scanViolations;
    }

    public void UpdateScanViolation(
      IVssRequestContext requestContext,
      ScanViolationItem scanViolation)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSService), nameof (UpdateScanViolation));
      using (ScanItemComponent component = requestContext.CreateComponent<ScanItemComponent>())
      {
        if (component is ScanItemComponent2 scanItemComponent2)
          scanItemComponent2.CreateorUpdateScanViolationItem(scanViolation);
      }
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSService), nameof (UpdateScanViolation));
    }

    public int DeleteScanViolations(
      IVssRequestContext requestContext,
      IEnumerable<Guid> extensionIds = null)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSService), nameof (DeleteScanViolations));
      int num = 0;
      using (ScanItemComponent component = requestContext.CreateComponent<ScanItemComponent>())
      {
        if (component is ScanItemComponent2 scanItemComponent2)
          num = scanItemComponent2.DeleteScanViolationItems(extensionIds);
      }
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSService), nameof (DeleteScanViolations));
      return num;
    }

    public bool IsScanTimeOut(IVssRequestContext requestContext, DateTime startTime)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSService), nameof (IsScanTimeOut));
      bool flag = false;
      if (Convert.ToInt64(DateTime.UtcNow.Subtract(startTime).TotalSeconds) > this.m_cvsTimeout)
        flag = true;
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSService), nameof (IsScanTimeOut));
      return flag;
    }

    public bool IsViolationNotifiableToMarketplace(
      IVssRequestContext requestContext,
      DateTime contactTime)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSService), "IsViolationNotifiableToMarketpalce");
      bool marketplace = false;
      long int64 = Convert.ToInt64(DateTime.UtcNow.Subtract(contactTime).TotalSeconds);
      if (int64 > this.m_blockedDuration && int64 < this.m_blockedDuration + this.m_blockedNotifyWindow)
        marketplace = true;
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSService), "IsViolationNotifiableToMarketpalce");
      return marketplace;
    }

    public TimeSpan GetRecheckTimespan(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSService), nameof (GetRecheckTimespan));
      TimeSpan recheckTimespan = TimeSpan.FromSeconds((double) this.m_cvsRecheckTime);
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSService), nameof (GetRecheckTimespan));
      return recheckTimespan;
    }

    public ScanItem SubmitItemForScan(
      IVssRequestContext requestContext,
      Guid scanId,
      Guid itemId,
      string itemDescription,
      ContentType cvsContentType,
      string externalId,
      IdentityPuid publisherPuid,
      DateTime? updateTime,
      bool isSynchronous = false)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSService), nameof (SubmitItemForScan));
      ScanItem scanItem = (ScanItem) null;
      try
      {
        if (scanId != Guid.Empty)
        {
          if (itemId != Guid.Empty)
          {
            if (this.m_cvsClient != null)
            {
              string absoluteUri = new Uri(this.m_blobStoreBaseURL, this.m_cvsContainerName + "/" + this.GetBlobResourcePath(scanId, itemId)).AbsoluteUri;
              if (!string.IsNullOrWhiteSpace(absoluteUri))
              {
                string callbackUrl = this.GetCallbackUrl(requestContext, scanId, itemId);
                CVSScanResponse cvsScanResponse = this.m_cvsClient.SubmitScan(requestContext, cvsContentType, absoluteUri, callbackUrl, externalId, publisherPuid, updateTime, isSynchronous);
                if (cvsScanResponse != null)
                {
                  scanItem = new ScanItem()
                  {
                    ContentType = (int) cvsContentType,
                    ScanId = scanId,
                    ItemId = itemId,
                    Description = itemDescription,
                    ValidationStatus = cvsScanResponse.ValidationStatus,
                    JobId = cvsScanResponse.JobId
                  };
                  if (!cvsScanResponse.Violations.IsNullOrEmpty<string>())
                    scanItem.ResultMessage = string.Join(Environment.NewLine, cvsScanResponse.Violations);
                  if (cvsScanResponse.Faults.IsNullOrEmpty<string>())
                  {
                    if (cvsScanResponse.FailureLogs.IsNullOrEmpty<char>())
                      goto label_14;
                  }
                  string faultDetails = (string) null;
                  if (!cvsScanResponse.Faults.IsNullOrEmpty<string>())
                    faultDetails = string.Join(Environment.NewLine, cvsScanResponse.Faults);
                  this.LogFaultClientTraceLog(requestContext, scanItem, faultDetails, cvsScanResponse.FailureLogs);
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061123, "gallery", nameof (CVSService), ex);
        throw;
      }
label_14:
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSService), nameof (SubmitItemForScan));
      return scanItem;
    }

    public bool IsCallbackEnabled() => this.m_isCallbackEnabled;

    public long GetRetryCount() => this.m_retryCount;

    public DateTime GetViolationTimeOutDate(DateTime contactDate) => contactDate.AddSeconds((double) this.m_blockedDuration);

    private string GetCallbackUrl(IVssRequestContext requestContext, Guid scanId, Guid itemId)
    {
      string callbackUrl = (string) null;
      if (this.m_isCallbackEnabled)
      {
        ILocationService service = requestContext.GetService<ILocationService>();
        Guid cvsLocationId = GalleryResourceIds.CVSLocationId;
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary[nameof (scanId)] = (object) scanId;
        dictionary[nameof (itemId)] = (object) itemId;
        dictionary["api-version"] = (object) "4.1-preview";
        IVssRequestContext requestContext1 = requestContext;
        Guid identifier = cvsLocationId;
        Dictionary<string, object> routeValues = dictionary;
        callbackUrl = service.GetResourceUri(requestContext1, "gallery", identifier, (object) routeValues, true, false).AbsoluteUri;
      }
      return callbackUrl;
    }

    private string PrepareStreamForScan(
      IVssRequestContext requestContext,
      Guid scanId,
      Guid itemId,
      Stream stream)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSService), nameof (PrepareStreamForScan));
      string str = (string) null;
      if (this.m_cvsBlobProvider != null && this.m_cvsBlobProvider.IsValidBlobProvider() && stream != null && this.m_blobStoreBaseURL != (Uri) null && scanId != Guid.Empty && itemId != Guid.Empty)
      {
        stream.Seek(0L, SeekOrigin.Begin);
        string blobResourcePath = this.GetBlobResourcePath(scanId, itemId);
        this.m_cvsBlobProvider.PutStream(requestContext, this.m_cvsContainerName, blobResourcePath, stream, (IDictionary<string, string>) null);
        str = new Uri(this.m_blobStoreBaseURL, this.m_cvsContainerName + "/" + blobResourcePath).AbsoluteUri;
      }
      else
        requestContext.Trace(12061123, TraceLevel.Info, "gallery", nameof (PrepareStreamForScan), "Blob not uploaded to CVS blob store");
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSService), nameof (PrepareStreamForScan));
      return str;
    }

    private void CleanupBlobstore(IVssRequestContext requestContext, Guid scanId, Guid itemId)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSService), "SubmitStreamForScan");
      if (this.m_cvsBlobProvider != null && this.m_cvsBlobProvider.IsValidBlobProvider() && this.m_blobStoreBaseURL != (Uri) null && scanId != Guid.Empty && itemId != Guid.Empty)
      {
        string blobResourcePath = this.GetBlobResourcePath(scanId, itemId);
        this.m_cvsBlobProvider.DeleteBlob(requestContext, this.m_cvsContainerName, blobResourcePath);
      }
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSService), "SubmitStreamForScan");
    }

    private void ConfigurationChangeCallback(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadCVSConfiguration(requestContext);
    }

    private void StrongboxValueChangeCallback(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      this.LoadBlobStorageBaseUrl(requestContext, 12061123, nameof (CVSService));
    }

    private void LoadCVSConfiguration(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSService), nameof (LoadCVSConfiguration));
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PackageVerification/**");
      this.m_cvsContainerName = registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/BlobContainer", "packageverification");
      this.m_cvsRecheckTime = long.Parse(registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/CVS/Times/CVSRecheckTime", 1800L.ToString((IFormatProvider) CultureInfo.InvariantCulture)), (IFormatProvider) CultureInfo.InvariantCulture);
      this.m_retryCount = long.Parse(registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/CVS/CVSRetryCountpath", 2L.ToString((IFormatProvider) CultureInfo.InvariantCulture)), (IFormatProvider) CultureInfo.InvariantCulture);
      this.m_cvsTimeout = long.Parse(registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/CVS/Times/CVSTimeout", 86400L.ToString((IFormatProvider) CultureInfo.InvariantCulture)), (IFormatProvider) CultureInfo.InvariantCulture);
      this.m_blockedDuration = long.Parse(registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/CVS/Times/CVSBlockedDuration ", 1296000L.ToString((IFormatProvider) CultureInfo.InvariantCulture)), (IFormatProvider) CultureInfo.InvariantCulture);
      this.m_blockedNotifyWindow = long.Parse(registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/CVS/Times/CVSBlockedNotifyWindow", 172800L.ToString((IFormatProvider) CultureInfo.InvariantCulture)), (IFormatProvider) CultureInfo.InvariantCulture);
      this.m_isCallbackEnabled = registryEntryCollection.GetValueFromPath<bool>("/Configuration/Service/Gallery/PackageVerification/CVS/CallbackEnabled", true);
      requestContext.Trace(12061123, TraceLevel.Info, "gallery", nameof (LoadCVSConfiguration), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Blob Container Name: {0}", (object) this.m_cvsContainerName));
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSService), nameof (LoadCVSConfiguration));
    }

    private string GetBlobResourcePath(Guid scanId, Guid itemId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/cvs/{1}", (object) scanId, (object) itemId);

    private bool IsScanComplete(ValidationStatus validationStatus) => validationStatus == ValidationStatus.Success || validationStatus == ValidationStatus.Failure;

    private void LogFaultClientTraceLog(
      IVssRequestContext requestContext,
      ScanItem scanItem,
      string faultDetails,
      string failureLogs)
    {
      ClientTraceData properties = new ClientTraceData();
      properties.Add("faults", (object) faultDetails);
      properties.Add("scanId", (object) scanItem.ScanId);
      properties.Add("itemId", (object) scanItem.ItemId);
      properties.Add("jobId", (object) scanItem.JobId);
      properties.Add("description", (object) scanItem.Description);
      properties.Add(nameof (failureLogs), (object) failureLogs);
      requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "CVS", properties);
    }
  }
}
