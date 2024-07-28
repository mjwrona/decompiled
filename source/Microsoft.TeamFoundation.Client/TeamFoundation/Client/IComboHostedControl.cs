// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.IComboHostedControl
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IComboHostedControl : IDisposable
  {
    string GetSelectedText(out int caretPos);

    bool Filter(string text);

    Size GetDesiredControlSize();

    Size GetMinimumControlSize();

    void HandleComboControlFocusLost();
  }
}
