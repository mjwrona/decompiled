// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ShallowEnvironmentRetentionPolicy
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class ShallowEnvironmentRetentionPolicy
  {
    private int daysToKeep;
    private int releasesToKeep;

    public ShallowEnvironmentRetentionPolicy()
    {
      this.DaysToKeep = 30;
      this.ReleasesToKeep = 3;
    }

    public int DaysToKeep
    {
      get => this.daysToKeep;
      set => this.daysToKeep = value;
    }

    public int ReleasesToKeep
    {
      get => this.releasesToKeep;
      set => this.releasesToKeep = value;
    }
  }
}
