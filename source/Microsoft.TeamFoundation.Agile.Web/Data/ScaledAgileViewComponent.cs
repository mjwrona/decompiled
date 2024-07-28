// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Data.ScaledAgileViewComponent
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Agile.Web.Services;
using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Work.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Data
{
  internal class ScaledAgileViewComponent : TeamFoundationSqlResourceComponent
  {
    public const int MaxViewNameLength = 128;
    public const int MaxCardSettingsScopeLength = 255;
    public const int MaxDescriptionLength = 256;
    public const int MaxCriteriaLength = 2000;
    public const int MaxMarkersLength = 7000;
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[10]
    {
      (IComponentCreator) new ComponentCreator<ScaledAgileViewComponent>(1),
      (IComponentCreator) new ComponentCreator<ScaledAgileViewComponent2>(2),
      (IComponentCreator) new ComponentCreator<ScaledAgileViewComponent3>(3),
      (IComponentCreator) new ComponentCreator<ScaledAgileViewComponent4>(4),
      (IComponentCreator) new ComponentCreator<ScaledAgileViewComponent5>(5),
      (IComponentCreator) new ComponentCreator<ScaledAgileViewComponent6>(6),
      (IComponentCreator) new ComponentCreator<ScaledAgileViewComponent7>(7),
      (IComponentCreator) new ComponentCreator<ScaledAgileViewComponent8>(8),
      (IComponentCreator) new ComponentCreator<ScaledAgileViewComponent9>(9),
      (IComponentCreator) new ComponentCreator<ScaledAgileViewComponent10>(10)
    }, "ScaledAgileView", "WorkItem");
    private static readonly SqlMetaData[] typ_ScaledAgileViewPropertyTable = new SqlMetaData[2]
    {
      new SqlMetaData("TeamId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("WorkItemTypeName", SqlDbType.NVarChar, (long) byte.MaxValue)
    };
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        1300004,
        new SqlExceptionFactory(typeof (ViewNotFoundException))
      },
      {
        1300009,
        new SqlExceptionFactory(typeof (ViewRevisionMismatchException))
      }
    };

    internal static ScaledAgileViewComponent CreateComponent(IVssRequestContext requestContext) => requestContext.CreateComponent<ScaledAgileViewComponent>();

    public ScaledAgileViewComponent()
    {
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    internal virtual ScaledAgileView GetView(Guid projectId, Guid viewId)
    {
      this.PrepareStoredProcedure("prc_GetScaledAgileView");
      this.BindDataspace(projectId);
      this.BindGuid("@viewId", viewId);
      ScaledAgileView view = (ScaledAgileView) null;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ShallowScaledAgileViewRecord>((ObjectBinder<ShallowScaledAgileViewRecord>) new ShallowScaledAgileViewRecordBinder());
        resultCollection.AddBinder<ScaledAgileViewPropertyRecord>((ObjectBinder<ScaledAgileViewPropertyRecord>) new ScaledAgileViewPropertyRecordBinder());
        ShallowScaledAgileViewRecord viewRecord = resultCollection.GetCurrent<ShallowScaledAgileViewRecord>().Items.FirstOrDefault<ShallowScaledAgileViewRecord>();
        if (viewRecord != null)
        {
          resultCollection.NextResult();
          List<ScaledAgileViewPropertyRecord> items = resultCollection.GetCurrent<ScaledAgileViewPropertyRecord>().Items;
          if (items != null)
            view = PlanUtils.DTOToScaledAgileView(viewRecord, (IEnumerable<ScaledAgileViewPropertyRecord>) items);
        }
      }
      return view;
    }

    internal virtual IList<Guid> DeleteScaledAgileView(Guid projectId, int daysOld) => throw new NotSupportedException();

    internal virtual void SoftDeleteScaledAgileView(Guid projectId, Guid viewID)
    {
      this.PrepareStoredProcedure("prc_SetScaledAgileViewDeleted");
      this.BindDataspace(projectId);
      this.BindGuid("@viewID", viewID);
      this.ExecuteNonQuery();
    }

    internal virtual void SoftDeleteScaledAgileView(Guid projectId, Guid viewID, Guid deletedBy) => throw new NotSupportedException();

    internal virtual IEnumerable<ShallowScaledAgileViewRecord> GetViewDefinitions(
      Guid projectId,
      Guid ownerId)
    {
      this.PrepareStoredProcedure("prc_GetScaledAgileViews");
      this.BindDataspace(projectId);
      this.BindGuid("@ownerId", ownerId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ShallowScaledAgileViewRecord>((ObjectBinder<ShallowScaledAgileViewRecord>) this.GetViewDefinitionsRecordBinder());
        return (IEnumerable<ShallowScaledAgileViewRecord>) resultCollection.GetCurrent<ShallowScaledAgileViewRecord>().Items;
      }
    }

    internal virtual IReadOnlyList<ShallowScaledAgileViewRecord> GetViewDefinitions(
      IReadOnlyList<Guid> planIds)
    {
      throw new NotSupportedException();
    }

    internal virtual int GetViewDefinitionCount(Guid projectId, Guid ownerId = default (Guid)) => throw new NotSupportedException();

    internal virtual ScaledAgileView CreateView(Guid projectId, ScaledAgileView view)
    {
      ArgumentUtility.CheckForNull<ScaledAgileView>(view, "ScaledAgileView");
      this.PrepareStoredProcedure("prc_CreateScaledAgileView");
      this.BindDataspace(projectId);
      this.BindGuid("@ownerId", view.OwnerId);
      this.BindString("@name", view.Name, 128, false, SqlDbType.NVarChar);
      this.BindInt("@type", (int) view.Type);
      this.BindScaledAgileViewPropertyTable("@scaledAgileViewProperty", view.Configuration.TeamBacklogMappings);
      using (SqlDataReader sqlDataReader = this.ExecuteReader())
      {
        view.Id = sqlDataReader.Read() ? sqlDataReader.GetGuid(0) : throw new UnexpectedDatabaseResultException(this.ProcedureName);
        view.CreatedDate = sqlDataReader.GetDateTime(1);
      }
      return view;
    }

    internal virtual ScaledAgileView CreateView(
      Guid projectId,
      ScaledAgileView view,
      Guid createdBy)
    {
      throw new NotSupportedException();
    }

    internal virtual void UpdateView(
      Guid projectId,
      Guid viewId,
      Guid modifiedBy,
      ScaledAgileView view)
    {
      throw new NotSupportedException();
    }

    internal virtual void UpdateLastAccessed(Guid projectId, Guid viewId)
    {
    }

    internal virtual ScaledAgileView GetView(Guid projectId, Guid viewId, bool includeCardSettings) => throw new NotSupportedException();

    internal virtual int GetVersion() => this.Version;

    protected virtual ShallowScaledAgileViewRecordBinder GetViewDefinitionsRecordBinder() => new ShallowScaledAgileViewRecordBinder();

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) ScaledAgileViewComponent.s_sqlExceptionFactories;

    protected void BindDataspace(Guid dataspaceIdentifier) => this.BindInt("@dataspaceId", this.GetDataspaceId(dataspaceIdentifier));

    protected virtual SqlParameter BindScaledAgileViewPropertyTable(
      string parameterName,
      IEnumerable<ScaledAgileViewProperty> rows)
    {
      ArgumentUtility.CheckForNull<IEnumerable<ScaledAgileViewProperty>>(rows, "IEnumerable");
      return this.BindTable(parameterName, "typ_ScaledAgileViewPropertyTable", rows.Select<ScaledAgileViewProperty, SqlDataRecord>((System.Func<ScaledAgileViewProperty, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ScaledAgileViewComponent.typ_ScaledAgileViewPropertyTable);
        sqlDataRecord.SetGuid(0, row.TeamId);
        sqlDataRecord.SetString(1, row.CategoryReferenceName);
        return sqlDataRecord;
      })));
    }
  }
}
