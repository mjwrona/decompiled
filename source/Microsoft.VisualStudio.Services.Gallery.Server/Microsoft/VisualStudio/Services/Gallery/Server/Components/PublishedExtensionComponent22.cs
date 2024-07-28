// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublishedExtensionComponent22
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublishedExtensionComponent22 : PublishedExtensionComponent21
  {
    public override void AddAssetsForExtensionVersion(
      PublishedExtension extension,
      Guid validationId,
      IEnumerable<ExtensionFile> assets)
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
      this.PrepareStoredProcedure("Gallery.prc_AddAssetsForExtensionVersion");
      this.BindGuid("extensionId", extension.ExtensionId);
      this.BindString("version", extension.Versions[0].Version, 43, false, SqlDbType.VarChar);
      this.BindNullableGuid(nameof (validationId), validationId);
      this.BindExtensionFileTable2(nameof (assets), assets);
      this.ExecuteNonQuery();
    }
  }
}
