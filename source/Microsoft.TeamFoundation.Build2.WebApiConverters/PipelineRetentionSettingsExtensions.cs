// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.PipelineRetentionSettingsExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class PipelineRetentionSettingsExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.RetentionSetting ToWebApiRetentionSetting(
      this Microsoft.TeamFoundation.Build2.Server.RetentionSetting retentionSetting,
      ISecuredObject securedObject)
    {
      return new Microsoft.TeamFoundation.Build.WebApi.RetentionSetting(securedObject)
      {
        Min = retentionSetting.Min,
        Max = retentionSetting.Max,
        Value = retentionSetting.Value
      };
    }

    public static ProjectRetentionSetting ToWebApiProjectRetentionSetting(
      this ProjectRetentionSettings retentionSetting,
      ISecuredObject securedObject)
    {
      return new ProjectRetentionSetting(securedObject)
      {
        PurgeArtifacts = retentionSetting.PurgeArtifacts.ToWebApiRetentionSetting(securedObject),
        PurgePullRequestRuns = retentionSetting.PurgePullRequestRuns.ToWebApiRetentionSetting(securedObject),
        PurgeRuns = retentionSetting.PurgeRuns.ToWebApiRetentionSetting(securedObject)
      };
    }
  }
}
