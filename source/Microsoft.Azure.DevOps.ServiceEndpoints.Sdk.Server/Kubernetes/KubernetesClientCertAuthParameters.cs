// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes.KubernetesClientCertAuthParameters
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes
{
  internal class KubernetesClientCertAuthParameters : KubernetesAuthorizationParameters
  {
    internal string ClientCertificateData { get; private set; }

    internal string ClientKeyData { get; private set; }

    internal void SetAuthorizationParameters(string clientCertificateData, string clientKeyData)
    {
      if (string.IsNullOrEmpty(clientCertificateData) || string.IsNullOrEmpty(clientKeyData))
        return;
      this.authorizationType = KubernetesAuthorizationType.ClientCertificate;
      this.ClientCertificateData = clientCertificateData;
      this.ClientKeyData = clientKeyData;
    }

    internal override KubernetesAuthorizationParameters GetAuthorizationParameters()
    {
      KubernetesClientCertAuthParameters authorizationParameters = new KubernetesClientCertAuthParameters();
      authorizationParameters.ClientCertificateData = this.ClientCertificateData;
      authorizationParameters.ClientKeyData = this.ClientKeyData;
      authorizationParameters.authorizationType = this.authorizationType;
      return (KubernetesAuthorizationParameters) authorizationParameters;
    }
  }
}
