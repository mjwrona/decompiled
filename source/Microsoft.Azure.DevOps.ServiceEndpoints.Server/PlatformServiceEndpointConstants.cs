// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.PlatformServiceEndpointConstants
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  internal static class PlatformServiceEndpointConstants
  {
    internal const string ServiceEndpoint = "DistributedTaskServiceEndpoint";
    internal const string DataspaceCategory = "DistributedTask";
    internal const string OboAuthorization = "oboAuthorization";
    internal const string ConvertAuthenticationScheme = "ConvertAuthenticationScheme";
    internal const string ChangeProperties = "ChangeProperties";
    internal static readonly RegistryQuery s_cleanUpOldSecretsAfterUpgradeDelayInMinutes = new RegistryQuery("/Service/ServiceEndpoints/Settings/CleanUpOldSecretsAfterUpgradeDelayInMinutes");
    internal const int s_defaultCleanUpOldSecretsAfterUpgradeDelayInMinutes = 10080;
  }
}
