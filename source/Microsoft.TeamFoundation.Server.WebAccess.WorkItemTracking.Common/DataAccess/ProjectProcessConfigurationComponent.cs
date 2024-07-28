// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.ProjectProcessConfigurationComponent
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.Tvps;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess
{
  internal class ProjectProcessConfigurationComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[6]
    {
      (IComponentCreator) new ComponentCreator<ProjectProcessConfigurationComponent>(1, true),
      (IComponentCreator) new ComponentCreator<ProjectProcessConfigurationComponent2>(2),
      (IComponentCreator) new ComponentCreator<ProjectProcessConfigurationComponent3>(3),
      (IComponentCreator) new ComponentCreator<ProjectProcessConfigurationComponent4>(4),
      (IComponentCreator) new ComponentCreator<ProjectProcessConfigurationComponent5>(5),
      (IComponentCreator) new ComponentCreator<ProjectProcessConfigurationComponent6>(6)
    }, "ProjectProcessConfiguration", "WorkItem");
    private static SqlMetaData[] typ_ProjectConfigurationTypeFieldTable = new SqlMetaData[3]
    {
      new SqlMetaData("FieldRefName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("FieldType", SqlDbType.TinyInt),
      new SqlMetaData("Format", SqlDbType.NVarChar, 256L)
    };
    private static SqlMetaData[] typ_ProjectConfigurationTypeFieldValueTable = new SqlMetaData[3]
    {
      new SqlMetaData("TypeFieldType", SqlDbType.TinyInt),
      new SqlMetaData("TypeFieldValueType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("TypeFieldValueValue", SqlDbType.NVarChar, 256L)
    };

    public static ProjectProcessConfigurationComponent CreateComponent(
      IVssRequestContext requestContext)
    {
      return requestContext.CreateComponent<ProjectProcessConfigurationComponent>();
    }

    public ProjectProcessConfigurationComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    internal virtual ProjectProcessConfiguration GetProjectProcessConfiguration(Guid projectId) => throw new NotSupportedException();

    internal virtual void SetProjectProcessConfiguration(
      Guid projectId,
      ProjectProcessConfiguration settings)
    {
      throw new NotSupportedException();
    }

    internal virtual void DeleteProjectProcessConfiguration(Guid projectId) => throw new NotSupportedException();

    internal virtual IDictionary<Guid, string> GetTeamFieldsForProjects(IEnumerable<Guid> projectIds) => throw new NotSupportedException();

    internal virtual IEnumerable<ProjectCategoryStateMap> GetCategoryStates(
      WorkItemTypeEnum categoryType)
    {
      return Enumerable.Empty<ProjectCategoryStateMap>();
    }

    internal virtual IReadOnlyCollection<ProjectGuidWatermarkPair> GetChangedStateProjectsSinceWatermark(
      int watermark)
    {
      throw new NotSupportedException();
    }

    protected virtual SqlParameter BindProjectConfigurationTypeFieldTable(
      string parameterName,
      IEnumerable<TypeField> rows)
    {
      return this.BindTable(parameterName, "typ_ProjectConfigurationTypeFieldTable", rows.Select<TypeField, SqlDataRecord>((System.Func<TypeField, SqlDataRecord>) (row =>
      {
        SqlDataRecord record = new SqlDataRecord(ProjectProcessConfigurationComponent.typ_ProjectConfigurationTypeFieldTable);
        record.SetString(0, row.Name);
        record.SetByte(1, (byte) row.Type);
        record.SetNullableString(2, row.Format);
        return record;
      })));
    }

    protected virtual void BindDataspaceIdOrProjectId(Guid dataspaceIdentifier) => this.BindGuid("@projectId", dataspaceIdentifier);

    protected virtual SqlParameter BindProjectConfigurationTypeFieldValueTable(
      string parameterName,
      IEnumerable<TypeFieldValueRow> rows)
    {
      return this.BindTable(parameterName, "typ_ProjectConfigurationTypeFieldValueTable", rows.Select<TypeFieldValueRow, SqlDataRecord>((System.Func<TypeFieldValueRow, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ProjectProcessConfigurationComponent.typ_ProjectConfigurationTypeFieldValueTable);
        sqlDataRecord.SetByte(0, (byte) row.FieldType);
        sqlDataRecord.SetString(1, row.Type);
        sqlDataRecord.SetString(2, row.Value);
        return sqlDataRecord;
      })));
    }
  }
}
