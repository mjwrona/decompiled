// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Utility.ReCaptchaUtility
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Utility
{
  public static class ReCaptchaUtility
  {
    private static readonly HttpClient httpClient = new HttpClient();
    private static readonly string ServiceLayer = nameof (ReCaptchaUtility);
    internal const string RegistryPathForApprovedUserAgentsForByPassingReCaptcha = "/Configuration/Service/Gallery/PublisherValidation/ApprovedUserAgentsForByPassingReCaptcha";

    public static bool IsReCaptchaTokenValid(IVssRequestContext requestContext, string token) => ReCaptchaUtility.IsReCaptchaTokenValidAsync(requestContext, token).GetAwaiter().GetResult();

    public static async Task<bool> IsReCaptchaTokenValidAsync(
      IVssRequestContext requestContext,
      string token)
    {
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      IVssRequestContext requestContext1 = requestContext.Elevate();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext1, "ConfigurationSecrets", "CaptchaPrivateKey", false);
      if (itemInfo == null)
        throw new Exception(GalleryResources.ReCaptchaStrongBoxItemNotFound());
      string str = service.GetString(requestContext1, itemInfo.DrawerId, itemInfo.LookupKey);
      if (string.IsNullOrWhiteSpace(str))
        throw new Exception(GalleryResources.ReCaptchaPrivateKeyNotFound());
      try
      {
        string requestUri = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", (object) str, (object) token);
        return JsonConvert.DeserializeObject<RecaptchaState>(await ReCaptchaUtility.httpClient.GetStringAsync(requestUri).ConfigureAwait(false)).Success;
      }
      catch (HttpRequestException ex)
      {
        requestContext.Trace(12062078, TraceLevel.Error, "gallery", ReCaptchaUtility.ServiceLayer, ex.Message);
        return false;
      }
    }

    public static bool IsReCaptchaEnabledForFeature(
      IVssRequestContext requestContext,
      string featureFlag)
    {
      return requestContext.IsFeatureEnabled(featureFlag) && !ReCaptchaUtility.IsUserAgentApproved(requestContext);
    }

    public static bool IsUserAgentApproved(IVssRequestContext requestContext) => ((IEnumerable<string>) requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PublisherValidation/ApprovedUserAgentsForByPassingReCaptcha", string.Empty).Split(new char[1]
    {
      ';'
    }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (approvedUserAgent => approvedUserAgent.Trim().ToLowerInvariant())).ToList<string>().Any<string>((Func<string, bool>) (approvedUserAgent => requestContext.UserAgent.ToLowerInvariant().Contains(approvedUserAgent)));
  }
}
