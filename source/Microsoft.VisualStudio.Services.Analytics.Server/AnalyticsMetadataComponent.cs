// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsMetadataComponent
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Model;
using Microsoft.VisualStudio.Services.Analytics.OData;
using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class AnalyticsMetadataComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[61]
    {
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent>(1, true),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent2>(2),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent2>(3),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent2>(4),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent2>(5),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent2>(6),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent2>(7),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent2>(8),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent2>(9),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent2>(10),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent2>(11),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent2>(12),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent2>(13),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent2>(14),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent2>(15),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent2>(16),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent2>(17),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent2>(18),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent3>(19),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent3>(20),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent3>(21),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent3>(22),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent4>(23),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent4>(24),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent5>(25),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent6>(26),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent6>(27),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent7>(28),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent8>(29),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent8>(30),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent9>(31),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent9>(32),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(33),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(34),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(35),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(36),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(37),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(38),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(39),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(40),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(41),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(42),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(43),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(44),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(45),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(46),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(47),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(48),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(49),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(50),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(51),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(52),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(53),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(54),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(55),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(56),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(57),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(58),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(59),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(60),
      (IComponentCreator) new ComponentCreator<AnalyticsMetadataComponent10>(61)
    }, "Analytics");

    public AnalyticsMetadataComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    public virtual int GetVersion() => this.Version;

    public virtual ModelCreationProcessFields GetModelCreationProcessFields(
      Guid? processId,
      bool includeDeleted = false)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_GetProcessFields");
      this.BindNullableGuid("@processId", processId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProcessFieldInfo>((ObjectBinder<ProcessFieldInfo>) new AnalyticsMetadataComponent.ProcessFieldInfoColumns());
        return new ModelCreationProcessFields(resultCollection.GetCurrent<ProcessFieldInfo>().Items.Where<ProcessFieldInfo>((System.Func<ProcessFieldInfo, bool>) (pf => pf.FieldType != "Identity")).ToList<ProcessFieldInfo>());
      }
    }

    public virtual Guid GetProjectProcessId(Guid projectId)
    {
      this.PrepareSqlBatch("SELECT ProcessId FROM AnalyticsModel.tbl_Project WHERE PartitionId = @partitionId AND ProjectId = @projectId OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))".Length);
      this.AddStatement("SELECT ProcessId FROM AnalyticsModel.tbl_Project WHERE PartitionId = @partitionId AND ProjectId = @projectId OPTION (OPTIMIZE FOR (@partitionId UNKNOWN))");
      this.BindGuid("@projectId", projectId);
      Guid? nullable = (Guid?) this.ExecuteScalar();
      if (!nullable.HasValue)
        nullable = new Guid?(Guid.Empty);
      return nullable.Value;
    }

    public virtual List<WorkItemTypeField> GetWorkItemTypeFields(
      IList<Guid> projectIds,
      IList<string> workItemTypes)
    {
      return new List<WorkItemTypeField>();
    }

    public virtual CollectionDateTime GetCollectionTimeZoneWithZoneId() => new CollectionDateTime()
    {
      CollectionTimeZone = TimeZoneInfo.Local
    };

    public virtual List<Area> GetAreas(ICollection<Guid> areaIds) => new List<Area>();

    public virtual List<Process> GetProcesses(IList<Guid> projectIds) => new List<Process>();

    public virtual List<Iteration> GetIterations(ICollection<Guid> iterationIds) => new List<Iteration>();

    public virtual List<Tag> GetTags(ICollection<Guid> tagIds) => new List<Tag>();

    public virtual List<Guid> GetModelProjectGuids() => throw new ServiceLevelException(AnalyticsResources.SERVICE_LEVEL_EXCEPTION());

    internal class ProcessFieldInfoColumns : ObjectBinder<ProcessFieldInfo>
    {
      private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
      private SqlColumnBinder ReferenceNameColumn = new SqlColumnBinder("ReferenceName");
      private SqlColumnBinder SourceFieldNameColumn = new SqlColumnBinder("SourceFieldName");
      private SqlColumnBinder DescriptionColumn = new SqlColumnBinder("Description");
      private SqlColumnBinder FieldTypeColumn = new SqlColumnBinder("FieldType");
      private SqlColumnBinder IsSystemColumn = new SqlColumnBinder("IsSystem");
      private SqlColumnBinder IsDeletedColumn = new SqlColumnBinder("IsDeleted");
      private SqlColumnBinder SchemaNameColumn = new SqlColumnBinder("SchemaName");
      private SqlColumnBinder ModelTableNameColumn = new SqlColumnBinder("ModelTableName");
      private SqlColumnBinder ModelColumnNameColumn = new SqlColumnBinder("ModelColumnName");
      private SqlColumnBinder IsHistoryEnabledColumn = new SqlColumnBinder("IsHistoryEnabled");
      private SqlColumnBinder IsPersonColumn = new SqlColumnBinder("IsPerson");
      private SqlColumnBinder SourceKeyFieldNameColumn = new SqlColumnBinder("SourceKeyFieldName");

      protected override ProcessFieldInfo Bind() => new ProcessFieldInfo()
      {
        Name = this.NameColumn.GetString((IDataReader) this.Reader, false),
        ReferenceName = this.ReferenceNameColumn.GetString((IDataReader) this.Reader, false),
        Description = this.DescriptionColumn.GetString((IDataReader) this.Reader, true),
        FieldType = this.FieldTypeColumn.GetString((IDataReader) this.Reader, false),
        IsSystem = this.IsSystemColumn.GetBoolean((IDataReader) this.Reader, false),
        IsDeleted = this.IsDeletedColumn.GetBoolean((IDataReader) this.Reader, false),
        IsHistoryEnabled = !this.IsHistoryEnabledColumn.ColumnExists((IDataReader) this.Reader) || this.IsHistoryEnabledColumn.GetBoolean((IDataReader) this.Reader, true),
        IsPerson = !this.IsPersonColumn.ColumnExists((IDataReader) this.Reader) || this.IsPersonColumn.GetBoolean((IDataReader) this.Reader, true),
        SourceFieldName = this.SourceFieldNameColumn.ColumnExists((IDataReader) this.Reader) ? this.SourceFieldNameColumn.GetString((IDataReader) this.Reader, true) : (string) null,
        SourceKeyFieldName = this.SourceKeyFieldNameColumn.ColumnExists((IDataReader) this.Reader) ? this.SourceKeyFieldNameColumn.GetString((IDataReader) this.Reader, true) : (string) null,
        ColumnName = this.ModelColumnNameColumn.ColumnExists((IDataReader) this.Reader) ? this.ModelColumnNameColumn.GetString((IDataReader) this.Reader, true) : (string) null,
        TableName = this.ModelTableNameColumn.ColumnExists((IDataReader) this.Reader) ? this.ModelTableNameColumn.GetString((IDataReader) this.Reader, true) : (string) null
      };
    }
  }
}
