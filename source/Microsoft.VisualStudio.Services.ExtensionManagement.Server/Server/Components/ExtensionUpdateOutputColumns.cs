// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components.ExtensionUpdateOutputColumns
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System.Data;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components
{
  internal class ExtensionUpdateOutputColumns : ObjectBinder<ExtensionUpdateOutput>
  {
    private SqlColumnBinder versionColumn = new SqlColumnBinder("PreviousVersion");
    private SqlColumnBinder flagsColumn = new SqlColumnBinder("PreviousFlags");

    protected override ExtensionUpdateOutput Bind() => new ExtensionUpdateOutput()
    {
      PreviousVersion = this.versionColumn.GetString((IDataReader) this.Reader, true),
      PreviousFlags = this.flagsColumn.IsNull((IDataReader) this.Reader) ? new ExtensionStateFlags?() : new ExtensionStateFlags?((ExtensionStateFlags) this.flagsColumn.GetInt32((IDataReader) this.Reader))
    };
  }
}
