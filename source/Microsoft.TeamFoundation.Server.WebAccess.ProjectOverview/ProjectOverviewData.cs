// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview.ProjectOverviewData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97A9928B-E499-4978-909F-1EBC8C5535AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.ProjectOverview
{
  [DataContract]
  public class ProjectOverviewData : AbstractProjectSecuredObject
  {
    [DataMember]
    public ProjectAboutData Info { get; set; }

    [DataMember]
    public bool IsProjectEmpty { get; set; }

    [DataMember]
    public bool HasCode { get; set; }

    [DataMember]
    public bool HasBuildConfigured { get; set; }

    [DataMember]
    public bool SupportsGit { get; set; }

    [DataMember]
    public bool SupportsTFVC { get; set; }

    [DataMember]
    public RepositoryData CurrentRepositoryData { get; set; }

    [DataMember]
    public bool IsRMFaultedIn { get; set; }

    [DataMember]
    public CurrentUserData CurrentUser { get; set; }

    [DataMember]
    public int? CodeMetricsAvailableForDays { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      ArgumentUtility.CheckForNull<ProjectAboutData>(this.Info, "Info");
      ArgumentUtility.CheckForNull<RepositoryData>(this.CurrentRepositoryData, "CurrentRepositoryData");
      ArgumentUtility.CheckForNull<CurrentUserData>(this.CurrentUser, "CurrentUser");
      base.SetSecuredObject(securedObject);
      this.Info.SetSecuredObject(securedObject);
      this.CurrentRepositoryData.SetSecuredObject(securedObject);
      this.CurrentUser.SetSecuredObject(securedObject);
    }
  }
}
