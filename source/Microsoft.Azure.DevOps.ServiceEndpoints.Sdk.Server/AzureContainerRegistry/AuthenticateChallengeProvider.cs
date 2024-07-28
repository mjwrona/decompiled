// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureContainerRegistry.AuthenticateChallengeProvider
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureContainerRegistry
{
  internal class AuthenticateChallengeProvider : IAuthenticateChallengeProvider
  {
    private readonly IWwwAuthenticateHeaderParser _parser;

    public AuthenticateChallengeProvider(IWwwAuthenticateHeaderParser parser)
    {
      ArgumentUtility.CheckForNull<IWwwAuthenticateHeaderParser>(parser, nameof (parser));
      this._parser = parser;
    }

    public IEnumerable<AuthenticationChallenge> GetChallenges(HttpWebRequest request)
    {
      try
      {
        request.GetResponse();
      }
      catch (WebException ex)
      {
        if (!(ex.Response is HttpWebResponse response))
          throw;
        else if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
          throw;
        }
        else
        {
          string[] values = response.Headers.GetValues("WWW-Authenticate");
          return values == null ? Enumerable.Empty<AuthenticationChallenge>() : ((IEnumerable<string>) values).SelectMany<string, AuthenticationChallenge>(new Func<string, IEnumerable<AuthenticationChallenge>>(this._parser.Parse));
        }
      }
      return Enumerable.Empty<AuthenticationChallenge>();
    }
  }
}
