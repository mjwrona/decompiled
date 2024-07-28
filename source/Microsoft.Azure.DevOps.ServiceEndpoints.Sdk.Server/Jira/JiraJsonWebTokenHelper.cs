// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira.JiraJsonWebTokenHelper
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira
{
  public sealed class JiraJsonWebTokenHelper
  {
    public static void ValidateJsonWebToken(
      IVssRequestContext requestContext,
      string token,
      string clientKey,
      string sharedSecret)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(token, nameof (token));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(clientKey, nameof (clientKey));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(sharedSecret, nameof (sharedSecret));
      JsonWebToken.Create(token).ValidateToken(new JsonWebTokenValidationParameters()
      {
        ValidateActor = false,
        ValidateAudience = false,
        AllowedAudiences = (IEnumerable<string>) null,
        ValidateExpiration = true,
        ValidateIssuer = true,
        ValidIssuers = (IEnumerable<string>) new string[1]
        {
          clientKey
        },
        ValidateNotBefore = false,
        ValidateSignature = true,
        SigningCredentials = VssSigningCredentials.Create(Encoding.UTF8.GetBytes(sharedSecret))
      });
    }

    public static string GenerateJsonWebToken(
      HttpRequestMessage request,
      JiraAuthentication authentication,
      string issuer = null)
    {
      DateTime utcNow = DateTime.UtcNow;
      return JsonWebToken.Create(issuer ?? authentication.JiraAppKey, authentication.ClientKey, utcNow, utcNow.AddMinutes(5.0), utcNow, (IEnumerable<Claim>) JiraJsonWebTokenHelper.GetAdditionalClaims(request), JiraJsonWebTokenHelper.GetCredentials(authentication)).EncodedToken;
    }

    public static string GetQueryStringHash(HttpRequestMessage request) => JiraJsonWebTokenHelper.GetSHA256HashHexString(Encoding.UTF8.GetBytes(JiraJsonWebTokenHelper.GetCanonicalRequest(request)));

    private static IList<Claim> GetAdditionalClaims(HttpRequestMessage request)
    {
      List<Claim> additionalClaims = new List<Claim>();
      additionalClaims.Add(new Claim("qsh", JiraJsonWebTokenHelper.GetQueryStringHash(request)));
      return (IList<Claim>) additionalClaims;
    }

    private static VssSigningCredentials GetCredentials(JiraAuthentication authentication) => VssSigningCredentials.Create(Encoding.UTF8.GetBytes(authentication.SharedSecret));

    private static string GetSHA256HashHexString(byte[] bytes)
    {
      byte[] hash = SHA256.Create().ComputeHash(bytes);
      StringBuilder stringBuilder = new StringBuilder(hash.Length * 2);
      for (int index = 0; index < hash.Length; ++index)
        stringBuilder.Append(hash[index].ToString("x2"));
      return stringBuilder.ToString();
    }

    private static string GetCanonicalRequest(HttpRequestMessage request) => string.Format("{0}&{1}&{2}", (object) JiraJsonWebTokenHelper.GetCanonicalMethod(request), (object) JiraJsonWebTokenHelper.GetCanonicalUri(request), (object) JiraJsonWebTokenHelper.GetCanonicalQueryString(request));

    private static string GetCanonicalQueryString(HttpRequestMessage request)
    {
      string enumerable = Uri.UnescapeDataString(request.RequestUri.Query);
      if (enumerable.IsNullOrEmpty<char>())
        return string.Empty;
      NameValueCollection queryString = HttpUtility.ParseQueryString(enumerable.Substring(1));
      Dictionary<string, string> source1 = new Dictionary<string, string>();
      foreach (string allKey in queryString.AllKeys)
      {
        if (!allKey.Equals("jwt", StringComparison.OrdinalIgnoreCase))
          source1.Add(Uri.EscapeDataString(allKey), Uri.EscapeDataString(queryString[allKey]));
      }
      IOrderedEnumerable<KeyValuePair<string, string>> source2 = source1.OrderBy<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (param => param.Key));
      string canonicalQueryString = "";
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) source2.OrderBy<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (param => param.Key)))
        canonicalQueryString = canonicalQueryString + keyValuePair.Key + "=" + keyValuePair.Value;
      return canonicalQueryString;
    }

    private static string GetCanonicalUri(HttpRequestMessage request) => request.RequestUri.LocalPath;

    private static string GetCanonicalMethod(HttpRequestMessage request) => request.Method.ToString().ToUpper();
  }
}
