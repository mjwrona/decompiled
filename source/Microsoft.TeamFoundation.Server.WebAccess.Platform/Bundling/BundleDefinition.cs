// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.BundleDefinition
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  public abstract class BundleDefinition
  {
    private static readonly string s_bundleNameFormat = "vss-bundle-{0}{1}";
    private string m_defintionDataForHash;

    public string FriendlyName { get; set; }

    public string StaticContentVersion { get; set; }

    public bool Diagnose { get; set; }

    public string CacheKey { get; private set; }

    public string GetDefinitionHashCode() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, BundleDefinition.s_bundleNameFormat, (object) this.FriendlyName, (object) this.DataUsedForHash.GetHashCode());

    public void SetCacheKey(string cacheKey)
    {
      this.m_defintionDataForHash = (string) null;
      this.CacheKey = cacheKey;
    }

    public string DataUsedForHash
    {
      get
      {
        if (this.m_defintionDataForHash == null)
        {
          StringBuilder contentHashBuilder = new StringBuilder();
          this.AppendContentHash(contentHashBuilder);
          this.m_defintionDataForHash = contentHashBuilder.ToString();
        }
        return this.m_defintionDataForHash;
      }
    }

    public string Name => "vss-bundle-" + this.FriendlyName;

    protected abstract void AppendContentHash(StringBuilder contentHashBuilder);

    public abstract string GetBundleContent(
      IVssRequestContext requestContext,
      BundledFile bundledFile,
      UrlHelper urlHelper,
      Dictionary<string, string[]> contentCache);

    public abstract IEnumerable<IBundleStreamProvider> GetBundleStreamProviders(
      IVssRequestContext requestContext,
      BundledFile bundledFile,
      UrlHelper urlHelper);

    public abstract string GetBundleHashCode(
      IVssRequestContext requestContext,
      BundledFile bundledFile,
      UrlHelper urlHelper,
      Dictionary<string, string[]> contentCache,
      out int bundleLength);
  }
}
