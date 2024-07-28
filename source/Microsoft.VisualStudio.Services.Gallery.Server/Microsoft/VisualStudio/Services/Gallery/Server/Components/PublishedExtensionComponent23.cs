// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublishedExtensionComponent23
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublishedExtensionComponent23 : PublishedExtensionComponent22
  {
    public virtual void ShareExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string collectionName,
      Guid collectionId,
      bool removeAccount)
    {
      this.PrepareStoredProcedure("Gallery.prc_ShareExtension");
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (extensionName), extensionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (collectionName), collectionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindGuid(nameof (collectionId), collectionId);
      this.BindBoolean(nameof (removeAccount), removeAccount);
      this.ExecuteNonQuery();
    }

    protected override void ProcessExtensionShares(
      ResultCollection resultCollection,
      ExtensionQueryFlags flags,
      Dictionary<Guid, PublishedExtension> extensionMap)
    {
      if (!flags.HasFlag((Enum) ExtensionQueryFlags.IncludeSharedAccounts))
        return;
      resultCollection.AddBinder<GalleryTag>((ObjectBinder<GalleryTag>) new GalleryTagBinder());
      resultCollection.NextResult();
      foreach (GalleryTag galleryTag in resultCollection.GetCurrent<GalleryTag>().Items)
      {
        PublishedExtension extension = extensionMap[galleryTag.ReferenceId];
        if (extension.SharedWith == null)
          extension.SharedWith = new List<ExtensionShare>();
        extension.SharedWith.Add(new ExtensionShare()
        {
          Id = galleryTag.TagName,
          Type = "account",
          Name = galleryTag.Comment
        });
      }
    }
  }
}
