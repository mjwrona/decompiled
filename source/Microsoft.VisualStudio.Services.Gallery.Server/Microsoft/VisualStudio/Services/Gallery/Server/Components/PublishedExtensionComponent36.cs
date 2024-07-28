// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublishedExtensionComponent36
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublishedExtensionComponent36 : PublishedExtensionComponent35
  {
    public override void UpdateisRepositorySignedProperty(
      Guid extensionId,
      string version,
      string targetPlatform = null,
      bool isRepositorySigned = false)
    {
      this.PrepareStoredProcedure("Gallery.prc_UpdateIsRepositorySignedProperties");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (targetPlatform), targetPlatform, 30, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindBoolean(nameof (isRepositorySigned), isRepositorySigned);
      this.ExecuteNonQuery();
    }
  }
}
