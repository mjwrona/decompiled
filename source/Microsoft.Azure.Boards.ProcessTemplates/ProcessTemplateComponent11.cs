// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessTemplateComponent11
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  internal class ProcessTemplateComponent11 : ProcessTemplateComponent10
  {
    public override ProcessTemplateDescriptorEntry RestoreProcess(Guid processTypeId)
    {
      if (!this.RequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return (ProcessTemplateDescriptorEntry) null;
      this.PrepareStoredProcedure("prc_RestoreSoftDeletedProcessTemplateDescriptor");
      this.BindGuid("@processTypeId", processTypeId);
      this.BindString("@serviceLevel", this.GetServiceLevelString(this.GetServiceLevel()), int.MaxValue, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplateDescriptorEntry>(this.GetProcessTemplateDescriptorRowBinder());
      return resultCollection.GetCurrent<ProcessTemplateDescriptorEntry>().Items.FirstOrDefault<ProcessTemplateDescriptorEntry>();
    }
  }
}
