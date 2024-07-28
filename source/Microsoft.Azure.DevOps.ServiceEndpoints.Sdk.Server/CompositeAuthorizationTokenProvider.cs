// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.CompositeAuthorizationTokenProvider
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  internal class CompositeAuthorizationTokenProvider : IAuthorizationTokenProvider
  {
    private readonly IAuthorizationTokenProvider[] _providers;

    public CompositeAuthorizationTokenProvider(IAuthorizationTokenProvider[] providers) => this._providers = providers;

    public bool CanProcess(HttpWebRequest request) => ((IEnumerable<IAuthorizationTokenProvider>) this._providers).Any<IAuthorizationTokenProvider>((Func<IAuthorizationTokenProvider, bool>) (provider => provider.CanProcess(request)));

    public string GetToken(HttpWebRequest request, string resourceUrl) => ((IEnumerable<IAuthorizationTokenProvider>) this._providers).FirstOrDefault<IAuthorizationTokenProvider>((Func<IAuthorizationTokenProvider, bool>) (p => p.CanProcess(request)))?.GetToken(request, resourceUrl);
  }
}
