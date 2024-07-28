// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.Internal.MergeOptions
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.VersionControl.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class MergeOptions : DiffOptions
  {
    private string m_latestLabel;
    private bool m_adjacentChangesConflict;

    public MergeOptions()
    {
      this.Flags = DiffOptionFlags.EnablePreambleHandling;
      this.AdjacentChangesConflict = true;
    }

    public string LatestLabel
    {
      get => this.m_latestLabel;
      set => this.m_latestLabel = value;
    }

    public bool AdjacentChangesConflict
    {
      get => this.m_adjacentChangesConflict;
      set => this.m_adjacentChangesConflict = value;
    }

    public bool WriteOriginalForConflictingRange { get; set; }
  }
}
