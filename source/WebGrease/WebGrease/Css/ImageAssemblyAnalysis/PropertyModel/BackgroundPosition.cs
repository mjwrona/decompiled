// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.ImageAssemblyAnalysis.PropertyModel.BackgroundPosition
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using WebGrease.Css.Ast;
using WebGrease.Css.Extensions;
using WebGrease.ImageAssemble;

namespace WebGrease.Css.ImageAssemblyAnalysis.PropertyModel
{
  internal sealed class BackgroundPosition
  {
    private readonly string outputUnit = "px";
    private readonly double outputUnitFactor = 1.0;

    internal BackgroundPosition(string outputUnit, double outputUnitFactor)
    {
      this.outputUnit = outputUnit;
      this.outputUnitFactor = outputUnitFactor;
    }

    internal BackgroundPosition(
      DeclarationNode declarationNode,
      string outputUnit,
      double outputUnitFactor)
    {
      this.outputUnit = outputUnit;
      this.outputUnitFactor = outputUnitFactor;
      this.DeclarationNode = declarationNode != null ? declarationNode : throw new ArgumentNullException(nameof (declarationNode));
      ExprNode exprNode = declarationNode.ExprNode;
      this.ParseTerm(exprNode.TermNode);
      exprNode.TermsWithOperators.ForEach<TermWithOperatorNode>(new Action<TermWithOperatorNode>(this.ParseTermWithOperator));
    }

    public DeclarationNode DeclarationNode { get; private set; }

    internal float? X { get; private set; }

    internal float? Y { get; private set; }

    internal Source? XSource { get; private set; }

    internal Source? YSource { get; private set; }

    private TermNode XTermNode { get; set; }

    private TermNode YTermNode { get; set; }

