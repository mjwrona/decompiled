// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Extensions.BackgroundAstNodeExtensions
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WebGrease.Css.Ast;
using WebGrease.Css.ImageAssemblyAnalysis;
using WebGrease.Css.ImageAssemblyAnalysis.LogModel;
using WebGrease.Css.ImageAssemblyAnalysis.PropertyModel;

namespace WebGrease.Css.Extensions
{
  public static class BackgroundAstNodeExtensions
  {
    internal static bool TryGetBackgroundDeclaration(
      this IEnumerable<DeclarationNode> declarationAstNodes,
      AstNode parentAstNode,
      out Background backgroundNode,
      out BackgroundImage backgroundImageNode,
      out BackgroundPosition backgroundPositionNode,
      out DeclarationNode backgroundSize,
      List<string> imageReferencesInInvalidDeclarations,
      HashSet<string> imageReferencesToIgnore,
      ImageAssemblyAnalysisLog imageAssemblyAnalysisLog,
      string outputUnit,
      double outputUnitFactor,
      bool ignoreImagesWithNonDefaultBackgroundSize = false)
    {
      backgroundNode = (Background) null;
      backgroundImageNode = (BackgroundImage) null;
      backgroundPositionNode = (BackgroundPosition) null;
      backgroundSize = (DeclarationNode) null;
      if (BackgroundImage.HasMultipleUrls(parentAstNode.MinifyPrint()))
      {
        imageAssemblyAnalysisLog.SafeAdd(parentAstNode, failureReason: new FailureReason?(FailureReason.MultipleUrls));
        return false;
      }
      DeclarationNode declarationNode1 = declarationAstNodes.FirstOrDefault<DeclarationNode>((Func<DeclarationNode, bool>) (d => d.Property == "-wg-spriting"));
      if (declarationNode1 != null && declarationNode1.ExprNode.TermNode.StringBasedValue == "ignore")
      {
        imageAssemblyAnalysisLog.SafeAdd(parentAstNode, failureReason: new FailureReason?(FailureReason.SpritingIgnore));
        return false;
      }
      Dictionary<string, DeclarationNode> declarationProperties = declarationAstNodes.LoadDeclarationPropertiesDictionary();
      DeclarationNode declarationNode2;
      if (declarationProperties.TryGetValue("background", out declarationNode2))
      {
        if (declarationProperties.ContainsKey("background-repeat") || declarationProperties.ContainsKey("background-image") || declarationProperties.ContainsKey("background-position"))
          throw new ImageAssembleException(CssStrings.DuplicateBackgroundFormatError);
        Background background = new Background(declarationNode2, outputUnit, outputUnitFactor);
        bool shouldIgnore;
        if (!background.BackgroundImage.VerifyBackgroundUrl(parentAstNode, imageReferencesToIgnore, imageAssemblyAnalysisLog, out shouldIgnore) || shouldIgnore)
          return false;
        if (!background.BackgroundRepeat.VerifyBackgroundNoRepeat())
        {
          imageAssemblyAnalysisLog.SafeAdd(parentAstNode, background.Url, new FailureReason?(FailureReason.BackgroundRepeatInvalid));
          BackgroundAstNodeExtensions.UpdateFailedUrlsList(background.Url, (ICollection<string>) imageReferencesInInvalidDeclarations);
          return false;
        }
        if (!background.BackgroundPosition.IsVerticalSpriteCandidate())
        {
          imageAssemblyAnalysisLog.SafeAdd(parentAstNode, background.Url, new FailureReason?(FailureReason.IncorrectPosition));
          BackgroundAstNodeExtensions.UpdateFailedUrlsList(background.Url, (ICollection<string>) imageReferencesInInvalidDeclarations);
          return false;
        }
        if (!BackgroundAstNodeExtensions.TryGetBackgroundSize(ignoreImagesWithNonDefaultBackgroundSize, (IDictionary<string, DeclarationNode>) declarationProperties, out backgroundSize))
        {
          imageAssemblyAnalysisLog.SafeAdd(parentAstNode, background.Url, new FailureReason?(FailureReason.BackgroundSizeIsSetToNonDefaultValue));
          BackgroundAstNodeExtensions.UpdateFailedUrlsList(background.Url, (ICollection<string>) imageReferencesInInvalidDeclarations);
          return false;
        }
        backgroundNode = background;
        imageAssemblyAnalysisLog.SafeAdd(parentAstNode, background.Url);
        return true;
      }
      if (!declarationProperties.TryGetValue("background-image", out declarationNode2))
        return false;
      BackgroundImage backgroundImage = new BackgroundImage(declarationNode2);
      bool shouldIgnore1;
      if (!backgroundImage.VerifyBackgroundUrl(parentAstNode, imageReferencesToIgnore, imageAssemblyAnalysisLog, out shouldIgnore1) || shouldIgnore1)
        return false;
      DeclarationNode declarationNode3;
      if (!declarationProperties.TryGetValue("background-repeat", out declarationNode3))
      {
        imageAssemblyAnalysisLog.SafeAdd(parentAstNode, backgroundImage.Url, new FailureReason?(FailureReason.NoRepeat));
        BackgroundAstNodeExtensions.UpdateFailedUrlsList(backgroundImage.Url, (ICollection<string>) imageReferencesInInvalidDeclarations);
        return false;
      }
      if (!new BackgroundRepeat(declarationNode3).VerifyBackgroundNoRepeat())
      {
        imageAssemblyAnalysisLog.SafeAdd(parentAstNode, backgroundImage.Url, new FailureReason?(FailureReason.BackgroundRepeatInvalid));
        BackgroundAstNodeExtensions.UpdateFailedUrlsList(backgroundImage.Url, (ICollection<string>) imageReferencesInInvalidDeclarations);
        return false;
      }
      if (!BackgroundAstNodeExtensions.TryGetBackgroundSize(ignoreImagesWithNonDefaultBackgroundSize, (IDictionary<string, DeclarationNode>) declarationProperties, out backgroundSize))
      {
        imageAssemblyAnalysisLog.SafeAdd(parentAstNode, backgroundImage.Url, new FailureReason?(FailureReason.BackgroundSizeIsSetToNonDefaultValue));
        BackgroundAstNodeExtensions.UpdateFailedUrlsList(backgroundImage.Url, (ICollection<string>) imageReferencesInInvalidDeclarations);
        return false;
      }
      DeclarationNode declarationNode4;
      if (declarationProperties.TryGetValue("background-position", out declarationNode4))
      {
        BackgroundPosition backgroundPosition = new BackgroundPosition(declarationNode4, outputUnit, outputUnitFactor);
        if (!backgroundPosition.IsVerticalSpriteCandidate())
        {
          imageAssemblyAnalysisLog.SafeAdd(parentAstNode, backgroundImage.Url, new FailureReason?(FailureReason.IncorrectPosition));
          BackgroundAstNodeExtensions.UpdateFailedUrlsList(backgroundImage.Url, (ICollection<string>) imageReferencesInInvalidDeclarations);
          return false;
        }
        backgroundImageNode = backgroundImage;
        backgroundPositionNode = backgroundPosition;
        imageAssemblyAnalysisLog.SafeAdd(parentAstNode, backgroundImage.Url);
        return true;
      }
      backgroundImageNode = backgroundImage;
      imageAssemblyAnalysisLog.SafeAdd(parentAstNode, backgroundImageNode.Url);
      return true;
    }

