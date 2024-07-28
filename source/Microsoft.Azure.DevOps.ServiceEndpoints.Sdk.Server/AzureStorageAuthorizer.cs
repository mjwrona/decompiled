// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureStorageAuthorizer
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.VisualStudio.Services.ServiceEndpoints.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  internal class AzureStorageAuthorizer : IEndpointAuthorizer
  {
    protected readonly ServiceEndpoint _serviceEndpoint;
    private const string versionHeader = "x-ms-version";
    private const string versionHeaderValue = "2017-04-17";
    private const string dateHeader = "x-ms-date";
    private const string authorizationHeader = "Authorization";
    private Func<string> GetCurrentTime;

    public AzureStorageAuthorizer(ServiceEndpoint serviceEndpoint)
    {
      this._serviceEndpoint = serviceEndpoint;
      this.GetCurrentTime = (Func<string>) (() => DateTime.UtcNow.ToString("R"));
    }

    protected AzureStorageAuthorizer(ServiceEndpoint serviceEndpoint, Func<string> getCurrentTime)
    {
      this._serviceEndpoint = serviceEndpoint;
      this.GetCurrentTime = getCurrentTime;
    }

    public void AuthorizeRequest(HttpWebRequest request, string resourceUrl)
    {
      Dictionary<string, string> dictionary = this._serviceEndpoint.Data.Union<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) this._serviceEndpoint.Authorization.Parameters).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (k => k.Key), (Func<KeyValuePair<string, string>, string>) (v => v.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      string storageAccessKey;
      dictionary.TryGetValue("StorageAccessKey", out storageAccessKey);
      string storageAccountName;
      dictionary.TryGetValue("StorageAccountName", out storageAccountName);
      if (string.IsNullOrEmpty(storageAccessKey) || string.IsNullOrEmpty(storageAccountName))
        throw new InvalidOperationException(ServiceEndpointSdkResources.NoStorageAccountDetails());
      this.AddRequiredHeadersForAuthentication(request, storageAccessKey, storageAccountName);
    }

    public string GetEndpointUrl() => this._serviceEndpoint.Url.AbsoluteUri;

    public string GetServiceEndpointType() => this._serviceEndpoint.Type;

    public bool SupportsAbsoluteEndpoint => true;

    private void AddRequiredHeadersForAuthentication(
      HttpWebRequest request,
      string storageAccessKey,
      string storageAccountName)
    {
      if (((IEnumerable<string>) request.Headers.AllKeys).Contains<string>("Authorization"))
        request.Headers.Remove("Authorization");
      request.Headers["x-ms-date"] = this.GetCurrentTime();
      request.Headers["x-ms-version"] = "2017-04-17";
      string signature = this.GenerateSignature(SharedKeyCanonicalizer.Instance.CanonicalizeHttpRequest(request, storageAccountName), storageAccessKey, storageAccountName);
      request.Headers["Authorization"] = signature;
    }

    private string GenerateSignature(string stringToSign, string accessKey, string account)
    {
      string str = string.Empty;
      using (HMACSHA256 hmacshA256 = new HMACSHA256(Convert.FromBase64String(accessKey)))
      {
        byte[] bytes = Encoding.UTF8.GetBytes(stringToSign);
        str = Convert.ToBase64String(hmacshA256.ComputeHash(bytes));
      }
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}:{2}", (object) "SharedKey", (object) account, (object) str);
    }
  }
}
