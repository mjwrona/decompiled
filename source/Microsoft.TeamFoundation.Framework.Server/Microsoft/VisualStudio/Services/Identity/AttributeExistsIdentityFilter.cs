// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.AttributeExistsIdentityFilter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class AttributeExistsIdentityFilter : IdentityFilter
  {
    private readonly bool m_includeMatches;
    private readonly string m_attributeName;

    public AttributeExistsIdentityFilter(string attributeName, bool includeMatches)
    {
      this.m_attributeName = attributeName;
      this.m_includeMatches = includeMatches;
    }

    public override bool IsMatch(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity) => !string.IsNullOrEmpty(identity.GetProperty<string>(this.m_attributeName, string.Empty)) == this.m_includeMatches;
  }
}
