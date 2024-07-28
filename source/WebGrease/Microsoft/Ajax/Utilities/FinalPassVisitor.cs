// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.FinalPassVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

namespace Microsoft.Ajax.Utilities
{
  internal class FinalPassVisitor : TreeVisitor
  {
    private CodeSettings m_settings;
    private StatementStartVisitor m_statementStart;

    private FinalPassVisitor(CodeSettings codeSettings)
    {
      this.m_settings = codeSettings;
      this.m_statementStart = new StatementStartVisitor();
    }

    public static void Apply(AstNode node, CodeSettings codeSettings)
    {
      FinalPassVisitor finalPassVisitor = new FinalPassVisitor(codeSettings);
      node.Accept((IVisitor) finalPassVisitor);
    }

    public override void Visit(BinaryOperator node)
    {
      if (node == null)
        return;
      if (node.OperatorToken == JSToken.Comma && this.m_settings.IsModificationAllowed(TreeModifications.UnfoldCommaExpressionStatements) && node.Parent is Block parent1 && (parent1.Parent == null || parent1.Parent is FunctionObject parent2 && (parent2.FunctionType != FunctionType.ArrowFunction || parent1.Count > 1) || parent1.Parent is TryNode || parent1.Parent is SwitchCase || parent1.Count > 1))
        this.PossiblyBreakExpressionStatement(node, parent1);
      else
        base.Visit(node);
    }

    private void PossiblyBreakExpressionStatement(BinaryOperator node, Block parentBlock)
    {
      if (node.Operand2 is AstNodeList operand2)
        this.PossiblyBreakExpressionList(node, parentBlock, operand2);
      else if (this.CanBeBroken(node.Operand2))
      {
        AstNode operand1 = node.Operand1;
        parentBlock.ReplaceChild((AstNode) node, operand1);
        parentBlock.InsertAfter(operand1, node.Operand2);
        operand1.Accept((IVisitor) this);
      }
      else
        base.Visit(node);
    }

    private void PossiblyBreakExpressionList(
      BinaryOperator node,
      Block parentBlock,
      AstNodeList nodeList)
    {
      if (this.CanBeBroken(nodeList[0]))
      {
        int index = parentBlock.IndexOf((AstNode) node);
        AstNode operand1 = node.Operand1;
        FinalPassVisitor.RotateOpeator(node, nodeList);
        parentBlock.Insert(index, operand1);
        operand1.Accept((IVisitor) this);
      }
      else
      {
        for (int index = 1; index < nodeList.Count; ++index)
        {
          if (this.CanBeBroken(nodeList[index]))
          {
            if (index == 1)
            {
              AstNode node1 = nodeList[0];
              nodeList.RemoveAt(0);
              node.Operand2 = node1;
              if (nodeList.Count > 0)
              {
                parentBlock.InsertAfter((AstNode) node, FinalPassVisitor.CreateSplitNodeFromEnd(nodeList, 0));
                break;
              }
              break;
            }
            parentBlock.InsertAfter((AstNode) node, FinalPassVisitor.CreateSplitNodeFromEnd(nodeList, index));
            break;
          }
        }
        base.Visit(node);
      }
    }

    private static AstNode CreateSplitNodeFromEnd(AstNodeList nodeList, int ndx)
    {
      AstNode splitNodeFromEnd;
      if (ndx == nodeList.Count - 1)
      {
        splitNodeFromEnd = nodeList[ndx];
        nodeList.RemoveAt(ndx);
      }
      else if (ndx == nodeList.Count - 2)
      {
        AstNode node1 = nodeList[ndx];
        nodeList.RemoveAt(ndx);
        AstNode node2 = nodeList[ndx];
        nodeList.RemoveAt(ndx);
        CommaOperator commaOperator = new CommaOperator(node1.Context.FlattenToStart());
        commaOperator.Operand1 = node1;
        commaOperator.Operand2 = node2;
        splitNodeFromEnd = (AstNode) commaOperator;
      }
      else
      {
        AstNode node3 = nodeList[ndx];
        nodeList.RemoveAt(ndx);
        AstNodeList astNodeList;
        if (ndx == 0)
        {
          astNodeList = nodeList;
        }
        else
        {
          astNodeList = new AstNodeList(nodeList[ndx].Context.FlattenToStart());
          while (ndx < nodeList.Count)
          {
            AstNode node4 = nodeList[ndx];
            nodeList.RemoveAt(ndx);
            astNodeList.Append(node4);
          }
        }
        CommaOperator commaOperator = new CommaOperator(node3.Context.FlattenToStart());
        commaOperator.Operand1 = node3;
        commaOperator.Operand2 = (AstNode) astNodeList;
        splitNodeFromEnd = (AstNode) commaOperator;
      }
      return splitNodeFromEnd;
    }

    private static void RotateOpeator(BinaryOperator node, AstNodeList rightSide)
    {
      if (rightSide.Count == 0)
        node.Parent.ReplaceChild((AstNode) node, (AstNode) null);
      else if (rightSide.Count == 1)
        node.Parent.ReplaceChild((AstNode) node, rightSide[0]);
      else if (rightSide.Count == 2)
      {
        node.Operand1 = rightSide[0];
        node.Operand2 = rightSide[1];
      }
      else
      {
        AstNode astNode = rightSide[0];
        rightSide.RemoveAt(0);
        node.Operand1 = astNode;
      }
    }

    private bool CanBeBroken(AstNode node)
    {
      if (!this.m_statementStart.IsSafe(node))
        return false;
      return !(node is AstNodeList astNodeList) || astNodeList.Count == 0 || this.CanBeBroken(astNodeList[0]);
    }

    public override void Visit(ConstantWrapper node)
    {
      if (node == null || node.PrimitiveType != PrimitiveType.Boolean || !this.m_settings.IsModificationAllowed(TreeModifications.BooleanLiteralsToNotOperators))
        return;
      node.Parent.ReplaceChild((AstNode) node, (AstNode) new UnaryOperator(node.Context)
      {
        Operand = (AstNode) new ConstantWrapper((object) (node.ToBoolean() ? 0 : 1), PrimitiveType.Number, node.Context),
        OperatorToken = JSToken.LogicalNot
      });
    }

    public override void Visit(ImportExportSpecifier node)
    {
      if (node == null || node.LocalIdentifier == null || !node.ExternalName.IsNullOrWhiteSpace())
        return;
      IRenameable localIdentifier = node.LocalIdentifier as IRenameable;
      if (!localIdentifier.WasRenamed)
        return;
      node.ExternalName = localIdentifier.OriginalName;
    }
  }
}
