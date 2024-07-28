// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.GlobalizationContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Routing;
using System.Web.Security.AntiXss;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class GlobalizationContext : WebSdkMetadata
  {
    public const string ThemeQueryParam = "theme";
    private IVssRequestContext m_tfsRequestContext;
    private RequestContext m_requestContext;
    private TimeZoneInfo m_timezone;
    private int? m_utcOffset;
    private string m_theme;
    private string m_explicitTheme;

    public GlobalizationContext(IVssRequestContext tfsRequestContext, RequestContext requestContext)
    {
      this.m_tfsRequestContext = tfsRequestContext;
      this.m_requestContext = requestContext;
    }

    [DataMember]
    public virtual string ExplicitTheme
    {
      get
      {
        if (this.m_explicitTheme == null)
        {
          string themeName = AntiXssEncoder.HtmlEncode(this.m_requestContext.HttpContext.Request["theme"], false);
          if (!string.IsNullOrEmpty(themeName) && ThemesUtility.IsThemeNameValid(themeName, this.m_requestContext.HttpContext))
          {
            this.m_explicitTheme = themeName;
          }
          else
          {
            IUserPreferencesService service = this.m_tfsRequestContext.GetService<IUserPreferencesService>();
            string webAccessTheme = service.GetUserPreferences(this.m_tfsRequestContext).WebAccessTheme;
            if (webAccessTheme != null && !string.Equals(webAccessTheme, ThemesUtility.DefaultThemeName, StringComparison.OrdinalIgnoreCase))
              this.m_explicitTheme = service.GetUserPreferences(this.m_tfsRequestContext).WebAccessTheme;
          }
          if (this.m_explicitTheme == null)
            this.m_explicitTheme = "";
        }
        return this.m_explicitTheme;
      }
    }

    [DataMember]
    public virtual string Theme
    {
      get
      {
        if (this.m_theme == null)
        {
          HttpCookie cookie = this.m_requestContext.HttpContext.Request.Cookies["TFS-AUTO-THEME"];
          this.m_theme = cookie == null ? ThemesUtility.DefaultThemeName : cookie.Value;
        }
        return this.m_theme;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public bool TypeAheadDisabled => this.m_tfsRequestContext.GetService<IUserPreferencesService>().GetUserPreferences(this.m_tfsRequestContext).TypeAheadDisabled;

    [DataMember(EmitDefaultValue = false)]
    public string Culture => CultureInfo.CurrentUICulture.Name;

    public TimeZoneInfo UserTimeZone
    {
      get
      {
        if (this.m_timezone == null)
          this.m_timezone = this.m_tfsRequestContext.GetTimeZone();
        return this.m_timezone;
      }
    }

    [DataMember]
    public int TimezoneOffset
    {
      get
      {
        if (!this.m_utcOffset.HasValue)
          this.m_utcOffset = new int?((int) this.UserTimeZone.GetUtcOffset(DateTime.Now).TotalMilliseconds);
        return this.m_utcOffset.Value;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public string TimeZoneId => this.UserTimeZone.Id;
  }
}
