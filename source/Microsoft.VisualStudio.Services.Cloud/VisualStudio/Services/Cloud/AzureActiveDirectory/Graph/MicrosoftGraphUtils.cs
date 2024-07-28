// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MicrosoftGraphUtils
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  public class MicrosoftGraphUtils
  {
    private const string TraceArea = "MicrosoftGraphClient";
    private const string TraceLayer = "MicrosoftGraphUtils";

    public static string GetSignInAddressFromUpn(User user) => MicrosoftGraphUtils.GetSignInAddressFromEncodedUpn(user.UserPrincipalName) ?? user.UserPrincipalName;

    public static string GetSignInAddressFromEncodedUpn(string upn) => AadIdentityAccountNameUtils.GetSignInAddressFromEncodedUpn(upn);

    public static string GetGraphPageNextLink(
      IVssRequestContext requestcontext,
      IBaseRequest currentRequest,
      IBaseRequest nextRequest)
    {
      if (currentRequest == nextRequest)
        throw new ArgumentException("currentRequest can't be same as nextRequest");
      if (nextRequest != null)
      {
        HttpRequestMessage httpRequestMessage = nextRequest.GetHttpRequestMessage();
        if ((object) httpRequestMessage.RequestUri != null)
          return new Uri(MicrosoftGraphConstants.DummyMicrosoftGraphDomain, httpRequestMessage.RequestUri.PathAndQuery).AbsoluteUri;
        requestcontext.Trace(44750223, TraceLevel.Error, "MicrosoftGraphClient", nameof (MicrosoftGraphUtils), "'nextRequest' does not have a valid request uri");
      }
      return (string) null;
    }

    public static PagingTokenType GetRequestPagingTokenType(AadServiceRequest request)
    {
      if (request == null || !(request is AadServicePagedRequest))
        return PagingTokenType.None;
      string pagingToken = (request as AadServicePagedRequest).PagingToken;
      if (string.IsNullOrEmpty(pagingToken))
        return PagingTokenType.None;
      Uri result;
      if (Uri.TryCreate(pagingToken, UriKind.Absolute, out result))
        return PagingTokenType.MicrosoftGraph;
      return Uri.TryCreate(pagingToken, UriKind.Relative, out result) ? PagingTokenType.AadGraph : PagingTokenType.Invalid;
    }

    public static string GetSkipTokenFromGraphRequest(IBaseRequest request)
    {
      if (request == null)
        return (string) null;
      IList<QueryOption> queryOptions = request.QueryOptions;
      if (queryOptions == null)
        return (string) null;
      return ((Option) queryOptions.FirstOrDefault<QueryOption>((Func<QueryOption, bool>) (x => string.Equals(((Option) x).Name, "$skiptoken", StringComparison.OrdinalIgnoreCase))))?.Value;
    }
  }
}
