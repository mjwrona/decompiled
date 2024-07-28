// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.TeamProject
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [DataContract]
  public class TeamProject : TeamProjectReference
  {
    public TeamProject()
    {
    }

    public TeamProject(TeamProjectReference projectReference)
    {
      this.Id = projectReference.Id;
      this.Name = projectReference.Name;
      this.Abbreviation = projectReference.Abbreviation;
      this.Url = projectReference.Url;
      this.State = projectReference.State;
      this.Description = projectReference.Description;
      this.Revision = projectReference.Revision;
      this.Links = new ReferenceLinks();
      this.Links.AddLink("self", this.Url, (ISecuredObject) this);
      this.Visibility = projectReference.Visibility;
      this.LastUpdateTime = projectReference.LastUpdateTime;
    }

    [DataMember(Order = 6, EmitDefaultValue = false)]
    public Dictionary<string, Dictionary<string, string>> Capabilities { get; set; }

    [DataMember(Name = "_links", Order = 7, EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    [DataMember(Order = 8, EmitDefaultValue = false)]
    public WebApiTeamRef DefaultTeam { get; set; }

    public string TfsUri { get; set; }
  }
}
