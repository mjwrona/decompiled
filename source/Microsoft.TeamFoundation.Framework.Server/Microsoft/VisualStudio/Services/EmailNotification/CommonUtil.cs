// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.EmailNotification.CommonUtil
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.Users.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.EmailNotification
{
  public static class CommonUtil
  {
    private const string TokenPattern = "%%##([^#]*)##%%";
    public const string LocalizedEmailsFeatureFlag = "VisualStudio.Services.ProfileService.LocalizedLifecycleEmails";

    public static void TraceEnter(
      IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      [CallerMemberName] string methodName = null)
    {
      requestContext.TraceEnter(tracepoint, area, layer, methodName);
    }

    public static void TraceLeave(
      IVssRequestContext requestContext,
      int tracePoint,
      string area,
      string layer,
      [CallerMemberName] string methodName = null)
    {
      requestContext.TraceLeave(tracePoint, area, layer, methodName);
    }

    public static string GetOpenInVisualStudioUri(Uri accountUri) => CommonUtil.GetOpenInVisualStudioUri(accountUri.ToString());

    public static string GetOpenInVisualStudioUri(string accountUri)
    {
      try
      {
        return "vsweb://vs/?Product=Visual_Studio&EncFormat=UTF8&tfslink=" + Convert.ToBase64String(Encoding.UTF8.GetBytes("vstfs:///Framework/TeamProject/00000000-0000-0000-0000-000000000000?url=" + accountUri + "DefaultCollection&project=00000000-0000-0000-0000-000000000000"));
      }
      catch (Exception ex)
      {
        return string.Empty;
      }
    }

    public static IEnumerable<string> GetTokensFromTemplate(
      string templateHtml,
      string tokenopen,
      string tokenclose)
    {
      MatchCollection matchCollection = !string.IsNullOrWhiteSpace(templateHtml) ? Regex.Matches(templateHtml, "%%##([^#]*)##%%") : throw new EmailNotificationArgumentException("Cannot extract tokens from null or whitespace template");
      List<string> tokensFromTemplate = new List<string>();
      foreach (Capture capture in matchCollection)
      {
        string str = capture.Value.TrimStart(tokenopen.ToCharArray()).TrimEnd(tokenclose.ToCharArray());
        if (!tokensFromTemplate.Contains(str))
          tokensFromTemplate.Add(str);
      }
      return (IEnumerable<string>) tokensFromTemplate;
    }

    public static string GetForwardLink(int linkId, string mc_id = null) => !string.IsNullOrWhiteSpace(mc_id) ? string.Format("https://go.microsoft.com/fwlink/?LinkID={0}&wt.mc_id={1}", (object) linkId, (object) mc_id) : string.Format("https://go.microsoft.com/fwlink/?LinkID={0}", (object) linkId);

    public static CultureInfo GetUsersPreferredLanguage(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.ProfileService.LocalizedLifecycleEmails"))
        return CultureInfo.GetCultureInfo(1033);
      try
      {
        return CultureInfo.GetCultureInfo(requestContext.GetService<IUserService>().GetAttribute(requestContext, identity.Id, WellKnownUserAttributeNames.TFSCulture).Value);
      }
      catch (Exception ex) when (ex is UserDoesNotExistException || ex is UserAttributeDoesNotExistException)
      {
        return CultureInfo.CurrentUICulture;
      }
    }

    public static string GenerateHtmlToken(
      string attributeName,
      string tokenOpen,
      string tokenClose)
    {
      ArgumentUtility.CheckForNull<string>(attributeName, nameof (attributeName));
      ArgumentUtility.CheckForNull<string>(tokenOpen, nameof (tokenOpen));
      ArgumentUtility.CheckForNull<string>(tokenClose, nameof (tokenClose));
      return tokenOpen + attributeName + tokenClose;
    }
  }
}
