// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Controls.DialogHost
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Client.Controls
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class DialogHost : IDialogHost
  {
    public Task<bool?> InvokeDialogAsync(InvokeDialogFunc showDialog, object state)
    {
      IntPtr owner = ClientNativeMethods.GetDefaultParentWindow();
      if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
        return Task.FromResult<bool?>(showDialog(owner, state));
      bool? result = new bool?();
      Thread thread = new Thread((ThreadStart) (() => result = showDialog(owner, state)));
      thread.IsBackground = true;
      thread.SetApartmentState(ApartmentState.STA);
      thread.Start();
      return Task.Run(new Action(thread.Join)).ContinueWith<bool?>((Func<Task, bool?>) (t => result));
    }
  }
}
