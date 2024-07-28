// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.CommaOperator
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

namespace Microsoft.Ajax.Utilities
{
  public class CommaOperator : BinaryOperator
  {
    public CommaOperator(Context context)
      : base(context)
    {
      this.OperatorToken = JSToken.Comma;
    }

    public static AstNode CombineWithComma(Context context, AstNode operand1, AstNode operand2)
    {
      CommaOperator commaOperator = new CommaOperator(context);
      BinaryOperator binaryOperator1 = operand1 as BinaryOperator;
      BinaryOperator binaryOperator2 = operand2 as BinaryOperator;
      if (binaryOperator1 != null && binaryOperator1.OperatorToken == JSToken.Comma)
      {
        commaOperator.Operand1 = binaryOperator1.Operand1;
        if (binaryOperator2 != null && binaryOperator2.OperatorToken == JSToken.Comma)
        {
          astNodeList = new AstNodeList(binaryOperator1.Context.FlattenToStart());
          astNodeList.Append(binaryOperator1.Operand2).Append(binaryOperator2.Operand1).Append(binaryOperator2.Operand2);
        }
        else
        {
          if (!(binaryOperator1.Operand2 is AstNodeList astNodeList))
          {
            astNodeList = new AstNodeList(binaryOperator1.Operand2.Context.FlattenToStart());
            astNodeList.Append(binaryOperator1.Operand2);
          }
          astNodeList.Append(operand2);
        }
        commaOperator.Operand2 = (AstNode) astNodeList;
      }
      else if (binaryOperator2 != null && binaryOperator2.OperatorToken == JSToken.Comma)
      {
        commaOperator.Operand1 = operand1;
        if (binaryOperator2.Operand2 is AstNodeList astNodeList)
        {
          astNodeList.Insert(0, binaryOperator2.Operand1);
        }
        else
        {
          astNodeList = new AstNodeList(binaryOperator2.Context);
          astNodeList.Append(binaryOperator2.Operand1);
          astNodeList.Append(binaryOperator2.Operand2);
        }
        commaOperator.Operand2 = (AstNode) astNodeList;
      }
      else
      {
        commaOperator.Operand1 = operand1;
        commaOperator.Operand2 = operand2;
      }
      return (AstNode) commaOperator;
    }
  }
}
