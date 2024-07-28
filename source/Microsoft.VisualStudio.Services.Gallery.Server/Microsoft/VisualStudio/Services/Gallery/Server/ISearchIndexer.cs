// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ISearchIndexer
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public interface ISearchIndexer
  {
    void PopulateIndex(
      IVssRequestContext requestContext,
      List<PublishedExtension> extensions,
      string productType = null);

    void CreateIndex(IVssRequestContext requestContext, string productType = null);

    void DeleteEntries(IVssRequestContext requestContext, List<PublishedExtension> extensions);

    void DeleteIndex(IVssRequestContext requestContext, string productType = null);

    void CreateOrUpdateIndex(IVssRequestContext requestContext, string productType = null);

    void UploadSynonymMap(
      IVssRequestContext requestContext,
      string synonymMapName,
      string productType = null);

    void RemoveSynonymMap(
      IVssRequestContext requestContext,
      string synonymMapName,
      string productType = null);
  }
}
