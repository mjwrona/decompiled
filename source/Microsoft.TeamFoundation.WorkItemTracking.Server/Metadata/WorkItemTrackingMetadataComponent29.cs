// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent29
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent29 : WorkItemTrackingMetadataComponent28
  {
    public override List<FieldRecord> GetFieldRecordsIncremental(
      long sinceFieldCacheStamp,
      out long maxCacheStamp,
      bool disableDataspaceRls = false,
      bool includeDeleted = false)
    {
      this.DataspaceRlsEnabled = !disableDataspaceRls;
      maxCacheStamp = 0L;
      this.PrepareStoredProcedure("prc_GetFieldEntriesIncremental");
      this.BindLong("@sinceFieldCacheStamp", sinceFieldCacheStamp);
      return this.ExecuteUnknown<List<FieldRecord>>((System.Func<IDataReader, List<FieldRecord>>) (reader => this.GetFieldRecordBinder().BindAll(reader).ToList<FieldRecord>()));
    }
  }
}
