// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Data.ScaledAgileViewComponent5
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Agile.Web.Services;
using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Data
{
  internal class ScaledAgileViewComponent5 : ScaledAgileViewComponent4
  {
    private static SqlMetaData[] typ_CardFieldsTable2 = new SqlMetaData[4]
    {
      new SqlMetaData("Type", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("FieldName", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("Property", SqlDbType.NVarChar, (long) byte.MaxValue),
      new SqlMetaData("Value", SqlDbType.NVarChar, (long) byte.MaxValue)
    };

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
      using (SqlDataReader sqlDataReader = this.ExecuteReader())
      {
        view.Id = sqlDataReader.Read() ? sqlDataReader.GetGuid(0) : throw new UnexpectedDatabaseResultException(this.ProcedureName);
        view.CreatedDate = sqlDataReader.GetDateTime(1);
        view.CreatedBy = createdBy;
        view.Revision = sqlDataReader.GetInt32(2);
      }
      return view;
    }

    internal override ScaledAgileView GetView(Guid projectId, Guid viewId) => this.GetView(projectId, viewId, true);

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
        rc.AddBinder<ShallowScaledAgileViewRecord>((ObjectBinder<ShallowScaledAgileViewRecord>) this.GetViewDefinitionsRecordBinder());
        rc.AddBinder<ScaledAgileViewPropertyRecord2>((ObjectBinder<ScaledAgileViewPropertyRecord2>) new ScaledAgileViewPropertyRecordBinder2());
        ShallowScaledAgileViewRecord viewRecord = rc.GetCurrent<ShallowScaledAgileViewRecord>().Items.FirstOrDefault<ShallowScaledAgileViewRecord>();
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
      this.BindScaledAgileViewPropertyTable("@scaledAgileViewProperty", view.Configuration.TeamBacklogMappings);
      this.BindCardFieldsTable("@cardFields", this.GetCardFieldsRows((Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings) view.Configuration.CardSettings));
      view.Revision = (int) this.ExecuteScalar();
    }

    protected override ShallowScaledAgileViewRecordBinder GetViewDefinitionsRecordBinder() => (ShallowScaledAgileViewRecordBinder) new ShallowScaledAgileViewRecordBinder3();

    protected virtual SqlParameter BindCardFieldsTable(
      string parameterName,
      IEnumerable<BoardCardSettingRow> rows)
    {
      ArgumentUtility.CheckForNull<IEnumerable<BoardCardSettingRow>>(rows, nameof (rows));
      return this.BindTable(parameterName, "typ_CardFieldsTable2", rows.Select<BoardCardSettingRow, SqlDataRecord>((System.Func<BoardCardSettingRow, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ScaledAgileViewComponent5.typ_CardFieldsTable2);
        sqlDataRecord.SetString(0, row.Type);
        sqlDataRecord.SetString(1, row.Field);
        sqlDataRecord.SetString(2, row.Property);
        sqlDataRecord.SetString(3, row.Value);
        return sqlDataRecord;
      })));
    }

    protected IEnumerable<BoardCardSettingRow> GetCardFieldsRows(Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings board)
    {
      IEnumerable<BoardCardSettingRow> first = (IEnumerable<BoardCardSettingRow>) new List<BoardCardSettingRow>();
      if (board.Cards != null)
      {
        foreach (KeyValuePair<string, List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting>> card in board.Cards)
        {
          foreach (Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting fieldSetting in card.Value)
          {
            IEnumerable<BoardCardSettingRow> itemFieldRows = this.GetItemFieldRows(card.Key, fieldSetting);
            first = first.Concat<BoardCardSettingRow>(itemFieldRows);
          }
        }
      }
      return first;
    }

    private IEnumerable<BoardCardSettingRow> GetItemFieldRows(
      string cardType,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting fieldSetting)
    {
      List<BoardCardSettingRow> itemFieldRows = new List<BoardCardSettingRow>();
      if (!fieldSetting.IsNullOrEmpty<KeyValuePair<string, string>>())
      {
        foreach (KeyValuePair<string, string> keyValuePair in (Dictionary<string, string>) fieldSetting)
        {
          BoardCardSettingRow boardCardSettingRow = new BoardCardSettingRow()
          {
            Type = cardType,
            Field = fieldSetting.FieldIdentifier,
            Property = keyValuePair.Key,
            Value = keyValuePair.Value
          };
          itemFieldRows.Add(boardCardSettingRow);
        }
      }
      return (IEnumerable<BoardCardSettingRow>) itemFieldRows;
    }

    protected ScaledAgileCardSettings GetCardSettings(
      Guid viewId,
      PlanType type,
      ResultCollection rc)
    {
      ScaledAgileCardSettings cardSettings = (ScaledAgileCardSettings) null;
      List<BoardCardSettingRow> items = rc.GetCurrent<BoardCardSettingRow>().Items;
      if (items.Count > 0)
        cardSettings = PlanUtils.DTOToPlansCardFields(viewId, type, (IEnumerable<BoardCardSettingRow>) items);
      return cardSettings;
    }
  }
}
