// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes.KubernetesBasicAuthParameters
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes
{
  internal class KubernetesBasicAuthParameters : KubernetesAuthorizationParameters
  {
    internal string Username { get; private set; }

    internal string Password { get; private set; }

    internal void SetAuthorizationParameters(string username, string password)
    {
      if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        return;
      this.authorizationType = KubernetesAuthorizationType.UsernamePassword;
      this.Username = username;
      this.Password = password;
    }

    internal override KubernetesAuthorizationParameters GetAuthorizationParameters()
    {
      KubernetesBasicAuthParameters authorizationParameters = new KubernetesBasicAuthParameters()
      {
        Username = this.Username
      };
      authorizationParameters.Username = this.Username;
      authorizationParameters.authorizationType = this.authorizationType;
      return (KubernetesAuthorizationParameters) authorizationParameters;
    }
  }
}
