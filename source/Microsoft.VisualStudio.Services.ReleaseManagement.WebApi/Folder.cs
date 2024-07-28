// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Folder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class Folder : ReleaseManagementSecuredObject
  {
    [DataMember]
    public string Path { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public DateTime CreatedOn { get; set; }

    [DataMember]
    public IdentityRef CreatedBy { get; set; }

    [DataMember]
    public DateTime? LastChangedDate { get; set; }

    [DataMember]
    public IdentityRef LastChangedBy { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual void SetSecuredObject(Guid projectId) => this.SetSecuredObject(TeamProjectSecurityConstants.NamespaceId, TeamProjectSecurityConstants.GetToken(LinkingUtilities.EncodeUri(new ArtifactId("Classification", "TeamProject", projectId.ToString("D")))), TeamProjectSecurityConstants.GenericRead);
  }
}
