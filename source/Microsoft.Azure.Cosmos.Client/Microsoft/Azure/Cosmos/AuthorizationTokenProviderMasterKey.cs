// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.AuthorizationTokenProviderMasterKey
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using System;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class AuthorizationTokenProviderMasterKey : AuthorizationTokenProvider
  {
    private const string MacSignatureString = "to sign";
    private const string EnableAuthFailureTracesConfig = "enableAuthFailureTraces";
    private readonly Lazy<bool> enableAuthFailureTraces;
    private IComputeHash authKeyHashFunction;

    public AuthorizationTokenProviderMasterKey(IComputeHash computeHash)
    {
      this.authKeyHashFunction = computeHash ?? throw new ArgumentNullException(nameof (computeHash));
      this.enableAuthFailureTraces = new Lazy<bool>((Func<bool>) (() =>
      {
        if (Assembly.GetEntryAssembly() == (Assembly) null)
          return false;
        string appSetting = ConfigurationManager.AppSettings[nameof (enableAuthFailureTraces)];
        bool result;
        return !string.IsNullOrEmpty(appSetting) && bool.TryParse(appSetting, out result) && result;
      }));
    }

    public AuthorizationTokenProviderMasterKey(SecureString authKey)
      : this((IComputeHash) new SecureStringHMACSHA256Helper(authKey))
    {
    }

    public AuthorizationTokenProviderMasterKey(string authKey)
      : this((IComputeHash) new StringHMACSHA256Hash(authKey))
    {
    }

    public override ValueTask<(string token, string payload)> GetUserAuthorizationAsync(
      string resourceAddress,
      string resourceType,
      string requestVerb,
      INameValueCollection headers,
      AuthorizationTokenType tokenType)
    {
      headers["x-ms-date"] = Rfc1123DateTimeCache.UtcNow();
      AuthorizationHelper.ArrayOwner payload;
      string authorizationSignature = AuthorizationHelper.GenerateKeyAuthorizationSignature(requestVerb, resourceAddress, resourceType, headers, this.authKeyHashFunction, out payload);
      using (payload)
      {
        string str = (string) null;
        ArraySegment<byte> buffer = payload.Buffer;
        if (buffer.Count > 0)
        {
          Encoding utF8 = Encoding.UTF8;
          buffer = payload.Buffer;
          byte[] array = buffer.Array;
          buffer = payload.Buffer;
          int offset = buffer.Offset;
          buffer = payload.Buffer;
          int count = buffer.Count;
          str = utF8.GetString(array, offset, count);
        }
        return new ValueTask<(string, string)>((authorizationSignature, str));
      }
    }

    public override ValueTask<string> GetUserAuthorizationTokenAsync(
      string resourceAddress,
      string resourceType,
      string requestVerb,
      INameValueCollection headers,
      AuthorizationTokenType tokenType,
      ITrace trace)
    {
      headers["x-ms-date"] = Rfc1123DateTimeCache.UtcNow();
      AuthorizationHelper.ArrayOwner payload;
      string authorizationSignature = AuthorizationHelper.GenerateKeyAuthorizationSignature(requestVerb, resourceAddress, resourceType, headers, this.authKeyHashFunction, out payload);
      using (payload)
        return new ValueTask<string>(authorizationSignature);
    }

    public override ValueTask AddAuthorizationHeaderAsync(
      INameValueCollection headersCollection,
      Uri requestAddress,
      string verb,
      AuthorizationTokenType tokenType)
    {
      string str = Rfc1123DateTimeCache.UtcNow();
      headersCollection["x-ms-date"] = str;
      string authorizationSignature = AuthorizationHelper.GenerateKeyAuthorizationSignature(verb, requestAddress, headersCollection, this.authKeyHashFunction);
      headersCollection.Add("authorization", authorizationSignature);
      return new ValueTask();
    }

    public override void TraceUnauthorized(
      DocumentClientException dce,
      string authorizationToken,
      string payload)
    {
      if (payload == null || dce.Message == null || !dce.StatusCode.HasValue || dce.StatusCode.Value != HttpStatusCode.Unauthorized || !dce.Message.Contains("to sign"))
        return;
      string str1 = AuthorizationTokenProviderMasterKey.NormalizeAuthorizationPayload(payload);
      if (this.enableAuthFailureTraces.Value)
      {
        string str2 = HttpUtility.UrlDecode(authorizationToken).Split('&')[2].Split('=')[1].Substring(0, 5);
        ulong num = 0;
        if (this.authKeyHashFunction?.Key != null)
        {
          byte[] bytes = Encoding.UTF8.GetBytes(this.authKeyHashFunction?.Key?.ToString());
          num = Microsoft.Azure.Documents.Routing.MurmurHash3.Hash64(bytes, bytes.Length);
        }
        DefaultTrace.TraceError("Un-expected authorization payload mis-match. Actual payload={0}, token={1}..., hash={2:X}..., error={3}", (object) str1, (object) str2, (object) num, (object) dce.Message);
      }
      else
        DefaultTrace.TraceError("Un-expected authorization payload mis-match. Actual {0} service expected {1}", (object) str1, (object) dce.Message);
    }

    public override void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private static string NormalizeAuthorizationPayload(string input)
    {
      StringBuilder stringBuilder = new StringBuilder(input.Length + 12);
      for (int index = 0; index < input.Length; ++index)
      {
        switch (input[index])
        {
          case '\n':
            stringBuilder.Append("\\n");
            break;
          case '/':
            stringBuilder.Append("\\/");
            break;
          default:
            stringBuilder.Append(input[index]);
            break;
        }
      }
      return stringBuilder.ToString();
    }

    private void Dispose(bool disposing)
    {
      this.authKeyHashFunction?.Dispose();
      this.authKeyHashFunction = (IComputeHash) null;
    }

    ~AuthorizationTokenProviderMasterKey() => this.Dispose(false);
  }
}
