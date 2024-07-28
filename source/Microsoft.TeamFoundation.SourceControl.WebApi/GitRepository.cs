// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitRepository
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [ServiceEventObject]
  [DataContract]
  [KnownType(typeof (GitRepositoriesCollection))]
  [KnownType(typeof (GitForkTeamProjectReference))]
  public class GitRepository : VersionControlSecuredObject
  {
    public const string c_rootPath = "/";

    [DataMember(EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(Name = "Project", EmitDefaultValue = false)]
    public TeamProjectReference ProjectReference { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DefaultBranch { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public long? Size { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string RemoteUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string SshUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string WebUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string[] ValidRemoteUrls { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsFork { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public GitRepositoryRef ParentRepository { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "_links")]
    public ReferenceLinks Links { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? IsDisabled { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? IsInMaintenance { get; set; }

    public string GetTeamProjectUri()
    {
      string teamProjectUri = (string) null;
      if (this.ProjectReference != null)
        teamProjectUri = LinkingUtilities.EncodeUri(new ArtifactId("Classification", "TeamProject", this.ProjectReference.Id.ToString("D")));
      return teamProjectUri;
    }

    public override void SetSecuredObject(ISecuredObject securedObject) => base.SetSecuredObject(securedObject);
  }
}
