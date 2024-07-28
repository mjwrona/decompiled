// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility.MetadataCompatibilityComponent2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Compatibility
{
  internal class MetadataCompatibilityComponent2 : MetadataCompatibilityComponent
  {
    public override void IncreaseWorkItemMetadataBucketIds(IEnumerable<MetadataTable> metadataTypes)
    {
      this.PrepareStoredProcedure("prc_IncreaseWorkItemMetadataBucketIds");
      this.BindInt32Table("@metadataTypes", metadataTypes.Select<MetadataTable, int>((Func<MetadataTable, int>) (m => (int) m)));
      this.ExecuteNonQuery();
    }
  }
}
