// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.Types.TeamContext
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Core.WebApi.Types
{
  [DataContract]
  public class TeamContext
  {
    public TeamContext(Guid projectId, Guid? teamId = null)
    {
      this.ProjectId = new Guid?(projectId);
      this.TeamId = teamId;
    }

    public TeamContext(string project, string team = null)
    {
      this.Project = project;
      this.Team = team;
    }

    [DataMember]
    public Guid? ProjectId { get; set; }

    [DataMember]
    public string Project { get; set; }

    [DataMember]
    public Guid? TeamId { get; set; }

    [DataMember]
    public string Team { get; set; }
  }
}
