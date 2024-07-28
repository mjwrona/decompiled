// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionVersionBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionVersionBinder : ObjectBinder<ExtensionVersion>
  {
    protected SqlColumnBinder extensionIdColumn = new SqlColumnBinder("ExtensionId");
    protected SqlColumnBinder versionColumn = new SqlColumnBinder("Version");
    protected SqlColumnBinder lastUpdatedColumn = new SqlColumnBinder("LastUpdated");
    protected SqlColumnBinder versionDescription = new SqlColumnBinder("VersionDescription");
    protected SqlColumnBinder flagsColumn = new SqlColumnBinder("Flags");
    protected SqlColumnBinder validationResultMessage = new SqlColumnBinder("ValidationResultMessage");
    protected SqlColumnBinder cdnDirectoryColumn = new SqlColumnBinder("CdnDirectory");
    protected SqlColumnBinder isCdnEnabledColumn = new SqlColumnBinder("IsCdnEnabled");

    protected override ExtensionVersion Bind()
    {
      ExtensionVersion extensionVersion = new ExtensionVersion()
      {
        ExtensionId = this.extensionIdColumn.GetGuid((IDataReader) this.Reader),
        Version = this.versionColumn.GetString((IDataReader) this.Reader, false),
        LastUpdated = this.lastUpdatedColumn.GetDateTime((IDataReader) this.Reader),
        VersionDescription = this.versionDescription.GetString((IDataReader) this.Reader, true),
        Flags = ExtensionVersionFlags.Validated
      };
      extensionVersion.Flags = (ExtensionVersionFlags) this.flagsColumn.GetInt32((IDataReader) this.Reader);
      extensionVersion.ValidationResultMessage = this.validationResultMessage.GetString((IDataReader) this.Reader, true);
      extensionVersion.CdnDirectory = this.cdnDirectoryColumn.GetString((IDataReader) this.Reader, true);
      extensionVersion.IsCdnEnabled = this.isCdnEnabledColumn.GetBoolean((IDataReader) this.Reader);
      return extensionVersion;
    }
  }
}
