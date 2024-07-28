// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.ChangeQueryResults
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi.Legacy
{
  [DataContract]
  public class ChangeQueryResults : VersionControlSecuredObject
  {
    [DataMember(Name = "results")]
    public IEnumerable<Change> Results { get; set; }

    [DataMember(Name = "moreResultsAvailable", EmitDefaultValue = false)]
    public bool MoreResultsAvailable { get; set; }

    [DataMember(Name = "changeCounts", EmitDefaultValue = false)]
    public Dictionary<VersionControlChangeType, int> ChangeCounts { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      IEnumerable<Change> results = this.Results;
      if (results == null)
        return;
      results.SetSecuredObject<Change>(securedObject);
    }
  }
}
