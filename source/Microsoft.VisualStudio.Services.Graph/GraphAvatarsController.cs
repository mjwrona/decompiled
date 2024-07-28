// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphAvatarsController
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Profile;
using Microsoft.VisualStudio.Services.Users.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Graph
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "Graph", ResourceName = "Avatars")]
  public class GraphAvatarsController : GraphControllerBase
  {
    private static readonly HashSet<string> c_GroupProperties = new HashSet<string>()
    {
      "Microsoft.TeamFoundation.Identity.Image.Data",
      "Microsoft.TeamFoundation.Identity.Image.Id",
      "Microsoft.TeamFoundation.Identity.Image.Type"
    };

    [HttpGet]
    [TraceFilter(630800, 630809)]
    [ClientResponseType(typeof (Microsoft.VisualStudio.Services.Profile.Avatar), null, null)]
    public HttpResponseMessage GetAvatar(
      [ClientParameterType(typeof (string), false)] SubjectDescriptor subjectDescriptor,
      Microsoft.VisualStudio.Services.Profile.AvatarSize size = Microsoft.VisualStudio.Services.Profile.AvatarSize.Medium,
      string format = null)
    {
      this.CheckPermissions(this.TfsRequestContext, GraphSecurityConstants.SubjectsToken, 1);
      if (subjectDescriptor.IsUserType())
        return this.GetUserAvatar(subjectDescriptor, size, format);
      if (!subjectDescriptor.IsGroupType())
        return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Resources.GraphAvatarUnsupportedSubject());
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      return this.GetGroupAvatar(subjectDescriptor, size, format);
    }

    [HttpPut]
    [TraceFilter(630810, 630819)]
    [ClientResponseType(typeof (void), null, null)]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    public HttpResponseMessage SetAvatar([ClientParameterType(typeof (string), false)] SubjectDescriptor subjectDescriptor, [FromBody] Microsoft.VisualStudio.Services.Profile.Avatar avatar)
    {
      bool flag = false;
      if (avatar != null && avatar.Value != null && avatar.Value.Length != 0 && avatar.Value.Length < 2621440)
        flag = true;
      this.TfsRequestContext.TraceAlways(630811, TraceLevel.Info, "Graph", "Avatars", "{{ \"identityType\": \"{0}\", \"size\": {1}, \"accepted\": {2} }}", (object) subjectDescriptor.SubjectType, (object) avatar?.Value?.Length.GetValueOrDefault(), (object) flag);
      if (avatar.Value.Length > 2621440)
        return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Resources.GraphAvatarTooBig());
      if (subjectDescriptor.IsClaimsUserType())
      {
        this.TfsRequestContext.GetService<IUserService>().UpdateAvatar(this.TfsRequestContext, subjectDescriptor, new Microsoft.VisualStudio.Services.Users.Avatar()
        {
          Image = avatar.Value
        });
        return this.Request.CreateResponse(HttpStatusCode.NoContent);
      }
      if (!subjectDescriptor.IsGroupType())
        return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Resources.GraphAvatarUnsupportedSubject());
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      PlatformIdentityService service = this.TfsRequestContext.GetService<PlatformIdentityService>();
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      List<SubjectDescriptor> subjectDescriptors = new List<SubjectDescriptor>();
      subjectDescriptors.Add(subjectDescriptor);
      HashSet<string> cGroupProperties = GraphAvatarsController.c_GroupProperties;
      Microsoft.VisualStudio.Services.Identity.Identity identity = service.ReadIdentities(tfsRequestContext, (IList<SubjectDescriptor>) subjectDescriptors, QueryMembership.None, (IEnumerable<string>) cGroupProperties, false).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      identity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Data", (object) avatar.Value);
      identity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Id", (object) Guid.NewGuid().ToByteArray());
      identity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Type", (object) "image/png");
      service.UpdateIdentities(this.TfsRequestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        identity
      });
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    [HttpDelete]
    [TraceFilter(630820, 630829)]
    [ClientResponseType(typeof (void), null, null)]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    public HttpResponseMessage DeleteAvatar([ClientParameterType(typeof (string), false)] SubjectDescriptor subjectDescriptor)
    {
      if (subjectDescriptor.IsClaimsUserType())
        this.TfsRequestContext.GetService<IUserService>().DeleteAvatar(this.TfsRequestContext, subjectDescriptor);
      else if (subjectDescriptor.IsGroupType())
      {
        this.TfsRequestContext.CheckProjectCollectionRequestContext();
        PlatformIdentityService service = this.TfsRequestContext.GetService<PlatformIdentityService>();
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        List<SubjectDescriptor> subjectDescriptors = new List<SubjectDescriptor>();
        subjectDescriptors.Add(subjectDescriptor);
        HashSet<string> cGroupProperties = GraphAvatarsController.c_GroupProperties;
        Microsoft.VisualStudio.Services.Identity.Identity identity = service.ReadIdentities(tfsRequestContext, (IList<SubjectDescriptor>) subjectDescriptors, QueryMembership.None, (IEnumerable<string>) cGroupProperties, false).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        identity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Data", (object) null);
        identity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Id", (object) null);
        identity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Type", (object) null);
        service.UpdateIdentities(this.TfsRequestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          identity
        });
      }
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    private HttpResponseMessage GetGroupAvatar(
      SubjectDescriptor subjectDescriptor,
      Microsoft.VisualStudio.Services.Profile.AvatarSize size,
      string format)
    {
      PlatformIdentityService service = this.TfsRequestContext.GetService<PlatformIdentityService>();
      PlatformIdentityService platformIdentityService = service;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      List<SubjectDescriptor> subjectDescriptors = new List<SubjectDescriptor>();
      subjectDescriptors.Add(subjectDescriptor);
      HashSet<string> cGroupProperties = GraphAvatarsController.c_GroupProperties;
      Microsoft.VisualStudio.Services.Identity.Identity identity = platformIdentityService.ReadIdentities(tfsRequestContext, (IList<SubjectDescriptor>) subjectDescriptors, QueryMembership.None, (IEnumerable<string>) cGroupProperties, false).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null)
        return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, Resources.GraphSubjectForDescriptorNotFound((object) subjectDescriptor));
      byte[] buffer = (byte[]) null;
      Guid result = Guid.Empty;
      bool flag = false;
      object b;
      object obj;
      if (identity.TryGetProperty("Microsoft.TeamFoundation.Identity.Image.Id", out b) && identity.TryGetProperty("Microsoft.TeamFoundation.Identity.Image.Data", out obj))
      {
        if (Guid.TryParse(b.ToString(), out result))
        {
          this.TfsRequestContext.TraceAlways(858498132, TraceLevel.Info, this.TraceArea, "GraphGroupsController", "Fixing bad Avatar ImageId format");
          identity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Id", (object) result.ToByteArray());
          service.UpdateIdentities(this.TfsRequestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
          {
            identity
          });
        }
        else
          result = new Guid((byte[]) b);
        buffer = (byte[]) obj;
        int avatarSizeInPixels = ProfileAvatarUtils.MapToAvatarSizeInPixels(size);
        using (MemoryStream imageDataToResize = new MemoryStream(buffer))
          buffer = ImageResizeUtils.ResizeWhileMaintainingAspectRatio((Stream) imageDataToResize, avatarSizeInPixels, AvatarImageFormat.Png);
      }
      if (result == Guid.Empty && this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        buffer = AvatarUtils.GenerateAvatar(identity.DisplayName, (ImageSize) size, AvatarImageFormat.Png, true);
        flag = true;
      }
      HttpResponseMessage groupAvatar = AvatarUtils.FormatResponse(this.Request, new Microsoft.VisualStudio.Services.Profile.Avatar()
      {
        IsAutoGenerated = new bool?(flag),
        Size = size,
        Value = buffer,
        TimeStamp = DateTimeOffset.MinValue
      }, format);
      if (result != Guid.Empty)
        groupAvatar.Headers.ETag = AvatarUtils.GetETag(result);
      return groupAvatar;
    }

    private HttpResponseMessage GetUserAvatar(
      SubjectDescriptor subjectDescriptor,
      Microsoft.VisualStudio.Services.Profile.AvatarSize size,
      string format)
    {
      Microsoft.VisualStudio.Services.Profile.Avatar profileAvatar = ProfileCompatHelper.ConvertToProfileAvatar(this.TfsRequestContext.GetService<IUserService>().GetAvatar(this.TfsRequestContext, subjectDescriptor, (Microsoft.VisualStudio.Services.Users.AvatarSize) size));
      if (profileAvatar == null)
        return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, Resources.GraphAvatarNotFound((object) subjectDescriptor));
      return AvatarUtils.IsResourceNotRequiredInResponse(this.Request, AvatarUtils.GetETag(profileAvatar.TimeStamp), profileAvatar.TimeStamp) ? this.Request.CreateResponse(HttpStatusCode.NotModified) : AvatarUtils.FormatResponse(this.Request, profileAvatar, format);
    }
  }
}
