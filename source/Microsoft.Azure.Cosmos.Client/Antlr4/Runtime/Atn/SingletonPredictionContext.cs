// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.SingletonPredictionContext
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;

namespace Antlr4.Runtime.Atn
{
  internal class SingletonPredictionContext : PredictionContext
  {
    [NotNull]
    public readonly PredictionContext parent;
    public readonly int returnState;

    public static PredictionContext Create(PredictionContext parent, int returnState) => returnState == PredictionContext.EMPTY_RETURN_STATE && parent == null ? (PredictionContext) PredictionContext.EMPTY : (PredictionContext) new SingletonPredictionContext(parent, returnState);

    internal SingletonPredictionContext(PredictionContext parent, int returnState)
      : base(PredictionContext.CalculateHashCode(parent, returnState))
    {
      this.parent = parent;
      this.returnState = returnState;
    }

    public override PredictionContext GetParent(int index) => this.parent;

    public override int GetReturnState(int index) => this.returnState;

    public override int Size => 1;

    public override bool IsEmpty => false;

    public override bool Equals(object o)
    {
      if (o == this)
        return true;
      if (!(o is SingletonPredictionContext) || this.GetHashCode() != o.GetHashCode())
        return false;
      SingletonPredictionContext predictionContext = (SingletonPredictionContext) o;
      return this.returnState == predictionContext.returnState && this.parent.Equals((object) predictionContext.parent);
    }

    public override string ToString()
    {
      string str = this.parent != null ? this.parent.ToString() : "";
      if (str.Length != 0)
        return this.returnState.ToString() + " " + str;
      return this.returnState == PredictionContext.EMPTY_RETURN_STATE ? "$" : this.returnState.ToString();
    }
  }
}
