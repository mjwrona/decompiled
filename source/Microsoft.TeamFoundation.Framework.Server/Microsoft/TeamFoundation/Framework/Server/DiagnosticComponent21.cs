// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DiagnosticComponent21
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DiagnosticComponent21 : DiagnosticComponent20
  {
    public override List<VirtualFileStats> QueryVirtualFileStats()
    {
      string str = "DIAGNOSTIC.prc_QueryVirtualFileStats";
      this.PrepareStoredProcedure(str);
      using (IDataReader reader = (IDataReader) this.ExecuteReader())
      {
        using (ResultCollection resultCollection = new ResultCollection(reader, str, this.RequestContext))
        {
          resultCollection.AddBinder<VirtualFileStats>((ObjectBinder<VirtualFileStats>) new VirtualFileStatsBinder());
          return resultCollection.GetCurrent<VirtualFileStats>().Items;
        }
      }
    }
  }
}
