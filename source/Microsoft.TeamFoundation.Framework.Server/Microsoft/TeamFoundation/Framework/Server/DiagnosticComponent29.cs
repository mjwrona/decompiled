// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DiagnosticComponent29
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DiagnosticComponent29 : DiagnosticComponent28
  {
    public override async Task<IReadOnlyList<XEventSession>> QueryXEventSessions()
    {
      DiagnosticComponent29 diagnosticComponent29 = this;
      diagnosticComponent29.PrepareStoredProcedure("DIAGNOSTIC.prc_QueryXEventSessions");
      IReadOnlyList<XEventSession> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await diagnosticComponent29.ExecuteReaderAsync(), diagnosticComponent29.ProcedureName, diagnosticComponent29.RequestContext))
      {
        resultCollection.AddBinder<XEventSession>((ObjectBinder<XEventSession>) new XEventSessionBinder());
        items = (IReadOnlyList<XEventSession>) resultCollection.GetCurrent<XEventSession>().Items;
      }
      return items;
    }
  }
}
