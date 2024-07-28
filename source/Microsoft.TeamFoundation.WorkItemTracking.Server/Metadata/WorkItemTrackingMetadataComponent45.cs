// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent45
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent45 : WorkItemTrackingMetadataComponent44
  {
    public override IReadOnlyCollection<ProcessChangedRecord> GetProcessesForChangedWorkItemTypeBehaviors(
      DateTime sinceWatermark)
    {
      this.PrepareStoredProcedure("prc_GetProcessesForWorkItemTypeBehaviorChangedSinceWatermark");
      this.BindDateTime("@watermark", sinceWatermark);
      IEnumerable<ProcessChangedRecord> source = (IEnumerable<ProcessChangedRecord>) null;
      using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProcessChangedRecord>((ObjectBinder<ProcessChangedRecord>) new WorkItemTrackingMetadataComponent35.ProcessChangedRecordBinder());
        source = (IEnumerable<ProcessChangedRecord>) resultCollection.GetCurrent<ProcessChangedRecord>().Items;
      }
      return (IReadOnlyCollection<ProcessChangedRecord>) source.ToList<ProcessChangedRecord>();
    }
  }
}
