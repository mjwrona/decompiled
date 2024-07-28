// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.MergeCache
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;

namespace Antlr4.Runtime.Atn
{
  internal class MergeCache
  {
    private Dictionary<PredictionContext, Dictionary<PredictionContext, PredictionContext>> data = new Dictionary<PredictionContext, Dictionary<PredictionContext, PredictionContext>>();

    public PredictionContext Get(PredictionContext a, PredictionContext b)
    {
      Dictionary<PredictionContext, PredictionContext> dictionary;
      if (!this.data.TryGetValue(a, out dictionary))
        return (PredictionContext) null;
      PredictionContext predictionContext;
      return dictionary.TryGetValue(b, out predictionContext) ? predictionContext : (PredictionContext) null;
    }

    public void Put(PredictionContext a, PredictionContext b, PredictionContext value)
    {
      Dictionary<PredictionContext, PredictionContext> dictionary;
      if (!this.data.TryGetValue(a, out dictionary))
      {
        dictionary = new Dictionary<PredictionContext, PredictionContext>();
        this.data[a] = dictionary;
      }
      dictionary[b] = value;
    }
  }
}
