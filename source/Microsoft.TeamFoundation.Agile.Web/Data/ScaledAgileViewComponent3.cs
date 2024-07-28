// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Data.ScaledAgileViewComponent3
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Agile.Web.Services;
using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Data
{
  internal class ScaledAgileViewComponent3 : ScaledAgileViewComponent
  {
    private static readonly SqlMetaData[] typ_ScaledAgileViewPropertyTable3 = new SqlMetaData[3]
    {
      new SqlMetaData("TeamId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("CategoryReferenceName", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("Order", SqlDbType.Int)
    };

    internal override ScaledAgileView GetView(Guid projectId, Guid viewId)
    {
      this.PrepareStoredProcedure("prc_GetScaledAgileView");
      this.BindDataspace(projectId);
      this.BindGuid("@viewId", viewId);
      ScaledAgileView view = (ScaledAgileView) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ShallowScaledAgileViewRecord>((ObjectBinder<ShallowScaledAgileViewRecord>) new ShallowScaledAgileViewRecordBinder());
        resultCollection.AddBinder<ScaledAgileViewPropertyRecord2>((ObjectBinder<ScaledAgileViewPropertyRecord2>) new ScaledAgileViewPropertyRecordBinder2());
        ShallowScaledAgileViewRecord viewRecord = resultCollection.GetCurrent<ShallowScaledAgileViewRecord>().Items.FirstOrDefault<ShallowScaledAgileViewRecord>();
        if (viewRecord != null)
        {
          resultCollection.NextResult();
          List<ScaledAgileViewPropertyRecord2> items = resultCollection.GetCurrent<ScaledAgileViewPropertyRecord2>().Items;
          if (items != null)
            view = PlanUtils.DTOToScaledAgileView(viewRecord, (IEnumerable<ScaledAgileViewPropertyRecord2>) items);
        }
      }
      return view;
    }

    protected override SqlParameter BindScaledAgileViewPropertyTable(
      string parameterName,
      IEnumerable<ScaledAgileViewProperty> rows)
    {
      ArgumentUtility.CheckForNull<IEnumerable<ScaledAgileViewProperty>>(rows, "IEnumerable");
      return this.BindTable(parameterName, "typ_ScaledAgileViewPropertyTable3", rows.Select<ScaledAgileViewProperty, SqlDataRecord>((Func<ScaledAgileViewProperty, int, SqlDataRecord>) ((row, index) =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ScaledAgileViewComponent3.typ_ScaledAgileViewPropertyTable3);
        sqlDataRecord.SetGuid(0, row.TeamId);
        sqlDataRecord.SetString(1, row.CategoryReferenceName);
        sqlDataRecord.SetInt32(2, index);
        return sqlDataRecord;
      })));
    }
  }
}
