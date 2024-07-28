// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Transform.TransformComponent4
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.Transform
{
  internal class TransformComponent4 : TransformComponent3
  {
    public override TransformResult TransformNext(
      string table,
      IDictionary<string, string> transformSettings)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_TransformNextBatch");
      this.BindString("@tableName", table, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindKeyValuePairStringTable("@settings", (IEnumerable<KeyValuePair<string, string>>) transformSettings);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TransformResult>((ObjectBinder<TransformResult>) new TransformNextColumns4());
        return resultCollection.GetCurrent<TransformResult>().Items.SingleOrDefault<TransformResult>();
      }
    }
  }
}
