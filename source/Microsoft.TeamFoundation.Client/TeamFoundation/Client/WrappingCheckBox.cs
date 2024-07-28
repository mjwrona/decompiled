// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.WrappingCheckBox
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WrappingCheckBox : CheckBox
  {
    private Size cachedSizeOfOneLineOfText = Size.Empty;
    private Hashtable preferredSizeHash = new Hashtable(3);

    public WrappingCheckBox() => this.AutoSize = true;

    protected override void OnTextChanged(EventArgs e)
    {
      base.OnTextChanged(e);
      this.cachetextsize();
    }

    protected override void OnFontChanged(EventArgs e)
    {
      base.OnFontChanged(e);
      this.cachetextsize();
    }

    private void cachetextsize()
    {
      this.preferredSizeHash.Clear();
      if (string.IsNullOrEmpty(this.Text))
        this.cachedSizeOfOneLineOfText = Size.Empty;
      else
        this.cachedSizeOfOneLineOfText = TextRenderer.MeasureText(this.Text, this.Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.WordBreak);
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
      Size preferredSize = base.GetPreferredSize(proposedSize);
      if (preferredSize.Width > proposedSize.Width)
      {
        int num;
        if (!string.IsNullOrEmpty(this.Text))
        {
          num = proposedSize.Width;
          if (!num.Equals(int.MaxValue))
            goto label_4;
        }
        num = proposedSize.Height;
        if (num.Equals(int.MaxValue))
          goto label_7;
label_4:
        Size size1 = preferredSize - this.cachedSizeOfOneLineOfText;
        Size size2 = proposedSize - size1 - new Size(3, 0);
        if (!this.preferredSizeHash.ContainsKey((object) size2))
        {
          preferredSize = size1 + TextRenderer.MeasureText(this.Text, this.Font, size2, TextFormatFlags.WordBreak);
          this.preferredSizeHash[(object) size2] = (object) preferredSize;
        }
        else
          preferredSize = (Size) this.preferredSizeHash[(object) size2];
      }
label_7:
      return preferredSize;
    }
  }
}
