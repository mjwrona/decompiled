// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.PublisherService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.IndexUpdater;
using Microsoft.VisualStudio.Services.Gallery.Server.Utility;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.SecurityRoles;
using Microsoft.VisualStudio.Services.SecurityRoles.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class PublisherService : IPublisherService, IVssFrameworkService
  {
    private const int MaxQueryPageSize = 1000;
    private const int MinLogoHeight = 128;
    private const int MaxLogoHeight = 128;
    private const int MaxLogoWidth = 128;
    private const int SimilarityPercentageBoundary = 75;
    private string _disallowedWordsRegistryValue = string.Empty;
    private int _similarityPercentageBoundary = 75;
    private const string s_registryPathForMaxNumberOfPublishersPerAccount = "/Configuration/Service/Gallery/PublisherValidation/MaxNumberOfPublishersPerAccount";
    internal const string RegistryPathForApprovedVsids = "/Configuration/Service/Gallery/PublisherValidation/ApprovedVsids";
    internal const string RegistryPathForApprovedUserAgentsForByPassingReCaptcha = "/Configuration/Service/Gallery/PublisherValidation/ApprovedUserAgentsForByPassingReCaptcha";
    [StaticSafe]
    private static readonly IDictionary<string, string> ImageExtensionToContentTypeMap = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        ".jpg",
        "image/jpeg"
      },
      {
        ".jpeg",
        "image/jpeg"
      },
      {
        ".bmp",
        "image/png"
      },
      {
        ".gif",
        "image/gif"
      },
      {
        ".png",
        "image/png"
      }
    };

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      RegistryQuery filter = (RegistryQuery) "/Configuration/Service/Gallery/DisallowedWordsInPublisherDisplayName";
      service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.UpdateDisallowedNamesRegistryValue), in filter);
      this._disallowedWordsRegistryValue = service.GetValue<string>(systemRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/DisallowedWordsInPublisherDisplayName", string.Join(','.ToString(), (IEnumerable<string>) GalleryServiceConstants.DefaultReservedPublisherDisplayNames));
      this._similarityPercentageBoundary = service.GetValue<int>(systemRequestContext, (RegistryQuery) "/Configuration/Service/Gallery/SimilarityPercentageBoundary", 75);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.UpdateDisallowedNamesRegistryValue));

    public void UpdateDisallowedNamesRegistryValue(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this._disallowedWordsRegistryValue = changedEntries["/Configuration/Service/Gallery/DisallowedWordsInPublisherDisplayName"].Value;
    }

    public Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher CreatePublisher(
      IVssRequestContext requestContext,
      string publisherName,
      string displayName,
      PublisherFlags flags,
      string shortDescription,
      string longDescription)
    {
      return this.CreatePublisher(requestContext, publisherName, displayName, flags, shortDescription, longDescription, (ReferenceLinks) null, PublisherState.None, (string) null, (string) null);
    }

    private bool IsUserAgentApproved(IVssRequestContext requestContext)
    {
      if (string.IsNullOrWhiteSpace(requestContext.UserAgent))
        return false;
      return ((IEnumerable<string>) requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PublisherValidation/ApprovedUserAgentsForByPassingReCaptcha", string.Empty).Split(new char[1]
      {
        ';'
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (approvedUserAgent => approvedUserAgent.Trim())).ToList<string>().Where<string>((Func<string, bool>) (approvedUserAgent => requestContext.UserAgent.ToLowerInvariant().Contains(approvedUserAgent.ToLowerInvariant()))).ToList<string>().Count > 0;
    }

    private void ValidateRecaptchaToken(
      IVssRequestContext requestContext,
      string publisherName,
      string reCaptchaToken,
      string scenario)
    {
      Guid userId = requestContext.GetUserId();
      if (ReCaptchaUtility.IsReCaptchaTokenValid(requestContext, reCaptchaToken))
      {
        this.PublishReCaptchaTokenCIForPublisherProfile(requestContext, publisherName, userId, "Valid", scenario, reCaptchaToken);
      }
      else
      {
        this.PublishReCaptchaTokenCIForPublisherProfile(requestContext, publisherName, userId, "Invalid", scenario, reCaptchaToken);
        throw new InvalidReCaptchaTokenException(GalleryResources.InvalidReCaptchaToken());
      }
    }

    public Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher CreatePublisher(
      IVssRequestContext requestContext,
      string publisherName,
      string displayName,
      PublisherFlags flags,
      string shortDescription,
      string longDescription,
      ReferenceLinks links,
      PublisherState state,
      string domain = null,
      string reCaptchaToken = null)
    {
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher1 = (Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher) null;
      PublisherDbModel publisherDbModel = (PublisherDbModel) null;
      Guid userId = requestContext.GetUserId();
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaForCreatePublisherProfile"))
      {
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DisableReCaptchaBypassForCLICreatePublisherProfile"))
        {
          if (string.IsNullOrWhiteSpace(reCaptchaToken))
          {
            this.PublishReCaptchaTokenCIForPublisherProfile(requestContext, publisherName, userId, "NoReCaptcha", "CreateScenario", reCaptchaToken);
            throw new InvalidReCaptchaTokenException(GalleryResources.ErrorCreatePublisherNoRecaptchaToken());
          }
          this.ValidateRecaptchaToken(requestContext, publisherName, reCaptchaToken, "CreateScenario");
        }
        else if (!this.IsUserAgentApproved(requestContext))
          this.ValidateRecaptchaToken(requestContext, publisherName, reCaptchaToken, "CreateScenario");
      }
      GalleryUtil.CheckPublisherName(publisherName);
      if (displayName != null)
        displayName = displayName.Trim();
      this.ValidatePublisherDisplayName(requestContext, displayName);
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableDuplicatePublisherDisplayNameCheck"))
        this.ValidatePublisherDisplayNameIfInUse(requestContext, displayName);
      HashSet<string> collidedPublisherDisplayNames;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableTypoSquattingCheckForPublisherDisplayName") && requestContext.GetService<ITyposquattingService>().DoesSimilarPublisherDisplayNameExist(requestContext, displayName, publisherName, out collidedPublisherDisplayNames))
        throw new ArgumentException(GalleryResources.SimilarPublisherDisplayNameExists((object) string.Join(",", (IEnumerable<string>) collidedPublisherDisplayNames)));
      if (longDescription != null && longDescription.Length > 1024)
        throw new Exception(GalleryResources.PublisherDescriptionExceededLengthMessage((object) 1024));
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DisableUrlsInPublisherProfile") && links?.Links != null && links.Links.Any<KeyValuePair<string, object>>())
        throw new UrlsNotAllowedinPublisherProfileException(GalleryResources.UrlNotAllowedinPublisherProfileError());
      GallerySecurity.CheckRootPermission(requestContext, PublisherPermissions.CreatePublisher);
      Guid publisherKey = Guid.NewGuid();
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.CheckForPublisherSpam"))
      {
        Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher2 = new Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher();
        publisher2.DisplayName = displayName;
        publisher2.PublisherName = publisherName;
        publisher2.LongDescription = longDescription;
        publisher2.ShortDescription = shortDescription;
        Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher3 = publisher2;
        AntiSpamService.PublisherProfileHasSuspectedSpamContent(requestContext, publisher3);
      }
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.LimitMaxNumberOfPublishersPerAccount") && this.HasExceededMaxNumberOfPublishersPerAccount(requestContext, userId))
      {
        this.PublishLimitExceededPerAccountEvent(requestContext, userId, "ExceededMaxNumberOfPublishersPerAccount");
        throw new PublisherLimitExceededException(GalleryResources.MaxLimitOnPublisherError());
      }
      flags &= ~PublisherFlags.UnChanged;
      if ((flags & PublisherFlags.ServiceFlags) != PublisherFlags.None)
        GallerySecurity.CheckRootPermission(requestContext, PublisherPermissions.Admin);
      PublisherState state1 = PublisherState.None;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.MarkPublishersVerifiedByDefault"))
      {
        flags |= PublisherFlags.Verified;
      }
      else
      {
        if (state.HasFlag((Enum) PublisherState.VerificationPending))
          this.ValidatePublisherApplyingForVerification(longDescription, links);
        state1 |= state & PublisherState.VerificationPending;
      }
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.CheckForPublisherBlockedHosts") && AntiSpamService.PublisherHasBlockedHostsInReferrenceLinks(requestContext, links, publisher1))
        throw new BlockedHostValidationException(GalleryResources.PublisherProfileContainsBlockedHosts());
      if (!domain.IsNullOrEmpty<char>())
      {
        domain = domain.Trim();
        if (!domain.IsNullOrEmpty<char>() && !GalleryServerUtil.IsValidDomain(requestContext, domain))
          throw new ArgumentException(GalleryResources.DomainURLIncorrect());
      }
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
      {
        PublisherComponent7 publisherComponent7 = component as PublisherComponent7;
        PublisherMetadata metadata = new PublisherMetadata()
        {
          PublisherDetailsLinks = PublisherLinksHelper.GetValidatedPublisherDetailsLinks(links)
        };
        publisherDbModel = publisherComponent7 != null ? publisherComponent7.CreatePublisher(publisherName, displayName, userId, flags, shortDescription, longDescription, publisherKey, DateTime.UtcNow, metadata, state1, domain) : (component as PublisherComponent6).CreatePublisher(publisherName, displayName, userId, flags, shortDescription, longDescription, publisherKey, DateTime.UtcNow, metadata, state1);
      }
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher4 = publisherDbModel.GetPublisher(requestContext);
      GallerySecurity.OnDataChanged(requestContext);
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableDuplicatePublisherDisplayNameCheck"))
        GalleryServerUtil.NotifyGalleryDataChanged(requestContext, GalleryNotificationEventIds.PublisherUpdateDelete, "created:" + GalleryServerUtil.CalculateSHA256Hash(displayName.ToUpper()));
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.MarkPublishersVerifiedByDefault") && state.HasFlag((Enum) PublisherState.VerificationPending))
        this.SendMailNotificationForPublisherVerification(requestContext, publisher4);
      this.PublishCustomerIntelligenceEvent(requestContext, publisherName, flags, "Created");
      if (domain != null)
        this.PublishCustomerIntelligenceEventForDomain(requestContext, publisherName, domain, false);
      return publisher4;
    }

    private void PublishReCaptchaTokenCIForPublisherProfile(
      IVssRequestContext requestContext,
      string publisherName,
      Guid requestingIdentityId,
      string featureValidation,
      string scenario,
      string recaptchaToken)
    {
      bool flag = GalleryServerUtil.IsRequestFromChinaRegion(requestContext);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("Source", "Publisher");
      properties.Add(CustomerIntelligenceProperty.Action, "ReCaptchaValidation");
      properties.Add("Scenario", scenario);
      properties.Add("FeatureValidation", featureValidation);
      properties.Add("UserAgent", requestContext.UserAgent);
      properties.Add("PublisherName", publisherName);
      properties.Add("RecaptchaToken", recaptchaToken);
      properties.Add("IsRequestFromChinaRegion", flag);
      properties.Add("Vsid", (object) requestingIdentityId);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "ReCaptchaValidation", properties);
    }

    private void PublishLimitExceededPerAccountEvent(
      IVssRequestContext requestContext,
      Guid requestingIdentityId,
      string featureName)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(CustomerIntelligenceProperty.Action, featureName);
      properties.Add("Vsid", (object) requestingIdentityId);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", featureName, properties);
    }

    private bool HasExceededMaxNumberOfPublishersPerAccount(
      IVssRequestContext requestContext,
      Guid vsid)
    {
      if (vsid == Guid.Empty)
        return false;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int num1 = service.GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PublisherValidation/MaxNumberOfPublishersPerAccount", 100);
      int num2 = 0;
      List<string> stringList = new List<string>();
      using (PublisherComponent6 component = requestContext.CreateComponent<PublisherComponent6>())
        stringList = component.QueryPublishersByVsid(vsid.ToString());
      if (stringList.Count < num1)
        return false;
      foreach (string str in stringList)
      {
        Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher1 = new Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher();
        publisher1.PublisherName = str;
        Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher2 = publisher1;
        if (PublisherService.HasWritePermissions(GallerySecurity.GetPublisherRoleForGivenVSID(requestContext, publisher2, vsid)))
          ++num2;
      }
      return !((IEnumerable<string>) service.GetValue<string>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PublisherValidation/ApprovedVsids", string.Empty).Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (allowedVsid => allowedVsid.Trim())).ToList<string>().Contains(vsid.ToString()) && num2 > num1;
    }

    private static bool HasWritePermissions(SecurityRole role)
    {
      if (role == null)
        return false;
      return role.AllowPermissions == 4043 || role.AllowPermissions == 715 || role.AllowPermissions == 521;
    }

    public void DeletePublisher(IVssRequestContext requestContext, string publisherName)
    {
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher = (Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher) null;
      PublisherDbModel publisherDbModel = (PublisherDbModel) null;
      int num = -1;
      string str = (string) null;
      GalleryUtil.CheckPublisherName(publisherName);
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
      {
        if (!(component is PublisherComponent5 publisherComponent5))
          publisher = (component as PublisherComponent4).QueryPublisher(publisherName, PublisherQueryFlags.IncludeExtensions);
        else
          publisherDbModel = publisherComponent5.QueryPublisherDbModel(publisherName, PublisherQueryFlags.IncludeExtensions);
      }
      if (publisherDbModel != null)
      {
        publisher = publisherDbModel.GetPublisher(requestContext);
        num = publisherDbModel.LogoFileId;
        str = publisherDbModel.DisplayName;
      }
      PublisherPermissions requestedPermissions = PublisherPermissions.DeletePublisher;
      GallerySecurity.CheckPublisherPermission(requestContext, publisher, requestedPermissions);
      List<PublishedExtension> extensions = publisher.Extensions;
      PublishedExtensionService service1 = requestContext.GetService<PublishedExtensionService>();
      if (extensions != null)
      {
        foreach (PublishedExtension publishedExtension in extensions)
          service1.DeleteExtension(requestContext, publisherName, publishedExtension.ExtensionName, (string) null, true);
      }
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
        component.DeletePublisher(publisherName);
      if (num > -1)
      {
        try
        {
          ITeamFoundationFileService service2 = requestContext.GetService<ITeamFoundationFileService>();
          ICDNService service3 = requestContext.GetService<ICDNService>();
          CDNPathUtil cdnAssetPath = new CDNPathUtil()
          {
            PublisherName = publisher.PublisherName
          };
          IVssRequestContext requestContext1 = requestContext;
          long fileId = (long) num;
          service2.DeleteFile(requestContext1, fileId);
          service3.DeletePublisherAsset(requestContext, cdnAssetPath, num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(12062043, "gallery", nameof (DeletePublisher), ex);
        }
      }
      GallerySecurity.OnDataChanged(requestContext);
      if (extensions != null)
        GalleryServerUtil.QueueExtensionDataCleanUpJob(requestContext, extensions);
      if (!string.IsNullOrWhiteSpace(str) && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableDuplicatePublisherDisplayNameCheck"))
        GalleryServerUtil.NotifyGalleryDataChanged(requestContext, GalleryNotificationEventIds.PublisherUpdateDelete, "deleted:" + GalleryServerUtil.CalculateSHA256Hash(str.ToUpper()));
      this.PublishCustomerIntelligenceEvent(requestContext, publisherName, publisher.Flags, "Deleted");
    }

    public Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher QueryPublisher(
      IVssRequestContext requestContext,
      string publisherName,
      PublisherQueryFlags flags,
      bool allowAnonymousAccess = false)
    {
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher = (Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher) null;
      PublisherDbModel publisherDbModel = (PublisherDbModel) null;
      GalleryUtil.CheckPublisherName(publisherName);
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
      {
        if (!(component is PublisherComponent5 publisherComponent5))
          publisher = (component as PublisherComponent4).QueryPublisher(publisherName, flags);
        else
          publisherDbModel = publisherComponent5.QueryPublisherDbModel(publisherName, flags);
      }
      if (publisherDbModel != null)
        publisher = publisherDbModel.GetPublisher(requestContext);
      if (!allowAnonymousAccess && !GallerySecurity.HasPublisherPermission(requestContext, publisher, PublisherPermissions.Read))
        publisher = (Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher) null;
      if (publisher != null)
      {
        if (publisher.Extensions != null)
        {
          PublishedExtensionService service = requestContext.GetService<PublishedExtensionService>();
          for (int index = 0; index < publisher.Extensions.Count; ++index)
          {
            PublishedExtension extension = publisher.Extensions[index];
            if (!GallerySecurity.HasExtensionPermission(requestContext, extension, (string) null, PublisherPermissions.Read, false) && !GallerySecurity.HasExtensionPermission(requestContext, extension, (string) null, PublisherPermissions.PrivateRead, false))
              publisher.Extensions.RemoveAt(index--);
            else
              service.PrepareExtensionForClient(requestContext, extension, ExtensionQueryFlags.None);
          }
        }
        if ((flags & PublisherQueryFlags.IncludeEmailAddress) != PublisherQueryFlags.None)
          publisher.EmailAddress = this.GetOwnerEmailAddresses(requestContext, publisherName);
      }
      return publisher;
    }

    public void LinkPendingProfile(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity publisherIdentity)
    {
      string puid = publisherIdentity.Descriptor.Identifier.Substring(0, 16);
      using (PendingUserProfileComponent component = requestContext.CreateComponent<PendingUserProfileComponent>())
        component.LinkPendingProfile(puid, publisherIdentity.Id);
    }

    public PublisherQueryResult QueryPublishers(
      IVssRequestContext requestContext,
      PublisherQuery publisherQuery)
    {
      return this.QueryPublishers(requestContext, publisherQuery, false);
    }

    public PublisherQueryResult QueryPublishers(
      IVssRequestContext requestContext,
      PublisherQuery publisherQuery,
      bool queryForAllIdentities)
    {
      if (publisherQuery == null)
        throw new ArgumentNullException(nameof (publisherQuery));
      PublishedExtensionService service = requestContext.GetService<PublishedExtensionService>();
      PublisherQueryResult publisherQueryResult = (PublisherQueryResult) null;
      PublisherDbModelQueryResult modelQueryResult = (PublisherDbModelQueryResult) null;
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Guid empty = Guid.Empty;
      List<QueryFilterValue> filterValues = new List<QueryFilterValue>();
      for (int index1 = 0; index1 < publisherQuery.Filters.Count; ++index1)
      {
        QueryFilter filter = publisherQuery.Filters[index1];
        filter.QueryIndex = index1;
        if (filter.PageSize <= 0 || filter.PageSize > 1000)
          filter.PageSize = 1000;
        for (int index2 = 0; index2 < filter.Criteria.Count; ++index2)
        {
          FilterCriteria criterion = filter.Criteria[index2];
          PublisherQueryFilterType filterType = (PublisherQueryFilterType) criterion.FilterType;
          if (filterType != PublisherQueryFilterType.My)
            throw new ArgumentException();
          Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
          string str = !queryForAllIdentities || string.IsNullOrWhiteSpace(criterion.Value) ? userIdentity.Id.ToString() : criterion.Value;
          filterValues.Add(new QueryFilterValue()
          {
            QueryIndex = filter.QueryIndex,
            FilterIndex = 0,
            FilterValueType = (int) filterType,
            FilterValue = str
          });
        }
      }
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
      {
        PublisherQueryFlags flags = publisherQuery.Flags;
        if (!(component is PublisherComponent5 publisherComponent5))
          publisherQueryResult = component.QueryPublishersByQuery(publisherQuery.Filters, filterValues, flags);
        else
          modelQueryResult = publisherComponent5.QueryPublisherDbModelsByQuery(publisherQuery.Filters, filterValues, flags);
      }
      if (modelQueryResult != null)
      {
        publisherQueryResult = new PublisherQueryResult()
        {
          Results = new List<PublisherFilterResult>()
        };
        foreach (PublisherDbModelFilterResult result in modelQueryResult.Results)
        {
          PublisherFilterResult publisherFilterResult = new PublisherFilterResult();
          if (result.Publishers != null)
          {
            publisherFilterResult.Publishers = new List<Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher>();
            foreach (PublisherDbModel publisher in result.Publishers)
              publisherFilterResult.Publishers.Add(publisher.GetPublisher(requestContext));
          }
          publisherQueryResult.Results.Add(publisherFilterResult);
        }
      }
      foreach (PublisherFilterResult result in publisherQueryResult.Results)
      {
        if (result.Publishers != null)
        {
          for (int index3 = 0; index3 < result.Publishers.Count; ++index3)
          {
            Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher = result.Publishers[index3];
            if (!GallerySecurity.HasPublisherPermission(requestContext, publisher, PublisherPermissions.Read))
            {
              requestContext.Trace(12061032, TraceLevel.Info, "gallery", nameof (QueryPublishers), string.Format("Removed PublisherId = {0},Removed PublisherName = {1}, PublisherFlags = {2}", (object) publisher.PublisherId, (object) publisher.PublisherName, (object) publisher.Flags));
              result.Publishers.RemoveAt(index3--);
            }
            else if (publisher.Extensions != null)
            {
              for (int index4 = 0; index4 < publisher.Extensions.Count; ++index4)
              {
                PublishedExtension extension = publisher.Extensions[index4];
                if (!GallerySecurity.HasExtensionPermission(requestContext, extension, (string) null, PublisherPermissions.Read, false) && !GallerySecurity.HasExtensionPermission(requestContext, extension, (string) null, PublisherPermissions.PrivateRead, false))
                {
                  requestContext.Trace(12061031, TraceLevel.Info, "gallery", nameof (QueryPublishers), string.Format("Removed ExtensionId = {0}, PublisherId = {1}, PublisherName = {2}, ExtensionFlags = {3}, Removed Extension Name = {4}, PublisherFlags = {5}", (object) extension.ExtensionId, (object) publisher.PublisherId, (object) publisher.PublisherName, (object) extension.Flags, (object) extension.DisplayName, (object) publisher.Flags));
                  publisher.Extensions.RemoveAt(index4--);
                }
                else
                {
                  string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName);
                  if (!stringSet.Contains(fullyQualifiedName))
                  {
                    service.PrepareExtensionForClient(requestContext, extension, ExtensionQueryFlags.None);
                    stringSet.Add(fullyQualifiedName);
                  }
                }
              }
            }
          }
        }
      }
      return publisherQueryResult;
    }

    public Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher UpdatePublisher(
      IVssRequestContext requestContext,
      string publisherName,
      string displayName,
      PublisherFlags flags,
      string shortDescription,
      string longDescription,
      string reCaptchaToken = null)
    {
      return this.UpdatePublisher(requestContext, publisherName, displayName, flags, shortDescription, longDescription, new int?(), (ReferenceLinks) null, PublisherState.None, (string) null, false, false, reCaptchaToken);
    }

    public Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher UpdatePublisher(
      IVssRequestContext requestContext,
      string publisherName,
      string displayName,
      PublisherFlags flags,
      string shortDescription,
      string longDescription,
      int? logoFileId,
      ReferenceLinks links,
      PublisherState state,
      string domain = null,
      bool isDomainVerified = false,
      bool callFromLR = false,
      string reCaptchaToken = null)
    {
      PublisherDbModel publisherDbModel1 = (PublisherDbModel) null;
      PublisherDbModel publisherDbModel2 = (PublisherDbModel) null;
      Guid token = new Guid();
      bool flag = false;
      string str = (string) null;
      Guid userId = requestContext.GetUserId();
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaForUpdatePublisherProfile") && !this.IsTopPublisherOrPendingTopPublisher(state, flags) && !callFromLR)
      {
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DisableReCaptchaBypassForCLIUpdatePublisherProfile"))
        {
          if (string.IsNullOrWhiteSpace(reCaptchaToken))
          {
            this.PublishReCaptchaTokenCIForPublisherProfile(requestContext, publisherName, userId, "NoReCaptcha", "UpdateScenario", reCaptchaToken);
            throw new InvalidReCaptchaTokenException(GalleryResources.ErrorUpdatePublisherNoRecaptchaToken());
          }
          this.ValidateRecaptchaToken(requestContext, publisherName, reCaptchaToken, "UpdateScenario");
        }
        else if (!this.IsUserAgentApproved(requestContext))
          this.ValidateRecaptchaToken(requestContext, publisherName, reCaptchaToken, "UpdateScenario");
      }
      GalleryUtil.CheckPublisherName(publisherName);
      if (longDescription != null && longDescription.Length > 1024)
        throw new Exception(GalleryResources.PublisherDescriptionExceededLengthMessage((object) 1024));
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher1 = new Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher();
      publisher1.DisplayName = displayName;
      publisher1.PublisherName = publisherName;
      publisher1.LongDescription = longDescription;
      publisher1.ShortDescription = shortDescription;
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher2 = publisher1;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.CheckForPublisherSpam"))
        AntiSpamService.PublisherProfileHasSuspectedSpamContent(requestContext, publisher2);
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.MarkPublishersVerifiedByDefault") && state.HasFlag((Enum) PublisherState.VerificationPending))
        this.ValidatePublisherApplyingForVerification(longDescription, links);
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.CheckForPublisherBlockedHosts") && AntiSpamService.PublisherHasBlockedHostsInReferrenceLinks(requestContext, links, publisher2))
        throw new BlockedHostValidationException(GalleryResources.PublisherProfileContainsBlockedHosts());
      PublisherQueryFlags flags1 = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableExternalSearchIndexPopulationForAzureDevOps") ? PublisherQueryFlags.IncludeExtensions : PublisherQueryFlags.None;
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
        publisherDbModel1 = component is PublisherComponent7 publisherComponent7 ? publisherComponent7.QueryPublisherDbModel(publisherName, flags1) : (component as PublisherComponent6).QueryPublisherDbModel(publisherName, flags1);
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher3 = publisherDbModel1.GetPublisher(requestContext);
      bool removeDomain = false;
      this.SetDomainRelatedProperties(requestContext, publisher3, callFromLR, ref domain, ref isDomainVerified, ref removeDomain, ref token);
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DisableUrlsInPublisherProfile") && links != null && publisher3.Links != null && this.AreUrlsUpdated(links.Links, publisher3.Links.Links))
        throw new UrlsNotAllowedinPublisherProfileException(GalleryResources.UrlNotAllowedinPublisherProfileError());
      if (!string.IsNullOrEmpty(displayName) && !string.Equals(displayName, publisher3.DisplayName, StringComparison.OrdinalIgnoreCase))
      {
        this.ValidatePublisherDisplayName(requestContext, displayName);
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableDuplicatePublisherDisplayNameCheck"))
          this.ValidatePublisherDisplayNameIfInUse(requestContext, displayName);
        HashSet<string> collidedPublisherDisplayNames;
        if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableTypoSquattingCheckForPublisherDisplayName") && requestContext.GetService<ITyposquattingService>().DoesSimilarPublisherDisplayNameExist(requestContext, displayName, publisherName, out collidedPublisherDisplayNames) && !collidedPublisherDisplayNames.Contains(publisher3.DisplayName))
          throw new ArgumentException(GalleryResources.SimilarPublisherDisplayNameExists((object) string.Join(",", (IEnumerable<string>) collidedPublisherDisplayNames)));
        flag = true;
        str = publisher3.DisplayName;
      }
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.MarkPublishersVerifiedByDefault") && !string.IsNullOrEmpty(displayName) && publisher3.Flags.HasFlag((Enum) PublisherFlags.Verified) && !displayName.Equals(publisher3.DisplayName, StringComparison.OrdinalIgnoreCase) && !GallerySecurity.HasRootPermission(requestContext, PublisherPermissions.Admin))
        throw new PublisherVerifiedNameChangeException(GalleryResources.VerifiedPublisherNameChangeNotAllowed());
      PublisherPermissions requestedPermissions = PublisherPermissions.EditSettings;
      GallerySecurity.CheckPublisherPermission(requestContext, publisher3, requestedPermissions);
      if ((flags & PublisherFlags.UnChanged) != PublisherFlags.None)
        flags = PublisherFlags.UnChanged;
      if (flags != publisher3.Flags && (flags & PublisherFlags.ServiceFlags) != PublisherFlags.None)
        GallerySecurity.CheckRootPermission(requestContext, PublisherPermissions.Admin);
      PublisherState state1 = PublisherState.None;
      TeamFoundationExecutionEnvironment executionEnvironment = requestContext.ExecutionEnvironment;
      if (!executionEnvironment.IsOnPremisesDeployment)
        state1 = this.ValidateAndGetPublisherState(requestContext, publisher3.Flags, publisher3.State, flags, state);
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
      {
        PublisherComponent9 publisherComponent9 = component as PublisherComponent9;
        int? logoFileId1 = !logoFileId.HasValue ? new int?(publisherDbModel1.LogoFileId) : logoFileId;
        PublisherFlags flags2 = flags;
        executionEnvironment = requestContext.ExecutionEnvironment;
        if (!executionEnvironment.IsOnPremisesDeployment)
        {
          flags2 |= publisher3.Flags;
          if (!flags.HasFlag((Enum) PublisherFlags.Certified) && publisher3.Flags.HasFlag((Enum) PublisherFlags.Certified) && state.HasFlag((Enum) PublisherState.CertificationRevoked))
            flags2 = flags2 & ~PublisherFlags.UnChanged & ~PublisherFlags.Certified;
        }
        PublisherMetadata publisherMetadata;
        if (links != null)
          publisherMetadata = new PublisherMetadata()
          {
            PublisherDetailsLinks = PublisherLinksHelper.GetValidatedPublisherDetailsLinks(links)
          };
        else
          publisherMetadata = publisherDbModel1.Metadata;
        PublisherMetadata metadata = publisherMetadata;
        if (!domain.IsNullOrEmpty<char>() && !GalleryServerUtil.IsValidDomain(requestContext, domain))
          throw new ArgumentException(GalleryResources.DomainURLIncorrect());
        publisherDbModel2 = publisherComponent9 != null ? publisherComponent9.UpdatePublisher(publisherName, displayName, flags2, shortDescription, longDescription, Guid.Empty, DateTime.UtcNow, logoFileId1, metadata, state1, domain, isDomainVerified, removeDomain, isDomainVerified & callFromLR, token) : (component as PublisherComponent8).UpdatePublisher(publisherName, displayName, flags2, shortDescription, longDescription, Guid.Empty, DateTime.UtcNow, logoFileId1, metadata, state1, domain, isDomainVerified, removeDomain, isDomainVerified & callFromLR);
      }
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableUnVerifyDomainOnDisplayNameChange") && flag & isDomainVerified)
      {
        requestContext.GetService<IPublisherDomainVerificationService>().MarkPublisherDomainAsUnverified(requestContext, publisherName);
        requestContext.TraceAlways(12062108, TraceLevel.Warning, "gallery", "updatedPublisher", "Publisher domain verification revoked for " + publisherName);
      }
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher4 = publisherDbModel2.GetPublisher(requestContext);
      GallerySecurity.OnDataChanged(requestContext);
      GalleryServerUtil.NotifyGalleryDataChanged(requestContext, GalleryNotificationEventIds.ExtensionUpdateDelete, "UpdatePublisher: " + publisherName);
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableDuplicatePublisherDisplayNameCheck") && flag && !string.IsNullOrWhiteSpace(displayName) && !string.IsNullOrWhiteSpace(str))
        GalleryServerUtil.NotifyGalleryDataChanged(requestContext, GalleryNotificationEventIds.PublisherUpdateDelete, "updated:" + GalleryServerUtil.CalculateSHA256Hash(str.ToUpper()) + ";" + GalleryServerUtil.CalculateSHA256Hash(displayName.ToUpper()));
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.MarkPublishersVerifiedByDefault") && state.HasFlag((Enum) PublisherState.VerificationPending))
        this.SendMailNotificationForPublisherVerification(requestContext, publisher4);
      else if (state.HasFlag((Enum) PublisherState.CertificationPending))
        this.SendMailNotificationForPublisherCertification(requestContext, publisher4);
      if (domain != null && !callFromLR)
        this.PublishCustomerIntelligenceEventForDomain(requestContext, publisherName, domain, isDomainVerified);
      this.PublishClientTraceEventForUpdatePublisher(requestContext, publisher3, publisher4, "Updated");
      this.PublishCustomerIntelligenceEventAndAuditLogForPublisherCertification(requestContext, publisher3, publisher4);
      if (!publisher3.Extensions.IsNullOrEmpty<PublishedExtension>())
      {
        SearchHelper searchHelper = new SearchHelper();
        try
        {
          searchHelper.UpdateSearchIndex(requestContext, publisher3.Extensions);
        }
        catch (Exception ex)
        {
          string format = "Failed to update publisher display name for extensions in the search index.Publisher: " + publisher3.PublisherName + ", Publisher DisplayName: " + publisher3.DisplayName + "Exception " + ex.Message;
          requestContext.TraceAlways(12060103, TraceLevel.Error, "gallery", nameof (PublisherService), format);
        }
      }
      this.PublishPublisherUpdatedEvent(requestContext, publisher4.PublisherName);
      return publisher4;
    }

    public void PublishPublisherUpdatedEvent(
      IVssRequestContext requestContext,
      string publisherName)
    {
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnablePMPIntegration") || !requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnablePublisherEventsPublishing"))
        return;
      requestContext.GetService<IEventPublisherService>().PublishPublisherUpdatedEvent(requestContext, publisherName);
    }

    private bool IsTopPublisherOrPendingTopPublisher(PublisherState state, PublisherFlags flags) => state.HasFlag((Enum) PublisherState.CertificationPending) || flags.HasFlag((Enum) PublisherFlags.Certified);

    private bool AreUrlsUpdated(
      IReadOnlyDictionary<string, object> updatedLinks,
      IReadOnlyDictionary<string, object> existingLinks)
    {
      if (updatedLinks == null && existingLinks == null)
        return false;
      if (updatedLinks == null || existingLinks == null)
        return true;
      foreach (string key in existingLinks.Select<KeyValuePair<string, object>, string>((Func<KeyValuePair<string, object>, string>) (x => x.Key)).Union<string>(updatedLinks.Select<KeyValuePair<string, object>, string>((Func<KeyValuePair<string, object>, string>) (x => x.Key))).ToList<string>())
      {
        if (key != "logo" && key != "fallbackLogo")
        {
          ReferenceLink existingLink = existingLinks.ContainsKey(key) ? existingLinks[key] as ReferenceLink : (ReferenceLink) null;
          ReferenceLink updatedLink = updatedLinks.ContainsKey(key) ? updatedLinks[key] as ReferenceLink : (ReferenceLink) null;
          if (existingLink == null || updatedLink == null || !updatedLink.Href.Equals(existingLink.Href, StringComparison.OrdinalIgnoreCase))
            return true;
        }
      }
      return false;
    }

    public int UpdatePublisherAsset(
      IVssRequestContext requestContext,
      string publisherName,
      string assetFileName,
      Stream assetStream,
      string assetType)
    {
      if (!string.Equals(assetType, "logo", StringComparison.InvariantCultureIgnoreCase))
        throw new ArgumentException(GalleryResources.InvalidAssetType((object) assetType));
      PublisherDbModel publisherDbModel1 = (PublisherDbModel) null;
      int num = -1;
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
        publisherDbModel1 = component is PublisherComponent5 publisherComponent5 ? publisherComponent5.QueryPublisherDbModel(publisherName, PublisherQueryFlags.None) : throw new NotImplementedException(GalleryResources.PublisherLogoUploadFailedMessage());
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher = publisherDbModel1 != null ? publisherDbModel1.GetPublisher(requestContext) : throw new PublisherDoesNotExistException(GalleryResources.PublisherDoesNotExist());
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher oldPublisher = publisher;
      PublisherPermissions requestedPermissions = PublisherPermissions.EditSettings;
      GallerySecurity.CheckPublisherPermission(requestContext, publisher, requestedPermissions);
      if (assetStream != null)
      {
        this.ValidatePublisherLogo(requestContext, assetStream);
        string typeForLogoImage = this.GetContentyTypeForLogoImage(assetFileName);
        Stream stream = new ImageResizeUtils(requestContext).ResizeImage(assetStream, 128, 128, typeForLogoImage);
        if (stream != null)
          assetStream = stream;
        ITeamFoundationFileService service1 = requestContext.GetService<ITeamFoundationFileService>();
        ICDNService service2 = requestContext.GetService<ICDNService>();
        num = service1.UploadFile(requestContext, assetStream, OwnerId.Gallery, Guid.Empty);
        CDNPathUtil cdnAssetPath = new CDNPathUtil()
        {
          PublisherName = publisher.PublisherName
        };
        using (MemoryStream memoryStream = new MemoryStream())
        {
          assetStream.Seek(0L, SeekOrigin.Begin);
          using (GZipStream destination = new GZipStream((Stream) memoryStream, CompressionMode.Compress, true))
            assetStream.CopyTo((Stream) destination);
          memoryStream.Seek(0L, SeekOrigin.Begin);
          if (!service2.UploadPublisherAssetWithStream(requestContext, cdnAssetPath, (Stream) memoryStream, num.ToString((IFormatProvider) CultureInfo.InvariantCulture), typeForLogoImage))
            requestContext.TraceAlways(12062042, TraceLevel.Warning, "gallery", nameof (UpdatePublisherAsset), "Publisher asset upload to CDN failed");
        }
        PublisherDbModel publisherDbModel2;
        using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
        {
          if (!(component is PublisherComponent9 publisherComponent9))
          {
            publisherDbModel2 = (component as PublisherComponent6).UpdatePublisher(publisherName, publisher.DisplayName, publisher.Flags, publisher.ShortDescription, publisher.LongDescription, Guid.Empty, DateTime.UtcNow, new int?(num), publisherDbModel1.Metadata, publisher.State);
          }
          else
          {
            string publisherName1 = publisherName;
            string displayName = publisher.DisplayName;
            int flags = (int) publisher.Flags;
            string shortDescription = publisher.ShortDescription;
            string longDescription = publisher.LongDescription;
            Guid empty = Guid.Empty;
            DateTime utcNow = DateTime.UtcNow;
            int? logoFileId = new int?(num);
            PublisherMetadata metadata = publisherDbModel1.Metadata;
            int state = (int) publisher.State;
            Guid token = new Guid();
            publisherDbModel2 = publisherComponent9.UpdatePublisher(publisherName1, displayName, (PublisherFlags) flags, shortDescription, longDescription, empty, utcNow, logoFileId, metadata, (PublisherState) state, (string) null, token: token);
          }
        }
        if (publisherDbModel1.LogoFileId > -1)
        {
          try
          {
            service1.DeleteFile(requestContext, (long) publisherDbModel1.LogoFileId);
            service2.DeletePublisherAsset(requestContext, cdnAssetPath, publisherDbModel1.LogoFileId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          }
          catch (Exception ex)
          {
            requestContext.TraceAlways(12062042, TraceLevel.Warning, "gallery", nameof (UpdatePublisherAsset), string.Format("Deleting old publisher asset with fileId {0} failed", (object) publisherDbModel1.LogoFileId));
          }
        }
        this.PublishClientTraceEventForUpdatePublisher(requestContext, oldPublisher, publisherDbModel2.GetPublisher(requestContext), "Updated", true);
      }
      else
        requestContext.TraceAlways(12062042, TraceLevel.Warning, "gallery", nameof (UpdatePublisherAsset), "Asset file stream was null");
      return num;
    }

    public void DeletePublisherAsset(
      IVssRequestContext requestContext,
      string publisherName,
      string assetType)
    {
      if (!string.Equals(assetType, "logo", StringComparison.InvariantCultureIgnoreCase))
        throw new ArgumentException(GalleryResources.InvalidAssetType((object) assetType));
      PublisherDbModel publisherDbModel1 = (PublisherDbModel) null;
      PublisherDbModel publisherDbModel2 = (PublisherDbModel) null;
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
        publisherDbModel1 = component is PublisherComponent5 publisherComponent5 ? publisherComponent5.QueryPublisherDbModel(publisherName, PublisherQueryFlags.None) : throw new NotImplementedException(GalleryResources.PublisherLogoDeletionFailedMessage());
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher = publisherDbModel1 != null ? publisherDbModel1.GetPublisher(requestContext) : throw new PublisherDoesNotExistException(GalleryResources.PublisherDoesNotExist());
      PublisherPermissions requestedPermissions = PublisherPermissions.EditSettings;
      GallerySecurity.CheckPublisherPermission(requestContext, publisher, requestedPermissions);
      if (publisherDbModel1.LogoFileId > -1)
      {
        using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
        {
          if (!(component is PublisherComponent9 publisherComponent9))
          {
            publisherDbModel2 = (component as PublisherComponent6).UpdatePublisher(publisherName, publisher.DisplayName, publisher.Flags, publisher.ShortDescription, publisher.LongDescription, Guid.Empty, DateTime.UtcNow, new int?(-1), publisherDbModel1.Metadata, publisherDbModel1.State);
          }
          else
          {
            string publisherName1 = publisherName;
            string displayName = publisher.DisplayName;
            int flags = (int) publisher.Flags;
            string shortDescription = publisher.ShortDescription;
            string longDescription = publisher.LongDescription;
            Guid empty = Guid.Empty;
            DateTime utcNow = DateTime.UtcNow;
            int? logoFileId = new int?(-1);
            PublisherMetadata metadata = publisherDbModel1.Metadata;
            int state = (int) publisherDbModel1.State;
            Guid token = new Guid();
            publisherDbModel2 = publisherComponent9.UpdatePublisher(publisherName1, displayName, (PublisherFlags) flags, shortDescription, longDescription, empty, utcNow, logoFileId, metadata, (PublisherState) state, (string) null, token: token);
          }
        }
        try
        {
          ICDNService service = requestContext.GetService<ICDNService>();
          CDNPathUtil cdnPathUtil = new CDNPathUtil()
          {
            PublisherName = publisher.PublisherName
          };
          IVssRequestContext requestContext1 = requestContext;
          CDNPathUtil cdnAssetPath = cdnPathUtil;
          string assetName = publisherDbModel1.LogoFileId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          service.DeletePublisherAsset(requestContext1, cdnAssetPath, assetName);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(12062043, "gallery", nameof (DeletePublisherAsset), ex);
        }
        try
        {
          requestContext.GetService<ITeamFoundationFileService>().DeleteFile(requestContext, (long) publisherDbModel1.LogoFileId);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(12062043, "gallery", nameof (DeletePublisherAsset), ex);
        }
        this.PublishClientTraceEventForUpdatePublisher(requestContext, publisher, publisherDbModel2.GetPublisher(requestContext), "Updated");
      }
      else
        requestContext.TraceAlways(12062043, TraceLevel.Warning, "gallery", nameof (DeletePublisherAsset), "Publisher doesn't have asset to be deleted");
    }

    public Stream GetPublisherAsset(
      IVssRequestContext requestContext,
      string publisherName,
      string assetType)
    {
      if (!string.Equals(assetType, "logo", StringComparison.InvariantCultureIgnoreCase))
        throw new ArgumentException(GalleryResources.InvalidAssetType((object) assetType));
      PublisherDbModel publisherDbModel = (PublisherDbModel) null;
      Stream publisherAsset = (Stream) null;
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
        publisherDbModel = component is PublisherComponent5 publisherComponent5 ? publisherComponent5.QueryPublisherDbModel(publisherName, PublisherQueryFlags.None) : throw new NotImplementedException(GalleryResources.PublisherLogoUploadFailedMessage());
      if (publisherDbModel == null)
        throw new PublisherDoesNotExistException(GalleryResources.PublisherDoesNotExist());
      if (publisherDbModel.LogoFileId > -1)
        publisherAsset = requestContext.GetService<ITeamFoundationFileService>().RetrieveFile(requestContext, (long) publisherDbModel.LogoFileId, out CompressionType _);
      else
        requestContext.TraceAlways(12062045, TraceLevel.Warning, "gallery", nameof (GetPublisherAsset), "Publisher doesn't have asset {0} to be returned", (object) assetType);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(CustomerIntelligenceProperty.Action, nameof (GetPublisherAsset));
      properties.Add("Name", publisherDbModel.PublisherName);
      properties.Add("LogoFileId", (double) publisherDbModel.LogoFileId);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "Publisher", properties);
      return publisherAsset;
    }

    public void ManagePublisherVisibility(
      IVssRequestContext requestContext,
      string identifier,
      List<Guid> identityIds,
      bool removeTags = false)
    {
      ArgumentUtility.CheckForNull<List<Guid>>(identityIds, nameof (identityIds));
      string[] strArray = identifier.Split('.');
      string publisherName = strArray[0];
      string extensionName = strArray.Length > 1 ? strArray[1] : (string) null;
      GalleryUtil.CheckPublisherName(publisherName);
      if (!string.IsNullOrEmpty(extensionName))
        GalleryUtil.CheckExtensionName(extensionName);
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher;
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
        publisher = component.QueryPublisher(publisherName, PublisherQueryFlags.None);
      PublisherPermissions requestedPermissions = PublisherPermissions.ManagePermissions;
      GallerySecurity.CheckPublisherPermission(requestContext, publisher, requestedPermissions);
      foreach (Guid identityId in identityIds)
      {
        using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
          component.ManagePublisherVisibility(publisherName, extensionName, identityId, removeTags);
      }
      GallerySecurity.OnDataChanged(requestContext);
    }

    public ICollection<Microsoft.VisualStudio.Services.Identity.Identity> GetOwnerIdentitiesOfPublisher(
      IVssRequestContext requestContext,
      string publisherName)
    {
      return (ICollection<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetIdentitiesOfNamespace(requestContext, "/" + publisherName, PublisherPermissions.EditSettings);
    }

    public ICollection<Microsoft.VisualStudio.Services.Identity.Identity> GetContributorIdentitiesOfPublisherOrExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      return (ICollection<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetIdentitiesOfPublisherOrExtension(requestContext, publisherName, extensionName, PublisherPermissions.UpdateExtension);
    }

    public ICollection<Microsoft.VisualStudio.Services.Identity.Identity> GetCreatorIdentitiesOfPublisherOrExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      return (ICollection<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetIdentitiesOfPublisherOrExtension(requestContext, publisherName, extensionName, PublisherPermissions.PublishExtension);
    }

    public string GetReservedPublisherDisplayName() => this._disallowedWordsRegistryValue;

    private List<Microsoft.VisualStudio.Services.Identity.Identity> GetIdentitiesOfExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      PublisherPermissions permissions)
    {
      string securityNamespace = "/" + publisherName + "/" + extensionName;
      return this.GetIdentitiesOfNamespace(requestContext, securityNamespace, permissions);
    }

    private List<Microsoft.VisualStudio.Services.Identity.Identity> GetIdentitiesOfPublisher(
      IVssRequestContext requestContext,
      string publisherName,
      PublisherPermissions permissions)
    {
      return this.GetIdentitiesOfNamespace(requestContext, "/" + publisherName, permissions);
    }

    private List<Microsoft.VisualStudio.Services.Identity.Identity> GetIdentitiesOfPublisherOrExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      PublisherPermissions permissions)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> publisherOrExtension = this.GetIdentitiesOfExtension(requestContext, publisherName, extensionName, permissions);
      List<Microsoft.VisualStudio.Services.Identity.Identity> identitiesOfPublisher = this.GetIdentitiesOfPublisher(requestContext, publisherName, permissions);
      if (identitiesOfPublisher != null)
      {
        if (publisherOrExtension != null)
        {
          foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identitiesOfPublisher)
          {
            if (!publisherOrExtension.Contains(identity))
              publisherOrExtension.Add(identity);
          }
        }
        else
          publisherOrExtension = identitiesOfPublisher;
      }
      return publisherOrExtension;
    }

    private List<Microsoft.VisualStudio.Services.Identity.Identity> GetIdentitiesOfNamespace(
      IVssRequestContext requestContext,
      string securityNamespace,
      PublisherPermissions permissions)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> identitiesOfNamespace = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (GallerySecurity.HasRootPermission(requestContext, PublisherPermissions.Admin))
      {
        IVssSecurityNamespace securityNamespace1 = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, GallerySecurity.PublisherSecurityNamespace);
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        IVssRequestContext requestContext1 = vssRequestContext;
        string token = securityNamespace;
        IAccessControlList accessControlList = securityNamespace1.QueryAccessControlList(requestContext1, token, (IEnumerable<IdentityDescriptor>) null, false);
        if (accessControlList != null)
        {
          IdentityService service = vssRequestContext.GetService<IdentityService>();
          foreach (IAccessControlEntry accessControlEntry in accessControlList.AccessControlEntries)
          {
            if (accessControlEntry != null && ((PublisherPermissions) accessControlEntry.Allow & permissions) != (PublisherPermissions) 0)
            {
              Microsoft.VisualStudio.Services.Identity.Identity identity = service.ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
              {
                accessControlEntry.Descriptor
              }, QueryMembership.None, (IEnumerable<string>) null).First<Microsoft.VisualStudio.Services.Identity.Identity>();
              identitiesOfNamespace.Add(identity);
            }
          }
        }
      }
      return identitiesOfNamespace;
    }

    private void PublishCustomerIntelligenceEvent(
      IVssRequestContext requestContext,
      string publisherName,
      PublisherFlags publisherFlags,
      string action)
    {
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add(CustomerIntelligenceProperty.Action, action);
      intelligenceData.Add("Name", publisherName);
      intelligenceData.Add("VerifiedPublisher", publisherFlags.HasFlag((Enum) PublisherFlags.Verified));
      intelligenceData.AddGalleryUserIdentifier(requestContext);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "Publisher", intelligenceData);
    }

    private void PublishCustomerIntelligenceEventForDomain(
      IVssRequestContext requestContext,
      string publisherName,
      string domain,
      bool isDomainVerified)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(CustomerIntelligenceProperty.Action, "AddOrUpdateDomain");
      properties.Add("Name", publisherName);
      properties.Add("Domain", domain);
      properties.Add("IsDomainVerified", isDomainVerified);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "Publisher", properties);
    }

    private void PublishClientTraceEventForUpdatePublisher(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher oldPublisher,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher modifiedPublisher,
      string action,
      bool isLogoUpdated = false)
    {
      ClientTraceData clientTraceData1 = new ClientTraceData();
      clientTraceData1.Add(CustomerIntelligenceProperty.Action, (object) action);
      clientTraceData1.Add("Name", (object) oldPublisher.PublisherName);
      if (!oldPublisher.DisplayName.Equals(modifiedPublisher.DisplayName))
      {
        clientTraceData1.Add("OldDisplayName", (object) oldPublisher.DisplayName);
        clientTraceData1.Add("NewDisplayName", (object) modifiedPublisher.DisplayName);
      }
      if (!oldPublisher.Flags.Equals((object) modifiedPublisher.Flags))
      {
        ClientTraceData clientTraceData2 = clientTraceData1;
        PublisherFlags flags = oldPublisher.Flags;
        string str1 = flags.ToString();
        clientTraceData2.Add("OldPublisherFlags", (object) str1);
        ClientTraceData clientTraceData3 = clientTraceData1;
        flags = modifiedPublisher.Flags;
        string str2 = flags.ToString();
        clientTraceData3.Add("NewPublisherFlags", (object) str2);
      }
      clientTraceData1.Add("VerifiedPublisher", (object) modifiedPublisher.Flags.HasFlag((Enum) PublisherFlags.Verified));
      clientTraceData1.AddGalleryUserIdentifier(requestContext);
      clientTraceData1.Add("IsPublisherLogoUpdated", (object) isLogoUpdated);
      requestContext.GetService<ClientTraceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "Publisher", clientTraceData1);
    }

    private List<string> GetOwnerEmailAddresses(
      IVssRequestContext requestContext,
      string publisherName)
    {
      List<string> ownerEmailAddresses = new List<string>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetOwnerIdentitiesOfPublisher(requestContext, publisherName))
        ownerEmailAddresses.Add(identity.GetProperty<string>("Mail", ""));
      return ownerEmailAddresses;
    }

    public AzurePublisher QueryAssociatedAzurePublisher(
      IVssRequestContext requestContext,
      string publisherName)
    {
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher;
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
        publisher = component.QueryPublisher(publisherName, PublisherQueryFlags.None);
      AzurePublisher azurePublisher;
      using (AzurePublisherComponent component = requestContext.CreateComponent<AzurePublisherComponent>())
        azurePublisher = component.QueryAssociatedAzurePublisher(publisherName);
      if (!GallerySecurity.HasPublisherPermission(requestContext, publisher, PublisherPermissions.EditSettings))
        azurePublisher = (AzurePublisher) null;
      return azurePublisher;
    }

    public AzurePublisher AssociateAzurePublisher(
      IVssRequestContext requestContext,
      AzurePublisher azurePublisher)
    {
      GallerySecurity.CheckRootPermission(requestContext, PublisherPermissions.CreatePublisher);
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher;
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
        publisher = component.QueryPublisher(azurePublisher.PublisherName, PublisherQueryFlags.None);
      GallerySecurity.CheckPublisherPermission(requestContext, publisher, PublisherPermissions.EditSettings);
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      using (AzurePublisherComponent component = requestContext.CreateComponent<AzurePublisherComponent>())
        azurePublisher = component.UpdateAzurePublisher(azurePublisher.PublisherName, azurePublisher.AzurePublisherId, userIdentity.Id);
      return azurePublisher;
    }

    private void SendMailNotificationForPublisherVerification(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher)
    {
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.MarkPublishersVerifiedByDefault") || publisher == null)
        return;
      PublisherVerificationMailNotificationEvent notificationEvent = new PublisherVerificationMailNotificationEvent(requestContext);
      notificationEvent.PublisherName = publisher.PublisherName;
      notificationEvent.DisplayName = publisher.DisplayName;
      notificationEvent.Description = publisher.LongDescription;
      notificationEvent.CompanyLink = this.GetPublisherDetailsLink(publisher.Links, "company");
      notificationEvent.SupportLink = this.GetPublisherDetailsLink(publisher.Links, "support");
      notificationEvent.SourceCodeLink = this.GetPublisherDetailsLink(publisher.Links, "sourceCode");
      notificationEvent.LinkedInLink = this.GetPublisherDetailsLink(publisher.Links, "linkedIn");
      notificationEvent.TwitterLink = this.GetPublisherDetailsLink(publisher.Links, "twitter");
      notificationEvent.LogoLink = this.GetPublisherDetailsLink(publisher.Links, "logo");
      List<RoleAssignment> roleAssignments = requestContext.GetService<ISecurityRoleMappingService>().GetRoleAssignments(requestContext, publisher.PublisherName, "gallery.publisher");
      List<Guid> ccIdentities = new List<Guid>();
      foreach (RoleAssignment roleAssignment in roleAssignments)
      {
        if (roleAssignment.Role.AllowPermissions == 4043)
          ccIdentities.Add(new Guid(roleAssignment.Identity.Id));
      }
      new MailNotification().SendMailNotificationUsingTeamFoundationMailService(requestContext, (IList<Guid>) ccIdentities, (MailNotificationEventData) notificationEvent, "Publisher verification request for " + publisher.PublisherName);
    }

    private void SendMailNotificationForPublisherCertification(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher)
    {
      if (publisher == null)
        return;
      PublisherCertificationMailNotificationEvent notificationEvent = new PublisherCertificationMailNotificationEvent(requestContext);
      notificationEvent.PublisherName = publisher.PublisherName;
      notificationEvent.DisplayName = publisher.DisplayName;
      notificationEvent.ManagePageLink = GalleryServerUtil.GetManagePageUrl(requestContext, publisher.PublisherName);
      List<RoleAssignment> roleAssignments = requestContext.GetService<ISecurityRoleMappingService>().GetRoleAssignments(requestContext, publisher.PublisherName, "gallery.publisher");
      List<Guid> ccIdentities = new List<Guid>();
      foreach (RoleAssignment roleAssignment in roleAssignments)
      {
        if (roleAssignment.Role.AllowPermissions == 4043)
          ccIdentities.Add(new Guid(roleAssignment.Identity.Id));
      }
      new MailNotification().SendMailNotificationUsingTeamFoundationMailService(requestContext, (IList<Guid>) ccIdentities, (MailNotificationEventData) notificationEvent, "Publisher Certification Request.");
    }

    private string GetPublisherDetailsLink(ReferenceLinks links, string linkType)
    {
      string publisherDetailsLink = string.Empty;
      object obj;
      if (links != null && links.Links.TryGetValue(linkType, out obj))
        publisherDetailsLink = (obj as ReferenceLink).Href;
      return publisherDetailsLink;
    }

    private void ValidatePublisherDisplayName(IVssRequestContext requestContext, string displayName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(displayName, nameof (displayName));
      GalleryUtil.CheckPublisherDisplayName(displayName);
      if (GallerySecurity.HasRootPermission(requestContext, PublisherPermissions.Admin) || this._disallowedWordsRegistryValue.IsNullOrEmpty<char>())
        return;
      ReservedPublisher.IsPublisherNameReserved(this._disallowedWordsRegistryValue, displayName, this._similarityPercentageBoundary);
    }

    private void ValidatePublisherDisplayNameIfInUse(
      IVssRequestContext requestContext,
      string displayName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(displayName, nameof (displayName));
      if (requestContext.GetService<IDuplicatePublisherCheckService>().DoesPublisherDisplayNameExists(requestContext, displayName))
        throw new ArgumentException(GalleryResources.PublisherDisplayNameAlreadyExists());
    }

    private void ValidatePublisherLogo(IVssRequestContext requestContext, Stream logoStream)
    {
      long publisherLogoSizeInBytes = GalleryServerUtil.GetMaxPublisherLogoSizeInBytes(requestContext, 2097152L);
      if (logoStream.Length > publisherLogoSizeInBytes)
        throw new Exception(GalleryResources.PublisherLogoSizeExceededMaxLimit());
      using (Bitmap bitmap = new Bitmap(logoStream))
      {
        if (bitmap.Height >= 128)
        {
          if (bitmap.Width == bitmap.Height)
            goto label_9;
        }
        throw new InvalidPackageFormatException(GalleryResources.PublisherLogoSpecificationsException());
      }
label_9:
      logoStream.Seek(0L, SeekOrigin.Begin);
    }

    private void ValidatePublisherApplyingForVerification(string description, ReferenceLinks links)
    {
      if (description.IsNullOrEmpty<char>())
        throw new ArgumentException(GalleryResources.PublisherVerificationMandatoryDescriptionMessage());
      object obj1;
      if (!links.Links.TryGetValue("support", out obj1) || obj1 == null || (obj1 as ReferenceLink).Href.IsNullOrEmpty<char>())
        throw new ArgumentException(GalleryResources.PublisherVerificationMandatorySupportLinkMessage());
      object obj2;
      object obj3;
      object obj4;
      if ((!links.Links.TryGetValue("linkedIn", out obj2) || obj2 == null || (obj2 as ReferenceLink).Href.IsNullOrEmpty<char>()) && (!links.Links.TryGetValue("sourceCode", out obj3) || obj3 == null || (obj3 as ReferenceLink).Href.IsNullOrEmpty<char>()) && (!links.Links.TryGetValue("twitter", out obj4) || obj4 == null || (obj4 as ReferenceLink).Href.IsNullOrEmpty<char>()))
        throw new ArgumentException(GalleryResources.PublisherVerificationMandatoryLinksMessage());
    }

    private string GetContentyTypeForLogoImage(string logoFileName)
    {
      string extension = Path.GetExtension(logoFileName);
      string typeForLogoImage = "";
      if (PublisherService.ImageExtensionToContentTypeMap.TryGetValue(extension, out typeForLogoImage))
        return typeForLogoImage;
      throw new ArgumentException(GalleryResources.UnsupportedFileTypeError());
    }

    private PublisherState ValidateAndGetPublisherState(
      IVssRequestContext requestContext,
      PublisherFlags existingFlags,
      PublisherState existingState,
      PublisherFlags incomingFlags,
      PublisherState incomingState)
    {
      PublisherState publisherState = PublisherState.None | existingState & PublisherState.CertificationRevoked | existingState & PublisherState.VerificationPending;
      if (existingFlags == incomingFlags && existingState == incomingState)
        return existingState;
      if (existingFlags.HasFlag((Enum) PublisherFlags.Certified))
      {
        if (!incomingFlags.HasFlag((Enum) PublisherFlags.Certified) && incomingState.HasFlag((Enum) PublisherState.CertificationRevoked))
        {
          GallerySecurity.CheckRootPermission(requestContext, PublisherPermissions.Admin);
          publisherState = PublisherState.CertificationRevoked;
        }
      }
      else if (!incomingFlags.HasFlag((Enum) PublisherFlags.Certified))
      {
        if (incomingState.HasFlag((Enum) PublisherState.CertificationPending) && incomingState.HasFlag((Enum) PublisherState.CertificationRejected))
          throw new InvalidPublisherStateTransition(GalleryResources.CannotHaveCertificationPendingAndRejectionTogether());
        if (incomingState.HasFlag((Enum) PublisherState.CertificationRejected))
        {
          GallerySecurity.CheckRootPermission(requestContext, PublisherPermissions.Admin);
          publisherState |= PublisherState.CertificationRejected;
        }
        else if (incomingState.HasFlag((Enum) PublisherState.CertificationPending))
          publisherState |= PublisherState.CertificationPending;
        else
          publisherState |= existingState;
      }
      return publisherState;
    }

    public List<PublisherRoleAssignment> UpdatePublisherMembers(
      IVssRequestContext requestContext,
      string publisher,
      IEnumerable<PublisherUserRoleAssignmentRef> userRoles,
      bool limitToCallerIdentityDomain = false)
    {
      int num1 = userRoles.Count<PublisherUserRoleAssignmentRef>();
      if (num1 != 1)
      {
        string message1 = string.Format("Currently adding/updating roles of only one member is supported. Found {0} roles to be modified.", (object) num1);
        string message2 = message1 + " Input: " + string.Join(",", new string[1]
        {
          userRoles.Select<PublisherUserRoleAssignmentRef, string>((Func<PublisherUserRoleAssignmentRef, string>) (x => x.UniqueName)).ToString()
        });
        requestContext.Trace(12062070, TraceLevel.Error, "gallery", nameof (UpdatePublisherMembers), message2);
        throw new InvalidOperationException(message1);
      }
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableAddMembersUsingVsid"))
      {
        Guid userId = userRoles.ElementAt<PublisherUserRoleAssignmentRef>(0).UserId;
        if (userRoles.ElementAt<PublisherUserRoleAssignmentRef>(0).UserId != Guid.Empty)
        {
          ISecurityRoleMappingService service = requestContext.GetService<ISecurityRoleMappingService>();
          IVssRequestContext requestContext1 = requestContext;
          List<UserRoleAssignmentRef> userRoles1 = new List<UserRoleAssignmentRef>();
          userRoles1.Add(new UserRoleAssignmentRef()
          {
            UserId = userRoles.ElementAt<PublisherUserRoleAssignmentRef>(0).UserId,
            RoleName = userRoles.ElementAt<PublisherUserRoleAssignmentRef>(0).RoleName
          });
          string resourceId = publisher;
          int num2 = limitToCallerIdentityDomain ? 1 : 0;
          return service.SetRoleAssignments(requestContext1, userRoles1, resourceId, "gallery.publisher", num2 != 0).Select<RoleAssignment, PublisherRoleAssignment>((Func<RoleAssignment, PublisherRoleAssignment>) (x => this.GetPublisherRoleAssignment(x))).ToList<PublisherRoleAssignment>();
        }
      }
      string property = requestContext.GetUserIdentity().GetProperty<string>("Domain", string.Empty);
      Guid result = Guid.Empty;
      if (!Guid.TryParse(property, out result))
      {
        string message = "Couldn't parse the domain GUID of the current logged in user. Domain string " + property + ".";
        requestContext.Trace(12062070, TraceLevel.Error, "gallery", nameof (UpdatePublisherMembers), message);
        throw new IdentityNotFoundException("Invalid domain.");
      }
      string identifier = result.ToString() + "\\" + userRoles.ElementAt<PublisherUserRoleAssignmentRef>(0).UniqueName;
      IList<IdentityDescriptor> descriptors = (IList<IdentityDescriptor>) new List<IdentityDescriptor>()
      {
        new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", identifier)
      };
      List<Microsoft.VisualStudio.Services.Identity.Identity> list = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, descriptors, QueryMembership.None, (IEnumerable<string>) null).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (list.IsNullOrEmpty<Microsoft.VisualStudio.Services.Identity.Identity>() || list[0] == null)
      {
        string message = "Couldn't find an identity in AzureDevOps for the identifier " + identifier;
        requestContext.Trace(12062070, TraceLevel.Error, "gallery", nameof (UpdatePublisherMembers), message);
        throw new IdentityNotFoundException();
      }
      ISecurityRoleMappingService service1 = requestContext.GetService<ISecurityRoleMappingService>();
      IVssRequestContext requestContext2 = requestContext;
      List<UserRoleAssignmentRef> userRoles2 = new List<UserRoleAssignmentRef>();
      userRoles2.Add(new UserRoleAssignmentRef()
      {
        UserId = list[0].Id,
        RoleName = userRoles.ElementAt<PublisherUserRoleAssignmentRef>(0).RoleName
      });
      string resourceId1 = publisher;
      int num3 = limitToCallerIdentityDomain ? 1 : 0;
      return service1.SetRoleAssignments(requestContext2, userRoles2, resourceId1, "gallery.publisher", num3 != 0).Select<RoleAssignment, PublisherRoleAssignment>((Func<RoleAssignment, PublisherRoleAssignment>) (x => this.GetPublisherRoleAssignment(x))).ToList<PublisherRoleAssignment>();
    }

    private PublisherRoleAssignment GetPublisherRoleAssignment(RoleAssignment roleAssignment)
    {
      if (roleAssignment == null)
        return (PublisherRoleAssignment) null;
      return new PublisherRoleAssignment()
      {
        Access = roleAssignment.Access == RoleAccess.Assigned ? PublisherRoleAccess.Assigned : PublisherRoleAccess.Inherited,
        Identity = roleAssignment.Identity,
        Role = new PublisherSecurityRole(roleAssignment.Role.DisplayName, roleAssignment.Role.Name, roleAssignment.Role.AllowPermissions, roleAssignment.Role.DenyPermissions, roleAssignment.Role.Scope, roleAssignment.Role.Description)
      };
    }

    internal void DeletePublishersByUserVSIDs(
      IVssRequestContext requestContext,
      IReadOnlyCollection<Guid> userVsids)
    {
      if (userVsids.IsNullOrEmpty<Guid>())
        return;
      List<QueryFilter> queryFilterList = new List<QueryFilter>();
      string layer = nameof (PublisherService);
      string methodName = "DeletePublishersByVSID";
      requestContext.TraceEnter(12062076, "gallery", layer, methodName);
      userVsids.ForEach<Guid>((Action<Guid>) (vsid => queryFilterList.Add(new QueryFilter()
      {
        Criteria = new List<FilterCriteria>(1)
        {
          new FilterCriteria()
          {
            FilterType = 3,
            Value = vsid.ToString()
          }
        }
      })));
      PublisherQueryResult publisherQueryResult = this.QueryPublishers(requestContext, new PublisherQuery()
      {
        Filters = queryFilterList,
        Flags = PublisherQueryFlags.None
      }, true);
      if (publisherQueryResult != null)
      {
        if (publisherQueryResult.Results == null || publisherQueryResult.Results.Count != userVsids.Count)
        {
          requestContext.Trace(12062076, TraceLevel.Warning, "gallery", layer, string.Format("Query results count ({0}) does not equal VSID count ({1})", (object) (publisherQueryResult.Results == null ? 0 : publisherQueryResult.Results.Count), (object) userVsids.Count));
          return;
        }
        if (publisherQueryResult.Results.Any<PublisherFilterResult>())
        {
          for (int index = 0; index < userVsids.Count; ++index)
          {
            List<Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher> publishers = publisherQueryResult.Results[index].Publishers;
            if (!publishers.IsNullOrEmpty<Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher>())
            {
              foreach (Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher in publishers)
              {
                try
                {
                  this.DeletePublisher(requestContext, publisher.PublisherName);
                  requestContext.Trace(12062076, TraceLevel.Info, "gallery", layer, "Publisher " + publisher.PublisherName + " got deleted successfully");
                }
                catch (Exception ex)
                {
                  requestContext.Trace(12062076, TraceLevel.Info, "gallery", layer, "Exception encountered while deleting publisher " + publisher.PublisherName + ".");
                  requestContext.TraceException(12062076, "gallery", layer, ex);
                }
              }
            }
          }
        }
      }
      requestContext.TraceLeave(12062076, "gallery", layer, methodName);
    }

    private void PublishCustomerIntelligenceEventAndAuditLogForPublisherCertification(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher oldPublisher,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher modifiedPublisher)
    {
      if (oldPublisher.Flags == modifiedPublisher.Flags && oldPublisher.State == modifiedPublisher.State || !modifiedPublisher.Flags.HasFlag((Enum) PublisherFlags.Certified) && !modifiedPublisher.State.HasFlag((Enum) PublisherState.CertificationPending) && !modifiedPublisher.State.HasFlag((Enum) PublisherState.CertificationRejected) && !modifiedPublisher.State.HasFlag((Enum) PublisherState.CertificationRevoked))
        return;
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("PublisherName", oldPublisher.PublisherName);
      intelligenceData.Add("PreviousPublisherFlags", (object) oldPublisher.Flags);
      intelligenceData.Add("CurrentPublisherFlags", (object) modifiedPublisher.Flags);
      intelligenceData.Add("PreviousPublisherState", (object) oldPublisher.State);
      intelligenceData.Add("CurrentPublisherState", (object) modifiedPublisher.State);
      intelligenceData.Add("PreviousPublisherStatus", oldPublisher.Flags.ToString() + " | " + oldPublisher.State.ToString());
      intelligenceData.Add("CurrentPublisherStatus", modifiedPublisher.Flags.ToString() + " | " + modifiedPublisher.State.ToString());
      intelligenceData.AddGalleryUserIdentifier(requestContext);
      if (oldPublisher.Flags.HasFlag((Enum) PublisherFlags.Certified) && modifiedPublisher.State.HasFlag((Enum) PublisherState.CertificationRevoked))
        requestContext.GetService<IGalleryAuditLogService>().LogAuditEntry(requestContext, "CertificationRevoked", oldPublisher.PublisherName.ToString((IFormatProvider) CultureInfo.InvariantCulture), "Publisher");
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "TopPublisher", intelligenceData);
    }

    public string GetVSIDByPublisherName(IVssRequestContext requestContext, string publisherName)
    {
      if (string.IsNullOrWhiteSpace(publisherName))
        return string.Empty;
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
        return component.GetVSIDByPublisherName(publisherName);
    }

    public Dictionary<Guid, string> GetPublisherIds(
      IVssRequestContext requestContext,
      List<string> publisherName)
    {
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
        return component.GetPublisherIds(publisherName);
    }

    public void InsertSpamPublishers(
      IVssRequestContext requestContext,
      Dictionary<Guid, string> publishers)
    {
      using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
        component.InsertSpamPublishers(publishers);
    }

    private void SetDomainRelatedProperties(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher existingPublisher,
      bool callFromLR,
      ref string domain,
      ref bool isDomainVerified,
      ref bool removeDomain,
      ref Guid token)
    {
      if (!callFromLR && (existingPublisher.Domain != domain || !existingPublisher.IsDomainVerified))
        isDomainVerified = false;
      if (domain != null)
        domain = domain.Trim();
      if (domain != null && domain == existingPublisher.Domain && isDomainVerified == existingPublisher.IsDomainVerified)
      {
        domain = (string) null;
      }
      else
      {
        if (existingPublisher.Domain == null)
          return;
        if (domain.IsNullOrEmpty<char>())
        {
          removeDomain = true;
        }
        else
        {
          if (!(domain == existingPublisher.Domain))
            return;
          using (PublisherComponent component = requestContext.CreateComponent<PublisherComponent>())
          {
            if (!(component is PublisherComponent8))
              return;
            PublisherComponent8 publisherComponent8 = component as PublisherComponent8;
            token = publisherComponent8.FetchDomainToken(existingPublisher.PublisherName);
          }
        }
      }
    }
  }
}
