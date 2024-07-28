// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.PipelineGeneralSettings
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  [ClientIncludeModel]
  public class PipelineGeneralSettings : BaseSecuredObject
  {
    [DataMember(EmitDefaultValue = false)]
    public bool? EnforceReferencedRepoScopedToken;
    [DataMember(EmitDefaultValue = false)]
    public bool? DisableClassicPipelineCreation;
    [DataMember(EmitDefaultValue = false)]
    public bool? DisableClassicBuildPipelineCreation;
    [DataMember(EmitDefaultValue = false)]
    public bool? DisableClassicReleasePipelineCreation;
    [DataMember(EmitDefaultValue = false)]
    public bool? ForkProtectionEnabled;
    [DataMember(EmitDefaultValue = false)]
    public bool? BuildsEnabledForForks;
    [DataMember(EmitDefaultValue = false)]
    public bool? EnforceJobAuthScopeForForks;
    [DataMember(EmitDefaultValue = false)]
    public bool? EnforceNoAccessToSecretsFromForks;
    [DataMember(EmitDefaultValue = false)]
    public bool? IsCommentRequiredForPullRequest;
    [DataMember(EmitDefaultValue = false)]
    public bool? RequireCommentsForNonTeamMembersOnly;
    [DataMember(EmitDefaultValue = false)]
    public bool? RequireCommentsForNonTeamMemberAndNonContributors;
    [DataMember(EmitDefaultValue = false)]
    public bool? EnableShellTasksArgsSanitizing;
    [DataMember(EmitDefaultValue = false)]
    public bool? EnableShellTasksArgsSanitizingAudit;
    [DataMember(EmitDefaultValue = false)]
    public bool? DisableImpliedYAMLCiTrigger;

    public PipelineGeneralSettings(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember(EmitDefaultValue = false)]
    public bool? StatusBadgesArePrivate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? EnforceSettableVar { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? AuditEnforceSettableVar { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? EnforceJobAuthScope { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? EnforceJobAuthScopeForReleases { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? PublishPipelineMetadata { get; set; }
  }
}
