// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.IncludedGitCommit
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class IncludedGitCommit
  {
    [DataMember]
    public Guid RepositoryId { get; set; }

    [DataMember]
    public string CommitId { get; set; }

    [DataMember]
    public DateTime CommitTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<string> ParentCommitIds { get; set; }
  }
}
