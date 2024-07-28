// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.CSSBundleDefinition
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  public class CSSBundleDefinition : BundleDefinition
  {
    public IList<IBundledCSSContent> ContentList { get; set; }

    public string ThemeName { get; set; }

    public bool DebugStyles { get; set; }

    public IEnumerable<string> CSSModulePrefixes { get; set; }

    public CSSBundleDefinition()
    {
    }

    public CSSBundleDefinition(CSSBundleDefinition original)
    {
      this.ContentList = original.ContentList;
      this.CSSModulePrefixes = original.CSSModulePrefixes;
      this.DebugStyles = original.DebugStyles;
      this.Diagnose = original.Diagnose;
      this.FriendlyName = original.FriendlyName;
      this.StaticContentVersion = original.StaticContentVersion;
      this.ThemeName = original.ThemeName;
    }

    protected override void AppendContentHash(StringBuilder contentHashBuilder)
    {
      contentHashBuilder.Append(this.DebugStyles ? "debug:" : "min:");
      if (!string.IsNullOrEmpty(this.ThemeName))
        contentHashBuilder.Append(this.ThemeName);
      contentHashBuilder.Append(':');
      if (!string.IsNullOrEmpty(this.StaticContentVersion))
        contentHashBuilder.Append(this.StaticContentVersion);
      contentHashBuilder.Append(':');
      if (!string.IsNullOrEmpty(this.CacheKey))
      {
        contentHashBuilder.Append(this.CacheKey);
        contentHashBuilder.Append(':');
      }
      if (this.ContentList == null)
        return;
      foreach (IBundledCSSContent content in (IEnumerable<IBundledCSSContent>) this.ContentList)
      {
        contentHashBuilder.Append(content.GetDefinitionHash());
        contentHashBuilder.Append(';');
      }
    }

    public override string GetBundleContent(
      IVssRequestContext requestContext,
      BundledFile bundledFile,
      UrlHelper urlHelper,
      Dictionary<string, string[]> contentCache)
    {
      return BundlingHelper.JoinWithNewLine((IEnumerable<string>) this.ContentList.Select<IBundledCSSContent, string>((Func<IBundledCSSContent, string>) (c => c.GetContent(requestContext, this, bundledFile as BundledCSSFile, urlHelper, contentCache))).ToArray<string>());
    }

    public override IEnumerable<IBundleStreamProvider> GetBundleStreamProviders(
      IVssRequestContext requestContext,
      BundledFile bundledFile,
      UrlHelper urlHelper)
    {
      List<IBundleStreamProvider> bundleStreamProviders = new List<IBundleStreamProvider>();
      foreach (IBundledCSSContent content in (IEnumerable<IBundledCSSContent>) this.ContentList)
        bundleStreamProviders.AddRange(content.GetBundleStreamProviders(requestContext, this, bundledFile as BundledCSSFile, urlHelper));
      return (IEnumerable<IBundleStreamProvider>) bundleStreamProviders;
    }

    public override string GetBundleHashCode(
      IVssRequestContext requestContext,
      BundledFile bundledFile,
      UrlHelper urlHelper,
      Dictionary<string, string[]> contentCache,
      out int contentLength)
    {
      List<byte[]> contentHashList = new List<byte[]>();
      contentLength = 0;
      if (this.DebugStyles)
        contentHashList.Add(new byte[5]
        {
          (byte) 100,
          (byte) 101,
          (byte) 98,
          (byte) 117,
          (byte) 103
        });
      if (!string.IsNullOrEmpty(this.CacheKey))
        contentHashList.Add(Encoding.UTF8.GetBytes(this.CacheKey));
      foreach (IBundledCSSContent content in (IEnumerable<IBundledCSSContent>) this.ContentList)
      {
        int bundleLength;
        contentHashList.AddRange((IEnumerable<byte[]>) content.GetFileHash(requestContext, this, bundledFile as BundledCSSFile, urlHelper, contentCache, out bundleLength));
        contentLength += bundleLength;
      }
      return BundlingHelper.GetHashCodeFromHashList((IEnumerable<byte[]>) contentHashList);
    }
  }
}
