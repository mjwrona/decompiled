// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.GalleryConstants
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  public static class GalleryConstants
  {
    public static readonly Guid SecurityNamespace = new Guid("1CF5F1C2-7F95-4EA2-87FC-20716C87DD8D");
    public const string ServiceName = "Gallery";
    public const string ServiceOwner = "00000029-0000-8888-8000-000000000000";
    public const string RegistrationProperty = "RegistrationId";
    public const string TaskProperty = "Microsoft.VisualStudio.Services.TaskId";
    public const string ServiceEndpointType = "Microsoft.VisualStudio.Services.ServiceEndpointName";
    public const string MarketplaceUrl = "https://go.microsoft.com/fwlink/?LinkId=722825";
    public const string MarketplaceBrowseUrl = "https://go.microsoft.com/fwlink/?linkid=821987";
    public const string MarketplaceManagePublishersUrl = "https://go.microsoft.com/fwlink/?linkid=823198";
    public const string VerifyPublisherLink = "https://aka.ms/vsmarketplace-verify";
    public const string LatestVersion = "latest";
    public const string OnPremStatsUpdateParameter = "onpremDownload";
    public const string ItemDetailsRelativeUrlString = "items?itemName=";
    public const string RegistryPathForMinimumReviewsToShowStarRatings = "/Configuration/Service/Gallery/RatingAndReview/RegistryPathForMinimumReviewsToShowStarRatings";
    public const int MaxPreviousVersionsToBeRetrivedInVersionHistory = 5;
    public const string RegistryPathForMaxPreviousVersionsToBeRetrivedInVersionHistory = "/Configuration/Service/Gallery/ItemDetails/MaxPreviousVersionsToBeRetrivedInVersionHistory";
    public static string OtherCategory = "Other";
    public const string ExtensionChangeMessageQueue = "Microsoft.VisualStudio.Services.Gallery";
    public const string KeyChangeMessageQueue = "Microsoft.VisualStudio.Services.KeyManagement";
    public static string[] MessageVersions = new string[1]
    {
      "1.0"
    };
    public const string VsoInstallationTarget = "Microsoft.VisualStudio.Services";
    public const string VSSHostedResourceInstallationTarget = "Microsoft.VisualStudio.Services.Resource.Cloud";
    public const string VsoHostedInstallationTarget = "Microsoft.VisualStudio.Services.Cloud";
    public const string TfsInstallationTarget = "Microsoft.TeamFoundation.Server";
    public const string VsoIntegrationInstallationTarget = "Microsoft.VisualStudio.Services.Integration";
    public const string VsoHostedIntegrationInstallationTarget = "Microsoft.VisualStudio.Services.Cloud.Integration";
    public const string TfsIntegrationInstallationTarget = "Microsoft.TeamFoundation.Server.Integration";
    public const string VsCodeInstallationTarget = "Microsoft.VisualStudio.Code";
    public const string VsCloudOfferInstallationTarget = "Microsoft.VisualStudio.Offer";
    public const string VsInstallationTarget = "Microsoft.VisualStudio.Ide";
    public const string VsForMacInstallationTarget = "Microsoft.VisualStudio.Mac";
    public const string VsWinDesktopExpressInstallationTarget = "Microsoft.VisualStudio.VSWinDesktopExpress";
    public const string VsWinExpressInstallationTarget = "Microsoft.VisualStudio.VSWinExpress";
    public const string VsVwdExpressInstallationTarget = "Microsoft.VisualStudio.VWDExpress";
    public const string VsCommunityInstallationTarget = "Microsoft.VisualStudio.Community";
    public const string VsProInstallationTarget = "Microsoft.VisualStudio.Pro";
    public const string VsEnterpriseInstallationTarget = "Microsoft.VisualStudio.Enterprise";
    public const string VsIntegratedShellInstallationTarget = "Microsoft.VisualStudio.IntegratedShell";
    public const string VsIsolatedInstallationTarget = "Microsoft.VisualStudio.Isolated";
    public const string VsTestInstallationTarget = "Microsoft.VisualStudio.Test";
    public const string VsUltimateInstallationTarget = "Microsoft.VisualStudio.Ultimate";
    public const string VsPremiumInstallationTarget = "Microsoft.VisualStudio.Premium";
    public const string VsVstInstallationTarget = "Microsoft.VisualStudio.VST_All";
    public const string VsVslsInstallationTarget = "Microsoft.VisualStudio.VSLS";
    public const string VsVpdExpressInstallationTarget = "Microsoft.VisualStudio.VPDExpress";
    public const string GitHubPropertyId = "Microsoft.VisualStudio.Services.Links.GitHub";
    public const string SupportLinkProperty = "Microsoft.VisualStudio.Services.Links.Support";
    public const string GetStartedLinkProperty = "Microsoft.VisualStudio.Services.Links.Getstarted";
    public const string InstallLinkProperty = "Microsoft.VisualStudio.Services.Links.Install";
    public const string LearnLinkProperty = "Microsoft.VisualStudio.Services.Links.Learn";
    public const string FeedbackLinkProperty = "Microsoft.VisualStudio.Services.Links.Feedback";
    public const string AllCategoriesName = "all";
    public static readonly Guid ExtensionNameArtifactKind = new Guid("6DB1EC5A-CE9A-43F6-B82E-150794A6E275");
    public static readonly Guid ExtensionVersionArtifactKind = new Guid("BDEF69E2-625C-4446-B414-4EFAA2172439");
    public static readonly DateTime BeginningOfTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    public const string TrialDays = "Microsoft.VisualStudio.Services.GalleryProperties.TrialDays";

    public static class AssetType
    {
      public const string Assembly = "Microsoft.VisualStudio.Assembly";
      public const string VsixPackage = "Microsoft.VisualStudio.Services.VSIXPackage";
      public const string VsoManifest = "Microsoft.VisualStudio.Services.Manifest";
      public const string DefaultIcon = "Microsoft.VisualStudio.Services.Icons.Default";
      public const string WideIcon = "Microsoft.VisualStudio.Services.Icons.Wide";
      public const string BrandingIcon = "Microsoft.VisualStudio.Services.Icons.Branding";
      public const string PreviewImage = "Microsoft.VisualStudio.Services.Image.Preview";
      public const string VsCodeManifest = "Microsoft.VisualStudio.Code.Manifest";
      public const string Payload = "Microsoft.VisualStudio.Ide.Payload";
      public const string SmallIcon = "Microsoft.VisualStudio.Services.Icons.Small";
      public const string VsixManifest = "Microsoft.VisualStudio.Services.VsixManifest";
      public const string License = "Microsoft.VisualStudio.Services.Content.License";
      public const string Privacy = "Microsoft.VisualStudio.Services.Content.PrivacyPolicy";
      public const string Details = "Microsoft.VisualStudio.Services.Content.Details";
      public const string Changelog = "Microsoft.VisualStudio.Services.Content.Changelog";
      public const string Pricing = "Microsoft.VisualStudio.Services.Content.Pricing";
      public const string TrialDays = "Microsoft.VisualStudio.Services.GalleryProperties.TrialDays";
      public const string VsixSignature = "Microsoft.VisualStudio.Services.VsixSignature";
      public const string CommonVsixPackage = "Microsoft.VisualStudio.Services.CommonVSIXPackage";
    }

    public static class ShareType
    {
      public const string Account = "account";
      public const string Organization = "organization";
    }

    public static class MetaDataKeys
    {
      public const string DeploymentTechnology = "DeploymentTechnology";
      public const string ReferralLinkFileId = "ReferralLinkFileId";
      public const string ReferralUrl = "ReferralUrl";
      public const string VsixId = "VsixId";
      public const string SourceCodeUrl = "SourceCodeUrl";
      public const string MigratedFromVSGallery = "MigratedFromVSGallery";
      public const string ConvertedToMarkdown = "ConvertedToMarkdown";
      public const string OriginalExtensionSource = "OriginalExtensionSource";
    }

    public static class AppId
    {
      public const string Xamarin = "Xamarin";
      public const string VisualStudioSub = "VisualStudioSub";
      public const string VSTS = "VSTS";
    }

    public static class MetaDataDeploymentTechnologyTypes
    {
      public const string ReferralLinkText = "Referral Link";
      public const string ExeText = "EXE";
      public const string MsiText = "MSI";
      public const string VsixText = "VSIX";

      public static string GetDeployementTechnologyText(
        ExtensionDeploymentTechnology deploymentTechnology)
      {
        switch (deploymentTechnology)
        {
          case ExtensionDeploymentTechnology.Exe:
            return "EXE";
          case ExtensionDeploymentTechnology.Msi:
            return "MSI";
          case ExtensionDeploymentTechnology.Vsix:
            return "VSIX";
          case ExtensionDeploymentTechnology.ReferralLink:
            return "Referral Link";
          default:
            return (string) null;
        }
      }

      public static bool TryParseExtensionDeploymentTechnology(
        string metaDataDeploymentTechnologyText,
        out ExtensionDeploymentTechnology result)
      {
        if (string.Equals(metaDataDeploymentTechnologyText, "VSIX", StringComparison.OrdinalIgnoreCase))
        {
          result = ExtensionDeploymentTechnology.Vsix;
          return true;
        }
        if (string.Equals(metaDataDeploymentTechnologyText, "EXE", StringComparison.OrdinalIgnoreCase))
        {
          result = ExtensionDeploymentTechnology.Exe;
          return true;
        }
        if (string.Equals(metaDataDeploymentTechnologyText, "MSI", StringComparison.OrdinalIgnoreCase))
        {
          result = ExtensionDeploymentTechnology.Msi;
          return true;
        }
        if (string.Equals(metaDataDeploymentTechnologyText, "Referral Link", StringComparison.OrdinalIgnoreCase))
        {
          result = ExtensionDeploymentTechnology.ReferralLink;
          return true;
        }
        result = (ExtensionDeploymentTechnology) 0;
        return false;
      }
    }

    public static class ItemTags
    {
      public const string SYSTEM_TAG_ID = "$";
      public const string FEATURED_TAG = "$featured";
      public const string FEATURED_TAG1 = "$featured1";
      public const string FEATURED_TAG2 = "$featured2";
      public const string OFFER_MONTHLY_TAG = "$Monthly";
      public const string OFFER_ANNUAL_TAG = "$Annual";
      public const string PREVIEW_TAG = "$preview";
      public const string PAID_TAG = "$IsPaid";
      public const string OFFERS_TAG_NAME = "Offer";
      public const string DO_NOT_DOWNLOAD_SYSTEM_TAG = "$DoNotDownload";
      public const string DO_NOT_DOWNLOAD_TAG = "__DoNotDownload";
      public const string SRC_MARKET = "__market";
      public const string TRIAL_DAYS = "__TrialDays";
      public const string SRC_MARKET_DOLLAR_TAG = "$market";
      public const string BYOL = "__BYOL";
      public const string BYOLENFORCED = "__BYOLEnforced";
      public const string WEB_EXTENSION_TAG = "__web_extension";
    }

    public static class SmallIconMaxSize
    {
      public const int Width = 153;
      public const int Height = 72;
    }

    public static class XamarinUniversity
    {
      public const string PublisherName = "ms";
      public const string ExtensionName = "xamarin-university";
      public const string FullyQualifiedName = "ms.xamarin-university";
    }

    public static class VisualStudioEnterprise
    {
      public const string PublisherName = "ms";
      public const string ExtensionNameMonthly = "vs-enterprise-monthly";
      public const string FullyQualifiedNameMonthly = "ms.vs-enterprise-monthly";
      public const string ExtensionNameAnnual = "vs-enterprise-annual";
      public const string FullyQualifiedNameAnnual = "ms.vs-enterprise-annual";
    }

    public static class VisualStudioProfessional
    {
      public const string PublisherName = "ms";
      public const string ExtensionNameMonthly = "vs-professional-monthly";
      public const string FullyQualifiedNameMonthly = "ms.vs-professional-monthly";
      public const string ExtensionNameAnnual = "vs-professional-annual";
      public const string FullyQualifiedNameAnnual = "ms.vs-professional-annual";
    }
  }
}
