// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.ScriptBundleDefinition
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  public class ScriptBundleDefinition : BundleDefinition
  {
    public IList<IBundledScriptContent> ContentList { get; set; }

    public CultureInfo Culture { get; set; }

    public bool DebugScripts { get; set; }

    protected override void AppendContentHash(StringBuilder contentHashBuilder)
    {
      contentHashBuilder.Append(this.DebugScripts ? "debug:" : "min:");
      if (this.Diagnose)
        contentHashBuilder.Append("diagnose:");
      if (this.Culture != null)
      {
        contentHashBuilder.Append(this.Culture.LCID);
        contentHashBuilder.Append(':');
      }
      if (!string.IsNullOrEmpty(this.StaticContentVersion))
      {
        contentHashBuilder.Append(this.StaticContentVersion);
        contentHashBuilder.Append(':');
      }
      if (!string.IsNullOrEmpty(this.CacheKey))
      {
        contentHashBuilder.Append(this.CacheKey);
        contentHashBuilder.Append(':');
      }
      foreach (IBundledScriptContent content in (IEnumerable<IBundledScriptContent>) this.ContentList)
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
      return BundlingHelper.JoinWithNewLine((IEnumerable<string>) this.ContentList.Select<IBundledScriptContent, string>((Func<IBundledScriptContent, string>) (c => c.GetContent(requestContext, this, bundledFile as BundledScriptFile, urlHelper, contentCache))).ToArray<string>());
    }

    public override IEnumerable<IBundleStreamProvider> GetBundleStreamProviders(
      IVssRequestContext requestContext,
      BundledFile bundledFile,
      UrlHelper urlHelper)
    {
      List<IBundleStreamProvider> bundleStreamProviders = new List<IBundleStreamProvider>();
      foreach (IBundledScriptContent content in (IEnumerable<IBundledScriptContent>) this.ContentList)
        bundleStreamProviders.AddRange(content.GetBundleStreamProviders(requestContext, this, bundledFile as BundledScriptFile, urlHelper));
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
      if (this.DebugScripts)
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
      foreach (IBundledScriptContent content in (IEnumerable<IBundledScriptContent>) this.ContentList)
      {
        int bundleLength;
        contentHashList.AddRange((IEnumerable<byte[]>) content.GetFileHash(requestContext, this, bundledFile as BundledScriptFile, urlHelper, contentCache, out bundleLength));
        contentLength += bundleLength;
      }
      return BundlingHelper.GetHashCodeFromHashList((IEnumerable<byte[]>) contentHashList);
    }
  }
}
