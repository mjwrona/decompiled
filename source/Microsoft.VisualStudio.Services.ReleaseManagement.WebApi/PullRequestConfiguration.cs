// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PullRequestConfiguration
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class PullRequestConfiguration : ReleaseManagementSecuredObject
  {
    [DataMember]
    public bool UseArtifactReference { get; set; }

    [DataMember]
    public CodeRepositoryReference CodeRepositoryReference { get; set; }

    public PullRequestConfiguration() => this.CodeRepositoryReference = new CodeRepositoryReference();

    public PullRequestConfiguration(
      bool useArtifactReference,
      CodeRepositoryReference sourceCodeReference)
    {
      this.UseArtifactReference = useArtifactReference;
      this.CodeRepositoryReference = sourceCodeReference;
    }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.CodeRepositoryReference?.SetSecuredObject(token, requiredPermissions);
    }
  }
}
