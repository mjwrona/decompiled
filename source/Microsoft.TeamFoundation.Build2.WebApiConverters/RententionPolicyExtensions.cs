// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.RententionPolicyExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class RententionPolicyExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.RetentionPolicy ToWebApiRetentionPolicy(
      this Microsoft.TeamFoundation.Build2.Server.RetentionPolicy srvRententionPolicy,
      ISecuredObject securedObject)
    {
      if (srvRententionPolicy == null)
        return (Microsoft.TeamFoundation.Build.WebApi.RetentionPolicy) null;
      return new Microsoft.TeamFoundation.Build.WebApi.RetentionPolicy(securedObject)
      {
        DaysToKeep = srvRententionPolicy.DaysToKeep,
        MinimumToKeep = srvRententionPolicy.MinimumToKeep,
        DeleteBuildRecord = srvRententionPolicy.DeleteBuildRecord,
        DeleteTestResults = srvRententionPolicy.DeleteTestResults,
        Branches = srvRententionPolicy.Branches,
        ArtifactsToDelete = srvRententionPolicy.ArtifactsToDelete,
        ArtifactTypesToDelete = srvRententionPolicy.ArtifactTypesToDelete
      };
    }

    public static Microsoft.TeamFoundation.Build2.Server.RetentionPolicy ToBuildServerRetentionPolicy(
      this Microsoft.TeamFoundation.Build.WebApi.RetentionPolicy webApiRetentionPolicy)
    {
      if (webApiRetentionPolicy == null)
        return (Microsoft.TeamFoundation.Build2.Server.RetentionPolicy) null;
      return new Microsoft.TeamFoundation.Build2.Server.RetentionPolicy()
      {
        DaysToKeep = webApiRetentionPolicy.DaysToKeep,
        MinimumToKeep = webApiRetentionPolicy.MinimumToKeep,
        DeleteBuildRecord = webApiRetentionPolicy.DeleteBuildRecord,
        DeleteTestResults = webApiRetentionPolicy.DeleteTestResults,
        Branches = webApiRetentionPolicy.Branches,
        ArtifactsToDelete = webApiRetentionPolicy.ArtifactsToDelete,
        ArtifactTypesToDelete = webApiRetentionPolicy.ArtifactTypesToDelete
      };
    }
  }
}
