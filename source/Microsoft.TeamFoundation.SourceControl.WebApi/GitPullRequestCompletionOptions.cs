// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitPullRequestCompletionOptions
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitPullRequestCompletionOptions : VersionControlSecuredObject
  {
    [DataMember(Name = "mergeCommitMessage", EmitDefaultValue = false)]
    public string MergeCommitMessage { get; set; }

    [DataMember(Name = "deleteSourceBranch", EmitDefaultValue = false)]
    public bool DeleteSourceBranch { get; set; }

    [DataMember(Name = "squashMerge", IsRequired = false, EmitDefaultValue = false)]
    public bool SquashMerge { get; set; }

    [DataMember(Name = "mergeStrategy", IsRequired = false, EmitDefaultValue = false)]
    public GitPullRequestMergeStrategy? MergeStrategy { get; set; }

    [DataMember(Name = "bypassPolicy", EmitDefaultValue = false)]
    public bool BypassPolicy { get; set; }

    [DataMember(Name = "bypassReason", EmitDefaultValue = false)]
    public string BypassReason { get; set; }

    [DataMember(Name = "transitionWorkItems", EmitDefaultValue = false)]
    public bool TransitionWorkItems { get; set; }

    [DataMember(Name = "triggeredByAutoComplete", EmitDefaultValue = false)]
    public bool TriggeredByAutoComplete { get; set; }

    [DataMember(Name = "autoCompleteIgnoreConfigIds", EmitDefaultValue = false)]
    public List<int> AutoCompleteIgnoreConfigIds { get; set; }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      if (!(obj is GitPullRequestCompletionOptions completionOptions))
        return false;
      List<int> completeIgnoreConfigIds1 = this.AutoCompleteIgnoreConfigIds;
      // ISSUE: explicit non-virtual call
      int num1 = completeIgnoreConfigIds1 != null ? (__nonvirtual (completeIgnoreConfigIds1.Count) > 0 ? 1 : 0) : 0;
      List<int> completeIgnoreConfigIds2 = completionOptions.AutoCompleteIgnoreConfigIds;
      // ISSUE: explicit non-virtual call
      int num2 = completeIgnoreConfigIds2 != null ? (__nonvirtual (completeIgnoreConfigIds2.Count) > 0 ? 1 : 0) : 0;
      if (num1 != num2)
        return false;
      List<int> completeIgnoreConfigIds3 = this.AutoCompleteIgnoreConfigIds;
      // ISSUE: explicit non-virtual call
      if ((completeIgnoreConfigIds3 != null ? (__nonvirtual (completeIgnoreConfigIds3.Count) > 0 ? 1 : 0) : 0) != 0 && !new HashSet<int>((IEnumerable<int>) this.AutoCompleteIgnoreConfigIds).SetEquals((IEnumerable<int>) completionOptions.AutoCompleteIgnoreConfigIds) || this.DeleteSourceBranch != completionOptions.DeleteSourceBranch || this.SquashMerge != completionOptions.SquashMerge)
        return false;
      GitPullRequestMergeStrategy? mergeStrategy1 = this.MergeStrategy;
      GitPullRequestMergeStrategy? mergeStrategy2 = completionOptions.MergeStrategy;
      return mergeStrategy1.GetValueOrDefault() == mergeStrategy2.GetValueOrDefault() & mergeStrategy1.HasValue == mergeStrategy2.HasValue && this.MergeCommitMessage == completionOptions.MergeCommitMessage && this.BypassPolicy == completionOptions.BypassPolicy && this.TransitionWorkItems == completionOptions.TransitionWorkItems && this.TriggeredByAutoComplete == completionOptions.TriggeredByAutoComplete;
    }

    public override int GetHashCode() => base.GetHashCode();

    public void Normalize()
    {
      if (!this.MergeStrategy.HasValue)
      {
        this.MergeStrategy = new GitPullRequestMergeStrategy?(this.SquashMerge ? GitPullRequestMergeStrategy.Squash : GitPullRequestMergeStrategy.NoFastForward);
      }
      else
      {
        GitPullRequestMergeStrategy? mergeStrategy = this.MergeStrategy;
        GitPullRequestMergeStrategy requestMergeStrategy = GitPullRequestMergeStrategy.Squash;
        this.SquashMerge = mergeStrategy.GetValueOrDefault() == requestMergeStrategy & mergeStrategy.HasValue;
      }
      if (this.AutoCompleteIgnoreConfigIds != null)
        this.AutoCompleteIgnoreConfigIds = new HashSet<int>((IEnumerable<int>) this.AutoCompleteIgnoreConfigIds).OrderBy<int, int>((Func<int, int>) (x => x)).ToList<int>();
      else
        this.AutoCompleteIgnoreConfigIds = (List<int>) null;
    }
  }
}
