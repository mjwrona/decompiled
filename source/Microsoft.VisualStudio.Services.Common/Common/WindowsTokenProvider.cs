// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.WindowsTokenProvider
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Globalization;
using System.Net;

namespace Microsoft.VisualStudio.Services.Common
{
  internal sealed class WindowsTokenProvider : IssuedTokenProvider
  {
    public WindowsTokenProvider(WindowsCredential credential, Uri serverUrl)
      : base((IssuedTokenCredential) credential, serverUrl, serverUrl)
    {
    }

    protected override string AuthenticationScheme => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1} {2} {3}", (object) AuthenticationSchemes.Negotiate, (object) AuthenticationSchemes.Ntlm, (object) AuthenticationSchemes.Digest, (object) AuthenticationSchemes.Basic);

    public WindowsCredential Credential => (WindowsCredential) base.Credential;

    public override bool GetTokenIsInteractive => this.CurrentToken == null;
  }
}
