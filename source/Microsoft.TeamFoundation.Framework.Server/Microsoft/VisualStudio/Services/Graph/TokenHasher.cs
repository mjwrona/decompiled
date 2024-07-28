// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.TokenHasher
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Graph
{
  public static class TokenHasher
  {
    private const string Area = "Graph";
    private const string Layer = "TokenHasher";

    public static byte[] Hash(IVssRequestContext requestContext, string accessToken)
    {
      if (requestContext == null)
      {
        requestContext.Trace(10008240, TraceLevel.Verbose, "Graph", nameof (TokenHasher), "Request context is null during token hash request");
        return (byte[]) null;
      }
      ITokenHasherExtension extension = requestContext.GetExtension<ITokenHasherExtension>();
      if (extension != null)
        return extension.Hash(accessToken);
      requestContext.Trace(10008241, TraceLevel.Verbose, "Graph", nameof (TokenHasher), "Token hasher extension is null during token hash request");
      return (byte[]) null;
    }
  }
}
