// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.Team
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Agile.Server
{
  [DataContract]
  public class Team
  {
    public Team()
    {
    }

    public Team(Guid id, string name)
    {
      this.Id = id;
      this.Name = name;
    }

    public Team(WebApiTeam tfsTeam)
    {
      ArgumentUtility.CheckForNull<WebApiTeam>(tfsTeam, nameof (tfsTeam));
      this.Id = tfsTeam.Id;
      this.Name = tfsTeam.Name;
    }

    [DataMember(Name = "id")]
    public Guid Id { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["id"] = (object) this.Id;
      json["name"] = (object) this.Name;
      return json;
    }
  }
}
