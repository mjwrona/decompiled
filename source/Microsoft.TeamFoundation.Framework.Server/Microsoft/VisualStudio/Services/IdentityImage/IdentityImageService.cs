// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityImage.IdentityImageService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Xml;

namespace Microsoft.VisualStudio.Services.IdentityImage
{
  public class IdentityImageService : IVssFrameworkService
  {
    public const string IdentityImageMimeType = "image/png";
    internal const string LegacyIdentityImageMimeType = "image/jpeg";
    private const string c_cacheKeyPrefix = "_identity_image_";
    private const string c_identifierCacheKeyPrefix = "_identity_identifier_";
    private string RegistryRoot = "/WebAccess/IdentityImage";
    private const string c_gravatarUrl = "www.gravatar.com";
    private const string c_secureGravatarUrl = "secure.gravatar.com";
    private const string c_gravatarAvatarPath = "/avatar/";
    private const string c_gravatarAvatarParams = "?r=g&d=";
    private const string c_gravatarTfsDefault = "mm";
    private const string c_gravatarBlankDefault = "blank";
    private const string IdentityImageDefaultAvatarMimeType = "image/png";

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<TeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.IdentityChanged, new SqlNotificationCallback(this.OnIdentityChanged), true);
      systemRequestContext.GetService<CachedRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<TeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.IdentityChanged, new SqlNotificationCallback(this.OnIdentityChanged), true);
      systemRequestContext.GetService<CachedRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), false, this.RegistryRoot + "/...");
      this.ReadRegistrySettings(systemRequestContext);
    }

    private void ReadRegistrySettings(IVssRequestContext requestContext)
    {
      RegistryEntry registryEntry = requestContext.GetService<CachedRegistryService>().ReadEntries(requestContext, (RegistryQuery) (this.RegistryRoot + "/DisableGravatar")).FirstOrDefault<RegistryEntry>();
      this.DisableGravatar = registryEntry != null && registryEntry.GetValue<bool>();
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.ReadRegistrySettings(requestContext);
    }

    private void OnIdentityChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null
      };
      string identityType1 = (string) null;
      string identifier1 = (string) null;
      string identityType2 = (string) null;
      string identifier2 = (string) null;
      using (StringReader input = new StringReader(eventData))
      {
        using (XmlReader xmlReader = XmlReader.Create((TextReader) input, settings))
        {
          xmlReader.Read();
          if (xmlReader.NodeType == XmlNodeType.Element)
          {
            if (xmlReader.LocalName == "data")
            {
              identityType1 = xmlReader.GetAttribute("gt");
              identifier1 = xmlReader.GetAttribute("gi");
              identityType2 = xmlReader.GetAttribute("mt");
              identifier2 = xmlReader.GetAttribute("mi");
            }
          }
        }
      }
      if (!string.IsNullOrEmpty(identityType1) && !string.IsNullOrEmpty(identifier1))
        IdentityImageCacheProvider.Remove(requestContext, identityType1, identifier1);
      if (string.IsNullOrEmpty(identityType2) || string.IsNullOrEmpty(identifier2))
        return;
      IdentityImageCacheProvider.Remove(requestContext, identityType2, identifier2);
    }

    public static void InvalidateIdentityImage(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity) => IdentityImageCacheProvider.Remove(requestContext, identity.Descriptor.IdentityType, identity.Descriptor.Identifier);

    public Guid? GetCachedIdentityImageId(IVssRequestContext requestContext, Guid identityId)
    {
      Guid imageId;
      return IdentityImageCacheProvider.Get(requestContext, identityId, out bool _, out imageId) ? new Guid?(imageId) : new Guid?();
    }

    public virtual Guid GetIdentityImageId(
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
      if (IdentityImageCacheProvider.Get(requestContext, identityId, out isContainer, out imageId))
        return imageId;
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        identityId
      }, QueryMembership.None, (IEnumerable<string>) new string[1]
      {
        "Microsoft.TeamFoundation.Identity.Image.Id"
      })[0];
      if (readIdentity == null)
        return Guid.Empty;
      object b;
      imageId = !readIdentity.TryGetProperty("Microsoft.TeamFoundation.Identity.Image.Id", out b) ? Guid.Empty : new Guid((byte[]) b);
      isContainer = readIdentity.IsContainer;
      if (imageId == Guid.Empty && !isContainer && !string.IsNullOrEmpty(readIdentity.CustomDisplayName))
        imageId = Guid.NewGuid();
      IdentityImageCacheProvider.Add(requestContext, readIdentity, imageId);
      return imageId;
    }

    public string GetGravatar(
      IVssRequestContext requestContext,
      string email,
      string urlScheme,
      bool isSecure,
      string defaultGravatar = "")
    {
      string lowerInvariant;
      try
      {
        lowerInvariant = new MailAddress(email).Address.ToLowerInvariant();
      }
      catch (Exception ex)
      {
        return (string) null;
      }
      string gravatar = (string) null;
      if (!string.IsNullOrEmpty(lowerInvariant))
      {
        byte[] md5 = MD5Util.CalculateMD5(Encoding.UTF8.GetBytes(lowerInvariant));
        StringBuilder stringBuilder = new StringBuilder();
        for (int index = 0; index < md5.Length; ++index)
          stringBuilder.Append(md5[index].ToString("x2"));
        gravatar = new UriBuilder(urlScheme, isSecure ? "secure.gravatar.com" : "www.gravatar.com", -1, "/avatar/" + stringBuilder.ToString(), this.GetAvatarParms(defaultGravatar)).ToString();
      }
      return gravatar;
    }

    public virtual void GetIdentityImageData(
      IVssRequestContext requestContext,
      Guid identityId,
      bool previewCandidate,
      out byte[] imageData,
      out string contentType,
      ImageSize? imageSize = null,
      bool skipGenerateAvatar = false)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      string[] propertyNameFilters;
      if (previewCandidate)
        propertyNameFilters = new string[1]
        {
          "Microsoft.TeamFoundation.Identity.CandidateImage.Data"
        };
      else
        propertyNameFilters = new string[3]
        {
          "Microsoft.TeamFoundation.Identity.Image.Id",
          "Microsoft.TeamFoundation.Identity.Image.Data",
          "Microsoft.TeamFoundation.Identity.Image.Type"
        };
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = service.ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        identityId
      }, QueryMembership.None, (IEnumerable<string>) propertyNameFilters)[0];
      this.GetIdentityImageData(requestContext, readIdentity, previewCandidate, out imageData, out contentType, imageSize, skipGenerateAvatar);
    }

    public virtual void GetIdentityImageData(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      bool previewCandidate,
      out byte[] imageData,
      out string contentType,
      ImageSize? imageSize = null,
      bool skipGenerateAvatar = false)
    {
      imageData = (byte[]) null;
      contentType = "image/jpeg";
      if (identity == null)
        return;
      if (previewCandidate)
      {
        object obj;
        if (!identity.TryGetProperty("Microsoft.TeamFoundation.Identity.CandidateImage.Data", out obj))
          return;
        imageData = (byte[]) obj;
        contentType = "image/png";
      }
      else
      {
        Guid imageId = Guid.Empty;
        object b;
        object obj1;
        if (identity.TryGetProperty("Microsoft.TeamFoundation.Identity.Image.Id", out b) && identity.TryGetProperty("Microsoft.TeamFoundation.Identity.Image.Data", out obj1))
        {
          imageId = new Guid((byte[]) b);
          imageData = (byte[]) obj1;
          object obj2;
          if (identity.TryGetProperty("Microsoft.TeamFoundation.Identity.Image.Type", out obj2))
            contentType = (string) obj2;
        }
        string str = identity.CustomDisplayName;
        if (string.IsNullOrEmpty(str) && requestContext.ExecutionEnvironment.IsOnPremisesDeployment && !identity.IsContainer)
          str = identity.DisplayName;
        if (imageId == Guid.Empty && !string.IsNullOrEmpty(str) && !skipGenerateAvatar)
        {
          imageData = AvatarUtils.GenerateAvatar(str, AvatarUtils.MapToColor(AvatarUtils.GetRandomDefaultAvatarColor(str)), 220, AvatarImageFormat.Png);
          contentType = "image/png";
        }
        IdentityImageCacheProvider.Add(requestContext, identity, imageId);
      }
    }

    public Microsoft.VisualStudio.Services.Identity.Identity GetIdentityFromEmail(
      IVssRequestContext requestContext,
      ref string email,
      bool resolveAmbiguous = true)
    {
      try
      {
        MailAddress mailAddress = new MailAddress(email);
        email = mailAddress.Address.ToLowerInvariant();
      }
      catch (Exception ex)
      {
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
      return this.GetIdentity(requestContext, email, IdentitySearchFilter.MailAddress, resolveAmbiguous);
    }

    public Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext,
      string identifier,
      IdentitySearchFilter searchFactor,
      bool resolveAmbiguous)
    {
      string cacheKey = "_identity_identifier_" + identifier;
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        cacheKey = "_identity_identifier_" + (requestContext.ServiceHost != null ? requestContext.ServiceHost.InstanceId.ToString() : "") + identifier;
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (IdentityFromIdentifierCacheProvider.TryGet(requestContext, cacheKey, out identity))
        return identity;
      bool flag = true;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) requestContext.GetService<IdentityService>().ReadIdentities(requestContext, searchFactor, identifier, QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i != null)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (source.Count == 0)
        flag = true;
      else if (source.Count > 1)
      {
        source = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) source.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (id => id.IsActive)).ToArray<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (source.Count != 1)
          flag = resolveAmbiguous;
      }
      if (source.Count == 1)
        identity = source[0];
      else if (source.Count > 1 & resolveAmbiguous)
        identity = source[0];
      if (flag)
        IdentityFromIdentifierCacheProvider.Set(requestContext, cacheKey, identity);
      return identity;
    }

    private string GetAvatarParms(string defaultGravatar) => "?r=g&d=" + (defaultGravatar == "mm" ? "mm" : "blank");

    public bool DisableGravatar { get; private set; }
  }
}
