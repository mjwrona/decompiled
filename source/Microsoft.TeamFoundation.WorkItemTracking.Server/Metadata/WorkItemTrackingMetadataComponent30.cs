// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent30
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent30 : WorkItemTrackingMetadataComponent29
  {
    protected virtual WorkItemTrackingMetadataComponent30.FieldCacheStampRecordBinder GetFieldCacheStampRecordBinder() => new WorkItemTrackingMetadataComponent30.FieldCacheStampRecordBinder();

    public override List<FieldRecord> GetFieldRecordsIncremental(
      long sinceFieldCacheStamp,
      out long maxCacheStamp,
      bool disableDataspaceRls = false,
      bool includeDeleted = false)
    {
      this.DataspaceRlsEnabled = !disableDataspaceRls;
      this.PrepareStoredProcedure("prc_GetFieldEntriesIncremental");
      this.BindLong("@sinceFieldCacheStamp", sinceFieldCacheStamp);
      this.BindIncludeDeletedForFields(includeDeleted);
      long cacheStamp = 0;
      List<FieldRecord> recordsIncremental = this.ExecuteUnknown<List<FieldRecord>>((System.Func<IDataReader, List<FieldRecord>>) (reader =>
      {
        cacheStamp = this.GetFieldCacheStampRecordBinder().BindAll(reader).ToList<long>().First<long>();
        return reader.NextResult() ? this.GetFieldRecordBinder().BindAll(reader).ToList<FieldRecord>() : throw new InvalidOperationException("Next result missing in prc_GetFieldEntriesIncremental V30");
      }));
      maxCacheStamp = cacheStamp;
      return recordsIncremental;
    }

    protected class FieldCacheStampRecordBinder : WorkItemTrackingObjectBinder<long>
    {
      protected SqlColumnBinder m_cacheStamp;

      public FieldCacheStampRecordBinder() => this.m_cacheStamp = new SqlColumnBinder("CacheStamp");

      public override long Bind(IDataReader reader) => this.m_cacheStamp.GetInt64(reader);
    }
  }
}
