// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitAsyncRefOperationSource
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Git.Server
{
  [DataContract]
  public class GitAsyncRefOperationSource
  {
    private GitAsyncRefOperationSource()
    {
    }

    public GitAsyncRefOperationSource(int pullRequestId) => this.PullRequestId = new int?(pullRequestId);

    public GitAsyncRefOperationSource(Sha1Id[] commitList)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) commitList, nameof (commitList));
      this.CommitList = commitList;
    }

    [DataMember(EmitDefaultValue = false)]
    public int? PullRequestId { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public Sha1Id[] CommitList { get; private set; }

    internal string GetSourceString() => this.PullRequestId.HasValue ? string.Format("pull request {0}", (object) this.PullRequestId) : "commits " + string.Join<Sha1Id>(",", (IEnumerable<Sha1Id>) this.CommitList);
  }
}
