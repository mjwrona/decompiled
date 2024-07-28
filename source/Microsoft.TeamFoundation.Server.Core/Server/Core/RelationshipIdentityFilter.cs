// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.RelationshipIdentityFilter
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class RelationshipIdentityFilter : IdentityFilter
  {
    private HashSet<Guid> m_relatedIdentities;

    public RelationshipIdentityFilter(
      IVssRequestContext requestContext,
      ExpressionToken operatorToken,
      string factorValue)
    {
      TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
      Guid result;
      if (!Guid.TryParse(factorValue, out result))
        throw new IdentityExpressionException(FrameworkResources.IdentityExpression_TargetIdentityMustBeId());
      if (string.Equals(operatorToken.Value, "Near", StringComparison.OrdinalIgnoreCase))
        this.m_relatedIdentities = service.GetRecentUsers(requestContext, result);
      else if (!string.Equals(operatorToken.Value, "In", StringComparison.OrdinalIgnoreCase) && !string.Equals(operatorToken.Value, "Under", StringComparison.OrdinalIgnoreCase))
        throw new IdentityExpressionException(FrameworkResources.QueryExpression_Malformed());
    }

    public override bool IsMatch(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity) => this.m_relatedIdentities.Contains(identity.Id);
  }
}
