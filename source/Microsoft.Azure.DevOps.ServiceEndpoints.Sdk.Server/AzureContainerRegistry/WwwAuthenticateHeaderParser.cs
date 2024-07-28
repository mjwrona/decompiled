// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureContainerRegistry.WwwAuthenticateHeaderParser
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureContainerRegistry
{
  internal class WwwAuthenticateHeaderParser : IWwwAuthenticateHeaderParser
  {
    private readonly IAuthenticateOptionsParser _optionsParser;

    public WwwAuthenticateHeaderParser(IAuthenticateOptionsParser optionsParser)
    {
      ArgumentUtility.CheckForNull<IAuthenticateOptionsParser>(optionsParser, nameof (optionsParser));
      this._optionsParser = optionsParser;
    }

    public IEnumerable<AuthenticationChallenge> Parse(string header) => this.GetWwwAuthenticateHeaders(header).Select<AuthenticationHeaderValue, AuthenticationChallenge>(new Func<AuthenticationHeaderValue, AuthenticationChallenge>(this.ToAuthenticationChallenge));

    private AuthenticationChallenge ToAuthenticationChallenge(AuthenticationHeaderValue header) => new AuthenticationChallenge()
    {
      Scheme = header.Scheme,
      Options = this._optionsParser.Parse(header.Parameter)
    };

    private IEnumerable<AuthenticationHeaderValue> GetWwwAuthenticateHeaders(string header)
    {
      HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
      httpResponseMessage.Headers.Add("WWW-Authenticate", header);
      return (IEnumerable<AuthenticationHeaderValue>) httpResponseMessage.Headers.WwwAuthenticate;
    }
  }
}
