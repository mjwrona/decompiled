// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.CodeSearchQueryTransformerV4
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors;
using Microsoft.VisualStudio.Services.Search.Query.Correctors;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  internal class CodeSearchQueryTransformerV4 : CodeSearchQueryTransformerV3
  {
    [StaticSafe]
    private static readonly CompositeTermCorrector s_compositeTermCorrector = new CompositeTermCorrector(new ICorrector[11]
    {
      (ICorrector) new DoubleQuotesCorrectorForExpressionType(),
      (ICorrector) new CodeFilterAliasCorrectorV2(),
      (ICorrector) new DoubleQuotesCorrectorForExpressionValue(),
      (ICorrector) new FileMetadataExtensionTermCorrectorV2(),
      (ICorrector) new FileExtensionTermCorrector(),
      (ICorrector) new WildcardTermCorrector(),
      (ICorrector) new UnrecognizedTypeTermCorrectorV2(),
      (ICorrector) new RegexCorrectorForTrigram(),
      (ICorrector) new CodeSearchUnsupportedCharacterTermCorrectorV2(),
      (ICorrector) new CodeNormalizationTermCorrectorV4(),
      (ICorrector) new WildcardTermCorrector()
    });

    public CodeSearchQueryTransformerV4(IVssRequestContext requestContext)
      : base(requestContext)
    {
    }

    protected override CompositeTermCorrector GetCompositeTermCorrector() => CodeSearchQueryTransformerV4.s_compositeTermCorrector;
  }
}
