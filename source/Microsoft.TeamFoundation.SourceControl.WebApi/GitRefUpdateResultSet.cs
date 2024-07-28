// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitRefUpdateResultSet
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  [Obsolete("This is unused as of Dev15 M115 and may be deleted in the future")]
  public class GitRefUpdateResultSet
  {
    [DataMember]
    public int CountFailed { get; set; }

    [DataMember]
    public int CountSucceeded { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid PushCorrelationId { get; set; }

    [DataMember]
    public Dictionary<Guid, int> PushIds { get; set; }

    [DataMember]
    public DateTime PushTime { get; set; }

    [DataMember]
    public IEnumerable<GitRefUpdateResult> Results { get; set; }
  }
}
