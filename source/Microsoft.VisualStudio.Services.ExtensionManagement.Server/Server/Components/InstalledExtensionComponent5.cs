// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components.InstalledExtensionComponent5
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components
{
  internal class InstalledExtensionComponent5 : InstalledExtensionComponent4
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
      this.BindGuid("@updatedBy", installedBy);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Extension.prc_UpsertInstalledExtension", this.RequestContext);
      resultCollection.AddBinder<ExtensionState>((ObjectBinder<ExtensionState>) this.GetInstalledExtensionColumns());
      previousVersion = (string) null;
      previousFlags = new ExtensionStateFlags?();
      return resultCollection.GetCurrent<ExtensionState>().Items[0];
    }

    public override void UninstallExtension(
      string publisherName = null,
      string extensionName = null,
      Guid uninstalledBy = default (Guid))
    {
      try
      {
        this.TraceEnter(70210, nameof (UninstallExtension));
        this.PrepareStoredProcedure("Extension.prc_UninstallExtension");
        this.BindGuid("@extensionId", Guid.NewGuid());
        this.BindString("@publisherName", publisherName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindString("@extensionName", extensionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindGuid("@uninstalledBy", uninstalledBy);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(70215, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(70220, nameof (UninstallExtension));
      }
    }
  }
}
