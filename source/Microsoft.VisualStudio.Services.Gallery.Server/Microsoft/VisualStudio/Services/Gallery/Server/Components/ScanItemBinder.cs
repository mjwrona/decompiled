// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ScanItemBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ScanItemBinder : ObjectBinder<ScanItem>
  {
    protected SqlColumnBinder scanIdColumn = new SqlColumnBinder("ScanId");
    protected SqlColumnBinder itemIdColumn = new SqlColumnBinder("ItemId");
    protected SqlColumnBinder contentTypeColumn = new SqlColumnBinder("ContentType");
    protected SqlColumnBinder descriptionColumn = new SqlColumnBinder("Description");
    protected SqlColumnBinder jobIdColumn = new SqlColumnBinder("JobId");
    protected SqlColumnBinder validationStatusColumn = new SqlColumnBinder("ValidationStatus");
    protected SqlColumnBinder resultMessageColumn = new SqlColumnBinder("ResultMessage");

    protected override ScanItem Bind() => new ScanItem()
    {
      ScanId = this.scanIdColumn.GetGuid((IDataReader) this.Reader),
      ItemId = this.itemIdColumn.GetGuid((IDataReader) this.Reader),
      ContentType = this.contentTypeColumn.GetInt32((IDataReader) this.Reader),
      Description = this.descriptionColumn.GetString((IDataReader) this.Reader, true),
      JobId = this.jobIdColumn.GetString((IDataReader) this.Reader, true),
      ResultMessage = this.resultMessageColumn.GetString((IDataReader) this.Reader, true),
      ValidationStatus = (ValidationStatus) this.validationStatusColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
