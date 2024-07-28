// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublishedExtensionComponent37
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublishedExtensionComponent37 : PublishedExtensionComponent36
  {
    public override void UpdateAssetIdForExtensionVersion(
      PublishedExtension extension,
      string assetType,
      int fileId)
    {
      if (extension == null || extension.Versions == null || extension.Versions.Count != 1)
      {
        int num;
        if (extension == null)
        {
          num = 1;
        }
        else
        {
          Guid extensionId = extension.ExtensionId;
          num = 0;
        }
        throw new ExtensionDoesNotExistException(num != 0 ? "" : extension.ExtensionId.ToString());
      }
      this.PrepareStoredProcedure("Gallery.prc_UpdateAssetIdForExtensionVersion");
      this.BindGuid("extensionId", extension.ExtensionId);
      this.BindString("version", extension.Versions[0].Version, 43, false, SqlDbType.VarChar);
      this.BindString("targetPlatform", extension.Versions[0].TargetPlatform, 30, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindInt("newFileId", fileId);
      this.BindString(nameof (assetType), assetType, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }
  }
}
