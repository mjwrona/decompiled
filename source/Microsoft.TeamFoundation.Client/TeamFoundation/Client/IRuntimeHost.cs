// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.IRuntimeHost
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  public interface IRuntimeHost : IServiceProvider
  {
    DialogResult ShowMessageBox(
      IWin32Window parent,
      string text,
      string helpTopic,
      string caption,
      MessageBoxButtons buttons,
      MessageBoxIcon icon,
      MessageBoxDefaultButton defaultButton);

    bool EnableModeless(bool enable);

    IWin32Window DefaultParentWindow { get; }

    RuntimeEnvironmentFlags EnvironmentFlags { get; }

    void Write(LogFlags flags, LogCategory category, string message);

    string NewLine { get; }

    int OutputWidth { get; }

    Encoding OutputEncoding { get; }
  }
}
