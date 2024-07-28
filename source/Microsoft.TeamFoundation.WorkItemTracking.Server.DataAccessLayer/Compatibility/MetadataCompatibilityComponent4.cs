// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility.MetadataCompatibilityComponent4
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility
{
  internal class MetadataCompatibilityComponent4 : MetadataCompatibilityComponent3
  {
    public override LegacyCachestampDescriptor GetWorkItemMetadataBucketToCachestampMapping(
      MetadataTable metadataTable,
      long bucketId)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemMetadataBucketToCachestampMapping");
      this.BindInt("@metadataType", (int) metadataTable);
      this.BindLong("@bucketId", bucketId);
      return this.ExecuteUnknown<LegacyCachestampDescriptor>(new System.Func<IDataReader, LegacyCachestampDescriptor>(((WorkItemTrackingObjectBinder<LegacyCachestampDescriptor>) new MetadataCompatibilityComponent4.CachestampBinder2()).Bind));
    }

    protected class CachestampBinder2 : MetadataCompatibilityComponent3.CachestampBinder
    {
      private SqlColumnBinder m_isIdentityOnlyChange = new SqlColumnBinder("IsFollowedByIdentityOnlyChanges");

      public override LegacyCachestampDescriptor Bind(IDataReader reader)
      {
        LegacyCachestampDescriptor cachestampDescriptor = new LegacyCachestampDescriptor();
        if (reader.Read())
        {
          cachestampDescriptor.Cachestamp = this.m_cachestamp.GetNullableInt64(reader);
          cachestampDescriptor.IsFollowedByIdentityOnlyChanges = this.m_isIdentityOnlyChange.GetNullableBoolean(reader);
        }
        return cachestampDescriptor;
      }
    }
  }
}
