// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Helpers.MarkdownService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.VisualStudio.Services.Gallery.Web.Utility;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Helpers
{
  public class MarkdownService
  {
    private PublishedExtension m_extension;
    private string m_galleryResourceUrl;
    private Dictionary<string, string> m_assetTypeToSourceURL;

    public string ConvertMarkdownToHtml(
      string markdown,
      PublishedExtension extension,
      string galleryResourceUrl,
      Dictionary<string, double> blockExecutionTimeMap)
    {
      if (markdown == null)
        return string.Empty;
      this.m_extension = extension;
      this.m_galleryResourceUrl = galleryResourceUrl;
      using (new InstrumentBlock(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) nameof (ConvertMarkdownToHtml), (object) "ReplaceSpecialStrings"), blockExecutionTimeMap))
        markdown = this.ReplaceSpecialStrings(markdown);
      using (new InstrumentBlock(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) nameof (ConvertMarkdownToHtml), (object) "RenderToHtml"), blockExecutionTimeMap))
        return this.RenderToHtml(markdown);
    }

    private string RenderToHtml(string markdown)
    {
      MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAutoLinks().UsePipeTables().UseGridTables().UseAutoIdentifiers().Build();
      MarkdownDocument markdownObject = Markdown.Parse(markdown, pipeline);
      foreach (MarkdownObject descendant in markdownObject.Descendants())
      {
        if (descendant is LinkInline linkInline)
        {
          this.LinkTransformer(descendant, linkInline, this.m_galleryResourceUrl);
          if (!linkInline.IsImage)
            this.AddAdditionalLinkAttributes(descendant, linkInline.Url);
        }
        if (descendant is AutolinkInline autolinkInline)
          this.AddAdditionalLinkAttributes(descendant, autolinkInline.Url);
      }
      StringWriter writer = new StringWriter();
      HtmlRenderer renderer = new HtmlRenderer((TextWriter) writer);
      pipeline.Setup((IMarkdownRenderer) renderer);
      renderer.Render((MarkdownObject) markdownObject);
      return writer.ToString();
    }

    private string ReplaceSpecialStrings(string markdown)
    {
      markdown = this.ReplaceSpecialStringWithUrl(markdown, "CURRENT_EXTENSION_ASSET_BASE_PATH", (string) null);
      markdown = this.ReplaceSpecialStringWithUrl(markdown, "DEFAULT_ASSET_BASE_PATH", this.m_galleryResourceUrl);
      return markdown;
    }

    private string ReplaceSpecialStringWithUrl(string input, string specialString, string baseUrl)
    {
      for (Match match = new Regex("(['\"])\\{" + specialString + "\\}([^'\"]+)\\1", RegexOptions.IgnoreCase).Match(input); match.Success; match = match.NextMatch())
      {
        string assetType = match.Groups[2].Value;
        string str = baseUrl != null ? baseUrl + assetType : this.getAssetSourceURL(assetType);
        string oldValue = match.Groups[0].Value;
        input = input.Replace(oldValue, "\"" + str + "\"");
      }
      return input;
    }

    private void AddAdditionalLinkAttributes(MarkdownObject descendant, string url)
    {
      Uri result;
      if (url == null || !Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out result))
        return;
      if (result.IsAbsoluteUri && !url.ToLowerInvariant().StartsWith("mailto:"))
        descendant.GetAttributes().AddPropertyIfNotExist("target", (object) "_blank");
      descendant.GetAttributes().AddPropertyIfNotExist("rel", (object) "noreferrer noopener nofollow");
    }

    private void LinkTransformer(
      MarkdownObject descendant,
      LinkInline linkInline,
      string galleryUrl)
    {
      string url = linkInline.Url;
      if (url == null || url.ToLowerInvariant().StartsWith("mailto:") || url.StartsWith("#"))
        return;
      Uri result = (Uri) null;
      if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out result) || result.IsAbsoluteUri || string.IsNullOrWhiteSpace(result.OriginalString))
        return;
      string input = result.OriginalString;
      if (!linkInline.IsImage || !this.ContainsDefaultAssetBasePath(ref input, galleryUrl))
        input = this.getAssetSourceURL(result.OriginalString);
      linkInline.Url = input;
    }

    private bool ContainsDefaultAssetBasePath(ref string input, string baseUrl)
    {
      bool flag = false;
      Match match = new Regex("\\{DEFAULT_ASSET_BASE_PATH\\}([^)]+)", RegexOptions.IgnoreCase).Match(input);
      if (match.Success)
      {
        string oldValue = match.Groups[0].Value;
        string str = match.Groups[1].Value;
        string newValue = baseUrl + str;
        input = input.Replace(oldValue, newValue);
        flag = true;
      }
      return flag;
    }

    private string getAssetSourceURL(string assetType)
    {
      string assetSourceUrl = assetType;
      if (this.m_assetTypeToSourceURL == null)
        this.PopulateAssetToSourceURL();
      if (!string.IsNullOrWhiteSpace(assetType) && assetType.StartsWith("/"))
        assetType = assetType.Substring(1);
      if (this.m_assetTypeToSourceURL.ContainsKey(assetType))
        assetSourceUrl = this.m_assetTypeToSourceURL[assetType];
      return assetSourceUrl;
    }

    private void PopulateAssetToSourceURL()
    {
      this.m_assetTypeToSourceURL = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (ExtensionFile file in this.m_extension.Versions[0]?.Files)
        this.m_assetTypeToSourceURL[file.AssetType] = file.Source;
    }
  }
}
