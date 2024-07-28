// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CVS.CVSClient
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.Ops.Cvs.Client.DataContracts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.VisualStudio.Services.Gallery.Server.CVS
{
  internal class CVSClient : ICVSClient
  {
    private string m_certDrawerName;
    private string m_certLookupKey;
    private string m_apimKeyDrawerName;
    private string m_apimKeyLookupKey;
    private X509Certificate2 m_cvsAuthCert;
    private Uri m_ServerBaseUri;
    private string m_subscriptionKey;
    private const string s_legalText = "User information can be looked up on request. IP address is available for limited period from content publish. Please contact VSMarketplace@microsoft.com for more details.";
    private const string s_layer = "CVSClient";

    public CVSClient(
      IVssRequestContext requestContext,
      string certDrawerName,
      string certLookupKey,
      string apimKeyDrawerName,
      string apimKeyLookupKey)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSClient), nameof (CVSClient));
      this.m_certDrawerName = certDrawerName;
      this.m_certLookupKey = certLookupKey;
      this.m_apimKeyDrawerName = apimKeyDrawerName;
      this.m_apimKeyLookupKey = apimKeyLookupKey;
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      service.RegisterNotification(requestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxItemChanged), this.m_certDrawerName, (IEnumerable<string>) new string[1]
      {
        this.m_certLookupKey
      });
      service.RegisterNotification(requestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxItemChanged), this.m_apimKeyDrawerName, (IEnumerable<string>) new string[1]
      {
        this.m_apimKeyLookupKey
      });
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback), false, "/Configuration/Service/Gallery/PackageVerification/CVS/Server");
      this.LoadCVSConfiguration(requestContext);
      this.InitializeCVSCert(requestContext);
      this.InitializeCVSkey(requestContext);
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSClient), nameof (CVSClient));
    }

    public void Unload(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSClient), nameof (Unload));
      requestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(requestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxItemChanged));
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback));
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSClient), nameof (Unload));
    }

    public CVSScanResponse GetScanStatus(IVssRequestContext requestContext, string jobId)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSClient), nameof (GetScanStatus));
      CVSScanResponse scanStatus = (CVSScanResponse) null;
      using (ICvsServiceClient client = this.GetClient())
      {
        if (client != null)
          scanStatus = new CVSScanResponse(client.GetJobAsync(jobId).SyncResult<Job>());
      }
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSClient), nameof (GetScanStatus));
      return scanStatus;
    }

    public CVSScanResponse SubmitScan(
      IVssRequestContext requestContext,
      ContentType contentType,
      string path,
      string callbackURL,
      string externalId,
      IdentityPuid publisherPuid,
      DateTime? updateTime,
      bool isSynchronous)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSClient), nameof (SubmitScan));
      CVSScanResponse cvsScanResponse = (CVSScanResponse) null;
      ProcessingConfiguration processingConfiguration = this.GetProcessingConfiguration(requestContext, callbackURL, isSynchronous, publisherPuid);
      ContentItem contentItem = new ContentItem()
      {
        ExternalId = externalId,
        ContentType = contentType,
        Representation = (ContentRepresentation) 2,
        Value = path,
        IncidentTime = updateTime
      };
      if (publisherPuid != null && publisherPuid.PuidType == IdentityPuidType.Msa)
        contentItem.MsaPuidHex = publisherPuid.Value;
      using (ICvsServiceClient client = this.GetClient())
      {
        if (client != null)
          cvsScanResponse = new CVSScanResponse(client.CreateJobAsync(processingConfiguration, contentItem).SyncResult<Job>());
      }
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSClient), nameof (SubmitScan));
      return cvsScanResponse;
    }

    private ProcessingConfiguration GetProcessingConfiguration(
      IVssRequestContext requestContext,
      string callbackURL,
      bool isSynchronous,
      IdentityPuid publisherPuid)
    {
      string str = string.IsNullOrWhiteSpace(publisherPuid?.Value) ? string.Empty : "User Id: " + publisherPuid.Value;
      ProcessingConfiguration processingConfiguration = new ProcessingConfiguration();
      processingConfiguration.JobConfiguration = new JobConfiguration()
      {
        IsSynchronous = new bool?(isSynchronous),
        LegalInfo = "User information can be looked up on request. IP address is available for limited period from content publish. Please contact VSMarketplace@microsoft.com for more details." + str
      };
      processingConfiguration.TextScanConfiguration = new TextScanConfiguration()
      {
        Tier = (IEnumerable<int>) new List<int>() { 1 },
        EnableClassifier = new bool?(true)
      };
      processingConfiguration.ImageScanConfiguration = new ImageScanConfiguration();
      processingConfiguration.UrlScanConfiguration = new UrlScanConfiguration()
      {
        CheckHost = new bool?(true)
      };
      if (!isSynchronous)
      {
        processingConfiguration.JobConfiguration.CallbackEndpoint = callbackURL;
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.CVSEnableManualReview"))
          processingConfiguration.JobConfiguration.ReviewPreference = new ReviewPreference?((ReviewPreference) 1);
        processingConfiguration.TextScanConfiguration.ExtractUrl = new bool?(requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.CVSEnableUrlExtraction"));
        processingConfiguration.ImageScanConfiguration.EnableOcr = new bool?(requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.CVSEnableOCR"));
        processingConfiguration.UrlScanConfiguration.EnablePing = new bool?(true);
      }
      return processingConfiguration;
    }

    internal bool IsCertNull() => this.m_cvsAuthCert == null;

    internal bool IsAPIMKeyNull() => this.m_subscriptionKey == null;

    internal Uri GetServerUrl() => this.m_ServerBaseUri;

    private void ConfigurationChangeCallback(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadCVSConfiguration(requestContext);
    }

    private void LoadCVSConfiguration(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSClient), nameof (LoadCVSConfiguration));
      this.m_ServerBaseUri = (Uri) null;
      string uriString = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PackageVerification/CVS/Server", string.Empty);
      if (!string.IsNullOrWhiteSpace(uriString))
        this.m_ServerBaseUri = new Uri(uriString);
      requestContext.Trace(12061123, TraceLevel.Info, "gallery", nameof (LoadCVSConfiguration), "ServiceUri: {0}", (object) uriString);
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSClient), nameof (LoadCVSConfiguration));
    }

    private void OnStrongBoxItemChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSClient), nameof (OnStrongBoxItemChanged));
      if (itemNames.Any<StrongBoxItemName>((Func<StrongBoxItemName, bool>) (i => !i.LookupKey.EndsWith("-previous", StringComparison.OrdinalIgnoreCase))))
      {
        if (itemNames.Any<StrongBoxItemName>((Func<StrongBoxItemName, bool>) (i => i.LookupKey.Equals(this.m_certLookupKey, StringComparison.OrdinalIgnoreCase))))
          this.InitializeCVSCert(requestContext);
        if (itemNames.Any<StrongBoxItemName>((Func<StrongBoxItemName, bool>) (i => i.LookupKey.Equals(this.m_apimKeyLookupKey, StringComparison.OrdinalIgnoreCase))))
          this.InitializeCVSkey(requestContext);
      }
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSClient), nameof (OnStrongBoxItemChanged));
    }

    private void InitializeCVSCert(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSClient), nameof (InitializeCVSCert));
      this.m_cvsAuthCert = (X509Certificate2) null;
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext, this.m_certDrawerName, this.m_certLookupKey, false);
      if (itemInfo == null)
      {
        requestContext.Trace(12061123, TraceLevel.Info, "gallery", nameof (InitializeCVSCert), "No strongbox item info found.");
      }
      else
      {
        X509Certificate2 x509Certificate2 = service.RetrieveFileAsCertificate(requestContext, itemInfo.DrawerId, itemInfo.LookupKey);
        if (x509Certificate2 == null)
          requestContext.Trace(12061123, TraceLevel.Info, "gallery", nameof (InitializeCVSCert), "No certificate found.");
        else
          this.m_cvsAuthCert = x509Certificate2;
      }
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSClient), nameof (InitializeCVSCert));
    }

    private void InitializeCVSkey(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(12061123, "gallery", nameof (CVSClient), nameof (InitializeCVSkey));
      this.m_subscriptionKey = (string) null;
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext, this.m_apimKeyDrawerName, this.m_apimKeyLookupKey, false);
      if (itemInfo == null)
        requestContext.Trace(12061123, TraceLevel.Info, "gallery", nameof (InitializeCVSkey), "No strongbox item info found.");
      else
        this.m_subscriptionKey = service.GetString(requestContext, itemInfo.DrawerId, itemInfo.LookupKey);
      requestContext.TraceLeave(12061123, "gallery", nameof (CVSClient), nameof (InitializeCVSkey));
    }

    internal virtual ICvsServiceClient GetClient() => this.m_cvsAuthCert != null && this.m_ServerBaseUri != (Uri) null && !string.IsNullOrWhiteSpace(this.m_subscriptionKey) ? (ICvsServiceClient) new CvsServiceClient(this.m_ServerBaseUri, this.m_cvsAuthCert, this.m_subscriptionKey) : (ICvsServiceClient) null;
  }
}
