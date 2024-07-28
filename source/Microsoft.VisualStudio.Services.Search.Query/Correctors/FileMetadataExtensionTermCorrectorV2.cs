// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.Correctors.FileMetadataExtensionTermCorrectorV2
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Query.Correctors
{
  internal class FileMetadataExtensionTermCorrectorV2 : FileMetadataExtensionTermCorrector
  {
    public override IExpression CorrectTerm(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      if (termExpression == null)
        throw new ArgumentNullException(nameof (termExpression));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (!termExpression.IsOfType("*") || termExpression.Value.StartsWith("\"", StringComparison.Ordinal) && termExpression.Value.EndsWith("\"", StringComparison.Ordinal) || !termExpression.Value.Any<char>(FileMetadataExtensionTermCorrectorV2.\u003C\u003EO.\u003C0\u003E__IsLetterOrDigit ?? (FileMetadataExtensionTermCorrectorV2.\u003C\u003EO.\u003C0\u003E__IsLetterOrDigit = new Func<char, bool>(char.IsLetterOrDigit))))
        return (IExpression) termExpression;
      return (IExpression) new OrExpression(new IExpression[2]
      {
        (IExpression) termExpression,
        (IExpression) new TermExpression(CodeFileContract.CodeContractQueryableElement.FileName.InlineFilterName(), Operator.Matches, termExpression.Value)
      });
    }
  }
}
