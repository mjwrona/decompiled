// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationSteps.RepositorySigningStep
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.ESRP;
using Microsoft.VisualStudio.Services.Gallery.Server.SigningTool;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using MS.Ess.EsrpClient.Contracts.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationSteps
{
  internal class RepositorySigningStep : ValidationPipelineStepBase
  {
    [StaticSafe]
    private static string s_stepName = nameof (RepositorySigningStep);
    [StaticSafe("Grandfathered")]
    private static StepType s_stepType = StepType.RepositorySigning;
    private const int c_retryCount = 3;
    private const string s_layer = "RepositorySigningStep";

    public RepositorySigningStep()
      : base(RepositorySigningStep.s_stepName, RepositorySigningStep.s_stepType)
    {
    }

    public override void Initialize(Guid parentId, Guid validationId)
    {
      base.Initialize(parentId, validationId);
      this.m_resultMessage = string.Empty;
    }

    public override string BeginValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Stream packageStream)
    {
      requestContext.TraceEnter(12062096, "Gallery", nameof (RepositorySigningStep), nameof (BeginValidation));
      base.BeginValidation(requestContext, extension, packageStream);
      this.m_Result = ValidationStatus.InProgress;
      try
      {
        if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          this.m_Result = ValidationStatus.Success;
          return this.ValidationContext;
        }
        if (extension != null)
        {
          if (requestContext.GetService<IRepositorySigningService>() == null)
          {
            this.m_Result = ValidationStatus.Failure;
            return this.ValidationContext;
          }
          this.InitializeValidationContext(requestContext, extension);
        }
        else
        {
          ArgumentNullException argumentNullException = new ArgumentNullException("Invalid published extension object for Repository signing");
          requestContext.TraceException(12062096, "Gallery", nameof (RepositorySigningStep), (Exception) argumentNullException);
          this.LogRepositorySigningTelemetry(requestContext, (RepositorySigningInfo) null, false, true, TimeSpan.Zero, "BeginValidation failed: null extension", nameof (RepositorySigningStep));
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062096, "Gallery", nameof (RepositorySigningStep), ex);
        this.LogRepositorySigningTelemetry(requestContext, (RepositorySigningInfo) null, false, true, TimeSpan.Zero, "BeginValidation failed: Unknown exception", nameof (RepositorySigningStep));
      }
      finally
      {
        requestContext.TraceLeave(12062096, "Gallery", nameof (RepositorySigningStep), nameof (BeginValidation));
      }
      return this.ValidationContext;
    }

    public override string StartRepositorySigning(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      bool isRetry = false)
    {
      requestContext.TraceEnter(12062096, "Gallery", nameof (RepositorySigningStep), nameof (StartRepositorySigning));
      try
      {
        if (extension == null)
        {
          ArgumentException argumentException = new ArgumentException("Invalid published extension object for repository signing");
          requestContext.TraceException(12061120, "Gallery", nameof (RepositorySigningStep), (Exception) argumentException);
          this.LogRepositorySigningTelemetry(requestContext, (RepositorySigningInfo) null, false, true, DateTime.UtcNow - this.StartTime, "Failed at StartRepositorySigning because of null Extension object", nameof (RepositorySigningStep));
          throw argumentException;
        }
        RepositorySigningInfo signingInfo = JsonUtilities.Deserialize<RepositorySigningInfo>(this.ValidationContext);
        if (requestContext.GetService<IRepositorySigningService>() == null || signingInfo == null)
        {
          this.m_Result = ValidationStatus.Failure;
          this.m_resultMessage = GalleryResources.RepositorySigningFailedMessage();
          this.LogRepositorySigningTelemetry(requestContext, signingInfo, false, true, TimeSpan.Zero, "Failed during StartRepositorySigning", nameof (RepositorySigningStep));
          return this.ValidationContext;
        }
        if (signingInfo.Status != RepositorySigningStepStatus.NotStarted && !isRetry)
          return this.ValidationContext;
        if (!isRetry)
        {
          this.StartTime = DateTime.UtcNow;
          this.LogRepositorySigningTelemetry(requestContext, signingInfo, false, false, TimeSpan.Zero, "Repository signing started", nameof (RepositorySigningStep));
        }
        signingInfo.Status = RepositorySigningStepStatus.Started;
        long num = this.PrepareExtensionForSigning(requestContext, extension, signingInfo);
        signingInfo.FileSize = num;
        IESRPService service = requestContext.GetService<IESRPService>();
        if (this.GenerateSignatureManifest(requestContext, signingInfo.SignatureManifestFilePath, signingInfo.PackageFilePath) != ReturnCode.Success)
        {
          signingInfo.Status = RepositorySigningStepStatus.ManifestFileGenerationFailed;
          this.ValidationContext = signingInfo.Serialize<RepositorySigningInfo>();
          this.m_Result = ValidationStatus.Failure;
          this.m_resultMessage = GalleryResources.RepositorySigningFailedMessage();
          this.LogRepositorySigningTelemetry(requestContext, signingInfo, false, true, DateTime.UtcNow - this.StartTime, "", nameof (RepositorySigningStep));
          return this.ValidationContext;
        }
        signingInfo.SignatureManifestFileUrl = service.UploadFileToBlobStore(requestContext, signingInfo.SignatureManifestFilePath, ".signature.manifest", this.ParentValidationId);
        signingInfo.Status = RepositorySigningStepStatus.ManifestFileGenerated;
        this.LogRepositorySigningTelemetry(requestContext, signingInfo, false, false, TimeSpan.Zero, "Manifest file generated", nameof (RepositorySigningStep));
        EsrpStartOperationResponse operationResponse = this.SubmitFileForSigningWithRetry(requestContext, extension, signingInfo);
        if (operationResponse != null && operationResponse.OperationStatus == EsrpOperationStatus.Pass)
        {
          signingInfo.OperationId = operationResponse.OperationId;
          signingInfo.OperationStatus = operationResponse.OperationStatus;
          signingInfo.Status = RepositorySigningStepStatus.SubmittedForSignatureGeneration;
          this.LogRepositorySigningTelemetry(requestContext, signingInfo, false, false, TimeSpan.Zero, "Submitted file for signing to ESRP", nameof (RepositorySigningStep));
          this.ValidationContext = signingInfo.Serialize<RepositorySigningInfo>();
        }
        else
        {
          signingInfo.Status = RepositorySigningStepStatus.SubmissionForSignatureGenerationFailed;
          this.ValidationContext = signingInfo.Serialize<RepositorySigningInfo>();
          this.m_Result = ValidationStatus.Failure;
          this.m_resultMessage = GalleryResources.RepositorySigningFailedMessage();
          this.LogRepositorySigningTelemetry(requestContext, signingInfo, false, true, DateTime.UtcNow - this.StartTime, "Repository signing failed during ESRP sign request submission", nameof (RepositorySigningStep));
          return this.ValidationContext;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062096, "Gallery", nameof (RepositorySigningStep), ex);
        this.m_Result = ValidationStatus.Failure;
        this.m_resultMessage = GalleryResources.RepositorySigningFailedMessage();
        this.LogRepositorySigningTelemetry(requestContext, (RepositorySigningInfo) null, false, true, DateTime.UtcNow - this.StartTime, "Failed at StartRepositorySigning because of unknown exception", nameof (RepositorySigningStep));
      }
      finally
      {
        requestContext.TraceLeave(12062096, "Gallery", nameof (RepositorySigningStep), nameof (StartRepositorySigning));
      }
      return this.ValidationContext;
    }

    public override ValidationStatus RetrieveResult(
      IVssRequestContext requestContext,
      PublishedExtension extension = null)
    {
      if (this.m_Result == ValidationStatus.InProgress || this.m_Result == ValidationStatus.PendingAnalysis)
      {
        if (string.IsNullOrWhiteSpace(this.ValidationContext))
        {
          this.m_Result = ValidationStatus.Success;
        }
        else
        {
          RepositorySigningInfo signingInfo = JsonUtilities.Deserialize<RepositorySigningInfo>(this.ValidationContext);
          try
          {
            IRepositorySigningService service1 = requestContext.GetService<IRepositorySigningService>();
            if (signingInfo.Status == RepositorySigningStepStatus.SubmittedForSignatureGeneration)
            {
              EsrpGetOperationStatusResponse signingStatusWithRetry = this.GetSigningStatusWithRetry(requestContext, extension, signingInfo, signingInfo.OperationId.ToString());
              if (signingStatusWithRetry != null)
              {
                IESRPService service2 = requestContext.GetService<IESRPService>();
                long int64 = Convert.ToInt64(DateTime.UtcNow.Subtract(this.StartTime).TotalSeconds);
                switch (signingStatusWithRetry.OperationStatus)
                {
                  case EsrpOperationStatus.Pass:
                    if (service1.DownloadFile(requestContext, signingStatusWithRetry.SignedFileUrl, signingInfo.SignatureFilePath))
                    {
                      this.PrepareForCurrentState(requestContext, extension, signingInfo);
                      signingInfo.SignatureFileUrl = service2.UploadFileToBlobStore(requestContext, signingInfo.SignatureFilePath, ".signature.p7s", this.ParentValidationId);
                      signingInfo.Status = RepositorySigningStepStatus.SignatureGenerated;
                      ESRPJobHelper.LogSignCompleteTelemetry(requestContext, signingInfo, false, int64, signingStatusWithRetry, nameof (RepositorySigningStep));
                      break;
                    }
                    signingInfo.Status = RepositorySigningStepStatus.SignatureGenerationFailed;
                    this.ValidationContext = signingInfo.Serialize<RepositorySigningInfo>();
                    this.m_Result = ValidationStatus.Failure;
                    this.m_resultMessage = GalleryResources.RepositorySigningFailedMessage();
                    this.LogRepositorySigningTelemetry(requestContext, signingInfo, false, true, DateTime.UtcNow - this.StartTime, "Failed to download signature file", nameof (RepositorySigningStep));
                    return this.m_Result;
                  case EsrpOperationStatus.InProgress:
                    this.m_Result = ValidationStatus.InProgress;
                    break;
                  case EsrpOperationStatus.FailCannotRetry:
                    this.m_Result = ValidationStatus.Failure;
                    this.m_resultMessage = GalleryResources.RepositorySigningFailedMessage();
                    ESRPJobHelper.LogSignCompleteTelemetry(requestContext, signingInfo, false, int64, signingStatusWithRetry, nameof (RepositorySigningStep));
                    this.LogRepositorySigningTelemetry(requestContext, signingInfo, false, true, DateTime.UtcNow - this.StartTime, "Failed while getting esrp signing response", nameof (RepositorySigningStep));
                    break;
                  case EsrpOperationStatus.ManualAnalysisPending:
                    this.m_Result = ValidationStatus.PendingAnalysis;
                    ESRPJobHelper.LogSignCompleteTelemetry(requestContext, signingInfo, false, int64, signingStatusWithRetry, "SigningStepIntermediate");
                    break;
                  default:
                    this.m_Result = this.RetrySigningSubmission(requestContext, extension, signingInfo.RetryCount);
                    signingInfo = JsonUtilities.Deserialize<RepositorySigningInfo>(this.ValidationContext);
                    if (this.m_Result == ValidationStatus.Failure)
                    {
                      this.m_resultMessage = GalleryResources.RepositorySigningFailedMessage();
                      ESRPJobHelper.LogSignCompleteTelemetry(requestContext, signingInfo, false, int64, signingStatusWithRetry, nameof (RepositorySigningStep));
                      this.LogRepositorySigningTelemetry(requestContext, signingInfo, false, true, DateTime.UtcNow - this.StartTime, "Failed after retrying", nameof (RepositorySigningStep));
                      break;
                    }
                    break;
                }
                if ((this.m_Result == ValidationStatus.InProgress || this.m_Result == ValidationStatus.PendingAnalysis) && service2.IsSigningTimeOut(requestContext, (DateTimeOffset) this.StartTime, signingStatusWithRetry.OperationStatus))
                {
                  this.m_Result = this.RetrySigningSubmission(requestContext, extension, signingInfo.RetryCount);
                  signingInfo = JsonUtilities.Deserialize<RepositorySigningInfo>(this.ValidationContext);
                  if (this.m_Result == ValidationStatus.Failure)
                  {
                    this.m_resultMessage = GalleryResources.PublishFailureGeneric();
                    ESRPJobHelper.LogSignCompleteTelemetry(requestContext, signingInfo, true, int64, signingStatusWithRetry, nameof (RepositorySigningStep));
                    this.LogRepositorySigningTelemetry(requestContext, signingInfo, true, true, DateTime.UtcNow - this.StartTime, "Repositiory signing timed out after retrying", nameof (RepositorySigningStep));
                  }
                }
              }
              else
              {
                signingInfo.Status = RepositorySigningStepStatus.SignatureGenerationFailed;
                this.ValidationContext = signingInfo.Serialize<RepositorySigningInfo>();
                this.m_Result = ValidationStatus.Failure;
                this.m_resultMessage = GalleryResources.RepositorySigningFailedMessage();
                this.LogRepositorySigningTelemetry(requestContext, signingInfo, false, true, DateTime.UtcNow - this.StartTime, "Failed because of null response from esrp", nameof (RepositorySigningStep));
                return this.m_Result;
              }
            }
            if (signingInfo.Status == RepositorySigningStepStatus.SignatureGenerated && service1 != null)
            {
              if (this.GenerateSignatureArchiveFile(requestContext, signingInfo.SignatureManifestFilePath, signingInfo.SignatureFilePath, signingInfo.SignatureArchiveFilePath) == ReturnCode.Success)
              {
                signingInfo.Status = RepositorySigningStepStatus.SignatureArchiveGenerated;
              }
              else
              {
                signingInfo.Status = RepositorySigningStepStatus.SignatureArchiveGenerationFailed;
                this.m_Result = ValidationStatus.Failure;
                this.m_resultMessage = GalleryResources.RepositorySigningFailedMessage();
                this.LogRepositorySigningTelemetry(requestContext, signingInfo, false, true, DateTime.UtcNow - this.StartTime, "", nameof (RepositorySigningStep));
              }
            }
            if (signingInfo.Status == RepositorySigningStepStatus.SignatureArchiveGenerated)
            {
              if (this.VerifyPackageSignature(requestContext, signingInfo.PackageFilePath, signingInfo.SignatureArchiveFilePath) == ReturnCode.Success)
              {
                signingInfo.Status = RepositorySigningStepStatus.SignatureVerified;
              }
              else
              {
                signingInfo.Status = RepositorySigningStepStatus.SignatureVerificationFailed;
                this.m_Result = ValidationStatus.Failure;
                this.m_resultMessage = GalleryResources.RepositorySigningFailedMessage();
                this.LogRepositorySigningTelemetry(requestContext, signingInfo, false, true, DateTime.UtcNow - this.StartTime, "", nameof (RepositorySigningStep));
              }
            }
            if (signingInfo.Status == RepositorySigningStepStatus.SignatureVerified)
            {
              if (this.UploadSignatureFilesWithRetry(requestContext, extension, signingInfo, 3))
              {
                signingInfo.Status = RepositorySigningStepStatus.SignatureArchiveUploaded;
              }
              else
              {
                signingInfo.Status = RepositorySigningStepStatus.SignatureArchiveUploadFailed;
                this.m_Result = ValidationStatus.Failure;
                this.m_resultMessage = GalleryResources.RepositorySigningFailedMessage();
                this.LogRepositorySigningTelemetry(requestContext, signingInfo, false, true, DateTime.UtcNow - this.StartTime, "Signature file upload failed", nameof (RepositorySigningStep));
              }
            }
            if (signingInfo.Status == RepositorySigningStepStatus.SignatureArchiveUploaded)
            {
              this.m_Result = ValidationStatus.Success;
              this.LogRepositorySigningTelemetry(requestContext, signingInfo, false, false, DateTime.UtcNow - this.StartTime, "Signing completed", nameof (RepositorySigningStep));
            }
            this.ValidationContext = signingInfo.Serialize<RepositorySigningInfo>();
          }
          catch (Exception ex)
          {
            requestContext.TraceException(12062096, "Gallery", nameof (RepositorySigningStep), ex);
            this.m_Result = ValidationStatus.Failure;
            this.m_resultMessage = GalleryResources.RepositorySigningFailedMessage();
            this.LogRepositorySigningTelemetry(requestContext, (RepositorySigningInfo) null, false, true, DateTime.UtcNow - this.StartTime, "Failed at RetrieveResult because of unknown exception", nameof (RepositorySigningStep));
          }
          finally
          {
            this.ValidationContext = signingInfo.Serialize<RepositorySigningInfo>();
            this.LogRepositorySigningTelemetry(requestContext, (RepositorySigningInfo) null, false, false, DateTime.UtcNow - this.StartTime, this.ValidationContext, nameof (RepositorySigningStep));
          }
        }
      }
      return this.m_Result;
    }

    public override void PostValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      if (string.IsNullOrWhiteSpace(this.ValidationContext))
        return;
      RepositorySigningInfo repositorySigningInfo = JsonUtilities.Deserialize<RepositorySigningInfo>(this.ValidationContext);
      requestContext.GetService<IRepositorySigningService>().DeleteRepositorySigningFiles(new List<string>()
      {
        repositorySigningInfo.SignatureManifestFilePath,
        repositorySigningInfo.SignatureFilePath,
        repositorySigningInfo.PackageFilePath,
        repositorySigningInfo.SignatureArchiveFilePath
      });
    }

    public override TimeSpan GetRecheckTime(IVssRequestContext requestContext)
    {
      IESRPService service = requestContext.GetService<IESRPService>();
      StatusCode statusCode = this.m_Result == ValidationStatus.InProgress ? (StatusCode) 1 : (StatusCode) 4;
      IVssRequestContext requestContext1 = requestContext;
      StatusCode currentStatus = statusCode;
      return service.GetRecheckTimespanForSigning(requestContext1, currentStatus);
    }

    private void PrepareForCurrentState(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      RepositorySigningInfo signingInfo)
    {
      requestContext.TraceEnter(12062096, "Gallery", nameof (RepositorySigningStep), nameof (PrepareForCurrentState));
      if (!File.Exists(signingInfo.PackageFilePath))
      {
        requestContext.Trace(12062096, TraceLevel.Info, "gallery", nameof (PrepareForCurrentState), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Package file doesn't exists in JA. Trying to download it again: {0}", (object) signingInfo.PackageFilePath));
        this.PrepareExtensionForSigning(requestContext, extension, signingInfo);
      }
      if (!File.Exists(signingInfo.SignatureManifestFilePath))
      {
        requestContext.Trace(12062096, TraceLevel.Info, "gallery", nameof (PrepareForCurrentState), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Manifest file doesn't exist in JobAgent. Trying to download it again: {0}", (object) signingInfo.SignatureManifestFilePath));
        if (requestContext.GetService<IESRPService>().DoesFileExistInBlobStore(requestContext, ".signature.manifest", this.ParentValidationId))
        {
          requestContext.GetService<IRepositorySigningService>().DownloadFile(requestContext, signingInfo.SignatureManifestFileUrl, signingInfo.SignatureManifestFilePath);
        }
        else
        {
          requestContext.Trace(12062096, TraceLevel.Info, "gallery", nameof (PrepareForCurrentState), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Manifest file doesn't exist in ESRP blob store as well. Trying to generate it again: {0}", (object) signingInfo.SignatureManifestFilePath));
          int signatureManifest = (int) this.GenerateSignatureManifest(requestContext, signingInfo.SignatureManifestFilePath, signingInfo.PackageFilePath);
        }
      }
      requestContext.TraceLeave(12062096, "Gallery", nameof (RepositorySigningStep), nameof (PrepareForCurrentState));
    }

    private ReturnCode GenerateSignatureManifest(
      IVssRequestContext requestContext,
      string signatureManifestFilePath,
      string packageFilePath)
    {
      requestContext.TraceEnter(12062096, "Gallery", nameof (RepositorySigningStep), nameof (GenerateSignatureManifest));
      ReturnCode signatureManifest = ReturnCode.None;
      if (File.Exists(signatureManifestFilePath))
        return ReturnCode.Success;
      try
      {
        signatureManifest = requestContext.GetService<IRepositorySigningService>().GenerateSigningManifestFile(requestContext, packageFilePath, signatureManifestFilePath);
        requestContext.Trace(12062096, TraceLevel.Info, "gallery", nameof (GenerateSignatureManifest), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Generate Signature Manifest file command executed with returnCode: {0}", (object) signatureManifest));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062096, "gallery", nameof (RepositorySigningStep), ex);
      }
      requestContext.TraceLeave(12062096, "Gallery", nameof (RepositorySigningStep), nameof (GenerateSignatureManifest));
      return signatureManifest;
    }

    private ReturnCode GenerateSignatureArchiveFile(
      IVssRequestContext requestContext,
      string manifestFilePath,
      string signatureFile,
      string signatureArchiveFilePath)
    {
      requestContext.TraceEnter(12062096, "Gallery", nameof (RepositorySigningStep), nameof (GenerateSignatureArchiveFile));
      ReturnCode signatureArchiveFile = ReturnCode.None;
      try
      {
        signatureArchiveFile = requestContext.GetService<IRepositorySigningService>().GenerateSignatureArchive(requestContext, manifestFilePath, signatureFile, signatureArchiveFilePath);
        requestContext.Trace(12062096, TraceLevel.Info, "gallery", nameof (GenerateSignatureArchiveFile), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "GenerateSignatureArchive command executed with returnCode: {0}", (object) signatureArchiveFile));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062096, "gallery", nameof (RepositorySigningStep), ex);
      }
      requestContext.TraceLeave(12062096, "Gallery", nameof (RepositorySigningStep), nameof (GenerateSignatureArchiveFile));
      return signatureArchiveFile;
    }

    private ReturnCode VerifyPackageSignature(
      IVssRequestContext requestContext,
      string packageFilePath,
      string signatureArchiveFilePath)
    {
      requestContext.TraceEnter(12062096, "Gallery", nameof (RepositorySigningStep), nameof (VerifyPackageSignature));
      ReturnCode returnCode = ReturnCode.None;
      IRepositorySigningService service = requestContext.GetService<IRepositorySigningService>();
      try
      {
        returnCode = service.VerifyPackageSignature(requestContext, packageFilePath, signatureArchiveFilePath);
        requestContext.Trace(12062096, TraceLevel.Info, "gallery", nameof (VerifyPackageSignature), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "VerifySignedPackage command executed with returnCode: {0}", (object) returnCode));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062096, "gallery", nameof (RepositorySigningStep), ex);
      }
      requestContext.TraceLeave(12062096, "Gallery", nameof (RepositorySigningStep), nameof (VerifyPackageSignature));
      return returnCode;
    }

    private long PrepareExtensionForSigning(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      RepositorySigningInfo signingInfo)
    {
      requestContext.TraceEnter(12062096, "Gallery", nameof (RepositorySigningStep), nameof (PrepareExtensionForSigning));
      long num = 0;
      if (signingInfo != null && File.Exists(signingInfo.PackageFilePath))
        return signingInfo.FileSize;
      ExtensionFile extensionFile = (ExtensionFile) null;
      if (extension != null && extension.Versions[0] != null && extension.Versions[0].Files != null)
      {
        foreach (ExtensionFile file in extension.Versions[0].Files)
        {
          if (file.AssetType.Equals("Microsoft.VisualStudio.Services.VSIXPackage"))
          {
            extensionFile = file;
            break;
          }
        }
      }
      if (extensionFile != null)
      {
        requestContext.Trace(12062096, TraceLevel.Info, "Gallery", nameof (PrepareExtensionForSigning), "Vsix found for extension");
        long contentLength;
        using (Stream packageStream = requestContext.GetService<ITeamFoundationFileService>().RetrieveFile(requestContext, (long) extensionFile.FileId, false, out byte[] _, out contentLength, out CompressionType _))
        {
          this.CopyExtensionToLocalMachine(requestContext, packageStream, extension, signingInfo);
          num = contentLength;
        }
      }
      else
        requestContext.Trace(12062096, TraceLevel.Error, "Gallery", nameof (PrepareExtensionForSigning), "Vsix not found for extension");
      requestContext.TraceLeave(12062096, "Gallery", nameof (RepositorySigningStep), nameof (PrepareExtensionForSigning));
      return num;
    }

    private EsrpStartOperationResponse SubmitFileForSigningWithRetry(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      RepositorySigningInfo signingInfo,
      int retryCount = 3)
    {
      requestContext.TraceLeave(12062096, "Gallery", nameof (RepositorySigningStep), nameof (SubmitFileForSigningWithRetry));
      EsrpStartOperationResponse operationResponse = (EsrpStartOperationResponse) null;
      IESRPService service = requestContext.GetService<IESRPService>();
      string signingKeyCode = service.GetSigningKeyCode();
      while (retryCount > 0)
      {
        try
        {
          if (string.IsNullOrEmpty(signingInfo.SignatureManifestFilePath) || !File.Exists(signingInfo.SignatureManifestFilePath))
            return operationResponse;
          string sourceFileHash = string.Empty;
          long sourceFileSize = 0;
          using (FileStream fileStream = File.OpenRead(signingInfo.SignatureManifestFilePath))
          {
            sourceFileHash = GalleryServerUtil.CalculateBase64SHA256Hash((Stream) fileStream);
            sourceFileSize = fileStream.Length;
          }
          EsrpSignRequest esrpSignRequest = new EsrpSignRequest(signingInfo.SignatureManifestFilePath, sourceFileSize, sourceFileHash, signingInfo.SignatureFilePath, this.ParentValidationId, signingKeyCode);
          IReadOnlyList<EsrpStartOperationResponse> submissionResponse = service.SubmitFilesForSigning(requestContext, (IReadOnlyList<EsrpSignRequest>) new EsrpSignRequest[1]
          {
            esrpSignRequest
          });
          this.LogSignSubmissionCI(requestContext, extension, submissionResponse, retryCount);
          if (submissionResponse == null || submissionResponse.Count != 1)
          {
            --retryCount;
          }
          else
          {
            operationResponse = submissionResponse[0];
            switch (operationResponse.OperationStatus)
            {
              case EsrpOperationStatus.Pass:
                retryCount = 0;
                continue;
              case EsrpOperationStatus.FailCanRetry:
                --retryCount;
                continue;
              default:
                retryCount = 0;
                continue;
            }
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(12062096, "Gallery", nameof (RepositorySigningStep), ex);
          --retryCount;
        }
        finally
        {
          requestContext.TraceLeave(12062096, "Gallery", nameof (RepositorySigningStep), nameof (SubmitFileForSigningWithRetry));
        }
      }
      return operationResponse;
    }

    private EsrpGetOperationStatusResponse GetSigningStatusWithRetry(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      RepositorySigningInfo signingInfo,
      string operationId,
      int retryCount = 3)
    {
      requestContext.TraceLeave(12062096, "Gallery", nameof (RepositorySigningStep), nameof (GetSigningStatusWithRetry));
      EsrpGetOperationStatusResponse signingStatusWithRetry = (EsrpGetOperationStatusResponse) null;
      while (retryCount > 0)
      {
        try
        {
          IReadOnlyList<EsrpGetOperationStatusResponse> signStatus = requestContext.GetService<IESRPService>().GetSignStatus(requestContext, (IReadOnlyList<string>) new string[1]
          {
            operationId
          });
          this.LogGetSignStatusCI(requestContext, extension, signStatus, retryCount);
          if (signStatus == null || signStatus.Count != 1)
          {
            --retryCount;
          }
          else
          {
            signingStatusWithRetry = signStatus[0];
            switch (signingStatusWithRetry.OperationStatus)
            {
              case EsrpOperationStatus.Pass:
                retryCount = 0;
                continue;
              case EsrpOperationStatus.FailCanRetry:
                --retryCount;
                continue;
              default:
                retryCount = 0;
                continue;
            }
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(12062096, "Gallery", nameof (RepositorySigningStep), ex);
          --retryCount;
        }
        finally
        {
          requestContext.TraceLeave(12062096, "Gallery", nameof (RepositorySigningStep), nameof (GetSigningStatusWithRetry));
        }
      }
      return signingStatusWithRetry;
    }

    private ValidationStatus RetrySigningSubmission(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      int retryCount)
    {
      ValidationStatus validationStatus = ValidationStatus.Failure;
      if (retryCount > 0)
      {
        this.StartRepositorySigning(requestContext, extension, true);
        RepositorySigningInfo repositorySigningInfo = JsonUtilities.Deserialize<RepositorySigningInfo>(this.ValidationContext);
        --repositorySigningInfo.RetryCount;
        repositorySigningInfo.IsRetry = true;
        this.ValidationContext = repositorySigningInfo.Serialize<RepositorySigningInfo>();
        validationStatus = ValidationStatus.InProgress;
      }
      return validationStatus;
    }

    private RepositorySigningInfo InitializeValidationContext(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      RepositorySigningInfo repositorySigningInfo = new RepositorySigningInfo()
      {
        ExtensionName = extension.ExtensionName,
        PublisherName = extension.Publisher.PublisherName,
        Version = extension.Versions[0].Version,
        Status = RepositorySigningStepStatus.NotStarted,
        ValidationId = this.ParentValidationId,
        ValidationStepId = this.ValidationId,
        SignatureManifestFilePath = FileSpec.GetTempFileNameWithExtension(".signature.manifest"),
        SignatureFilePath = FileSpec.GetTempFileNameWithExtension(".signature.p7s"),
        SignatureArchiveFilePath = FileSpec.GetTempFileNameWithExtension(".signature.zip"),
        PackageFilePath = FileSpec.GetTempFileNameWithExtension("vsix"),
        RetryCount = 3,
        IsRetry = false
      };
      this.ValidationContext = repositorySigningInfo.Serialize<RepositorySigningInfo>();
      return repositorySigningInfo;
    }

    private void CopyExtensionToLocalMachine(
      IVssRequestContext requestContext,
      Stream packageStream,
      PublishedExtension extension,
      RepositorySigningInfo signingInfo)
    {
      requestContext.TraceEnter(12062096, "Gallery", nameof (RepositorySigningStep), nameof (CopyExtensionToLocalMachine));
      try
      {
        if (extension == null || File.Exists(signingInfo.PackageFilePath))
          return;
        if (packageStream == null)
          throw new ArgumentException("Stream should not be null");
        this.CopyStreamToLocalMachine(requestContext, packageStream, signingInfo.PackageFilePath);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062096, "Gallery", nameof (RepositorySigningStep), ex);
      }
      finally
      {
        requestContext.TraceLeave(12062096, "Gallery", nameof (RepositorySigningStep), nameof (CopyExtensionToLocalMachine));
      }
    }

    private void CopyStreamToLocalMachine(
      IVssRequestContext requestContext,
      Stream stream,
      string filePath)
    {
      stream.Seek(0L, SeekOrigin.Begin);
      using (FileStream destination = File.Create(filePath))
        stream.CopyTo((Stream) destination);
    }

    private bool UploadSignatureFilesWithRetry(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      RepositorySigningInfo signingInfo,
      int retryCount)
    {
      requestContext.TraceEnter(12062096, "Gallery", nameof (RepositorySigningStep), nameof (UploadSignatureFilesWithRetry));
      IPublishedExtensionService service = requestContext.GetService<IPublishedExtensionService>();
      bool flag = false;
      while (retryCount > 0)
      {
        try
        {
          using (FileStream signatureAssetStream = File.OpenRead(signingInfo.SignatureArchiveFilePath))
          {
            signatureAssetStream.Seek(0L, SeekOrigin.Begin);
            flag = !signingInfo.IsFromExistingExtensionSigningJob ? service.UploadSignatureAsset(requestContext, (Stream) signatureAssetStream, extension, this.ParentValidationId) : service.UploadSignatureAsset(requestContext, (Stream) signatureAssetStream, extension, Guid.Empty);
            if (flag)
              return flag;
            --retryCount;
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(12062096, "Gallery", nameof (UploadSignatureFilesWithRetry), ex);
          --retryCount;
        }
        finally
        {
          service.PublishExtensionCreateUpdateNotifications(requestContext, true, extension);
          this.ValidationContext = signingInfo.Serialize<RepositorySigningInfo>();
        }
      }
      requestContext.TraceLeave(12062096, "Gallery", nameof (RepositorySigningStep), nameof (UploadSignatureFilesWithRetry));
      return flag;
    }

    private void LogSignSubmissionCI(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      IReadOnlyList<EsrpStartOperationResponse> submissionResponse,
      int retryCount)
    {
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("PublisherName", extension.Publisher.PublisherName);
      intelligenceData.Add("ExtensionName", extension.ExtensionName);
      intelligenceData.Add("Version", extension.Versions[0].Version);
      intelligenceData.Add("RetryCount", (double) retryCount);
      if (submissionResponse == null)
        intelligenceData.Add("Message", "Null Submission response");
      else if (submissionResponse.Count != 1)
      {
        intelligenceData.Add("Message", "Unexpected Response. Expected 1 response, get " + submissionResponse.Count.ToString());
        intelligenceData.Add("AdditionalInformation", (object) submissionResponse);
      }
      else if (submissionResponse[0].OperationStatus != EsrpOperationStatus.Pass)
      {
        intelligenceData.Add("Message", "Submission failed with status: " + submissionResponse[0].OperationStatus.ToString() + " and error code:" + submissionResponse[0].ErrorCode);
        intelligenceData.Add("AdditionalInformation", submissionResponse[0].ErrorDetail);
      }
      else if (string.IsNullOrWhiteSpace(submissionResponse[0].OperationId))
      {
        intelligenceData.Add("Message", "Submission returned empty GUID as Operation ID ");
        intelligenceData.Add("AdditionalInformation", submissionResponse[0].ErrorDetail);
      }
      IVssRequestContext requestContext1 = requestContext;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(requestContext1, "Microsoft.VisualStudio.Services.Gallery", "RepositorySigningFeature", properties);
    }

    private void LogGetSignStatusCI(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      IReadOnlyList<EsrpGetOperationStatusResponse> submissionResponse,
      int retryCount)
    {
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("PublisherName", extension.Publisher.PublisherName);
      intelligenceData.Add("ExtensionName", extension.ExtensionName);
      intelligenceData.Add("Version", extension.Versions[0].Version);
      intelligenceData.Add("RetryCount", (double) retryCount);
      if (submissionResponse == null)
        intelligenceData.Add("Message", "Null Submission response");
      else if (submissionResponse.Count != 1)
      {
        intelligenceData.Add("Message", "Unexpected Response. Expected 1 response, get " + submissionResponse.Count.ToString());
        intelligenceData.Add("AdditionalInformation", (object) submissionResponse);
      }
      else if (submissionResponse[0].OperationStatus != EsrpOperationStatus.Pass)
      {
        intelligenceData.Add("Message", "Submission failed with status: " + submissionResponse[0].OperationStatus.ToString() + " and error code:" + submissionResponse[0].ErrorCode);
        intelligenceData.Add("AdditionalInformation", submissionResponse[0].ErrorDetail);
      }
      else if (string.IsNullOrWhiteSpace(submissionResponse[0].OperationId))
      {
        intelligenceData.Add("Message", "Submission returned empty GUID as Operation ID ");
        intelligenceData.Add("AdditionalInformation", submissionResponse[0].ErrorDetail);
      }
      IVssRequestContext requestContext1 = requestContext;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(requestContext1, "Microsoft.VisualStudio.Services.Gallery", "RepositorySigningFeature", properties);
    }

    private void LogRepositorySigningTelemetry(
      IVssRequestContext requestContext,
      RepositorySigningInfo signingInfo,
      bool isTimeout,
      bool isFailed,
      TimeSpan timeTaken,
      string additionalInfo,
      string feature)
    {
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      if (signingInfo != null)
      {
        intelligenceData.Add("PublisherName", signingInfo.PublisherName);
        intelligenceData.Add("ExtensionName", signingInfo.ExtensionName);
        intelligenceData.Add("Version", signingInfo.Version);
        intelligenceData.Add("TargetPlatform", signingInfo.TargetPlatform);
        intelligenceData.Add("ExtensionSize", (double) signingInfo.FileSize);
        intelligenceData.Add("OperationId", signingInfo.OperationId);
        intelligenceData.Add("IsRetry", signingInfo.IsRetry);
        intelligenceData.Add("RetryCount", (double) signingInfo.RetryCount);
        intelligenceData.Add("ValidationId", (object) signingInfo.ValidationId);
        intelligenceData.Add("ValidationStepId", (object) signingInfo.ValidationStepId);
        intelligenceData.Add("Status", (object) signingInfo.Status);
        intelligenceData.Add("IsFromExistingExtensionSigningJob", signingInfo.IsFromExistingExtensionSigningJob);
      }
      intelligenceData.Add("TimeTaken", timeTaken.TotalSeconds);
      intelligenceData.Add("IsScanTimeOut", isTimeout);
      intelligenceData.Add("AdditionalInfo", additionalInfo);
      intelligenceData.Add("IsFailed", isFailed);
      IVssRequestContext requestContext1 = requestContext;
      string feature1 = feature;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(requestContext1, "Microsoft.VisualStudio.Services.Gallery", feature1, properties);
    }
  }
}
