// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.MlSpamValidationStep
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation;
using Microsoft.VisualStudio.Services.Gallery.Server.Utility;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class MlSpamValidationStep : PackageValidationStepBase
  {
    private const PackageValidationStepBase.ValidationStepType s_stepType = PackageValidationStepBase.ValidationStepType.MlSpamValidation;

    public MlSpamValidationStep()
      : base(PackageValidationStepBase.ValidationStepType.MlSpamValidation)
    {
    }

    public override void BeginValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Stream packageStream)
    {
      this.m_result = ValidationStatus.InProgress;
      bool enableFailureOnSpam = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableFailureOnMlSpamValidation");
      try
      {
        bool isSpam = this.CheckMlModelForSpam(requestContext, extension);
        if (isSpam & enableFailureOnSpam)
        {
          this.ResultMessage = "Extension metadata has malicious content";
          this.m_result = ValidationStatus.Failure;
        }
        else
          this.m_result = ValidationStatus.Success;
        this.LogDetailedCILog(requestContext, extension.ExtensionId, extension.ExtensionName, isSpam, enableFailureOnSpam, false);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062099, "Gallery", nameof (MlSpamValidationStep), ex);
        this.m_result = ValidationStatus.Success;
        this.LogDetailedCILog(requestContext, extension.ExtensionId, extension.ExtensionName, false, enableFailureOnSpam, true);
      }
    }

    public bool CheckMlModelForSpam(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension)
    {
      UnpackagedExtensionData unpackagedExtensionData = this.GetUnpackagedExtensionData(publishedExtension);
      ExtensionFile extensionFile = publishedExtension.Versions[0].Files.Find((Predicate<ExtensionFile>) (x => "Microsoft.VisualStudio.Services.Content.Details".Equals(x.AssetType)));
      if (extensionFile == null)
        throw new ArgumentException("publishedExtension details asset is null ", "detailsAsset");
      Publisher publisher;
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
        publisher = component.QueryPublisher(publishedExtension.Publisher.PublisherName, PublisherQueryFlags.None);
      string fileContent = GalleryServerUtil.GetFileContent(requestContext, extensionFile.FileId);
      IMlSpamValidationService service = requestContext.GetService<IMlSpamValidationService>();
      if (service == null)
        throw new ArgumentException("service object initialized to null ", "MlSpamValidationService");
      return !AntiSpamService.IsKnownGenuinePublisher(requestContext, publisher) && service.IsSpamOrNot(requestContext, unpackagedExtensionData, fileContent);
    }

    public UnpackagedExtensionData GetUnpackagedExtensionData(PublishedExtension extension) => new UnpackagedExtensionData()
    {
      ExtensionName = extension.ExtensionName,
      DisplayName = extension.DisplayName,
      Description = extension.ShortDescription
    };

    private void LogDetailedCILog(
      IVssRequestContext requestContext,
      Guid extensionId,
      string extensionName,
      bool isSpam,
      bool enableFailureOnSpam,
      bool exceptionOccured)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("ExtensionId", (object) extensionId);
      properties.Add("ExtensionName", extensionName);
      properties.Add(nameof (isSpam), isSpam);
      properties.Add("ValidationFailedOnSpam", enableFailureOnSpam);
      properties.Add("ExceptionOccured", exceptionOccured);
      properties.Add("ResultMessage", this.ResultMessage);
      properties.Add("ValidationStatus", (object) this.m_result);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "MlSpamValidation", properties);
    }
  }
}
