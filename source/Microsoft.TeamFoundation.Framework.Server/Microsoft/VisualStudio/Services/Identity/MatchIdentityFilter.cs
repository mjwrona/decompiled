// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.MatchIdentityFilter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal abstract class MatchIdentityFilter : IdentityFilter
  {
    private readonly string m_factorValue;
    private readonly bool m_findAny;
    private readonly bool m_includeMatches;

    public MatchIdentityFilter(string factorValue, bool includeMatches = true, bool findAny = false)
    {
      this.m_factorValue = factorValue;
      this.m_findAny = findAny;
      this.m_includeMatches = includeMatches;
    }

    public override bool IsMatch(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      string filterProperty = this.GetFilterProperty(identity);
      return this.m_findAny ? filterProperty.IndexOf(this.m_factorValue, StringComparison.CurrentCultureIgnoreCase) >= 0 == this.m_includeMatches : filterProperty.StartsWith(this.m_factorValue, true, CultureInfo.CurrentCulture) == this.m_includeMatches;
    }

    protected abstract string GetFilterProperty(Microsoft.VisualStudio.Services.Identity.Identity identity);
  }
}
