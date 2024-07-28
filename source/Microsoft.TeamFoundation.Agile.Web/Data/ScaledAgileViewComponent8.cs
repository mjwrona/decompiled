// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Data.ScaledAgileViewComponent8
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Agile.Web.Services;
using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Data
{
  internal class ScaledAgileViewComponent8 : ScaledAgileViewComponent7
  {
    internal override int GetViewDefinitionCount(Guid projectId, Guid ownerId = default (Guid))
    {
      this.PrepareStoredProcedure("prc_GetScaledAgileViewCount");
      this.BindDataspace(projectId);
      if (ownerId != new Guid())
        this.BindGuid("@ownerId", ownerId);
      return (int) this.ExecuteScalar();
    }

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
      this.BindString("@description", view.Description, 256, true, SqlDbType.NVarChar);
      this.BindString("@criteria", view.Configuration.Criteria, 2000, true, SqlDbType.NVarChar);
      using (SqlDataReader sqlDataReader = this.ExecuteReader())
      {
        view.Id = sqlDataReader.Read() ? sqlDataReader.GetGuid(0) : throw new UnexpectedDatabaseResultException(this.ProcedureName);
        view.CreatedDate = sqlDataReader.GetDateTime(1);
        view.CreatedBy = createdBy;
        view.Revision = sqlDataReader.GetInt32(2);
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
      ArgumentUtility.CheckForNull<ScaledAgileCardSettings>(view.Configuration.CardSettings, "CardSettings");
      this.PrepareStoredProcedure("prc_UpdateScaledAgileView");
      this.BindDataspace(projectId);
      this.BindGuid("@viewId", viewId);
      this.BindGuid("@modifiedBy", modifiedBy);
      this.BindString("@name", view.Name, 128, false, SqlDbType.NVarChar);
      this.BindInt("@revision", view.Revision);
      this.BindString("@cardSettingsScope", view.Configuration.CardSettings.Scope.ToString(), (int) byte.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@description", view.Description, 256, true, SqlDbType.NVarChar);
      this.BindString("@criteria", view.Configuration.Criteria, 2000, true, SqlDbType.NVarChar);
      this.BindScaledAgileViewPropertyTable("@scaledAgileViewProperty", view.Configuration.TeamBacklogMappings);
      this.BindCardFieldsTable("@cardFields", this.GetCardFieldsRows((BoardCardSettings) view.Configuration.CardSettings));
      view.Revision = (int) this.ExecuteScalar();
    }

    internal override ScaledAgileView GetView(
      Guid projectId,
      Guid viewId,
      bool includeCardSettings)
    {
      this.PrepareStoredProcedure("prc_GetScaledAgileView");
      this.BindDataspace(projectId);
      this.BindGuid("@viewId", viewId);
      this.BindString("@cardSettingsScope", "DELIVERYTIMELINE", (int) byte.MaxValue, false, SqlDbType.NVarChar);
      this.BindBoolean("@includeSettings", includeCardSettings);
      ScaledAgileView view = (ScaledAgileView) null;
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<FullScaledAgileViewRecord>((ObjectBinder<FullScaledAgileViewRecord>) new FullScaledAgileViewRecordBinder());
        rc.AddBinder<ScaledAgileViewPropertyRecord2>((ObjectBinder<ScaledAgileViewPropertyRecord2>) new ScaledAgileViewPropertyRecordBinder2());
        FullScaledAgileViewRecord viewRecord = rc.GetCurrent<FullScaledAgileViewRecord>().Items.FirstOrDefault<FullScaledAgileViewRecord>();
        if (viewRecord != null)
        {
          rc.NextResult();
          List<ScaledAgileViewPropertyRecord2> items = rc.GetCurrent<ScaledAgileViewPropertyRecord2>().Items;
          if (items != null)
          {
            view = PlanUtils.DTOToScaledAgileView(viewRecord, (IEnumerable<ScaledAgileViewPropertyRecord2>) items);
            if (includeCardSettings)
            {
              if (rc.TryNextResult())
              {
                rc.AddBinder<BoardCardSettingRow>((ObjectBinder<BoardCardSettingRow>) new BoardCardSettingRowBinder());
                view.Configuration.CardSettings = this.GetCardSettings(view.Id, view.Type, rc);
              }
            }
          }
        }
      }
      return view;
    }

    internal override IList<Guid> DeleteScaledAgileView(Guid projectId, int daysOld)
    {
      this.PrepareStoredProcedure("prc_DeleteScaledAgileView");
      this.BindDataspace(projectId);
      this.BindInt("@daysOld", daysOld);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new DeletedViewIdsBinder());
        return (IList<Guid>) resultCollection.GetCurrent<Guid>().Items;
      }
    }
  }
}
