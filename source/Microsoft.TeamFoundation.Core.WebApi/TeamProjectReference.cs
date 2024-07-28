// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.TeamProjectReference
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [DataContract]
  public class TeamProjectReference : ISecuredObject
  {
    public TeamProjectReference()
    {
      this.State = ProjectState.Unchanged;
      this.Visibility = ProjectVisibility.Unchanged;
    }

    [DataMember(Order = 0, EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(Order = 1, EmitDefaultValue = false)]
    public string Abbreviation { get; set; }

    [DataMember(Order = 2, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(Order = 3, EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(Order = 4, EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(Order = 5)]
    public ProjectState State { get; set; }

    [DataMember(Order = 6, EmitDefaultValue = false)]
    public long Revision { get; set; }

    [DataMember(Order = 7)]
    public ProjectVisibility Visibility { get; set; }

    [DataMember(Order = 8, EmitDefaultValue = false)]
    public string DefaultTeamImageUrl { get; set; }

    [DataMember(Order = 9)]
    public DateTime LastUpdateTime { get; set; }

    Guid ISecuredObject.NamespaceId => this.NamespaceId;

    int ISecuredObject.RequiredPermissions => this.RequiredPermissions;

    string ISecuredObject.GetToken() => this.GetToken();

    protected virtual Guid NamespaceId => TeamProjectSecurityConstants.NamespaceId;

    protected virtual int RequiredPermissions => TeamProjectSecurityConstants.GenericRead;

    protected virtual string GetToken() => TeamProjectSecurityConstants.GetToken(ProjectInfo.GetProjectUri(this.Id));
  }
}
