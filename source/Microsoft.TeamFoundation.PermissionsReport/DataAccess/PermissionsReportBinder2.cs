// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.DataAccess.PermissionsReportBinder2
// Assembly: Microsoft.TeamFoundation.PermissionsReport, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5F2A073E-CE9C-4394-ABB1-512D7FB702F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.PermissionsReport.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.PermissionsReport.DataAccess
{
  public class PermissionsReportBinder2 : PermissionsReportBinder
  {
    protected SqlColumnBinder m_RequestorId = new SqlColumnBinder("RequestorId");

    protected override PermissionsReportStoreItem Bind()
    {
      PermissionsReportStoreItem permissionsReportStoreItem = base.Bind();
      permissionsReportStoreItem.RequestorId = this.m_RequestorId.GetGuid((IDataReader) this.Reader, false);
      return permissionsReportStoreItem;
    }
  }
}
