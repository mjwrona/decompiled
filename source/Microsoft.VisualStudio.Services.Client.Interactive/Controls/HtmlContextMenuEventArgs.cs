// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Controls.HtmlContextMenuEventArgs
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System;
using System.Windows;

namespace Microsoft.VisualStudio.Services.Client.Controls
{
  internal class HtmlContextMenuEventArgs : EventArgs
  {
    public HtmlContextMenuEventArgs(Point point, object element)
    {
      this.Point = point;
      this.Element = element;
    }

    public Point Point { get; private set; }

    public object Element { get; private set; }
  }
}