    internal static DeclarationNode CreateNewDeclaration(
      float? updatedX,
      float? updatedY,
      float webGreaseBackgroundDpi,
      string outputUnit,
      double outputUnitFactor)
    {
      if (updatedX.HasValue)
      {
        float? nullable1 = updatedX;
        if (((double) nullable1.GetValueOrDefault() != 0.0 ? 0 : (nullable1.HasValue ? 1 : 0)) != 0)
        {
          float? nullable2 = updatedY;
          if (((double) nullable2.GetValueOrDefault() != 0.0 ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
            goto label_3;
        }
        float? number1 = new float?((float) Math.Round((double) updatedX.Value * outputUnitFactor / (double) webGreaseBackgroundDpi, 3));
        TermNode termNode1 = new TermNode(number1.UnaryOperator(), number1.CssUnitValue(outputUnit), (string) null, (string) null, (FunctionNode) null, (ReadOnlyCollection<ImportantCommentNode>) null);
        List<TermWithOperatorNode> withOperatorNodeList = new List<TermWithOperatorNode>();
        if (updatedY.HasValue)
        {
          float? nullable3 = updatedY;
          if (((double) nullable3.GetValueOrDefault() != 0.0 ? 1 : (!nullable3.HasValue ? 1 : 0)) != 0)
          {
            float? number2 = new float?((float) Math.Round((double) updatedY.Value * outputUnitFactor / (double) webGreaseBackgroundDpi, 3));
            TermNode termNode2 = new TermNode(number2.UnaryOperator(), number2.CssUnitValue(outputUnit), (string) null, (string) null, (FunctionNode) null, (ReadOnlyCollection<ImportantCommentNode>) null);
            withOperatorNodeList.Add(new TermWithOperatorNode(" ", termNode2));
          }
        }
        return new DeclarationNode("background-position", new ExprNode(termNode1, withOperatorNodeList.AsReadOnly(), (ReadOnlyCollection<ImportantCommentNode>) null), (string) null, (ReadOnlyCollection<ImportantCommentNode>) null);
      }
label_3:
      return (DeclarationNode) null;
    }

    internal void AddingMissingXAndY(
      float? updatedX,
      float? updatedY,
      bool isXUpdated,
      bool isYUpdated,
      int indexX,
      int indexY,
      List<TermWithOperatorNode> newTermsWithOperators,
      float webGreaseBackgroundDpi)
    {
      string unaryOperator1 = (string) null;
      string unaryOperator2 = (string) null;
      string numberBasedValue1 = "center";
      string numberBasedValue2 = "center";
      if (!isXUpdated && !isYUpdated)
      {
        float? number1 = new float?((float) Math.Round((double) updatedX.GetValueOrDefault() * this.outputUnitFactor / (double) webGreaseBackgroundDpi, 3));
        float? number2 = new float?((float) Math.Round((double) updatedY.GetValueOrDefault() * this.outputUnitFactor / (double) webGreaseBackgroundDpi, 3));
        unaryOperator1 = number1.UnaryOperator();
        unaryOperator2 = number2.UnaryOperator();
        numberBasedValue1 = number1.CssUnitValue(this.outputUnit);
        numberBasedValue2 = number2.CssUnitValue(this.outputUnit);
      }
      if (!isXUpdated)
      {
        newTermsWithOperators.Insert(indexX, new TermWithOperatorNode(" ", new TermNode(unaryOperator1, numberBasedValue1, (string) null, (string) null, (FunctionNode) null, (ReadOnlyCollection<ImportantCommentNode>) null)));
        indexY = indexX + 1;
      }
      if (isYUpdated)
        return;
      newTermsWithOperators.Insert(indexY, new TermWithOperatorNode(" ", new TermNode(unaryOperator2, numberBasedValue2, (string) null, (string) null, (FunctionNode) null, (ReadOnlyCollection<ImportantCommentNode>) null)));
    }

    internal bool IsVerticalSpriteCandidate()
    {
      if ((this.X.HasValue || this.XSource.HasValue || this.Y.HasValue || this.YSource.HasValue) && (!this.Y.HasValue || (double) this.Y.Value != 0.0))
      {
        Source? ysource = this.YSource;
        if ((ysource.GetValueOrDefault() != Source.Px ? 0 : (ysource.HasValue ? 1 : 0)) == 0)
          return false;
      }
      return true;
    }

    internal bool IsHorizontalRightAligned()
    {
      Source? xsource1 = this.XSource;
      if ((xsource1.GetValueOrDefault() != Source.Right ? 0 : (xsource1.HasValue ? 1 : 0)) == 0 || !this.Y.HasValue || (double) this.Y.Value != 0.0)
      {
        Source? xsource2 = this.XSource;
        if ((xsource2.GetValueOrDefault() != Source.Right ? 0 : (xsource2.HasValue ? 1 : 0)) != 0)
        {
          Source? ysource = this.YSource;
          if ((ysource.GetValueOrDefault() != Source.Px ? 0 : (ysource.HasValue ? 1 : 0)) != 0)
            goto label_9;
        }
        Source? xsource3 = this.XSource;
        if ((xsource3.GetValueOrDefault() != Source.Percentage ? 0 : (xsource3.HasValue ? 1 : 0)) == 0 || !this.X.HasValue || (double) this.X.Value != 100.0 || !this.Y.HasValue || (double) this.Y.Value != 0.0)
        {
          Source? xsource4 = this.XSource;
          if ((xsource4.GetValueOrDefault() != Source.Percentage ? 0 : (xsource4.HasValue ? 1 : 0)) == 0 || !this.X.HasValue || (double) this.X.Value != 100.0)
            return false;
          Source? ysource = this.YSource;
          return ysource.GetValueOrDefault() == Source.Px && ysource.HasValue;
        }
      }
label_9:
      return true;
    }

    internal bool IsHorizontalCenterAligned()
    {
      if (!this.XSource.HasValue)
      {
        Source? ysource = this.YSource;
        if ((ysource.GetValueOrDefault() != Source.Top ? 0 : (ysource.HasValue ? 1 : 0)) != 0)
          goto label_11;
      }
      Source? xsource1 = this.XSource;
      if ((xsource1.GetValueOrDefault() != Source.Center ? 0 : (xsource1.HasValue ? 1 : 0)) == 0 || !this.Y.HasValue || (double) this.Y.Value != 0.0)
      {
        Source? xsource2 = this.XSource;
        if ((xsource2.GetValueOrDefault() != Source.Center ? 0 : (xsource2.HasValue ? 1 : 0)) != 0)
        {
          Source? ysource = this.YSource;
          if ((ysource.GetValueOrDefault() != Source.Px ? 0 : (ysource.HasValue ? 1 : 0)) != 0)
            goto label_11;
        }
        Source? xsource3 = this.XSource;
        if ((xsource3.GetValueOrDefault() != Source.Percentage ? 0 : (xsource3.HasValue ? 1 : 0)) == 0 || !this.X.HasValue || (double) this.X.Value != 50.0 || !this.Y.HasValue || (double) this.Y.Value != 0.0)
        {
          Source? xsource4 = this.XSource;
          if ((xsource4.GetValueOrDefault() != Source.Percentage ? 0 : (xsource4.HasValue ? 1 : 0)) == 0 || !this.X.HasValue || (double) this.X.Value != 50.0)
            return false;
          Source? ysource = this.YSource;
          return ysource.GetValueOrDefault() == Source.Px && ysource.HasValue;
        }
      }
label_11:
      return true;
    }

    internal ImagePosition GetImagePositionInVerticalSprite()
    {
      if (this.IsHorizontalCenterAligned())
        return ImagePosition.Center;
      return this.IsHorizontalRightAligned() ? ImagePosition.Right : ImagePosition.Left;
    }

    internal void ParseTerm(TermNode termNode)
    {
      if (!string.IsNullOrWhiteSpace(termNode.StringBasedValue))
      {
        switch (termNode.StringBasedValue)
        {
          case "left":
            this.TrySwapXCoordinate();
            this.AssignX(termNode, new float?(0.0f), new int?(1), Source.Left);
            break;
          case "right":
            this.TrySwapXCoordinate();
            this.AssignX(termNode, new float?(), new int?(1), Source.Right);
            break;
          case "center":
            this.AssignXy(termNode, new float?(), new int?(1), Source.Center);
            break;
          case "top":
            this.AssignY(termNode, new float?(0.0f), new int?(1), Source.Top);
            break;
          case "bottom":
            this.AssignY(termNode, new float?(), new int?(1), Source.Bottom);
            break;
        }
      }
      if (string.IsNullOrWhiteSpace(termNode.NumberBasedValue))
        return;
      string numberBasedValue = termNode.NumberBasedValue;
      float result;
      if (numberBasedValue.EndsWith("px", StringComparison.OrdinalIgnoreCase) && numberBasedValue.Length > 2 && float.TryParse(numberBasedValue.Substring(0, numberBasedValue.Length - 2), out result))
        this.AssignXy(termNode, new float?(result), new int?(termNode.UnaryOperator.SignInt()), Source.Px);
      else if (numberBasedValue.EndsWith("rem", StringComparison.OrdinalIgnoreCase) && numberBasedValue.Length > 1 && this.outputUnit == "rem" && float.TryParse(numberBasedValue.Substring(0, numberBasedValue.Length - 3), out result))
        this.AssignXy(termNode, new float?(result / (float) this.outputUnitFactor), new int?(termNode.UnaryOperator.SignInt()), Source.Px);
      else if (numberBasedValue.EndsWith("em", StringComparison.OrdinalIgnoreCase) && numberBasedValue.Length > 1 && this.outputUnit == "em" && float.TryParse(numberBasedValue.Substring(0, numberBasedValue.Length - 2), out result))
        this.AssignXy(termNode, new float?(result / (float) this.outputUnitFactor), new int?(termNode.UnaryOperator.SignInt()), Source.Px);
      else if (numberBasedValue.EndsWith("%", StringComparison.OrdinalIgnoreCase) && numberBasedValue.Length > 1 && float.TryParse(numberBasedValue.Substring(0, numberBasedValue.Length - 1), out result))
        this.AssignXy(termNode, new float?(result), new int?(termNode.UnaryOperator.SignInt()), Source.Percentage);
      else if (numberBasedValue.TryParseZeroBasedNumberValue())
        this.AssignXy(termNode, new float?(0.0f), new int?(termNode.UnaryOperator.SignInt()), Source.NoUnits);
      else if (float.TryParse(numberBasedValue, out result))
        this.AssignXy(termNode, new float?(result), new int?(termNode.UnaryOperator.SignInt()), Source.NoUnits);
      else
        this.AssignXy(termNode, new float?(), new int?(1), Source.Unknown);
    }

    internal void ParseTermWithOperator(TermWithOperatorNode termWithOperatorNode) => this.ParseTerm(termWithOperatorNode.TermNode);

    internal bool UpdateTermForX(
      TermNode termNode,
      out TermNode updatedTermNode,
      float? updatedX,
      float webGreaseBackgroundDpi)
    {
      if (termNode == this.XTermNode)
      {
        float? x = this.X;
        if (((double) x.GetValueOrDefault() != 0.0 ? 0 : (x.HasValue ? 1 : 0)) == 0)
        {
          Source? xsource = this.XSource;
          if ((xsource.GetValueOrDefault() != Source.Px ? 0 : (xsource.HasValue ? 1 : 0)) == 0)
          {
            updatedTermNode = termNode;
            goto label_5;
          }
        }
        float? number = new float?((float) Math.Round(((double) this.X.GetValueOrDefault() + (double) updatedX.GetValueOrDefault() / (double) webGreaseBackgroundDpi) * this.outputUnitFactor, 3));
        updatedTermNode = new TermNode(number.UnaryOperator(), number.CssUnitValue(this.outputUnit), (string) null, (string) null, (FunctionNode) null, (ReadOnlyCollection<ImportantCommentNode>) null);
label_5:
        return true;
      }
      updatedTermNode = termNode;
      return false;
    }

    internal bool UpdateTermForY(
      TermNode termNode,
      out TermNode updatedTermNode,
      float? updatedY,
      float webGreaseBackgroundDpi)
    {
      if (termNode == this.YTermNode)
      {
        float? y = this.Y;
        if (((double) y.GetValueOrDefault() != 0.0 ? 0 : (y.HasValue ? 1 : 0)) == 0)
        {
          Source? ysource = this.YSource;
          if ((ysource.GetValueOrDefault() != Source.Px ? 0 : (ysource.HasValue ? 1 : 0)) == 0)
          {
            updatedTermNode = termNode;
            goto label_5;
          }
        }
        float? number = new float?((float) Math.Round(((double) this.Y.GetValueOrDefault() + (double) updatedY.GetValueOrDefault() / (double) webGreaseBackgroundDpi) * this.outputUnitFactor, 3));
        updatedTermNode = new TermNode(number.UnaryOperator(), number.CssUnitValue(this.outputUnit), (string) null, (string) null, (FunctionNode) null, (ReadOnlyCollection<ImportantCommentNode>) null);
label_5:
        return true;
      }
      updatedTermNode = termNode;
      return false;
    }

    internal DeclarationNode UpdateBackgroundPositionNode(
      float? updatedX,
      float? updatedY,
      float webGreaseBackgroundDpi)
    {
      if (this.DeclarationNode == null)
        return (DeclarationNode) null;
      bool isXUpdated = false;
      bool isYUpdated = false;
      int num1 = 0;
      int num2 = 0;
      List<TermWithOperatorNode> withOperatorNodeList = new List<TermWithOperatorNode>();
      foreach (TermWithOperatorNode withOperatorNode in this.DeclarationNode.DeclarationEnumerator())
      {
        TermNode updatedTermNode;
        if (!isXUpdated)
        {
          isXUpdated = this.UpdateTermForX(withOperatorNode.TermNode, out updatedTermNode, updatedX, webGreaseBackgroundDpi);
          if (isXUpdated)
          {
            if (isYUpdated)
            {
              withOperatorNodeList.Insert(num1, new TermWithOperatorNode(withOperatorNode.Operator, updatedTermNode.CopyTerm()));
              continue;
            }
            withOperatorNodeList.Add(new TermWithOperatorNode(withOperatorNode.Operator, updatedTermNode.CopyTerm()));
            num2 = withOperatorNodeList.Count;
            continue;
          }
        }
        if (!isYUpdated)
        {
          isYUpdated = this.UpdateTermForY(withOperatorNode.TermNode, out updatedTermNode, updatedY, webGreaseBackgroundDpi);
          if (isYUpdated)
          {
            if (isXUpdated)
            {
              withOperatorNodeList.Insert(num2, new TermWithOperatorNode(withOperatorNode.Operator, updatedTermNode.CopyTerm()));
              continue;
            }
            withOperatorNodeList.Add(new TermWithOperatorNode(withOperatorNode.Operator, updatedTermNode.CopyTerm()));
            num1 = withOperatorNodeList.Count - 1;
            continue;
          }
        }
        withOperatorNodeList.Add(withOperatorNode);
      }
      this.AddingMissingXAndY(updatedX, updatedY, isXUpdated, isYUpdated, num1, num2, withOperatorNodeList, webGreaseBackgroundDpi);
      return this.DeclarationNode.CreateDeclarationNode(withOperatorNodeList);
    }

    private void TrySwapXCoordinate()
    {
      Source? xsource = this.XSource;
      if ((xsource.GetValueOrDefault() != Source.Center ? 1 : (!xsource.HasValue ? 1 : 0)) != 0)
        return;
      this.AssignY(this.XTermNode, this.X, new int?(1), this.XSource.Value);
      this.XTermNode = (TermNode) null;
      this.X = new float?();
      this.XSource = new Source?();
    }

    private void AssignX(TermNode termNode, float? offset, int? sign, Source source)
    {
      if (this.XSource.HasValue)
        throw new ImageAssembleException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, CssStrings.TooManyLengthsError, new object[1]
        {
          (object) termNode.PrettyPrint()
        }));
      this.XTermNode = termNode;
      if (offset.HasValue && sign.HasValue)
      {
        float? nullable1 = offset;
        int? nullable2 = sign;
        this.X = nullable1.HasValue & nullable2.HasValue ? new float?(nullable1.GetValueOrDefault() * (float) nullable2.GetValueOrDefault()) : new float?();
      }
      this.XSource = new Source?(source);
    }

    private void AssignY(TermNode termNode, float? offset, int? sign, Source source)
    {
      if (this.YSource.HasValue)
        throw new ImageAssembleException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, CssStrings.TooManyLengthsError, new object[1]
        {
          (object) termNode.PrettyPrint()
        }));
      this.YTermNode = termNode;
      if (offset.HasValue && sign.HasValue)
      {
        float? nullable1 = offset;
        int? nullable2 = sign;
        this.Y = nullable1.HasValue & nullable2.HasValue ? new float?(nullable1.GetValueOrDefault() * (float) nullable2.GetValueOrDefault()) : new float?();
      }
      this.YSource = new Source?(source);
    }

    private void AssignXy(TermNode termNode, float? offset, int? sign, Source source)
    {
      if (!this.XSource.HasValue)
        this.AssignX(termNode, offset, sign, source);
      else if (!this.YSource.HasValue)
        this.AssignY(termNode, offset, sign, source);
      else
        throw new ImageAssembleException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, CssStrings.TooManyLengthsError, new object[1]
        {
          (object) termNode.PrettyPrint()
        }));
    }
  }
}
