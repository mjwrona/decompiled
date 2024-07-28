// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator.MsiExePayloadValidator
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Utility;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator
{
  internal class MsiExePayloadValidator : ExtensionPayloadValidatorBase
  {
    private readonly ExtensionDeploymentTechnology _supportedDeploymentTechnology;
    private const string _serviceLayer = "MsiExePayloadValidator";

    public MsiExePayloadValidator(ExtensionDeploymentTechnology deploymentTechnology) => this._supportedDeploymentTechnology = deploymentTechnology;

    public override PayloadValidationResult ValidatePayload(
      IVssRequestContext requestContext,
      Stream payloadStream,
      string fileName,
      Publisher publisher,
      PublishedExtension existingExtension)
    {
      PayloadValidationResult payloadValidationResult = new PayloadValidationResult()
      {
        ValidationErrors = new List<KeyValuePair<string, string>>(),
        FileName = fileName,
        DeploymentTechnology = this._supportedDeploymentTechnology
      };
      payloadValidationResult.IsValid = this.PerformMicrosoftEmployeeCheck(requestContext, payloadValidationResult, publisher);
      payloadValidationResult.IsSignedByMicrosoft = false;
      return payloadValidationResult;
    }

    public override ExtensionPayload GetPayloadFromValidationResult(
      PayloadValidationResult payloadValidationResult)
    {
      return new ExtensionPayload()
      {
        FileName = payloadValidationResult.FileName,
        Type = payloadValidationResult.DeploymentTechnology,
        IsValid = payloadValidationResult.IsValid,
        IsSignedByMicrosoft = payloadValidationResult.IsSignedByMicrosoft,
        Metadata = new List<KeyValuePair<string, string>>()
      };
    }

    public override PayloadValidationResult ValidatePayloadDetails(
      IVssRequestContext requestContext,
      Stream payloadStream,
      string fileName,
      UnpackagedExtensionData extensionData,
      Publisher publisher,
      PublishedExtension existingExtension)
    {
      if (existingExtension != null && !existingExtension.Lcids.IsNullOrEmpty<int>())
      {
        extensionData.Lcids = existingExtension.Lcids;
      }
      else
      {
        extensionData.Lcids = new List<int>();
        extensionData.Lcids.Add(1033);
      }
      PayloadValidationResult payloadValidationResult = this.ValidatePayload(requestContext, payloadStream, fileName, publisher, existingExtension);
      if (payloadValidationResult.IsValid)
      {
        bool flag1 = this.PerformCommonValidations(requestContext, payloadValidationResult, extensionData, existingExtension);
        bool flag2 = this.PerformNonVsixValidations(payloadValidationResult, extensionData) & flag1;
        if (flag2)
        {
          if (payloadValidationResult.ExtensionType != VsExtensionType.Control && payloadValidationResult.ExtensionType != VsExtensionType.Tool)
          {
            payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("UploadFile", GalleryResources.ValidationMessageForIncorrectMsiExePayload()));
            flag2 = false;
          }
          IFirstPartyPublisherAccessService service = requestContext.GetService<IFirstPartyPublisherAccessService>();
          if (extensionData.InstallationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (x => x.Target.ToUpperInvariant().Contains("express".ToUpperInvariant()))) && payloadValidationResult.ExtensionType == VsExtensionType.Tool && !service.IsMicrosoftPublisher(requestContext, publisher))
          {
            flag2 = false;
            payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("UploadFile", GalleryResources.ToolCannotUseExpress()));
          }
        }
        if (flag2 && requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.CheckForExtensionSpam") && !AntiSpamService.IsKnownGenuinePublisher(requestContext, publisher))
        {
          requestContext.Trace(12062075, TraceLevel.Info, "gallery", nameof (MsiExePayloadValidator), "ExtensionSpamWords validation started for the extensions : {0}", (object) extensionData.ExtensionName);
          if (AntiSpamService.ExtensionHasSuspectedSpamContent(requestContext, extensionData, publisher))
          {
            payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("extensionHasSuspiciousContent", GalleryResources.ExtensionMetadataHasSuspiciousContent()));
            flag2 = false;
          }
        }
        payloadValidationResult.IsValid = flag2;
      }
      return payloadValidationResult;
    }
  }
}
