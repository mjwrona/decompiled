// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.HistoryQueryResults`1
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [Obsolete("This is unused as of Dev15 M108 and may be deleted in the future")]
  [DataContract]
  [KnownType(typeof (GitHistoryQueryResults))]
  public class HistoryQueryResults<T>
  {
    [DataMember]
    public IEnumerable<HistoryEntry<T>> Results { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool MoreResultsAvailable { get; set; }
  }
}
