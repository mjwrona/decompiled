// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview.GitRepositoryData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97A9928B-E499-4978-909F-1EBC8C5535AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview
{
  [DataContract]
  public class GitRepositoryData : AbstractProjectSecuredObject
  {
    [DataMember]
    public GitRepository Repository { get; set; }

    [DataMember]
    public bool SshEnabled { get; set; }

    [DataMember]
    public string SshUrl { get; set; }

    [DataMember]
    public string CloneUrl { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      ArgumentUtility.CheckForNull<GitRepository>(this.Repository, "Repository");
      base.SetSecuredObject(securedObject);
      this.Repository.SetSecuredObject(securedObject);
    }
  }
}
