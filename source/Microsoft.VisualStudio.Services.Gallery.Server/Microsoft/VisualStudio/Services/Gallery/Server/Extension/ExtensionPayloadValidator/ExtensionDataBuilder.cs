// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator.ExtensionDataBuilder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator
{
  internal class ExtensionDataBuilder
  {
    [StaticSafe("Grandfathered")]
    private static readonly IDictionary<VsExtensionType, string> CategoryToDepartmentMap = (IDictionary<VsExtensionType, string>) new Dictionary<VsExtensionType, string>()
    {
      {
        VsExtensionType.Tool,
        "Tools"
      },
      {
        VsExtensionType.Template,
        "Templates"
      },
      {
        VsExtensionType.Control,
        "Controls"
      }
    };

    public virtual UnpackagedExtensionData PrepareExtensionDataForCreate(
      UnpackagedExtensionData extensionData,
      PayloadValidationResult payloadValidationResult,
      Publisher publisher)
    {
      IDictionary<string, string> collection = this.BuildMetadata(extensionData, payloadValidationResult, publisher);
      extensionData.Lcids = extensionData.Lcids;
      extensionData.Metadata = new List<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) collection);
      this.PrepareExtensionFlags(extensionData, (PublishedExtension) null);
      this.BuildMetadataForVersionProperties(extensionData, payloadValidationResult);
      return extensionData;
    }

    private void BuildMetadataForVersionProperties(
      UnpackagedExtensionData extensionData,
      PayloadValidationResult payloadValidationResult)
    {
      string keyName = "PROPERTY::Microsoft.VisualStudio.Services.EnableMarketplaceQnA";
      string str = extensionData.QnAEnabled.ToString();
      extensionData.Metadata.RemoveAll((Predicate<KeyValuePair<string, string>>) (x => string.Equals(x.Key, keyName, StringComparison.OrdinalIgnoreCase)));
      extensionData.Metadata.Add(new KeyValuePair<string, string>(keyName, str));
      keyName = "PROPERTY::Microsoft.VisualStudio.Services.Payload.FileName";
      extensionData.Metadata.RemoveAll((Predicate<KeyValuePair<string, string>>) (x => string.Equals(x.Key, keyName, StringComparison.OrdinalIgnoreCase)));
      if (payloadValidationResult.DeploymentTechnology == ExtensionDeploymentTechnology.ReferralLink || payloadValidationResult.FileName.IsNullOrEmpty<char>())
        return;
      string fileName = payloadValidationResult.FileName;
      extensionData.Metadata.Add(new KeyValuePair<string, string>(keyName, fileName));
    }

    public virtual UnpackagedExtensionData PrepareExtensionDataForEdit(
      UnpackagedExtensionData extensionData,
      PayloadValidationResult payloadValidationResult,
      PublishedExtension existingExtension,
      Publisher publisher)
    {
      IDictionary<string, string> collection = this.BuildEditMetadata(extensionData, payloadValidationResult, existingExtension.Metadata, publisher);
      extensionData.Lcids = extensionData.Lcids;
      extensionData.Metadata = new List<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) collection);
      this.PrepareExtensionFlags(extensionData, existingExtension);
      this.BuildMetadataForVersionProperties(extensionData, payloadValidationResult);
      return extensionData;
    }

    private void PrepareExtensionFlags(
      UnpackagedExtensionData extensionData,
      PublishedExtension existingExtension)
    {
      extensionData.Flags = PublishedExtensionFlags.None;
      if (existingExtension != null && existingExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public))
        extensionData.Flags |= PublishedExtensionFlags.Public;
      if (string.Equals(extensionData.PricingCategory, "Paid", StringComparison.OrdinalIgnoreCase))
        extensionData.Flags |= PublishedExtensionFlags.Paid;
      if (string.Equals(extensionData.PricingCategory, "Trial", StringComparison.OrdinalIgnoreCase))
        extensionData.Flags |= PublishedExtensionFlags.Trial;
      if (!extensionData.IsPreview)
        return;
      extensionData.Flags |= PublishedExtensionFlags.Preview;
    }

    private IDictionary<string, string> BuildMetadata(
      UnpackagedExtensionData extensionData,
      PayloadValidationResult payloadValidationResult,
      Publisher publisher)
    {
      Dictionary<string, string> metadata = new Dictionary<string, string>();
      metadata["Type"] = payloadValidationResult.ExtensionType.ToString();
      metadata["Department"] = ExtensionDataBuilder.GetDepartment(payloadValidationResult.ExtensionType);
      metadata["AlliancePromotion"] = "false";
      metadata["PremierPromotion"] = "false";
      metadata["PartnerId"] = string.Empty;
      metadata["DeploymentTechnology"] = GalleryConstants.MetaDataDeploymentTechnologyTypes.GetDeployementTechnologyText(payloadValidationResult.DeploymentTechnology);
      metadata["ContentType"] = payloadValidationResult.ExtensionType.ToString();
      metadata["OriginalExtensionSource"] = "MarketPlace";
      metadata["Affiliation"] = this.GetAffiliationValue(publisher, (Dictionary<string, string>) null);
      if (!string.IsNullOrEmpty(extensionData.VsixId))
      {
        metadata["VsixId"] = extensionData.VsixId;
        metadata["VsixVersion"] = extensionData.Version;
      }
      metadata["DigitalSignatureValidationStatus"] = payloadValidationResult.IsSignedByMicrosoft ? "SignedByMicrosoft" : "NotSignedByMicrosoft";
      if (payloadValidationResult.DeploymentTechnology == ExtensionDeploymentTechnology.ReferralLink)
        metadata["ReferralUrl"] = extensionData.ReferralUrl;
      metadata["SourceCodeUrl"] = extensionData.RepositoryUrl ?? string.Empty;
      ExtensionDataBuilder.AddVsixMetadata(payloadValidationResult, (IDictionary<string, string>) metadata);
      return (IDictionary<string, string>) metadata;
    }

    private string GetAffiliationValue(Publisher publisher, Dictionary<string, string> metadata)
    {
      if (string.Equals(publisher.DisplayName, "Microsoft", StringComparison.OrdinalIgnoreCase))
        return "Microsoft";
      if (string.Equals(publisher.DisplayName, "Microsoft DevLabs", StringComparison.OrdinalIgnoreCase))
        return "DevLabs";
      return metadata != null && (bool.Parse(metadata["AlliancePromotion"]) || bool.Parse(metadata["PremierPromotion"])) ? "VSIPPartner" : "Community";
    }

    private static void AddVsixMetadata(
      PayloadValidationResult payloadValidationResult,
      IDictionary<string, string> metadata)
    {
      if (payloadValidationResult.VsixMetadata == null)
        return;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) payloadValidationResult.VsixMetadata)
      {
        if (keyValuePair.Key.Equals("ProjectSubType", StringComparison.OrdinalIgnoreCase))
          metadata["SubType"] = keyValuePair.Value ?? string.Empty;
        else
          metadata[keyValuePair.Key] = keyValuePair.Value ?? string.Empty;
      }
    }

    private IDictionary<string, string> BuildEditMetadata(
      UnpackagedExtensionData extensionData,
      PayloadValidationResult payloadValidationResult,
      List<ExtensionMetadata> existingMetadata,
      Publisher publisher)
    {
      Dictionary<string, string> metadata = new Dictionary<string, string>();
      foreach (ExtensionMetadata extensionMetadata in existingMetadata)
        metadata[extensionMetadata.Key] = extensionMetadata.Value;
      if (metadata.ContainsKey("ConvertedToMarkdown") && !bool.Parse(metadata["ConvertedToMarkdown"]))
        metadata["ConvertedToMarkdown"] = extensionData.IsConvertedToMarkdown.ToString();
      metadata["MigratedFromVSGallery"] = "false";
      metadata["DeploymentTechnology"] = GalleryConstants.MetaDataDeploymentTechnologyTypes.GetDeployementTechnologyText(payloadValidationResult.DeploymentTechnology);
      metadata["Affiliation"] = this.GetAffiliationValue(publisher, metadata);
      if (payloadValidationResult.DeploymentTechnology == ExtensionDeploymentTechnology.ReferralLink)
        metadata["ReferralUrl"] = extensionData.ReferralUrl;
      else if (metadata.ContainsKey("ReferralUrl"))
        metadata.Remove("ReferralUrl");
      if (!string.IsNullOrEmpty(extensionData.VsixId))
      {
        metadata["VsixId"] = extensionData.VsixId;
        metadata["VsixVersion"] = extensionData.Version;
      }
      else if (payloadValidationResult.DeploymentTechnology != ExtensionDeploymentTechnology.Vsix)
      {
        metadata.Remove("VsixId");
        metadata.Remove("VsixVersion");
      }
      if (!payloadValidationResult.PayloadVerificationSkipped)
        metadata["DigitalSignatureValidationStatus"] = payloadValidationResult.IsSignedByMicrosoft ? "SignedByMicrosoft" : "NotSignedByMicrosoft";
      ExtensionDataBuilder.AddVsixMetadata(payloadValidationResult, (IDictionary<string, string>) metadata);
      if (payloadValidationResult.VsixMetadata != null && !payloadValidationResult.VsixMetadata.ContainsKey("PackedExtensionsVsixIDs"))
        metadata.Remove("PackedExtensionsVsixIDs");
      metadata["SourceCodeUrl"] = extensionData.RepositoryUrl ?? string.Empty;
      return (IDictionary<string, string>) metadata;
    }

    private static string GetDepartment(VsExtensionType type)
    {
      string department;
      if (ExtensionDataBuilder.CategoryToDepartmentMap.TryGetValue(type, out department))
        return department;
      throw new ArgumentException("Invalid root category name");
    }
  }
}
