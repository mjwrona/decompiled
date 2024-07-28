// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IPublisherService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [DefaultServiceImplementation(typeof (PublisherService))]
  public interface IPublisherService : IVssFrameworkService
  {
    Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher CreatePublisher(
      IVssRequestContext requestContext,
      string publisherName,
      string displayName,
      PublisherFlags flags,
      string shortDescription,
      string longDescription);

    Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher CreatePublisher(
      IVssRequestContext requestContext,
      string publisherName,
      string displayName,
      PublisherFlags flags,
      string shortDescription,
      string longDescription,
      ReferenceLinks links,
      PublisherState state,
      string domain = null,
      string reCaptchaToken = null);

    void DeletePublisher(IVssRequestContext requestContext, string publisherName);

    Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher QueryPublisher(
      IVssRequestContext requestContext,
      string publisherName,
      PublisherQueryFlags flags,
      bool allowAnonymousAccess = false);

    Dictionary<Guid, string> GetPublisherIds(
      IVssRequestContext requestContext,
      List<string> publisherNames);

    string GetVSIDByPublisherName(IVssRequestContext requestContext, string publisherName);

    AzurePublisher QueryAssociatedAzurePublisher(
      IVssRequestContext requestContext,
      string publisherName);

    void InsertSpamPublishers(
      IVssRequestContext requestContext,
      Dictionary<Guid, string> publishers);

    AzurePublisher AssociateAzurePublisher(
      IVssRequestContext requestContext,
      AzurePublisher publisher);

    PublisherQueryResult QueryPublishers(
      IVssRequestContext requestContext,
      PublisherQuery publisherQuery);

    PublisherQueryResult QueryPublishers(
      IVssRequestContext requestContext,
      PublisherQuery publisherQuery,
      bool queryForAllIdentities);

    Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher UpdatePublisher(
      IVssRequestContext requestContext,
      string publisherName,
      string displayName,
      PublisherFlags flags,
      string shortDescription,
      string longDescription,
      string reCaptchaToken = null);

    Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher UpdatePublisher(
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
      bool isVerifiedManually = false,
      string reCaptchaToken = null);

    Stream GetPublisherAsset(
      IVssRequestContext requestContext,
      string publisherName,
      string assetType);

    int UpdatePublisherAsset(
      IVssRequestContext requestContext,
      string publisherName,
      string logoFileName,
      Stream logoImageStream,
      string assetType);

    void DeletePublisherAsset(
      IVssRequestContext requestContext,
      string publisherName,
      string assetType);

    void ManagePublisherVisibility(
      IVssRequestContext requestContext,
      string identifier,
      List<Guid> identityIds,
      bool removeTags);

    ICollection<Microsoft.VisualStudio.Services.Identity.Identity> GetOwnerIdentitiesOfPublisher(
      IVssRequestContext requestContext,
      string publisherName);

    ICollection<Microsoft.VisualStudio.Services.Identity.Identity> GetContributorIdentitiesOfPublisherOrExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName);

    ICollection<Microsoft.VisualStudio.Services.Identity.Identity> GetCreatorIdentitiesOfPublisherOrExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName);

    string GetReservedPublisherDisplayName();

    void LinkPendingProfile(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity publisherIdentity);

    List<PublisherRoleAssignment> UpdatePublisherMembers(
      IVssRequestContext requestContext,
      string publisher,
      IEnumerable<PublisherUserRoleAssignmentRef> userRoles,
      bool limitToCallerIdentityDomain = false);

    void PublishPublisherUpdatedEvent(IVssRequestContext requestContext, string publisherName);
  }
}
