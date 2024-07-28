// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingOperationColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingOperationColumns : ObjectBinder<ServicingOperation>
  {
    private SqlColumnBinder operationColumn = new SqlColumnBinder("ServicingOperation");
    private SqlColumnBinder handlersColumn = new SqlColumnBinder("Handlers");

    protected override ServicingOperation Bind()
    {
      ServicingOperation servicingOperation = new ServicingOperation();
      servicingOperation.Name = this.operationColumn.GetString((IDataReader) this.Reader, false);
      string str = this.handlersColumn.GetString((IDataReader) this.Reader, true);
      if (!string.IsNullOrEmpty(str))
        servicingOperation.ExecutionHandlers.AddRange(((IEnumerable<string>) str.Split(';')).Select<string, ServicingExecutionHandlerData>((System.Func<string, ServicingExecutionHandlerData>) (type => new ServicingExecutionHandlerData(type))));
      return servicingOperation;
    }
  }
}
