// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.IBundledContent`2
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  public interface IBundledContent<TDefinition, TFile>
    where TDefinition : BundleDefinition
    where TFile : BundledFile
  {
    string GetDefinitionHash();

    string GetContent(
      IVssRequestContext requestContext,
      TDefinition bundleDefinition,
      TFile bundledFile,
      UrlHelper urlHelper,
      Dictionary<string, string[]> contentCache);

    IEnumerable<IBundleStreamProvider> GetBundleStreamProviders(
      IVssRequestContext requestContext,
      TDefinition bundleDefinition,
      TFile bundledFile,
      UrlHelper urlHelper);

    List<byte[]> GetFileHash(
      IVssRequestContext requestContext,
      TDefinition bundleDefinition,
      TFile bundledFile,
      UrlHelper urlHelper,
      Dictionary<string, string[]> contentCache,
      out int bundleLength);
  }
}
