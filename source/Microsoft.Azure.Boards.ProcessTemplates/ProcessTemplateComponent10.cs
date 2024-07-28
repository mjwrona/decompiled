// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessTemplateComponent10
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  internal class ProcessTemplateComponent10 : ProcessTemplateComponent9
  {
    protected override ObjectBinder<ProcessTemplateDescriptorEntry> GetProcessTemplateDescriptorRowBinder() => (ObjectBinder<ProcessTemplateDescriptorEntry>) new ProcessTemplateComponent.ProcessTemplateDescriptorRowBinder3(this.Scope);

    public override ProcessTemplateDescriptorEntry UpdateProcessTemplateDescriptor(
      ServiceLevel serviceLevel,
      string name,
      string description,
      int majorVersion,
      int minorVersion,
      Guid processTypeId,
      byte[] fileHashValue,
      int fileId,
      string referenceName,
      Guid? baseTypeId,
      Guid? changedBy)
    {
      this.PrepareStoredProcedure("prc_AddProcessTemplateDescriptor");
      this.BindString("@name", name, 256, false, SqlDbType.NVarChar);
      this.BindString("@description", description, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindInt("@majorVersion", majorVersion);
      this.BindInt("@minorVersion", minorVersion);
      this.BindGuid("@type", processTypeId);
      this.BindBinary("@fileHashValue", fileHashValue, 16, SqlDbType.Binary);
      this.BindString("@plugins", "", -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("@fileId", fileId);
      this.BindString("@serviceLevel", this.GetServiceLevelString(serviceLevel), int.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@processDataXml", "", int.MaxValue, false, SqlDbType.NVarChar);
      this.BindInt("@maxProcessCount", int.MaxValue);
      this.BindString("@referenceName", referenceName, 256, true, SqlDbType.NVarChar);
      if (baseTypeId.HasValue || changedBy.HasValue)
        this.BindString("@extensionData", TeamFoundationSerializationUtility.SerializeToString<ProcessExtensionData>(new ProcessExtensionData()
        {
          CreatedDate = DateTime.UtcNow,
          CreatedBy = changedBy.GetValueOrDefault(),
          Inherits = baseTypeId.GetValueOrDefault()
        }), int.MaxValue, false, SqlDbType.NVarChar);
      else
        this.BindString("@extensionData", (string) null, int.MaxValue, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplateDescriptorEntry>(this.GetProcessTemplateDescriptorRowBinder());
      return resultCollection.GetCurrent<ProcessTemplateDescriptorEntry>().Items.FirstOrDefault<ProcessTemplateDescriptorEntry>();
    }

    public override void DeleteProcess(Guid specificProcessId, Guid processTypeId)
    {
      this.DeleteProcessProperties(processTypeId);
      this.PrepareStoredProcedure("prc_DeleteProcessTemplateDescriptor");
      this.BindGuid("templateId", specificProcessId);
      this.ExecuteNonQuery();
    }

    protected override void DeleteProcessProperties(Guid processTypeId)
    {
      if (!this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return;
      DateTime utcNow = DateTime.UtcNow;
      this.PrepareStoredProcedure("prc_DeleteAllProcessProperties");
      this.BindGuid("templateTypeId", processTypeId);
      this.BindGuid("updatedBy", this.RequestContext.GetUserId());
      this.BindDateTime("updateDate", utcNow);
      this.BindString("@propertyName", "All", 64, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@propertyValue", "All", 64, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    protected override IReadOnlyCollection<ProcessPropertyEntry> GetProcessProperties(
      Guid templateTypeId)
    {
      if (!this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return (IReadOnlyCollection<ProcessPropertyEntry>) Array.Empty<ProcessPropertyEntry>();
      this.PrepareStoredProcedure("prc_GetProcessProperties");
      this.BindNullableGuid(nameof (templateTypeId), templateTypeId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessPropertyEntry>(this.GetProcessPropertiesRowBinder());
      return (IReadOnlyCollection<ProcessPropertyEntry>) resultCollection.GetCurrent<ProcessPropertyEntry>().Items;
    }

    protected override void UpdateProcessProperties(
      Guid processTypeId,
      string propertyName,
      string propertyValue,
      Guid updatedBy)
    {
      if (!this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return;
      DateTime utcNow = DateTime.UtcNow;
      this.PrepareStoredProcedure("prc_UpdateProcessProperty");
      this.BindGuid("templateTypeId", processTypeId);
      this.BindString("@propertyName", propertyName, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@propertyValue", propertyValue, 64, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid(nameof (updatedBy), updatedBy);
      this.BindDateTime("updateDate", utcNow);
      this.ExecuteNonQuery();
    }

    protected override void UpdateDefaultProcessProperty(
      Guid templateTypeId,
      string propertyName,
      string propertyValue,
      Guid updatedBy)
    {
      if (!this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return;
      DateTime utcNow = DateTime.UtcNow;
      this.PrepareStoredProcedure("prc_UpdateDefaultProcessProperty");
      this.BindGuid(nameof (templateTypeId), templateTypeId);
      this.BindString("@propertyName", propertyName, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@propertyValue", propertyValue, 64, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindGuid(nameof (updatedBy), updatedBy);
      this.BindDateTime("updateDate", utcNow);
      this.ExecuteNonQuery();
    }
  }
}
