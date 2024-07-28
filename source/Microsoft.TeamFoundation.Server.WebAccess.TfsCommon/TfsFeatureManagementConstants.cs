// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TfsCommon.TfsFeatureManagementConstants
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TfsCommon, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3690C2EA-1623-4663-B65B-BB4B63BFE368
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TfsCommon.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Server.WebAccess.TfsCommon
{
  public static class TfsFeatureManagementConstants
  {
    public const string ManagedFeaturesContributionIdHosted = "ms.vss-web.managed-features";
    public const string ManagedFeaturesContributionIdOnPrem = "ms.vss-web.managed-features-onprem";

    public static string GetManagedFeaturesContributionId(IVssRequestContext requestContext) => !requestContext.ExecutionEnvironment.IsHostedDeployment ? "ms.vss-web.managed-features-onprem" : "ms.vss-web.managed-features";
  }
}