    internal static void SafeAdd(
      this ImageAssemblyAnalysisLog imageAssemblyAnalysisLog,
      AstNode parentAstNode,
      string image = null,
      FailureReason? failureReason = null)
    {
      if (imageAssemblyAnalysisLog == null)
        return;
      imageAssemblyAnalysisLog.Add(new WebGrease.Css.ImageAssemblyAnalysis.LogModel.ImageAssemblyAnalysis()
      {
        AstNode = parentAstNode,
        Image = image,
        FailureReason = failureReason
      });
    }

    internal static IEnumerable<TermWithOperatorNode> DeclarationEnumerator(
      this DeclarationNode declarationNode)
    {
      if (declarationNode != null)
      {
        yield return new TermWithOperatorNode(" ", declarationNode.ExprNode.TermNode);
        foreach (TermWithOperatorNode termWithOperatorNode in declarationNode.ExprNode.TermsWithOperators)
          yield return termWithOperatorNode;
      }
    }

    internal static TermNode CopyTerm(this TermNode termNode) => termNode != null ? new TermNode(termNode.UnaryOperator, termNode.NumberBasedValue, termNode.StringBasedValue, termNode.Hexcolor, termNode.FunctionNode, termNode.ImportantComments) : (TermNode) null;

    internal static DeclarationNode CreateDeclarationNode(
      this DeclarationNode declarationNode,
      List<TermWithOperatorNode> termWithOperatorNodes)
    {
      if (declarationNode == null || termWithOperatorNodes == null || termWithOperatorNodes.Count <= 0)
        return declarationNode;
      TermNode termNode = termWithOperatorNodes[0].TermNode;
      termWithOperatorNodes.RemoveAt(0);
      return new DeclarationNode(declarationNode.Property, new ExprNode(termNode, termWithOperatorNodes.AsReadOnly(), (ReadOnlyCollection<ImportantCommentNode>) null), declarationNode.Prio, declarationNode.ImportantComments);
    }

    private static bool TryGetBackgroundSize(
      bool ignoreImagesWithNonDefaultBackgroundSize,
      IDictionary<string, DeclarationNode> declarationProperties,
      out DeclarationNode backgroundSize)
    {
      if (declarationProperties.TryGetValue("background-size", out backgroundSize) && ignoreImagesWithNonDefaultBackgroundSize)
      {
        string str = backgroundSize.ExprNode.MinifyPrint();
        if (!str.Equals("auto") && !str.Equals("auto auto"))
          return false;
      }
      return true;
    }

    private static void UpdateFailedUrlsList(
      string parsedUrl,
      ICollection<string> imagesCriteriaFailedUrls)
    {
      if (imagesCriteriaFailedUrls == null || string.IsNullOrWhiteSpace(parsedUrl))
        return;
      imagesCriteriaFailedUrls.Add(parsedUrl);
    }

    private static Dictionary<string, DeclarationNode> LoadDeclarationPropertiesDictionary(
      this IEnumerable<DeclarationNode> declarationNodes)
    {
      Dictionary<string, List<DeclarationNode>> declarationPropertyNames = new Dictionary<string, List<DeclarationNode>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      declarationNodes.ForEach<DeclarationNode>((Action<DeclarationNode>) (declarationNode =>
      {
        string property = declarationNode.Property;
        List<DeclarationNode> declarationNodeList;
        if (!declarationPropertyNames.TryGetValue(property, out declarationNodeList))
          declarationPropertyNames[property] = declarationNodeList = new List<DeclarationNode>();
        declarationNodeList.Add(declarationNode);
      }));
      return declarationPropertyNames.ToDictionary<KeyValuePair<string, List<DeclarationNode>>, string, DeclarationNode>((Func<KeyValuePair<string, List<DeclarationNode>>, string>) (d => d.Key), (Func<KeyValuePair<string, List<DeclarationNode>>, DeclarationNode>) (d => d.Value.LastOrDefault<DeclarationNode>()));
    }
  }
}
