// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionDraftServiceHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class ExtensionDraftServiceHelper
  {
    private const string EnableVsixConsolidationWarningMessageKey = "EnableConsolidation";

    public virtual void ValidateExtensionVersionImmutabilityOnUpload(
      IVssRequestContext requestContext,
      string newExtensionVersion,
      PublishedExtension existingExtension,
      ExtensionDraft extensionDraft)
    {
      List<InstallationTarget> installationTargets = extensionDraft.Payload.InstallationTargets;
      if (GalleryServerUtil.IsVsixConsolidationEnabledForVsExtension(existingExtension.Metadata))
      {
        this.ValidateExtensionVersionImmutabilityForConsolidatedVsExtension(requestContext, existingExtension, newExtensionVersion, installationTargets);
        if (!this.IsConsolidationAllowedForGivenExtensionOrUploadedPayloadVsVersion(requestContext, existingExtension, installationTargets))
          throw new VsVersionNotAllowedForConsolidationException(GalleryResources.VsVersionNotAllowedForConsolidationException());
      }
      else
      {
        if (!this.ShouldEnableVsixConsolidation(requestContext, existingExtension, newExtensionVersion, installationTargets) || !this.IsConsolidationAllowedForGivenExtensionOrUploadedPayloadVsVersion(requestContext, existingExtension, installationTargets))
          return;
        this.PublishCustomerIntelligenceEvent(requestContext, "EnableVsixConsolidationWarningOnUpload", newExtensionVersion, installationTargets, existingExtension, false, true);
      }
    }

    public virtual void ValidateExtensionVersionImmutability(
      IVssRequestContext requestContext,
      PublishedExtension existingExtension,
      UnpackagedExtensionData newExtensionData)
    {
      if (ExtensionDeploymentTechnology.Vsix.Equals((object) existingExtension.DeploymentType))
      {
        if (GalleryServerUtil.IsVsixConsolidationEnabledForVsExtension(existingExtension.Metadata))
        {
          this.ValidateExtensionVersionImmutabilityForConsolidatedVsExtension(requestContext, existingExtension, newExtensionData.Version, newExtensionData.InstallationTargets);
          if (!this.IsConsolidationAllowedForGivenExtensionOrUploadedPayloadVsVersion(requestContext, existingExtension, newExtensionData.InstallationTargets))
            throw new VsVersionNotAllowedForConsolidationException(GalleryResources.VsVersionNotAllowedForConsolidationException());
        }
        else
        {
          if (!this.ShouldEnableVsixConsolidation(requestContext, existingExtension, newExtensionData.Version, newExtensionData.InstallationTargets) || !this.IsConsolidationAllowedForGivenExtensionOrUploadedPayloadVsVersion(requestContext, existingExtension, newExtensionData.InstallationTargets))
            return;
          this.EnableVsExtensionConsolidation(requestContext, existingExtension, newExtensionData);
        }
      }
      else
        this.ValidateExtensionVersionImmutabilityForNonVsixExtension(requestContext, existingExtension, newExtensionData.Version, newExtensionData.InstallationTargets);
    }

    private void ValidateExtensionVersionImmutabilityForConsolidatedVsExtension(
      IVssRequestContext requestContext,
      PublishedExtension existingExtension,
      string newExtensionVersion,
      List<InstallationTarget> newInstallationTargetList)
    {
      Version newVersion = Version.Parse(newExtensionVersion);
      if (!ExtensionDraftServiceHelper.AreDifferentInstallationTargets(existingExtension.InstallationTargets.FindAll((Predicate<InstallationTarget>) (it => !"Microsoft.VisualStudio.Ide".Equals(it.Target) && Version.Parse(it.ExtensionVersion).CompareTo(newVersion) == 0)), newInstallationTargetList))
      {
        this.PublishCustomerIntelligenceEvent(requestContext, "VsExtensionVersionImmutabilityError", newExtensionVersion, newInstallationTargetList, existingExtension, true, true);
        throw new VsExtensionVersionImmutabilityException(GalleryResources.VsixTypeConsolidatedVsExtensionVersionImmutablityError((object) newExtensionVersion));
      }
    }

    private bool ShouldEnableVsixConsolidation(
      IVssRequestContext requestContext,
      PublishedExtension existingExtension,
      string newExtensionVersion,
      List<InstallationTarget> newInstallationTargetList)
    {
      bool flag1 = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVsixConsolidationForExistingVsExtensions");
      bool flag2 = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVsExtensionVersionImmutabilityForVsixType");
      bool flag3 = existingExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.Validated);
      if (this.GetValidatedExtensionVersion(requestContext, existingExtension.ExtensionId, newExtensionVersion) != null)
      {
        if (flag1 && ExtensionDraftServiceHelper.AreDifferentInstallationTargets(existingExtension.InstallationTargets, newInstallationTargetList))
          return flag1 & flag3;
        if (flag2)
        {
          this.PublishCustomerIntelligenceEvent(requestContext, "VsExtensionVersionImmutabilityError", newExtensionVersion, newInstallationTargetList, existingExtension, false, true);
          throw new VsExtensionVersionImmutabilityException(GalleryResources.VsixTypeVsExtensionVersionImmutabilityError((object) newExtensionVersion));
        }
      }
      else if (flag1 & flag3 && ExtensionDraftServiceHelper.AreDifferentInstallationTargets(existingExtension.InstallationTargets, newInstallationTargetList))
        return true;
      return false;
    }

    private bool IsConsolidationAllowedForGivenExtensionOrUploadedPayloadVsVersion(
      IVssRequestContext requestContext,
      PublishedExtension existingExtension,
      List<InstallationTarget> newInstallationTargetList)
    {
      bool flag1 = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableConsolidationForSpecificVsExtension");
      bool flag2 = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableConsolidationOnlyForVsVersion2022");
      if (!flag1 && !flag2)
        return true;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      if (flag1)
      {
        string b = existingExtension.Publisher.PublisherName + "." + existingExtension.ExtensionName;
        string str1 = service.GetValue<string>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/VsConsolidation/ExtensionAllowList", string.Empty);
        if (!string.IsNullOrEmpty(str1))
        {
          string str2 = str1;
          char[] separator = new char[3]{ ',', ';', ' ' };
          foreach (string a in str2.Split(separator, StringSplitOptions.RemoveEmptyEntries))
          {
            if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase))
              return true;
          }
        }
      }
      if (!flag2)
        return false;
      foreach (InstallationTarget installationTarget in existingExtension.InstallationTargets)
      {
        if (string.Equals(installationTarget.ProductArchitecture, "x86", StringComparison.OrdinalIgnoreCase))
          return false;
      }
      foreach (InstallationTarget installationTarget in newInstallationTargetList)
      {
        if (string.Equals(installationTarget.ProductArchitecture, "x86", StringComparison.OrdinalIgnoreCase))
          return false;
      }
      return true;
    }

    private ExtensionVersion GetValidatedExtensionVersion(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version)
    {
      PublishedExtension publishedExtension = requestContext.GetService<IPublishedExtensionService>().QueryExtensionById(requestContext, extensionId, version, ExtensionQueryFlags.IncludeVersions, Guid.Empty);
      return !publishedExtension.Versions.IsNullOrEmpty<ExtensionVersion>() ? GalleryServerUtil.GetLatestValidatedExtensionVersion(publishedExtension.Versions) : (ExtensionVersion) null;
    }

    private void EnableVsExtensionConsolidation(
      IVssRequestContext requestContext,
      PublishedExtension existingExtension,
      UnpackagedExtensionData newExtensionData)
    {
      requestContext.GetService<IPublishedExtensionService>().EnableVsExtensionConsolidation(requestContext, existingExtension);
      newExtensionData.Metadata.Add(new KeyValuePair<string, string>("HasConsolidatedVsix", bool.TrueString));
      this.PublishCustomerIntelligenceEvent(requestContext, "EnabledVsixConsolidationForVsExtension", newExtensionData.Version, newExtensionData.InstallationTargets, existingExtension, false, true);
    }

    private void ValidateExtensionVersionImmutabilityForNonVsixExtension(
      IVssRequestContext requestContext,
      PublishedExtension existingExtension,
      string newExtensionVersion,
      List<InstallationTarget> newInstallationTargetList)
    {
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVsExtensionVersionImmutabilityForNonVsixType") && existingExtension.Versions.Any<ExtensionVersion>((Func<ExtensionVersion, bool>) (v => v.Flags.HasFlag((Enum) ExtensionVersionFlags.Validated) && newExtensionVersion.Equals(v.Version))))
      {
        this.PublishCustomerIntelligenceEvent(requestContext, "VsExtensionVersionImmutabilityError", newExtensionVersion, newInstallationTargetList, existingExtension, false, false);
        throw new VsExtensionVersionImmutabilityException(GalleryResources.NonVsixTypeVsExtensionVersionImmutabilityError((object) newExtensionVersion));
      }
    }

    private static bool AreDifferentInstallationTargets(
      List<InstallationTarget> installationTargetList1,
      List<InstallationTarget> installationTargetList2)
    {
      foreach (InstallationTarget installationTarget1 in installationTargetList1)
      {
        if (!"Microsoft.VisualStudio.Ide".Equals(installationTarget1.Target))
        {
          foreach (InstallationTarget installationTarget2 in installationTargetList2)
          {
            if (!"Microsoft.VisualStudio.Ide".Equals(installationTarget2.Target) && installationTarget1.ProductArchitecture.Equals(installationTarget2.ProductArchitecture) && ExtensionDraftServiceHelper.IsOverlappingVSIdeVersionRange(installationTarget1, installationTarget2))
              return false;
          }
        }
      }
      return true;
    }

    private static bool IsOverlappingVSIdeVersionRange(
      InstallationTarget installationTarget1,
      InstallationTarget installationTarget2)
    {
      GalleryServerUtil.InternalParseInstallationTargetVersion(installationTarget1);
      GalleryServerUtil.InternalParseInstallationTargetVersion(installationTarget2);
      if (installationTarget2.MinVersion.CompareTo(installationTarget1.MaxVersion) > 0 || installationTarget2.MaxVersion.CompareTo(installationTarget1.MinVersion) < 0)
        return false;
      if (installationTarget2.MinVersion.CompareTo(installationTarget1.MaxVersion) == 0)
        return installationTarget2.MinInclusive && installationTarget1.MaxInclusive;
      if (installationTarget2.MaxVersion.CompareTo(installationTarget1.MinVersion) == 0)
        return installationTarget2.MaxInclusive && installationTarget1.MinInclusive;
      if (installationTarget2.IsApplicableForVersion(installationTarget1.MinVersion) || installationTarget2.IsApplicableForVersion(installationTarget1.MaxVersion) || installationTarget1.IsApplicableForVersion(installationTarget2.MinVersion))
        return true;
      installationTarget1.IsApplicableForVersion(installationTarget2.MaxVersion);
      return true;
    }

    private void PublishCustomerIntelligenceEvent(
      IVssRequestContext requestContext,
      string action,
      string newExtensionVersion,
      List<InstallationTarget> newInstallationTargetList,
      PublishedExtension existingExtension,
      bool hasConsolidatedVsixsAlready,
      bool isVsixTypeExtension)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(CustomerIntelligenceProperty.Action, action);
      properties.Add("PublisherName", existingExtension.Publisher.PublisherName);
      properties.Add("ExtensionName", existingExtension.ExtensionName);
      properties.Add("NewExtensionVersion", newExtensionVersion);
      properties.Add("HasConsolidatedVsixsAlready", hasConsolidatedVsixsAlready);
      properties.Add("IsVsixTypeExtension", isVsixTypeExtension);
      if (!newInstallationTargetList.IsNullOrEmpty<InstallationTarget>())
        properties.Add("NewInstallationTargetList", newInstallationTargetList.Serialize<List<InstallationTarget>>());
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "Extension", properties);
    }
  }
}
