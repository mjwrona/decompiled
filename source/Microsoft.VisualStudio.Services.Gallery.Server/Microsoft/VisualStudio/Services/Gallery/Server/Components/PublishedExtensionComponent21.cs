// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublishedExtensionComponent21
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublishedExtensionComponent21 : PublishedExtensionComponent
  {
    public override void UpdateExtensionInstallationTargets(
      string publisherName,
      string extensionName,
      bool isExtensionPublic,
      IEnumerable<InstallationTarget> installationTargets)
    {
      this.PrepareStoredProcedure("Gallery.prc_UpdateExtensionInstallationTarget");
      this.BindExtensionInstallationTargetTable("extensionInstallationTargets", installationTargets);
      this.BindString(nameof (publisherName), publisherName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString(nameof (extensionName), extensionName, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindBoolean("isPublicExtension", isExtensionPublic);
      this.ExecuteNonQuery();
    }
  }
}
