// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.BundleFileContent
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  public class BundleFileContent : 
    IBundledScriptContent,
    IBundledContent<ScriptBundleDefinition, BundledScriptFile>,
    IBundledCSSContent,
    IBundledContent<CSSBundleDefinition, BundledCSSFile>
  {
    public string LocalFilePath { get; set; }

    public string Name { get; set; }

    public string GetDefinitionHash() => this.Name;

    public string GetContent(
      IVssRequestContext requestContext,
      ScriptBundleDefinition bundleDefinition,
      BundledScriptFile bundledFile,
      UrlHelper urlHelper,
      Dictionary<string, string[]> contentCache)
    {
      return this.GetContent(contentCache);
    }

    public string GetContent(
      IVssRequestContext requestContext,
      CSSBundleDefinition bundleDefinition,
      BundledCSSFile bundledFile,
      UrlHelper urlHelper,
      Dictionary<string, string[]> contentCache)
    {
      return this.GetContent(contentCache);
    }

    private string GetContent(Dictionary<string, string[]> contentCache)
    {
      VssScriptFileInfo vssScriptFile = VssScriptFileInfo.GetVssScriptFile(this.LocalFilePath);
      return vssScriptFile.FileInfo.Exists ? BundlingHelper.JoinWithNewLine((IEnumerable<string>) vssScriptFile.ReadLinesAndSetHash(contentCache)) : string.Empty;
    }

    public List<byte[]> GetFileHash(
      IVssRequestContext requestContext,
      ScriptBundleDefinition bundleDefinition,
      BundledScriptFile bundledFile,
      UrlHelper urlHelper,
      Dictionary<string, string[]> contentCache,
      out int contentLength)
    {
      return this.GetFileHash(contentCache, out contentLength);
    }

    public List<byte[]> GetFileHash(
      IVssRequestContext requestContext,
      CSSBundleDefinition bundleDefinition,
      BundledCSSFile bundledFile,
      UrlHelper urlHelper,
      Dictionary<string, string[]> contentCache,
      out int contentLength)
    {
      return this.GetFileHash(contentCache, out contentLength);
    }

    public List<byte[]> GetFileHash(
      Dictionary<string, string[]> contentCache,
      out int contentLength)
    {
      VssScriptFileInfo vssScriptFile = VssScriptFileInfo.GetVssScriptFile(this.LocalFilePath);
      if (vssScriptFile.HashCode == null)
      {
        string[] strArray;
        if (vssScriptFile.FileInfo.Exists)
          strArray = vssScriptFile.ReadLinesAndSetHash(contentCache);
        else
          strArray = new string[1]{ string.Empty };
        contentCache[vssScriptFile.FileInfo.FullName] = strArray;
      }
      contentLength = vssScriptFile.FileInfo.Exists ? (int) vssScriptFile.FileInfo.Length : 0;
      return new List<byte[]>()
      {
        vssScriptFile.HashCode ?? Array.Empty<byte>()
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

    private IEnumerable<IBundleStreamProvider> GetBundleStreamProviders()
    {
      VssScriptFileInfo vssScriptFile = VssScriptFileInfo.GetVssScriptFile(this.LocalFilePath);
      if (vssScriptFile?.FileInfo == null)
        return (IEnumerable<IBundleStreamProvider>) Array.Empty<IBundleStreamProvider>();
      return (IEnumerable<IBundleStreamProvider>) new IBundleStreamProvider[1]
      {
        (IBundleStreamProvider) new BundleFileStreamProvider(vssScriptFile.FileInfo)
      };
    }
  }
}
