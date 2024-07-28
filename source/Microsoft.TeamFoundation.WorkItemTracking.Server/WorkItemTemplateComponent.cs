// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTemplateComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemTemplateComponent : WorkItemTrackingResourceComponent
  {
    internal static readonly int MAX_WORK_ITEM_TEMPLATE_NAME_LENGTH = 1024;
    internal static readonly int MAX_WORK_ITEM_TYPE_NAME_LENGTH = 513;
    internal static readonly int MAX_FIELD_REF_NAME_LENGTH = 512;
    internal static readonly int MAX_DESCRIPTION_LENGTH = 4000;
    internal static readonly int MAX_TEMPLATES_PER_TYPE = 100;
    internal static readonly int MAX_FIELD_VALUE_LENGTH = 65536;
    private static readonly int ERROR_WORK_ITEM_TEMPLATE_DOES_NOT_EXIST = 600513;
    private static readonly IDictionary<int, SqlExceptionFactory> s_exceptionFactories = (IDictionary<int, SqlExceptionFactory>) new Dictionary<int, SqlExceptionFactory>();
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<WorkItemTemplateComponent>(1, true)
    }, "WorkItemTemplate", "WorkItem");

    static WorkItemTemplateComponent() => WorkItemTemplateComponent.s_exceptionFactories[WorkItemTemplateComponent.ERROR_WORK_ITEM_TEMPLATE_DOES_NOT_EXIST] = new SqlExceptionFactory(typeof (WorkItemTemplateNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((req, errNo, sqlException, sqlErr) => (Exception) new WorkItemTemplateNotFoundException()));

    public virtual void CreateTemplate(WorkItemTemplate template)
    {
      this.PrepareStoredProcedure("prc_CreateWorkItemTemplate");
      this.BindTemplateParameters(template);
      this.BindCreatedBy();
      this.ExecuteNonQuery();
    }

    public virtual void UpdateTemplate(WorkItemTemplate template)
    {
      this.PrepareStoredProcedure("prc_UpdateWorkItemTemplate");
      this.BindTemplateParameters(template);
      this.BindChangedBy();
      this.ExecuteNonQuery();
    }

    public virtual WorkItemTemplate GetTemplate(Guid projectId, Guid templateId)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemTemplateById");
      this.BindDataspaceId(projectId);
      this.BindGuid("@templateId", templateId);
      this.BindBoolean("@includeFieldValues", true);
      ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<WorkItemTemplateComponent.WorkItemTemplateDescriptorRow>((ObjectBinder<WorkItemTemplateComponent.WorkItemTemplateDescriptorRow>) new WorkItemTemplateComponent.WorkItemTemplateDescriptorRowBinder());
      resultCollection.AddBinder<WorkItemTemplateComponent.WorkItemTemplateFieldRow>((ObjectBinder<WorkItemTemplateComponent.WorkItemTemplateFieldRow>) new WorkItemTemplateComponent.WorkItemTemplateFieldRowBinder());
      WorkItemTemplateComponent.WorkItemTemplateDescriptorRow templateRow = resultCollection.GetCurrent<WorkItemTemplateComponent.WorkItemTemplateDescriptorRow>().FirstOrDefault<WorkItemTemplateComponent.WorkItemTemplateDescriptorRow>();
      if (templateRow == null)
        return (WorkItemTemplate) null;
      IEnumerable<WorkItemTemplateComponent.WorkItemTemplateFieldRow> fields = (IEnumerable<WorkItemTemplateComponent.WorkItemTemplateFieldRow>) null;
      if (resultCollection.TryNextResult())
        fields = (IEnumerable<WorkItemTemplateComponent.WorkItemTemplateFieldRow>) resultCollection.GetCurrent<WorkItemTemplateComponent.WorkItemTemplateFieldRow>();
      if (fields == null)
        fields = Enumerable.Empty<WorkItemTemplateComponent.WorkItemTemplateFieldRow>();
      return this.TemplateFromRow(templateRow, fields, projectId);
    }

    public virtual IEnumerable<WorkItemTemplateDescriptor> GetTemplates(
      Guid projectId,
      Guid ownerId)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemTemplates");
      this.BindDataspaceId(projectId);
      this.BindGuid("@ownerId", ownerId);
      this.BindString("@workItemTypeName", (string) null, WorkItemTemplateComponent.MAX_WORK_ITEM_TYPE_NAME_LENGTH, true, SqlDbType.NVarChar);
      this.BindBoolean("@includeFieldValues", false);
      ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<WorkItemTemplateComponent.WorkItemTemplateDescriptorRow>((ObjectBinder<WorkItemTemplateComponent.WorkItemTemplateDescriptorRow>) new WorkItemTemplateComponent.WorkItemTemplateDescriptorRowBinder());
      ObjectBinder<WorkItemTemplateComponent.WorkItemTemplateDescriptorRow> current = resultCollection.GetCurrent<WorkItemTemplateComponent.WorkItemTemplateDescriptorRow>();
      return current == null || current.Items == null || !current.Items.Any<WorkItemTemplateComponent.WorkItemTemplateDescriptorRow>() ? Enumerable.Empty<WorkItemTemplateDescriptor>() : (IEnumerable<WorkItemTemplateDescriptor>) current.Items.Select<WorkItemTemplateComponent.WorkItemTemplateDescriptorRow, WorkItemTemplateDescriptor>((System.Func<WorkItemTemplateComponent.WorkItemTemplateDescriptorRow, WorkItemTemplateDescriptor>) (row => this.TemplateDescriptorFromRow(row, projectId))).ToArray<WorkItemTemplateDescriptor>();
    }

    public virtual void DeleteTemplate(Guid projectId, Guid templateId)
    {
      this.PrepareStoredProcedure("prc_DeleteWorkItemTemplateByTemplateId");
      this.BindDataspaceId(projectId);
      this.BindGuid("@templateId", templateId);
      this.BindChangedBy();
      this.ExecuteNonQuery();
    }

    public virtual void DeleteAllTemplatesInProject(Guid projectId)
    {
      this.PrepareStoredProcedure("prc_DeleteWorkItemTemplatesByDataspaceId");
      this.BindDataspaceId(projectId);
      this.BindChangedBy();
      this.ExecuteNonQuery();
    }

    public virtual void DeleteTemplatesByOwner(Guid projectId, Guid ownerId)
    {
      this.PrepareStoredProcedure("prc_DeleteWorkItemTemplatesByOwnerId");
      this.BindDataspaceId(projectId);
      this.BindGuid("@ownerId", ownerId);
      this.BindChangedBy();
      this.ExecuteNonQuery();
    }

    protected WorkItemTemplateDescriptor TemplateDescriptorFromRow(
      WorkItemTemplateComponent.WorkItemTemplateDescriptorRow templateRow,
      Guid projectId)
    {
      return new WorkItemTemplateDescriptor(templateRow.Id, templateRow.Name, templateRow.Description, templateRow.WorkItemTypeName, templateRow.OwnerId, projectId);
    }

    protected WorkItemTemplate TemplateFromRow(
      WorkItemTemplateComponent.WorkItemTemplateDescriptorRow templateRow,
      IEnumerable<WorkItemTemplateComponent.WorkItemTemplateFieldRow> fields,
      Guid projectId)
    {
      return new WorkItemTemplate(templateRow.Id, templateRow.Name, (IDictionary<string, string>) fields.ToDictionary<WorkItemTemplateComponent.WorkItemTemplateFieldRow, string, string>((System.Func<WorkItemTemplateComponent.WorkItemTemplateFieldRow, string>) (f => f.FieldRefName), (System.Func<WorkItemTemplateComponent.WorkItemTemplateFieldRow, string>) (f => f.Value), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName), templateRow.Description, templateRow.WorkItemTypeName, templateRow.OwnerId, projectId);
    }

    protected void BindDataspaceId(Guid dataspaceIdentifier) => this.BindInt("@dataspaceId", this.GetDataspaceId(dataspaceIdentifier));

    protected void BindCreatedBy() => this.BindGuid("@createdBy", this.RequestContext.GetUserId());

    protected void BindChangedBy() => this.BindGuid("@changedBy", this.RequestContext.GetUserId());

    protected void BindTemplateParameters(WorkItemTemplate template)
    {
      this.BindDataspaceId(template.ProjectId);
      this.BindGuid("@ownerId", template.OwnerId);
      this.BindString("@workItemTypeName", template.WorkItemTypeName, WorkItemTemplateComponent.MAX_WORK_ITEM_TYPE_NAME_LENGTH, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid("@templateId", template.Id);
      this.BindString("@name", template.Name, WorkItemTemplateComponent.MAX_WORK_ITEM_TEMPLATE_NAME_LENGTH, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@description", template.Description, WorkItemTemplateComponent.MAX_DESCRIPTION_LENGTH, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindKeyValuePairStringTable("@fieldValues", (IEnumerable<KeyValuePair<string, string>>) template.Fields);
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => WorkItemTemplateComponent.s_exceptionFactories;

    internal class WorkItemTemplateDescriptorRow
    {
      internal Guid Id { get; set; }

      internal string Name { get; set; }

      internal string Description { get; set; }

      internal string WorkItemTypeName { get; set; }

      internal Guid OwnerId { get; set; }
    }

    internal class WorkItemTemplateDescriptorRowBinder : 
      ObjectBinder<WorkItemTemplateComponent.WorkItemTemplateDescriptorRow>
    {
      private SqlColumnBinder templateIdColumn = new SqlColumnBinder("TemplateId");
      private SqlColumnBinder ownerIdColumn = new SqlColumnBinder("OwnerId");
      private SqlColumnBinder workItemTypeNameColumn = new SqlColumnBinder("WorkItemTypeName");
      private SqlColumnBinder nameColumn = new SqlColumnBinder("Name");
      private SqlColumnBinder descriptionColumn = new SqlColumnBinder("Description");

      protected override WorkItemTemplateComponent.WorkItemTemplateDescriptorRow Bind() => new WorkItemTemplateComponent.WorkItemTemplateDescriptorRow()
      {
        Id = this.templateIdColumn.GetGuid((IDataReader) this.Reader),
        OwnerId = this.ownerIdColumn.GetGuid((IDataReader) this.Reader),
        WorkItemTypeName = this.workItemTypeNameColumn.GetString((IDataReader) this.Reader, false),
        Name = this.nameColumn.GetString((IDataReader) this.Reader, false),
        Description = this.descriptionColumn.GetString((IDataReader) this.Reader, true)
      };
    }

    internal class WorkItemTemplateFieldRow
    {
      internal string FieldRefName { get; set; }

      internal string Value { get; set; }
    }

    internal class WorkItemTemplateFieldRowBinder : 
      ObjectBinder<WorkItemTemplateComponent.WorkItemTemplateFieldRow>
    {
      private SqlColumnBinder fieldRefNameColumn = new SqlColumnBinder("FieldRefName");
      private SqlColumnBinder valueColumn = new SqlColumnBinder("Value");

      protected override WorkItemTemplateComponent.WorkItemTemplateFieldRow Bind() => new WorkItemTemplateComponent.WorkItemTemplateFieldRow()
      {
        FieldRefName = this.fieldRefNameColumn.GetString((IDataReader) this.Reader, false),
        Value = this.valueColumn.GetString((IDataReader) this.Reader, true)
      };
    }
  }
}
