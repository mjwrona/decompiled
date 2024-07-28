// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Visitor.ImageAssemblyScanVisitor
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using WebGrease.Activities;
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
  public sealed class ImageAssemblyScanVisitor : NodeVisitor
  {
    private readonly bool _ignoreImagesWithNonDefaultBackgroundSize;
    private readonly string outputUnit;
    private readonly double outputUnitFactor;
    private readonly string _cssPath;
    private readonly string _missingImage;
    private readonly ImageAssemblyScanOutput _defaultImageAssemblyScanOutput = new ImageAssemblyScanOutput();
    private readonly ImageAssemblyAnalysisLog _imageAssemblyAnalysisLog = new ImageAssemblyAnalysisLog();
    private readonly IList<ImageAssemblyScanOutput> _imageAssemblyScanOutputs = (IList<ImageAssemblyScanOutput>) new List<ImageAssemblyScanOutput>();
    private readonly HashSet<string> _imageReferencesToIgnore = new HashSet<string>();
    private readonly IDictionary<string, string> _availableImageSources;
    private readonly HashSet<string> _imagesCriteriaFailedReferences = new HashSet<string>();
    private bool imageNotFoundThrowError;

    internal IWebGreaseContext Context { get; set; }

    internal ImageAssemblyScanOutput DefaultImageAssemblyScanOutput => this._defaultImageAssemblyScanOutput;

    internal IList<ImageAssemblyScanOutput> ImageAssemblyScanOutputs => this._imageAssemblyScanOutputs;

    public ImageAssemblyScanVisitor(
      string cssPath,
      IEnumerable<string> imageReferencesToIgnore,
      bool ignoreImagesWithNonDefaultBackgroundSize = false,
      string outputUnit = "px",
      double outputUnitFactor = 1.0,
      IDictionary<string, string> availableImageSources = null,
      string missingImage = null,
      bool imageNotFoundThrowError = false)
    {
      this._missingImage = missingImage;
      this.imageNotFoundThrowError = imageNotFoundThrowError;
      this._availableImageSources = availableImageSources;
      this._imageAssemblyScanOutputs.Add(this._defaultImageAssemblyScanOutput);
      this._cssPath = cssPath.GetFullPathWithLowercase();
      this._ignoreImagesWithNonDefaultBackgroundSize = ignoreImagesWithNonDefaultBackgroundSize;
      this.outputUnit = outputUnit;
      this.outputUnitFactor = outputUnitFactor;
      if (imageReferencesToIgnore == null)
        return;
      imageReferencesToIgnore.ForEach<string>((Action<string>) (imageReferenceToIgnore => this._imageReferencesToIgnore.Add(imageReferenceToIgnore.NormalizeUrl())));
    }

    public ImageAssemblyAnalysisLog ImageAssemblyAnalysisLog => this._imageAssemblyAnalysisLog;

    public override AstNode VisitStyleSheetNode(StyleSheetNode styleSheet)
    {
      this._imagesCriteriaFailedReferences.Clear();
      styleSheet.StyleSheetRules.ForEach<StyleSheetRuleNode>((Action<StyleSheetRuleNode>) (styleSheetRuleNode => styleSheetRuleNode.Accept((NodeVisitor) this)));
      return (AstNode) styleSheet;
    }

    public override AstNode VisitRulesetNode(RulesetNode rulesetNode)
    {
      this.VisitBackgroundDeclarationNode((IEnumerable<DeclarationNode>) rulesetNode.Declarations, (AstNode) rulesetNode);
      return (AstNode) rulesetNode;
    }

    public override AstNode VisitMediaNode(MediaNode mediaNode)
    {
      mediaNode.Rulesets.ForEach<RulesetNode>((Action<RulesetNode>) (rulesetNode => rulesetNode.Accept((NodeVisitor) this)));
      mediaNode.PageNodes.ForEach<PageNode>((Action<PageNode>) (pageNode => pageNode.Accept((NodeVisitor) this)));
      return (AstNode) mediaNode;
    }

    public override AstNode VisitPageNode(PageNode pageNode)
    {
      this.VisitBackgroundDeclarationNode((IEnumerable<DeclarationNode>) pageNode.Declarations, (AstNode) pageNode);
      return (AstNode) pageNode;
    }

    public override AstNode VisitTermWithOperatorNode(TermWithOperatorNode termWithOperatorNode)
    {
      termWithOperatorNode.TermNode.Accept((NodeVisitor) this);
      return (AstNode) termWithOperatorNode;
    }

    private void VisitBackgroundDeclarationNode(
      IEnumerable<DeclarationNode> declarations,
      AstNode parent)
    {
      try
      {
        List<string> imageReferencesInInvalidDeclarations = new List<string>();
        Background backgroundNode;
        BackgroundImage backgroundImageNode;
        BackgroundPosition backgroundPositionNode;
        if (!declarations.TryGetBackgroundDeclaration(parent, out backgroundNode, out backgroundImageNode, out backgroundPositionNode, out DeclarationNode _, imageReferencesInInvalidDeclarations, this._imageReferencesToIgnore, this._imageAssemblyAnalysisLog, this.outputUnit, this.outputUnitFactor, this._ignoreImagesWithNonDefaultBackgroundSize))
          imageReferencesInInvalidDeclarations.ForEach((Action<string>) (imagesCriteriaFailedUrl =>
          {
            string url = imagesCriteriaFailedUrl.NormalizeUrl().MakeAbsoluteTo(this._cssPath);
            if (this._imageAssemblyScanOutputs.Any<ImageAssemblyScanOutput>((Func<ImageAssemblyScanOutput, bool>) (imageAssemblyScanOutput => imageAssemblyScanOutput.ImageReferencesToAssemble.Where<InputImage>((Func<InputImage, bool>) (imageReference => imageReference.AbsoluteImagePath == url)).Any<InputImage>())))
              throw new ImageAssembleException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, CssStrings.DuplicateImageReferenceWithDifferentRulesError, new object[1]
              {
                (object) url
              }));
            this._imagesCriteriaFailedReferences.Add(url);
          }));
        else if (backgroundNode != null)
        {
          this.AddImageReference(backgroundNode.Url, backgroundNode.BackgroundPosition);
        }
        else
        {
          if (backgroundImageNode == null || backgroundPositionNode == null)
            return;
          this.AddImageReference(backgroundImageNode.Url, backgroundPositionNode);
        }
      }
      catch (Exception ex)
      {
        throw new ImageAssembleException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, CssStrings.InnerExceptionSelector, new object[1]
        {
          (object) parent.PrettyPrint()
        }), ex);
      }
    }

    private void AddImageReference(string url, BackgroundPosition backgroundPosition)
    {
      string str1 = url.NormalizeUrl();
      string str2 = url;
      if (ResourcesResolver.LocalizationResourceKeyRegex.IsMatch(str1))
        return;
      url = this.GetAbsoluteImagePath(str1);
      if (this._imageReferencesToIgnore.Contains(str1) || this._imageReferencesToIgnore.Contains(Path.GetDirectoryName(str1) + "\\*"))
        return;
      if (this._imagesCriteriaFailedReferences.Any<string>((Func<string, bool>) (ir => ir.Equals(url, StringComparison.OrdinalIgnoreCase))))
        throw new ImageAssembleException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, CssStrings.DuplicateImageReferenceWithDifferentRulesError, new object[1]
        {
          (object) url
        }));
      ImagePosition imagePosition = backgroundPosition.GetImagePositionInVerticalSprite();
      bool flag = false;
      for (int index = 1; index < this._imageAssemblyScanOutputs.Count; ++index)
      {
        ImageAssemblyScanOutput assemblyScanOutput = this._imageAssemblyScanOutputs[index];
        if (assemblyScanOutput.ImageAssemblyScanInput.ImagesInBucket.Contains(url) && !assemblyScanOutput.ImageReferencesToAssemble.Any<InputImage>((Func<InputImage, bool>) (inputImage => inputImage.AbsoluteImagePath == url && inputImage.Position == imagePosition)))
        {
          assemblyScanOutput.ImageReferencesToAssemble.Add(new InputImage()
          {
            AbsoluteImagePath = url,
            Position = imagePosition,
            OriginalImagePath = str2
          });
          flag = true;
        }
      }
      if (flag || this._defaultImageAssemblyScanOutput.ImageReferencesToAssemble.Any<InputImage>((Func<InputImage, bool>) (inputImage => inputImage.AbsoluteImagePath == url && inputImage.Position == imagePosition)))
        return;
      this._defaultImageAssemblyScanOutput.ImageReferencesToAssemble.Add(new InputImage()
      {
        AbsoluteImagePath = url,
        Position = imagePosition,
        OriginalImagePath = str2
      });
    }

    private string GetAbsoluteImagePath(string relativeUrl)
    {
      string path = this._availableImageSources == null ? relativeUrl.MakeAbsoluteTo(this._cssPath) : (this._availableImageSources.ContainsKey(relativeUrl) ? this._availableImageSources[relativeUrl] : (string) null);
      if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
      {
        bool flag = relativeUrl.Equals(this._missingImage);
        if (!string.IsNullOrWhiteSpace(this._missingImage) && !flag)
          path = this.GetAbsoluteImagePath(this._missingImage);
        else if (this.imageNotFoundThrowError)
          throw new FileNotFoundException("Could not find the image file:" + relativeUrl + " (" + path + ")", path ?? string.Empty);
      }
      return path;
    }
  }
}
