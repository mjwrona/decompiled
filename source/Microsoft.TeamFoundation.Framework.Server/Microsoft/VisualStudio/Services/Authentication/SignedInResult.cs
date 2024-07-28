// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Authentication.SignedInResult
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Compliance;
using Microsoft.VisualStudio.Services.Identity;

namespace Microsoft.VisualStudio.Services.Authentication
{
  internal class SignedInResult
  {
    public SignedInResultAction SignedInResultAction { get; set; }

    public SecureFlowLocation NextLocation { get; set; }

    public string TenantSwitchAccessToken { get; set; }

    public TenantSwitchException TenantSwitchException { get; set; }

    public bool IgnoreJavascriptNotify { get; set; }
  }
}
