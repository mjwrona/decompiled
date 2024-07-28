// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.TokenBasedEndpointAuthorizer
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  public class TokenBasedEndpointAuthorizer : IEndpointAuthorizer
  {
    private readonly ServiceEndpoint serviceEndpoint;
    private readonly List<AuthorizationHeader> authorizationHeaders;
    private readonly int m_httpTimeoutInSeconds = 100;

    public bool SupportsAbsoluteEndpoint => true;

    public string GetEndpointUrl() => this.serviceEndpoint.Url.AbsoluteUri;

    public string GetServiceEndpointType() => this.serviceEndpoint.Type;

    public TokenBasedEndpointAuthorizer(
      ServiceEndpoint serviceEndpoint,
      List<AuthorizationHeader> authorizationHeaders)
    {
      this.serviceEndpoint = serviceEndpoint;
      this.authorizationHeaders = authorizationHeaders;
    }

    public void AuthorizeRequest(HttpWebRequest request, string resourceUrl)
    {
      if (!string.IsNullOrEmpty(resourceUrl))
        throw new InvalidOperationException(Resources.ResourceUrlNotSupported((object) this.serviceEndpoint.Type, (object) this.serviceEndpoint.Authorization.Scheme));
      Dictionary<string, string> dictionary1 = this.serviceEndpoint.Data.Union<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) this.serviceEndpoint.Authorization.Parameters).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (k => k.Key), (Func<KeyValuePair<string, string>, string>) (v => v.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, string> dictionary2 = new Dictionary<string, string>()
      {
        {
          "Method",
          request.Method
        },
        {
          "Host",
          request.Host
        },
        {
          "AbsolutePath",
          request.RequestUri.AbsolutePath
        },
        {
          "Query",
          request.RequestUri.Query
        }
      };
      JObject replacementContext = JObject.FromObject((object) new
      {
        endpoint = dictionary1,
        request = dictionary2
      });
      MustacheTemplateEngine mustacheTemplateEngine = new MustacheTemplateEngine();
      mustacheTemplateEngine.RegisterHelper("#getCurrentUTCDate", new MustacheTemplateHelperMethod(this.GetCurrentUTCDateHelper));
      mustacheTemplateEngine.RegisterHelper("#getHashedPayload", new MustacheTemplateHelperMethod(this.GetHashedPayloadHelper));
      mustacheTemplateEngine.RegisterHelper("#getAuthorizationHeader", new MustacheTemplateHelperMethod(this.GetAuthorizationHeaderHelper));
      mustacheTemplateEngine.RegisterHelper("#getTokenUsingBasicAuth", new MustacheTemplateHelperMethod(this.GetTokenUsingBasicAuthHelper));
      foreach (AuthorizationHeader authorizationHeader in this.authorizationHeaders)
      {
        string name = authorizationHeader.Name.Trim();
        string template = mustacheTemplateEngine.EvaluateTemplate(authorizationHeader.Value.Trim(), (JToken) replacementContext);
        if (name.Equals("Accept", StringComparison.OrdinalIgnoreCase))
          request.Accept = template;
        else if (name.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
          request.ContentType = template;
        else
          request.Headers[name] = template;
      }
    }

    private string GetCurrentUTCDateHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      return DateTime.UtcNow.ToString("yyyyMMddTHHmmss") + "Z";
    }

    private static byte[] ToBytes(string str) => Encoding.UTF8.GetBytes(str);

    private static string HexEncode(byte[] bytes) => HexConverter.ToStringLowerCase(bytes);

    private byte[] Hash(byte[] bytes) => SHA256.Create().ComputeHash(bytes);

    private byte[] HmacSha256(string data, byte[] key) => new HMACSHA256(key).ComputeHash(TokenBasedEndpointAuthorizer.ToBytes(data));

    private byte[] GetSignatureKey(
      string key,
      string dateStamp,
      string regionName,
      string serviceName)
    {
      byte[] key1 = this.HmacSha256(dateStamp, TokenBasedEndpointAuthorizer.ToBytes("AWS4" + key));
      byte[] key2 = this.HmacSha256(regionName, key1);
      return this.HmacSha256("aws4_request", this.HmacSha256(serviceName, key2));
    }

    private string GetHashedPayloadHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      return TokenBasedEndpointAuthorizer.HexEncode(this.Hash(TokenBasedEndpointAuthorizer.ToBytes("")));
    }

    private string GetAuthorizationHeaderHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      BearerTokenArgument bearerTokenArgument = JsonConvert.DeserializeObject<BearerTokenArgument>(expression.Expression.Trim());
      if (!string.Equals(bearerTokenArgument.Method, "GET", StringComparison.OrdinalIgnoreCase) && !string.Equals(bearerTokenArgument.Method, "POST", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException(Resources.InvalidMethod((object) bearerTokenArgument.Method));
      string userName = bearerTokenArgument?.GetUserName(this.serviceEndpoint);
      string password = bearerTokenArgument?.GetPassword(this.serviceEndpoint);
      JObject jobject = JObject.FromObject(context.ReplacementObject);
      string regionName = jobject["endpoint"][(object) "region"].ToString();
      string str1 = jobject["request"][(object) "Method"].ToString();
      string str2 = jobject["request"][(object) "AbsolutePath"].ToString();
      string str3 = jobject["request"][(object) "Host"].ToString();
      string str4 = jobject["request"][(object) "Query"].ToString();
      string str5 = str4;
      if (!string.IsNullOrEmpty(str4) && str4.StartsWith("?"))
        str5 = str4.Substring(1);
      DateTime utcNow = DateTime.UtcNow;
      string str6 = "AWS4-HMAC-SHA256";
      string serviceName = "s3";
      string str7 = "";
      string str8 = TokenBasedEndpointAuthorizer.HexEncode(this.Hash(TokenBasedEndpointAuthorizer.ToBytes(str7)));
      string dateStamp = utcNow.ToString("yyyyMMdd");
      string str9 = utcNow.ToString("yyyyMMddTHHmmss") + "Z";
      string str10 = string.Format("{0}/{1}/{2}/aws4_request", (object) dateStamp, (object) regionName, (object) serviceName);
      TokenBasedEndpointAuthorizer.ToBytes(str7);
      SortedDictionary<string, string> source = new SortedDictionary<string, string>()
      {
        {
          "host",
          str3
        },
        {
          "x-amz-content-sha256",
          str8
        },
        {
          "x-amz-date",
          str9
        }
      };
      string str11 = string.Join("\n", source.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (x => x.Key.ToLowerInvariant() + ":" + x.Value.Trim()))) + "\n";
      string str12 = string.Join(";", source.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (x => x.Key.ToLowerInvariant())));
      string str13 = TokenBasedEndpointAuthorizer.HexEncode(this.Hash(TokenBasedEndpointAuthorizer.ToBytes(str1 + "\n" + str2 + "\n" + str5 + "\n" + str11 + "\n" + str12 + "\n" + str8)));
      string str14 = TokenBasedEndpointAuthorizer.HexEncode(this.HmacSha256(string.Format("{0}\n{1}\n{2}\n{3}", (object) str6, (object) str9, (object) str10, (object) str13), this.GetSignatureKey(password, dateStamp, regionName, serviceName)));
      return string.Format("{0} Credential={1}/{2}/{3}/{4}/aws4_request, SignedHeaders={5}, Signature={6}", (object) str6, (object) userName, (object) dateStamp, (object) regionName, (object) serviceName, (object) str12, (object) str14);
    }

    private string GetTokenUsingBasicAuthHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      BearerTokenArgument bearerTokenArgument = JsonConvert.DeserializeObject<BearerTokenArgument>(expression.Expression.Trim());
      string authServerUrl = bearerTokenArgument.GetAuthServerUrl(this.serviceEndpoint);
      string body = bearerTokenArgument.GetBody(this.serviceEndpoint);
      if (!string.Equals(bearerTokenArgument.Method, "GET", StringComparison.OrdinalIgnoreCase) && !string.Equals(bearerTokenArgument.Method, "POST", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException(Resources.InvalidMethod((object) bearerTokenArgument.Method));
      if (!string.Equals(bearerTokenArgument.ContentType, "application/json", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException(Resources.InvalidContentTypeArgument((object) bearerTokenArgument.ContentType));
      if (body.Length > BearerTokenArgument.MaxBodySizeSupported)
        throw new InvalidOperationException(Resources.InvalidBodyArgument((object) BearerTokenArgument.MaxBodySizeSupported));
      HttpWebRequest httpWebRequest = WebRequest.Create(authServerUrl) as HttpWebRequest;
      httpWebRequest.Method = bearerTokenArgument.Method;
      httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(bearerTokenArgument.GetUserName(this.serviceEndpoint) + ":" + bearerTokenArgument.GetPassword(this.serviceEndpoint)));
      httpWebRequest.ContentType = bearerTokenArgument.ContentType;
      if (string.Equals(bearerTokenArgument.Method, "POST", StringComparison.OrdinalIgnoreCase))
      {
        using (Stream result = httpWebRequest.GetRequestStreamAsync().Result)
        {
          byte[] bytes = Encoding.UTF8.GetBytes(body);
          result.Write(bytes, 0, body.Length);
        }
      }
      HttpWebResponse httpWebResponse = (HttpWebResponse) null;
      try
      {
        httpWebResponse = httpWebRequest.GetResponseAsync().Result as HttpWebResponse;
      }
      catch (Exception ex) when (ex is WebException || ex.InnerException is WebException)
      {
        if (!bearerTokenArgument.IgnoreError)
        {
          WebException webException = !(ex is WebException) ? ex.InnerException as WebException : ex as WebException;
          if (webException.Response != null)
          {
            using (WebResponse response = webException.Response)
            {
              using (Stream responseStream = response.GetResponseStream())
              {
                using (StreamReader streamReader = new StreamReader(responseStream))
                  throw new InvalidAuthorizationDetailsException(streamReader.ReadToEnd());
              }
            }
          }
        }
      }
      string empty = string.Empty;
      if (httpWebResponse != null && httpWebResponse.ContentLength > (long) BearerTokenArgument.MaxBodySizeSupported && !bearerTokenArgument.IgnoreError)
        throw new InvalidOperationException(Resources.InvalidResponseSize((object) BearerTokenArgument.MaxBodySizeSupported));
      if (httpWebResponse != null && httpWebResponse.StatusCode == HttpStatusCode.OK)
      {
        string json;
        using (Stream responseStream = httpWebResponse.GetResponseStream())
        {
          Task<string> endAsync = new StreamReader(responseStream, Encoding.UTF8).ReadToEndAsync();
          json = endAsync.Wait(this.m_httpTimeoutInSeconds * 1000) ? endAsync.Result : throw new InvalidOperationException(Resources.HttpTimeoutException((object) this.m_httpTimeoutInSeconds));
        }
        empty = JToken.Parse(json).SelectToken(bearerTokenArgument.GetResultSelector()).ToString();
      }
      return empty;
    }
  }
}
