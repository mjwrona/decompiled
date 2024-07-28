// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.FederatedCredential
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Net;

namespace Microsoft.TeamFoundation.Client
{
  [Obsolete("This class is deprecated and will be removed in a future release. See Microsoft.VisualStudio.Services.Common.FederatedCredential instead.", false)]
  [Serializable]
  public abstract class FederatedCredential : IssuedTokenCredential
  {
    internal FederatedCredential(IssuedToken initialToken)
      : base(initialToken)
    {
    }

    internal override bool IsAuthenticationChallenge(HttpWebResponse webResponse)
    {
      if (webResponse == null)
        return false;
      return webResponse.StatusCode == HttpStatusCode.Found || webResponse.StatusCode == HttpStatusCode.Found ? !string.IsNullOrEmpty(webResponse.Headers["X-TFS-FedAuthRealm"]) : webResponse.StatusCode == HttpStatusCode.Unauthorized;
    }
  }
}
