// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DeploymentConstants
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

namespace Microsoft.TeamFoundation.Build.Server
{
  internal static class DeploymentConstants
  {
    internal const string StrongBoxBaseName = "{0}.{1}.EnvironmentData";
    internal const string ApiEndpointLookupKey = "ApiEndpoint";
    internal const string CredentialsLookupKey = "ApiCredentials";
    internal const string ConnectedServiceLock = "DeploymentEnvironmentLock.{0}.{1}";
    internal const string DeploymentSlotKey = "AlternateDeploymentSlot";
    internal const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
    internal const string SolutionToBuildKey = "SolutionToBuild";
    internal const string DeploymentEnvironmentKey = "DeploymentEnvironmentName";
    internal const string ProjectsToBuild = "ProjectsToBuild";
    internal const string DeploymentSettings = "DeploymentSettings";
    internal const string DeploymentBuildParameter = "{{\"SharePointDeploymentEnvironmentName\":\"\",\"ProviderHostedDeploymentEnvironmentName\":\"{0}\",\"PublishProfile\":\"\",\"AllowUntrustedCertificates\":true,\"AllowUpgrade\":true}}";
  }
}
