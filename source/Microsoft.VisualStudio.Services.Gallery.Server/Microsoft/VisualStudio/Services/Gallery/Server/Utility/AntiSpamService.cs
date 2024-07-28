// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Utility.AntiSpamService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Utility
{
  public static class AntiSpamService
  {
    private const string ServiceLayer = "AntiSpamService";
    private const string UrlRegex = "\\b(?:https?://)\\S+\\b";
    private static readonly Regex LinkRegex = new Regex("\\b(?:https?://)\\S+\\b", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromSeconds(60.0));
    internal const string RegistryPathForApprovedPublishers = "/Configuration/Service/Gallery/ExtensionValidation/ApprovedPublishers";
    internal const string RegistryPathForDefiningOldPublisherTimeline_InMonths = "/Configuration/Service/Gallery/PublisherValidation/OldPublisherTimeline_InMonths";
    internal const string RegistryPathForExtensionBlockedSpamWords = "/Configuration/Service/Gallery/ExtensionValidation/BlockedSpamWords";
    internal const string RegistryPathForPublisherBlockedSpamWords = "/Configuration/Service/Gallery/PublisherValidation/BlockedSpamWords";
    internal const string RegistryPathForExtensionBlockedReferralHosts = "/Configuration/Service/Gallery/ExtensionValidation/BlockedReferralHosts";
    internal const string RegistryPathForPublisherBlockedReferralHosts = "/Configuration/Service/Gallery/PublisherValidation/BlockedReferralHosts";

    public static void PublisherProfileHasSuspectedSpamContent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher)
    {
      requestContext.Trace(12062077, TraceLevel.Info, "gallery", nameof (AntiSpamService), "Publisher anti-spam validation started for publisher ID: {0}", (object) publisher.PublisherId);
      string spamContent;
      if (AntiSpamService.HasSpamContentInMetadata(AntiSpamService.GetPublisherMetadataToValidate(publisher), AntiSpamService.GetPublisherBlockedSpamWords(requestContext), out spamContent))
      {
        requestContext.Trace(12062077, TraceLevel.Info, "gallery", nameof (AntiSpamService), "Publisher anti-spam validation failed for publisher ID: {0}", (object) publisher.PublisherId);
        AntiSpamService.PublishSuspiciousPublisherDetectedEvent(requestContext, publisher, "SpamPublisherKeyWordValidation", spamContent: spamContent);
        throw new PublisherSpamValidationException(GalleryResources.PublisherMetadataSpamCheckError());
      }
      requestContext.Trace(12062077, TraceLevel.Info, "gallery", nameof (AntiSpamService), "Publisher anti-spam validation successful for publisher ID: {0}", (object) publisher.PublisherId);
    }

    public static bool ExtensionHasSuspectedSpamContent(
      IVssRequestContext requestContext,
      UnpackagedExtensionData extensionData,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher,
      bool isVSExtension = true)
    {
      requestContext.Trace(12062077, TraceLevel.Info, "gallery", nameof (AntiSpamService), "Extension anti-spam validation started for extension: {0}", (object) extensionData.ExtensionName);
      string spamContent;
      if (AntiSpamService.HasSpamContentInMetadata(AntiSpamService.GetExtensionMetadataToValidate(extensionData, publisher), AntiSpamService.GetExtensionBlockedSpamWords(requestContext), out spamContent))
      {
        requestContext.Trace(12062077, TraceLevel.Info, "gallery", nameof (AntiSpamService), "Extension anti-spam validation failed for extension: {0}", (object) extensionData.ExtensionName);
        AntiSpamService.PublishSuspiciousExtensionDetectedEvent(requestContext, extensionData, publisher, "SpamExtensionKeyWordValidation", isVSExtension, spamContent: spamContent);
        return true;
      }
      requestContext.Trace(12062077, TraceLevel.Info, "gallery", nameof (AntiSpamService), "Extension anti-spam validation successful for extension: {0}", (object) extensionData.ExtensionName);
      return false;
    }

    public static bool ExtensionHasBlockedHost(
      IVssRequestContext requestContext,
      UnpackagedExtensionData extensionData,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher,
      bool isVSExtension = true)
    {
      if (AntiSpamService.IsKnownGenuinePublisher(requestContext, publisher))
        return false;
      requestContext.Trace(12062077, TraceLevel.Info, "gallery", nameof (AntiSpamService), "Extension anti-spam validation started for the extension: {0}", (object) extensionData.ExtensionName);
      List<string> source = new List<string>();
      if (!string.IsNullOrEmpty(extensionData.ReferralUrl))
      {
        string str = extensionData.ReferralUrl.Normalize(NormalizationForm.FormKC);
        source.Add(str);
      }
      if (!string.IsNullOrEmpty(extensionData.RepositoryUrl))
      {
        string str = extensionData.RepositoryUrl.Normalize(NormalizationForm.FormKC);
        source.Add(str);
      }
      if (source.Any<string>())
      {
        IReadOnlyCollection<string> blockedReferralHosts = AntiSpamService.GetExtensionBlockedReferralHosts(requestContext);
        foreach (string str1 in source)
        {
          Uri result;
          if (Uri.TryCreate(str1, UriKind.Absolute, out result))
          {
            foreach (string str2 in (IEnumerable<string>) blockedReferralHosts)
            {
              string a = str2.Normalize(NormalizationForm.FormKC);
              if (string.Equals(a, result.Host, StringComparison.OrdinalIgnoreCase) || result.Host.EndsWith("." + a, StringComparison.OrdinalIgnoreCase))
              {
                requestContext.Trace(12062077, TraceLevel.Info, "gallery", nameof (AntiSpamService), "Extension anti-spam validation failed for extension: {0}", (object) extensionData.ExtensionName);
                AntiSpamService.PublishSuspiciousExtensionDetectedEvent(requestContext, extensionData, publisher, "SpamExtensionUrlValidation", isVSExtension, str1);
                return true;
              }
            }
          }
        }
      }
      requestContext.Trace(12062077, TraceLevel.Info, "gallery", nameof (AntiSpamService), "Extension anti-spam validation successful for extension: {0}", (object) extensionData.ExtensionName);
      return false;
    }

    public static bool ContentHasBlockedHosts(
      IVssRequestContext requestContext,
      string content,
      UnpackagedExtensionData extensionData,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher,
      bool isVSExtension = true)
    {
      IReadOnlyCollection<string> blockedReferralHosts = AntiSpamService.GetExtensionBlockedReferralHosts(requestContext);
      if (!string.IsNullOrWhiteSpace(content) && blockedReferralHosts.Any<string>())
      {
        foreach (Capture match in AntiSpamService.LinkRegex.Matches(content))
        {
          Uri result;
          if (Uri.TryCreate(match.Value.Normalize(NormalizationForm.FormKC), UriKind.Absolute, out result) && AntiSpamService.IsBlockedReferralHost(result, blockedReferralHosts))
          {
            requestContext.Trace(12062077, TraceLevel.Info, "gallery", nameof (AntiSpamService), "Extension Overview Content anti-spam validation failed for extension: {0}", (object) extensionData.ExtensionName);
            AntiSpamService.PublishSuspiciousExtensionDetectedEvent(requestContext, extensionData, publisher, "SpamExtensionOverviewContentValidation", isVSExtension, result.ToString());
            return true;
          }
        }
      }
      requestContext.Trace(12062077, TraceLevel.Info, "gallery", nameof (AntiSpamService), "Extension Overview Content anti-spam validation successful for extension: {0}", (object) extensionData.ExtensionName);
      return false;
    }

    public static bool PublisherHasBlockedHostsInReferrenceLinks(
      IVssRequestContext requestContext,
      ReferenceLinks referenceLinks,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher)
    {
      if (referenceLinks == null || referenceLinks.Links == null || referenceLinks.Links.Values == null)
        return false;
      IReadOnlyCollection<string> blockedReferralHosts = AntiSpamService.GetPublisherBlockedReferralHosts(requestContext);
      foreach (ReferenceLink referenceLink in referenceLinks.Links.Values)
      {
        Uri result;
        if (!string.IsNullOrEmpty(referenceLink.Href) && Uri.TryCreate(referenceLink.Href, UriKind.Absolute, out result))
        {
          string b = result.Host.Normalize(NormalizationForm.FormKC);
          foreach (string str in (IEnumerable<string>) blockedReferralHosts)
          {
            string a = str.Normalize(NormalizationForm.FormKC);
            if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase) || b.EndsWith("." + a, StringComparison.OrdinalIgnoreCase))
            {
              AntiSpamService.PublishSuspiciousPublisherDetectedEvent(requestContext, publisher, "SpamPublisherValidation", referenceLink.Href);
              return true;
            }
          }
        }
      }
      return false;
    }

    private static bool HasSpamContentInMetadata(
      IReadOnlyCollection<string> extensionMetadataToValidate,
      IReadOnlyCollection<string> spamDetectionRules,
      out string spamContent)
    {
      foreach (string spamDetectionRule in (IEnumerable<string>) spamDetectionRules)
      {
        List<string> list = ((IEnumerable<string>) spamDetectionRule.Split(new char[1]
        {
          ','
        }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (blockedHost => blockedHost.Trim())).ToList<string>();
        bool flag1 = true;
        string str1 = string.Empty;
        foreach (string str2 in list)
        {
          string lowerInvariant = str2.ToLowerInvariant();
          bool flag2 = false;
          foreach (string str3 in (IEnumerable<string>) extensionMetadataToValidate)
          {
            if (str3.Contains(lowerInvariant))
            {
              flag2 = true;
              string str4;
              if (!(str1 == string.Empty))
                str4 = string.Join(",", new string[2]
                {
                  str1,
                  lowerInvariant
                });
              else
                str4 = lowerInvariant;
              str1 = str4;
              break;
            }
          }
          if (!flag2)
          {
            flag1 = false;
            break;
          }
        }
        if (flag1)
        {
          spamContent = str1;
          return true;
        }
      }
      spamContent = string.Empty;
      return false;
    }

    private static void PublishSuspiciousExtensionDetectedEvent(
      IVssRequestContext requestContext,
      UnpackagedExtensionData extension,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher,
      string featureName,
      bool isVSExtension = true,
      string blockedHost = null,
      string spamContent = null)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(CustomerIntelligenceProperty.Action, featureName);
      if (publisher != null)
      {
        properties.Add("PublisherId", (object) publisher.PublisherId);
        properties.Add("PublisherName", publisher.PublisherName);
        properties.Add("PublisherDisplayName", publisher.DisplayName);
        properties.Add("PublisherShortDescription", publisher.ShortDescription);
        properties.Add("PublisherLongDescription", publisher.LongDescription);
      }
      if (extension != null)
      {
        properties.Add("DraftId", (object) extension.DraftId);
        properties.Add("ExtensionName", extension.ExtensionName);
        properties.Add("ExtensionDisplayName", extension.DisplayName);
        properties.Add("ExtensionShortDescription", extension.Description);
        properties.Add("ReferralUrl", extension.ReferralUrl);
        properties.Add("RepositoryUrl", extension.RepositoryUrl);
      }
      properties.Add(nameof (isVSExtension), isVSExtension);
      if (!string.IsNullOrEmpty(blockedHost))
        properties.Add("BlockedHost", blockedHost);
      if (!string.IsNullOrEmpty(spamContent))
        properties.Add("SpamContent", spamContent);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", featureName, properties);
    }

    public static bool IsKnownGenuinePublisher(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher)
    {
      if (publisher == null)
        return false;
      if (publisher.IsDomainVerified || requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.BypassSpamCheckForOldPublishers") && AntiSpamService.IsOldPublisher(requestContext, publisher))
        return true;
      IReadOnlyCollection<string> approvedPublishers = AntiSpamService.GetApprovedPublishers(requestContext);
      if (!approvedPublishers.Any<string>())
        return false;
      foreach (string a in (IEnumerable<string>) approvedPublishers)
      {
        if (string.Equals(a, publisher.PublisherName, StringComparison.InvariantCultureIgnoreCase))
        {
          requestContext.Trace(12062077, TraceLevel.Info, "gallery", nameof (AntiSpamService), "{0} is an approved publisher", (object) publisher.PublisherName);
          return true;
        }
      }
      return false;
    }

    private static bool IsBlockedReferralHost(
      Uri uriToVerify,
      IReadOnlyCollection<string> blockedReferralHosts)
    {
      foreach (string blockedReferralHost in (IEnumerable<string>) blockedReferralHosts)
      {
        string a = blockedReferralHost.Normalize(NormalizationForm.FormKC);
        if (string.Equals(a, uriToVerify.Host, StringComparison.OrdinalIgnoreCase) || uriToVerify.Host.EndsWith("." + a, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    private static IReadOnlyCollection<string> GetPublisherMetadataToValidate(Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher)
    {
      List<string> metadataToValidate = new List<string>();
      if (!string.IsNullOrEmpty(publisher.PublisherName))
        metadataToValidate.Add(publisher.PublisherName.ToLowerInvariant());
      if (!string.IsNullOrEmpty(publisher.DisplayName))
        metadataToValidate.Add(publisher.DisplayName.ToLowerInvariant());
      if (!string.IsNullOrEmpty(publisher.LongDescription))
        metadataToValidate.Add(publisher.LongDescription.ToLowerInvariant());
      if (!string.IsNullOrEmpty(publisher.ShortDescription))
        metadataToValidate.Add(publisher.ShortDescription.ToLowerInvariant());
      return (IReadOnlyCollection<string>) metadataToValidate;
    }

    private static IReadOnlyCollection<string> GetExtensionMetadataToValidate(
      UnpackagedExtensionData extensionData,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher)
    {
      List<string> metadataToValidate = new List<string>();
      if (!string.IsNullOrEmpty(publisher.DisplayName))
        metadataToValidate.Add(publisher.DisplayName.ToLowerInvariant());
      if (!string.IsNullOrEmpty(publisher.PublisherName))
        metadataToValidate.Add(publisher.PublisherName.ToLowerInvariant());
      if (!string.IsNullOrEmpty(extensionData.DisplayName))
        metadataToValidate.Add(extensionData.DisplayName.ToLowerInvariant());
      if (!string.IsNullOrEmpty(extensionData.ExtensionName))
        metadataToValidate.Add(extensionData.ExtensionName.ToLowerInvariant());
      if (!string.IsNullOrEmpty(extensionData.Description))
        metadataToValidate.Add(extensionData.Description.ToLowerInvariant());
      if (!string.IsNullOrEmpty(publisher.LongDescription))
        metadataToValidate.Add(publisher.LongDescription.ToLowerInvariant());
      if (!string.IsNullOrEmpty(publisher.ShortDescription))
        metadataToValidate.Add(publisher.ShortDescription.ToLowerInvariant());
      return (IReadOnlyCollection<string>) metadataToValidate;
    }

    private static IReadOnlyCollection<string> GetApprovedPublishers(
      IVssRequestContext requestContext)
    {
      return AntiSpamService.SplitDelimiterSeparatedTerms(requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/ExtensionValidation/ApprovedPublishers", string.Empty), '|');
    }

    private static IReadOnlyCollection<string> GetPublisherBlockedSpamWords(
      IVssRequestContext requestContext)
    {
      return AntiSpamService.SplitDelimiterSeparatedTerms(requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PublisherValidation/BlockedSpamWords", string.Empty), '|');
    }

    private static IReadOnlyCollection<string> GetExtensionBlockedSpamWords(
      IVssRequestContext requestContext)
    {
      return AntiSpamService.SplitDelimiterSeparatedTerms(requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/ExtensionValidation/BlockedSpamWords", string.Empty), '|');
    }

    private static bool IsOldPublisher(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher) => DateTime.UtcNow.AddMonths(-requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PublisherValidation/OldPublisherTimeline_InMonths", 6)) >= publisher.LastUpdated;

    private static IReadOnlyCollection<string> GetExtensionBlockedReferralHosts(
      IVssRequestContext requestContext)
    {
      return AntiSpamService.SplitDelimiterSeparatedTerms(requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/ExtensionValidation/BlockedReferralHosts", string.Empty), ';');
    }

    private static IReadOnlyCollection<string> GetPublisherBlockedReferralHosts(
      IVssRequestContext requestContext)
    {
      return AntiSpamService.SplitDelimiterSeparatedTerms(requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PublisherValidation/BlockedReferralHosts", string.Empty), ';');
    }

    private static IReadOnlyCollection<string> SplitDelimiterSeparatedTerms(
      string input,
      char delimiter)
    {
      if (string.IsNullOrEmpty(input))
        return (IReadOnlyCollection<string>) new List<string>();
      return (IReadOnlyCollection<string>) ((IEnumerable<string>) input.Split(new char[1]
      {
        delimiter
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (i => i.Trim())).ToList<string>();
    }

    private static void PublishSuspiciousPublisherDetectedEvent(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher,
      string featureName,
      string blockedHost = null,
      string spamContent = null)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(CustomerIntelligenceProperty.Action, featureName);
      if (!string.IsNullOrEmpty(publisher.PublisherName))
        properties.Add("PublisherName", publisher.PublisherName);
      if (!string.IsNullOrEmpty(publisher.DisplayName))
        properties.Add("PublisherDisplayName", publisher.DisplayName);
      if (!string.IsNullOrEmpty(publisher.ShortDescription))
        properties.Add("PublisherShortDescription", publisher.ShortDescription);
      if (!string.IsNullOrEmpty(publisher.LongDescription))
        properties.Add("PublisherLongDescription", publisher.LongDescription);
      if (!string.IsNullOrEmpty(blockedHost))
        properties.Add("BlockedHost", blockedHost);
      if (!string.IsNullOrEmpty(spamContent))
        properties.Add("SpamContent", spamContent);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", featureName, properties);
    }
  }
}
