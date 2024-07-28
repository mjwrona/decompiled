// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Auth.SharedKeyCanonicalizer
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.Azure.Storage.Core.Auth
{
  public sealed class SharedKeyCanonicalizer : ICanonicalizer
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
      if (request.Content != null)
      {
        canonicalizedString.AppendCanonicalizedElement(HttpWebUtility.CombineHttpHeaderValues((IEnumerable<string>) request.Content.Headers.ContentEncoding));
        canonicalizedString.AppendCanonicalizedElement(HttpWebUtility.CombineHttpHeaderValues((IEnumerable<string>) request.Content.Headers.ContentLanguage));
        AuthenticationUtility.AppendCanonicalizedContentLengthHeader(canonicalizedString, request);
        canonicalizedString.AppendCanonicalizedElement(request.Content.Headers.ContentMD5 == null ? (string) null : Convert.ToBase64String(request.Content.Headers.ContentMD5));
        canonicalizedString.AppendCanonicalizedElement(request.Content.Headers.ContentType == null ? (string) null : request.Content.Headers.ContentType.ToString());
      }
      else
      {
        canonicalizedString.AppendCanonicalizedElement((string) null);
        canonicalizedString.AppendCanonicalizedElement((string) null);
        canonicalizedString.AppendCanonicalizedElement((string) null);
        canonicalizedString.AppendCanonicalizedElement((string) null);
        canonicalizedString.AppendCanonicalizedElement((string) null);
      }
      AuthenticationUtility.AppendCanonicalizedDateHeader(canonicalizedString, request);
      canonicalizedString.AppendCanonicalizedElement(AuthenticationUtility.GetCanonicalizedHeaderValue(request.Headers.IfModifiedSince));
      canonicalizedString.AppendCanonicalizedElement(CommonUtility.GetFirstHeaderValue<EntityTagHeaderValue>((IEnumerable<EntityTagHeaderValue>) request.Headers.IfMatch));
      canonicalizedString.AppendCanonicalizedElement(CommonUtility.GetFirstHeaderValue<EntityTagHeaderValue>((IEnumerable<EntityTagHeaderValue>) request.Headers.IfNoneMatch));
      canonicalizedString.AppendCanonicalizedElement(AuthenticationUtility.GetCanonicalizedHeaderValue(request.Headers.IfUnmodifiedSince));
      canonicalizedString.AppendCanonicalizedElement(request.Headers.Range == null ? (string) null : CommonUtility.GetFirstHeaderValue<RangeItemHeaderValue>((IEnumerable<RangeItemHeaderValue>) request.Headers.Range.Ranges));
      AuthenticationUtility.AppendCanonicalizedCustomHeaders(canonicalizedString, request);
      string canonicalizedResourceString = AuthenticationUtility.GetCanonicalizedResourceString(request.RequestUri, accountName);
      canonicalizedString.AppendCanonicalizedElement(canonicalizedResourceString);
      return canonicalizedString.ToString();
    }
  }
}
