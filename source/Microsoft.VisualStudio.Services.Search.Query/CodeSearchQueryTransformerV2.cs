// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.CodeSearchQueryTransformerV2
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Model;
using Microsoft.VisualStudio.Services.Search.Query.Correctors;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  internal class CodeSearchQueryTransformerV2 : CodeSearchQueryTransformerBase
  {
    private bool m_isProximitySearchEnabled;
    [StaticSafe]
    private static readonly ISet<string> s_supportedOperators = (ISet<string>) new HashSet<string>()
    {
      "AND",
      "OR",
      "NOT",
      "(",
      ")",
      "[",
      "]",
      "\"",
      "\\\"",
      ":"
    };
    [StaticSafe]
    private static readonly ISet<string> s_supportedOperatorsWithProximityOperators = (ISet<string>) new HashSet<string>()
    {
      "NEAR",
      "BEFORE",
      "AFTER",
      "AND",
      "OR",
      "NOT",
      "(",
      ")",
      "[",
      "]",
      "\"",
      "\\\"",
      ":"
    };
    [StaticSafe]
    private static readonly CompositeTermCorrector s_compositeTermCorrector = new CompositeTermCorrector(new ICorrector[9]
    {
      (ICorrector) new DoubleQuotesCorrector(),
      (ICorrector) new FileMetadataExtensionTermCorrectorV2(),
      (ICorrector) new FileExtensionTermCorrector(),
      (ICorrector) new CodeFilterAliasCorrector(),
      (ICorrector) new WildcardTermCorrector(),
      (ICorrector) new UnrecognizedTypeTermCorrectorV2(),
      (ICorrector) new CodeSearchUnsupportedCharacterTermCorrector(),
      (ICorrector) new CodeNormalizationTermCorrectorV2(),
      (ICorrector) new WildcardTermCorrector()
    });

    public CodeSearchQueryTransformerV2(IVssRequestContext requestContext) => this.m_isProximitySearchEnabled = requestContext.IsFeatureEnabled("Search.Server.Code.ProximitySearch");

    protected override CompositeTermCorrector GetCompositeTermCorrector() => CodeSearchQueryTransformerV2.s_compositeTermCorrector;

    protected override ISet<string> GetSupportedOperators() => !this.m_isProximitySearchEnabled ? CodeSearchQueryTransformerV2.s_supportedOperators : CodeSearchQueryTransformerV2.s_supportedOperatorsWithProximityOperators;

    public override IExpression ParseSearchText(string searchText) => searchText != null ? new CodeQueryAdvancedParser(this.GetSupportedOperators()).Parse(searchText) : throw new ArgumentNullException(nameof (searchText));
  }
}
