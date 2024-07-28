// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.GraphProfile.Server.GraphProfileMemberAvatarsController
// Assembly: Microsoft.TeamFoundation.GraphProfile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E08BE30A-4AE3-40A5-BE5B-3FCDC59E061E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.GraphProfile.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.GraphProfile.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.IdentityImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.TeamFoundation.GraphProfile.Server
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "GraphProfile", ResourceName = "MemberAvatars")]
  public class GraphProfileMemberAvatarsController : TfsApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (GraphMemberAvatarResponse), null, null)]
    [ClientResponseType(typeof (byte[]), "GetMemberAvatarImageData", "image/*")]
    [ClientHeaderParameter("If-None-Match", typeof (string), "ifNoneMatch", "the list of etags to compare against to receive a 304 Not Modified instead of receiving the full data for more efficient cache updates", true, false)]
    [TraceFilter(6304871, 6304879)]
    [PublicCollectionRequestRestrictions(false, true, null)]
    public HttpResponseMessage GetMemberAvatar(
      string memberDescriptor,
      GraphMemberAvatarSize size = GraphMemberAvatarSize.Medium,
      string stamp = null,
      string overrideDisplayName = null,
      bool generateDefaultMemberAvatar = false)
    {
      this.TfsRequestContext.CheckPermissionToReadPublicIdentityInfo();
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
      Microsoft.VisualStudio.Services.Identity.Identity identity = GraphProfileMemberAvatarsController.GetIdentity(memberDescriptor, vssRequestContext, this.TfsRequestContext.GetUserIdentity());
      if (identity == null)
        return new HttpResponseMessage(HttpStatusCode.NotFound);
      IdentityImageService identityImageService = IdentityImageServiceUtil.GetIdentityImageService(vssRequestContext);
      Guid identityImageId = identityImageService.GetIdentityImageId(vssRequestContext, identity.Id, out bool _);
      string etag = "\"" + (object) identityImageId + "\"";
      if (identityImageId == Guid.Empty | generateDefaultMemberAvatar)
        etag = "\"" + (object) (overrideDisplayName ?? identity.DisplayName).GetStableHashCode() + "\"";
      if (this.RequestedEtagsContain(etag))
      {
        HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.NotModified);
        this.AddCacheAndEtagHeadersToResponse(response, etag, stamp != null);
        return response;
      }
      byte[] imageData = (byte[]) null;
      string contentType = (string) null;
      ImageSize serverImageSize = this.ConvertToServerImageSize(size);
      if (!generateDefaultMemberAvatar)
        identityImageService.GetIdentityImageData(vssRequestContext, identity, false, out imageData, out contentType, new ImageSize?(serverImageSize), overrideDisplayName != null);
      if (imageData == null)
      {
        imageData = AvatarUtils.GenerateAvatar(overrideDisplayName ?? identity.DisplayName, serverImageSize, AvatarImageFormat.Png, identity.IsContainer);
        contentType = "image/png";
      }
      return this.GenerateResponse(GraphProfileResultExtensions.CreateGraphMemberAvatar(this.TfsRequestContext, identity.SubjectDescriptor, identity.Id, imageData, contentType, size), etag, stamp != null);
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      string memberDescriptor,
      IVssRequestContext elevatedRequestContext,
      Microsoft.VisualStudio.Services.Identity.Identity requestIdentity)
    {
      IdentityService service = elevatedRequestContext.GetService<IdentityService>();
      string[] strArray;
      if (!elevatedRequestContext.ExecutionEnvironment.IsHostedDeployment)
        strArray = new string[3]
        {
          "Microsoft.TeamFoundation.Identity.Image.Id",
          "Microsoft.TeamFoundation.Identity.Image.Data",
          "Microsoft.TeamFoundation.Identity.Image.Type"
        };
      else
        strArray = (string[]) null;
      string[] propertyNameFilters = strArray;
      Guid result;
      if (Guid.TryParse(memberDescriptor, out result))
      {
        if (result == requestIdentity.Id && elevatedRequestContext.ExecutionEnvironment.IsHostedDeployment)
          return requestIdentity;
        return service.ReadIdentities(elevatedRequestContext, (IList<Guid>) new Guid[1]
        {
          result
        }, QueryMembership.None, (IEnumerable<string>) propertyNameFilters).Single<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      SubjectDescriptor subjectDescriptor = SubjectDescriptor.FromString(memberDescriptor);
      if (!(subjectDescriptor != new SubjectDescriptor()) || subjectDescriptor.IsUnknownSubjectType())
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (subjectDescriptor == requestIdentity.SubjectDescriptor && elevatedRequestContext.ExecutionEnvironment.IsHostedDeployment)
        return requestIdentity;
      return service.ReadIdentities(elevatedRequestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
      {
        subjectDescriptor
      }, QueryMembership.None, (IEnumerable<string>) propertyNameFilters).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
    }

    private bool RequestedEtagsContain(string etag)
    {
      HttpHeaderValueCollection<EntityTagHeaderValue> ifNoneMatch = this.Request.Headers.IfNoneMatch;
      return ifNoneMatch != null && ifNoneMatch.Any<EntityTagHeaderValue>((Func<EntityTagHeaderValue, bool>) (requestedEtag => requestedEtag.Tag.Equals(etag)));
    }

    private ImageSize ConvertToServerImageSize(GraphMemberAvatarSize size)
    {
      switch (size)
      {
        case GraphMemberAvatarSize.Small:
          return ImageSize.Small;
        case GraphMemberAvatarSize.Medium:
          return ImageSize.Medium;
        case GraphMemberAvatarSize.Large:
          return ImageSize.Large;
        default:
          throw new ArgumentException(string.Format("Invalid avatar size: {0}", (object) size), nameof (size));
      }
    }

    private HttpResponseMessage GenerateResponse(
      GraphMemberAvatar avatar,
      string etag,
      bool hasCacheBustingStamp)
    {
      HttpResponseMessage responseWithContentOnly = this.GenerateResponseWithContentOnly(avatar);
      this.AddCacheAndEtagHeadersToResponse(responseWithContentOnly, etag, hasCacheBustingStamp);
      return responseWithContentOnly;
    }

    private void AddCacheAndEtagHeadersToResponse(
      HttpResponseMessage response,
      string etag,
      bool hasCacheBustingStamp)
    {
      response.Headers.ETag = EntityTagHeaderValue.Parse(etag);
      TimeSpan timeSpan = hasCacheBustingStamp ? TimeSpan.FromHours(8760.0) : TimeSpan.FromHours(48.0);
      response.Headers.CacheControl = new CacheControlHeaderValue()
      {
        MaxAge = new TimeSpan?(timeSpan),
        MustRevalidate = true,
        Public = true
      };
    }

    private HttpResponseMessage GenerateResponseWithContentOnly(GraphMemberAvatar avatar)
    {
      HttpContent responseContent = GetResponseContent();
      if (responseContent == null)
        return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
      return new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = responseContent
      };

      HttpContent GetResponseContent()
      {
        HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> accept = this.Request.Headers.Accept;
        if (accept.Count == 0)
          return ByteArrayContent();
        foreach (MediaTypeWithQualityHeaderValue qualityHeaderValue in accept)
        {
          if (qualityHeaderValue.MediaType == avatar.ImageType || qualityHeaderValue.MediaType == "image/*" || qualityHeaderValue.MediaType == "*/*")
            return ByteArrayContent();
          if (qualityHeaderValue.MediaType == "application/json")
            return JsonContent();
        }
        return (HttpContent) null;
      }

      HttpContent ByteArrayContent()
      {
        VssServerByteArrayContent byteArrayContent = new VssServerByteArrayContent(avatar.ImageData, (object) avatar);
        byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue(avatar.ImageType);
        return (HttpContent) byteArrayContent;
      }

      HttpContent JsonContent()
      {
        ObjectContent<GraphMemberAvatar> objectContent = new ObjectContent<GraphMemberAvatar>(avatar, (System.Net.Http.Formatting.MediaTypeFormatter) GraphProfileServerConstants.MediaTypeFormatter.Json);
        objectContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return (HttpContent) objectContent;
      }
    }

    public override string TraceArea => "GraphProfile";

    public override string ActivityLogArea => "Graph Profile";
  }
}
