// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.Correctors.MeCorrector
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Correctors;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using System;

namespace Microsoft.VisualStudio.Services.Search.Query.Correctors
{
  internal class MeCorrector : TermCorrector
  {
    public override IExpression CorrectTerm(
      IVssRequestContext requestContext,
      TermExpression termExpression)
    {
      if (termExpression == null)
        throw new ArgumentNullException(nameof (termExpression));
      IExpression expression = (IExpression) null;
      if (termExpression.Value.Equals("@me", StringComparison.OrdinalIgnoreCase))
      {
        string distinctDisplayName = new Microsoft.VisualStudio.Services.Search.Query.IdentityHelper(requestContext.GetUserIdentity()).GetDistinctDisplayName();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1081420, "Query Pipeline", "Corrector", distinctDisplayName);
        expression = (IExpression) new TermExpression(termExpression.Type, termExpression.Operator, distinctDisplayName);
      }
      return expression;
    }
  }
}
