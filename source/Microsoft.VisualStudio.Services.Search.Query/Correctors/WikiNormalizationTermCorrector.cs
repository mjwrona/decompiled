// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.Correctors.WikiNormalizationTermCorrector
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Indexer;
using System;

namespace Microsoft.VisualStudio.Services.Search.Query.Correctors
{
  internal class WikiNormalizationTermCorrector : TermCorrector
  {
    public override IExpression CorrectTerm(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      if (termExpression == null)
        throw new ArgumentNullException(nameof (termExpression));
      int? indexVersion;
      if (!requestContext.TryGetQueryIndexVersion((EntityType) WikiEntityType.GetInstance(), out indexVersion))
        indexVersion = new int?();
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        int? nullable = indexVersion;
        int num = 10;
        if (nullable.GetValueOrDefault() >= num & nullable.HasValue)
        {
          termExpression.Value = termExpression.Value.NormalizeStringWithCurrentCulture();
          goto label_8;
        }
      }
      termExpression.Value = !termExpression.IsOfType("*") ? termExpression.Value.NormalizeString() : termExpression.Value.NormalizeStringAndReplaceTurkishDottedI();
label_8:
      return (IExpression) termExpression;
    }
  }
}
