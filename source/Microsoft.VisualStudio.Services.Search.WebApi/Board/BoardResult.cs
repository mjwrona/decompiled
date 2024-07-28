// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Board.BoardResult
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9E496DAE-109A-4A16-A97B-F7DEDEC6CB20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Board
{
  [DataContract]
  public class BoardResult : SearchSecuredV2Object
  {
    public BoardResult(Team team, Project project, Collection collection, string boardType)
    {
      this.Collection = collection;
      this.Project = project;
      this.Team = team;
      this.BoardType = boardType;
    }

    [DataMember(Name = "collection")]
    public Collection Collection { get; set; }

    [DataMember(Name = "team")]
    public Team Team { get; set; }

    [DataMember(Name = "boardtype")]
    public string BoardType { get; set; }

    [DataMember(Name = "project")]
    public Project Project { get; set; }

    public override string ToString() => JsonConvert.SerializeObject((object) this, Formatting.None, new JsonSerializerSettings()
    {
      NullValueHandling = NullValueHandling.Ignore
    });
  }
}
