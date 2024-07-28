// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.TeamFoundationPolicy`3
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Policy.Server
{
  public abstract class TeamFoundationPolicy<TSettings, TContext, TTarget> : 
    TeamFoundationPolicy<TSettings, TContext>
    where TContext : TeamFoundationPolicyEvaluationRecordContext
    where TTarget : class, ITeamFoundationPolicyTarget
  {
    public override sealed bool IsApplicableTo(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyTarget target)
    {
      return target is TTarget target1 && this.IsApplicableTo(requestContext, target1);
    }

    protected abstract bool IsApplicableTo(IVssRequestContext requestContext, TTarget target);
  }
}
