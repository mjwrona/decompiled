// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.DowngradesBinder
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class DowngradesBinder : ObjectBinder<DowngradedResource>
  {
    private SqlColumnBinder meterIdColumn = new SqlColumnBinder("MeterId");
    private SqlColumnBinder resourceSeqColumn = new SqlColumnBinder("ResourceSeq");
    private SqlColumnBinder newQuantityColumn = new SqlColumnBinder("NewQuantity");

    protected override DowngradedResource Bind() => new DowngradedResource()
    {
      MeterId = this.meterIdColumn.GetInt32((IDataReader) this.Reader),
      RenewalGroup = (ResourceRenewalGroup) this.resourceSeqColumn.GetByte((IDataReader) this.Reader, (byte) 0, (byte) 0),
      NewQuantity = this.newQuantityColumn.GetInt32((IDataReader) this.Reader)
    };
  }
}
