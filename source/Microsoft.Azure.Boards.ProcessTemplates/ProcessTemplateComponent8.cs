// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessTemplateComponent8
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
  internal class ProcessTemplateComponent8 : ProcessTemplateComponent7
  {
    protected override ObjectBinder<ProcessTemplateDescriptorEntry> GetProcessTemplateDescriptorRowBinder() => (ObjectBinder<ProcessTemplateDescriptorEntry>) new ProcessTemplateComponent.ProcessTemplateDescriptorRowBinder2(this.Scope);

    public override void UpdateProcessStatuses(IEnumerable<Guid> typeIds, ProcessStatus status)
    {
      this.PrepareStoredProcedure("prc_UpdateProcessStatuses");
      this.BindGuidTable("@typeIds", typeIds);
      this.BindInt("@processStatus", (int) status);
      this.ExecuteNonQuery();
    }

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
      this.BindString("@description", description, 1024, false, SqlDbType.NVarChar);
      this.BindInt("@majorVersion", majorVersion);
      this.BindInt("@minorVersion", minorVersion);
      this.BindGuid("@type", processTypeId);
      this.BindBinary("@fileHashValue", fileHashValue, 16, SqlDbType.Binary);
      this.BindString("@plugins", "", -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("@fileId", fileId);
      this.BindString("@serviceLevel", this.GetServiceLevelString(serviceLevel), int.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@processDataXml", "", int.MaxValue, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplateDescriptorEntry>(this.GetProcessTemplateDescriptorRowBinder());
      return resultCollection.GetCurrent<ProcessTemplateDescriptorEntry>().Items.FirstOrDefault<ProcessTemplateDescriptorEntry>();
    }
  }
}
