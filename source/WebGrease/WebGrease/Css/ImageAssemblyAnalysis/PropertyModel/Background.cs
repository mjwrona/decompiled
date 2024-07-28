// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.ImageAssemblyAnalysis.PropertyModel.Background
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using WebGrease.Css.Ast;
using WebGrease.Css.Extensions;

namespace WebGrease.Css.ImageAssemblyAnalysis.PropertyModel
{
  internal sealed class Background
  {
    internal Background(
      DeclarationNode declarationAstNode,
      string outputUnit,
      double outputUnitFactor)
    {
      this.DeclarationAstNode = declarationAstNode;
      this.BackgroundImage = new BackgroundImage();
      this.BackgroundPosition = new BackgroundPosition(outputUnit, outputUnitFactor);
      this.BackgroundRepeat = new BackgroundRepeat();
      ExprNode exprNode = declarationAstNode.ExprNode;
      TermNode termNode = exprNode.TermNode;
      this.BackgroundImage.ParseTerm(termNode);
      this.BackgroundPosition.ParseTerm(termNode);
      this.BackgroundRepeat.ParseTerm(termNode);
      exprNode.TermsWithOperators.ForEach<TermWithOperatorNode>((Action<TermWithOperatorNode>) (termWithOperator =>
      {
        this.BackgroundImage.ParseTermWithOperator(termWithOperator);
        this.BackgroundPosition.ParseTermWithOperator(termWithOperator);
        this.BackgroundRepeat.ParseTermWithOperator(termWithOperator);
      }));
    }

    public DeclarationNode DeclarationAstNode { get; private set; }

    internal BackgroundImage BackgroundImage { get; private set; }

    internal BackgroundPosition BackgroundPosition { get; private set; }

    internal BackgroundRepeat BackgroundRepeat { get; private set; }

    internal string Url => this.BackgroundImage.Url;

    internal DeclarationNode UpdateBackgroundNode(
      string updatedUrl,
      int? updatedX,
      int? updatedY,
      float webGreaseBackgroundDpi)
    {
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      int index1 = 0;
      int index2 = 0;
      List<TermWithOperatorNode> termWithOperatorNodes = new List<TermWithOperatorNode>();
      foreach (TermWithOperatorNode withOperatorNode in this.DeclarationAstNode.DeclarationEnumerator())
      {
        TermNode updatedTermNode;
        if (!flag1)
        {
          flag1 = this.BackgroundImage.UpdateTermForUrl(withOperatorNode.TermNode, out updatedTermNode, updatedUrl);
          if (flag1)
          {
            termWithOperatorNodes.Add(new TermWithOperatorNode(withOperatorNode.Operator, updatedTermNode.CopyTerm()));
            continue;
          }
        }
        if (!flag2)
        {
          BackgroundPosition backgroundPosition = this.BackgroundPosition;
          TermNode termNode = withOperatorNode.TermNode;
          ref TermNode local = ref updatedTermNode;
          int? nullable = updatedX;
          float? updatedX1 = nullable.HasValue ? new float?((float) nullable.GetValueOrDefault()) : new float?();
          double webGreaseBackgroundDpi1 = (double) webGreaseBackgroundDpi;
          flag2 = backgroundPosition.UpdateTermForX(termNode, out local, updatedX1, (float) webGreaseBackgroundDpi1);
          if (flag2)
          {
            if (flag3)
            {
              termWithOperatorNodes.Insert(index1, new TermWithOperatorNode(withOperatorNode.Operator, updatedTermNode.CopyTerm()));
              continue;
            }
            termWithOperatorNodes.Add(new TermWithOperatorNode(withOperatorNode.Operator, updatedTermNode.CopyTerm()));
            index2 = termWithOperatorNodes.Count;
            continue;
          }
        }
        if (!flag3)
        {
          BackgroundPosition backgroundPosition = this.BackgroundPosition;
          TermNode termNode = withOperatorNode.TermNode;
          ref TermNode local = ref updatedTermNode;
          int? nullable = updatedY;
          float? updatedY1 = nullable.HasValue ? new float?((float) nullable.GetValueOrDefault()) : new float?();
          double webGreaseBackgroundDpi2 = (double) webGreaseBackgroundDpi;
          flag3 = backgroundPosition.UpdateTermForY(termNode, out local, updatedY1, (float) webGreaseBackgroundDpi2);
          if (flag3)
          {
            if (flag2)
            {
              termWithOperatorNodes.Insert(index2, new TermWithOperatorNode(withOperatorNode.Operator, updatedTermNode.CopyTerm()));
              continue;
            }
            termWithOperatorNodes.Add(new TermWithOperatorNode(withOperatorNode.Operator, updatedTermNode.CopyTerm()));
            index1 = termWithOperatorNodes.Count - 1;
            continue;
          }
        }
        termWithOperatorNodes.Add(withOperatorNode);
      }
      BackgroundPosition backgroundPosition1 = this.BackgroundPosition;
      int? nullable1 = updatedX;
      float? updatedX2 = nullable1.HasValue ? new float?((float) nullable1.GetValueOrDefault()) : new float?();
      int? nullable2 = updatedY;
      float? updatedY2 = nullable2.HasValue ? new float?((float) nullable2.GetValueOrDefault()) : new float?();
      int num1 = flag2 ? 1 : 0;
      int num2 = flag3 ? 1 : 0;
      int indexX = index1;
      int indexY = index2;
      List<TermWithOperatorNode> newTermsWithOperators = termWithOperatorNodes;
      double webGreaseBackgroundDpi3 = (double) webGreaseBackgroundDpi;
      backgroundPosition1.AddingMissingXAndY(updatedX2, updatedY2, num1 != 0, num2 != 0, indexX, indexY, newTermsWithOperators, (float) webGreaseBackgroundDpi3);
      return this.DeclarationAstNode.CreateDeclarationNode(termWithOperatorNodes);
    }
  }
}
