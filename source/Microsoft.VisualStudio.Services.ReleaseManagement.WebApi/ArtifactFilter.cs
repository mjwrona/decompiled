// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactFilter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class ArtifactFilter : ReleaseManagementSecuredObject
  {
    [DataMember]
    public string SourceBranch { get; set; }

    [DataMember]
    public IList<string> Tags { get; set; }

    [DataMember]
    public TagFilter TagFilter { get; set; }

    [DataMember]
    public bool UseBuildDefinitionBranch { get; set; }

    [DataMember]
    public bool CreateReleaseOnBuildTagging { get; set; }

    public ArtifactFilter()
    {
      this.SourceBranch = string.Empty;
      this.Tags = (IList<string>) new List<string>();
      this.UseBuildDefinitionBranch = false;
    }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      this.TagFilter?.SetSecuredObject(token, requiredPermissions);
    }
  }
}
