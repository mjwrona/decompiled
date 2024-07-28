// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Authentication.FrameworkSignedInHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Authentication
{
  [ExtensionPriority(1)]
  internal class FrameworkSignedInHandler : ISignedInHandler
  {
    private const string s_area = "Authentication";
    private const string s_layer = "FrameworkSignedInHandler";

    public SignedInResult SignedIn(IVssRequestContext requestContext, SignedInParameters parameters)
    {
      requestContext.TraceEnter(1450830, "Authentication", nameof (FrameworkSignedInHandler), nameof (SignedIn));
      SignedInResult signedInResult = new SignedInResult();
      signedInResult.NextLocation = parameters.FinalLocation;
      signedInResult.SignedInResultAction = SignedInResultAction.FinalEndpoint;
      requestContext.TraceLeave(1450834, "Authentication", nameof (FrameworkSignedInHandler), nameof (SignedIn));
      return signedInResult;
    }
  }
}
