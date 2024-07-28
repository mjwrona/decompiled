// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview.RepositoryData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97A9928B-E499-4978-909F-1EBC8C5535AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview.dll

using Microsoft.TeamFoundation.SourceControl.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview
{
  [DataContract]
  public class RepositoryData : AbstractProjectSecuredObject
  {
    [DataMember]
    public SourceRepositoryTypes SourceRepositoryType { get; set; }

    [DataMember]
    public bool IsDefaultReadmeRepoPresent { get; set; }

    [DataMember]
    public ItemModel ReadmeFileItemModel { get; set; }

    [DataMember]
    public string WikiPagePath { get; set; }

    [DataMember]
    public string DisplayContent { get; set; }

    [DataMember]
    public GitRepositoryData GitRepositoryData { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      base.SetSecuredObject(securedObject);
      this.GitRepositoryData?.SetSecuredObject(securedObject);
      this.ReadmeFileItemModel?.SetSecuredObject(securedObject);
    }
  }
}
