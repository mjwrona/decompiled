// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingOperationDataBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingOperationDataBinder : ObjectBinder<ServicingOperationData>
  {
    private SqlColumnBinder m_servicingTargetColumn = new SqlColumnBinder("ServicingTarget");
    private SqlColumnBinder m_servicingOperationColumn = new SqlColumnBinder("ServicingOperation");
    private SqlColumnBinder m_handlersColumn = new SqlColumnBinder("Handlers");

    protected override ServicingOperationData Bind() => new ServicingOperationData()
    {
      ServicingOperation = this.m_servicingOperationColumn.GetString((IDataReader) this.Reader, false),
      Handlers = this.m_handlersColumn.GetString((IDataReader) this.Reader, true) ?? "",
      ServicingTarget = (ServicingOperationTarget) this.m_servicingTargetColumn.GetByte((IDataReader) this.Reader)
    };
  }
}
