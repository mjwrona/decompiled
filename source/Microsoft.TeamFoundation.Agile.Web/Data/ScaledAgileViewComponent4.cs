// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Data.ScaledAgileViewComponent4
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

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
  internal class ScaledAgileViewComponent4 : ScaledAgileViewComponent3
  {
    internal override ScaledAgileView CreateView(
      Guid projectId,
      ScaledAgileView view,
      Guid createdBy)
    {
      ArgumentUtility.CheckForNull<ScaledAgileView>(view, "ScaledAgileView");
      this.PrepareStoredProcedure("prc_CreateScaledAgileView");
      this.BindDataspace(projectId);
      this.BindGuid("@ownerId", view.OwnerId);
      this.BindString("@name", view.Name, 128, false, SqlDbType.NVarChar);
      this.BindInt("@type", (int) view.Type);
      this.BindScaledAgileViewPropertyTable("@scaledAgileViewProperty", view.Configuration.TeamBacklogMappings);
      this.BindGuid("@createdBy", createdBy);
      using (SqlDataReader sqlDataReader = this.ExecuteReader())
      {
        view.Id = sqlDataReader.Read() ? sqlDataReader.GetGuid(0) : throw new UnexpectedDatabaseResultException(this.ProcedureName);
        view.CreatedDate = sqlDataReader.GetDateTime(1);
        view.CreatedBy = createdBy;
      }
      return view;
    }

    internal override ScaledAgileView GetView(Guid projectId, Guid viewId)
    {
      this.PrepareStoredProcedure("prc_GetScaledAgileView");
      this.BindDataspace(projectId);
      this.BindGuid("@viewId", viewId);
      ScaledAgileView view = (ScaledAgileView) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ShallowScaledAgileViewRecord>((ObjectBinder<ShallowScaledAgileViewRecord>) new ShallowScaledAgileViewRecordBinder2());
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

    internal override void UpdateView(
      Guid projectId,
      Guid viewId,
      Guid modifiedBy,
      ScaledAgileView view)
    {
      ArgumentUtility.CheckForNull<ScaledAgileView>(view, "ScaledAgileView");
      this.PrepareStoredProcedure("prc_UpdateScaledAgileView");
      this.BindDataspace(projectId);
      this.BindGuid("@viewId", viewId);
      this.BindGuid("@modifiedBy", modifiedBy);
      this.BindString("@name", view.Name, 128, false, SqlDbType.NVarChar);
      this.BindScaledAgileViewPropertyTable("@scaledAgileViewProperty", view.Configuration.TeamBacklogMappings);
      this.ExecuteNonQuery();
    }

    internal override void SoftDeleteScaledAgileView(Guid projectId, Guid viewID, Guid deletedBy)
    {
      this.PrepareStoredProcedure("prc_SetScaledAgileViewDeleted");
      this.BindDataspace(projectId);
      this.BindGuid("@viewID", viewID);
      this.BindGuid("@deletedBy", deletedBy);
      this.ExecuteNonQuery();
    }

    protected override ShallowScaledAgileViewRecordBinder GetViewDefinitionsRecordBinder() => (ShallowScaledAgileViewRecordBinder) new ShallowScaledAgileViewRecordBinder2();

    internal override ScaledAgileView CreateView(Guid projectId, ScaledAgileView view) => throw new NotSupportedException();

    internal override void SoftDeleteScaledAgileView(Guid projectId, Guid viewID) => throw new NotSupportedException();
  }
}
