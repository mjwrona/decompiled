// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Atn.ArrayPredictionContext
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Sharpen;
using System.Text;

namespace Antlr4.Runtime.Atn
{
  internal class ArrayPredictionContext : PredictionContext
  {
    public readonly PredictionContext[] parents;
    public readonly int[] returnStates;

    public ArrayPredictionContext(SingletonPredictionContext a)
      : this(new PredictionContext[1]{ a.parent }, new int[1]
      {
        a.returnState
      })
    {
    }

    public ArrayPredictionContext(PredictionContext[] parents, int[] returnStates)
      : base(PredictionContext.CalculateHashCode(parents, returnStates))
    {
      this.parents = parents;
      this.returnStates = returnStates;
    }

    public override bool IsEmpty => this.returnStates[0] == PredictionContext.EMPTY_RETURN_STATE;

    public override int Size => this.returnStates.Length;

    public override PredictionContext GetParent(int index) => this.parents[index];

    public override int GetReturnState(int index) => this.returnStates[index];

    public override bool Equals(object o)
    {
      if (this == o)
        return true;
      if (!(o is ArrayPredictionContext) || this.GetHashCode() != o.GetHashCode())
        return false;
      ArrayPredictionContext predictionContext = (ArrayPredictionContext) o;
      return Arrays.Equals<int>(this.returnStates, predictionContext.returnStates) && Arrays.Equals<PredictionContext>(this.parents, predictionContext.parents);
    }

    public override string ToString()
    {
      if (this.IsEmpty)
        return "[]";
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("[");
      for (int index = 0; index < this.returnStates.Length; ++index)
      {
        if (index > 0)
          stringBuilder.Append(", ");
        if (this.returnStates[index] == PredictionContext.EMPTY_RETURN_STATE)
        {
          stringBuilder.Append("$");
        }
        else
        {
          stringBuilder.Append(this.returnStates[index]);
          if (this.parents[index] != null)
          {
            stringBuilder.Append(' ');
            stringBuilder.Append(this.parents[index].ToString());
          }
          else
            stringBuilder.Append("null");
        }
      }
      stringBuilder.Append("]");
      return stringBuilder.ToString();
    }
  }
}
