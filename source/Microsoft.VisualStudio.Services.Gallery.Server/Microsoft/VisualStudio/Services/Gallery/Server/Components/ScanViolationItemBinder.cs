// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ScanViolationItemBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ScanViolationItemBinder : ObjectBinder<ScanViolationItem>
  {
    protected SqlColumnBinder extensionIdColumn = new SqlColumnBinder("ExtensionId");
    protected SqlColumnBinder latestScanIdColumn = new SqlColumnBinder("LatestScanId");
    protected SqlColumnBinder violationFoundColumn = new SqlColumnBinder("ViolationFound");
    protected SqlColumnBinder resultMessageColumn = new SqlColumnBinder("ResultMessage");
    protected SqlColumnBinder contactDateColumn = new SqlColumnBinder("ContactDate");

    protected override ScanViolationItem Bind() => new ScanViolationItem()
    {
      ExtensionId = this.extensionIdColumn.GetGuid((IDataReader) this.Reader),
      LatestScanId = this.latestScanIdColumn.GetGuid((IDataReader) this.Reader),
      ViolationFound = this.violationFoundColumn.GetBoolean((IDataReader) this.Reader),
      ResultMessage = this.resultMessageColumn.GetString((IDataReader) this.Reader, true),
      ContactDate = this.contactDateColumn.GetNullableDateTime((IDataReader) this.Reader)
    };
  }
}
