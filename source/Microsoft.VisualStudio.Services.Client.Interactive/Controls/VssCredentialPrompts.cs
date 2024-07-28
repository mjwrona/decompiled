// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Controls.VssCredentialPrompts
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Client.Controls
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VssCredentialPrompts : IVssCredentialPrompts, IVssCredentialPrompt
  {
    public VssCredentialPrompts()
      : this((VssCredentialPromptContext) null)
    {
    }

    public VssCredentialPrompts(VssCredentialPromptContext context)
      : this((IVssCredentialPrompt) new VssFederatedCredentialPrompt(context), (IVssCredentialPrompt) new WindowsCredentialPrompt())
    {
    }

    public VssCredentialPrompts(
      IVssCredentialPrompt federatedCredentialPrompt,
      IVssCredentialPrompt windowsCredentialPrompt)
    {
      this.FederatedPrompt = federatedCredentialPrompt;
      this.WindowsPrompt = windowsCredentialPrompt;
    }

    public IDictionary<string, string> Parameters { get; set; }

    Task<IssuedToken> IVssCredentialPrompt.GetTokenAsync(
      IssuedTokenProvider provider,
      IssuedToken failedToken)
    {
      if (this.WindowsPrompt != null && provider is WindowsTokenProvider)
        return this.WindowsPrompt.GetTokenAsync(provider, failedToken);
      return this.FederatedPrompt != null ? this.FederatedPrompt.GetTokenAsync(provider, failedToken) : Task.FromResult<IssuedToken>((IssuedToken) null);
    }

    public IVssCredentialPrompt FederatedPrompt { get; private set; }

    public IVssCredentialPrompt WindowsPrompt { get; private set; }

    public static VssCredentialPrompts CreateDefault(
      WindowsCredential windowsCredential,
      FederatedCredential federatedCredential)
    {
      return VssCredentialPrompts.CreatePromptsWithHost(windowsCredential, federatedCredential, (IDialogHost) null);
    }

    public static VssCredentialPrompts CreatePromptsWithHost(
      WindowsCredential windowsCredential,
      FederatedCredential federatedCredential,
      IDialogHost host)
    {
      IVssCredentialPrompt windowsCredentialPrompt = (IVssCredentialPrompt) null;
      if (windowsCredential != null)
        windowsCredentialPrompt = (IVssCredentialPrompt) new WindowsCredentialPrompt();
      IVssCredentialPrompt federatedCredentialPrompt = (IVssCredentialPrompt) null;
      switch (federatedCredential)
      {
        case VssFederatedCredential _:
          federatedCredentialPrompt = host == null ? (IVssCredentialPrompt) new VssFederatedCredentialPrompt() : (IVssCredentialPrompt) new VssFederatedCredentialPrompt(host);
          break;
        case VssBasicCredential _:
          federatedCredentialPrompt = (IVssCredentialPrompt) new WindowsCredentialPrompt(CachedCredentialsType.Basic);
          break;
      }
      return new VssCredentialPrompts(federatedCredentialPrompt, windowsCredentialPrompt);
    }
  }
}
