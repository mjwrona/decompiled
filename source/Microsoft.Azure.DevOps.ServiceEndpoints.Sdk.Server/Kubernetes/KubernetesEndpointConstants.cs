// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes.KubernetesEndpointConstants
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes
{
  public static class KubernetesEndpointConstants
  {
    public const string KubernetesEndpointJobName = "KubernetesEndpointJob";
    public const string KubernetesEndpointJobExtensionName = "Microsoft.Azure.DevOps.ServiceEndpoints.Server.Extensions.KubernetesEndpointJob";
    public const int KubernetesSetupServiceAccountRetries = 5;
    public const int KubernetesCreateNamespaceRetries = 5;
    public const double KubernetesTimeBetweenRetriesInSeconds = 1.0;
    public const string Kubeconfig = "Kubeconfig";
    public const string ServiceAccount = "ServiceAccount";
    public const string AzureSubscription = "AzureSubscription";
    public const string Namespace = "namespace";
    public const string Default = "default";
    public const string ClusterId = "clusterId";
    public const string AzureSubscriptionId = "azureSubscriptionId";
    public const string AzureSubscriptionName = "azureSubscriptionName";
    public const string AzureEnvironment = "azureEnvironment";
    public const string AzureTenantId = "azureTenantId";
    public const string AzureAccessToken = "azureAccessToken";
    public const string RoleBindingName = "roleBindingName";
    public const string ServiceAccountName = "serviceAccountName";
    public const string SecretName = "secretName";
    public const string AzureSpnId = "spnId";
    public const string AzureSpnKey = "spnKey";
    public const string CloudUrl = "cloudUrl";
    public const string SPNCreationMethod = "spnCreationMethod";
    public const string ClusterAdmin = "clusterAdmin";
    public const string CreateNamespace = "operation.createNamespace";
    public const string CreateOrReuseNamespace = "operation.createOrReuseNamespace";
    public const string OperationType = "operation.type";
    public const string Update = "update";
  }
}
