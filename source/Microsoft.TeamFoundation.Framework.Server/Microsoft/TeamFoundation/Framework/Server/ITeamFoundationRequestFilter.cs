// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationRequestFilter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [InheritedExport]
  public interface ITeamFoundationRequestFilter
  {
    void BeginRequest(IVssRequestContext requestContext);

    Task BeginRequestAsync(IVssRequestContext requestContext);

    Task PostAuthenticateRequest(IVssRequestContext requestContext);

    void PostAuthorizeRequest(IVssRequestContext requestContext);

    void EnterMethod(IVssRequestContext requestContext);

    void LeaveMethod(IVssRequestContext requestContext);

    Task PostLogRequestAsync(IVssRequestContext requestContext);

    void EndRequest(IVssRequestContext requestContext);
  }
}
