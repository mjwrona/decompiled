// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ProjectRetentionSettings
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

namespace Microsoft.TeamFoundation.Build2.Server
{
  public struct ProjectRetentionSettings
  {
    public RetentionSetting PurgeRuns;
    public RetentionSetting PurgePullRequestRuns;
    public RetentionSetting PurgeArtifacts;
    public RetentionSetting RunsToRetainPerProtectedBranch;
    public static readonly ProjectRetentionSettings HostedDefault = new ProjectRetentionSettings()
    {
      PurgeRuns = new RetentionSetting()
      {
        Min = 30,
        Max = 731,
        Value = 30
      },
      PurgePullRequestRuns = new RetentionSetting()
      {
        Min = 1,
        Max = 30,
        Value = 10
      },
      PurgeArtifacts = new RetentionSetting()
      {
        Min = 1,
        Max = 60,
        Value = 30
      },
      RunsToRetainPerProtectedBranch = new RetentionSetting()
      {
        Min = 0,
        Max = 50,
        Value = 3
      }
    };
    public static readonly ProjectRetentionSettings OnPremDefault = new ProjectRetentionSettings()
    {
      PurgeRuns = new RetentionSetting()
      {
        Min = 30,
        Max = 18251,
        Value = 30
      },
      PurgePullRequestRuns = new RetentionSetting()
      {
        Min = 1,
        Max = 18251,
        Value = 10
      },
      PurgeArtifacts = new RetentionSetting()
      {
        Min = 1,
        Max = 18251,
        Value = 30
      },
      RunsToRetainPerProtectedBranch = new RetentionSetting()
      {
        Min = 0,
        Max = 5000,
        Value = 3
      }
    };

    public override string ToString() => string.Format("runs [{0}]\n", (object) this.PurgeRuns) + string.Format("pr runs [{0}]\n", (object) this.PurgePullRequestRuns) + string.Format("artifacts [{0}]\n", (object) this.PurgeArtifacts) + string.Format("protected runs [{0}]", (object) this.RunsToRetainPerProtectedBranch);
  }
}
