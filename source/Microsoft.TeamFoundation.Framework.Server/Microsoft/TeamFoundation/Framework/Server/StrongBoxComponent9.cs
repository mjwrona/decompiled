// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StrongBoxComponent9
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class StrongBoxComponent9 : StrongBoxComponent8
  {
    public override bool IsDrawerEmpty(Guid drawerId)
    {
      this.PrepareStoredProcedure("prc_IsDrawerEmpty");
      this.BindGuid("@drawerId", drawerId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder isEmptyColumn = new SqlColumnBinder("IsEmpty");
        resultCollection.AddBinder<bool>((ObjectBinder<bool>) new SimpleObjectBinder<bool>((System.Func<IDataReader, bool>) (reader => isEmptyColumn.GetBoolean(reader, true))));
        return resultCollection.GetCurrent<bool>().Items.SingleOrDefault<bool>();
      }
    }
  }
}
