// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components.ExtensionDataColumns
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server.Components
{
  internal class ExtensionDataColumns : ObjectBinder<ExtensionDataDocument>
  {
    private SqlColumnBinder documentIdColumn = new SqlColumnBinder("DocumentId");
    private SqlColumnBinder documentValueColumn = new SqlColumnBinder("DocumentValue");
    private SqlColumnBinder sizeColumn = new SqlColumnBinder("Size");
    private SqlColumnBinder versionColumn = new SqlColumnBinder("Version");
    private SqlColumnBinder locationColumn = new SqlColumnBinder("Location");
    private IVssRequestContext m_requestContext;
    private const string s_area = "ExtensionDataComponent";
    private const string s_layer = "ObjectBinder";

    public ExtensionDataColumns(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(70325, "ExtensionDataComponent", "ObjectBinder", "ExtensionDataColumns.ctor");
      this.m_requestContext = requestContext;
      this.m_requestContext.TraceLeave(70330, "ExtensionDataComponent", "ObjectBinder", "ExtensionDataColumns.ctor");
    }

    protected override ExtensionDataDocument Bind()
    {
      this.m_requestContext.TraceEnter(70335, "ExtensionDataComponent", "ObjectBinder", nameof (Bind));
      ExtensionDataDocument extensionDataDocument = new ExtensionDataDocument();
      extensionDataDocument.DocumentId = this.documentIdColumn.GetBytes((IDataReader) this.Reader, false);
      extensionDataDocument.DocumentValue = this.documentValueColumn.GetString((IDataReader) this.Reader, false);
      extensionDataDocument.Size = this.sizeColumn.GetInt64((IDataReader) this.Reader);
      extensionDataDocument.Version = this.versionColumn.GetInt32((IDataReader) this.Reader);
      extensionDataDocument.Location = this.locationColumn.GetByte((IDataReader) this.Reader);
      this.m_requestContext.TraceLeave(70340, "ExtensionDataComponent", "ObjectBinder", nameof (Bind));
      return extensionDataDocument;
    }
  }
}
