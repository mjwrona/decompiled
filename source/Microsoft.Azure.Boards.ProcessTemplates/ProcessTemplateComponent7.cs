// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessTemplateComponent7
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  internal class ProcessTemplateComponent7 : ProcessTemplateComponent6
  {
    public override ICollection<ProcessTemplateDescriptorEntry> GetProcessHistory(Guid typeId)
    {
      this.PrepareStoredProcedure("prc_GetProcessTemplateTypeHistory");
      this.BindGuid("@typeId", typeId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ProcessTemplateDescriptorEntry>(this.GetProcessTemplateDescriptorRowBinder());
      return (ICollection<ProcessTemplateDescriptorEntry>) resultCollection.GetCurrent<ProcessTemplateDescriptorEntry>().Items;
    }
  }
}
