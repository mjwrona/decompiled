// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TokenRevocation.TokenRevocationRuleExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.TokenRevocation
{
  public static class TokenRevocationRuleExtensions
  {
    public static bool MatchRule(
      this TokenRevocationRule rule,
      DateTimeOffset validFrom,
      string[] tokenParts,
      Guid userId)
    {
      bool flag = false;
      if (rule.CreatedBefore.HasValue)
      {
        if (!(validFrom < (DateTimeOffset) rule.CreatedBefore.Value))
          return false;
        flag = true;
      }
      if (!string.IsNullOrWhiteSpace(rule.Scopes))
      {
        if (tokenParts == null || !((IEnumerable<string>) rule.GetScopes()).Intersect<string>((IEnumerable<string>) tokenParts, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Any<string>())
          return false;
        flag = true;
      }
      if (rule.IdentityId.HasValue && rule.IdentityId.Value != Guid.Empty)
      {
        if (!(rule.IdentityId.Value == userId))
          return false;
        flag = true;
      }
      return flag;
    }
  }
}
