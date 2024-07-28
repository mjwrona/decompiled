// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.MergeSummary
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  public class MergeSummary
  {
    private int m_totalCommon;
    private int m_totalModified;
    private int m_totalLatest;
    private int m_totalBoth;
    private int m_totalConflicting;

    [Obsolete("Please use the TotalModified, TotalLatest, TotalConflicting and TotalBoth properties to determine the results of the merge.")]
    public int TotalCommon
    {
      get => this.m_totalCommon;
      set => this.m_totalCommon = value;
    }

    public int TotalModified
    {
      get => this.m_totalModified;
      set => this.m_totalModified = value;
    }

    public int TotalLatest
    {
      get => this.m_totalLatest;
      set => this.m_totalLatest = value;
    }

    public int TotalBoth
    {
      get => this.m_totalBoth;
      set => this.m_totalBoth = value;
    }

    public int TotalConflicting
    {
      get => this.m_totalConflicting;
      set => this.m_totalConflicting = value;
    }
  }
}
