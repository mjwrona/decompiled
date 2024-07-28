// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyRegistryPaths
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

namespace Microsoft.TeamFoundation.Policy.Server
{
  internal static class PolicyRegistryPaths
  {
    public const string PolicyServiceRoot = "/Service/Policy";
    public const string MinPoliciesNumberToApplyCaching = "/Service/Policy/Settings/PolicyEvaluation/MinPoliciesNumberToApplyCaching";
    public const string MinFilesNumberToApplyCaching = "/Service/Policy/Settings/PolicyEvaluation/MinFilesNumberToApplyCaching";
    public const string PullRequestDetailsCacheExpirationSeconds = "/Service/Git/Settings/PullRequestDetails/PullRequestDetailsCacheExpirationSeconds";
  }
}
