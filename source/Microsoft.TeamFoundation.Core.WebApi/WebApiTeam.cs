// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.WebApiTeam
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [DataContract]
  public class WebApiTeam : WebApiTeamRef
  {
    public WebApiTeam()
    {
    }

    public WebApiTeam(WebApiTeamRef teamRef)
      : base((ISecuredObject) teamRef)
    {
      this.Id = teamRef.Id;
      this.Name = teamRef.Name;
      this.Url = teamRef.Url;
    }

    public WebApiTeam(WebApiTeam copy, ISecuredObject securedObject)
      : base((WebApiTeamRef) copy, securedObject)
    {
      this.Description = copy.Description;
      this.IdentityUrl = copy.IdentityUrl;
      this.ProjectName = copy.ProjectName;
      this.ProjectId = copy.ProjectId;
      this.Identity = copy.Identity;
    }

    [DataMember(Order = 4)]
    public string Description { get; set; }

    [DataMember(Order = 5)]
    public string IdentityUrl { get; set; }

    [DataMember(Order = 6)]
    public string ProjectName { get; set; }

    [DataMember(Order = 7)]
    public Guid ProjectId { get; set; }

    [DataMember(Name = "identity", EmitDefaultValue = false, Order = 8)]
    public Microsoft.VisualStudio.Services.Identity.Identity Identity { get; set; }
  }
}
