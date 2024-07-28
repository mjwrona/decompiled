// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Rules.JobRule
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.CodeSense.Server.Rules
{
  public abstract class JobRule
  {
    protected IVssRequestContext RequestContext;
    private readonly JobRule.SatisfactionHandler _handleYes;
    private readonly JobRule.SatisfactionHandler _handleNo;

    protected JobRule(
      IVssRequestContext requestContext,
      JobRule.SatisfactionHandler callIfSatisfied,
      JobRule.SatisfactionHandler callIfUnsatisfied)
    {
      this.RequestContext = requestContext;
      this._handleYes = callIfSatisfied ?? (JobRule.SatisfactionHandler) (() => { });
      this._handleNo = callIfUnsatisfied ?? (JobRule.SatisfactionHandler) (() => { });
    }

    protected abstract bool IsSatisfiedBy();

    public bool Implement()
    {
      if (this.IsSatisfiedBy())
      {
        this._handleYes();
        return true;
      }
      this._handleNo();
      return false;
    }

    public delegate void SatisfactionHandler();
  }
}
