// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.HistoryQueryResults
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi.Legacy
{
  [DataContract]
  [KnownType(typeof (GitHistoryQueryResults))]
  public class HistoryQueryResults
  {
    [DataMember(Name = "results")]
    public IEnumerable<HistoryEntry> Results { get; set; }

    [DataMember(Name = "moreResultsAvailable", EmitDefaultValue = false)]
    public bool MoreResultsAvailable { get; set; }
  }
}
