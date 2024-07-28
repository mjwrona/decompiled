// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TeamFoundationRuntimeHost
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class TeamFoundationRuntimeHost : IRuntimeHost, IServiceProvider
  {
    protected TeamFoundationRuntimeHost() => NotificationManager.Initialize();

    public virtual void Shutdown()
    {
      NotificationManager.Shutdown();
      WindowsHost.ShutdownImpl();
    }

    public virtual object GetService(Type type) => !(type == typeof (IRuntimeHost)) ? (object) null : (object) this;

    public virtual Icon AppIcon => (Icon) null;

    public virtual bool EnableModeless(bool enable) => WindowsHost.EnableModelessImpl(enable);

    public abstract DialogResult ShowMessageBox(
      IWin32Window parent,
      string text,
      string helpTopic,
      string caption,
      MessageBoxButtons buttons,
      MessageBoxIcon icon,
      MessageBoxDefaultButton defaultButton);

    public abstract IWin32Window DefaultParentWindow { get; }

    public abstract RuntimeEnvironmentFlags EnvironmentFlags { get; }

    public abstract void Write(LogFlags flags, LogCategory category, string message);

    public virtual string NewLine => WindowsHost.NewLineImpl;

    public virtual int OutputWidth => WindowsHost.OutputWidthImpl;

    public virtual Encoding OutputEncoding => WindowsHost.OutputEncodingImpl;

    public virtual string VsApplicationDataPath => WindowsHost.VsApplicationDataPathImpl;

    public virtual RegistryKey UserRegistryRoot => UIHost.UserRegistryRoot;

    public virtual RegistryKey ApplicationRegistryRoot => UIHost.ApplicationRegistryRoot;

    public abstract bool DisplayHelp(string helpTopic);

    public abstract DialogResult ShowModalDialog(
      Form form,
      IWin32Window parent,
      bool showHelpButton);

    public abstract UIHostColors HostColors { get; }

    public abstract WinformsStyler WinformsStyler { get; }

    public abstract Font Font { get; }

    public abstract WaitCursor GetWaitCursor();

    internal virtual string ExtractTFSDisplayName(string url) => UIHost.ExtractTFSDisplayName(url);

    public virtual Bitmap ThemeBitmap(Bitmap image, System.Drawing.Color backgroundColor) => image;

    public virtual void UpdateCommandStatus()
    {
    }
  }
}
