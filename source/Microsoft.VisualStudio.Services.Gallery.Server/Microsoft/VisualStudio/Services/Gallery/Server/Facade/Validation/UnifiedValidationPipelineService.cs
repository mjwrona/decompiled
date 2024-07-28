// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.Validation.UnifiedValidationPipelineService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Newtonsoft.Json.Linq;
using NuGet.Services.ServiceBus;
using NuGet.Services.Validation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.Validation
{
  public class UnifiedValidationPipelineService : 
    PackageVerificationService,
    IUnifiedValidationPipelineService,
    IVssFrameworkService
  {
    private IValidationRequestEnqueuer _validationRequestEnqueuer;
    private ITopicClient _topicClient;
    private GalleryBlobProvider _uvpBlobProvider;
    private const string Layer = "UnifiedValidationPipelineService";
    private string _uvpContainerName = "unifiedpackageverification";
    private long _uvpRetryCount = 3;
    private long _uvpDelayTime = 2;
    private string _serviceBusTopicName = "validation";

    public UnifiedValidationPipelineService()
    {
    }

    public UnifiedValidationPipelineService(
      IValidationRequestEnqueuer validationRequestEnqueuer)
    {
      this._validationRequestEnqueuer = validationRequestEnqueuer;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(12062081, "gallery", nameof (UnifiedValidationPipelineService), nameof (ServiceStart));
      if (systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback), false, "/Configuration/Service/Gallery/PackageVerification/**");
        systemRequestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.StrongboxValueChangeCallback), "ConfigurationSecrets", (IEnumerable<string>) new string[1]
        {
          "UnifiedValidationPipelineSBConnectionString"
        });
        this.LoadUnifiedValidationPipelineConfiguration(systemRequestContext);
        this.LoadServiceBusConfiguration(systemRequestContext);
        this.LoadBlobStorageBaseUrl(systemRequestContext, 12062081, nameof (UnifiedValidationPipelineService));
        this._uvpBlobProvider = new GalleryBlobProvider(systemRequestContext, "ConfigurationSecrets", "GalleryPVStorageConnectionString");
      }
      systemRequestContext.TraceLeave(12062081, "gallery", nameof (UnifiedValidationPipelineService), nameof (ServiceStart));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(12062081, "gallery", nameof (UnifiedValidationPipelineService), nameof (ServiceEnd));
      if (systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback));
        systemRequestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.StrongboxValueChangeCallback));
        this._uvpBlobProvider?.Unload(systemRequestContext);
      }
      systemRequestContext.TraceLeave(12061123, "gallery", nameof (UnifiedValidationPipelineService), nameof (ServiceEnd));
    }

    public void SendMessage(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Guid validationId)
    {
      string uriString = this.PrepareUvpBlobUrl(requestContext, extension, validationId);
      if (!string.IsNullOrWhiteSpace(uriString))
      {
        PackageValidationMessageData packageValidationMessage = PackageValidationMessageData.NewStartValidation(validationId, UnifiedValidationPipelineService.GetExtensionType(extension), new Uri(uriString), new JObject());
        if (this._validationRequestEnqueuer != null)
          this.PerformWithRetry(requestContext, packageValidationMessage);
        else
          requestContext.Trace(12062081, TraceLevel.Info, "gallery", nameof (SendMessage), "validation enqueuer is null");
      }
      else
      {
        requestContext.Trace(12062081, TraceLevel.Info, "gallery", nameof (SendMessage), "Blob path null or empty");
        UnifiedValidationPipelineService.LogFaultClientTraceLog(requestContext, (PackageValidationMessageData) null, "Blob path null or empty", validationId);
      }
    }

    private void PerformWithRetry(
      IVssRequestContext requestContext,
      PackageValidationMessageData packageValidationMessage)
    {
      int num = 0;
      while (true)
      {
        try
        {
          this._validationRequestEnqueuer.SendMessage(packageValidationMessage);
          break;
        }
        catch (Exception ex)
        {
          ++num;
          requestContext.Trace(12062081, TraceLevel.Info, "gallery", nameof (PerformWithRetry), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to publish request to service bus validationId {0} retry count {1} exception {2}", (object) packageValidationMessage.StartValidation.ValidationTrackingId, (object) num, (object) ex.ToString()));
          if ((long) num > this._uvpRetryCount)
          {
            requestContext.Trace(12062081, TraceLevel.Info, "gallery", nameof (PerformWithRetry), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "All {0} retries exhaust unable to publish request to service bus request {1}", (object) this._uvpRetryCount, (object) packageValidationMessage.StartValidation.ValidationTrackingId));
            UnifiedValidationPipelineService.LogFaultClientTraceLog(requestContext, packageValidationMessage, "All retries exhaust", packageValidationMessage.StartValidation.ValidationTrackingId, ex);
            break;
          }
        }
        Thread.Sleep(TimeSpan.FromSeconds((double) this._uvpDelayTime));
      }
    }

    private void LoadUnifiedValidationPipelineConfiguration(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(12062081, "gallery", nameof (UnifiedValidationPipelineService), nameof (LoadUnifiedValidationPipelineConfiguration));
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PackageVerification/**");
      this._uvpContainerName = registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/UVP/BlobContainer", "unifiedpackageverification");
      this._uvpRetryCount = (long) registryEntryCollection.GetValueFromPath<int>("/Configuration/Service/Gallery/PackageVerification/UVP/Times/UVPRetryCount", 3);
      this._uvpDelayTime = (long) registryEntryCollection.GetValueFromPath<int>("/Configuration/Service/Gallery/PackageVerification/UVP/Times/UVPDelayTime", 2);
      this._serviceBusTopicName = registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/UVP/UVPServiceBusTopicNamePath", "validation");
      requestContext.Trace(12062081, TraceLevel.Info, "gallery", nameof (LoadUnifiedValidationPipelineConfiguration), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Blob Container Name: {0}", (object) this._uvpContainerName));
      requestContext.TraceLeave(12062081, "gallery", nameof (UnifiedValidationPipelineService), nameof (LoadUnifiedValidationPipelineConfiguration));
    }

    private void LoadServiceBusConfiguration(IVssRequestContext systemRequestContext)
    {
      ITeamFoundationStrongBoxService service = systemRequestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(systemRequestContext, "ConfigurationSecrets", "UnifiedValidationPipelineSBConnectionString", false);
      if (itemInfo == null)
      {
        systemRequestContext.Trace(12062081, TraceLevel.Info, "gallery", "ServiceStart", "No strongbox item info found.");
      }
      else
      {
        string str = service.GetString(systemRequestContext, itemInfo);
        if (string.IsNullOrWhiteSpace(str))
        {
          systemRequestContext.Trace(12062081, TraceLevel.Info, "gallery", nameof (LoadServiceBusConfiguration), "No connection string found.");
        }
        else
        {
          this._topicClient = (ITopicClient) new TopicClientWrapper(str, this._serviceBusTopicName);
          this._validationRequestEnqueuer = (IValidationRequestEnqueuer) new ValidationRequestEnqueuer(this._topicClient);
        }
      }
    }

    private string PrepareUvpBlobUrl(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Guid validationId)
    {
      Stream packageStream = UnifiedValidationPipelineService.GetPackageStream(requestContext, extension);
      return this.UploadPackageOnBlobStorage(requestContext, extension.ExtensionId, validationId, packageStream);
    }

    private string UploadPackageOnBlobStorage(
      IVssRequestContext requestContext,
      Guid extensionId,
      Guid validationId,
      Stream stream)
    {
      string str = (string) null;
      if (this._uvpBlobProvider != null && this._uvpBlobProvider.IsValidBlobProvider() && stream != null && this.m_blobStoreBaseURL != (Uri) null && extensionId != Guid.Empty && validationId != Guid.Empty)
      {
        stream.Seek(0L, SeekOrigin.Begin);
        string blobResourcePath = UnifiedValidationPipelineService.GetBlobResourcePath(extensionId, validationId);
        this._uvpBlobProvider.PutStream(requestContext, this._uvpContainerName, blobResourcePath, stream, (IDictionary<string, string>) null);
        str = new Uri(this.m_blobStoreBaseURL, this._uvpContainerName + "/" + blobResourcePath).AbsoluteUri;
      }
      else
        requestContext.Trace(12062081, TraceLevel.Info, "gallery", "PrepareStreamForScan", "Blob not uploaded to UVP blob store");
      requestContext.TraceLeave(12062081, "gallery", nameof (UnifiedValidationPipelineService), "PrepareStreamForScan");
      return str;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is comming for tracepoints so ignoring")]
    private static Stream GetPackageStream(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      ExtensionFile extensionFile1 = (ExtensionFile) null;
      if (extension?.Versions[0]?.Files != null)
      {
        using (IEnumerator<ExtensionFile> enumerator = extension.Versions[0].Files.Where<ExtensionFile>((Func<ExtensionFile, bool>) (extensionFile => extensionFile.AssetType.Equals("Microsoft.VisualStudio.Services.VSIXPackage"))).GetEnumerator())
        {
          if (enumerator.MoveNext())
            extensionFile1 = enumerator.Current;
        }
        if (extensionFile1 == null)
        {
          using (IEnumerator<ExtensionFile> enumerator = extension.Versions[0].Files.Where<ExtensionFile>((Func<ExtensionFile, bool>) (extensionFile => extensionFile.AssetType.Equals("Microsoft.VisualStudio.Ide.Payload", StringComparison.OrdinalIgnoreCase) || extensionFile.AssetType.EndsWith("VSIX", StringComparison.OrdinalIgnoreCase))).GetEnumerator())
          {
            if (enumerator.MoveNext())
              extensionFile1 = enumerator.Current;
          }
        }
      }
      if (extensionFile1 == null)
        return (Stream) null;
      requestContext.Trace(12061092, TraceLevel.Info, "Gallery", nameof (UnifiedValidationPipelineService), "VSIX found for extension");
      return requestContext.GetService<ITeamFoundationFileService>().RetrieveFile(requestContext, (long) extensionFile1.FileId, false, out byte[] _, out long _, out CompressionType _);
    }

    private static string GetExtensionType(PublishedExtension extension) => !extension.IsVsCodeExtension() ? string.Empty : "VsCodeExtensionV1";

    private static string GetBlobResourcePath(Guid extensionId, Guid validationId)
    {
      string str = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/uvp/{1}/{2}", (object) extensionId, (object) validationId, (object) str);
    }

    private void StrongboxValueChangeCallback(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      this.LoadBlobStorageBaseUrl(requestContext, 12062081, nameof (UnifiedValidationPipelineService));
      this.LoadServiceBusConfiguration(requestContext);
    }

    private void ConfigurationChangeCallback(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadUnifiedValidationPipelineConfiguration(requestContext);
    }

    private static void LogFaultClientTraceLog(
      IVssRequestContext requestContext,
      PackageValidationMessageData uvpRequest,
      string errorMessage,
      Guid validationTrackingId,
      Exception exception = null)
    {
      ClientTraceData properties = new ClientTraceData();
      if (uvpRequest != null)
      {
        properties.Add("ValidationTrackingId", (object) uvpRequest.StartValidation.ValidationTrackingId);
        properties.Add("ContentType", (object) uvpRequest.StartValidation.ContentType);
        properties.Add("ContentUrl", (object) uvpRequest.StartValidation.ContentUrl);
        properties.Add(nameof (errorMessage), (object) errorMessage);
        properties.Add(nameof (exception), (object) exception);
      }
      else
      {
        properties.Add("ValidationTrackingId", (object) validationTrackingId);
        properties.Add(nameof (errorMessage), (object) errorMessage);
      }
      requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "UVP", properties);
    }
  }
}
