// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalPersonIdMapElement
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalPersonIdMapElement : DalSqlElement
  {
    private ISet<string> m_personNames = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);

    public void AddPersonName(string personName) => this.m_personNames.Add(personName);

    public void JoinBatch(ElementGroup group)
    {
      this.SetOutputs(0);
      this.SetGroup(group);
      if (this.Version >= 18)
        this.AppendSql(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\r\ndeclare {0} typ_WitConstIdMapTable\r\ninsert into {0}\r\nexec {3} = dbo.prc_iiGeneratePersonIdMap {2}, {1}\r\nif {3} <> 0\r\nbegin \r\n    rollback tran\r\n    {4}\r\n    return\r\nend\r\n", (object) "@personIdMap", (object) this.SqlBatch.AddParameterTable<string>((WorkItemTrackingTableValueParameter<string>) new StringTable((IEnumerable<string>) this.m_personNames), "@personNames"), (object) "@partitionId", (object) "@status", this.m_update.NeedToReleaseWatermark ? (object) "EXEC dbo.prc_ReleaseWorkItemWatermark @partitionId, @watermark" : (object) ""));
      else if (this.IsSchemaPartitioned)
        this.AppendSql(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\r\ndeclare @distinctPersonCount int\r\ndeclare @distinctPersonNames dbo.typ_StringTable\r\ninsert into @distinctPersonNames\r\n select distinct * from {1}\r\nset @distinctPersonCount = @@rowcount\r\ndeclare {0} typ_WitConstIdMapTable;\r\nif (@distinctPersonCount > 0)\r\nbegin\r\n insert into {0} select * from [dbo].[GetConstantsForWorkItems]({2},@distinctPersonNames)\r\n option (optimize for({2} unknown))\r\n if (@@rowcount < @distinctPersonCount)\r\n begin\r\n  declare @newPersonNames dbo.typ_StringTable;\r\n  insert into @newPersonNames (Data) select Data from @distinctPersonNames where Data not in (select DisplayPart from {0});\r\n  exec {3} = [dbo].[AddServerConstants] {2},@newPersonNames\r\n  if {3} <> 0\r\n  begin\r\n   rollback transaction\r\n   return\r\n  end\r\n  delete from {0}\r\n  insert into {0} select * from [dbo].[GetConstantsForWorkItems]({2},@distinctPersonNames)\r\n  option (optimize for({2} unknown))\r\n end\r\nend\r\n", (object) "@personIdMap", (object) this.SqlBatch.AddParameterTable<string>((WorkItemTrackingTableValueParameter<string>) new StringTable((IEnumerable<string>) this.m_personNames), "@personNames"), (object) "@partitionId", (object) "@status"));
      else
        this.AppendSql(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\r\ndeclare @distinctPersonCount int\r\ndeclare @distinctPersonNames dbo.typ_StringTable\r\ninsert into @distinctPersonNames\r\n select distinct * from {1}\r\nset @distinctPersonCount = @@rowcount\r\ndeclare {0} typ_WitConstIdMapTable;\r\nif (@distinctPersonCount > 0)\r\nbegin\r\n insert into {0} select * from [dbo].[GetConstantsForWorkItems](@distinctPersonNames)\r\n if (@@rowcount < @distinctPersonCount)\r\n begin\r\n  declare @newPersonNames dbo.typ_StringTable;\r\n  insert into @newPersonNames (Data) select Data from @distinctPersonNames where Data not in (select DisplayPart from {0});\r\n  exec [dbo].[AddServerConstants] @newPersonNames\r\n  delete from {0}\r\n  insert into {0} select * from [dbo].[GetConstantsForWorkItems](@distinctPersonNames)\r\n end\r\nend\r\n", (object) "@personIdMap", (object) this.SqlBatch.AddParameterTable<string>((WorkItemTrackingTableValueParameter<string>) new StringTable((IEnumerable<string>) this.m_personNames), "@personNames")));
    }
  }
}
