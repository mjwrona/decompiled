// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components.InstalledExtensionComponent3
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components
{
  internal class InstalledExtensionComponent3 : InstalledExtensionComponent2
  {
    public override ExtensionState InstallExtension(
      string version,
      ExtensionStateFlags flags,
      DateTime? lastVersionCheck,
      string publisherName,
      string extensionName,
      bool failIfInstalled,
      Guid installedBy,
      out string previousVersion,
      out ExtensionStateFlags? previousFlags)
    {
      this.PrepareStoredProcedure("Extension.prc_UpsertInstalledExtension");
      this.BindGuid("@extensionId", Guid.NewGuid());
      this.BindString("@version", version, 25, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindInt("@flags", (int) flags);
      this.BindNullableDateTime("@lastVersionCheck", lastVersionCheck);
      this.BindString("@publisherName", publisherName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("@extensionName", extensionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindBoolean("@failIfInstalled", failIfInstalled);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Extension.prc_UpsertInstalledExtension", this.RequestContext);
      resultCollection.AddBinder<ExtensionState>((ObjectBinder<ExtensionState>) this.GetInstalledExtensionColumns());
      previousVersion = (string) null;
      previousFlags = new ExtensionStateFlags?();
      return resultCollection.GetCurrent<ExtensionState>().Items[0];
    }
  }
}
