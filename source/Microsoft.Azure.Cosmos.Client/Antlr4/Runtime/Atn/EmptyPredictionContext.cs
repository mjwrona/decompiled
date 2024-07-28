// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.EmptyPredictionContext
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Antlr4.Runtime.Atn
{
  internal sealed class EmptyPredictionContext : SingletonPredictionContext
  {
    internal EmptyPredictionContext()
      : base((PredictionContext) null, PredictionContext.EMPTY_RETURN_STATE)
    {
    }

    public override PredictionContext GetParent(int index) => (PredictionContext) null;

    public override int GetReturnState(int index) => this.returnState;

    public override int Size => 1;

    public override bool IsEmpty => true;

    public override bool Equals(object o) => this == o;

    public override string ToString() => "$";

    public override string[] ToStrings(IRecognizer recognizer, int currentState) => new string[1]
    {
      "[]"
    };

    public override string[] ToStrings(
      IRecognizer recognizer,
      PredictionContext stop,
      int currentState)
    {
      return new string[1]{ "[]" };
    }
  }
}
