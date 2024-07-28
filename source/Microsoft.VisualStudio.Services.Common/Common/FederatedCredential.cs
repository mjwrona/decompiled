// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.FederatedCredential
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.Common
{
  [Serializable]
  public abstract class FederatedCredential : IssuedTokenCredential
  {
    protected FederatedCredential(IssuedToken initialToken)
      : base(initialToken)
    {
    }

    public override bool IsAuthenticationChallenge(IHttpResponse webResponse)
    {
      if (webResponse == null)
        return false;
      return webResponse.StatusCode == HttpStatusCode.Found || webResponse.StatusCode == HttpStatusCode.Found ? webResponse.Headers.GetValues("X-TFS-FedAuthRealm").Any<string>() : webResponse.StatusCode == HttpStatusCode.Unauthorized;
    }
  }
}
