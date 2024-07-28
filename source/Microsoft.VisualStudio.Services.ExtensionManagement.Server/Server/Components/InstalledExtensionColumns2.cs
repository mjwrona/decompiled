// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components.InstalledExtensionColumns2
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components
{
  internal class InstalledExtensionColumns2 : InstalledExtensionColumns
  {
    private SqlColumnBinder versionColumn = new SqlColumnBinder("Version");
    private SqlColumnBinder flagsColumn = new SqlColumnBinder("Flags");
    private SqlColumnBinder lastUpdatedColumn = new SqlColumnBinder("LastUpdated");
    private SqlColumnBinder lastVersionCheckColumn = new SqlColumnBinder("LastVersionCheck");
    private SqlColumnBinder publisherNameColumn = new SqlColumnBinder("PublisherName");
    private SqlColumnBinder extensionNameColumn = new SqlColumnBinder("ExtensionName");

    protected override ExtensionState Bind()
    {
      ExtensionState extensionState = new ExtensionState();
      extensionState.Flags = (ExtensionStateFlags) this.flagsColumn.GetInt32((IDataReader) this.Reader);
      extensionState.LastUpdated = this.lastUpdatedColumn.GetDateTime((IDataReader) this.Reader);
      extensionState.LastVersionCheck = new DateTime?(this.lastVersionCheckColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue));
      extensionState.PublisherName = this.publisherNameColumn.GetString((IDataReader) this.Reader, true);
      extensionState.ExtensionName = this.extensionNameColumn.GetString((IDataReader) this.Reader, true);
      extensionState.Version = this.versionColumn.GetString((IDataReader) this.Reader, true);
      return extensionState;
    }
  }
}
