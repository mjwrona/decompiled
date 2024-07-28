// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs.MarkerBinaryExpression
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

namespace Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs
{
  public class MarkerBinaryExpression : MarkerExpression
  {
    public MarkerExpression Left { get; }

    public Operator Operator { get; }

    public MarkerExpression Right { get; }

    public MarkerBinaryExpression(
      MarkerExpression left,
      Operator @operator,
      MarkerExpression right)
    {
      this.Left = left;
      this.Operator = @operator;
      this.Right = right;
    }

    public override string Dump(string indent, string newline)
    {
      string indent1 = string.IsNullOrEmpty(indent) ? "" : indent + "    ";
      return "MarkerBinaryExpression(" + newline + indent1 + this.Left.Dump(indent1, newline) + "," + newline + indent1 + this.Operator.Dump(indent1, newline) + "," + newline + indent1 + this.Right.Dump(indent1, newline) + newline + indent + ")";
    }

    public override string ToString()
    {
      return string.Format("{0} {1} {2}", (object) FormatOperand(this.Left), (object) this.Operator, (object) FormatOperand(this.Right));

      static string FormatOperand(MarkerExpression markerExpression)
      {
        int num;
        switch (markerExpression)
        {
          case MarkerVariable _:
            num = 1;
            break;
          case MarkerBinaryExpression binaryExpression:
            if (binaryExpression.Left is MarkerVariable)
            {
              num = binaryExpression.Right is MarkerVariable ? 1 : 0;
              break;
            }
            goto default;
          default:
            num = 0;
            break;
        }
        return num == 0 ? string.Format("({0})", (object) markerExpression) : markerExpression.ToString();
      }
    }
  }
}
