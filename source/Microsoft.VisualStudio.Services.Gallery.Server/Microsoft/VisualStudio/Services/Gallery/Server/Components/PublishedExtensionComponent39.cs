// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublishedExtensionComponent39
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublishedExtensionComponent39 : PublishedExtensionComponent38
  {
    public override bool DoesExtensionDisplayNameExists(
      string extensionDisplayName,
      string installationTarget)
    {
      this.PrepareStoredProcedure("Gallery.prc_DoesExtensionDisplayNameExistForTarget");
      this.BindString(nameof (extensionDisplayName), extensionDisplayName, 200, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindString("target", installationTarget, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      return (bool) this.ExecuteScalar();
    }
  }
}
