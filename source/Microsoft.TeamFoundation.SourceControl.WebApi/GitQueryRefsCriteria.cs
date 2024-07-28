// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitQueryRefsCriteria
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitQueryRefsCriteria
  {
    [DataMember(Name = "refNames", EmitDefaultValue = false)]
    public IEnumerable<string> RefNames { get; set; }

    [DataMember(Name = "commitIds", EmitDefaultValue = false)]
    public IEnumerable<string> CommitIds { get; set; }

    [DataMember(Name = "searchType", EmitDefaultValue = false)]
    public GitRefSearchType SearchType { get; set; }
  }
}
