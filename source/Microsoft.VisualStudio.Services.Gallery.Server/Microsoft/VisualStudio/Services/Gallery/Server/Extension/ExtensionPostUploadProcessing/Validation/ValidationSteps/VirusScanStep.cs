// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationSteps.VirusScanStep
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.ESRP;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using MS.Ess.EsrpClient.Contracts.Common;
using MS.Ess.EsrpClient.Contracts.Scan.V1;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationSteps
{
  internal class VirusScanStep : ValidationPipelineStepBase
  {
    [StaticSafe]
    private static string s_stepName = nameof (VirusScanStep);
    [StaticSafe("Grandfathered")]
    private static StepType s_stepType = StepType.VirusScan;
    private const string c_scanPayloadName = "Microsoft.VisualStudio.ESRPPayload";
    private const long c_retryCount = 1;
    private const string s_layer = "VirusScanStep";

    public VirusScanStep()
      : base(VirusScanStep.s_stepName, VirusScanStep.s_stepType)
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
      requestContext.TraceEnter(12061120, "Gallery", nameof (VirusScanStep), nameof (BeginValidation));
      base.BeginValidation(requestContext, extension, packageStream);
      this.ValidationContext = string.Empty;
      bool flag = false;
      this.m_Result = ValidationStatus.InProgress;
      try
      {
        if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          if (extension != null)
          {
            flag = this.IsActiveScanEnabled(requestContext, extension);
            FileInfo fileInfo = this.PrepareExtensionForScan(requestContext, packageStream, extension);
            ExtensionScanInfo extensionScanInfo = this.SubmitFileForScan(requestContext, extension, fileInfo);
            if (extensionScanInfo != null)
            {
              if (!flag)
              {
                this.SchedulePassiveResultCheckJob(requestContext, extensionScanInfo);
              }
              else
              {
                extensionScanInfo.RetryCount = new long?(1L);
                extensionScanInfo.IsRetry = new bool?(false);
                this.ValidationContext = extensionScanInfo.Serialize<ExtensionScanInfo>();
              }
            }
          }
          else
          {
            ArgumentException argumentException = new ArgumentException("Invalid published extension object for version validation");
            requestContext.TraceException(12061120, "Gallery", nameof (VirusScanStep), (Exception) argumentException);
          }
        }
        else
          this.m_Result = ValidationStatus.Success;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061120, "Gallery", nameof (VirusScanStep), ex);
      }
      finally
      {
        if (!flag)
          this.m_Result = ValidationStatus.Success;
        requestContext.TraceLeave(12061120, "Gallery", nameof (VirusScanStep), nameof (BeginValidation));
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
          ExtensionScanInfo extensionScanInfo1 = JsonUtilities.Deserialize<ExtensionScanInfo>(this.ValidationContext);
          IESRPService service = requestContext.GetService<IESRPService>();
          if (extensionScanInfo1 != null && extensionScanInfo1.OperationId != Guid.Empty)
          {
            long int64 = Convert.ToInt64(DateTime.UtcNow.Subtract(this.StartTime).TotalSeconds);
            ScanBatchCompletionResponse scanStatus = service.GetScanStatus(requestContext, new List<Guid>()
            {
              extensionScanInfo1.OperationId
            });
            if (scanStatus != null && scanStatus.SubmissionResponses != null && scanStatus.SubmissionResponses.Length == 1)
            {
              bool? isRetry;
              switch ((int) ((ResponseBase) scanStatus.SubmissionResponses[0]).StatusCode)
              {
                case 0:
                  this.m_Result = ValidationStatus.Success;
                  IVssRequestContext requestContext1 = requestContext;
                  ExtensionScanInfo extensionScanInfo2 = extensionScanInfo1;
                  long timeTaken1 = int64;
                  ScanStatusResponse submissionResponse1 = scanStatus.SubmissionResponses[0];
                  isRetry = extensionScanInfo1.IsRetry;
                  int num1 = isRetry.GetValueOrDefault() ? 1 : 0;
                  ESRPJobHelper.LogScanCompleteTelemetry(requestContext1, extensionScanInfo2, false, timeTaken1, (ResponseBase) submissionResponse1, nameof (VirusScanStep), num1 != 0);
                  break;
                case 1:
                  this.m_Result = ValidationStatus.InProgress;
                  break;
                case 4:
                  this.m_Result = ValidationStatus.PendingAnalysis;
                  IVssRequestContext requestContext2 = requestContext;
                  ExtensionScanInfo extensionScanInfo3 = extensionScanInfo1;
                  long timeTaken2 = int64;
                  ScanStatusResponse submissionResponse2 = scanStatus.SubmissionResponses[0];
                  isRetry = extensionScanInfo1.IsRetry;
                  int num2 = isRetry.GetValueOrDefault() ? 1 : 0;
                  ESRPJobHelper.LogScanCompleteTelemetry(requestContext2, extensionScanInfo3, false, timeTaken2, (ResponseBase) submissionResponse2, "VirusScanStepIntermediate", num2 != 0);
                  break;
                default:
                  this.m_Result = this.RetrySubmission(requestContext, extensionScanInfo1);
                  if (this.m_Result == ValidationStatus.Failure)
                  {
                    this.m_resultMessage = GalleryResources.VirusScanFailedMessage();
                    IVssRequestContext requestContext3 = requestContext;
                    ExtensionScanInfo extensionScanInfo4 = extensionScanInfo1;
                    long timeTaken3 = int64;
                    ScanStatusResponse submissionResponse3 = scanStatus.SubmissionResponses[0];
                    isRetry = extensionScanInfo1.IsRetry;
                    int num3 = isRetry.GetValueOrDefault() ? 1 : 0;
                    ESRPJobHelper.LogScanCompleteTelemetry(requestContext3, extensionScanInfo4, false, timeTaken3, (ResponseBase) submissionResponse3, nameof (VirusScanStep), num3 != 0);
                    break;
                  }
                  break;
              }
              if ((this.m_Result == ValidationStatus.InProgress || this.m_Result == ValidationStatus.PendingAnalysis) && service.IsScanTimeOut(requestContext, this.StartTime, ((ResponseBase) scanStatus.SubmissionResponses[0]).StatusCode))
              {
                this.m_Result = this.RetrySubmission(requestContext, extensionScanInfo1);
                if (this.m_Result == ValidationStatus.Failure)
                {
                  this.m_resultMessage = GalleryResources.PublishFailureGeneric();
                  IVssRequestContext requestContext4 = requestContext;
                  ExtensionScanInfo extensionScanInfo5 = extensionScanInfo1;
                  long timeTaken4 = int64;
                  ScanStatusResponse submissionResponse4 = scanStatus.SubmissionResponses[0];
                  isRetry = extensionScanInfo1.IsRetry;
                  int num4 = isRetry.GetValueOrDefault() ? 1 : 0;
                  ESRPJobHelper.LogScanCompleteTelemetry(requestContext4, extensionScanInfo5, true, timeTaken4, (ResponseBase) submissionResponse4, nameof (VirusScanStep), num4 != 0);
                }
              }
            }
            else
            {
              this.m_Result = ValidationStatus.Failure;
              this.m_resultMessage = GalleryResources.VirusScanFailedMessage();
            }
          }
        }
      }
      return this.m_Result;
    }

    public override void PostValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      if (extension != null && extension.IsVsCodeExtension() && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.SkipEsrpBlobDeletionAfterScanForVSCode") || string.IsNullOrWhiteSpace(this.ValidationContext))
        return;
      ExtensionScanInfo extensionScanInfo = JsonUtilities.Deserialize<ExtensionScanInfo>(this.ValidationContext);
      if (extensionScanInfo == null || string.IsNullOrWhiteSpace(extensionScanInfo.BlobFilePath))
        return;
      requestContext.GetService<IESRPService>().CleanupBlobstore(requestContext, extensionScanInfo.BlobFilePath);
    }

    public override TimeSpan GetRecheckTime(IVssRequestContext requestContext)
    {
      IESRPService service = requestContext.GetService<IESRPService>();
      StatusCode statusCode = this.m_Result == ValidationStatus.InProgress ? (StatusCode) 1 : (StatusCode) 4;
      IVssRequestContext requestContext1 = requestContext;
      StatusCode currentStatus = statusCode;
      return service.GetRecheckTimespan(requestContext1, currentStatus);
    }

    private ExtensionScanInfo SubmitFileForScan(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      FileInfo fileInfo)
    {
      ExtensionScanInfo extensionScanInfo = (ExtensionScanInfo) null;
      if (fileInfo != null)
      {
        ScanBatchSubmissionResponse submissionResponse = this.SubmitExtensionForScan(requestContext, fileInfo);
        if (submissionResponse != null && submissionResponse.SubmissionResponses != null && submissionResponse.SubmissionResponses.Length == 1 && ((ResponseBase) submissionResponse.SubmissionResponses[0]).StatusCode == null && ((ResponseBase) submissionResponse.SubmissionResponses[0]).OperationId != Guid.Empty)
        {
          extensionScanInfo = this.GetExtensionScanInfo(extension, fileInfo, submissionResponse.SubmissionResponses[0]);
        }
        else
        {
          this.LogScanSubmissionFailureCI(requestContext, extension, submissionResponse);
          this.CleanupExtensionPackageBlob(requestContext, fileInfo);
        }
      }
      return extensionScanInfo;
    }

    private ValidationStatus RetrySubmission(
      IVssRequestContext requestContext,
      ExtensionScanInfo extensionScanInfo)
    {
      ValidationStatus validationStatus = ValidationStatus.Failure;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVirusScanRetry") && extensionScanInfo?.FileInfo != null && extensionScanInfo != null && extensionScanInfo.RetryCount.GetValueOrDefault(0L) > 0L)
      {
        PublishedExtension extension = new PublishedExtension()
        {
          ExtensionName = extensionScanInfo.ExtensionName,
          Publisher = new PublisherFacts()
          {
            PublisherName = extensionScanInfo.PublisherName
          },
          Versions = new List<ExtensionVersion>()
          {
            new ExtensionVersion()
            {
              Version = extensionScanInfo.Version
            }
          }
        };
        ExtensionScanInfo extensionScanInfo1 = this.SubmitFileForScan(requestContext, extension, extensionScanInfo.FileInfo);
        if (extensionScanInfo1 != null)
        {
          extensionScanInfo1.RetryCount = new long?(extensionScanInfo.RetryCount.Value - 1L);
          extensionScanInfo1.IsRetry = new bool?(true);
          this.StartTime = DateTime.UtcNow;
          this.ValidationContext = extensionScanInfo1.Serialize<ExtensionScanInfo>();
          validationStatus = ValidationStatus.InProgress;
        }
      }
      return validationStatus;
    }

    private FileInfo PrepareExtensionForScan(
      IVssRequestContext requestContext,
      Stream packageStream,
      PublishedExtension extension)
    {
      requestContext.TraceEnter(12061120, "Gallery", nameof (VirusScanStep), nameof (PrepareExtensionForScan));
      FileInfo fileInfo = (FileInfo) null;
      try
      {
        if (extension != null)
          fileInfo = requestContext.GetService<IESRPService>().PrepareStreamForScan(requestContext, (extension.IsVsExtension() ? this.GetStreamToScan(requestContext, extension) : packageStream) ?? throw new ArgumentException("Stream should not be null"), this.ParentValidationId, "Microsoft.VisualStudio.ESRPPayload");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061120, "Gallery", nameof (VirusScanStep), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(12061120, "Gallery", nameof (VirusScanStep), nameof (PrepareExtensionForScan));
      }
      return fileInfo;
    }

    private Stream GetStreamToScan(IVssRequestContext requestContext, PublishedExtension extension)
    {
      Stream stream1 = (Stream) null;
      if (extension != null && extension.Versions != null)
      {
        if (extension.Versions.Count == 1)
        {
          try
          {
            stream1 = (Stream) new MemoryStream();
            using (ZipArchive zipArchive = new ZipArchive(stream1, ZipArchiveMode.Create, true))
            {
              ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
              Dictionary<int, string> dictionary = new Dictionary<int, string>();
              foreach (ExtensionFile file in extension.Versions[0].Files)
              {
                AssetInfo assetInfo = new AssetInfo(file.AssetType, file.Language);
                if (!dictionary.ContainsKey(file.FileId))
                {
                  using (Stream stream2 = service.RetrieveFile(requestContext, (long) file.FileId, false, out byte[] _, out long _, out CompressionType _))
                  {
                    using (Stream destination = zipArchive.CreateEntry(assetInfo.ToString()).Open())
                    {
                      stream2.Seek(0L, SeekOrigin.Begin);
                      stream2.CopyTo(destination);
                    }
                  }
                  dictionary[file.FileId] = assetInfo.ToString();
                }
                else
                  requestContext.Trace(12061120, TraceLevel.Info, "Gallery", nameof (VirusScanStep), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Skipping asset, duplicate file id. Same for {0} and {1}", (object) dictionary[file.FileId], (object) assetInfo.ToString()));
              }
            }
          }
          catch (Exception ex)
          {
            if (stream1 != null)
              ;
            requestContext.TraceException(12061120, "Gallery", nameof (VirusScanStep), ex);
            throw;
          }
        }
      }
      return stream1;
    }

    private ScanBatchSubmissionResponse SubmitExtensionForScan(
      IVssRequestContext requestContext,
      FileInfo fileInfo)
    {
      requestContext.TraceEnter(12061120, "Gallery", nameof (VirusScanStep), nameof (SubmitExtensionForScan));
      ScanBatchSubmissionResponse submissionResponse = (ScanBatchSubmissionResponse) null;
      try
      {
        if (fileInfo != null)
        {
          List<FileInfo> filesToScan = new List<FileInfo>()
          {
            fileInfo
          };
          submissionResponse = requestContext.GetService<IESRPService>().SubmitScan(requestContext, filesToScan);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061120, "Gallery", nameof (VirusScanStep), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(12061120, "Gallery", nameof (VirusScanStep), nameof (SubmitExtensionForScan));
      }
      return submissionResponse;
    }

    private void SchedulePassiveResultCheckJob(
      IVssRequestContext requestContext,
      ExtensionScanInfo extensionScanInfo)
    {
      requestContext.TraceEnter(12061120, "Gallery", nameof (VirusScanStep), nameof (SchedulePassiveResultCheckJob));
      try
      {
        if (extensionScanInfo == null)
          return;
        ScanStatusJobData scanStatusJobData = ESRPJobHelper.PrepareScanStatusJobData(DateTime.UtcNow, (ICollection<ExtensionScanInfo>) new List<ExtensionScanInfo>()
        {
          extensionScanInfo
        });
        if (scanStatusJobData == null || scanStatusJobData.ExtensionsScanInfo.IsNullOrEmpty<ExtensionScanInfo>())
          return;
        ESRPJobHelper.ScheduleScanStatusJob(requestContext, scanStatusJobData, (StatusCode) 1);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061120, "Gallery", nameof (VirusScanStep), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(12061120, "Gallery", nameof (VirusScanStep), nameof (SchedulePassiveResultCheckJob));
      }
    }

    private void CleanupExtensionPackageBlob(IVssRequestContext requestContext, FileInfo fileInfo)
    {
      requestContext.TraceEnter(12061120, "Gallery", nameof (VirusScanStep), nameof (CleanupExtensionPackageBlob));
      try
      {
        if (fileInfo == null || string.IsNullOrWhiteSpace(fileInfo.SourceLocation))
          return;
        new List<FileInfo>() { fileInfo };
        requestContext.GetService<IESRPService>().CleanupBlobstore(requestContext, fileInfo.SourceLocation);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061120, "Gallery", nameof (VirusScanStep), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(12061120, "Gallery", nameof (VirusScanStep), nameof (CleanupExtensionPackageBlob));
      }
    }

    private ExtensionScanInfo GetExtensionScanInfo(
      PublishedExtension extension,
      FileInfo fileInfo,
      ScanSubmissionResponse scanSubmissionResponse)
    {
      ExtensionScanInfo extensionScanInfo = (ExtensionScanInfo) null;
      if (fileInfo != null && scanSubmissionResponse != null && ((ResponseBase) scanSubmissionResponse).StatusCode == null && ((ResponseBase) scanSubmissionResponse).OperationId != Guid.Empty)
        extensionScanInfo = new ExtensionScanInfo()
        {
          ExtensionName = extension.ExtensionName,
          PublisherName = extension.Publisher.PublisherName,
          Version = extension.Versions[0].Version,
          BlobFilePath = fileInfo.SourceLocation,
          FileSize = fileInfo.SizeInBytes,
          OperationId = ((ResponseBase) scanSubmissionResponse).OperationId,
          FileInfo = fileInfo
        };
      return extensionScanInfo;
    }

    private bool IsActiveScanEnabled(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      bool flag = false;
      if (extension.IsVsExtension())
      {
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableActiveVirusScanVS"))
          flag = true;
      }
      else if (extension.IsVsCodeExtension())
      {
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableActiveVirusScanVSCode"))
          flag = true;
      }
      else if (extension.IsPublic() && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableActiveVirusScanVSTS"))
        flag = true;
      return flag;
    }

    private void LogScanSubmissionFailureCI(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      ScanBatchSubmissionResponse submissionResponse)
    {
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("PublisherName", extension.Publisher.PublisherName);
      intelligenceData.Add("ExtensionName", extension.ExtensionName);
      intelligenceData.Add("Version", extension.Versions[0].Version);
      if (submissionResponse == null || submissionResponse.SubmissionResponses == null)
        intelligenceData.Add("Message", "Null Submission response");
      else if (submissionResponse.SubmissionResponses.Length != 1)
      {
        intelligenceData.Add("Message", "Unexpected Response. Expected 1 response, get " + submissionResponse.SubmissionResponses.Length.ToString());
        intelligenceData.Add("AdditionalInformation", (object) submissionResponse.SubmissionResponses);
      }
      else if (((ResponseBase) submissionResponse.SubmissionResponses[0]).StatusCode != null)
      {
        intelligenceData.Add("Message", "Submission failed with status: " + ((ResponseBase) submissionResponse.SubmissionResponses[0]).StatusCode.ToString());
        intelligenceData.Add("AdditionalInformation", (object) ((ResponseBase) submissionResponse.SubmissionResponses[0]).ErrorInfo);
      }
      else if (((ResponseBase) submissionResponse.SubmissionResponses[0]).OperationId == Guid.Empty)
      {
        intelligenceData.Add("Message", "Submission returned empty GUID as Operation ID ");
        intelligenceData.Add("AdditionalInformation", (object) ((ResponseBase) submissionResponse.SubmissionResponses[0]).ErrorInfo);
      }
      IVssRequestContext requestContext1 = requestContext;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(requestContext1, "Microsoft.VisualStudio.Services.Gallery", "ESRP", properties);
    }
  }
}
