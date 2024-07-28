// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsViewsSqlComponent
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class AnalyticsViewsSqlComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<AnalyticsViewsSqlComponent>(1)
    }, "AnalyticsViews");
    private static readonly SqlMetaData[] typ_ViewTable = new SqlMetaData[9]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Description", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Visibility", SqlDbType.Int),
      new SqlMetaData("ViewType", SqlDbType.Int),
      new SqlMetaData("Definition", SqlDbType.NVarChar, SqlMetaData.Max),
      new SqlMetaData("CreatedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("LastModifiedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("IsDeleted", SqlDbType.Bit)
    };
    private static readonly SqlMetaData[] typ_QueryTable = new SqlMetaData[5]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ViewId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("EntitySet", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Template", SqlDbType.NVarChar, SqlMetaData.Max),
      new SqlMetaData("IsDeleted", SqlDbType.Bit)
    };

    public AnalyticsViewsSqlComponent()
    {
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    public virtual IReadOnlyList<AnalyticsView> GetViews(Guid projectId)
    {
      this.PrepareStoredProcedure("AnalyticsViews.prc_GetAnalyticsViews");
      this.BindDataspaceId(projectId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<AnalyticsView>((ObjectBinder<AnalyticsView>) new AnalyticsViewBinder());
        return (IReadOnlyList<AnalyticsView>) resultCollection.GetCurrent<AnalyticsView>().Items;
      }
    }

    public virtual IReadOnlyList<AnalyticsView> CreateViews(
      Guid viewScopeId,
      List<AnalyticsView> views)
    {
      return this.MergeViews(viewScopeId, views, AnalyticsViewsSqlComponent.Operation.Insert);
    }

    public virtual IReadOnlyList<AnalyticsView> UpdateViews(
      Guid viewScopeId,
      List<AnalyticsView> views)
    {
      return this.MergeViews(viewScopeId, views, AnalyticsViewsSqlComponent.Operation.Update);
    }

    public virtual void DeleteViews(Guid viewScopeId, List<AnalyticsView> views) => this.MergeViews(viewScopeId, views, AnalyticsViewsSqlComponent.Operation.Delete);

    public virtual AnalyticsView GetView(Guid viewScopeId, Guid viewId)
    {
      List<AnalyticsView> source = (List<AnalyticsView>) null;
      this.PrepareStoredProcedure("AnalyticsViews.prc_GetAnalyticsViewByViewId");
      this.BindGuid("@viewId", viewId);
      this.BindDataspaceId(viewScopeId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<AnalyticsView>((ObjectBinder<AnalyticsView>) new AnalyticsViewAndQueryBinder());
        source = resultCollection.GetCurrent<AnalyticsView>().Items;
      }
      return source.FirstOrDefault<AnalyticsView>();
    }

    public virtual AnalyticsView CreateView(Guid viewScopeId, AnalyticsView view) => this.MergeViews(viewScopeId, new List<AnalyticsView>()
    {
      view
    }, AnalyticsViewsSqlComponent.Operation.Insert).FirstOrDefault<AnalyticsView>();

    public virtual AnalyticsView UpdateView(Guid viewScopeId, AnalyticsView view) => this.MergeViews(viewScopeId, new List<AnalyticsView>()
    {
      view
    }, AnalyticsViewsSqlComponent.Operation.Update).FirstOrDefault<AnalyticsView>();

    public virtual void DeleteView(Guid viewScopeId, AnalyticsView view) => this.MergeViews(viewScopeId, new List<AnalyticsView>()
    {
      view
    }, AnalyticsViewsSqlComponent.Operation.Delete);

    protected virtual IReadOnlyList<AnalyticsView> MergeViews(
      Guid viewScopeId,
      List<AnalyticsView> views,
      AnalyticsViewsSqlComponent.Operation operation)
    {
      this.PrepareStoredProcedure("AnalyticsViews.prc_SetAnalyticsViews");
      this.BindDataspaceId(viewScopeId);
      this.BindViewTable("@viewTable", views, operation);
      this.BindQueryTable("@queryTable", views, operation);
      this.ExecuteNonQuery();
      return (IReadOnlyList<AnalyticsView>) views;
    }

    protected virtual SqlParameter BindViewTable(
      string parameterName,
      List<AnalyticsView> views,
      AnalyticsViewsSqlComponent.Operation operation)
    {
      views = views ?? new List<AnalyticsView>();
      System.Func<AnalyticsView, SqlDataRecord> selector = (System.Func<AnalyticsView, SqlDataRecord>) (view =>
      {
        ArgumentUtility.CheckForNull<AnalyticsView>(view, nameof (view));
        Guid result1 = Guid.Empty;
        Guid result2 = Guid.Empty;
        if (view.CreatedBy != null)
          Guid.TryParse(view.CreatedBy.Id, out result1);
        if (view.LastModifiedBy != null)
          Guid.TryParse(view.LastModifiedBy.Id, out result2);
        SqlDataRecord record = new SqlDataRecord(AnalyticsViewsSqlComponent.typ_ViewTable);
        int ordinal1 = 0;
        int num1 = ordinal1 + 1;
        record.SetGuid(ordinal1, view.Id);
        int ordinal2 = num1;
        int num2 = ordinal2 + 1;
        record.SetString(ordinal2, view.Name);
        int ordinal3 = num2;
        int num3 = ordinal3 + 1;
        record.SetNullableString(ordinal3, view.Description);
        int ordinal4 = num3;
        int num4 = ordinal4 + 1;
        record.SetInt32(ordinal4, (int) view.Visibility);
        int ordinal5 = num4;
        int num5 = ordinal5 + 1;
        record.SetInt32(ordinal5, (int) view.ViewType);
        int ordinal6 = num5;
        int num6 = ordinal6 + 1;
        record.SetNullableString(ordinal6, view.Definition);
        int ordinal7 = num6;
        int num7 = ordinal7 + 1;
        record.SetNullableGuid(ordinal7, result1);
        int ordinal8 = num7;
        int num8 = ordinal8 + 1;
        record.SetNullableGuid(ordinal8, result2);
        int ordinal9 = num8;
        int num9 = ordinal9 + 1;
        record.SetBoolean(ordinal9, operation == AnalyticsViewsSqlComponent.Operation.Delete);
        return record;
      });
      return this.BindTable(parameterName, "AnalyticsViews.typ_AnalyticsView", views.Select<AnalyticsView, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindQueryTable(
      string parameterName,
      List<AnalyticsView> views,
      AnalyticsViewsSqlComponent.Operation operation)
    {
      views = views ?? new List<AnalyticsView>();
      System.Func<AnalyticsView, SqlDataRecord> selector = (System.Func<AnalyticsView, SqlDataRecord>) (view =>
      {
        ArgumentUtility.CheckForNull<AnalyticsView>(view, nameof (view));
        SqlDataRecord sqlDataRecord = new SqlDataRecord(AnalyticsViewsSqlComponent.typ_QueryTable);
        int ordinal1 = 0;
        int num1 = ordinal1 + 1;
        sqlDataRecord.SetGuid(ordinal1, view.Query.Id);
        int ordinal2 = num1;
        int num2 = ordinal2 + 1;
        sqlDataRecord.SetGuid(ordinal2, view.Id);
        int ordinal3 = num2;
        int num3 = ordinal3 + 1;
        sqlDataRecord.SetString(ordinal3, view.Query.EntitySet);
        int ordinal4 = num3;
        int num4 = ordinal4 + 1;
        sqlDataRecord.SetString(ordinal4, view.Query.ODataTemplate);
        int ordinal5 = num4;
        int num5 = ordinal5 + 1;
        sqlDataRecord.SetBoolean(ordinal5, operation == AnalyticsViewsSqlComponent.Operation.Delete);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "AnalyticsViews.typ_AnalyticsViewQuery", views.Select<AnalyticsView, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindDataspaceId(Guid dataspaceId) => this.BindInt("@dataspaceId", this.GetDataspaceId(dataspaceId, true));

    protected enum Operation
    {
      Insert,
      Update,
      Delete,
    }
  }
}
