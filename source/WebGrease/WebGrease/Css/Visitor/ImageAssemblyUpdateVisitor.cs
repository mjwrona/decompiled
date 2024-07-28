// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Visitor.ImageAssemblyUpdateVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using WebGrease.Css.Ast;
using WebGrease.Css.Ast.MediaQuery;
using WebGrease.Css.Extensions;
using WebGrease.Css.ImageAssemblyAnalysis;
using WebGrease.Css.ImageAssemblyAnalysis.LogModel;
using WebGrease.Css.ImageAssemblyAnalysis.PropertyModel;
using WebGrease.Extensions;
using WebGrease.ImageAssemble;

namespace WebGrease.Css.Visitor
{
  public class ImageAssemblyUpdateVisitor : NodeVisitor
  {
    private readonly string outputUnit;
    private readonly double outputUnitFactor;
    private readonly string cssPath;
    private readonly IEnumerable<AssembledImage> inputImages;
    private readonly float dpi;
    private readonly string destinationDirectory;
    private readonly string prependToDestination;
    private readonly IDictionary<string, string> availableSourceImages;
    private readonly string missingImage;

    internal ImageAssemblyUpdateVisitor(
      string cssPath,
      IEnumerable<ImageLog> imageLogs,
      float dpi = 1f,
      string outputUnit = "px",
      double outputUnitFactor = 1.0,
      string destinationDirectory = null,
      string prependToDestination = null,
      IDictionary<string, string> availableSourceImages = null,
      string missingImage = null)
    {
      this.outputUnit = outputUnit;
      this.outputUnitFactor = outputUnitFactor;
      this.destinationDirectory = destinationDirectory;
      this.prependToDestination = prependToDestination;
      this.availableSourceImages = availableSourceImages;
      this.missingImage = missingImage;
      this.dpi = dpi;
      this.cssPath = cssPath.GetFullPathWithLowercase();
      try
      {
        this.inputImages = imageLogs.SelectMany<ImageLog, AssembledImage>((Func<ImageLog, IEnumerable<AssembledImage>>) (i => (IEnumerable<AssembledImage>) i.InputImages));
      }
      catch (Exception ex)
      {
        throw new ImageAssembleException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, CssStrings.InnerExceptionFile, new object[1]
        {
          (object) string.Join<ImageLog>(','.ToString((IFormatProvider) CultureInfo.InvariantCulture), imageLogs)
        }), ex);
      }
    }

    public override AstNode VisitStyleSheetNode(StyleSheetNode styleSheet)
    {
      List<StyleSheetRuleNode> updatedStyleSheetRuleNodes = new List<StyleSheetRuleNode>();
      styleSheet.StyleSheetRules.ForEach<StyleSheetRuleNode>((Action<StyleSheetRuleNode>) (styleSheetRuleNode => updatedStyleSheetRuleNodes.Add((StyleSheetRuleNode) styleSheetRuleNode.Accept((NodeVisitor) this))));
      return (AstNode) new StyleSheetNode(styleSheet.CharSetString, styleSheet.Imports, styleSheet.Namespaces, updatedStyleSheetRuleNodes.AsReadOnly());
    }

    public override AstNode VisitRulesetNode(RulesetNode rulesetNode) => (AstNode) new RulesetNode(rulesetNode.SelectorsGroupNode, this.UpdateDeclarations(rulesetNode.Declarations, (AstNode) rulesetNode), rulesetNode.ImportantComments);

    public override AstNode VisitMediaNode(MediaNode mediaNode)
    {
      List<RulesetNode> updatedRulesets = new List<RulesetNode>();
      List<PageNode> updatedPageNodes = new List<PageNode>();
      mediaNode.Rulesets.ForEach<RulesetNode>((Action<RulesetNode>) (rulesetNode => updatedRulesets.Add((RulesetNode) rulesetNode.Accept((NodeVisitor) this))));
      mediaNode.PageNodes.ForEach<PageNode>((Action<PageNode>) (pageNode => updatedPageNodes.Add((PageNode) pageNode.Accept((NodeVisitor) this))));
      return (AstNode) new MediaNode(mediaNode.MediaQueries, updatedRulesets.AsReadOnly(), updatedPageNodes.AsReadOnly());
    }

    public override AstNode VisitPageNode(PageNode pageNode) => (AstNode) new PageNode(pageNode.PseudoPage, this.UpdateDeclarations(pageNode.Declarations, (AstNode) pageNode));

    private static void UpdateDeclarations(
      IList<DeclarationNode> declarationNodes,
      DeclarationNode originalDeclarationNode,
      DeclarationNode updatedDeclarationNode)
    {
      declarationNodes[declarationNodes.IndexOf(originalDeclarationNode)] = updatedDeclarationNode;
    }

    private static string GetPositionString(float? value, Source? source)
    {
      if (source.HasValue)
      {
        switch (source.Value)
        {
          case Source.Left:
            return "left";
          case Source.Right:
            return "right";
          case Source.Center:
            return "center";
          case Source.Top:
            return "top";
          case Source.Bottom:
            return "bottom";
          case Source.Px:
            return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}px", new object[1]
            {
              (object) value.GetValueOrDefault()
            });
          case Source.Percentage:
            return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}%", new object[1]
            {
              (object) value.GetValueOrDefault()
            });
          case Source.NoUnits:
            return value.HasValue ? value.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture) : string.Empty;
          case Source.Rem:
            return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}rem", new object[1]
            {
              (object) value.GetValueOrDefault()
            });
          case Source.Em:
            return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}em", new object[1]
            {
              (object) value.GetValueOrDefault()
            });
          default:
            return (value.HasValue ? value.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture) : string.Empty) + source.Value.ToString();
        }
      }
      else
        return value.HasValue ? value.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture) : "center";
    }

    private static DeclarationNode CreateDebugOriginalPositionComment(
      float? xPosition,
      Source? xSource,
      float? yPosition,
      Source? ySource)
    {
      bool flag1 = xPosition.HasValue || xSource.HasValue;
      bool flag2 = yPosition.HasValue || ySource.HasValue;
      return !flag1 && !flag2 ? ImageAssemblyUpdateVisitor.CreateDebugDeclarationComment("-wg-original-position", "0 0") : ImageAssemblyUpdateVisitor.CreateDebugDeclarationComment("-wg-original-position", ImageAssemblyUpdateVisitor.GetPositionString(xPosition, xSource) + " " + ImageAssemblyUpdateVisitor.GetPositionString(yPosition, ySource));
    }

    private static DeclarationNode CreateDebugSpritePositionComment(int? xPixels, int? yPixels) => ImageAssemblyUpdateVisitor.CreateDebugDeclarationComment("-wg-sprite-position", Math.Abs(xPixels.GetValueOrDefault()).ToString() + "px " + (object) Math.Abs(yPixels.GetValueOrDefault()) + "px");

    private static DeclarationNode CreateDpiComment(double dpi) => ImageAssemblyUpdateVisitor.CreateDebugDeclarationComment("-wg-background-dpi", dpi.ToString((IFormatProvider) CultureInfo.InvariantCulture));

    private static DeclarationNode CreateDebugDeclarationComment(
      string propertyName,
      string propertyValue)
    {
      return new DeclarationNode("/* " + propertyName, new ExprNode(new TermNode(string.Empty, (string) null, propertyValue + "; */", (string) null, (FunctionNode) null, (ReadOnlyCollection<ImportantCommentNode>) null), (ReadOnlyCollection<TermWithOperatorNode>) null, (ReadOnlyCollection<ImportantCommentNode>) null), string.Empty, (ReadOnlyCollection<ImportantCommentNode>) null);
    }

    private ReadOnlyCollection<DeclarationNode> UpdateDeclarations(
      ReadOnlyCollection<DeclarationNode> declarationNodes,
      AstNode parent)
    {
      try
      {
        Background backgroundNode;
        BackgroundImage backgroundImageNode;
        BackgroundPosition backgroundPositionNode;
        DeclarationNode backgroundSize;
        if (!declarationNodes.TryGetBackgroundDeclaration(parent, out backgroundNode, out backgroundImageNode, out backgroundPositionNode, out backgroundSize, (List<string>) null, (HashSet<string>) null, (ImageAssemblyAnalysisLog) null, this.outputUnit, this.outputUnitFactor))
          return declarationNodes;
        List<DeclarationNode> declarationNodeList = new List<DeclarationNode>((IEnumerable<DeclarationNode>) declarationNodes);
        if ((double) this.dpi != 1.0)
          declarationNodeList.Insert(0, ImageAssemblyUpdateVisitor.CreateDpiComment((double) this.dpi));
        if (backgroundNode != null)
        {
          AssembledImage assembledImage;
          if (!this.TryGetAssembledImage(backgroundNode.Url, backgroundNode.BackgroundPosition, out assembledImage))
            return declarationNodes;
          declarationNodeList.Insert(0, ImageAssemblyUpdateVisitor.CreateDebugOriginalPositionComment(backgroundNode.BackgroundPosition.X, backgroundNode.BackgroundPosition.XSource, backgroundNode.BackgroundPosition.Y, backgroundNode.BackgroundPosition.YSource));
          declarationNodeList.Insert(0, ImageAssemblyUpdateVisitor.CreateDebugSpritePositionComment(assembledImage.X, assembledImage.Y));
          DeclarationNode updatedDeclarationNode = backgroundNode.UpdateBackgroundNode(assembledImage.RelativeOutputFilePath, assembledImage.X, assembledImage.Y, this.dpi);
          ImageAssemblyUpdateVisitor.UpdateDeclarations((IList<DeclarationNode>) declarationNodeList, backgroundNode.DeclarationAstNode, updatedDeclarationNode);
          this.SetBackgroundSize(declarationNodeList, backgroundSize, this.dpi, assembledImage);
        }
        else if (backgroundImageNode != null)
        {
          AssembledImage assembledImage;
          if (!this.TryGetAssembledImage(backgroundImageNode.Url, backgroundPositionNode, out assembledImage))
            return declarationNodes;
          DeclarationNode updatedDeclarationNode1 = backgroundImageNode.UpdateBackgroundImageNode(assembledImage.RelativeOutputFilePath);
          ImageAssemblyUpdateVisitor.UpdateDeclarations((IList<DeclarationNode>) declarationNodeList, backgroundImageNode.DeclarationNode, updatedDeclarationNode1);
          if (backgroundPositionNode != null)
          {
            declarationNodeList.Insert(0, ImageAssemblyUpdateVisitor.CreateDebugOriginalPositionComment(backgroundPositionNode.X, backgroundPositionNode.XSource, backgroundPositionNode.Y, backgroundPositionNode.YSource));
            declarationNodeList.Insert(0, ImageAssemblyUpdateVisitor.CreateDebugSpritePositionComment(assembledImage.X, assembledImage.Y));
            BackgroundPosition backgroundPosition = backgroundPositionNode;
            int? x = assembledImage.X;
            float? updatedX = x.HasValue ? new float?((float) x.GetValueOrDefault()) : new float?();
            int? y = assembledImage.Y;
            float? updatedY = y.HasValue ? new float?((float) y.GetValueOrDefault()) : new float?();
            double dpi = (double) this.dpi;
            DeclarationNode updatedDeclarationNode2 = backgroundPosition.UpdateBackgroundPositionNode(updatedX, updatedY, (float) dpi);
            ImageAssemblyUpdateVisitor.UpdateDeclarations((IList<DeclarationNode>) declarationNodeList, backgroundPositionNode.DeclarationNode, updatedDeclarationNode2);
          }
          else
          {
            int? x = assembledImage.X;
            float? updatedX = x.HasValue ? new float?((float) x.GetValueOrDefault()) : new float?();
            int? y = assembledImage.Y;
            float? updatedY = y.HasValue ? new float?((float) y.GetValueOrDefault()) : new float?();
            double dpi = (double) this.dpi;
            string outputUnit = this.outputUnit;
            double outputUnitFactor = this.outputUnitFactor;
            DeclarationNode newDeclaration = BackgroundPosition.CreateNewDeclaration(updatedX, updatedY, (float) dpi, outputUnit, outputUnitFactor);
            declarationNodeList.Insert(0, ImageAssemblyUpdateVisitor.CreateDebugSpritePositionComment(assembledImage.X, assembledImage.Y));
            if (newDeclaration != null)
              declarationNodeList.Add(newDeclaration);
          }
          this.SetBackgroundSize(declarationNodeList, backgroundSize, this.dpi, assembledImage);
        }
        return declarationNodeList.AsReadOnly();
      }
      catch (Exception ex)
      {
        throw new ImageAssembleException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, CssStrings.InnerExceptionSelector, new object[1]
        {
          (object) parent.PrettyPrint()
        }), ex);
      }
    }

    private void SetBackgroundSize(
      List<DeclarationNode> updatedDeclarations,
      DeclarationNode backgroundSizeNode,
      float dpiFactor,
      AssembledImage assembledImage)
    {
      if (backgroundSizeNode != null)
        updatedDeclarations.Remove(backgroundSizeNode);
      if ((double) dpiFactor == 1.0 && this.outputUnit == null || !assembledImage.SpriteHeight.HasValue || !assembledImage.SpriteWidth.HasValue)
        return;
      updatedDeclarations.AddRange(this.CreateBackgroundSizeNode(assembledImage, dpiFactor));
    }

    private IEnumerable<DeclarationNode> CreateBackgroundSizeNode(
      AssembledImage assembledImage,
      float dpiFactor)
    {
      float? number1 = new float?((float) Math.Round(((double) assembledImage.SpriteWidth ?? 0.0) * this.outputUnitFactor / (double) dpiFactor, 3));
      float? number2 = new float?((float) Math.Round(((double) assembledImage.SpriteHeight ?? 0.0) * this.outputUnitFactor / (double) dpiFactor, 3));
      DeclarationNode declarationNode = new DeclarationNode("background-size", new ExprNode(new TermNode(number1.UnaryOperator(), number1.CssUnitValue(this.outputUnit), (string) null, (string) null, (FunctionNode) null, (ReadOnlyCollection<ImportantCommentNode>) null), new List<TermWithOperatorNode>()
      {
        new TermWithOperatorNode(" ", new TermNode(number2.UnaryOperator(), number2.CssUnitValue(this.outputUnit), (string) null, (string) null, (FunctionNode) null, (ReadOnlyCollection<ImportantCommentNode>) null))
      }.ToSafeReadOnlyCollection<TermWithOperatorNode>(), (ReadOnlyCollection<ImportantCommentNode>) null), (string) null, (ReadOnlyCollection<ImportantCommentNode>) null);
      return (IEnumerable<DeclarationNode>) new DeclarationNode[2]
      {
        ImageAssemblyUpdateVisitor.CreateDebugDeclarationComment("-wg-background-size-params", " (sprite size: " + (object) assembledImage.SpriteWidth + "px " + (object) assembledImage.SpriteHeight + "px) (output unit factor: " + (object) this.outputUnitFactor + ") (dpi: " + (object) dpiFactor + ") (imageposition:" + (object) assembledImage.ImagePosition + ")"),
        declarationNode
      };
    }

    private bool TryGetAssembledImage(
      string parsedImagePath,
      BackgroundPosition backgroundPosition,
      out AssembledImage assembledImage)
    {
      if (this.availableSourceImages == null && string.IsNullOrWhiteSpace(this.cssPath))
        throw new BuildWorkflowException("Need either images or css path to be able to set a valid image file.");
      assembledImage = (AssembledImage) null;
      if (this.inputImages == null)
        return false;
      if (parsedImagePath.StartsWith("hash://", StringComparison.OrdinalIgnoreCase))
        parsedImagePath = parsedImagePath.Substring(7);
      if (this.availableSourceImages != null)
      {
        if (!this.availableSourceImages.TryGetValue(parsedImagePath.NormalizeUrl(), out parsedImagePath) && !string.IsNullOrWhiteSpace(this.missingImage))
          parsedImagePath = this.availableSourceImages.TryGetValue<string, string>(this.missingImage);
      }
      else
        parsedImagePath = parsedImagePath.MakeAbsoluteTo(this.cssPath);
      ImagePosition imagePosition = ImagePosition.Left;
      if (backgroundPosition != null)
        imagePosition = backgroundPosition.GetImagePositionInVerticalSprite();
      assembledImage = this.inputImages.FirstOrDefault<AssembledImage>((Func<AssembledImage, bool>) (inputImage =>
      {
        ImagePosition? imagePosition1 = inputImage.ImagePosition;
        ImagePosition imagePosition2 = imagePosition;
        return (imagePosition1.GetValueOrDefault() != imagePosition2 ? 0 : (imagePosition1.HasValue ? 1 : 0)) != 0 && inputImage.OriginalFilePath.Equals(parsedImagePath, StringComparison.OrdinalIgnoreCase);
      }));
      if (assembledImage == null || assembledImage.OutputFilePath == null)
        return false;
      assembledImage.RelativeOutputFilePath = !string.IsNullOrWhiteSpace(this.destinationDirectory) ? Path.Combine(this.prependToDestination, assembledImage.OutputFilePath.MakeRelativeToDirectory(this.destinationDirectory).Replace('\\', '/')) : assembledImage.OutputFilePath.MakeRelativeTo(this.cssPath);
      if (!string.IsNullOrWhiteSpace(assembledImage.RelativeOutputFilePath))
        assembledImage.RelativeOutputFilePath = assembledImage.RelativeOutputFilePath.Replace('\\', '/');
      return true;
    }
  }
}
