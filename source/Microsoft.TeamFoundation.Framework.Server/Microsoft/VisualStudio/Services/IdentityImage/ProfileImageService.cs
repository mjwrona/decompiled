// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityImage.ProfileImageService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.Users.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.IdentityImage
{
  public class ProfileImageService : IdentityImageService
  {
    private IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper;
    public const string ProfileImageMimeType = "image/png";
    public const string SvgProfileImageMimeType = "image/svg+xml";
    private static readonly byte[] applicationProfileImageData = Encoding.UTF8.GetBytes("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"28\" height=\"28\" viewBox=\"-3 -3 23 23\"> <defs> <linearGradient id=\"b715ec20-95db-414c-982b-71456fb0c9ab\" x1=\"-6784.85\" y1=\"1118.78\" x2=\"-6784.85\" y2=\"1089.98\" gradientTransform=\"matrix(0.5, 0, 0, -0.5, 3400.41, 559.99)\" gradientUnits=\"userSpaceOnUse\"> <stop offset=\"0\" stop-color=\"#5ea0ef\" /> <stop offset=\"0.18\" stop-color=\"#589eed\" /> <stop offset=\"0.41\" stop-color=\"#4897e9\" /> <stop offset=\"0.66\" stop-color=\"#2e8ce1\" /> <stop offset=\"0.94\" stop-color=\"#0a7cd7\" /> <stop offset=\"1\" stop-color=\"#0078d4\" /> </linearGradient> <linearGradient id=\"f7e46209-f134-49aa-a850-2e9a1b04fba6\" x1=\"-1.47\" y1=\"14.91\" x2=\"17.16\" y2=\"14.8\" gradientUnits=\"userSpaceOnUse\"> <stop offset=\"0\" stop-color=\"#449cdd\" stop-opacity=\"0.15\" /> <stop offset=\"0.16\" stop-color=\"#2870ab\" stop-opacity=\"0\" /> <stop offset=\"0.18\" stop-color=\"#2469a3\" stop-opacity=\"0.06\" /> <stop offset=\"0.23\" stop-color=\"#1a5991\" stop-opacity=\"0.19\" /> <stop offset=\"0.28\" stop-color=\"#144f86\" stop-opacity=\"0.27\" /> <stop offset=\"0.34\" stop-color=\"#124c82\" stop-opacity=\"0.3\" /> <stop offset=\"0.76\" stop-color=\"#002851\" stop-opacity=\"0.35\" /> <stop offset=\"0.9\" stop-color=\"#2f7ab6\" stop-opacity=\"0\" /> <stop offset=\"1\" stop-color=\"#449cdd\" stop-opacity=\"0\" /> </linearGradient> <clipPath id=\"b36c7b72-34c8-47c6-ac8b-a96783f174f2\"> <circle cx=\"13.26\" cy=\"13.27\" r=\"4.16\" fill=\"none\" /> </clipPath> </defs> <title>Icon-identity-225</title> <path d=\"M5.61,10.65H9.94V15H5.61Zm-5-5.76H4.89V.57H1.17a.6.6,0,0,0-.6.6ZM1.17,15H4.89V10.65H.57v3.72A.6.6,0,0,0,1.17,15Zm-.6-5H4.89V5.61H.57Zm10.09,5h3.72a.6.6,0,0,0,.6-.6V10.65H10.66Zm-5-5H9.94V5.61H5.61Zm5.05,0H15V5.61H10.66Zm0-9.36V4.89H15V1.17a.6.6,0,0,0-.6-.6Zm-5,4.32H9.94V.57H5.61Z\" fill=\"url(#b715ec20-95db-414c-982b-71456fb0c9ab)\" /> <path d=\"M10.66,15h4.15a.59.59,0,0,1-.18-.29h-4Z\" opacity=\"0.95\" fill=\"url(#f7e46209-f134-49aa-a850-2e9a1b04fba6)\" /> <circle cx=\"13.26\" cy=\"13.27\" r=\"4.16\" fill=\"#32bedd\" /> <g clip-path=\"url(#b36c7b72-34c8-47c6-ac8b-a96783f174f2)\"> <path d=\"M17.06,13.87c-.21.11-.51.05-.65.16a1.6,1.6,0,0,0-.63.66c0,.06,0,.15-.07.18-.28.19-.56.39-.3.81-.3,0-.2-.27-.34-.35a.77.77,0,0,0-1.06.54.34.34,0,0,0,.16.37.26.26,0,0,0,.36-.12.18.18,0,0,1,.26-.06c0,.16-.28.45.18.46.12,0,.09.14.06.23s.06.24.19.3,0,0,0,.05,0,0,0,0c-.37,0-.45-.45-.81-.5a4.28,4.28,0,0,1-.43-.14c-.27-.09-.58-.14-.64-.54a.77.77,0,0,0-.36-.49c-.12-.08-.19-.23-.31-.32a1,1,0,0,0-.22-.36.94.94,0,0,1-.14-.87,2.32,2.32,0,0,0-.1-1.68c-.1-.32-.12-.65-.48-.86s-.3.12-.44.05-.22.13-.26.15c-.29.08-.54.33-.91.29.14-.07.25-.11.35-.17s.29-.14.07-.38-.14-.59.45-.83c-.05-.14-.41-.11-.25-.37s.28-.13.43,0c.1-.19-.12-.43.05-.54a2.86,2.86,0,0,1,.72-.29.25.25,0,0,1,.34.11c.18.28.54.33.72.61.05.09.15,0,.22,0,.41-.14.68.12,1,.33s.27.34.53.31a.49.49,0,0,1,.21,0c.16.07.31.11.43-.06a.3.3,0,0,0,.26-.23c.06-.09.09-.22.24-.16A1.15,1.15,0,0,0,16,11c.13-.1.26-.16.11-.39a.37.37,0,0,1,.25-.56.54.54,0,0,1,.63.23c.19.34.25.73.47,1,.07.09,0,.13,0,.2s-.18,0-.28-.06l.07.43a.66.66,0,0,1-.82-.25c0-.12,0-.17.12-.23s.19-.11.19-.24a.19.19,0,0,0-.1-.2c-.08,0-.13,0-.15.09s-.33.25-.28.53-.19.22-.33.14c-.33-.17-.45.13-.59.29s0,.34.16.46.31.21.37.4a.88.88,0,0,0,.29-.58c0-.18.07-.43.32-.46a.43.43,0,0,1,.44.28c.05.11.13.12.23.13a8.79,8.79,0,0,1,.54,1.52l-.37-.05c0-.29-.25-.16-.39-.22C16.84,13.64,17.07,13.69,17.06,13.87Z\" fill=\"#b4ec36\" /> <path d=\"M15.62,10.27c-.13-.12-.27-.23-.06-.4s.2-.29.3,0A.37.37,0,0,1,15.62,10.27Z\" fill=\"#b4ec36\" /> <path d=\"M15.93,9.87c.11-.11.25-.1.27,0s-.13.19-.26.23S15.85,10,15.93,9.87Z\" fill=\"#b4ec36\" /> <path d=\"M15.16,8.94,14.81,9c0-.25.25-.27.42-.23S15.21,8.89,15.16,8.94Z\" fill=\"#b4ec36\" /> </g></svg>");
    private static readonly byte[] managedIdentityProfileImageData = Encoding.UTF8.GetBytes("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"28\" height=\"28\" viewBox=\"-3 -3 23 23\"> <defs> <linearGradient id=\"a\" x1=\"13.18\" x2=\"8.63\" y1=\"13.01\" y2=\"4.38\" gradientUnits=\"userSpaceOnUse\"> <stop stop-color=\"#1988d9\" offset=\"0\"/> <stop stop-color=\"#54aef0\" offset=\".9\"/> </linearGradient> <linearGradient id=\"b\" x1=\"11.22\" x2=\"14.37\" y1=\"10.5\" y2=\"15.93\" gradientUnits=\"userSpaceOnUse\"> <stop stop-color=\"#54aef0\" offset=\".1\"/> <stop stop-color=\"#4fabee\" offset=\".29\"/> <stop stop-color=\"#41a2e9\" offset=\".51\"/> <stop stop-color=\"#2a93e0\" offset=\".74\"/> <stop stop-color=\"#1988d9\" offset=\".88\"/> </linearGradient> </defs> <title>Icon-identity-227</title> <polygon points=\"1.15 10.22 8.93 15.27 16.85 10.21 17.85 11.36 8.93 17.11 0.15 11.37\" fill=\"#50e6ff\"/> <polygon points=\"1.73 9.58 8.93 1 16.28 9.59 8.93 14.23\" fill=\"#fff\"/> <polygon points=\"8.93 1 8.93 14.23 1.73 9.58\" fill=\"#50e6ff\"/> <polygon points=\"8.93 1 8.93 14.23 16.28 9.59\" fill=\"url(#a)\"/> <polygon points=\"8.93 7.83 16.28 9.59 8.93 14.23\" fill=\"#53b1e0\"/> <polygon points=\"8.93 14.23 1.73 9.58 8.93 7.83\" fill=\"#9cebff\"/> <polygon points=\"8.93 17.11 17.85 11.36 16.85 10.21 8.93 15.27\" fill=\"url(#b)\"/> <path d=\"M16.32,11.88a1.16,1.16,0,0,0,0-1.65h0l-2-2a1.16,1.16,0,0,0-1.64,0h0l-2,2a1.18,1.18,0,0,0,0,1.65l1.67,1.67a.32.32,0,0,1,.09.23v3.1a.36.36,0,0,0,.12.28l.76.76a.25.25,0,0,0,.37,0l.73-.73h0l.44-.44a.15.15,0,0,0,0-.21l-.31-.31a.16.16,0,0,1,0-.24l.31-.31a.16.16,0,0,0,0-.22l-.31-.31a.15.15,0,0,1,0-.23l.31-.32a.15.15,0,0,0,0-.21l-.44-.44v-.16ZM13.5,8.71a.66.66,0,1,1-.66.66.66.66,0,0,1,.66-.66Z\" fill=\"#ffca00\"/> <path d=\"M13,17h0a.14.14,0,0,0,.24-.11V14.33a.16.16,0,0,0-.06-.13h0a.14.14,0,0,0-.22.13v2.51A.15.15,0,0,0,13,17Z\" fill=\"#ff9300\" opacity=\".75\"/> <rect x=\"11.9\" y=\"10.9\" width=\"3.29\" height=\".39\" rx=\".18\" fill=\"#ff9300\" opacity=\".75\"/> <rect x=\"11.9\" y=\"11.53\" width=\"3.29\" height=\".39\" rx=\".18\" fill=\"#ff9300\" opacity=\".75\"/></svg>");

    public ProfileImageService()
      : this(AadServicePrincipalConfigurationHelper.Instance)
    {
    }

    public ProfileImageService(
      IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper)
    {
      this.aadServicePrincipalConfigurationHelper = aadServicePrincipalConfigurationHelper;
    }

    public override void GetIdentityImageData(
      IVssRequestContext requestContext,
      Guid identityId,
      bool previewCandidate,
      out byte[] imageData,
      out string contentType,
      ImageSize? imageSize = null,
      bool skipGenerateAvatar = false)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        identityId
      }, QueryMembership.None, (IEnumerable<string>) null).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      this.GetIdentityImageData(requestContext, identity, previewCandidate, out imageData, out contentType, imageSize, skipGenerateAvatar);
    }

    public override void GetIdentityImageData(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity requestedIdentity,
      bool previewCandidate,
      out byte[] imageData,
      out string contentType,
      ImageSize? imageSize = null,
      bool skipGenerateAvatar = false)
    {
      if (requestedIdentity == null)
      {
        contentType = (string) null;
        imageData = (byte[]) null;
      }
      else
      {
        Guid id = requestedIdentity.Id;
        bool flag = this.aadServicePrincipalConfigurationHelper.IsUserHubSupportServicePrincipalsEnabled(requestContext);
        if (flag && requestedIdentity.MetaType == IdentityMetaType.Application)
        {
          imageData = ProfileImageService.applicationProfileImageData;
          contentType = "image/svg+xml";
        }
        else if (flag && requestedIdentity.MetaType == IdentityMetaType.ManagedIdentity)
        {
          imageData = ProfileImageService.managedIdentityProfileImageData;
          contentType = "image/svg+xml";
        }
        else if (requestedIdentity.IsContainer)
        {
          string name = previewCandidate ? "Microsoft.TeamFoundation.Identity.CandidateImage.Data" : "Microsoft.TeamFoundation.Identity.Image.Data";
          requestedIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
          {
            id
          }, QueryMembership.None, (IEnumerable<string>) new string[1]
          {
            name
          }).Single<Microsoft.VisualStudio.Services.Identity.Identity>();
          object obj;
          requestedIdentity.TryGetProperty(name, out obj);
          imageData = obj as byte[];
          contentType = "image/png";
        }
        else if (previewCandidate)
        {
          IUserService service = requestContext.GetService<IUserService>();
          imageData = (byte[]) null;
          contentType = (string) null;
          try
          {
            UserAttribute attribute = service.GetAttribute(requestContext, id, WellKnownUserAttributeNames.AvatarPreview);
            if (attribute == null)
              return;
            imageData = Convert.FromBase64String(attribute.Value);
            contentType = "image/png";
          }
          catch
          {
          }
        }
        else
        {
          IUserService service = requestContext.GetService<IUserService>();
          AvatarSize size = !imageSize.HasValue || !Enum.IsDefined(typeof (AvatarSize), (object) (int) imageSize.Value) ? AvatarSize.Large : (AvatarSize) imageSize.Value;
          Avatar avatar = (Avatar) null;
          try
          {
            avatar = service.GetAvatar(requestContext, id, size);
          }
          catch
          {
          }
          if (avatar != null)
          {
            imageData = avatar.Image;
            contentType = "image/png";
          }
          else
          {
            imageData = (byte[]) null;
            contentType = (string) null;
          }
        }
      }
    }

    private Guid FetchIdentityImageId(
      IVssRequestContext requestContext,
      Guid identityId,
      out bool isContainer)
    {
      isContainer = false;
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        identityId
      }, QueryMembership.None, (IEnumerable<string>) new string[1]
      {
        "Microsoft.TeamFoundation.Identity.Image.Id"
      }).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null)
        return Guid.Empty;
      if (identity.IsContainer)
      {
        isContainer = true;
        if (identityId == Guid.Empty)
        {
          isContainer = false;
          return Guid.Empty;
        }
        byte[] property = identity.GetProperty<byte[]>("Microsoft.TeamFoundation.Identity.Image.Id", (byte[]) null);
        return property == null ? Guid.Empty : new Guid(property);
      }
      IUserService service = requestContext.GetService<IUserService>();
      try
      {
        return new Guid(0, (short) 0, (short) 0, BitConverter.GetBytes(service.GetUser(requestContext, identityId).LastModified.UtcTicks));
      }
      catch
      {
        return Guid.Empty;
      }
    }

    public override Guid GetIdentityImageId(
      IVssRequestContext requestContext,
      Guid identityId,
      out bool isContainer)
    {
      if (identityId == Guid.Empty)
      {
        isContainer = false;
        return Guid.Empty;
      }
      Guid imageId;
      if (!IdentityImageCacheProvider.Get(requestContext, identityId, out isContainer, out imageId))
      {
        imageId = this.FetchIdentityImageId(requestContext, identityId, out isContainer);
        if (imageId == Guid.Empty)
          return imageId;
        Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
        {
          identityId
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity != null)
          IdentityImageCacheProvider.Add(requestContext, identity, imageId);
      }
      return imageId;
    }
  }
}
