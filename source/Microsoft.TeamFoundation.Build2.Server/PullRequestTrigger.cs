// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.PullRequestTrigger
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public class PullRequestTrigger : FilteredBuildTrigger
  {
    [DataMember(Name = "Forks", EmitDefaultValue = false)]
    private Forks m_forks;

    public PullRequestTrigger()
      : base(DefinitionTriggerType.PullRequest)
    {
    }

    public Forks Forks
    {
      get
      {
        if (this.m_forks == null)
          this.m_forks = new Forks();
        return this.m_forks;
      }
      set => this.m_forks = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public bool RequireCommentsForNonTeamMembersOnly { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool RequireCommentsForNonTeamMemberAndNonContributors { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsCommentRequiredForPullRequest { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? AutoCancel { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PipelineTriggerSettings PipelineTriggerSettings { get; set; }

    public override BuildTrigger Clone()
    {
      PullRequestTrigger trigger = new PullRequestTrigger();
      this.CloneInternal((BuildTrigger) trigger);
      return (BuildTrigger) trigger;
    }

    protected override BuildTrigger CloneInternal(BuildTrigger trigger)
    {
      base.CloneInternal(trigger);
      (trigger as PullRequestTrigger).Forks = new Forks()
      {
        AllowSecrets = this.Forks.AllowSecrets,
        Enabled = this.Forks.Enabled
      };
      return trigger;
    }
  }
}
