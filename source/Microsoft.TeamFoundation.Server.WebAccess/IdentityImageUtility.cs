// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.IdentityImageUtility
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.IdentityImage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class IdentityImageUtility
  {
    public const string IdentityImageSVGMimeType = "image/svg+xml";
    private const int c_maxImageDimension = 144;

    public static T GetIdentityViewModel<T>(TeamFoundationIdentity identity, bool isTeam = false) where T : IdentityViewModelBase => IdentityImageUtility.GetIdentityViewModel(identity, isTeam: isTeam) as T;

    public static IdentityViewModelBase GetIdentityViewModel(
      TeamFoundationIdentity identity,
      bool loadAadTenantData = true,
      bool isTeam = false)
    {
      ArgumentUtility.CheckForNull<TeamFoundationIdentity>(identity, nameof (identity));
      isTeam = isTeam ? isTeam : identity.TryGetProperty(TeamConstants.TeamPropertyName, out object _);
      if (isTeam)
        return (IdentityViewModelBase) new TeamIdentityViewModel(identity);
      return identity.IsContainer ? (IdentityViewModelBase) new GroupIdentityViewModel(identity) : (IdentityViewModelBase) new UserIdentityViewModel(identity, loadAadTenantData);
    }

    public static void RemoveImage(IVssRequestContext requestContext, Guid tfid)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = service.ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        tfid
      }, QueryMembership.None, (IEnumerable<string>) null)[0];
      if (readIdentity == null)
        throw new IdentityNotFoundException(tfid);
      readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Data", (object) null);
      readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Id", (object) null);
      readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Type", (object) null);
      readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.CandidateImage.Data", (object) null);
      readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.CandidateImage.UploadDate", (object) null);
      service.UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        readIdentity
      });
      IdentityImageService.InvalidateIdentityImage(requestContext, readIdentity);
    }

    public static byte[] ParseImage(HttpPostedFileBase postedFile)
    {
      if (postedFile == null || postedFile.ContentLength < 1)
        throw new ArgumentException(WACommonResources.NoImageDataUploaded);
      if (postedFile.ContentLength > 2621440)
        throw new ArgumentException(WACommonResources.ImageSizeTooLarge);
      Image image = string.IsNullOrWhiteSpace(postedFile.ContentType) || postedFile.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) ? Image.FromStream(postedFile.InputStream) : throw new ArgumentException(WACommonResources.InvalidContentType);
      int width = 144;
      int height = 144;
      if (image.Height > image.Width)
        width = 144 * image.Width / image.Height;
      else
        height = 144 * image.Height / image.Width;
      int x = (144 - width) / 2;
      int y = (144 - height) / 2;
      Bitmap bitmap = new Bitmap(144, 144);
      using (Graphics graphics = Graphics.FromImage((Image) bitmap))
        graphics.DrawImage(image, x, y, width, height);
      MemoryStream memoryStream = new MemoryStream();
      bitmap.Save((Stream) memoryStream, ImageFormat.Png);
      return memoryStream.ToArray();
    }

    public static void RemoveCandidateImage(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      identity.SetProperty("Microsoft.TeamFoundation.Identity.CandidateImage.Data", (object) null);
      identity.SetProperty("Microsoft.TeamFoundation.Identity.CandidateImage.UploadDate", (object) null);
      IVssRequestContext requestContext1 = requestContext;
      Microsoft.VisualStudio.Services.Identity.Identity[] identities = new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        identity
      };
      service.UpdateIdentities(requestContext1, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities);
      IdentityImageService.InvalidateIdentityImage(requestContext, identity);
    }

    public static void CommitCandidateImage(IVssRequestContext requestContext, Guid tfid)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = service.ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        tfid
      }, QueryMembership.None, (IEnumerable<string>) new string[1]
      {
        "Microsoft.TeamFoundation.Identity.CandidateImage.Data"
      })[0];
      if (readIdentity == null)
        throw new IdentityNotFoundException(tfid);
      object obj;
      if (!readIdentity.TryGetProperty("Microsoft.TeamFoundation.Identity.CandidateImage.Data", out obj))
        return;
      readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Data", obj);
      readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Type", (object) "image/png");
      readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Id", (object) Guid.NewGuid().ToByteArray());
      readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.CandidateImage.Data", (object) null);
      readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.CandidateImage.UploadDate", (object) null);
      service.UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        readIdentity
      });
      IdentityImageService.InvalidateIdentityImage(requestContext, readIdentity);
    }
  }
}
