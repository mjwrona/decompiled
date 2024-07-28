// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.PublisherController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "publishers")]
  public class PublisherController : GalleryController
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<PublisherVerifiedNameChangeException>(HttpStatusCode.BadRequest);
    }

    [HttpGet]
    [ClientLocationId("4DDEC66A-E4F6-4F5D-999E-9E77710D7FF4")]
    public Publisher GetPublisher(string publisherName, int flags = 0) => this.TfsRequestContext.GetService<IPublisherService>().QueryPublisher(this.TfsRequestContext, publisherName, (PublisherQueryFlags) flags);

    [HttpPost]
    [ClientResponseType(typeof (Publisher), null, null)]
    [ClientLocationId("4DDEC66A-E4F6-4F5D-999E-9E77710D7FF4")]
    public HttpResponseMessage CreatePublisher(Publisher publisher)
    {
      ArgumentUtility.CheckForNull<Publisher>(publisher, nameof (publisher));
      if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DisablePublisherCreation"))
        throw new HttpException(400, GalleryResources.MaintenanceMessage());
      return this.Request.CreateResponse<Publisher>(HttpStatusCode.Created, this.TfsRequestContext.GetService<IPublisherService>().CreatePublisher(this.TfsRequestContext, publisher.PublisherName, publisher.DisplayName, publisher.Flags, publisher.ShortDescription, publisher.LongDescription, publisher.Links, publisher.State, publisher.Domain, publisher.ReCaptchaToken));
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("4DDEC66A-E4F6-4F5D-999E-9E77710D7FF4")]
    public HttpResponseMessage DeletePublisher(string publisherName)
    {
      this.TfsRequestContext.GetService<IPublisherService>().DeletePublisher(this.TfsRequestContext, publisherName);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    [HttpPut]
    [ClientLocationId("4DDEC66A-E4F6-4F5D-999E-9E77710D7FF4")]
    public Publisher UpdatePublisher(string publisherName, Publisher publisher)
    {
      ArgumentUtility.CheckForNull<Publisher>(publisher, nameof (publisher));
      if (!string.Equals(publisherName, publisher.PublisherName))
        throw new ArgumentException(GalleryResources.PathMustMatchPublisher((object) publisherName, (object) publisher.PublisherName));
      return this.TfsRequestContext.GetService<IPublisherService>().UpdatePublisher(this.TfsRequestContext, publisherName, publisher.DisplayName, publisher.Flags, publisher.ShortDescription, publisher.LongDescription, new int?(), publisher.Links, publisher.State, publisher.Domain, publisher.IsDomainVerified, reCaptchaToken: publisher.ReCaptchaToken);
    }

    [HttpPost]
    [ClientLocationId("4DDEC66A-E4F6-4F5D-999E-9E77710D7FF4")]
    public IEnumerable<PublisherRoleAssignment> UpdatePublisherMembers(
      string publisherName,
      [FromBody] IEnumerable<PublisherUserRoleAssignmentRef> roleAssignments,
      bool limitToCallerIdentityDomain = false)
    {
      return (IEnumerable<PublisherRoleAssignment>) this.TfsRequestContext.GetService<IPublisherService>().UpdatePublisherMembers(this.TfsRequestContext, publisherName, roleAssignments, limitToCallerIdentityDomain);
    }
  }
}
