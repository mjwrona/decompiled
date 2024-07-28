// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent7
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskTrackingComponent7 : TaskTrackingComponent6
  {
    public override async Task<TaskAttachmentData> CreateAttachmentAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name,
      string attachmentPath,
      Guid requestedBy)
    {
      TaskTrackingComponent7 component = this;
      TaskAttachmentData attachmentAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (CreateAttachmentAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_CreateAttachment");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        component.BindGuid("@timelineId", timelineId);
        component.BindGuid("@recordId", recordId);
        component.BindString("@type", type, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@name", name, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@path", attachmentPath, 400, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindGuid("@requestedBy", requestedBy);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAttachmentData>((ObjectBinder<TaskAttachmentData>) new TaskAttachmentDataBinder());
          attachmentAsync = resultCollection.GetCurrent<TaskAttachmentData>().FirstOrDefault<TaskAttachmentData>();
        }
      }
      return attachmentAsync;
    }

    public override async Task<TaskAttachmentData> GetAttachmentAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name)
    {
      TaskTrackingComponent7 component = this;
      TaskAttachmentData attachmentAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAttachmentAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAttachment");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        component.BindGuid("@timelineId", timelineId);
        component.BindGuid("@recordId", recordId);
        component.BindString("@type", type, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@name", name, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAttachmentData>((ObjectBinder<TaskAttachmentData>) new TaskAttachmentDataBinder());
          attachmentAsync = resultCollection.GetCurrent<TaskAttachmentData>().FirstOrDefault<TaskAttachmentData>();
        }
      }
      return attachmentAsync;
    }

    public override async Task<IList<TaskAttachmentData>> GetAttachmentsAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid? timelineId,
      Guid? recordId,
      string type)
    {
      TaskTrackingComponent7 component = this;
      IList<TaskAttachmentData> items;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAttachmentsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAttachmentsByType");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        component.BindNullableGuid("@timelineId", timelineId, false);
        component.BindNullableGuid("@recordId", recordId, false);
        component.BindString("@type", type, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAttachmentData>((ObjectBinder<TaskAttachmentData>) new TaskAttachmentDataBinder());
          items = (IList<TaskAttachmentData>) resultCollection.GetCurrent<TaskAttachmentData>().Items;
        }
      }
      return items;
    }
  }
}
