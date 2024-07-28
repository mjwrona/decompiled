// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.AttributeMatchIdentityFilter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class AttributeMatchIdentityFilter : MatchIdentityFilter
  {
    private readonly string m_attributeName;

    public AttributeMatchIdentityFilter(
      string attributeName,
      string factorValue,
      bool includeMatches = true,
      bool findAny = false)
      : base(factorValue, includeMatches, findAny)
    {
      this.m_attributeName = attributeName;
    }

    protected override string GetFilterProperty(Microsoft.VisualStudio.Services.Identity.Identity identity) => identity.GetProperty<string>(this.m_attributeName, string.Empty);
  }
}
