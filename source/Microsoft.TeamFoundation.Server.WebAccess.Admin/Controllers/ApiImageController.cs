// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.Controllers.ApiImageController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.IdentityImage;
using Microsoft.VisualStudio.Services.Profile;
using Microsoft.VisualStudio.Services.Users.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.Controllers
{
  [SupportedRouteArea("Api", NavigationContextLevels.ApplicationAll)]
  [Microsoft.TeamFoundation.Server.WebAccess.DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class ApiImageController : AdminAreaController
  {
    private const string c_ProfileImageServiceUpdateFeatureFlag = "WebAccess.Profile.ProfileImageService";
    private const int WebAccessExceptionEaten = 599999;
    private IVssRequestContext m_tfsRequestContext;

    [HttpGet]
    [TfsTraceFilter(500640, 500650)]
    public ActionResult ChangeImage(Guid id, bool isOrganizationLevel = false)
    {
      if (ApiImageController.ShouldElevateToOrganization(isOrganizationLevel, this.TfsRequestContext))
        this.TfsRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      IdentityViewModelBase identityViewModel = IdentityImageUtility.GetIdentityViewModel(this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(this.TfsRequestContext, new Guid[1]
      {
        id
      }, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null)[0]);
      this.ViewData["IsOrganizationLevel"] = (object) isOrganizationLevel;
      return (ActionResult) this.View((object) identityViewModel);
    }

    [HttpPost]
    [TfsTraceFilter(500100, 500110)]
    public ActionResult UploadImage(Guid tfid, bool isOrganizationLevel = false)
    {
      if (ApiImageController.ShouldElevateToOrganization(isOrganizationLevel, this.TfsRequestContext))
        this.TfsRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      try
      {
        this.CheckChangeImagePermissions(tfid);
        IdentityService service1 = this.TfsRequestContext.GetService<IdentityService>();
        Microsoft.VisualStudio.Services.Identity.Identity readIdentity = service1.ReadIdentities(this.TfsRequestContext, (IList<Guid>) new Guid[1]
        {
          tfid
        }, QueryMembership.None, (IEnumerable<string>) null)[0];
        if (readIdentity == null)
          throw new IdentityNotFoundException(tfid);
        if (this.UseProfileImageService)
          ProfileImageUtility.RemoveCandidateImage(this.TfsRequestContext, readIdentity);
        else
          IdentityImageUtility.RemoveCandidateImage(this.TfsRequestContext, readIdentity);
        SubjectDescriptor subjectDescriptor = readIdentity.Descriptor.ToSubjectDescriptor(this.TfsRequestContext);
        HttpPostedFileBase file = this.Request.Files["idImage"];
        bool flag = false;
        if (file != null && file.ContentLength > 0 && file.ContentLength < 2621440)
          flag = true;
        this.TfsRequestContext.TraceAlways(916621846, TraceLevel.Info, nameof (ApiImageController), nameof (UploadImage), "{{ \"identityType\": \"{0}\", \"size\": {1}, \"accepted\": {2} }}", (object) subjectDescriptor.SubjectType, (object) (file != null ? file.ContentLength : 0), (object) flag);
        byte[] image = IdentityImageUtility.ParseImage(file);
        if (this.UseProfileImageService && !readIdentity.IsContainer)
        {
          IUserService service2 = this.TfsRequestContext.GetService<IUserService>();
          byte[] inArray = (byte[]) null;
          using (MemoryStream imageDataToResize = new MemoryStream(image))
            inArray = ImageResizeUtils.ResizeWhileMaintainingAspectRatio((Stream) imageDataToResize, 220, AvatarImageFormat.Png);
          service2.SetAttribute(this.TfsRequestContext, readIdentity.Id, WellKnownUserAttributeNames.AvatarPreview, Convert.ToBase64String(inArray));
        }
        else
        {
          readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.CandidateImage.Data", (object) image);
          readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.CandidateImage.UploadDate", (object) DateTime.UtcNow);
          service1.UpdateIdentities(this.TfsRequestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
          {
            readIdentity
          });
        }
        IdentityImageService.InvalidateIdentityImage(this.TfsRequestContext, readIdentity);
        return (ActionResult) this.View((object) new ImageModel()
        {
          TeamFoundationId = tfid
        });
      }
      catch (Exception ex)
      {
        this.TraceException(599999, ex);
        return (ActionResult) this.View((object) new ImageModel()
        {
          ErrorMessage = (ex is ProfileServiceUnavailableException ? AdminServerResources.ProfileImageSaveError : ex.Message)
        });
      }
    }

    [HttpPost]
    [TfsTraceFilter(500040, 500050)]
    public ActionResult CancelImageUpload(Guid tfid, bool isOrganizationLevel = false)
    {
      if (ApiImageController.ShouldElevateToOrganization(isOrganizationLevel, this.TfsRequestContext))
        this.TfsRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      this.CheckChangeImagePermissions(tfid);
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = this.TfsRequestContext.GetService<IdentityService>().ReadIdentities(this.TfsRequestContext, (IList<Guid>) new Guid[1]
      {
        tfid
      }, QueryMembership.None, (IEnumerable<string>) null)[0];
      if (readIdentity == null)
        throw new IdentityNotFoundException(tfid);
      if (this.UseProfileImageService)
        ProfileImageUtility.RemoveCandidateImage(this.TfsRequestContext, readIdentity);
      else
        IdentityImageUtility.RemoveCandidateImage(this.TfsRequestContext, readIdentity);
      return (ActionResult) new EmptyResult();
    }

    [HttpPost]
    [TfsTraceFilter(500060, 500070)]
    public ActionResult CommitCandidateImage(Guid tfid, bool isOrganizationLevel = false)
    {
      if (ApiImageController.ShouldElevateToOrganization(isOrganizationLevel, this.TfsRequestContext))
        this.TfsRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      this.CheckChangeImagePermissions(tfid);
      if (this.UseProfileImageService)
        ProfileImageUtility.CommitCandidateImage(this.TfsRequestContext, tfid);
      else
        IdentityImageUtility.CommitCandidateImage(this.TfsRequestContext, tfid);
      return (ActionResult) new EmptyResult();
    }

    [HttpPost]
    [TfsTraceFilter(500080, 500090)]
    public ActionResult RemoveImage(Guid tfid, bool isOrganizationLevel = false)
    {
      if (ApiImageController.ShouldElevateToOrganization(isOrganizationLevel, this.TfsRequestContext))
        this.TfsRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      this.CheckChangeImagePermissions(tfid);
      if (this.UseProfileImageService)
        ProfileImageUtility.RemoveImage(this.TfsRequestContext, tfid);
      else
        IdentityImageUtility.RemoveImage(this.TfsRequestContext, tfid);
      return (ActionResult) new EmptyResult();
    }

    private void CheckChangeImagePermissions(Guid tfid) => TeamsUtility.CheckChangeImagePermissions(tfid, this.TfsRequestContext);

    private bool UseProfileImageService => this.TfsRequestContext.IsHosted() && this.TfsRequestContext.IsFeatureEnabled("WebAccess.Profile.ProfileImageService");

    public new virtual IVssRequestContext TfsRequestContext
    {
      get
      {
        if (this.m_tfsRequestContext == null)
          this.m_tfsRequestContext = base.TfsRequestContext;
        return this.m_tfsRequestContext;
      }
      set => this.m_tfsRequestContext = value;
    }

    private static bool ShouldElevateToOrganization(
      bool isOrganizationLevel,
      IVssRequestContext requestContext)
    {
      return isOrganizationLevel && requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.IsFeatureEnabled("VisualStudio.Services.Web.OrgAdmin.UserExperience");
    }
  }
}
