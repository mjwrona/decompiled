// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ContentValidationBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ContentValidationBinder : ObjectBinder<FileServiceContentValidationMetadata>
  {
    private SqlColumnBinder m_fileIdColumn = new SqlColumnBinder("FileId");
    private SqlColumnBinder m_dataspaceIdColumn = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder m_uploaderColumn = new SqlColumnBinder("Uploader");
    private SqlColumnBinder m_ipAddressColumn = new SqlColumnBinder("IPAddress");
    private SqlColumnBinder m_filenameColumn = new SqlColumnBinder("FileName");
    private SqlColumnBinder m_scanTypeColumn = new SqlColumnBinder("ScanType");
    private IVssRequestContext m_requestContext;

    public ContentValidationBinder(IVssRequestContext requestContext) => this.m_requestContext = requestContext;

    protected override FileServiceContentValidationMetadata Bind() => new FileServiceContentValidationMetadata()
    {
      FileId = this.m_fileIdColumn.GetInt32((IDataReader) this.Reader),
      DataspaceId = this.m_dataspaceIdColumn.GetInt32((IDataReader) this.Reader),
      FileName = this.m_filenameColumn.GetString((IDataReader) this.Reader, true),
      Uploader = this.m_uploaderColumn.GetGuid((IDataReader) this.Reader, true),
      IPAddress = this.m_ipAddressColumn.GetString((IDataReader) this.Reader, true),
      ScanType = this.m_scanTypeColumn.IsNull((IDataReader) this.Reader) ? new ContentValidationScanType?() : new ContentValidationScanType?((ContentValidationScanType) this.m_scanTypeColumn.GetByte((IDataReader) this.Reader, (byte) 0, (byte) 0))
    };
  }
}
