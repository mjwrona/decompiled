// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitPullRequestQuery
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitPullRequestQuery : VersionControlSecuredObject
  {
    [DataMember(Name = "queries", EmitDefaultValue = false)]
    public List<GitPullRequestQueryInput> QueryInputs { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<IDictionary<string, List<GitPullRequest>>> Results { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      List<GitPullRequestQueryInput> queryInputs = this.QueryInputs;
      if (queryInputs != null)
        queryInputs.SetSecuredObject<GitPullRequestQueryInput>(securedObject);
      if (this.Results == null)
        return;
      foreach (IDictionary<string, List<GitPullRequest>> result in this.Results)
      {
        foreach (List<GitPullRequest> securableObjects in (IEnumerable<List<GitPullRequest>>) result.Values)
        {
          if (securableObjects != null)
            securableObjects.SetSecuredObject<GitPullRequest>(securedObject);
        }
      }
    }
  }
}
