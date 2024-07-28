// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.FontController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Web.Mvc;
using System.Web.UI;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [ByPassTfsActivityLogFilter]
  [SupportedRouteArea("Public", NavigationContextLevels.All)]
  public class FontController : WebPlatformController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(Duration = 31536000, Location = OutputCacheLocation.Any, VaryByParam = "*", VaryByContentEncoding = "gzip;deflate")]
    public ActionResult Register(string locale = null, string css = null)
    {
      CultureInfo culture = (CultureInfo) null;
      if (!string.IsNullOrEmpty(locale))
      {
        if (!locale.Equals("auto", StringComparison.OrdinalIgnoreCase))
        {
          try
          {
            culture = new CultureInfo(locale);
          }
          catch (Exception ex)
          {
            this.TfsRequestContext.TraceException(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, ex);
          }
        }
      }
      string registeredFontFaces = FontRegistration.GetRegisteredFontFaces(this.TfsRequestContext, css, culture, "../../_static/3rdParty/_fonts/");
      if (string.IsNullOrEmpty(registeredFontFaces))
        return (ActionResult) this.HttpNotFound("Font registration css not found");
      return (ActionResult) new ContentResult()
      {
        Content = registeredFontFaces,
        ContentType = "text/css"
      };
    }
  }
}
