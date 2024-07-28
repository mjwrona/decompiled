// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent27
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent27 : WorkItemTrackingMetadataComponent26
  {
    public override IEnumerable<ConstantsSearchRecord> SearchConstantsRecords(
      IEnumerable<string> searchValues,
      IEnumerable<Guid> tfIds,
      bool includeInactiveIdentities,
      bool isHostedDeployment)
    {
      IEnumerable<string> rows = searchValues.Distinct<string>();
      this.PrepareStoredProcedure("prc_SearchConstantsNames");
      this.BindStringTable("@personNames", rows);
      this.BindBoolean("@includeInactiveIdentities", includeInactiveIdentities);
      this.BindBoolean("@isHostedDeployment", isHostedDeployment);
      return this.ExecuteUnknown<IEnumerable<ConstantsSearchRecord>>((System.Func<IDataReader, IEnumerable<ConstantsSearchRecord>>) (reader => (IEnumerable<ConstantsSearchRecord>) this.GetSearchConstantRecordBinder().BindAll(reader).ToList<ConstantsSearchRecord>()));
    }
  }
}
