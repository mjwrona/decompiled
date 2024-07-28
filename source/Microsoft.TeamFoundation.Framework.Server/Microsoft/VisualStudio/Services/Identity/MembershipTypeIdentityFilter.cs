// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.MembershipTypeIdentityFilter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class MembershipTypeIdentityFilter : IdentityFilter
  {
    private readonly MembershipTypeIdentityFilter.MembershipType m_type;

    public MembershipTypeIdentityFilter(MembershipTypeIdentityFilter.MembershipType type) => this.m_type = type;

    public MembershipTypeIdentityFilter(ExpressionToken operatorToken, string factorValue)
    {
      if (operatorToken.TokenType != Microsoft.TeamFoundation.Framework.Server.TokenType.Equal)
        throw new IdentityExpressionException(FrameworkResources.QueryExpression_Malformed());
      if (factorValue.Equals("Group", StringComparison.OrdinalIgnoreCase))
      {
        this.m_type = MembershipTypeIdentityFilter.MembershipType.Groups;
      }
      else
      {
        if (!factorValue.Equals("User", StringComparison.OrdinalIgnoreCase))
          throw new IdentityExpressionException(FrameworkResources.QueryExpression_Malformed());
        this.m_type = MembershipTypeIdentityFilter.MembershipType.Users;
      }
    }

    public override bool IsMatch(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      bool flag = this.m_type == MembershipTypeIdentityFilter.MembershipType.Groups;
      return identity.IsContainer == flag;
    }

    internal enum MembershipType
    {
      Groups,
      Users,
    }
  }
}
