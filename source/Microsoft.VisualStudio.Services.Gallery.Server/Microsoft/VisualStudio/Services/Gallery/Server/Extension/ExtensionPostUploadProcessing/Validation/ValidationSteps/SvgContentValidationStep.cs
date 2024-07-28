// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationSteps.SvgContentValidationStep
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using HtmlAgilityPack;
using MarkdownDeep;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.Telemetry;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationSteps
{
  internal class SvgContentValidationStep : PackageValidationStepBase
  {
    private bool m_shouldValidateMarkdownFiles;
    private const string s_layer = "SvgValidationStep";
    private const string s_svgImageContentType = "image/svg";
    private const PackageValidationStepBase.ValidationStepType s_stepType = PackageValidationStepBase.ValidationStepType.SvgContentValidation;
    private const string s_registryPath = "/Configuration/Service/Gallery/Svg/SemiColonSeparatedAllowedSvgHosts";
    private const string githubBadgeRegExPattern = "^https:\\/\\/github\\.com\\/[^/]+\\/[^/]+\\/(actions\\/)?workflows\\/.*badge\\.svg";
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(2.0);
    private static readonly Regex githubBadgeUrlRegEx = new Regex("^https:\\/\\/github\\.com\\/[^/]+\\/[^/]+\\/(actions\\/)?workflows\\/.*badge\\.svg", RegexOptions.Compiled, SvgContentValidationStep.Timeout);
    private List<MarkdownFileProps> m_markdownFileProps;
    private IReadOnlyCollection<string> m_AllowedSvgHosts;

    public SvgContentValidationStep()
      : base(PackageValidationStepBase.ValidationStepType.SvgContentValidation)
    {
      this.m_markdownFileProps = new List<MarkdownFileProps>();
    }

    public override void BeginValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Stream packageStream)
    {
      this.m_result = ValidationStatus.InProgress;
      bool flag = this.IsSvgContentValidationFeatureFlagEnabled(requestContext);
      if (!flag || extension.Flags.HasFlag((Enum) PublishedExtensionFlags.Trusted))
      {
        requestContext.Trace(12061102, TraceLevel.Info, "Gallery", "SvgValidationStep", "By-passing SVG validation. Feature flag status:{0}, itemName:{1}, isBuiltIn:{2}", (object) flag, (object) extension.GetFullyQualifiedName(), (object) extension.IsBuiltIn());
        this.m_result = ValidationStatus.Success;
        this.ResultMessage = string.Empty;
      }
      else
      {
        this.m_shouldValidateMarkdownFiles = this.ShouldValidateMarkdownFiles(requestContext);
        this.m_AllowedSvgHosts = (IReadOnlyCollection<string>) this.GetAllowedSvgHosts(requestContext);
        try
        {
          bool isVsExtension = extension.IsVsExtension();
          if (isVsExtension)
          {
            this.PopulateMarkdownFileProps(requestContext, extension);
          }
          else
          {
            VSIXPackage.Parse(packageStream, new Func<ManifestFile, Stream, bool>(this.ManifestFileCallback));
            this.CheckExtensionBadgesForSvg(requestContext, extension);
          }
          if (this.m_markdownFileProps != null && this.m_markdownFileProps.Count > 0)
          {
            foreach (MarkdownFileProps markdownFileProp in this.m_markdownFileProps)
              this.ProcessMarkdownFile(requestContext, markdownFileProp.MarkdownContent, markdownFileProp.MarkdownFileFullPath, isVsExtension);
          }
          this.m_result = ValidationStatus.Success;
        }
        catch (VersionValidationException ex)
        {
          this.m_result = ValidationStatus.Failure;
          this.ResultMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error processing '{0}.{1}' {2}", (object) extension.Publisher.PublisherName, (object) extension.ExtensionName, (object) ex.Message);
          if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
            return;
          string action = "PublishTimeError";
          requestContext.GetService<IGalleryTelemetryHelperService>().PublishAppInsightsPerExtensionTelemetryHelper(requestContext, extension, action);
        }
        catch (Exception ex)
        {
          this.m_result = ValidationStatus.Success;
          this.ResultMessage = string.Empty;
          requestContext.TraceAlways(12061101, TraceLevel.Error, "Gallery", "SvgValidationStep", "Publisher:{0}, Extension:{1}, Version:{2}, ExtensionId:{3}, Exception:{4}", (object) extension.Publisher.PublisherName, (object) extension.ExtensionName, (object) extension.Versions.FirstOrDefault<ExtensionVersion>()?.Version, (object) extension.ExtensionId, (object) ex);
        }
      }
    }

    public bool ManifestFileCallback(ManifestFile manifestFile, Stream stream)
    {
      if (manifestFile != null && manifestFile.Addressable && manifestFile.ContentType != null)
      {
        if (manifestFile.ContentType.StartsWith("image/svg", StringComparison.OrdinalIgnoreCase))
          throw new VersionValidationException(GalleryResources.SvgAssetsNotSupported((object) manifestFile.FullPath));
        if (this.m_shouldValidateMarkdownFiles && (manifestFile.AssetType.Equals("Microsoft.VisualStudio.Services.Content.Details", StringComparison.OrdinalIgnoreCase) || manifestFile.AssetType.Equals("Microsoft.VisualStudio.Services.Content.Changelog", StringComparison.OrdinalIgnoreCase) || manifestFile.AssetType.Equals("Microsoft.VisualStudio.Services.Content.License", StringComparison.OrdinalIgnoreCase)))
          this.m_markdownFileProps.Add(new MarkdownFileProps()
          {
            MarkdownContent = this.GetContentFromStream(stream),
            MarkdownFileFullPath = manifestFile.FullPath
          });
      }
      return false;
    }

    private void PopulateMarkdownFileProps(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
      foreach (ExtensionFile file in extension.Versions[0].Files)
      {
        if (file.ContentType.StartsWith("image/svg", StringComparison.OrdinalIgnoreCase))
          throw new VersionValidationException(GalleryResources.SvgAssetsNotSupported((object) file.AssetType));
        if (this.m_shouldValidateMarkdownFiles && (file.AssetType.Equals("Microsoft.VisualStudio.Services.Content.Details", StringComparison.OrdinalIgnoreCase) || file.AssetType.Equals("Microsoft.VisualStudio.Services.Content.Changelog", StringComparison.OrdinalIgnoreCase) || file.AssetType.Equals("Microsoft.VisualStudio.Services.Content.License", StringComparison.OrdinalIgnoreCase)))
        {
          using (Stream stream = service.RetrieveFile(requestContext, (long) file.FileId, false, out byte[] _, out long _, out CompressionType _))
            this.m_markdownFileProps.Add(new MarkdownFileProps()
            {
              MarkdownContent = this.GetContentFromStream(stream),
              MarkdownFileFullPath = file.AssetType
            });
        }
      }
    }

    internal virtual void ProcessMarkdownFile(
      IVssRequestContext requestContext,
      string markdownContent,
      string fullPath,
      bool isVsExtension)
    {
      try
      {
        IEnumerable<HtmlNode> markDown = this.ParseMarkDown(markdownContent, "img");
        IEnumerable<HtmlNode> html = this.ParseHtml(markdownContent, "img");
        HashSet<HtmlNode> htmlNodeSet = new HashSet<HtmlNode>(markDown);
        foreach (HtmlNode htmlNode in html)
          htmlNodeSet.Add(htmlNode);
        foreach (HtmlNode htmlNode in htmlNodeSet)
        {
          string attributeValue = htmlNode.GetAttributeValue("src", "");
          if (!string.IsNullOrEmpty(attributeValue))
            this.CheckUriForSvg(requestContext, attributeValue);
        }
      }
      catch (SvgFromNonAllowedHostException ex)
      {
        throw new VersionValidationException(isVsExtension ? GalleryResources.SvgReferenceInOverview((object) ex.Message) : GalleryResources.SvgReferenceInFile((object) fullPath, (object) ex.Message), (Exception) ex);
      }
    }

    private IEnumerable<HtmlNode> ParseHtml(string htmlContent, string tag)
    {
      HtmlDocument htmlDocument = new HtmlDocument();
      htmlDocument.LoadHtml(htmlContent);
      return htmlDocument.DocumentNode.Descendants(tag);
    }

    private IEnumerable<HtmlNode> ParseMarkDown(string markdownContent, string tag)
    {
      HtmlDocument htmlDocument = new HtmlDocument();
      htmlDocument.LoadHtml(new Markdown()
      {
        SafeMode = true
      }.Transform(markdownContent));
      return htmlDocument.DocumentNode.Descendants(tag);
    }

    public bool IsGitHubBadge(Uri href)
    {
      ArgumentUtility.CheckForNull<Uri>(href, nameof (href));
      try
      {
        return SvgContentValidationStep.githubBadgeUrlRegEx.IsMatch(href.ToString());
      }
      catch (RegexMatchTimeoutException ex)
      {
        return false;
      }
    }

    internal virtual void CheckUriForSvg(IVssRequestContext requestContext, string uri)
    {
      Uri result;
      if (string.IsNullOrEmpty(uri) || uri.IndexOf(".svg", StringComparison.OrdinalIgnoreCase) <= 0 || !Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out result) || !result.IsAbsoluteUri || !result.AbsolutePath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) || this.IsAllowedSvgHost(result.Host))
        return;
      if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.AllowBadgesFromGithubWorkflows"))
        throw new SvgFromNonAllowedHostException(GalleryResources.SvgReferenceInMarkdown((object) uri));
      if (!this.IsGitHubBadge(result))
        throw new SvgFromNonAllowedHostException(GalleryResources.SvgReferenceInMarkdown((object) uri));
    }

    internal virtual bool IsAllowedSvgHost(string host)
    {
      bool flag = true;
      if (string.IsNullOrEmpty(host) || this.m_AllowedSvgHosts == null)
        return flag;
      if (host.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
        host = host.Replace("www.", "");
      return this.m_AllowedSvgHosts.Contains<string>(host, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    internal virtual void CheckExtensionBadgesForSvg(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      try
      {
        string str1;
        if (extension == null)
        {
          str1 = (string) null;
        }
        else
        {
          List<ExtensionVersion> versions = extension.Versions;
          str1 = versions != null ? versions.FirstOrDefault<ExtensionVersion>()?.Version : (string) null;
        }
        string version = str1;
        string str2;
        if (extension == null)
        {
          str2 = (string) null;
        }
        else
        {
          List<ExtensionVersion> versions = extension.Versions;
          str2 = versions != null ? versions.FirstOrDefault<ExtensionVersion>()?.TargetPlatform : (string) null;
        }
        string targetPlatform = str2;
        if (string.IsNullOrEmpty(version))
          return;
        foreach (Badge extensionBadge in this.GetExtensionBadges(requestContext.Elevate(), extension, version, targetPlatform, this.m_validationId))
          this.CheckUriForSvg(requestContext, extensionBadge.imgUri);
      }
      catch (SvgFromNonAllowedHostException ex)
      {
        throw new VersionValidationException(GalleryResources.SvgReferenceInBadge((object) ex.Message), (Exception) ex);
      }
    }

    internal virtual List<string> GetAllowedSvgHosts(IVssRequestContext requestContext)
    {
      string str = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/Svg/SemiColonSeparatedAllowedSvgHosts", string.Empty);
      requestContext.Trace(12061102, TraceLevel.Info, "Gallery", "SvgValidationStep", "Semi-colon separated allowed hosts:{0}", (object) str);
      List<string> allowedSvgHosts = (List<string>) null;
      if (!string.IsNullOrEmpty(str))
        allowedSvgHosts = ((IEnumerable<string>) str.Split(';')).ToList<string>();
      return allowedSvgHosts;
    }

    internal virtual List<Badge> GetExtensionBadges(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string version,
      string targetPlatform,
      Guid parentValidationId)
    {
      return GalleryServerUtil.GetExtensionBadges(requestContext.Elevate(), extension, version, parentValidationId, targetPlatform);
    }

    internal virtual bool IsSvgContentValidationFeatureFlagEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.BlockSvgAssets");

    internal virtual bool ShouldValidateMarkdownFiles(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.ValidateMarkdownForSvg");

    internal virtual string GetContentFromStream(Stream stream)
    {
      stream.Seek(0L, SeekOrigin.Begin);
      return new StreamReader(stream).ReadToEnd();
    }

    internal void SetAllowedSvgHosts(List<string> allowedSvgHosts) => this.m_AllowedSvgHosts = (IReadOnlyCollection<string>) allowedSvgHosts;
  }
}
