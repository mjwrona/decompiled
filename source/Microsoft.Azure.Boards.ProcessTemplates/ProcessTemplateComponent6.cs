// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessTemplateComponent6
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  internal class ProcessTemplateComponent6 : ProcessTemplateComponent
  {
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
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplateDescriptorEntry>(this.GetProcessTemplateDescriptorRowBinder());
      return resultCollection.GetCurrent<ProcessTemplateDescriptorEntry>().Items.FirstOrDefault<ProcessTemplateDescriptorEntry>();
    }

    public override IReadOnlyCollection<ProcessTemplateDescriptorEntry> GetAllProcessDescriptors(
      ServiceLevel serviceLevel,
      bool onlyTipVersion = true)
    {
      this.PrepareStoredProcedure("prc_GetAllNonDeletedProcessTemplateDescriptors");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplateDescriptorEntry>(this.GetProcessTemplateDescriptorRowBinder());
      return ProcessTemplateComponent.FilterDescriptors((IEnumerable<ProcessTemplateDescriptorEntry>) resultCollection.GetCurrent<ProcessTemplateDescriptorEntry>().Items, serviceLevel, onlyTipVersion);
    }

    public override Guid? GetProcessDescriptorSpecificId(int integerId)
    {
      this.PrepareStoredProcedure("prc_GetProcessTemplateDescriptorIdByIntegerId");
      this.BindInt("@integerId", this.ConvertToDatabaseIntegerId(integerId));
      SqlDataReader sqlDataReader = this.ExecuteReader();
      return sqlDataReader.Read() ? new Guid?(sqlDataReader.GetGuid(0)) : new Guid?();
    }

    public override IReadOnlyCollection<ProcessTemplateDescriptorEntry> GetSpecificProcessDescriptors(
      IEnumerable<Guid> templateIds)
    {
      this.PrepareStoredProcedure("prc_GetProcessTemplateDescriptors");
      this.BindGuidTable("@templateIds", templateIds);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplateDescriptorEntry>(this.GetProcessTemplateDescriptorRowBinder());
      return (IReadOnlyCollection<ProcessTemplateDescriptorEntry>) resultCollection.GetCurrent<ProcessTemplateDescriptorEntry>().Items;
    }

    public override void SetDefaultProcessType(Guid processTypeId, Guid changedBy)
    {
      if (!this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return;
      this.PrepareStoredProcedure("prc_SetDefaultProcessTemplateType");
      this.BindGuid("@templateTypeId", processTypeId);
      this.ExecuteNonQuery();
    }

    public override Guid? GetProcessDescriptorSpecificId(
      Guid typeId,
      int majorVersion,
      int minorVersion)
    {
      this.PrepareStoredProcedure("prc_GetProcessTemplateDescriptorIdByVersion");
      this.BindGuid(nameof (typeId), typeId);
      this.BindInt(nameof (majorVersion), majorVersion);
      this.BindInt(nameof (minorVersion), minorVersion);
      SqlDataReader sqlDataReader = this.ExecuteReader();
      return sqlDataReader.Read() ? new Guid?(sqlDataReader.GetGuid(0)) : new Guid?();
    }

    public override Guid GetDefaultProcessTypeId()
    {
      if (!this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return new Guid("6B724908-EF14-45CF-84F8-768B5384DA45");
      this.PrepareStoredProcedure("prc_GetDefaultProcessTemplateType");
      SqlDataReader sqlDataReader = this.ExecuteReader();
      return sqlDataReader.Read() ? sqlDataReader.GetGuid(0) : Guid.Empty;
    }

    public override void DeleteProcess(Guid specificProcessId, Guid processTypeId)
    {
      this.DeleteProcessProperties(processTypeId);
      this.PrepareStoredProcedure("prc_DeleteProcessTemplateDescriptor");
      this.BindGuid("templateId", specificProcessId);
      this.ExecuteNonQuery();
    }
  }
}
