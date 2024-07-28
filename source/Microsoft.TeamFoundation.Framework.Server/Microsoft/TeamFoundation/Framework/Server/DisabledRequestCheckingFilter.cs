// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DisabledRequestCheckingFilter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DisabledRequestCheckingFilter : ITeamFoundationRequestFilter
  {
    private static readonly string s_Area = "RequestFilter";
    private static readonly string s_Layer = nameof (DisabledRequestCheckingFilter);

    void ITeamFoundationRequestFilter.BeginRequest(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<DisabledRequestCheckingService>().CheckIfDisabled(vssRequestContext, true);
    }

    public Task BeginRequestAsync(IVssRequestContext requestContext) => Task.CompletedTask;

    Task ITeamFoundationRequestFilter.PostAuthenticateRequest(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(0, DisabledRequestCheckingFilter.s_Area, DisabledRequestCheckingFilter.s_Layer, "PostAuthenticateRequest");
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<DisabledRequestCheckingService>().CheckIfDisabled(vssRequestContext, false);
      requestContext.TraceLeave(0, DisabledRequestCheckingFilter.s_Area, DisabledRequestCheckingFilter.s_Layer, "PostAuthenticateRequest");
      return (Task) Task.FromResult<int>(0);
    }

    void ITeamFoundationRequestFilter.EnterMethod(IVssRequestContext requestContext)
    {
    }

    void ITeamFoundationRequestFilter.LeaveMethod(IVssRequestContext requestContext)
    {
    }

    Task ITeamFoundationRequestFilter.PostLogRequestAsync(IVssRequestContext requestContext) => (Task) Task.FromResult<int>(0);

    void ITeamFoundationRequestFilter.EndRequest(IVssRequestContext requestContext)
    {
    }

    void ITeamFoundationRequestFilter.PostAuthorizeRequest(IVssRequestContext requestContext)
    {
    }
  }
}
