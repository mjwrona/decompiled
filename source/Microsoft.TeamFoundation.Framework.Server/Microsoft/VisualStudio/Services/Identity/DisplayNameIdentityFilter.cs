// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.DisplayNameIdentityFilter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class DisplayNameIdentityFilter : IdentityFilter
  {
    private readonly string m_factorValue;
    private readonly DisplayNameIdentityFilter.ComaprisonType m_comparisonType;

    public DisplayNameIdentityFilter(string factorValue, bool findAny = false)
    {
      this.m_factorValue = factorValue;
      this.m_comparisonType = findAny ? DisplayNameIdentityFilter.ComaprisonType.Contains : DisplayNameIdentityFilter.ComaprisonType.StartsWith;
    }

    public DisplayNameIdentityFilter(ExpressionToken operatorToken, string factorValue)
    {
      if (string.Equals(operatorToken.Value, "=", StringComparison.OrdinalIgnoreCase))
        this.m_comparisonType = DisplayNameIdentityFilter.ComaprisonType.Equal;
      else if (string.Equals(operatorToken.Value, "StartsWith", StringComparison.OrdinalIgnoreCase))
      {
        this.m_comparisonType = DisplayNameIdentityFilter.ComaprisonType.StartsWith;
      }
      else
      {
        if (!string.Equals(operatorToken.Value, "Contains", StringComparison.OrdinalIgnoreCase))
          throw new IdentityExpressionException(FrameworkResources.QueryExpression_Malformed());
        this.m_comparisonType = DisplayNameIdentityFilter.ComaprisonType.Contains;
      }
      this.m_factorValue = factorValue;
    }

    public override bool IsMatch(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      switch (this.m_comparisonType)
      {
        case DisplayNameIdentityFilter.ComaprisonType.Equal:
          return identity.DisplayName.Equals(this.m_factorValue, StringComparison.OrdinalIgnoreCase);
        case DisplayNameIdentityFilter.ComaprisonType.StartsWith:
          return identity.DisplayName.StartsWith(this.m_factorValue, StringComparison.OrdinalIgnoreCase);
        case DisplayNameIdentityFilter.ComaprisonType.Contains:
          return identity.DisplayName.IndexOf(this.m_factorValue, StringComparison.OrdinalIgnoreCase) >= 0;
        default:
          return false;
      }
    }

    private enum ComaprisonType
    {
      Equal,
      StartsWith,
      Contains,
    }
  }
}
