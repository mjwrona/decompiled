// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ListViewColumnSizedEventArgs
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ListViewColumnSizedEventArgs : EventArgs
  {
    private int m_columnIndex;
    private int m_width;

    public ListViewColumnSizedEventArgs(int columnIndex, int width)
    {
      this.m_columnIndex = columnIndex;
      this.m_width = width;
    }

    public int ColumnIndex => this.m_columnIndex;

    public int Width => this.m_width;
  }
}
