// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.GalleryUtil
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  public static class GalleryUtil
  {
    private static readonly HashSet<char> s_nameCharacters = new HashSet<char>()
    {
      'A',
      'B',
      'C',
      'D',
      'E',
      'F',
      'G',
      'H',
      'I',
      'J',
      'K',
      'L',
      'M',
      'N',
      'O',
      'P',
      'Q',
      'R',
      'S',
      'T',
      'U',
      'V',
      'W',
      'X',
      'Y',
      'Z',
      'a',
      'b',
      'c',
      'd',
      'e',
      'f',
      'g',
      'h',
      'i',
      'j',
      'k',
      'l',
      'm',
      'n',
      'o',
      'p',
      'q',
      'r',
      's',
      't',
      'u',
      'v',
      'w',
      'x',
      'y',
      'z',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9',
      '0',
      '-'
    };
    private static readonly HashSet<char> s_categoryNameCharacters = new HashSet<char>()
    {
      'A',
      'B',
      'C',
      'D',
      'E',
      'F',
      'G',
      'H',
      'I',
      'J',
      'K',
      'L',
      'M',
      'N',
      'O',
      'P',
      'Q',
      'R',
      'S',
      'T',
      'U',
      'V',
      'W',
      'X',
      'Y',
      'Z',
      'a',
      'b',
      'c',
      'd',
      'e',
      'f',
      'g',
      'h',
      'i',
      'j',
      'k',
      'l',
      'm',
      'n',
      'o',
      'p',
      'q',
      'r',
      's',
      't',
      'u',
      'v',
      'w',
      'x',
      'y',
      'z',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9',
      '0',
      '-',
      ' ',
      '&'
    };
    private static readonly HashSet<char> s_assetTypeCharacters = new HashSet<char>()
    {
      'A',
      'B',
      'C',
      'D',
      'E',
      'F',
      'G',
      'H',
      'I',
      'J',
      'K',
      'L',
      'M',
      'N',
      'O',
      'P',
      'Q',
      'R',
      'S',
      'T',
      'U',
      'V',
      'W',
      'X',
      'Y',
      'Z',
      'a',
      'b',
      'c',
      'd',
      'e',
      'f',
      'g',
      'h',
      'i',
      'j',
      'k',
      'l',
      'm',
      'n',
      'o',
      'p',
      'q',
      'r',
      's',
      't',
      'u',
      'v',
      'w',
      'x',
      'y',
      'z',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9',
      '0',
      '.',
      '-',
      '/',
      '_',
      '@'
    };
    private static readonly HashSet<char> s_reservedTagCharacters = new HashSet<char>()
    {
      '!',
      '@',
      '#',
      '$',
      '%',
      '^',
      '&',
      '*'
    };
    private static readonly HashSet<char> s_publisherNameRestrictedCharacters = new HashSet<char>()
    {
      '<',
      '>',
      '"',
      ';',
      '|',
      '@',
      '%',
      '[',
      ']',
      ',',
      '\\',
      '/',
      '='
    };

    public static void CheckExtensionName(string extensionName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(extensionName, nameof (extensionName));
      if (extensionName.StartsWith("-") || extensionName.EndsWith("-") || extensionName.Length > 63 || StringUtil.ContainsIllegalCharacters(extensionName, GalleryUtil.s_nameCharacters))
        throw new ArgumentException(GalleryWebApiResources.InvalidExtensionName((object) extensionName));
    }

    public static void CheckCategoryName(string categoryName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(categoryName, nameof (categoryName));
      if (categoryName.StartsWith("-") || categoryName.EndsWith("-") || categoryName.StartsWith(" ") || categoryName.EndsWith(" ") || categoryName.StartsWith("&") || categoryName.EndsWith("&") || categoryName.Length > 63 || StringUtil.ContainsIllegalCharacters(categoryName, GalleryUtil.s_categoryNameCharacters))
        throw new ArgumentException(GalleryWebApiResources.InvalidCategoryName((object) categoryName));
    }

    public static void CheckPublisherName(string publisherName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(publisherName, nameof (publisherName));
      if (publisherName.StartsWith("-") || publisherName.EndsWith("-") || publisherName.Length > 63 || StringUtil.ContainsIllegalCharacters(publisherName, GalleryUtil.s_nameCharacters))
        throw new ArgumentException(GalleryWebApiResources.InvalidPublisherName((object) publisherName));
    }

    public static void CheckPublisherDisplayName(string publisherDisplayName)
    {
      if (GalleryUtil.CheckIllegalCharacters(publisherDisplayName, GalleryUtil.s_publisherNameRestrictedCharacters))
        throw new ArgumentException(GalleryWebApiResources.InvalidPublisherDisplayName((object) publisherDisplayName));
    }

    public static bool CheckIllegalCharacters(
      string stringToValidate,
      HashSet<char> nonValidCharacters)
    {
      bool flag = false;
      foreach (char ch in stringToValidate)
      {
        if (nonValidCharacters.Contains(ch))
        {
          flag = true;
          break;
        }
      }
      return flag;
    }

    public static string CreateFullyQualifiedName(string publisherName, string extensionName)
    {
      GalleryUtil.CheckPublisherName(publisherName);
      GalleryUtil.CheckExtensionName(extensionName);
      return string.Format("{0}.{1}", (object) publisherName, (object) extensionName);
    }

    public static string CreateExtensionIdForExceptionMessage(
      string publisherName,
      string extensionName)
    {
      return string.Format("{0}.{1}", (object) publisherName, (object) extensionName);
    }

    public static string GetFullyQualifiedName(this PublishedExtension extension) => GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName);

    public static bool IsVsExtension(this PublishedExtension extension)
    {
      if (extension != null && !extension.InstallationTargets.IsNullOrEmpty<InstallationTarget>())
      {
        foreach (InstallationTarget installationTarget in extension.InstallationTargets)
        {
          if (string.Equals(installationTarget.Target, "Microsoft.VisualStudio.Ide", StringComparison.OrdinalIgnoreCase))
            return true;
        }
      }
      return false;
    }

    public static bool IsVsForMacExtension(this PublishedExtension extension) => extension != null && !extension.InstallationTargets.IsNullOrEmpty<InstallationTarget>() && GalleryUtil.IsVSForMacInstallationTargets((IEnumerable<InstallationTarget>) extension.InstallationTargets);

    public static bool IsMigratedFromVSGallery(this PublishedExtension extension)
    {
      if (extension != null && !extension.Metadata.IsNullOrEmpty<ExtensionMetadata>())
      {
        ExtensionMetadata extensionMetadata = extension.Metadata.Find((Predicate<ExtensionMetadata>) (x => string.Equals(x.Key, "MigratedFromVSGallery", StringComparison.OrdinalIgnoreCase)));
        bool result;
        if (extensionMetadata != null && bool.TryParse(extensionMetadata.Value, out result))
          return result;
      }
      return false;
    }

    public static bool IsVsCodeExtension(this PublishedExtension extension)
    {
      if (extension != null && !extension.InstallationTargets.IsNullOrEmpty<InstallationTarget>())
      {
        foreach (InstallationTarget installationTarget in extension.InstallationTargets)
        {
          if (string.Equals(installationTarget.Target, "Microsoft.VisualStudio.Code", StringComparison.OrdinalIgnoreCase))
            return true;
        }
      }
      return false;
    }

    public static bool IsVsOrVsCodeOrVsForMacExtension(this PublishedExtension extension) => extension != null && !extension.InstallationTargets.IsNullOrEmpty<InstallationTarget>() && extension.InstallationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => string.Equals(t.Target, "Microsoft.VisualStudio.Mac", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.VisualStudio.Ide", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.VisualStudio.Code", StringComparison.OrdinalIgnoreCase)));

    public static bool IsVSTSExtensionResourceOrIntegration(this PublishedExtension extension) => GalleryUtil.IsVSTSExtensionResourceOrIntegration((IEnumerable<InstallationTarget>) extension?.InstallationTargets);

    public static bool IsVSTSExtensionResourceOrIntegration(
      IEnumerable<InstallationTarget> installationTargets)
    {
      if (installationTargets.IsNullOrEmpty<InstallationTarget>())
        return false;
      return GalleryUtil.InstallationTargetsHasVSTS(installationTargets) || GalleryUtil.InstallationTargetsHasVSTSResource(installationTargets) || GalleryUtil.IsVSTSOrTFSIntegrationTargets(installationTargets);
    }

    public static bool IsPublic(this PublishedExtension extension) => extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Public);

    public static bool IsValidated(this PublishedExtension extension) => extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Validated);

    public static bool IsBuiltIn(this PublishedExtension extension) => extension.Flags.HasFlag((Enum) PublishedExtensionFlags.BuiltIn);

    public static bool IsPaid(this PublishedExtension extension) => GalleryUtil.HasExtensionTag((IEnumerable<string>) extension.Tags, "$IsPaid") || extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Paid);

    public static bool IsPreview(this PublishedExtension extension) => extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Preview) || GalleryUtil.HasExtensionTag((IEnumerable<string>) extension.Tags, "$preview");

    public static bool IsTrial(this PublishedExtension extension) => extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Trial);

    public static bool IsFirstParty(this PublishedExtension extension) => GalleryUtil.IsFirstParty(extension.Publisher.DisplayName);

    public static bool IsFirstParty(string publisherDisplayName) => string.Equals(publisherDisplayName, "Microsoft", StringComparison.OrdinalIgnoreCase);

    public static bool IsMSExtension(this PublishedExtension extension) => extension != null && extension.Publisher != null && !extension.Publisher.PublisherName.IsNullOrEmpty<char>() && extension.Publisher.PublisherName.Equals("ms", StringComparison.OrdinalIgnoreCase);

    public static bool IsThirdParty(this PublishedExtension extension) => !extension.IsFirstParty();

    public static bool IsFirstPartyAndPaid(this PublishedExtension extension) => extension.IsFirstParty() && extension.IsPaid() && !extension.IsPreview();

    public static bool IsFirstPartyAndPaidVSTS(this PublishedExtension extension) => extension.IsFirstPartyAndPaid() && GalleryUtil.InstallationTargetsHasVSTS((IEnumerable<InstallationTarget>) extension.InstallationTargets);

    public static bool IsThirdPartyAndPaid(this PublishedExtension extension) => extension.IsThirdParty() && extension.IsPaid();

    public static bool ShouldNotDownload(this PublishedExtension extension) => GalleryUtil.HasExtensionTag((IEnumerable<string>) extension.Tags, "$DoNotDownload") || GalleryUtil.HasExtensionTag((IEnumerable<string>) extension.Tags, "__DoNotDownload");

    public static bool IsMarketExtension(this PublishedExtension extension)
    {
      if (extension == null || extension.Tags == null)
        return false;
      return GalleryUtil.HasExtensionTag((IEnumerable<string>) extension.Tags, "__market") || GalleryUtil.HasExtensionTag((IEnumerable<string>) extension.Tags, "$market");
    }

    public static bool IsByolExtension(this PublishedExtension extension) => extension.IsByolEnabledExtension() || extension.IsByolEnforcedExtension();

    public static bool IsTrialEnabledForByolExtension(this PublishedExtension extension) => extension.IsByolExtension() && extension != null && extension.Tags != null && extension.Tags.Any<string>((Func<string, bool>) (s => s.StartsWith("__TrialDays", StringComparison.OrdinalIgnoreCase)));

    public static bool IsByolEnabledExtension(this PublishedExtension extension) => extension?.Tags != null && GalleryUtil.HasExtensionTag((IEnumerable<string>) extension.Tags, "__BYOL");

    public static bool IsByolEnforcedExtension(this PublishedExtension extension) => extension?.Tags != null && GalleryUtil.HasExtensionTag((IEnumerable<string>) extension.Tags, "__BYOLEnforced");

    public static bool IsSystemTag(string tagToCheck) => !string.IsNullOrEmpty(tagToCheck) && tagToCheck.ElementAt<char>(0).Equals('$');

    private static bool HasExtensionTag(IEnumerable<string> tagsList, string searchTag) => tagsList != null && tagsList.Any<string>((Func<string, bool>) (s => s.Equals(searchTag, StringComparison.OrdinalIgnoreCase)));

    public static void CheckAssetType(string assetType)
    {
      if (assetType.Contains("..") || StringUtil.ContainsIllegalCharacters(assetType, GalleryUtil.s_assetTypeCharacters))
        throw new ArgumentException(GalleryWebApiResources.InvalidAssetType((object) assetType));
    }

    public static string FixAssetType(string assetType, char replacement)
    {
      StringBuilder stringBuilder = new StringBuilder(assetType);
      for (int index = 0; index < stringBuilder.Length; ++index)
      {
        char ch = stringBuilder[index];
        if (!GalleryUtil.s_assetTypeCharacters.Contains(ch))
          stringBuilder[index] = replacement;
      }
      return stringBuilder.ToString();
    }

    public static void CheckTag(string tag)
    {
      if (string.IsNullOrEmpty(tag))
        throw new InvalidTagException(GalleryWebApiResources.InvalidTag((object) string.Empty));
      if (!tag.Equals("$IsPaid", StringComparison.OrdinalIgnoreCase) && GalleryUtil.s_reservedTagCharacters.Contains(tag[0]))
        throw new InvalidTagException(GalleryWebApiResources.InvalidTag((object) tag));
    }

    public static bool InstallationTargetsHasVSTS(
      IEnumerable<InstallationTarget> installationTargets)
    {
      return installationTargets != null && installationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => string.Equals(t.Target, "Microsoft.VisualStudio.Services", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.VisualStudio.Services.Cloud", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.TeamFoundation.Server", StringComparison.OrdinalIgnoreCase)));
    }

    public static bool InstallationTargetsHasVSTSResource(
      IEnumerable<InstallationTarget> installationTargets)
    {
      return installationTargets != null && installationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => string.Equals(t.Target, "Microsoft.VisualStudio.Services.Resource.Cloud", StringComparison.OrdinalIgnoreCase)));
    }

    public static bool HasInterestingTargetsForEMS(
      IEnumerable<InstallationTarget> installationTargets)
    {
      return installationTargets != null && installationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => string.Equals(t.Target, "Microsoft.VisualStudio.Services", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.VisualStudio.Services.Cloud", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.TeamFoundation.Server", StringComparison.OrdinalIgnoreCase)));
    }

    public static bool HasInterestingTargets(
      IEnumerable<InstallationTarget> installationTargets,
      string interestingTargets)
    {
      if (installationTargets != null)
      {
        if (string.IsNullOrWhiteSpace(interestingTargets))
          return true;
        interestingTargets = interestingTargets.ToLower();
        if (installationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => interestingTargets.Contains(t.Target.ToLower()))))
          return true;
      }
      return false;
    }

    public static bool IsVSTSOrTFSInstallationTargets(
      IEnumerable<InstallationTarget> installationTargets)
    {
      return installationTargets != null && installationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => GalleryUtil.IsVSTSOrTFSInstallationTarget(t.Target)));
    }

    public static bool IsVSTSOrTFSInstallationTarget(string installationTarget) => string.Equals(installationTarget, "Microsoft.VisualStudio.Services", StringComparison.OrdinalIgnoreCase) || string.Equals(installationTarget, "Microsoft.TeamFoundation.Server", StringComparison.OrdinalIgnoreCase) || string.Equals(installationTarget, "Microsoft.VisualStudio.Services.Integration", StringComparison.OrdinalIgnoreCase) || string.Equals(installationTarget, "Microsoft.TeamFoundation.Server.Integration", StringComparison.OrdinalIgnoreCase) || string.Equals(installationTarget, "Microsoft.VisualStudio.Services.Cloud.Integration", StringComparison.OrdinalIgnoreCase);

    public static bool IsOnPremSupported(
      IEnumerable<InstallationTarget> installationTargets)
    {
      return installationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => string.Equals(t.Target, "Microsoft.VisualStudio.Services", StringComparison.OrdinalIgnoreCase))) || installationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => string.Equals(t.Target, "Microsoft.TeamFoundation.Server", StringComparison.OrdinalIgnoreCase)));
    }

    public static bool IsVSTSOrTFSIntegrationTargets(
      IEnumerable<InstallationTarget> installationTargets)
    {
      return installationTargets != null && installationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => GalleryUtil.IsVSTSOrTFSIntegrationTargets(t.Target)));
    }

    public static bool IsVSTSOrTFSIntegrationTargets(string installationTarget) => string.Equals(installationTarget, "Microsoft.VisualStudio.Services.Integration", StringComparison.OrdinalIgnoreCase) || string.Equals(installationTarget, "Microsoft.TeamFoundation.Server.Integration", StringComparison.OrdinalIgnoreCase) || string.Equals(installationTarget, "Microsoft.VisualStudio.Services.Cloud.Integration", StringComparison.OrdinalIgnoreCase);

    public static bool IsVSSExtensionInstallationTarget(
      IEnumerable<InstallationTarget> installationTargets)
    {
      return installationTargets != null && installationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => string.Equals(t.Target, "Microsoft.VisualStudio.Services", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.VisualStudio.Services.Cloud", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.TeamFoundation.Server", StringComparison.OrdinalIgnoreCase)));
    }

    public static bool IsOnlyVSTSInstallationTarget(
      IEnumerable<InstallationTarget> installationTargets)
    {
      return installationTargets != null && !installationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => string.Equals(t.Target, "Microsoft.TeamFoundation.Server", StringComparison.OrdinalIgnoreCase))) && installationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => string.Equals(t.Target, "Microsoft.VisualStudio.Services.Cloud", StringComparison.OrdinalIgnoreCase)));
    }

    public static bool IsVSCodeInstallationTargets(
      IEnumerable<InstallationTarget> installationTargets)
    {
      return installationTargets != null && installationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => GalleryUtil.IsVSCodeInstallationTarget(t.Target)));
    }

    public static bool IsVSCodeInstallationTarget(string installationTarget) => string.Equals(installationTarget, "Microsoft.VisualStudio.Code", StringComparison.OrdinalIgnoreCase);

    public static bool IsVSInstallationTargets(
      IEnumerable<InstallationTarget> installationTargets)
    {
      return installationTargets != null && installationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => GalleryUtil.IsVSInstallationTarget(t.Target)));
    }

    public static bool IsVSInstallationTarget(string installationTarget) => string.Equals(installationTarget, "Microsoft.VisualStudio.Ide", StringComparison.OrdinalIgnoreCase);

    public static bool IsVSForMacInstallationTargets(
      IEnumerable<InstallationTarget> installationTargets)
    {
      return installationTargets != null && installationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => GalleryUtil.IsVSForMacInstallationTarget(t.Target)));
    }

    public static bool IsVSForMacInstallationTarget(string installationTarget) => string.Equals(installationTarget, "Microsoft.VisualStudio.Mac", StringComparison.OrdinalIgnoreCase);

    public static bool IsVSSubsInstallationTarget(
      IEnumerable<InstallationTarget> installationTargets)
    {
      return installationTargets != null && installationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => string.Equals(t.Target, "Microsoft.VisualStudio.Offer", StringComparison.OrdinalIgnoreCase)));
    }

    public static bool IsHostedResourceInstallationTarget(
      IEnumerable<InstallationTarget> installationTargets)
    {
      return installationTargets != null && installationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => string.Equals(t.Target, "Microsoft.VisualStudio.Services.Resource.Cloud", StringComparison.OrdinalIgnoreCase)));
    }

    public static bool HasAcquisitionExperience(PublishedExtension extension) => extension != null && extension.InstallationTargets != null && extension.InstallationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => string.Equals(t.Target, "Microsoft.VisualStudio.Services", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.VisualStudio.Services.Cloud", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.TeamFoundation.Server", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.VisualStudio.Offer", StringComparison.OrdinalIgnoreCase) || string.Equals(t.Target, "Microsoft.VisualStudio.Services.Resource.Cloud", StringComparison.OrdinalIgnoreCase)));

    public static string GetProductTypeForInstallationTargets(
      IEnumerable<InstallationTarget> installationTargets,
      bool useAzureDevOps = false)
    {
      if (installationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => string.Equals(t.Target, "Microsoft.VisualStudio.Code", StringComparison.OrdinalIgnoreCase))))
        return "vscode";
      if (installationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => string.Equals(t.Target, "Microsoft.VisualStudio.Ide", StringComparison.OrdinalIgnoreCase))))
        return "vs";
      if (installationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => string.Equals(t.Target, "Microsoft.VisualStudio.Mac", StringComparison.OrdinalIgnoreCase))))
        return "vsformac";
      if (installationTargets.Any<InstallationTarget>((Func<InstallationTarget, bool>) (t => string.Equals(t.Target, "Microsoft.VisualStudio.Offer", StringComparison.OrdinalIgnoreCase))))
        return "subscriptions";
      return !useAzureDevOps ? "vsts" : "azuredevops";
    }

    public static List<string> GetInstallationTargetsForProduct(
      string productType,
      bool useAzureDevOps = false)
    {
      List<string> targetsForProduct = new List<string>();
      if (string.Equals(productType, "vs", StringComparison.OrdinalIgnoreCase))
        targetsForProduct.Add("Microsoft.VisualStudio.Ide");
      else if (string.Equals(productType, "vscode", StringComparison.OrdinalIgnoreCase))
        targetsForProduct.Add("Microsoft.VisualStudio.Code");
      else if (string.Equals(productType, "vsformac", StringComparison.OrdinalIgnoreCase))
        targetsForProduct.Add("Microsoft.VisualStudio.Mac");
      else if (string.Equals(productType, "vsts", StringComparison.OrdinalIgnoreCase) || useAzureDevOps && string.Equals(productType, "azuredevops", StringComparison.OrdinalIgnoreCase))
      {
        targetsForProduct.Add("Microsoft.VisualStudio.Services");
        targetsForProduct.Add("Microsoft.VisualStudio.Services.Resource.Cloud");
        targetsForProduct.Add("Microsoft.VisualStudio.Services.Cloud");
        targetsForProduct.Add("Microsoft.TeamFoundation.Server");
        targetsForProduct.Add("Microsoft.VisualStudio.Services.Integration");
        targetsForProduct.Add("Microsoft.VisualStudio.Services.Cloud.Integration");
        targetsForProduct.Add("Microsoft.TeamFoundation.Server.Integration");
      }
      else if (string.Equals(productType, "subscriptions", StringComparison.OrdinalIgnoreCase))
        targetsForProduct.Add("Microsoft.VisualStudio.Offer");
      return targetsForProduct;
    }

    public static void LoadExtensionDeploymentType(PublishedExtension extension)
    {
      if (extension == null || extension.Metadata == null)
        return;
      foreach (ExtensionMetadata extensionMetadata in extension.Metadata)
      {
        if (extensionMetadata.Key.Equals("DeploymentTechnology", StringComparison.OrdinalIgnoreCase))
        {
          switch (extensionMetadata.Value)
          {
            case "Referral Link":
              extension.DeploymentType = ExtensionDeploymentTechnology.ReferralLink;
              continue;
            case "EXE":
              extension.DeploymentType = ExtensionDeploymentTechnology.Exe;
              continue;
            case "MSI":
              extension.DeploymentType = ExtensionDeploymentTechnology.Msi;
              continue;
            default:
              extension.DeploymentType = ExtensionDeploymentTechnology.Vsix;
              continue;
          }
        }
      }
    }

    public static bool IsVsVsixTypeZipPackage(PackageDetails packageDetails) => packageDetails?.Manifest?.Assets != null && packageDetails.Manifest.Assets.Any<ManifestFile>((Func<ManifestFile, bool>) (t => string.Equals(t.AssetType, "Microsoft.VisualStudio.Ide.Payload", StringComparison.OrdinalIgnoreCase) && t.FullPath.EndsWith(".vsix", StringComparison.OrdinalIgnoreCase)));

    public static bool IsVsixTypeExtension(this PublishedExtension extension) => extension.DeploymentType == ExtensionDeploymentTechnology.Vsix || extension.IsVsCodeExtension();

    public static IEnumerable<string> GetCleanedCategoriesListForVSCode(
      PackageDetails packageDetails)
    {
      if (packageDetails == null)
        return (IEnumerable<string>) null;
      PackageManifest manifest = packageDetails.Manifest;
      bool? nullable1;
      if (manifest == null)
      {
        nullable1 = new bool?();
      }
      else
      {
        List<InstallationTarget> installation = manifest.Installation;
        nullable1 = installation != null ? new bool?(installation.Any<InstallationTarget>((Func<InstallationTarget, bool>) (x => "Microsoft.VisualStudio.Code".Equals(x.Target)))) : new bool?();
      }
      IEnumerable<string> categories = packageDetails.Categories;
      bool? nullable2 = nullable1;
      bool flag = true;
      if (!(nullable2.GetValueOrDefault() == flag & nullable2.HasValue) || categories.IsNullOrEmpty<string>() || categories.Count<string>() <= 1)
        return packageDetails.Categories;
      List<string> list = categories.ToList<string>();
      list.RemoveAll((Predicate<string>) (category => GalleryConstants.OtherCategory.Equals(category, StringComparison.OrdinalIgnoreCase)));
      return list.AsEnumerable<string>();
    }

    public static string GetProductTypeFromExtensionQuery(ExtensionQuery extensionQuery)
    {
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      bool flag4 = false;
      foreach (QueryFilter filter in extensionQuery.Filters)
      {
        foreach (FilterCriteria criterion in filter.Criteria)
        {
          if (criterion.FilterType == 8)
          {
            if (GalleryUtil.IsVSTSOrTFSInstallationTarget(criterion.Value))
              flag3 = true;
            else if (GalleryUtil.IsVSInstallationTarget(criterion.Value))
              flag1 = true;
            else if (GalleryUtil.IsVSCodeInstallationTarget(criterion.Value))
              flag2 = true;
            else if (GalleryUtil.IsVSForMacInstallationTarget(criterion.Value))
              flag4 = true;
          }
        }
      }
      if (flag1 && !flag2 && !flag3 && !flag4)
        return "vs";
      if (!flag1 & flag2 && !flag3 && !flag4)
        return "vscode";
      if (((flag1 ? 0 : (!flag2 ? 1 : 0)) & (flag3 ? 1 : 0)) != 0 && !flag4)
        return "vsts";
      return ((flag1 || flag2 ? 0 : (!flag3 ? 1 : 0)) & (flag4 ? 1 : 0)) != 0 ? "vsformac" : "all";
    }

    public static IDictionary<string, string> GetExtensionProperties(PublishedExtension extension)
    {
      IDictionary<string, string> extensionProperties = (IDictionary<string, string>) new Dictionary<string, string>();
      if (extension.Versions[0].Properties != null)
        extensionProperties = (IDictionary<string, string>) extension.Versions[0].Properties.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (pair => pair.Key), (Func<KeyValuePair<string, string>, string>) (pair => pair.Value));
      if (extension.IsVsExtension() && extension.Metadata != null && extension.Metadata.Count > 0)
      {
        foreach (ExtensionMetadata extensionMetadata in extension.Metadata)
        {
          if (extensionMetadata != null && extensionMetadata.Key.Equals("SourceCodeUrl", StringComparison.OrdinalIgnoreCase))
          {
            if (extensionMetadata.Value != null)
            {
              try
              {
                if (new Uri(extensionMetadata.Value).Host.Equals("github.com", StringComparison.OrdinalIgnoreCase))
                  extensionProperties.Add("Microsoft.VisualStudio.Services.Links.GitHub", extensionMetadata.Value);
              }
              catch (UriFormatException ex)
              {
              }
            }
          }
        }
      }
      return extensionProperties;
    }

    public static int GetCultureMatchingDistance(CultureInfo cultureA, CultureInfo cultureB)
    {
      CultureInfo invariantCulture = CultureInfo.InvariantCulture;
      if (cultureA == null || cultureB == null || cultureA.Equals((object) invariantCulture) || cultureB.Equals((object) invariantCulture))
        return -1;
      int num = 0;
      CultureInfo cultureInfo = cultureA;
      while (!cultureInfo.Equals((object) cultureB) && !cultureInfo.Equals((object) invariantCulture))
      {
        cultureInfo = cultureInfo.Parent;
        ++num;
      }
      if (cultureInfo.Equals((object) invariantCulture))
      {
        num = 1;
        cultureInfo = cultureB.Parent;
        while (!cultureInfo.Equals((object) cultureA) && !cultureInfo.Equals((object) invariantCulture))
        {
          cultureInfo = cultureInfo.Parent;
          ++num;
        }
      }
      return cultureInfo.Equals((object) invariantCulture) ? -1 : num;
    }

    public static IReadOnlyCollection<string> SplitDelimiterSeparatedTerms(
      string input,
      char delimiter)
    {
      if (string.IsNullOrWhiteSpace(input))
        return (IReadOnlyCollection<string>) new List<string>();
      return (IReadOnlyCollection<string>) ((IEnumerable<string>) input.Split(new char[1]
      {
        delimiter
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (i => i.Trim())).ToList<string>();
    }

    public static List<ExtensionVersion> GetLatestExtensionVersionForSupportedTargetPlatforms(
      List<ExtensionVersion> extensionVersions)
    {
      List<ExtensionVersion> supportedTargetPlatforms = new List<ExtensionVersion>();
      ISet<string> stringSet = (ISet<string>) new HashSet<string>();
      if (extensionVersions.IsNullOrEmpty<ExtensionVersion>() || extensionVersions.Count <= 1)
        return extensionVersions;
      foreach (ExtensionVersion extensionVersion in extensionVersions)
      {
        if (stringSet.Add(extensionVersion.TargetPlatform))
          supportedTargetPlatforms.Add(extensionVersion);
      }
      return supportedTargetPlatforms;
    }

    public static PublishedExtension CloneExtension(
      PublishedExtension extension,
      ExtensionQueryFlags flags)
    {
      PublishedExtension publishedExtension = extension.ShallowCopy();
      if (flags.HasFlag((Enum) ExtensionQueryFlags.IncludeCategoryAndTags))
      {
        if (extension.Categories != null)
          publishedExtension.Categories = new List<string>((IEnumerable<string>) extension.Categories);
        if (extension.Tags != null)
          publishedExtension.Tags = new List<string>((IEnumerable<string>) extension.Tags);
      }
      else
      {
        publishedExtension.Categories = (List<string>) null;
        publishedExtension.Tags = (List<string>) null;
      }
      if (flags.HasFlag((Enum) ExtensionQueryFlags.IncludeInstallationTargets) && extension.InstallationTargets != null)
      {
        List<InstallationTarget> installationTargetList = new List<InstallationTarget>();
        foreach (InstallationTarget installationTarget in extension.InstallationTargets)
          installationTargetList.Add(installationTarget.ShallowCopy());
        publishedExtension.InstallationTargets = installationTargetList;
      }
      else
        publishedExtension.InstallationTargets = (List<InstallationTarget>) null;
      if (flags.HasFlag((Enum) ExtensionQueryFlags.IncludeStatistics) && extension.Statistics != null)
      {
        List<ExtensionStatistic> extensionStatisticList = new List<ExtensionStatistic>();
        foreach (ExtensionStatistic statistic in extension.Statistics)
          extensionStatisticList.Add(statistic.ShallowCopy());
        publishedExtension.Statistics = extensionStatisticList;
      }
      else
        publishedExtension.Statistics = (List<ExtensionStatistic>) null;
      if ((flags.HasFlag((Enum) ExtensionQueryFlags.IncludeSharedAccounts) || flags.HasFlag((Enum) ExtensionQueryFlags.IncludeSharedOrganizations)) && extension.SharedWith != null)
      {
        List<ExtensionShare> extensionShareList = new List<ExtensionShare>();
        foreach (ExtensionShare extensionShare in extension.SharedWith)
        {
          if (flags.HasFlag((Enum) ExtensionQueryFlags.IncludeSharedAccounts) && extensionShare.Type == "account")
            extensionShareList.Add(extensionShare.ShallowCopy());
          if (flags.HasFlag((Enum) ExtensionQueryFlags.IncludeSharedOrganizations) && extensionShare.Type == "organization")
            extensionShareList.Add(extensionShare.ShallowCopy());
        }
        publishedExtension.SharedWith = extensionShareList;
      }
      else
        publishedExtension.SharedWith = (List<ExtensionShare>) null;
      if (flags.HasFlag((Enum) ExtensionQueryFlags.IncludeMetadata) && extension.Metadata != null)
      {
        List<ExtensionMetadata> extensionMetadataList = new List<ExtensionMetadata>();
        foreach (ExtensionMetadata extensionMetadata in extension.Metadata)
          extensionMetadataList.Add(extensionMetadata.ShallowCopy());
        publishedExtension.Metadata = extensionMetadataList;
      }
      else
        publishedExtension.Metadata = (List<ExtensionMetadata>) null;
      if (!extension.Versions.IsNullOrEmpty<ExtensionVersion>() && (flags.HasFlag((Enum) ExtensionQueryFlags.IncludeVersions) || flags.HasFlag((Enum) ExtensionQueryFlags.IncludeFiles) || flags.HasFlag((Enum) ExtensionQueryFlags.IncludeVersionProperties) || flags.HasFlag((Enum) ExtensionQueryFlags.IncludeLatestVersionOnly)))
      {
        List<ExtensionVersion> extensionVersionList = new List<ExtensionVersion>();
        foreach (ExtensionVersion version in publishedExtension.Versions)
        {
          ExtensionVersion extensionVersion = version.ShallowCopy();
          if (!flags.HasFlag((Enum) ExtensionQueryFlags.IncludeAssetUri) && !flags.HasFlag((Enum) ExtensionQueryFlags.IncludeFiles))
          {
            extensionVersion.AssetUri = (string) null;
            extensionVersion.FallbackAssetUri = (string) null;
          }
          if (!flags.HasFlag((Enum) ExtensionQueryFlags.IncludeFiles))
            extensionVersion.Files = (List<ExtensionFile>) null;
          if (!flags.HasFlag((Enum) ExtensionQueryFlags.IncludeVersionProperties))
            extensionVersion.Properties = (List<KeyValuePair<string, string>>) null;
          extensionVersionList.Add(extensionVersion);
        }
        publishedExtension.Versions = extensionVersionList;
        if (flags.HasFlag((Enum) ExtensionQueryFlags.ExcludeNonValidated))
          publishedExtension.Versions.RemoveAll((Predicate<ExtensionVersion>) (version => !version.Flags.HasFlag((Enum) ExtensionVersionFlags.Validated)));
        if (flags.HasFlag((Enum) ExtensionQueryFlags.IncludeLatestVersionOnly) && publishedExtension.Versions.Count > 1)
        {
          if (extension.IsVsCodeExtension())
            publishedExtension.Versions = GalleryUtil.GetLatestExtensionVersionForSupportedTargetPlatforms(publishedExtension.Versions);
          else
            publishedExtension.Versions.RemoveRange(1, publishedExtension.Versions.Count - 1);
        }
      }
      else
        publishedExtension.Versions = (List<ExtensionVersion>) null;
      return publishedExtension;
    }
  }
}
