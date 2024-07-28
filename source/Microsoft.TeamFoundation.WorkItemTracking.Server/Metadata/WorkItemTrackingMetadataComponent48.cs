// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent48
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent48 : WorkItemTrackingMetadataComponent47
  {
    internal override int GetMaxProvisionedWorkItemTypeId()
    {
      this.PrepareStoredProcedure("prc_GetMaxProvisionedWorkItemTypeId");
      using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new WorkItemTrackingMetadataComponent48.MaxWorkItemTypeIDColumns());
        return resultCollection.GetCurrent<int>().Items.FirstOrDefault<int>();
      }
    }

    internal class MaxWorkItemTypeIDColumns : ObjectBinder<int>
    {
      private SqlColumnBinder maxWorkItemTypeID = new SqlColumnBinder("MaxWorkItemTypeID");

      protected override int Bind() => this.maxWorkItemTypeID.GetInt32((IDataReader) this.Reader, 0);
    }
  }
}
