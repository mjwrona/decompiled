// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TokenRevocation.FrameworkTokenRevocationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.TokenRevocation
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class FrameworkTokenRevocationService : TokenRevocationServiceBase
  {
    protected override string ExpirationRegistrationPath => "/TokenRevocation.Framework.ExpirationHours";

    protected internal override TokenRevocationRule[] RetrieveAllRules(
      IVssRequestContext requestContext)
    {
      try
      {
        requestContext.TraceEnter(1049066, "TokenRevocation", "Service", nameof (RetrieveAllRules));
        return this.GetHttpClient(requestContext).ListRulesAsync().SyncResult<List<TokenRevocationRule>>().ToArray();
      }
      finally
      {
        requestContext.TraceLeave(1049068, "TokenRevocation", "Service", nameof (RetrieveAllRules));
      }
    }

    public override IList<Guid> CreateRules(
      IVssRequestContext requestContext,
      IEnumerable<TokenRevocationRule> rules)
    {
      this.ValidateServiceHost(requestContext);
      try
      {
        requestContext.TraceEnter(1049069, "TokenRevocation", "Service", nameof (CreateRules));
        List<Guid> rules1 = this.GetHttpClient(requestContext).CreateRulesAsync(rules).SyncResult<List<Guid>>();
        this.ClearCache(requestContext);
        return (IList<Guid>) rules1;
      }
      finally
      {
        requestContext.TraceLeave(1049071, "TokenRevocation", "Service", nameof (CreateRules));
      }
    }

    public override void DeleteRule(IVssRequestContext requestContext, Guid ruleId)
    {
      this.ValidateServiceHost(requestContext);
      try
      {
        requestContext.TraceEnter(1049072, "TokenRevocation", "Service", nameof (DeleteRule));
        this.GetHttpClient(requestContext).DeleteRuleAsync(ruleId, (object) requestContext).SyncResult();
        this.ClearCache(requestContext);
      }
      finally
      {
        requestContext.TraceLeave(1049074, "TokenRevocation", "Service", nameof (DeleteRule));
      }
    }

    private TokenRevocationHttpClient GetHttpClient(IVssRequestContext requestContext) => requestContext.Elevate().GetClient<TokenRevocationHttpClient>();
  }
}
