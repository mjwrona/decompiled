// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.SharedKeyCanonicalizer
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using System.Net;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public sealed class SharedKeyCanonicalizer
  {
    private const string SharedKeyAuthorizationScheme = "SharedKey";
    private static readonly SharedKeyCanonicalizer s_instance = new SharedKeyCanonicalizer();

    public static SharedKeyCanonicalizer Instance => SharedKeyCanonicalizer.s_instance;

    private SharedKeyCanonicalizer()
    {
    }

    public string AuthorizationScheme => "SharedKey";

    public string CanonicalizeHttpRequest(HttpWebRequest request, string accountName)
    {
      CanonicalizedString canonicalizedString = new CanonicalizedString(request.Method);
      canonicalizedString.AppendCanonicalizedElement(request.Headers[HttpRequestHeader.ContentEncoding]);
      canonicalizedString.AppendCanonicalizedElement(request.Headers[HttpRequestHeader.ContentLanguage]);
      AuthenticationUtility.AppendCanonicalizedContentLengthHeader(canonicalizedString, request);
      canonicalizedString.AppendCanonicalizedElement(request.Headers[HttpRequestHeader.ContentMd5]);
      canonicalizedString.AppendCanonicalizedElement(request.ContentType);
      AuthenticationUtility.AppendCanonicalizedDateHeader(canonicalizedString, request);
      canonicalizedString.AppendCanonicalizedElement(request.Headers[HttpRequestHeader.IfModifiedSince]);
      canonicalizedString.AppendCanonicalizedElement(request.Headers[HttpRequestHeader.IfMatch]);
      canonicalizedString.AppendCanonicalizedElement(request.Headers[HttpRequestHeader.IfNoneMatch]);
      canonicalizedString.AppendCanonicalizedElement(request.Headers[HttpRequestHeader.IfUnmodifiedSince]);
      canonicalizedString.AppendCanonicalizedElement(request.Headers[HttpRequestHeader.Range]);
      AuthenticationUtility.AppendCanonicalizedCustomHeaders(canonicalizedString, request);
      canonicalizedString.AppendCanonicalizedElement(AuthenticationUtility.GetCanonicalizedResourceString(request.RequestUri, accountName));
      return canonicalizedString.ToString();
    }
  }
}
