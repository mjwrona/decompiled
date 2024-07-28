// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.BundleTextContent
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  public class BundleTextContent : 
    IBundledScriptContent,
    IBundledContent<ScriptBundleDefinition, BundledScriptFile>,
    IBundledCSSContent,
    IBundledContent<CSSBundleDefinition, BundledCSSFile>
  {
    public string Text { get; set; }

    public string GetDefinitionHash() => this.Text;

    public string GetContent(
      IVssRequestContext requestContext,
      ScriptBundleDefinition bundleDefinition,
      BundledScriptFile bundledFile,
      UrlHelper urlHelper,
      Dictionary<string, string[]> contentCache)
    {
      return this.GetContent();
    }

    public string GetContent(
      IVssRequestContext requestContext,
      CSSBundleDefinition bundleDefinition,
      BundledCSSFile bundledFile,
      UrlHelper urlHelper,
      Dictionary<string, string[]> contentCache)
    {
      return this.GetContent();
    }

    private string GetContent() => this.Text;

    public List<byte[]> GetFileHash(
      IVssRequestContext requestContext,
      ScriptBundleDefinition bundleDefinition,
      BundledScriptFile bundledFile,
      UrlHelper urlHelper,
      Dictionary<string, string[]> contentCache,
      out int contentLength)
    {
      return this.GetFileHash(out contentLength);
    }

    public List<byte[]> GetFileHash(
      IVssRequestContext requestContext,
      CSSBundleDefinition bundleDefinition,
      BundledCSSFile bundledFile,
      UrlHelper urlHelper,
      Dictionary<string, string[]> contentCache,
      out int contentLength)
    {
      return this.GetFileHash(out contentLength);
    }

    public List<byte[]> GetFileHash(out int contentLength)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(this.Text);
      contentLength = bytes.Length;
      return new List<byte[]>()
      {
        BundlingHelper.CalculateHashFromBytes(bytes)
      };
    }

    public IEnumerable<IBundleStreamProvider> GetBundleStreamProviders(
      IVssRequestContext requestContext,
      ScriptBundleDefinition bundleDefinition,
      BundledScriptFile bundledFile,
      UrlHelper urlHelper)
    {
      return this.GetBundleStreamProviders();
    }

    public IEnumerable<IBundleStreamProvider> GetBundleStreamProviders(
      IVssRequestContext requestContext,
      CSSBundleDefinition bundleDefinition,
      BundledCSSFile bundledFile,
      UrlHelper urlHelper)
    {
      return this.GetBundleStreamProviders();
    }

    private IEnumerable<IBundleStreamProvider> GetBundleStreamProviders() => (IEnumerable<IBundleStreamProvider>) new IBundleStreamProvider[1]
    {
      (IBundleStreamProvider) new BundleTextStreamProvider(this.Text)
    };
  }
}
