// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.WebApiProject
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [DataContract]
  public class WebApiProject : TeamProjectReference
  {
    public WebApiProject()
    {
    }

    public WebApiProject(TeamProjectReference projRef)
    {
      this.Id = projRef.Id;
      this.Name = projRef.Name;
      this.Url = projRef.Url;
      this.State = projRef.State;
      this.Description = projRef.Description;
      this.Revision = projRef.Revision;
      this.Visibility = projRef.Visibility;
    }

    [DataMember(Order = 5)]
    public WebApiProjectCollectionRef Collection { get; set; }

    [DataMember(Order = 6)]
    public WebApiTeamRef DefaultTeam { get; set; }

    [DataMember(Order = 7, EmitDefaultValue = false)]
    public Dictionary<string, Dictionary<string, string>> Capabilities { get; set; }

    public string TfsUri { get; set; }
  }
}
