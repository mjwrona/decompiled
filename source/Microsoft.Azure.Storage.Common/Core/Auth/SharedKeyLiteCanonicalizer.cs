// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Auth.SharedKeyLiteCanonicalizer
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Net.Http;

namespace Microsoft.Azure.Storage.Core.Auth
{
  public sealed class SharedKeyLiteCanonicalizer : ICanonicalizer
  {
    private const string SharedKeyLiteAuthorizationScheme = "SharedKeyLite";
    private const int ExpectedCanonicalizedStringLength = 250;
    private static SharedKeyLiteCanonicalizer instance = new SharedKeyLiteCanonicalizer();

    public static SharedKeyLiteCanonicalizer Instance => SharedKeyLiteCanonicalizer.instance;

    private SharedKeyLiteCanonicalizer()
    {
    }

    public string AuthorizationScheme => "SharedKeyLite";

    public string CanonicalizeHttpRequest(HttpRequestMessage request, string accountName)
    {
      CommonUtility.AssertNotNull(nameof (request), (object) request);
      CanonicalizedString canonicalizedString = new CanonicalizedString(request.Method.Method, 250);
      if (request.Content != null)
      {
        canonicalizedString.AppendCanonicalizedElement(request.Content.Headers.ContentMD5 == null ? (string) null : Convert.ToBase64String(request.Content.Headers.ContentMD5));
        canonicalizedString.AppendCanonicalizedElement(request.Content.Headers.ContentType == null ? (string) null : request.Content.Headers.ContentType.ToString());
      }
      else
      {
        canonicalizedString.AppendCanonicalizedElement((string) null);
        canonicalizedString.AppendCanonicalizedElement((string) null);
      }
      AuthenticationUtility.AppendCanonicalizedDateHeader(canonicalizedString, request);
      AuthenticationUtility.AppendCanonicalizedCustomHeaders(canonicalizedString, request);
      string canonicalizedResourceString = AuthenticationUtility.GetCanonicalizedResourceString(request.RequestUri, accountName, true);
      canonicalizedString.AppendCanonicalizedElement(canonicalizedResourceString);
      return canonicalizedString.ToString();
    }
  }
}
