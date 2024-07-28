// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionBaseController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using Microsoft.VisualStudio.Services.Gallery.Server.Utility;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class ExtensionBaseController : GalleryController
  {
    [HttpGet]
    [ClientLocationId("E11EA35A-16FE-4B80-AB11-C4CAB88A0966")]
    [ClientHeaderParameter("X-Market-AccountToken", typeof (string), "accountTokenHeader", "Header to pass the account token", true, true)]
    public PublishedExtension GetExtension(
      string publisherName,
      string extensionName,
      string version = null,
      ExtensionQueryFlags flags = ExtensionQueryFlags.None,
      string accountToken = null)
    {
      accountToken = GalleryServerUtil.TryUseAccountTokenFromHttpHeader(this.TfsRequestContext, this.Request?.Headers, "ExtensionBaseController.GetExtension", accountToken);
      return this.TfsRequestContext.GetService<IPublishedExtensionService>().QueryExtension(this.TfsRequestContext, publisherName, extensionName, version, flags, accountToken);
    }

    [HttpGet]
    [ClientLocationId("A41192C8-9525-4B58-BC86-179FA549D80D")]
    public PublishedExtension GetExtensionById(
      Guid extensionId,
      string version = null,
      ExtensionQueryFlags flags = ExtensionQueryFlags.None)
    {
      return this.TfsRequestContext.GetService<IPublishedExtensionService>().QueryExtensionById(this.TfsRequestContext, extensionId, version, flags, Guid.Empty);
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("E11EA35A-16FE-4B80-AB11-C4CAB88A0966")]
    public HttpResponseMessage DeleteExtension(
      string publisherName,
      string extensionName,
      string version = null)
    {
      if (string.IsNullOrEmpty(version))
      {
        this.TfsRequestContext.GetService<IPublishedExtensionService>().DeleteExtension(this.TfsRequestContext, publisherName, extensionName, version);
        return this.Request.CreateResponse(HttpStatusCode.NoContent);
      }
      throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
      {
        ReasonPhrase = GalleryResources.VersionDeletionNotSupported()
      });
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("A41192C8-9525-4B58-BC86-179FA549D80D")]
    public HttpResponseMessage DeleteExtensionById(Guid extensionId, string version = null)
    {
      if (string.IsNullOrEmpty(version))
      {
        this.TfsRequestContext.GetService<IPublishedExtensionService>().DeleteExtension(this.TfsRequestContext, extensionId, version);
        return this.Request.CreateResponse(HttpStatusCode.NoContent);
      }
      throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
      {
        ReasonPhrase = GalleryResources.VersionDeletionNotSupported()
      });
    }

    [HttpPatch]
    [ClientResponseType(typeof (PublishedExtension), null, null)]
    [ClientLocationId("E11EA35A-16FE-4B80-AB11-C4CAB88A0966")]
    public HttpResponseMessage UpdateExtensionProperties(
      string publisherName,
      string extensionName,
      PublishedExtensionFlags flags)
    {
      IPublishedExtensionService service = this.TfsRequestContext.GetService<IPublishedExtensionService>();
      PublishedExtension publishedExtension = service.QueryExtension(this.TfsRequestContext, publisherName, extensionName, (string) null, ExtensionQueryFlags.IncludeInstallationTargets, (string) null);
      if (publishedExtension.Flags.HasFlag((System.Enum) PublishedExtensionFlags.Locked))
        throw new Exception(GalleryResources.LockedExtensionEditErrorMessage());
      flags &= ~PublishedExtensionFlags.Validated;
      return this.Request.CreateResponse<PublishedExtension>(HttpStatusCode.OK, service.UpdateExtensionProperties(this.TfsRequestContext, publisherName, extensionName, publishedExtension.DisplayName, flags, publishedExtension.ShortDescription, publishedExtension.LongDescription));
    }

    protected PublishedExtension CreateOrUpdateExtensionInternal(
      string publisherName,
      string extensionName,
      string extensionType = null,
      string reCaptchaToken = null)
    {
      using (Stream extensionPackageStream = GalleryServerUtil.GetExtensionPackageStream(this.Request))
      {
        try
        {
          using (PublishedExtensionComponent component = this.TfsRequestContext.CreateComponent<PublishedExtensionComponent>())
            component.QueryExtension(publisherName, extensionName, (string) null, Guid.Empty, ExtensionQueryFlags.None);
        }
        catch (ExtensionDoesNotExistException ex)
        {
          return this.CreateExtensionWithPublisherInternal(extensionPackageStream, publisherName, extensionType, reCaptchaToken);
        }
        return this.UpdateExtensionInternal(extensionPackageStream, publisherName, extensionName, extensionType, reCaptchaToken);
      }
    }

    protected PublishedExtension CreateExtensionWithPublisherInternal(
      Stream extensionPackageStream,
      string publisherName,
      string extensionType = null,
      string reCaptchaToken = null)
    {
      this.TfsRequestContext.TraceEnter(12062083, "gallery", "ReCaptchaValidation", nameof (CreateExtensionWithPublisherInternal));
      this.TfsRequestContext.RequestTimer.SetTimeToFirstPageBegin();
      IPublishedExtensionService service = this.TfsRequestContext.GetService<IPublishedExtensionService>();
      if (!string.IsNullOrWhiteSpace(extensionType) && extensionType == "Visual Studio Code")
      {
        this.TfsRequestContext.Trace(12062083, TraceLevel.Info, "gallery", "ReCaptchaValidation", "Extension Type =" + extensionType);
        if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaForCreateVisualStudioCodeExtension") && !ReCaptchaUtility.IsUserAgentApproved(this.TfsRequestContext))
        {
          this.TfsRequestContext.Trace(12062083, TraceLevel.Info, "gallery", "ReCaptchaValidation", "VS Code Recaptcha enabled for Create. Token =" + reCaptchaToken);
          bool isValid = ReCaptchaUtility.IsReCaptchaTokenValid(this.TfsRequestContext, reCaptchaToken);
          Dictionary<string, object> ciData = this.prepareCIData(isValid, publisherName, "CreateScenario", string.Empty, reCaptchaToken);
          service.PublishReCaptchaTokenCIForVSCodeExtension(this.TfsRequestContext, (IDictionary<string, object>) ciData);
          if (!isValid)
          {
            InvalidReCaptchaTokenException captchaTokenException = new InvalidReCaptchaTokenException(GalleryResources.InvalidReCaptchaToken());
            this.TfsRequestContext.TraceException(12062083, "gallery", "ReCaptchaValidation", (Exception) captchaTokenException);
            throw captchaTokenException;
          }
        }
      }
      PublishedExtension extension = service.CreateExtension(this.TfsRequestContext, extensionPackageStream, publisherName);
      this.TfsRequestContext.TraceLeave(12062083, "gallery", "ReCaptchaValidation", nameof (CreateExtensionWithPublisherInternal));
      return extension;
    }

    protected PublishedExtension UpdateExtensionInternal(
      Stream extensionPackageStream,
      string publisherName,
      string extensionName,
      string extensionType = null,
      string reCaptchaToken = null)
    {
      this.TfsRequestContext.TraceEnter(12062084, "gallery", "ReCaptchaValidation", nameof (UpdateExtensionInternal));
      this.TfsRequestContext.RequestTimer.SetTimeToFirstPageBegin();
      IPublishedExtensionService service = this.TfsRequestContext.GetService<IPublishedExtensionService>();
      if (!string.IsNullOrWhiteSpace(extensionType) && extensionType == "Visual Studio Code")
      {
        this.TfsRequestContext.Trace(12062084, TraceLevel.Info, "gallery", "ReCaptchaValidation", "Extension Type =" + extensionType);
        if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableReCaptchaForUpdateVisualStudioCodeExtension") && !ReCaptchaUtility.IsUserAgentApproved(this.TfsRequestContext))
        {
          this.TfsRequestContext.Trace(12062084, TraceLevel.Info, "gallery", "ReCaptchaValidation", "VS Code Recaptcha enabled for update. Token =" + reCaptchaToken);
          bool isValid = ReCaptchaUtility.IsReCaptchaTokenValid(this.TfsRequestContext, reCaptchaToken);
          Dictionary<string, object> ciData = this.prepareCIData(isValid, publisherName, "UpdateScenario", extensionName, reCaptchaToken);
          service.PublishReCaptchaTokenCIForVSCodeExtension(this.TfsRequestContext, (IDictionary<string, object>) ciData);
          if (!isValid)
          {
            InvalidReCaptchaTokenException captchaTokenException = new InvalidReCaptchaTokenException(GalleryResources.InvalidReCaptchaToken());
            this.TfsRequestContext.TraceException(12062084, "gallery", "ReCaptchaValidation", (Exception) captchaTokenException);
            throw captchaTokenException;
          }
        }
      }
      PublishedExtension publishedExtension = service.UpdateExtension(this.TfsRequestContext, extensionPackageStream, extensionName, publisherName);
      this.TfsRequestContext.TraceLeave(12062084, "gallery", "ReCaptchaValidation", nameof (UpdateExtensionInternal));
      return publishedExtension;
    }

    private Dictionary<string, object> prepareCIData(
      bool isValid,
      string publisherName,
      string scenario,
      string extensionName,
      string reCaptchaToken = "")
    {
      bool flag = GalleryServerUtil.IsRequestFromChinaRegion(this.TfsRequestContext);
      return new Dictionary<string, object>()
      {
        {
          CustomerIntelligenceProperty.Action,
          (object) "ReCaptchaValidation"
        },
        {
          "FeatureValidation",
          isValid ? (object) "Valid" : (object) "Invalid"
        },
        {
          "Source",
          (object) "ExtensionForVSCode"
        },
        {
          "UserAgent",
          (object) this.TfsRequestContext.UserAgent
        },
        {
          "PublisherName",
          (object) publisherName
        },
        {
          "Scenario",
          (object) scenario
        },
        {
          "ExtensionName",
          (object) extensionName
        },
        {
          "RecaptchaToken",
          (object) reCaptchaToken
        },
        {
          "IsRequestFromChinaRegion",
          (object) flag
        }
      };
    }
  }
}
