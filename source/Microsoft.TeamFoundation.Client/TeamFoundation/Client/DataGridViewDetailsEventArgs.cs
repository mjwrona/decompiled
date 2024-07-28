// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.DataGridViewDetailsEventArgs
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class DataGridViewDetailsEventArgs : CancelEventArgs
  {
    private int m_rowIndex;
    private bool m_userInitiated;
    private bool m_collapseDetails;

    public DataGridViewDetailsEventArgs(int rowIndex, bool userInitiated)
      : base(false)
    {
      this.m_rowIndex = rowIndex;
      this.m_userInitiated = userInitiated;
      this.m_collapseDetails = false;
    }

    public int RowIndex => this.m_rowIndex;

    public bool UserInitiated => this.m_userInitiated;

    public bool CollapseDetails
    {
      get => this.m_collapseDetails;
      set => this.m_collapseDetails = value;
    }
  }
}
