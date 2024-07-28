// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Controls.VssCredentialPromptContext
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Client.Controls
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class VssCredentialPromptContext
  {
    public VssCredentialPromptContext()
      : this((IDialogHost) new Microsoft.VisualStudio.Services.Client.Controls.DialogHost())
    {
    }

    public VssCredentialPromptContext(IDialogHost dialogHost)
    {
      ArgumentUtility.CheckForNull<IDialogHost>(dialogHost, nameof (dialogHost));
      this.DialogHost = dialogHost;
    }

    public IDialogHost DialogHost { get; private set; }
  }
}
