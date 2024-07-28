// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.FeatureFlag
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public static class FeatureFlag
  {
    public const string UseMicrosoftGraph = "ServiceEnpoints.Service.UseMicrosoftGraph";
    public const string EnableOidcFederation = "ms.vss-distributedtask-web.workload-identity-federation";
    public const string EnableArmServiceConnectionUpgradeToWif = "ServiceEndpoints.EnableARMServiceConnectionUpgradeToWIF";
    public const string ProhibitCreatingNewWorkloadIdentityFederationServiceConnections = "ServiceEndpoints.ProhibitCreatingNewWorkloadIdentityFederationServiceConnections";
    public const string EnableProjectReferencesDataFilling = "ServiceEndpoints.EnableProjectReferencesDataFilling";
    public const string EnableOriginalARMServiceEndpointType = "ServiceEndpoints.EnableOriginalARMServiceEndpointType";
    public const string UseUserIdentityToCheckSystemContext = "ServiceEndpoints.UseUserIdentityToCheckSystemContext";
  }
}
