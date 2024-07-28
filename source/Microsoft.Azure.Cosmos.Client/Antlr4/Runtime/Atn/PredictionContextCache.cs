// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.PredictionContextCache
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Sharpen;
using System.Collections.Generic;

namespace Antlr4.Runtime.Atn
{
  internal class PredictionContextCache
  {
    protected readonly Dictionary<PredictionContext, PredictionContext> cache = new Dictionary<PredictionContext, PredictionContext>();

    public PredictionContext Add(PredictionContext ctx)
    {
      if (ctx == PredictionContext.EMPTY)
        return (PredictionContext) PredictionContext.EMPTY;
      PredictionContext predictionContext = this.cache.Get<PredictionContext, PredictionContext>(ctx);
      if (predictionContext != null)
        return predictionContext;
      this.cache.Put<PredictionContext, PredictionContext>(ctx, ctx);
      return ctx;
    }

    public PredictionContext Get(PredictionContext ctx) => this.cache.Get<PredictionContext, PredictionContext>(ctx);

    public int Count => this.cache.Count;
  }
}
