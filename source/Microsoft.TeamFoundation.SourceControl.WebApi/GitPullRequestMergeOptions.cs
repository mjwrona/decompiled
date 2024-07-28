// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitPullRequestMergeOptions
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitPullRequestMergeOptions : VersionControlSecuredObject
  {
    [DataMember(Name = "disableRenames", EmitDefaultValue = false)]
    public bool? DisableRenames { get; set; }

    [DataMember(Name = "detectRenameFalsePositives", EmitDefaultValue = false)]
    public bool? DetectRenameFalsePositives { get; set; }

    [DataMember(Name = "conflictAuthorshipCommits", EmitDefaultValue = false)]
    public bool? ConflictAuthorshipCommits { get; set; }

    public override bool Equals(object obj)
    {
      if (this == obj)
        return true;
      if (!(obj is GitPullRequestMergeOptions requestMergeOptions))
        return false;
      bool? nullable1 = this.DisableRenames;
      bool? disableRenames = requestMergeOptions.DisableRenames;
      if (nullable1.GetValueOrDefault() == disableRenames.GetValueOrDefault() & nullable1.HasValue == disableRenames.HasValue)
      {
        bool? nullable2 = this.DetectRenameFalsePositives;
        nullable1 = requestMergeOptions.DetectRenameFalsePositives;
        if (nullable2.GetValueOrDefault() == nullable1.GetValueOrDefault() & nullable2.HasValue == nullable1.HasValue)
        {
          nullable1 = this.ConflictAuthorshipCommits;
          nullable2 = requestMergeOptions.ConflictAuthorshipCommits;
          return nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() & nullable1.HasValue == nullable2.HasValue;
        }
      }
      return false;
    }

    public override int GetHashCode() => base.GetHashCode();
  }
}
