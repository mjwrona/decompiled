// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationSteps.SpamValidationStep
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.Server.Utility;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationSteps
{
  internal class SpamValidationStep : PackageValidationStepBase
  {
    private const PackageValidationStepBase.ValidationStepType s_stepType = PackageValidationStepBase.ValidationStepType.SpamValidation;

    public SpamValidationStep()
      : base(PackageValidationStepBase.ValidationStepType.SpamValidation)
    {
    }

    public override void BeginValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Stream packageStream)
    {
      this.m_result = ValidationStatus.InProgress;
      try
      {
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.CheckForExtensionSpam") && this.DoesExtensionOverviewContainBlockedHosts(requestContext, extension))
          throw new HttpException(400, GalleryResources.ExtensionMetadataHasSuspiciousContent());
        this.m_result = ValidationStatus.Success;
      }
      catch (Exception ex)
      {
        this.ResultMessage = ex.Message;
        this.m_result = ValidationStatus.Failure;
      }
    }

    public bool DoesExtensionOverviewContainBlockedHosts(
      IVssRequestContext requestContext,
      PublishedExtension publishedExtension)
    {
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.ScanOverviewContentForBlockedHosts") && publishedExtension.IsVsCodeExtension())
      {
        Publisher publisher;
        using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
          publisher = component.QueryPublisher(publishedExtension.Publisher.PublisherName, PublisherQueryFlags.None);
        UnpackagedExtensionData unpackagedExtensionData = this.GetUnpackagedExtensionData(publishedExtension);
        ExtensionFile extensionFile = publishedExtension.Versions[0].Files.Find((Predicate<ExtensionFile>) (x => x.AssetType == "Microsoft.VisualStudio.Services.Content.Details"));
        if (extensionFile != null)
        {
          string fileContent = GalleryServerUtil.GetFileContent(requestContext, extensionFile.FileId);
          try
          {
            if (!AntiSpamService.IsKnownGenuinePublisher(requestContext, publisher))
            {
              if (AntiSpamService.ContentHasBlockedHosts(requestContext, fileContent, unpackagedExtensionData, publisher, false))
                return true;
            }
          }
          catch (RegexMatchTimeoutException ex)
          {
            throw new HttpException(400, GalleryResources.ExtensionOverviewScanTimedOut());
          }
        }
      }
      return false;
    }

    public Publisher GetPublisherFromPublisherFact(PublisherFacts publisherFacts)
    {
      Publisher fromPublisherFact = new Publisher();
      fromPublisherFact.DisplayName = publisherFacts.DisplayName;
      fromPublisherFact.PublisherName = publisherFacts.PublisherName;
      return fromPublisherFact;
    }

    public UnpackagedExtensionData GetUnpackagedExtensionData(PublishedExtension extension) => new UnpackagedExtensionData()
    {
      ExtensionName = extension.ExtensionName,
      DisplayName = extension.DisplayName,
      Description = extension.ShortDescription
    };
  }
}
