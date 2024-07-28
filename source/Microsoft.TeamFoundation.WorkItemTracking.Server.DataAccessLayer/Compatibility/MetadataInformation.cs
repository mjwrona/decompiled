// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility.MetadataInformation
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility
{
  internal class MetadataInformation
  {
    private MetadataBucket[] m_buckets;

    private MetadataInformation()
    {
      int length = (int) (Enum.GetValues(typeof (MetadataTable)).Cast<MetadataTable>().Max<MetadataTable>() + 1);
      this.m_buckets = new MetadataBucket[length];
      for (int index = 0; index < length; ++index)
        this.m_buckets[index] = new MetadataBucket();
    }

    internal static MetadataInformation Create(IEnumerable<MetadataInformationRecord> records)
    {
      MetadataInformation metadataInformation = new MetadataInformation();
      foreach (MetadataInformationRecord record in records)
        metadataInformation.m_buckets[record.MetadataType] = new MetadataBucket(record.BucketId, record.MaskBits);
      return metadataInformation;
    }

    public MetadataBucket this[MetadataTable table] => this.m_buckets[(int) table];

    public void Merge(MetadataInformation other)
    {
      for (int index = 0; index < this.m_buckets.Length; ++index)
        this.m_buckets[index].Merge(other.m_buckets[index]);
    }
  }
}
