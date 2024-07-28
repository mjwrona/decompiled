// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator.ReferralLinkPayloadValidator
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
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator
{
  internal class ReferralLinkPayloadValidator : ExtensionPayloadValidatorBase
  {
    private const ExtensionDeploymentTechnology _supportedDeploymentTechnology = ExtensionDeploymentTechnology.ReferralLink;
    private const string _serviceLayer = "ReferralLinkPayloadValidator";
    internal const string BlockedReferralUrlEndsWithDefaultValue = ".vsix;.exe;.msi";
    internal const string BlockedReferralUrlEndsWithRegistryPath = "/Configuration/Service/Gallery/VsIde/BlockedReferralUrlEndsWith";

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
        DeploymentTechnology = ExtensionDeploymentTechnology.ReferralLink
      };
      payloadValidationResult.IsValid = this.PerformMicrosoftEmployeeCheck(requestContext, payloadValidationResult, publisher);
      return payloadValidationResult;
    }

    public override ExtensionPayload GetPayloadFromValidationResult(
      PayloadValidationResult payloadValidationResult)
    {
      return new ExtensionPayload()
      {
        Type = payloadValidationResult.DeploymentTechnology,
        IsValid = payloadValidationResult.IsValid,
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
      requestContext.Trace(12062075, TraceLevel.Info, "gallery", nameof (ReferralLinkPayloadValidator), "ExtensionValidation started for the extension : {0}", (object) extensionData.ExtensionName);
      if (existingExtension != null && !existingExtension.Lcids.IsNullOrEmpty<int>())
        extensionData.Lcids = existingExtension.Lcids;
      else
        extensionData.Lcids = new List<int>() { 1033 };
      PayloadValidationResult payloadValidationResult = this.ValidatePayload(requestContext, payloadStream, fileName, publisher, existingExtension);
      if (payloadValidationResult.IsValid)
      {
        bool flag1 = true;
        if (!Uri.IsWellFormedUriString(extensionData.ReferralUrl, UriKind.Absolute))
        {
          payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("UploadFile", GalleryResources.LinkMustBeValidUrl()));
          flag1 = false;
        }
        if (flag1 && requestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.CheckForExtensionBlockedHosts") && AntiSpamService.ExtensionHasBlockedHost(requestContext, extensionData, publisher))
          {
            payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("ExtensionHasBlockedHosts", GalleryResources.ExtensionContainsBlockedHosts()));
            flag1 = false;
          }
          if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.CheckForExtensionSpam") && !AntiSpamService.IsKnownGenuinePublisher(requestContext, publisher) && AntiSpamService.ExtensionHasSuspectedSpamContent(requestContext, extensionData, publisher))
          {
            payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("extensionHasSuspiciousContent", GalleryResources.ExtensionMetadataHasSuspiciousContent()));
            flag1 = false;
          }
          if (this.HasBlockedReferralUrlEndsWith(requestContext, extensionData))
          {
            string str = existingExtension == null ? "Created" : "Updated";
            CustomerIntelligenceData properties = new CustomerIntelligenceData();
            properties.Add(CustomerIntelligenceProperty.Action, str);
            properties.Add("PublisherName", extensionData.PublisherName);
            properties.Add("ExtensionName", extensionData.ExtensionName);
            properties.Add("ExtensionState", GalleryServerUtil.GetExtensionState(extensionData.Flags));
            properties.Add("Version", extensionData.Version);
            properties.Add("ReferralUrl", extensionData.ReferralUrl);
            properties.Add("IsReferralLinkToHostedFile", true);
            requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "Extension", properties);
          }
        }
        bool flag2 = this.PerformCommonValidations(requestContext, payloadValidationResult, extensionData, existingExtension) & flag1;
        bool flag3 = this.PerformNonVsixValidations(payloadValidationResult, extensionData) & flag2;
        if (flag3)
        {
          if (payloadValidationResult.ExtensionType == VsExtensionType.Template)
          {
            payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("UploadFile", GalleryResources.ValidationMessageForTemplateNotSupportedWithLinkType()));
            flag3 = false;
          }
          IFirstPartyPublisherAccessService service = requestContext.GetService<IFirstPartyPublisherAccessService>();
          if (extensionData.InstallationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (x => x.Target.ToUpperInvariant().Contains("express".ToUpperInvariant()))) && payloadValidationResult.ExtensionType == VsExtensionType.Tool && !service.IsMicrosoftPublisher(requestContext, publisher))
          {
            flag3 = false;
            payloadValidationResult.ValidationErrors.Add(new KeyValuePair<string, string>("UploadFile", GalleryResources.ToolCannotUseExpress()));
          }
        }
        payloadValidationResult.IsValid = flag3;
      }
      requestContext.Trace(12062075, TraceLevel.Info, "gallery", nameof (ReferralLinkPayloadValidator), "ExtensionValidation completed for the extension : {0}", (object) extensionData.ExtensionName);
      return payloadValidationResult;
    }

    private bool HasBlockedReferralUrlEndsWith(
      IVssRequestContext requestContext,
      UnpackagedExtensionData extensionData)
    {
      RegistryQuery query = new RegistryQuery("/Configuration/Service/Gallery/VsIde/BlockedReferralUrlEndsWith");
      return ((IEnumerable<string>) requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, in query, ".vsix;.exe;.msi").Trim().Split(new string[1]
      {
        ";"
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (value => value.Trim())).ToList<string>().Any<string>((Func<string, bool>) (fileExtension => extensionData.ReferralUrl.EndsWith(fileExtension, true, CultureInfo.InvariantCulture)));
    }
  }
}
