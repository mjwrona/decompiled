// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes.KubernetesTokenBasedAuthParameters
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes
{
  internal class KubernetesTokenBasedAuthParameters : KubernetesAuthorizationParameters
  {
    internal string AccessToken { get; private set; }

    internal void SetAuthorizationParameters(string accessToken)
    {
      if (string.IsNullOrEmpty(accessToken))
        return;
      this.authorizationType = KubernetesAuthorizationType.Token;
      this.AccessToken = accessToken;
    }

    internal override KubernetesAuthorizationParameters GetAuthorizationParameters()
    {
      KubernetesTokenBasedAuthParameters authorizationParameters = new KubernetesTokenBasedAuthParameters();
      authorizationParameters.AccessToken = this.AccessToken;
      authorizationParameters.authorizationType = this.authorizationType;
      return (KubernetesAuthorizationParameters) authorizationParameters;
    }
  }
}
