// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.PackageManagementPlatformServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.Utility;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP
{
  public class PackageManagementPlatformServiceFacade : 
    IPackageManagementPlatformServiceFacade,
    IVssFrameworkService
  {
    private IUploadServiceClient uploadServiceClient;
    private IGraphQLClient _graphQLClient;
    private const string Layer = "PMPUploadService";
    private string _uploadServiceUrl = "https://{0}/api/upload?RegistryName={1}&ArtifactFileTypeMoniker={2}";
    private string graphQLServiceUri = string.Empty;
    private string graphQLServiceAdminUri = string.Empty;
    private string _uploadServiceTargetHost = string.Empty;
    private long _pmpUploadRetryCount = 3;
    private long _pmpUploadRequestDelayTime = 2;
    private IPackageAggregateMutation _packageAggregateMutationBuilder;
    private IPackageMutationsBuilder _packageMutationsBuilder;

    public PackageManagementPlatformServiceFacade(
      IUploadServiceClient uploadService,
      IGraphQLClient graphQlClient,
      string targetHost,
      string graphQlHost)
    {
      this.uploadServiceClient = uploadService;
      this._uploadServiceTargetHost = targetHost;
      this._graphQLClient = graphQlClient;
      this.graphQLServiceUri = graphQlHost;
    }

    public PackageManagementPlatformServiceFacade()
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      using (systemRequestContext.TraceBlock(12062087, 12062087, "gallery", "PMPUploadService", nameof (ServiceStart)))
      {
        systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback), false, "/Configuration/Service/Gallery/PMP/**");
        this.LoadPMPUploadServiceConfiguration(systemRequestContext);
        if (this.uploadServiceClient == null)
          this.uploadServiceClient = (IUploadServiceClient) new UploadServiceClient(Microsoft.VisualStudio.Services.Gallery.Server.Facade.HttpClientFactory.New());
      }
      using (systemRequestContext.TraceBlock(12062089, 12062089, "gallery", "PMPUploadService", nameof (ServiceStart)))
      {
        if (this._graphQLClient == null)
          this._graphQLClient = (IGraphQLClient) new GraphQLClient(systemRequestContext, Microsoft.VisualStudio.Services.Gallery.Server.Facade.HttpClientFactory.New());
        if (this._packageAggregateMutationBuilder == null)
          this._packageAggregateMutationBuilder = (IPackageAggregateMutation) new PackageAggregateMutation();
        if (this._packageMutationsBuilder != null)
          return;
        this._packageMutationsBuilder = (IPackageMutationsBuilder) new PackageMutationsBuilder();
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      using (systemRequestContext.TraceBlock(12062087, 12062087, "gallery", "PMPUploadService", nameof (ServiceEnd)))
        systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback));
    }

    public void UploadArtifactFile(
      IVssRequestContext requestContext,
      Stream artifactFile,
      ExtensionVersionToPublish extensionToPublish,
      string registryName,
      string artifactFileTypeMoniker)
    {
      using (requestContext.TraceBlock(12062087, 12062087, "gallery", "PMPUploadService", nameof (UploadArtifactFile)))
      {
        ArgumentUtility.CheckForNull<PackageManagementPlatformServiceFacade>(this, nameof (artifactFile));
        ArgumentUtility.CheckForNull<PackageManagementPlatformServiceFacade>(this, nameof (extensionToPublish));
        ArgumentUtility.CheckForNull<PackageManagementPlatformServiceFacade>(this, nameof (registryName));
        ArgumentUtility.CheckForNull<PackageManagementPlatformServiceFacade>(this, nameof (artifactFileTypeMoniker));
        ArgumentUtility.CheckForNull<PackageManagementPlatformServiceFacade>(this, "_uploadServiceTargetHost");
        ArgumentUtility.CheckForNull<PackageManagementPlatformServiceFacade>(this, "_uploadServiceUrl");
        string requestUri = string.Format(this._uploadServiceUrl, (object) this._uploadServiceTargetHost, (object) registryName, (object) artifactFileTypeMoniker);
        this.UploadWithRetryAsync(requestContext, requestUri, artifactFile, extensionToPublish).SyncResult();
        artifactFile.Position = 0L;
      }
    }

    private async Task UploadWithRetryAsync(
      IVssRequestContext requestContext,
      string requestUri,
      Stream artifactFile,
      ExtensionVersionToPublish extensionToPublish)
    {
      int currentRetry = 0;
      bool uploadSucceeded = false;
      while (!uploadSucceeded && (long) currentRetry < this._pmpUploadRetryCount)
      {
        artifactFile.Position = 0L;
        MultipartFormDataContent content = new MultipartFormDataContent()
        {
          {
            (HttpContent) new StreamContent((Stream) new NonOwnedStream(artifactFile)),
            nameof (artifactFile),
            nameof (artifactFile)
          }
        };
        string existsInConflictList = (requestContext.GetService<IExtensionNameConflictService>() ?? throw new ArgumentNullException("PackageManagementPlatformServiceFacade.extensionConflictService")).GetNewExtensionNameIfExistsInConflictList(extensionToPublish.PublisherName + "." + extensionToPublish.ExtensionName);
        if (!string.IsNullOrEmpty(existsInConflictList))
        {
          content.Headers.Add("X-VsCodeServer-ExtensionNameOverride", existsInConflictList);
          requestContext.TraceAlways(12062087, TraceLevel.Info, "gallery", nameof (UploadWithRetryAsync), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Using conflict list for extension Id {0} name {1}", (object) extensionToPublish.ExtensionId, (object) extensionToPublish.ExtensionName));
        }
        bool isBlockPublishForPmpBadRequestsFFEnabled = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.BlockPublishForPmpBadRequests");
        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
          using (HttpResponseMessage httpResponseMessage = await this.uploadServiceClient.UploadArtifactFileToPMPAsync(requestContext, requestUri, content).ConfigureAwait(false))
          {
            stopwatch.Stop();
            string responseContent = string.Empty;
            if (httpResponseMessage.Content != null)
              responseContent = httpResponseMessage.Content.ReadAsStringAsync().SyncResult<string>();
            if (httpResponseMessage.IsSuccessStatusCode)
            {
              uploadSucceeded = true;
              PackageManagementPlatformServiceFacade.PublishTelemetryForPMPUploadService(requestContext, extensionToPublish, (int) httpResponseMessage.StatusCode, responseContent, currentRetry, uploadSucceeded, stopwatch, "Artifact file uploaded to PMP upload service successfully");
              break;
            }
            if (isBlockPublishForPmpBadRequestsFFEnabled && httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
              throw new PmpUploadServiceBadRequestException(PackageManagementPlatformServiceFacade.GetEndUserErrorMessage(requestContext, responseContent));
            if (httpResponseMessage.StatusCode >= HttpStatusCode.InternalServerError)
            {
              ++currentRetry;
              requestContext.TraceAlways(12062087, TraceLevel.Info, "gallery", nameof (UploadWithRetryAsync), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to send request to upload Service for extension Id {0} name {1} retry count {2} errorCode {3}", (object) extensionToPublish.ExtensionId, (object) extensionToPublish.ExtensionName, (object) currentRetry, (object) httpResponseMessage.StatusCode));
              if ((long) currentRetry >= this._pmpUploadRetryCount)
              {
                PackageManagementPlatformServiceFacade.PublishTelemetryForPMPUploadService(requestContext, extensionToPublish, (int) httpResponseMessage.StatusCode, responseContent, currentRetry, uploadSucceeded, stopwatch, "All retries exhaust");
                break;
              }
            }
            else
            {
              PackageManagementPlatformServiceFacade.PublishTelemetryForPMPUploadService(requestContext, extensionToPublish, (int) httpResponseMessage.StatusCode, responseContent, currentRetry, uploadSucceeded, stopwatch, "Unable to upload artifact file to upload Service");
              break;
            }
          }
        }
        catch (Exception ex) when (!(ex is PmpUploadServiceBadRequestException))
        {
          stopwatch.Stop();
          requestContext.TraceAlways(12062087, TraceLevel.Error, "gallery", nameof (UploadWithRetryAsync), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to send request to upload Service for extension Id {0} name {1} exception {2}", (object) extensionToPublish.ExtensionId, (object) extensionToPublish.ExtensionName, (object) ex));
          PackageManagementPlatformServiceFacade.PublishTelemetryForPMPUploadService(requestContext, extensionToPublish, 0, "Exception occurred while sending request", currentRetry, uploadSucceeded, stopwatch, ex.Message);
          break;
        }
        await Task.Delay(TimeSpan.FromSeconds((double) this._pmpUploadRequestDelayTime));
        stopwatch = (Stopwatch) null;
      }
    }

    public async Task<HttpResponseMessage> UpdatePackageAggregateArchivedState(
      IVssRequestContext requestContext,
      string registryName,
      PublishedExtension extension,
      bool isArchived)
    {
      ArgumentUtility.CheckForNull<string>(registryName, nameof (registryName));
      ArgumentUtility.CheckForNull<PublishedExtension>(extension, nameof (extension));
      ArgumentUtility.CheckForNull<string>(extension.ExtensionName, "ExtensionName");
      string packageName = extension.ExtensionName;
      string existsInConflictList = (requestContext.GetService<IExtensionNameConflictService>() ?? throw new ArgumentNullException("PackageManagementPlatformServiceFacade.extensionConflictService")).GetNewExtensionNameIfExistsInConflictList(extension.Publisher.PublisherName + "." + extension.ExtensionName);
      if (!string.IsNullOrEmpty(existsInConflictList))
      {
        packageName = existsInConflictList;
        requestContext.TraceAlways(12062087, TraceLevel.Info, "gallery", "UploadWithRetryAsync", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Using conflict list for extension Id {0} name {1}", (object) extension.ExtensionId, (object) extension.ExtensionName));
      }
      string stateMutationString = this._packageAggregateMutationBuilder.GetUpdatePackageAggregateArchiveStateMutationString(registryName, packageName, isArchived);
      return await this._graphQLClient.CallMutationWithRetry(requestContext, this.graphQLServiceUri, stateMutationString, extension.ExtensionId, "updatePackageAggregateArchiveState").ConfigureAwait(false);
    }

    public async Task<HttpResponseMessage> DeletePackageAggregate(
      IVssRequestContext requestContext,
      string registryName,
      PublishedExtension extension)
    {
      ArgumentUtility.CheckForNull<string>(registryName, nameof (registryName));
      ArgumentUtility.CheckForNull<PublishedExtension>(extension, nameof (extension));
      ArgumentUtility.CheckForNull<string>(extension.ExtensionName, "ExtensionName");
      string packageName = extension.ExtensionName;
      string existsInConflictList = (requestContext.GetService<IExtensionNameConflictService>() ?? throw new ArgumentNullException("PackageManagementPlatformServiceFacade.extensionConflictService")).GetNewExtensionNameIfExistsInConflictList(extension.Publisher.PublisherName + "." + extension.ExtensionName);
      if (!string.IsNullOrEmpty(existsInConflictList))
      {
        packageName = existsInConflictList;
        requestContext.TraceAlways(12062087, TraceLevel.Info, "gallery", "UploadWithRetryAsync", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Using conflict list for extension Id {0} name {1}", (object) extension.ExtensionId, (object) extension.ExtensionName));
      }
      string aggregateMutationString = this._packageAggregateMutationBuilder.GetDeletePackageAggregateMutationString(registryName, packageName);
      return await this._graphQLClient.CallMutationWithRetry(requestContext, this.graphQLServiceAdminUri, aggregateMutationString, extension.ExtensionId, "removeFullPackageAggregate").ConfigureAwait(false);
    }

    public async Task<HttpResponseMessage> UpdatePackageAggregateLockState(
      IVssRequestContext requestContext,
      string registryName,
      PublishedExtension extension,
      bool isLocked)
    {
      ArgumentUtility.CheckForNull<string>(registryName, nameof (registryName));
      ArgumentUtility.CheckForNull<PublishedExtension>(extension, nameof (extension));
      ArgumentUtility.CheckForNull<string>(extension.ExtensionName, "ExtensionName");
      string packageName = extension.ExtensionName;
      string existsInConflictList = (requestContext.GetService<IExtensionNameConflictService>() ?? throw new ArgumentNullException("PackageManagementPlatformServiceFacade.extensionConflictService")).GetNewExtensionNameIfExistsInConflictList(extension.Publisher.PublisherName + "." + extension.ExtensionName);
      if (!string.IsNullOrEmpty(existsInConflictList))
      {
        packageName = existsInConflictList;
        requestContext.TraceAlways(12062087, TraceLevel.Info, "gallery", "UploadWithRetryAsync", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Using conflict list for extension Id {0} name {1}", (object) extension.ExtensionId, (object) extension.ExtensionName));
      }
      string statusMutationString = this._packageAggregateMutationBuilder.GetUpdatePackageAggregateLockStatusMutationString(registryName, packageName, isLocked);
      return await this._graphQLClient.CallMutationWithRetry(requestContext, this.graphQLServiceUri, statusMutationString, extension.ExtensionId, "updatePackageAggregateLockState").ConfigureAwait(false);
    }

    public async Task<HttpResponseMessage> DeletePackage(
      IVssRequestContext requestContext,
      string registryName,
      PublishedExtension extension,
      string version)
    {
      ArgumentUtility.CheckForNull<string>(registryName, nameof (registryName));
      ArgumentUtility.CheckForNull<PublishedExtension>(extension, nameof (extension));
      ArgumentUtility.CheckForNull<string>(extension.ExtensionName, "ExtensionName");
      ArgumentUtility.CheckStringForNullOrEmpty(version, nameof (version));
      string packageName = extension.ExtensionName;
      string existsInConflictList = (requestContext.GetService<IExtensionNameConflictService>() ?? throw new ArgumentNullException("PackageManagementPlatformServiceFacade.extensionConflictService")).GetNewExtensionNameIfExistsInConflictList(extension.Publisher.PublisherName + "." + extension.ExtensionName);
      if (!string.IsNullOrEmpty(existsInConflictList))
      {
        packageName = existsInConflictList;
        requestContext.TraceAlways(12062087, TraceLevel.Info, "gallery", "UploadWithRetryAsync", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Using conflict list for extension Id {0} name {1}", (object) extension.ExtensionId, (object) extension.ExtensionName));
      }
      string packageMutationString = this._packageMutationsBuilder.GetDeletePackageMutationString(registryName, packageName, version);
      return await this._graphQLClient.CallMutationWithRetry(requestContext, this.graphQLServiceAdminUri, packageMutationString, extension.ExtensionId, "removePackage").ConfigureAwait(false);
    }

    private void ConfigurationChangeCallback(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadPMPUploadServiceConfiguration(requestContext);
    }

    private void LoadPMPUploadServiceConfiguration(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(12062087, "gallery", "PMPUploadService", nameof (LoadPMPUploadServiceConfiguration));
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      RegistryEntryCollection registryEntryCollection = service.ReadEntries(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PMP/**");
      this._uploadServiceUrl = registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PMP/UploadService/Url", "https://{0}/api/upload?RegistryName={1}&ArtifactFileTypeMoniker={2}");
      this._uploadServiceTargetHost = registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PMP/UploadService/TargetHost", this._uploadServiceTargetHost);
      this._pmpUploadRetryCount = (long) registryEntryCollection.GetValueFromPath<int>("/Configuration/Service/Gallery/PMP/UploadService/Times/RetryCount", 3);
      this._pmpUploadRequestDelayTime = (long) registryEntryCollection.GetValueFromPath<int>("/Configuration/Service/Gallery/PMP/UploadService/Times/DelayTime", 2);
      this.graphQLServiceUri = service.GetValue<string>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PMP/GraphQLService/TargetHost", string.Empty);
      this.graphQLServiceAdminUri = service.GetValue<string>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PMP/GraphQLAdminService/TargetHost", string.Empty);
      requestContext.TraceAlways(12062081, TraceLevel.Info, "gallery", nameof (LoadPMPUploadServiceConfiguration), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "URL: {0}, TargetHost: {1}", (object) this._uploadServiceUrl, (object) this._uploadServiceTargetHost));
      requestContext.TraceLeave(12062087, "gallery", "PMPUploadService", nameof (LoadPMPUploadServiceConfiguration));
    }

    private static void PublishTelemetryForPMPUploadService(
      IVssRequestContext requestContext,
      ExtensionVersionToPublish extension,
      int statusCode,
      string responseContent,
      int retryCount,
      bool uploadSucceeded,
      Stopwatch stopwatch,
      string message)
    {
      string str = new Dictionary<string, string>()
      {
        {
          "ExtensionId",
          extension.ExtensionId.ToString()
        },
        {
          "ExtensionName",
          extension.ExtensionName
        },
        {
          "PublisherName",
          extension.PublisherName
        },
        {
          "ExtensionVersion",
          extension.Version
        },
        {
          "TargetPlatform",
          extension.TargetPlatform
        },
        {
          "isUploadSucceeded",
          uploadSucceeded.ToString()
        },
        {
          "StatusCode",
          statusCode.ToString()
        },
        {
          "Reason",
          responseContent
        },
        {
          "TimeSpan",
          stopwatch.ElapsedMilliseconds.ToString()
        },
        {
          "RetryCount",
          retryCount.ToString()
        },
        {
          "MessageDetails",
          message
        }
      }.Serialize<Dictionary<string, string>>();
      requestContext.TraceAlways(12062087, TraceLevel.Info, "gallery", nameof (PublishTelemetryForPMPUploadService), "{0}", (object) str);
    }

    private static PmpUploadServiceErrorResponse DeserializeToPmpUploadServiceErrorResponse(
      IVssRequestContext requestContext,
      string responseContent)
    {
      PmpUploadServiceErrorResponse serviceErrorResponse;
      try
      {
        serviceErrorResponse = JsonConvert.DeserializeObject<PmpUploadServiceErrorResponse>(responseContent);
      }
      catch (JsonSerializationException ex)
      {
        requestContext.TraceAlways(12062087, TraceLevel.Error, "gallery", "GetEndUserErrorMessage", "Exception occurred while deserializing error response from PMP Upload Service: " + responseContent);
        throw new PmpUploadServiceBadRequestException("Unexpected error occurred.");
      }
      if (serviceErrorResponse == null || serviceErrorResponse.Error?.Message == null)
      {
        requestContext.TraceAlways(12062087, TraceLevel.Error, "gallery", "GetEndUserErrorMessage", "Error message from PMP Upload Service got deserialized to null: " + responseContent);
        throw new PmpUploadServiceBadRequestException("Unexpected error occurred.");
      }
      return serviceErrorResponse;
    }

    private static string GetEndUserErrorMessage(
      IVssRequestContext requestContext,
      string responseContent)
    {
      PmpUploadServiceErrorResponse serviceErrorResponse = PackageManagementPlatformServiceFacade.DeserializeToPmpUploadServiceErrorResponse(requestContext, responseContent);
      string userErrorMessage = serviceErrorResponse.Error.Message;
      switch (serviceErrorResponse.Error.Code)
      {
        case ErrorCode.FailFastValidationFailed:
          if (!serviceErrorResponse.Error.Details.IsNullOrEmpty<ErrorDetail>())
          {
            ErrorDetailCode code = serviceErrorResponse.Error.Details[0].Code;
            switch (code)
            {
              case ErrorDetailCode.ZipSlipValidationFailed:
                userErrorMessage = GalleryResources.ZipSlipErrorMessage();
                break;
              case ErrorDetailCode.ZipBombValidationFailed:
                userErrorMessage = GalleryResources.ZipBombErrorMessage();
                break;
              case ErrorDetailCode.DuplicateEntryValidationFailed:
                userErrorMessage = GalleryResources.DuplicateEntryErrorMessage();
                break;
              case ErrorDetailCode.FutureModifyDateValidationFailed:
                userErrorMessage = GalleryResources.FutureModifyDateErrorMessage();
                break;
              default:
                requestContext.TraceAlways(12062087, TraceLevel.Warning, "gallery", nameof (GetEndUserErrorMessage), string.Format("No overriding message found for '{0}' error detail code for '{1}' from PMP.", (object) code, (object) serviceErrorResponse.Error.Code));
                break;
            }
          }
          else
          {
            requestContext.TraceAlways(12062087, TraceLevel.Error, "gallery", nameof (GetEndUserErrorMessage), string.Format("No detail found for '{0}' from PMP.", (object) serviceErrorResponse.Error.Code));
            break;
          }
          break;
        case ErrorCode.ManifestFileParsingFailed:
          userErrorMessage = GalleryResources.ManifestFileParsingErrorMessage();
          if (!serviceErrorResponse.Error.Details.IsNullOrEmpty<ErrorDetail>())
          {
            userErrorMessage = serviceErrorResponse.Error.Details[0].Message;
            break;
          }
          requestContext.TraceAlways(12062087, TraceLevel.Error, "gallery", nameof (GetEndUserErrorMessage), string.Format("No detail found for '{0}' from PMP.", (object) serviceErrorResponse.Error.Code));
          break;
        default:
          requestContext.TraceAlways(12062087, TraceLevel.Warning, "gallery", nameof (GetEndUserErrorMessage), string.Format("No overriding message found for '{0}' error code from PMP.", (object) serviceErrorResponse.Error.Code));
          break;
      }
      return userErrorMessage;
    }
  }
}
