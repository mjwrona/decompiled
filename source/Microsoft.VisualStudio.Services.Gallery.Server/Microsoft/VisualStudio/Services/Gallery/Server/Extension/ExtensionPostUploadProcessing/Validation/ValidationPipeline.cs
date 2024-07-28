// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationPipeline
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.CVS;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationSteps;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.TagGenerators;
using Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IndexUpdater;
using Microsoft.VisualStudio.Services.Gallery.Server.Facade.Validation;
using Microsoft.VisualStudio.Services.Gallery.Server.Telemetry;
using Microsoft.VisualStudio.Services.Gallery.Server.Utility;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation
{
  public class ValidationPipeline
  {
    private string m_validatorBasePath;
    private IList<IValidationPipelineStep> m_validationPipelineSteps;
    private string m_resultMessage = string.Empty;

    public ValidationPipeline()
    {
    }

    internal ValidationPipeline(
      IList<IValidationPipelineStep> validationPipelineSteps)
    {
      this.m_validationPipelineSteps = validationPipelineSteps;
    }

    public string Run(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Stream packageStream,
      Guid validationId,
      Guid vsid)
    {
      try
      {
        this.m_validatorBasePath = Path.Combine(FileSpec.GetTempDirectory(), "VersionValidator");
        Directory.CreateDirectory(this.m_validatorBasePath);
        if (this.PreValidationCheck(requestContext, extension, validationId))
        {
          if (this.IsValidationRequired(extension))
          {
            this.CleanupExisitingValidationSteps(requestContext, validationId);
            this.InitializeSteps(requestContext, extension);
            this.InitializeValidation(requestContext, validationId);
            this.BeginValidation(requestContext, extension, packageStream);
          }
          this.ProcessValidationResult(requestContext, extension, validationId, vsid);
        }
      }
      catch (ExtensionDoesNotExistException ex)
      {
        this.m_resultMessage = ex.ToString();
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          string action = "PublishTimeError";
          requestContext.GetService<IGalleryTelemetryHelperService>().PublishAppInsightsPerExtensionTelemetryHelper(requestContext, extension, action);
        }
        requestContext.TraceException(12061094, "Gallery", "ProcessExtension", (Exception) ex);
      }
      return this.m_resultMessage;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "This is for addition of CDN job addition. It is required here.")]
    public string ProcessValidationResult(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Guid validationId,
      Guid vsid)
    {
      if (extension != null)
      {
        TimeSpan recheckTimeSpan = TimeSpan.MaxValue;
        bool isVsExtension = extension.IsVsExtension();
        int num = this.IsValidationRequired(extension) ? 1 : 0;
        bool flag1 = true;
        bool flag2 = true;
        bool flag3 = false;
        bool flag4 = false;
        IPublishedExtensionService service1 = requestContext.GetService<IPublishedExtensionService>();
        IEnumerable<ExtensionVersionValidationStep> validationStepsByParent = service1.GetAllValidationStepsByParent(requestContext, validationId);
        ValidationPipelineStepFactory stepFactory = new ValidationPipelineStepFactory();
        bool flag5 = false;
        if (num != 0 && validationStepsByParent != null)
        {
          foreach (ExtensionVersionValidationStep validationStep in validationStepsByParent)
          {
            if (validationStep.StepType == 0 && this.IsStepComplete(validationStep.StepStatus))
              this.SendRequestToUnifiedValidationPipeline(requestContext, extension, validationId);
            if (!this.IsStepComplete(validationStep.StepStatus))
            {
              flag3 = flag3 || validationStep.StepStatus == 6;
              IValidationPipelineStep validationPipelineStep = stepFactory.GetValidationPipelineStep(requestContext, extension, validationStep);
              if (!(3 == validationStep.StepType & flag5))
                this.UpdatePipelineStepStatus(requestContext, extension, validationPipelineStep, validationStep);
              TimeSpan recheckTime = validationPipelineStep.GetRecheckTime(requestContext);
              if (recheckTime < recheckTimeSpan)
                recheckTimeSpan = recheckTime;
            }
            if (!this.IsStepComplete(validationStep.StepStatus))
            {
              flag1 = false;
              flag4 = flag4 || validationStep.StepStatus == 6;
            }
            else if (validationStep.StepStatus != 2)
              flag2 = false;
            if (this.IsRepositorySigningEnabled(requestContext, extension) && !this.IsRepositorySigningCompleted(validationStepsByParent) && 1 == validationStep.StepType && this.IsStepComplete(validationStep.StepStatus) && validationStep.StepStatus == 2)
            {
              RepositorySigningStep repositorySigningStep = this.GetIncompleteRepositorySigningStep(requestContext, extension, validationStepsByParent, stepFactory);
              if (repositorySigningStep != null)
              {
                repositorySigningStep.StartRepositorySigning(requestContext, extension, false);
                if (repositorySigningStep.RetrieveResult(requestContext, extension) == ValidationStatus.Failure)
                  flag2 = false;
                flag5 = true;
                this.UpdateRepositorySigningPipelineStepStatus(requestContext, extension, validationStepsByParent, (IValidationPipelineStep) repositorySigningStep);
              }
            }
          }
        }
        if (flag2)
        {
          if (flag1)
          {
            bool updateExtension = false;
            bool flag6 = false;
            if (isVsExtension && !extension.Metadata.IsNullOrEmpty<ExtensionMetadata>())
            {
              ExtensionVersion version = extension.Versions[0];
              foreach (ExtensionMetadata extensionMetadata in extension.Metadata)
              {
                if (extensionMetadata.Key.StartsWith("PROPERTY::", StringComparison.OrdinalIgnoreCase))
                {
                  string propertyKey = extensionMetadata.Key.Substring("PROPERTY::".Length);
                  if (version.Properties.IsNullOrEmpty<KeyValuePair<string, string>>())
                    version.Properties = new List<KeyValuePair<string, string>>();
                  else
                    version.Properties.RemoveAll((Predicate<KeyValuePair<string, string>>) (x => string.Equals(x.Key, propertyKey, StringComparison.OrdinalIgnoreCase)));
                  version.Properties.Add(new KeyValuePair<string, string>(propertyKey, extensionMetadata.Value));
                  updateExtension = true;
                }
              }
            }
            if (isVsExtension || requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.AssetUploadToCDNPostValidation"))
              service1.UpdateExtensionCDNProperties(requestContext, extension.ExtensionId, extension.Versions[0].Version, targetPlatform: extension.Versions[0].TargetPlatform);
            service1.ProcessValidationResult(requestContext, extension.ExtensionId, extension.Versions[0].Version, extension.Versions[0].TargetPlatform, validationId, extension.Flags, string.Empty, true, extension, updateExtension, isVsExtension, new int?());
            if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnablePassiveCVSScan"))
              CVSJobHelper.ScheduleSubmitScanJob(requestContext, extension.ExtensionId.ToString(), vsid.ToString());
            this.SendMailNotificationToPublisher(requestContext, extension, ValidationStatus.Success);
            if (isVsExtension)
            {
              flag6 = this.QueueAssetUploadToCDNJob(requestContext, extension);
            }
            else
            {
              this.QueueAutoTaggingJob(requestContext, extension);
              if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.AssetUploadToCDNPostValidation"))
                flag6 = this.QueueAssetUploadToCDNJob(requestContext, extension);
            }
            if (extension.IsVsCodeExtension() && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVSCodeWebTagPopulatorJob"))
              service1.QueueVSCodeWebExtensionTagPopulatorJob(requestContext, extension);
            if (!flag6)
              service1.PublishExtensionCreateUpdateNotifications(requestContext, true, extension);
            if (extension.IsVsCodeExtension() && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnablePMPIntegration") && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVSCodeExtensionUploadEventPublishing"))
            {
              IExtensionNameConflictService service2 = requestContext.GetService<IExtensionNameConflictService>();
              string extensionName1 = extension.ExtensionName;
              string extensionName2 = extension.Publisher.PublisherName + "." + extension.ExtensionName;
              string existsInConflictList = service2.GetNewExtensionNameIfExistsInConflictList(extensionName2);
              if (!string.IsNullOrEmpty(existsInConflictList))
                extensionName1 = existsInConflictList;
              requestContext.GetService<IEventPublisherService>().PublishArtifactFilePublishedEvent(requestContext, extensionName1, extension.Versions[0].Version, extension.Versions[0].TargetPlatform);
            }
          }
          else
          {
            if (!flag3 & flag4)
              this.SendMailNotificationToPublisher(requestContext, extension, ValidationStatus.PendingAnalysis);
            this.QueueValidationStatusCheckJob(requestContext, extension, validationId, vsid, recheckTimeSpan);
          }
        }
        else
        {
          foreach (ExtensionVersionValidationStep validationStep in validationStepsByParent)
          {
            if (!this.IsStepComplete(validationStep.StepStatus))
            {
              IValidationPipelineStep validationPipelineStep = stepFactory.GetValidationPipelineStep(requestContext, extension, validationStep);
              validationStep.StepStatus = 5;
              validationStep.LastUpdated = DateTime.UtcNow;
              IVssRequestContext requestContext1 = requestContext;
              PublishedExtension extension1 = extension;
              validationPipelineStep.PostValidation(requestContext1, extension1);
              service1.UpdateVersionValidationStep(requestContext, validationStep);
            }
          }
          int pushStreamContent = this.GetZipPushStreamContent(requestContext, validationStepsByParent);
          service1.ProcessValidationResult(requestContext, extension.ExtensionId, extension.Versions[0].Version, extension.Versions[0].TargetPlatform, validationId, PublishedExtensionFlags.None, this.m_resultMessage, false, (PublishedExtension) null, false, false, new int?(pushStreamContent));
          service1.PublishExtensionCreateUpdateNotifications(requestContext, false, extension);
          this.SendMailNotificationToPublisher(requestContext, extension, ValidationStatus.Failure);
        }
      }
      return this.m_resultMessage;
    }

    private RepositorySigningStep GetIncompleteRepositorySigningStep(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      IEnumerable<ExtensionVersionValidationStep> validationSteps,
      ValidationPipelineStepFactory stepFactory)
    {
      foreach (ExtensionVersionValidationStep validationStep in validationSteps)
      {
        if (!this.IsStepComplete(validationStep.StepStatus))
        {
          IValidationPipelineStep validationPipelineStep = stepFactory.GetValidationPipelineStep(requestContext, extension, validationStep);
          if (StepType.RepositorySigning == validationPipelineStep.StepType)
            return (RepositorySigningStep) validationPipelineStep;
        }
      }
      return (RepositorySigningStep) null;
    }

    private bool IsRepositorySigningCompleted(
      IEnumerable<ExtensionVersionValidationStep> validationSteps)
    {
      foreach (ExtensionVersionValidationStep validationStep in validationSteps)
      {
        if (3 == validationStep.StepType && this.IsStepComplete(validationStep.StepStatus))
          return true;
      }
      return false;
    }

    private bool IsRepositorySigningEnabled(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      return requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableRepositorySigningForVSCode") && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVirusScan") && extension.IsVsCodeExtension();
    }

    private void SendRequestToUnifiedValidationPipeline(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Guid validationId)
    {
      try
      {
        if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableUVPScan") || !extension.IsVsCodeExtension())
          return;
        requestContext.GetService<IUnifiedValidationPipelineService>().SendMessage(requestContext, extension, validationId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062081, "Gallery", nameof (SendRequestToUnifiedValidationPipeline), ex);
      }
    }

    [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "http://stackoverflow.com/questions/10139718/code-anlysis-rule-ca2000-ca2202")]
    private int GetZipPushStreamContent(
      IVssRequestContext requestContext,
      IEnumerable<ExtensionVersionValidationStep> validationSteps)
    {
      ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
      using (MemoryStream content = new MemoryStream())
      {
        using (ZipArchive zipArchive = new ZipArchive((Stream) content, ZipArchiveMode.Create, true))
        {
          int num = 0;
          foreach (ExtensionVersionValidationStep validationStep in validationSteps)
          {
            if (validationStep.StepStatus == 3 && validationStep.ResultFileId != 0)
            {
              using (Stream stream1 = service.RetrieveFile(requestContext, (long) validationStep.ResultFileId, false, out byte[] _, out long _, out CompressionType _))
              {
                ++num;
                StepType stepType = (StepType) validationStep.StepType;
                using (Stream destination = zipArchive.CreateEntry(num.ToString() + "_" + stepType.ToString() + ".log").Open())
                {
                  using (Stream stream2 = stream1)
                    stream2.CopyTo(destination);
                }
              }
            }
          }
        }
        content.Seek(0L, SeekOrigin.Begin);
        return service.UploadFile(requestContext, (Stream) content, OwnerId.Gallery, Guid.Empty);
      }
    }

    private bool PreValidationCheck(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Guid validationId)
    {
      if (extension == null || extension.Versions == null || extension.Versions.Count != 1)
        throw new ExtensionDoesNotExistException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Extension not found"));
      ExtensionVersionValidation versionValidation = requestContext.GetService<IPublishedExtensionService>().QueryVersionValidation(requestContext, extension.ExtensionId, extension.Versions[0].Version, extension.Versions[0].TargetPlatform);
      if (versionValidation != null && versionValidation.ValidationId.Equals(validationId))
        return true;
      this.m_resultMessage = "Version validation information for queued job does not exist.";
      return false;
    }

    private void CleanupExisitingValidationSteps(
      IVssRequestContext requestContext,
      Guid validationId)
    {
      requestContext.GetService<IPublishedExtensionService>().DeleteValidationStepsByParentId(requestContext, validationId);
    }

    protected virtual void InitializeSteps(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      if (!this.m_validationPipelineSteps.IsNullOrEmpty<IValidationPipelineStep>())
        return;
      this.m_validationPipelineSteps = new ValidationPipelineStepFactory().GetValidationPipelineSteps(requestContext, extension);
    }

    private void InitializeValidation(IVssRequestContext requestContext, Guid validationId)
    {
      foreach (IValidationPipelineStep validationPipelineStep in (IEnumerable<IValidationPipelineStep>) this.m_validationPipelineSteps)
      {
        Guid guid1 = validationId;
        Guid guid2 = Guid.NewGuid();
        validationPipelineStep.Initialize(guid1, guid2);
        int stepType = (int) validationPipelineStep.StepType;
        int stepStatus = (int) validationPipelineStep.RetrieveResult(requestContext);
        DateTime utcNow = DateTime.UtcNow;
        requestContext.GetService<IPublishedExtensionService>().CreateVersionValidationStep(requestContext, guid2, guid1, stepType, stepStatus, utcNow);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We want to catch the exception and show it as part of result")]
    private void BeginValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Stream packageStream)
    {
      foreach (IValidationPipelineStep validationPipelineStep in (IEnumerable<IValidationPipelineStep>) this.m_validationPipelineSteps)
      {
        ExtensionVersionValidationStep validationStep = new ExtensionVersionValidationStep();
        validationStep.StepId = validationPipelineStep.ValidationId;
        validationStep.ParentId = validationPipelineStep.ParentValidationId;
        validationStep.StepType = (int) validationPipelineStep.StepType;
        try
        {
          validationStep.ValidationContext = validationPipelineStep.BeginValidation(requestContext, extension, packageStream);
          validationStep.StartTime = validationPipelineStep.StartTime;
          this.UpdatePipelineStepStatus(requestContext, extension, validationPipelineStep, validationStep);
        }
        catch (Exception ex)
        {
          validationStep.StepStatus = 3;
          this.m_resultMessage += GalleryResources.PublishFailureGeneric();
          validationStep.ResultFileId = this.GenerateLogFileFromString(requestContext, GalleryResources.PublishFailureGeneric());
          requestContext.TraceException(12061094, "Gallery", nameof (BeginValidation), ex);
          requestContext.GetService<IPublishedExtensionService>().UpdateVersionValidationStep(requestContext, validationStep);
        }
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We want to catch the exception and show it as part of result")]
    private void UpdatePipelineStepStatus(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      IValidationPipelineStep validationPipelineStep,
      ExtensionVersionValidationStep validationStep)
    {
      try
      {
        validationStep.StepStatus = (int) validationPipelineStep.RetrieveResult(requestContext, extension);
        validationStep.ValidationContext = validationPipelineStep.ValidationContext;
        validationStep.StartTime = validationPipelineStep.StartTime;
        validationStep.LastUpdated = DateTime.UtcNow;
        if (!this.IsStepComplete(validationStep.StepStatus))
          return;
        validationPipelineStep.PostValidation(requestContext, extension);
        string message = validationPipelineStep.RetrieveResultMessage();
        if (validationStep.StepStatus != 2)
          validationStep.ResultFileId = this.GenerateLogFileFromString(requestContext, message);
        this.m_resultMessage += message;
      }
      catch (Exception ex)
      {
        validationStep.StepStatus = 3;
        this.m_resultMessage += GalleryResources.PublishFailureGeneric();
        validationStep.ResultFileId = this.GenerateLogFileFromString(requestContext, GalleryResources.PublishFailureGeneric());
        requestContext.TraceException(12061094, "Gallery", nameof (UpdatePipelineStepStatus), ex);
      }
      finally
      {
        requestContext.GetService<IPublishedExtensionService>().UpdateVersionValidationStep(requestContext, validationStep);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We want to catch the exception and show it as part of result")]
    private void UpdateRepositorySigningPipelineStepStatus(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      IEnumerable<ExtensionVersionValidationStep> validationSteps,
      IValidationPipelineStep validationPipelineStep)
    {
      foreach (ExtensionVersionValidationStep validationStep in validationSteps)
      {
        if (3 == validationStep.StepType)
          this.UpdatePipelineStepStatus(requestContext, extension, validationPipelineStep, validationStep);
      }
    }

    private int GenerateLogFileFromString(IVssRequestContext requestContext, string message)
    {
      using (Stream streamFromString = ValidationPipeline.GenerateStreamFromString(message))
        return requestContext.GetService<ITeamFoundationFileService>().UploadFile(requestContext, streamFromString, OwnerId.Gallery, Guid.Empty);
    }

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Instance is disposed elsewhere")]
    private static Stream GenerateStreamFromString(string s)
    {
      MemoryStream streamFromString = new MemoryStream();
      StreamWriter streamWriter = new StreamWriter((Stream) streamFromString);
      streamWriter.Write(s);
      streamWriter.Flush();
      streamFromString.Position = 0L;
      return (Stream) streamFromString;
    }

    private void QueueAutoTaggingJob(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Generate Tags for product (extension: {0})", (object) extension.ExtensionId.ToString());
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new TagPopulatorJobData()
      {
        ExtensionName = extension.GetFullyQualifiedName(),
        Product = (string) null
      });
      IVssRequestContext requestContext1 = requestContext;
      string jobName = str;
      XmlNode jobData = xml;
      service.QueueOneTimeJob(requestContext1, jobName, "Microsoft.VisualStudio.Services.Gallery.Extensions.JobAgentPlugins.TagPopulatorJob", jobData, false);
    }

    private bool QueueAssetUploadToCDNJob(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      bool cdnJob = false;
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return cdnJob;
      if (extension == null || extension.Versions == null || extension.Versions.Count != 1)
        throw new ExtensionDoesNotExistException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Extension not found"));
      requestContext.Trace(12061115, TraceLevel.Info, "Gallery", "ProcessExtension", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Uploading Assets for extension: {0}.{1} version {2} targetPlatform {3}", (object) extension.Publisher.PublisherName, (object) extension.ExtensionName, (object) extension.Versions[0].Version, (object) extension.Versions[0].TargetPlatform));
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new UploadExtensionAssetsToCdnJobData()
      {
        PublisherName = extension.Publisher.PublisherName,
        ExtensionName = extension.ExtensionName,
        Version = extension.Versions[0].Version,
        TargetPlatform = extension.Versions[0].TargetPlatform,
        NoCompression = extension.IsVsExtension()
      });
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "UploadToCDNJob for ext {0} and ver {1}", (object) extension.ExtensionId, (object) extension.Versions[0].Version);
      IVssRequestContext requestContext1 = requestContext;
      string jobName = str;
      XmlNode jobData = xml;
      service.QueueOneTimeJob(requestContext1, jobName, "Microsoft.VisualStudio.Services.Gallery.Extensions.UploadExtensionAssetsToCDNJob", jobData, true);
      return true;
    }

    private bool IsStepComplete(int status) => status == 2 || status == 3 || status == 4;

    private bool IsValidationRequired(PublishedExtension extension) => !extension.IsVsExtension() || !extension.IsMigratedFromVSGallery();

    private void QueueValidationStatusCheckJob(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Guid validationId,
      Guid vsid,
      TimeSpan recheckTimeSpan)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) new ExtensionProcessingJobData()
      {
        ExtensionId = extension.ExtensionId,
        PublisherName = extension.Publisher.PublisherName,
        ExtensionName = extension.ExtensionName,
        Version = extension.Versions[0].Version,
        TargetPlatform = extension.Versions[0].TargetPlatform,
        ValidationId = validationId,
        Vsid = new Guid?(vsid)
      });
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Check Validation Status (validationId: {0})", (object) validationId);
      IVssRequestContext requestContext1 = requestContext;
      string jobName = str;
      XmlNode jobData = xml;
      TimeSpan startOffset = recheckTimeSpan;
      service.QueueOneTimeJob(requestContext1, jobName, "Microsoft.VisualStudio.Services.Gallery.Extensions.VersionValidationStatusJob", jobData, startOffset);
    }

    private void SendMailNotificationToPublisher(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      ValidationStatus validationStatus)
    {
      try
      {
        if (extension == null || extension.Publisher == null || extension.Versions.IsNullOrEmpty<ExtensionVersion>() || extension.IsMSExtension())
          return;
        VersionValidationMailNotificationEvent notificationEventData = new VersionValidationMailNotificationEvent(requestContext);
        notificationEventData.ExtensionName = extension.DisplayName;
        notificationEventData.ExtensionFQName = extension.GetFullyQualifiedName();
        notificationEventData.Version = extension.Versions[0].Version;
        notificationEventData.DetailsPageUrl = GalleryServerUtil.GetGalleryDetailsPageUrl(requestContext, extension.Publisher.PublisherName, extension.ExtensionName);
        notificationEventData.ActionButtonUrl = GalleryServerUtil.GetManagePageUrl(requestContext, extension.Publisher.PublisherName);
        notificationEventData.LogFileUrl = GalleryServerUtil.GetValidationLogsUrl(requestContext, extension);
        notificationEventData.ValidationStatus = validationStatus;
        notificationEventData.IsCvs = "false";
        notificationEventData.TargetPlatform = extension.Versions[0].TargetPlatform != null ? GalleryServerUtil.GetAllVSCodeTargetPlatformPairs(requestContext)[extension.Versions[0].TargetPlatform] : (string) null;
        new MailNotification().SendExtensionPublishMailNotification(requestContext, extension, notificationEventData, new Guid());
      }
      catch (Exception ex)
      {
        string str1 = "null";
        string str2 = "null";
        if (!extension.Versions.IsNullOrEmpty<ExtensionVersion>())
        {
          str1 = extension.Versions[0].Version;
          str2 = extension.Versions[0].TargetPlatform;
        }
        string message = "Failed to send mail notification to publisher for ExtensionFQN:" + (extension != null ? extension.GetFullyQualifiedName() : (string) null) + ", version: " + str1 + ", targetPlatform: " + str2 + ", validationStatus: " + validationStatus.ToString();
        requestContext.Trace(12061094, TraceLevel.Error, "Gallery", nameof (SendMailNotificationToPublisher), message);
        requestContext.TraceException(12061094, "Gallery", nameof (SendMailNotificationToPublisher), ex);
      }
    }
  }
}
