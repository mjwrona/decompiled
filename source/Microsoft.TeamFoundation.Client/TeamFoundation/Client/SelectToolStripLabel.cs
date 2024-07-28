// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.SelectToolStripLabel
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  internal class SelectToolStripLabel : ToolStripLabel
  {
    protected override Size DefaultSize => Size.Empty;

    protected override Padding DefaultMargin => Padding.Empty;

    protected override Padding DefaultPadding => Padding.Empty;

    public override Size GetPreferredSize(Size proposedSize) => Size.Empty;

    internal void SelectToolStrip()
    {
      if (this.Owner == null)
        return;
      this.ProcessMnemonic('z');
    }
  }
}
