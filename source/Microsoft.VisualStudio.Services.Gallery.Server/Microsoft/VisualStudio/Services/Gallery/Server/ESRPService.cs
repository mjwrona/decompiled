// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ESRPService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Configuration;
using MS.Ess.EsrpClient.Contracts.Common;
using MS.Ess.EsrpClient.Contracts.Scan.V1;
using MS.Ess.EsrpClient.Contracts.Sign.V1;
using MS.Ess.EsrpClient.Contracts.Sign.V1.Response;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class ESRPService : PackageVerificationService, IESRPService, IVssFrameworkService
  {
    private GalleryBlobProvider m_esrpBlobProvider;
    private string m_esrpClientPath;
    private string m_esrpContainerName;
    private string m_esrpClientId;
    private string m_esrpApiEndPoint;
    private string m_esrpCertificateSubjectname;
    private string m_esrpTenantId;
    private string m_esrpSigningKeyCode;
    private ESRPConfigHelper m_esrpConfigHelper;
    private GalleryFileSystemHelper m_fileSystemHelper;
    private GalleryProcessHandlerHelper m_processHandlerHelper;
    private IESRPAuthProviderService m_eSRPAuthProviderService;
    private long m_virusScanRecheckTimeInProgress = 30;
    private long m_virusScanRecheckTimePendingAnalysis = 10800;
    private long m_virusScanTimeoutInProgress = 3600;
    private long m_virusScanTimeoutPendingAnalysis = 180000;
    private long m_fileSigningRecheckTimeInProgress = 30;
    private long m_fileSigningRecheckTimePendingAnalysis = 7200;
    private long m_fileSigningTimeoutInProgress = 3600;
    private long m_fileSigningTimeoutPendingAnalysis = 180000;
    private IEsrpConverter m_esrpConverter;
    private static readonly string s_layer = nameof (ESRPService);

    public ESRPService()
    {
    }

    public ESRPService(
      GalleryFileSystemHelper galleryFSHelper,
      GalleryProcessHandlerHelper galleryProcessHandlerHelper)
    {
      this.m_fileSystemHelper = galleryFSHelper;
      this.m_processHandlerHelper = galleryProcessHandlerHelper;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(12061119, "gallery", ESRPService.s_layer, nameof (ServiceStart));
      if (systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback), false, "/Configuration/Service/Gallery/PackageVerification/**");
        systemRequestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.StrongboxValueChangeCallback), "ConfigurationSecrets", (IEnumerable<string>) new string[1]
        {
          "GalleryPVStorageConnectionString"
        });
        this.m_fileSystemHelper = this.m_fileSystemHelper != null ? this.m_fileSystemHelper : new GalleryFileSystemHelper();
        this.m_processHandlerHelper = this.m_processHandlerHelper != null ? this.m_processHandlerHelper : new GalleryProcessHandlerHelper();
        this.m_eSRPAuthProviderService = systemRequestContext.GetService<IESRPAuthProviderService>();
        this.m_esrpConverter = (IEsrpConverter) new EsrpConverter();
        this.LoadESRPConfiguration(systemRequestContext);
        this.LoadBlobStorageBaseUrl(systemRequestContext, 12061119, ESRPService.s_layer);
        this.m_esrpBlobProvider = new GalleryBlobProvider(systemRequestContext, "ConfigurationSecrets", "GalleryPVStorageConnectionString");
      }
      systemRequestContext.TraceLeave(12061119, "gallery", ESRPService.s_layer, nameof (ServiceStart));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback));
      if (this.m_esrpBlobProvider != null)
        this.m_esrpBlobProvider.Unload(systemRequestContext);
      systemRequestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.StrongboxValueChangeCallback));
    }

    public FileInfo PrepareStreamForScan(
      IVssRequestContext requestContext,
      Stream stream,
      Guid scanId,
      string fileName)
    {
      requestContext.TraceEnter(12061119, "gallery", ESRPService.s_layer, nameof (PrepareStreamForScan));
      FileInfo fileInfo = (FileInfo) null;
      if (this.m_esrpBlobProvider != null && this.m_esrpBlobProvider.IsValidBlobProvider() && stream != null && this.m_blobStoreBaseURL != (Uri) null && !string.IsNullOrWhiteSpace(fileName))
      {
        string base64ShA256Hash = GalleryServerUtil.CalculateBase64SHA256Hash(stream);
        stream.Seek(0L, SeekOrigin.Begin);
        string resourceId = Path.Combine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/esrp/", (object) scanId), fileName);
        this.m_esrpBlobProvider.PutStream(requestContext, this.m_esrpContainerName, resourceId, stream, (IDictionary<string, string>) null);
        string absoluteUri = new Uri(this.m_blobStoreBaseURL, this.m_esrpContainerName + "/" + resourceId).AbsoluteUri;
        fileInfo = new FileInfo()
        {
          CustomerCorrelationId = Guid.NewGuid().ToString(),
          HashType = (HashType) 0,
          SourceHash = base64ShA256Hash,
          SourceLocation = absoluteUri,
          SizeInBytes = stream.Length
        };
      }
      else
        requestContext.Trace(12061119, TraceLevel.Info, "gallery", nameof (PrepareStreamForScan), "Blob not uploaded to ESRP blob store");
      requestContext.TraceLeave(12061119, "gallery", ESRPService.s_layer, nameof (PrepareStreamForScan));
      return fileInfo;
    }

    public void CleanupBlobstore(IVssRequestContext requestContext, string blobURL)
    {
      requestContext.TraceEnter(12061119, "gallery", ESRPService.s_layer, nameof (CleanupBlobstore));
      if (this.m_esrpBlobProvider != null && this.m_esrpBlobProvider.IsValidBlobProvider() && this.m_blobStoreBaseURL != (Uri) null)
      {
        string absoluteUri = new Uri(this.m_blobStoreBaseURL, this.m_esrpContainerName + "/").AbsoluteUri;
        if (blobURL.StartsWith(absoluteUri, StringComparison.OrdinalIgnoreCase))
        {
          int length = absoluteUri.Length;
          string resourceId = blobURL.Substring(length);
          this.m_esrpBlobProvider.DeleteBlob(requestContext, this.m_esrpContainerName, resourceId);
        }
      }
      requestContext.TraceLeave(12061119, "gallery", ESRPService.s_layer, nameof (CleanupBlobstore));
    }

    public ScanBatchSubmissionResponse SubmitScan(
      IVssRequestContext requestContext,
      List<FileInfo> filesToScan)
    {
      requestContext.TraceEnter(12061119, "gallery", ESRPService.s_layer, nameof (SubmitScan));
      ScanBatchSubmissionResponse submissionResponse = (ScanBatchSubmissionResponse) null;
      if (filesToScan != null && filesToScan.Count > 0)
      {
        string scanSubmitInputFile = this.CreateScanSubmitInputFile(filesToScan);
        string filePath = string.Empty;
        try
        {
          filePath = !requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.UseOfOAuthForESRPEnabled") ? this.ExecuteESRPClient(requestContext, nameof (SubmitScan), scanSubmitInputFile) : this.ExecuteESRPClientUsingOuth(requestContext, nameof (SubmitScan), scanSubmitInputFile);
          if (!string.IsNullOrEmpty(filePath))
            submissionResponse = JsonUtilities.Deserialize<ScanBatchSubmissionResponse>(this.m_fileSystemHelper.ReadFileContent(filePath));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(12061119, "gallery", "ESRP: SubmitScan", ex);
          throw;
        }
        finally
        {
          this.m_fileSystemHelper.DeleteFiles(new List<string>()
          {
            scanSubmitInputFile,
            filePath
          });
        }
      }
      requestContext.TraceLeave(12061119, "gallery", ESRPService.s_layer, nameof (SubmitScan));
      return submissionResponse;
    }

    public ScanBatchCompletionResponse GetScanStatus(
      IVssRequestContext requestContext,
      List<Guid> operationIds)
    {
      requestContext.TraceEnter(12061119, "gallery", ESRPService.s_layer, nameof (GetScanStatus));
      ScanBatchCompletionResponse scanStatus = (ScanBatchCompletionResponse) null;
      if (operationIds != null && operationIds.Count > 0)
      {
        string scanStatusInputFile = this.CreateScanStatusInputFile(operationIds);
        string filePath = string.Empty;
        try
        {
          filePath = !requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.UseOfOAuthForESRPEnabled") ? this.ExecuteESRPClient(requestContext, "ScanStatus", scanStatusInputFile) : this.ExecuteESRPClientUsingOuth(requestContext, "ScanStatus", scanStatusInputFile);
          if (!string.IsNullOrEmpty(filePath))
            scanStatus = JsonUtilities.Deserialize<ScanBatchCompletionResponse>(this.m_fileSystemHelper.ReadFileContent(filePath));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(12061119, "gallery", "ESRP: ScanStatus", ex);
          throw;
        }
        finally
        {
          this.m_fileSystemHelper.DeleteFiles(new List<string>()
          {
            scanStatusInputFile,
            filePath
          });
        }
      }
      requestContext.TraceLeave(12061119, "gallery", ESRPService.s_layer, nameof (GetScanStatus));
      return scanStatus;
    }

    public bool IsScanTimeOut(
      IVssRequestContext requestContext,
      DateTime startTime,
      StatusCode currentStatus)
    {
      bool flag = false;
      long int64 = Convert.ToInt64(DateTime.UtcNow.Subtract(startTime).TotalSeconds);
      if (currentStatus == 1 && int64 > this.m_virusScanTimeoutInProgress)
        flag = true;
      else if (currentStatus == 4 && int64 > this.m_virusScanTimeoutPendingAnalysis)
        flag = true;
      return flag;
    }

    public bool IsSigningTimeOut(
      IVssRequestContext requestContext,
      DateTimeOffset startTime,
      EsrpOperationStatus currentStatus)
    {
      bool flag = false;
      TimeSpan timeSpan = DateTimeOffset.UtcNow - startTime;
      if (currentStatus == EsrpOperationStatus.InProgress && timeSpan.TotalSeconds > (double) this.m_fileSigningTimeoutInProgress)
        flag = true;
      else if (currentStatus == EsrpOperationStatus.ManualAnalysisPending && timeSpan.TotalSeconds > (double) this.m_fileSigningTimeoutPendingAnalysis)
        flag = true;
      return flag;
    }

    public TimeSpan GetRecheckTimespan(IVssRequestContext requestContext, StatusCode currentStatus) => TimeSpan.FromSeconds(currentStatus == 1 ? (double) this.m_virusScanRecheckTimeInProgress : (double) this.m_virusScanRecheckTimePendingAnalysis);

    public TimeSpan GetRecheckTimespanForSigning(
      IVssRequestContext requestContext,
      StatusCode currentStatus)
    {
      return TimeSpan.FromSeconds(currentStatus == 1 ? (double) this.m_fileSigningRecheckTimeInProgress : (double) this.m_fileSigningRecheckTimePendingAnalysis);
    }

    public IReadOnlyList<EsrpStartOperationResponse> SubmitFilesForSigning(
      IVssRequestContext requestContext,
      IReadOnlyList<EsrpSignRequest> requestData)
    {
      requestContext.TraceEnter(12061119, "gallery", ESRPService.s_layer, nameof (SubmitFilesForSigning));
      if (requestData == null)
      {
        requestContext.Trace(12061119, TraceLevel.Info, "gallery", nameof (SubmitFilesForSigning), "ESRP sign request is null");
        throw new ArgumentNullException(nameof (requestData));
      }
      StaticSigningData signRequest = this.m_esrpConverter.CreateSignRequest((IReadOnlyCollection<EsrpSignRequest>) requestData);
      IReadOnlyList<EsrpStartOperationResponse> operationResponseList = (IReadOnlyList<EsrpStartOperationResponse>) null;
      string filePath = string.Empty;
      if (signRequest != null)
      {
        string nameWithExtension = FileSpec.GetTempFileNameWithExtension("json");
        this.m_fileSystemHelper.SerializeAndWriteObjectToFile((object) signRequest, nameWithExtension);
        try
        {
          filePath = !requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.UseOfOAuthForESRPEnabled") ? this.ExecuteESRPClient(requestContext, "SubmitSign", nameWithExtension) : this.ExecuteESRPClientUsingOuth(requestContext, "SubmitSign", nameWithExtension, true);
          if (!string.IsNullOrEmpty(filePath))
            operationResponseList = this.m_esrpConverter.CreateEsrpSignResponse(JsonUtilities.Deserialize<SignBatchSubmissionResponse>(this.m_fileSystemHelper.ReadFileContent(filePath)));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(12061119, "gallery", "ESRP: SubmitSign", ex);
          throw;
        }
        finally
        {
          this.m_fileSystemHelper.DeleteFiles(new List<string>()
          {
            nameWithExtension,
            filePath
          });
        }
      }
      else
        requestContext.Trace(12061119, TraceLevel.Error, "gallery", "SubmitSign", "Signing data is null or empty");
      requestContext.TraceLeave(12061119, "gallery", ESRPService.s_layer, nameof (SubmitFilesForSigning));
      return operationResponseList;
    }

    public IReadOnlyList<EsrpGetOperationStatusResponse> GetSignStatus(
      IVssRequestContext requestContext,
      IReadOnlyList<string> operationIds)
    {
      requestContext.TraceEnter(12061119, "gallery", ESRPService.s_layer, nameof (GetSignStatus));
      if (operationIds == null)
        throw new ArgumentNullException(nameof (operationIds));
      requestContext.Trace(12061119, TraceLevel.Info, "gallery", nameof (GetSignStatus), "Requesting sign operation status update for ESRP operation {OperationIds}", (object) operationIds);
      IReadOnlyList<EsrpGetOperationStatusResponse> signStatus = (IReadOnlyList<EsrpGetOperationStatusResponse>) null;
      string filePath = string.Empty;
      SignStatusRequest signStatusRequest = this.m_esrpConverter.CreateGetSignStatusRequest((IReadOnlyCollection<EsrpGetOperationStatusRequest>) operationIds.Select<string, EsrpGetOperationStatusRequest>((Func<string, EsrpGetOperationStatusRequest>) (id => new EsrpGetOperationStatusRequest(id))).ToList<EsrpGetOperationStatusRequest>());
      string nameWithExtension = FileSpec.GetTempFileNameWithExtension("json");
      this.m_fileSystemHelper.SerializeAndWriteObjectToFile((object) signStatusRequest, nameWithExtension);
      try
      {
        filePath = !requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.UseOfOAuthForESRPEnabled") ? this.ExecuteESRPClient(requestContext, "SignStatus", nameWithExtension) : this.ExecuteESRPClientUsingOuth(requestContext, "SignStatus", nameWithExtension, true);
        if (!string.IsNullOrEmpty(filePath))
          signStatus = this.m_esrpConverter.CreateGetSignStatusResponse(JsonUtilities.Deserialize<SignBatchCompletionResponse>(this.m_fileSystemHelper.ReadFileContent(filePath)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061119, "gallery", "ESRP: GetSignStatus", ex);
        throw;
      }
      finally
      {
        this.m_fileSystemHelper.DeleteFiles(new List<string>()
        {
          nameWithExtension,
          filePath
        });
      }
      requestContext.TraceLeave(12061119, "gallery", ESRPService.s_layer, nameof (GetSignStatus));
      return signStatus;
    }

    private void ConfigurationChangeCallback(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadESRPConfiguration(requestContext);
    }

    private void StrongboxValueChangeCallback(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      this.LoadBlobStorageBaseUrl(requestContext, 12061119, ESRPService.s_layer);
    }

    private void LoadESRPConfiguration(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(12061119, "gallery", ESRPService.s_layer, nameof (LoadESRPConfiguration));
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PackageVerification/**");
      string configurationSetting = AzureRoleUtil.GetOverridableConfigurationSetting("RuntimeServicePrincipalClientId");
      string valueFromPath = registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/ESRP/Server", string.Empty);
      this.m_esrpContainerName = registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/BlobContainer", "packageverification");
      this.m_esrpClientId = registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/ESRP/ESRPClientId", configurationSetting);
      this.m_esrpApiEndPoint = registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/ESRP/ESRPApiEndpoint", "https://api.esrp.microsoft.com/.default");
      this.m_esrpCertificateSubjectname = registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/ESRP/CertificateSubjectName", "CN=" + this.m_esrpClientId + "@microsoft.com");
      this.m_esrpTenantId = registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/ESRP/ESRPTenantId", "33e01921-4d64-4f8c-a055-5bdaffd5e33d");
      this.m_esrpClientPath = Path.Combine(requestContext.ServiceHost.PhysicalDirectory, "bin\\ESRP\\ESRPClient.exe");
      this.m_esrpSigningKeyCode = registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/ESRP/RepositorySigningKeyCodeRegistryPath", string.Empty);
      this.m_esrpConfigHelper = new ESRPConfigHelper(valueFromPath, configurationSetting, this.m_fileSystemHelper);
      this.m_virusScanRecheckTimeInProgress = long.Parse(registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/ESRP/Times/VirusScanRecheckTimeInProgress", 30L.ToString((IFormatProvider) CultureInfo.InvariantCulture)), (IFormatProvider) CultureInfo.InvariantCulture);
      this.m_virusScanRecheckTimePendingAnalysis = long.Parse(registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/ESRP/Times/VirusScanRecheckTimePendingAnalysis", 10800L.ToString((IFormatProvider) CultureInfo.InvariantCulture)), (IFormatProvider) CultureInfo.InvariantCulture);
      this.m_virusScanTimeoutInProgress = long.Parse(registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/ESRP/Times/VirusScanTimeoutInProgress", 3600L.ToString((IFormatProvider) CultureInfo.InvariantCulture)), (IFormatProvider) CultureInfo.InvariantCulture);
      this.m_virusScanTimeoutPendingAnalysis = long.Parse(registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/ESRP/Times/VirusScanTimeoutPendingAnalysis", 180000L.ToString((IFormatProvider) CultureInfo.InvariantCulture)), (IFormatProvider) CultureInfo.InvariantCulture);
      this.m_fileSigningRecheckTimeInProgress = long.Parse(registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/ESRP/Times/FileSigningRecheckTimeInProgress", 30L.ToString((IFormatProvider) CultureInfo.InvariantCulture)), (IFormatProvider) CultureInfo.InvariantCulture);
      this.m_fileSigningRecheckTimePendingAnalysis = long.Parse(registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/ESRP/Times/FileSigningRecheckTimePendingAnalysis", 7200L.ToString((IFormatProvider) CultureInfo.InvariantCulture)), (IFormatProvider) CultureInfo.InvariantCulture);
      this.m_fileSigningTimeoutInProgress = long.Parse(registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/ESRP/Times/FileSigningTimeoutInProgress", 3600L.ToString((IFormatProvider) CultureInfo.InvariantCulture)), (IFormatProvider) CultureInfo.InvariantCulture);
      this.m_fileSigningTimeoutPendingAnalysis = long.Parse(registryEntryCollection.GetValueFromPath<string>("/Configuration/Service/Gallery/PackageVerification/ESRP/Times/FileSigningTimeoutPendingAnalysis", 180000L.ToString((IFormatProvider) CultureInfo.InvariantCulture)), (IFormatProvider) CultureInfo.InvariantCulture);
      requestContext.Trace(12061119, TraceLevel.Info, "gallery", nameof (LoadESRPConfiguration), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ESRP Server URL: {0}, Blob Container Name: {1}, ESRP Client Path: {2}", (object) valueFromPath, (object) this.m_esrpContainerName, (object) this.m_esrpClientPath));
      requestContext.TraceLeave(12061119, "gallery", ESRPService.s_layer, nameof (LoadESRPConfiguration));
    }

    private string CreateScanSubmitInputFile(List<FileInfo> FilesToScan)
    {
      ScanBatchInfo scanBatchInfo1 = new ScanBatchInfo();
      ((BaseBatches) scanBatchInfo1).SourceLocationType = (FileLocationType) 0;
      scanBatchInfo1.ScanRequestFiles = (IList<FileInfo>) FilesToScan;
      ScanBatchInfo scanBatchInfo2 = scanBatchInfo1;
      ScanRequest objectToSerialize = new ScanRequest()
      {
        ScanBatches = (IList<ScanBatchInfo>) new List<ScanBatchInfo>()
        {
          scanBatchInfo2
        }
      };
      string nameWithExtension = FileSpec.GetTempFileNameWithExtension("json");
      this.m_fileSystemHelper.SerializeAndWriteObjectToFile((object) objectToSerialize, nameWithExtension);
      return nameWithExtension;
    }

    private string CreateScanStatusInputFile(List<Guid> operationIds)
    {
      ScanStatusRequest objectToSerialize = new ScanStatusRequest()
      {
        OperationIds = operationIds
      };
      string nameWithExtension = FileSpec.GetTempFileNameWithExtension("json");
      this.m_fileSystemHelper.SerializeAndWriteObjectToFile((object) objectToSerialize, nameWithExtension);
      return nameWithExtension;
    }

    private CommandLineBuilder GetESRPCommandlineArgs(
      string command,
      string inputFilePath,
      string outputFilePath,
      string logFilePath,
      bool isSigning = false)
    {
      CommandLineBuilder esrpCommandlineArgs = new CommandLineBuilder();
      esrpCommandlineArgs.Append(command);
      esrpCommandlineArgs.Append("-a");
      if (isSigning)
        esrpCommandlineArgs.Append(this.m_esrpConfigHelper.SigningAuthFilePath);
      else
        esrpCommandlineArgs.Append(this.m_esrpConfigHelper.AuthFilePath);
      esrpCommandlineArgs.Append("-c");
      esrpCommandlineArgs.Append(this.m_esrpConfigHelper.ConfigFilePath);
      esrpCommandlineArgs.Append("-p");
      esrpCommandlineArgs.Append(this.m_esrpConfigHelper.PolicyFilePath);
      esrpCommandlineArgs.Append("-i");
      esrpCommandlineArgs.Append(inputFilePath);
      esrpCommandlineArgs.Append("-o");
      esrpCommandlineArgs.Append(outputFilePath);
      esrpCommandlineArgs.Append("-l");
      esrpCommandlineArgs.Append("Verbose");
      esrpCommandlineArgs.Append("-f");
      esrpCommandlineArgs.Append(logFilePath);
      return esrpCommandlineArgs;
    }

    private void LogDetailedCILog(
      IVssRequestContext requestContext,
      string commandLineArgs,
      ProcessOutput esrpClientProcess,
      string inputFilePath,
      string outputFilePath,
      string logFilePath)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("ExitCode", (double) esrpClientProcess.ExitCode);
      properties.Add("ExecutionTime", esrpClientProcess.RunningTime.TotalMilliseconds.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      properties.Add("CommandLine", commandLineArgs);
      properties.Add("StdError", esrpClientProcess.StdErr);
      properties.Add("StdOut", esrpClientProcess.StdOut);
      properties.Add("AuthFile", this.m_fileSystemHelper.ReadFileContent(this.m_esrpConfigHelper.AuthFilePath));
      properties.Add("ConfigFile", this.m_fileSystemHelper.ReadFileContent(this.m_esrpConfigHelper.ConfigFilePath));
      properties.Add("PolicyFile", this.m_fileSystemHelper.ReadFileContent(this.m_esrpConfigHelper.PolicyFilePath));
      properties.Add("InputFile", this.m_fileSystemHelper.ReadFileContent(inputFilePath));
      properties.Add("OutputFile", this.m_fileSystemHelper.ReadFileContent(outputFilePath));
      properties.Add(nameof (logFilePath), this.m_fileSystemHelper.ReadFileContent(logFilePath));
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "ESRP", properties);
    }

    private string ExecuteESRPClient(
      IVssRequestContext requestContext,
      string command,
      string inputFilePath)
    {
      string tempFileName1 = FileSpec.GetTempFileName();
      string tempFileName2 = FileSpec.GetTempFileName();
      string str = string.Empty;
      string configurationSetting = AzureRoleUtil.GetOverridableConfigurationSetting("RuntimeServicePrincipalCertThumbprint");
      this.m_esrpConfigHelper.ValidateAndCreateESRPConfigFiles(requestContext, configurationSetting, this.m_esrpClientId, this.m_esrpTenantId, this.m_esrpApiEndPoint, this.m_esrpCertificateSubjectname);
      try
      {
        CommandLineBuilder esrpCommandlineArgs = this.GetESRPCommandlineArgs(command, inputFilePath, tempFileName1, tempFileName2);
        ProcessOutput esrpClientProcess = this.m_processHandlerHelper.RunExe(this.m_esrpClientPath, esrpCommandlineArgs, (ITFLogger) new NullLogger());
        if (esrpClientProcess.ExitCode == 0)
        {
          str = tempFileName1;
        }
        else
        {
          this.LogDetailedCILog(requestContext, esrpCommandlineArgs.ToString(), esrpClientProcess, inputFilePath, tempFileName1, tempFileName2);
          this.m_esrpConfigHelper.Reset();
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061119, "gallery", command, ex);
        throw;
      }
      finally
      {
        this.m_fileSystemHelper.DeleteFiles(new List<string>()
        {
          tempFileName2
        });
      }
      return str;
    }

    private string ExecuteESRPClientUsingOuth(
      IVssRequestContext requestContext,
      string command,
      string inputFilePath,
      bool isSigning = false)
    {
      requestContext.TraceEnter(12061119, "gallery", ESRPService.s_layer, nameof (ExecuteESRPClientUsingOuth));
      string tempFileName1 = FileSpec.GetTempFileName();
      string tempFileName2 = FileSpec.GetTempFileName();
      string str = string.Empty;
      string configurationSetting = AzureRoleUtil.GetOverridableConfigurationSetting("RuntimeServicePrincipalCertThumbprint");
      try
      {
        this.m_esrpConfigHelper.ValidateAndCreateESRPConfigFiles(requestContext, configurationSetting, this.m_esrpClientId, this.m_esrpTenantId, this.m_esrpApiEndPoint, this.m_esrpCertificateSubjectname, isSigning);
        CommandLineBuilder esrpCommandlineArgs = this.GetESRPCommandlineArgs(command, inputFilePath, tempFileName1, tempFileName2, isSigning);
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
          FileName = this.m_esrpClientPath,
          Arguments = esrpCommandlineArgs.ToString()
        };
        string oauthToken = this.m_eSRPAuthProviderService.GetOAuthToken(requestContext, Guid.Parse("33e01921-4d64-4f8c-a055-5bdaffd5e33d"));
        startInfo.EnvironmentVariables.Add("ESRPOAuthToken", oauthToken);
        ProcessOutput esrpClientProcess = this.m_processHandlerHelper.RunExe(startInfo, esrpCommandlineArgs.ToHiddenString(), (ITFLogger) new NullLogger());
        if (esrpClientProcess.ExitCode == 0)
        {
          str = tempFileName1;
        }
        else
        {
          this.LogDetailedCILog(requestContext, esrpCommandlineArgs.ToString(), esrpClientProcess, inputFilePath, tempFileName1, tempFileName2);
          this.m_esrpConfigHelper.Reset();
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061119, "gallery", command, ex);
        throw;
      }
      finally
      {
        this.m_fileSystemHelper.DeleteFiles(new List<string>()
        {
          tempFileName2
        });
        requestContext.TraceLeave(12061119, "gallery", ESRPService.s_layer, nameof (ExecuteESRPClientUsingOuth));
      }
      return str;
    }

    public string UploadFileToBlobStore(
      IVssRequestContext requestContext,
      string filePath,
      string blobName,
      Guid scanId)
    {
      requestContext.TraceEnter(12061119, "gallery", ESRPService.s_layer, nameof (UploadFileToBlobStore));
      string blobStore = (string) null;
      if (this.m_esrpBlobProvider != null && this.m_esrpBlobProvider.IsValidBlobProvider() && this.m_blobStoreBaseURL != (Uri) null)
      {
        if (!string.IsNullOrWhiteSpace(filePath))
        {
          try
          {
            string resourceId = Path.Combine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/esrp/", (object) scanId), blobName);
            using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
              this.m_esrpBlobProvider.PutStream(requestContext, this.m_esrpContainerName, resourceId, (Stream) fileStream, (IDictionary<string, string>) null);
              blobStore = new Uri(this.m_blobStoreBaseURL, this.m_esrpContainerName + "/" + resourceId).AbsoluteUri;
              goto label_9;
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceException(12061119, "gallery", nameof (UploadFileToBlobStore), ex);
            goto label_9;
          }
        }
      }
      requestContext.Trace(12061119, TraceLevel.Info, "gallery", nameof (UploadFileToBlobStore), "file not uploaded to ESRP blob store");
label_9:
      requestContext.TraceLeave(12061119, "gallery", ESRPService.s_layer, nameof (UploadFileToBlobStore));
      return blobStore;
    }

    public bool DoesFileExistInBlobStore(
      IVssRequestContext requestContext,
      string fileName,
      Guid scanId)
    {
      requestContext.TraceEnter(12061119, "gallery", ESRPService.s_layer, nameof (DoesFileExistInBlobStore));
      bool flag = false;
      if (this.m_esrpBlobProvider != null && this.m_esrpBlobProvider.IsValidBlobProvider() && this.m_blobStoreBaseURL != (Uri) null)
      {
        if (!string.IsNullOrWhiteSpace(fileName))
        {
          try
          {
            string resourceId = Path.Combine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/esrp/", (object) scanId), fileName);
            flag = this.m_esrpBlobProvider.BlobExists(requestContext, this.m_esrpContainerName, resourceId);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(12061119, "gallery", nameof (DoesFileExistInBlobStore), ex);
          }
        }
      }
      requestContext.TraceLeave(12061119, "gallery", ESRPService.s_layer, nameof (DoesFileExistInBlobStore));
      return flag;
    }

    public string GetSigningKeyCode() => this.m_esrpSigningKeyCode;
  }
}
