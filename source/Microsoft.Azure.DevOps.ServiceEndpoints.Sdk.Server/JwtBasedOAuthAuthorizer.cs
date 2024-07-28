// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.JwtBasedOAuthAuthorizer
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.InformationProtection.X509;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public class JwtBasedOAuthAuthorizer : IEndpointAuthorizer
  {
    private const int ClientTimeoutInSeconds = 30;
    private readonly ServiceEndpoint _endPoint;
    private readonly Func<string, string, string> _retreiveAuthorizationToken;
    private readonly IVssRequestContext _context;

    public bool SupportsAbsoluteEndpoint => true;

    public JwtBasedOAuthAuthorizer(ServiceEndpoint endPoint, IVssRequestContext context)
      : this(endPoint, context, JwtBasedOAuthAuthorizer.\u003C\u003EO.\u003C0\u003E__FetchAuthorizationToken ?? (JwtBasedOAuthAuthorizer.\u003C\u003EO.\u003C0\u003E__FetchAuthorizationToken = new Func<string, string, string>(JwtBasedOAuthAuthorizer.FetchAuthorizationToken)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    protected internal JwtBasedOAuthAuthorizer(
      ServiceEndpoint endPoint,
      IVssRequestContext context,
      Func<string, string, string> retreiveAuthorizationToken)
    {
      this._endPoint = endPoint;
      this._context = context;
      this._retreiveAuthorizationToken = retreiveAuthorizationToken;
    }

    public string GetEndpointUrl() => this._endPoint.Url.AbsoluteUri;

    public string GetServiceEndpointType() => this._endPoint.Type;

    public void AuthorizeRequest(HttpWebRequest request, string resourceUrl)
    {
      string authorizationParameter1 = this.GetAuthorizationParameter("Issuer");
      string authorizationParameter2 = this.GetAuthorizationParameter("PrivateKey");
      string authorizationParameter3 = this.GetAuthorizationParameter("Audience");
      string authorizationParameter4 = this.GetAuthorizationParameter("Scope");
      object accessToken = this.GetAccessToken(authorizationParameter1, authorizationParameter2, authorizationParameter4, authorizationParameter3);
      // ISSUE: reference to a compiler-generated field
      if (JwtBasedOAuthAuthorizer.\u003C\u003Eo__10.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        JwtBasedOAuthAuthorizer.\u003C\u003Eo__10.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (JwtBasedOAuthAuthorizer), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, bool> target1 = JwtBasedOAuthAuthorizer.\u003C\u003Eo__10.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, bool>> p1 = JwtBasedOAuthAuthorizer.\u003C\u003Eo__10.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (JwtBasedOAuthAuthorizer.\u003C\u003Eo__10.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        JwtBasedOAuthAuthorizer.\u003C\u003Eo__10.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (JwtBasedOAuthAuthorizer), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj1 = JwtBasedOAuthAuthorizer.\u003C\u003Eo__10.\u003C\u003Ep__0.Target((CallSite) JwtBasedOAuthAuthorizer.\u003C\u003Eo__10.\u003C\u003Ep__0, accessToken, (object) null);
      if (!target1((CallSite) p1, obj1))
        return;
      // ISSUE: reference to a compiler-generated field
      if (JwtBasedOAuthAuthorizer.\u003C\u003Eo__10.\u003C\u003Ep__3 == null)
      {
        // ISSUE: reference to a compiler-generated field
        JwtBasedOAuthAuthorizer.\u003C\u003Eo__10.\u003C\u003Ep__3 = CallSite<Func<CallSite, string, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Add, typeof (JwtBasedOAuthAuthorizer), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, string, object, object> target2 = JwtBasedOAuthAuthorizer.\u003C\u003Eo__10.\u003C\u003Ep__3.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, string, object, object>> p3 = JwtBasedOAuthAuthorizer.\u003C\u003Eo__10.\u003C\u003Ep__3;
      // ISSUE: reference to a compiler-generated field
      if (JwtBasedOAuthAuthorizer.\u003C\u003Eo__10.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        JwtBasedOAuthAuthorizer.\u003C\u003Eo__10.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof (JwtBasedOAuthAuthorizer), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = JwtBasedOAuthAuthorizer.\u003C\u003Eo__10.\u003C\u003Ep__2.Target((CallSite) JwtBasedOAuthAuthorizer.\u003C\u003Eo__10.\u003C\u003Ep__2, accessToken, "access_token");
      object obj3 = target2((CallSite) p3, "Bearer ", obj2);
      // ISSUE: reference to a compiler-generated field
      if (JwtBasedOAuthAuthorizer.\u003C\u003Eo__10.\u003C\u003Ep__4 == null)
      {
        // ISSUE: reference to a compiler-generated field
        JwtBasedOAuthAuthorizer.\u003C\u003Eo__10.\u003C\u003Ep__4 = CallSite<Action<CallSite, WebHeaderCollection, HttpRequestHeader, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "Add", (IEnumerable<Type>) null, typeof (JwtBasedOAuthAuthorizer), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      JwtBasedOAuthAuthorizer.\u003C\u003Eo__10.\u003C\u003Ep__4.Target((CallSite) JwtBasedOAuthAuthorizer.\u003C\u003Eo__10.\u003C\u003Ep__4, request.Headers, HttpRequestHeader.Authorization, obj3);
    }

    private object GetAccessToken(string issuer, string pemkey, string scope, string audience)
    {
      RSAParameters rsaXmlKey = RSAConversions.Pkcs8ToRsaParameters(Convert.FromBase64String(CertificateHelper.ExtractPrivateKey(pemkey)));
      VssSigningCredentials credentials = VssSigningCredentials.Create((Func<RSACryptoServiceProvider>) (() =>
      {
        RSACryptoServiceProvider accessToken = new RSACryptoServiceProvider();
        accessToken.ImportParameters(rsaXmlKey);
        return accessToken;
      }));
      List<Claim> additionalClaims = new List<Claim>()
      {
        new Claim(nameof (scope), scope)
      };
      JsonWebToken jsonWebToken = JsonWebToken.Create(issuer, audience, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(60.0), DateTime.UtcNow, (IEnumerable<Claim>) additionalClaims, credentials);
      Dictionary<string, string> dictionary = this.DeserializeResponse(this._retreiveAuthorizationToken(audience, jsonWebToken.EncodedToken));
      return dictionary == null || !dictionary.ContainsKey("error") ? (object) dictionary : throw new ServiceEndpointQueryFailedException(ServiceEndpointSdkResources.AuthorizationTokenFetchFailed((object) dictionary["error"], (object) dictionary["error_description"]));
    }

    private Dictionary<string, string> DeserializeResponse(string response)
    {
      if (this._context.IsFeatureEnabled("ServiceEndpoints.DisableJavaScriptSerializerForJwt"))
        return JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
      if (!this._context.IsFeatureEnabled("ServiceEndpoints.EnableSerializerTransitionMode"))
        return new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(response);
      JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
      Stopwatch stopwatch1 = Stopwatch.StartNew();
      Dictionary<string, string> result = scriptSerializer.Deserialize<Dictionary<string, string>>(response);
      stopwatch1.Stop();
      try
      {
        Stopwatch stopwatch2 = Stopwatch.StartNew();
        Dictionary<string, string> source = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
        stopwatch2.Stop();
        this._context.TraceVerbose(34000227, "ServiceEndpoints", "Serialization time difference. Old serializer: {0} ms; New serialzier: {1} ms", (object) stopwatch1.ElapsedMilliseconds, (object) stopwatch2.ElapsedMilliseconds);
        if (result.Count == source.Count)
        {
          string str;
          if (source.All<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (kvp => result.TryGetValue(kvp.Key, out str) && str == kvp.Value)))
            goto label_8;
        }
        this._context.TraceError(34000227, "ServiceEndpoints", "New JWT deserializer result differs from old deserializer result. Old keys: {0}; New keys: {1}", (object) string.Join(" ", (IEnumerable<string>) result.Keys), (object) string.Join(" ", (IEnumerable<string>) source.Keys));
      }
      catch (Exception ex)
      {
        this._context.TraceException(34000227, "ServiceEndpoints", ex);
      }
label_8:
      return result;
    }

    private static string FetchAuthorizationToken(string audience, string jsonToken)
    {
      HttpClient httpClient = new HttpClient()
      {
        Timeout = TimeSpan.FromSeconds(30.0)
      };
      Dictionary<string, string> nameValueCollection = new Dictionary<string, string>()
      {
        {
          "assertion",
          jsonToken
        },
        {
          "grant_type",
          "urn:ietf:params:oauth:grant-type:jwt-bearer"
        }
      };
      try
      {
        byte[] bytes = Microsoft.VisualStudio.Services.WebApi.TaskExtensions.SyncResult(httpClient.PostAsync(audience, (HttpContent) new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>) nameValueCollection))).Content.ReadAsByteArrayAsync().SyncResult<byte[]>();
        return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
      }
      catch (TaskCanceledException ex)
      {
        throw new ServiceEndpointQueryFailedException(ServiceEndpointSdkResources.AuthorizationTokenFetchTimeout((object) 30));
      }
    }

    private string GetAuthorizationParameter(string parameterName)
    {
      if (this._endPoint.Authorization.Parameters.ContainsKey(parameterName))
        return this._endPoint.Authorization.Parameters[parameterName];
      throw new ServiceEndpointQueryFailedException(ServiceEndpointSdkResources.AuthorizationParameterNotFound((object) parameterName));
    }
  }
}
