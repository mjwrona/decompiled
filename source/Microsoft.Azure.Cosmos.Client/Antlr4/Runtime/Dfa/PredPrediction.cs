// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Dfa.PredPrediction
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Atn;

namespace Antlr4.Runtime.Dfa
{
  internal class PredPrediction
  {
    public SemanticContext pred;
    public int alt;

    public PredPrediction(SemanticContext pred, int alt)
    {
      this.alt = alt;
      this.pred = pred;
    }

    public override string ToString() => "(" + this.pred?.ToString() + ", " + this.alt.ToString() + ")";
  }
}
