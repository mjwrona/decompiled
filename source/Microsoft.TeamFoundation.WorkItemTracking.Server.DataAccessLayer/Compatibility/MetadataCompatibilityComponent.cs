// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility.MetadataCompatibilityComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility
{
  internal class MetadataCompatibilityComponent : WorkItemTrackingResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[5]
    {
      (IComponentCreator) new ComponentCreator<MetadataCompatibilityComponent>(1),
      (IComponentCreator) new ComponentCreator<MetadataCompatibilityComponent2>(2),
      (IComponentCreator) new ComponentCreator<MetadataCompatibilityComponent3>(3),
      (IComponentCreator) new ComponentCreator<MetadataCompatibilityComponent4>(4),
      (IComponentCreator) new ComponentCreator<MetadataCompatibilityComponent5>(5)
    }, "WorkItemMetadataCompatibility");

    public virtual IEnumerable<MetadataInformationRecord> GetMetadataInformation()
    {
      this.PrepareStoredProcedure("prc_GetWorkItemMetadataInformation");
      return this.ExecuteUnknown<IEnumerable<MetadataInformationRecord>>(new System.Func<IDataReader, IEnumerable<MetadataInformationRecord>>(((WorkItemTrackingObjectBinder<MetadataInformationRecord>) new MetadataCompatibilityComponent.MetadataInformationBinder()).BindAll));
    }

    public virtual void UpdateWorkItemMetadataInformation(
      IEnumerable<KeyValuePair<MetadataTable, int>> newMaskBits)
    {
      this.PrepareStoredProcedure("prc_StampWorkItemMetadata");
      this.BindKeyValuePairInt32Int32Table("@maskBits", newMaskBits.Select<KeyValuePair<MetadataTable, int>, KeyValuePair<int, int>>((System.Func<KeyValuePair<MetadataTable, int>, KeyValuePair<int, int>>) (x => new KeyValuePair<int, int>((int) x.Key, x.Value))));
      this.ExecuteNonQuery();
    }

    public virtual void IncreaseWorkItemMetadataBucketIds(IEnumerable<MetadataTable> metadataTypes) => throw new NotSupportedException();

    public virtual LegacyCachestampDescriptor GetWorkItemMetadataBucketToCachestampMapping(
      MetadataTable metadataTable,
      long bucketId)
    {
      return (LegacyCachestampDescriptor) null;
    }

    public virtual StampDbRequestResult StampDbIfBackCompatTablesChanged(int autoStampDbState) => StampDbRequestResult.SprocNotYetAvailable;

    private class MetadataInformationBinder : WorkItemTrackingObjectBinder<MetadataInformationRecord>
    {
      private SqlColumnBinder m_metadataType = new SqlColumnBinder("MetadataType");
      private SqlColumnBinder m_bucketId = new SqlColumnBinder("BucketId");
      private SqlColumnBinder m_maskBits = new SqlColumnBinder("MaskBits");

      public override MetadataInformationRecord Bind(IDataReader reader) => new MetadataInformationRecord()
      {
        MetadataType = this.m_metadataType.GetInt32(reader),
        BucketId = this.m_bucketId.GetInt64(reader),
        MaskBits = this.m_maskBits.GetInt32(reader)
      };
    }
  }
}
