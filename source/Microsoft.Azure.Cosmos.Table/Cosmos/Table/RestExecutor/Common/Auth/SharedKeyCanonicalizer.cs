// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.Common.Auth.SharedKeyCanonicalizer
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Net.Http;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.Common.Auth
{
  internal sealed class SharedKeyCanonicalizer : ICanonicalizer
  {
    private const string SharedKeyAuthorizationScheme = "SharedKey";
    private static SharedKeyCanonicalizer instance = new SharedKeyCanonicalizer();

    public static SharedKeyCanonicalizer Instance => SharedKeyCanonicalizer.instance;

    private SharedKeyCanonicalizer()
    {
    }

    public string AuthorizationScheme => "SharedKey";

    public string CanonicalizeHttpRequest(HttpRequestMessage request, string accountName)
    {
      CommonUtility.AssertNotNull(nameof (request), (object) request);
      CanonicalizedString canonicalizedString = new CanonicalizedString(request.Method.Method);
      if (request.Content != null && request.Content.Headers.ContentMD5 != null)
        canonicalizedString.AppendCanonicalizedElement(Convert.ToBase64String(request.Content.Headers.ContentMD5));
      else
        canonicalizedString.AppendCanonicalizedElement((string) null);
      if (request.Content != null && request.Content.Headers.ContentType != null)
        canonicalizedString.AppendCanonicalizedElement(request.Content.Headers.ContentType.ToString());
      else
        canonicalizedString.AppendCanonicalizedElement((string) null);
      AuthenticationUtility.AppendCanonicalizedDateHeader(canonicalizedString, request, true);
      string canonicalizedResourceString = AuthenticationUtility.GetCanonicalizedResourceString(request.RequestUri, accountName);
      canonicalizedString.AppendCanonicalizedElement(canonicalizedResourceString);
      return canonicalizedString.ToString();
    }
  }
}
