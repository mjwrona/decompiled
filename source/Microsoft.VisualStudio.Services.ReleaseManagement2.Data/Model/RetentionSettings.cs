// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.RetentionSettings
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class RetentionSettings
  {
    private EnvironmentRetentionPolicy defaultEnvironmentRetentionPolicy;
    private EnvironmentRetentionPolicy maximumEnvironmentRetentionPolicy;
    private int daysToKeepDeletedReleases;

    public EnvironmentRetentionPolicy DefaultEnvironmentRetentionPolicy
    {
      get => this.defaultEnvironmentRetentionPolicy;
      set => this.defaultEnvironmentRetentionPolicy = value;
    }

    public EnvironmentRetentionPolicy MaximumEnvironmentRetentionPolicy
    {
      get => this.maximumEnvironmentRetentionPolicy;
      set => this.maximumEnvironmentRetentionPolicy = value;
    }

    public int DaysToKeepDeletedReleases
    {
      get => this.daysToKeepDeletedReleases;
      set => this.daysToKeepDeletedReleases = value;
    }
  }
}
