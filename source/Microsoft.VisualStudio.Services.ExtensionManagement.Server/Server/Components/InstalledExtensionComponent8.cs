// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components.InstalledExtensionComponent8
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components
{
  internal class InstalledExtensionComponent8 : InstalledExtensionComponent7
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
      this.BindString("@version", version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindInt("@flags", (int) flags);
      this.BindNullableDateTime("@lastVersionCheck", lastVersionCheck);
      this.BindString("@publisherName", publisherName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindString("@extensionName", extensionName, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindBoolean("@failIfInstalled", failIfInstalled);
      this.BindGuid("@updatedBy", installedBy);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Extension.prc_UpsertInstalledExtension", this.RequestContext);
      resultCollection.AddBinder<ExtensionState>((ObjectBinder<ExtensionState>) this.GetInstalledExtensionColumns());
      resultCollection.AddBinder<ExtensionUpdateOutput>((ObjectBinder<ExtensionUpdateOutput>) new ExtensionUpdateOutputColumns());
      ExtensionState extensionState = resultCollection.GetCurrent<ExtensionState>().Items[0];
      resultCollection.NextResult();
      ExtensionUpdateOutput extensionUpdateOutput = resultCollection.GetCurrent<ExtensionUpdateOutput>().Items[0];
      previousVersion = extensionUpdateOutput.PreviousVersion;
      previousFlags = extensionUpdateOutput.PreviousFlags;
      return extensionState;
    }
  }
}
