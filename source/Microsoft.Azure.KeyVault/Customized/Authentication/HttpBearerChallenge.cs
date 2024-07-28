// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.HttpBearerChallenge
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.Azure.KeyVault
{
  public sealed class HttpBearerChallenge
  {
    private const string Authorization = "authorization";
    private const string AuthorizationUri = "authorization_uri";
    private const string Bearer = "Bearer";
    private Dictionary<string, string> _parameters;
    private string _sourceAuthority;
    private Uri _sourceUri;

    public static bool IsBearerChallenge(string challenge) => !string.IsNullOrEmpty(challenge) && challenge.Trim().StartsWith("Bearer ");

    internal static HttpBearerChallenge GetBearerChallengeFromResponse(HttpResponseMessage response)
    {
      if (response == null)
        return (HttpBearerChallenge) null;
      string challenge = response != null ? response.Headers.WwwAuthenticate.FirstOrDefault<AuthenticationHeaderValue>()?.ToString() : (string) null;
      return !string.IsNullOrEmpty(challenge) && HttpBearerChallenge.IsBearerChallenge(challenge) ? new HttpBearerChallenge(response.RequestMessage.RequestUri, challenge) : (HttpBearerChallenge) null;
    }

    public HttpBearerChallenge(Uri requestUri, string challenge)
    {
      string str1 = HttpBearerChallenge.ValidateRequestURI(requestUri);
      string str2 = HttpBearerChallenge.ValidateChallenge(challenge);
      this._sourceAuthority = str1;
      this._sourceUri = requestUri;
      this._parameters = new Dictionary<string, string>();
      string[] separator = new string[1]{ "," };
      string[] strArray1 = str2.Split(separator, StringSplitOptions.RemoveEmptyEntries);
      if (strArray1 != null && strArray1.Length != 0)
      {
        for (int index = 0; index < strArray1.Length; ++index)
        {
          string[] strArray2 = strArray1[index].Split('=');
          if (strArray2.Length == 2)
          {
            string key = strArray2[0].Trim().Trim('"');
            string str3 = strArray2[1].Trim().Trim('"');
            if (!string.IsNullOrEmpty(key))
              this._parameters[key] = str3;
          }
        }
      }
      if (this._parameters.Count < 1)
        throw new ArgumentException("Invalid challenge parameters", nameof (challenge));
      if (!this._parameters.ContainsKey("authorization") && !this._parameters.ContainsKey("authorization_uri"))
        throw new ArgumentException("Invalid challenge parameters", nameof (challenge));
    }

    public bool TryGetValue(string key, out string value) => this._parameters.TryGetValue(key, out value);

    public string AuthorizationServer
    {
      get
      {
        string empty = string.Empty;
        return this._parameters.TryGetValue("authorization_uri", out empty) || this._parameters.TryGetValue("authorization", out empty) ? empty : string.Empty;
      }
    }

    public string Resource
    {
      get
      {
        string empty = string.Empty;
        return this._parameters.TryGetValue("resource", out empty) ? empty : this._sourceAuthority;
      }
    }

    public string Scope
    {
      get
      {
        string empty = string.Empty;
        return this._parameters.TryGetValue("scope", out empty) ? empty : string.Empty;
      }
    }

    public string SourceAuthority => this._sourceAuthority;

    public Uri SourceUri => this._sourceUri;

    private static string ValidateChallenge(string challenge)
    {
      string str = !string.IsNullOrEmpty(challenge) ? challenge.Trim() : throw new ArgumentNullException(nameof (challenge));
      return str.StartsWith("Bearer ") ? str.Substring("Bearer".Length + 1) : throw new ArgumentException("Challenge is not Bearer", nameof (challenge));
    }

    private static string ValidateRequestURI(Uri requestUri)
    {
      if ((Uri) null == requestUri)
        throw new ArgumentNullException(nameof (requestUri));
      if (!requestUri.IsAbsoluteUri)
        throw new ArgumentException("The requestUri must be an absolute URI", nameof (requestUri));
      return requestUri.Scheme.Equals("http", StringComparison.CurrentCultureIgnoreCase) || requestUri.Scheme.Equals("https", StringComparison.CurrentCultureIgnoreCase) ? requestUri.FullAuthority() : throw new ArgumentException("The requestUri must be HTTP or HTTPS", nameof (requestUri));
    }
  }
}
