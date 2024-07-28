// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ProfileImageUtility
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.IdentityImage;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.Users.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class ProfileImageUtility
  {
    public const string ProfileImageSVGMimeType = "image/svg+xml";
    internal const string LegacyProfileImageMimeType = "image/jpeg";
    private const int c_maxImageDimension = 144;
    private const int c_maxUploadSize = 4194304;

    public static void RemoveImage(IVssRequestContext requestContext, Guid tfid)
    {
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        tfid
      }, QueryMembership.None, (IEnumerable<string>) null)[0];
      if (readIdentity == null)
        throw new IdentityNotFoundException(tfid);
      if (readIdentity.IsContainer)
      {
        IdentityImageUtility.RemoveImage(requestContext, tfid);
      }
      else
      {
        requestContext.GetService<IUserService>().DeleteAvatar(requestContext, tfid);
        IdentityImageService.InvalidateIdentityImage(requestContext, readIdentity);
      }
    }

    public static void RemoveCandidateImage(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (identity.IsContainer)
      {
        IdentityImageUtility.RemoveCandidateImage(requestContext, identity);
      }
      else
      {
        IUserService service = requestContext.GetService<IUserService>();
        try
        {
          service.DeleteAttribute(requestContext, identity.Id, WellKnownUserAttributeNames.AvatarPreview);
        }
        catch
        {
        }
        IdentityImageService.InvalidateIdentityImage(requestContext, identity);
      }
    }

    public static void CommitCandidateImage(IVssRequestContext requestContext, Guid tfid)
    {
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        tfid
      }, QueryMembership.None, (IEnumerable<string>) new string[1]
      {
        "Microsoft.TeamFoundation.Identity.CandidateImage.Data"
      })[0];
      if (readIdentity == null)
        throw new IdentityNotFoundException(tfid);
      if (readIdentity.IsContainer)
      {
        IdentityImageUtility.CommitCandidateImage(requestContext, tfid);
      }
      else
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
        IUserService service = vssRequestContext.GetService<IUserService>();
        try
        {
          if (service.GetUser(vssRequestContext, tfid) == null)
            throw new IdentityNotFoundException(tfid);
          UserAttribute attribute = service.GetAttribute(vssRequestContext, tfid, WellKnownUserAttributeNames.AvatarPreview);
          byte[] numArray = attribute != null && !string.IsNullOrEmpty(attribute.Value) ? Convert.FromBase64String(attribute.Value) : throw new BadAvatarValueException(string.Format("Candidate image could not be found for identity {0}", (object) tfid.ToString()));
          service.UpdateAvatar(vssRequestContext, tfid, new Avatar()
          {
            Image = numArray
          });
        }
        catch (IdentityNotFoundException ex)
        {
          throw;
        }
        catch
        {
        }
        ProfileImageUtility.RemoveCandidateImage(requestContext, readIdentity);
      }
    }
  }
}
