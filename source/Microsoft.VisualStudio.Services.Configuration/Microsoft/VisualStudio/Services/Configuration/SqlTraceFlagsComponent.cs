// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SqlTraceFlagsComponent
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class SqlTraceFlagsComponent : TeamFoundationSqlResourceComponent
  {
    public List<SqlTraceFlagStatus> DbccTraceStatus(int tracePoint)
    {
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DBCC TRACESTATUS({0}) WITH NO_INFOMSGS", (object) tracePoint);
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "DBCC TRACESTATUS", (IVssRequestContext) null);
      resultCollection.AddBinder<SqlTraceFlagStatus>((ObjectBinder<SqlTraceFlagStatus>) new SqlTraceFlagStatusColumns());
      return resultCollection.GetCurrent<SqlTraceFlagStatus>().Items;
    }
  }
}
