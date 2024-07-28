// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Board.BoardSearchResponse
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9E496DAE-109A-4A16-A97B-F7DEDEC6CB20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Board
{
  [DataContract]
  public class BoardSearchResponse : EntitySearchResponse
  {
    [DataMember(Name = "count")]
    public int Count { get; set; }

    [DataMember(Name = "results")]
    public IEnumerable<BoardResult> Results { get; set; }

    public override string ToString()
    {
      int count = this.Count;
      IEnumerable<BoardResult> results = this.Results;
      IEnumerable<\u003C\u003Ef__AnonymousType2<Collection, Project, Team, string>> datas = results != null ? results.Select(v => new
      {
        collection = v.Collection,
        project = v.Project,
        teamName = v.Team,
        boardType = v.BoardType
      }) : null;
      return JsonConvert.SerializeObject((object) new
      {
        results = new{ count = count, results = datas },
        infoCode = this.InfoCode
      }, Formatting.None, new JsonSerializerSettings()
      {
        NullValueHandling = NullValueHandling.Ignore
      });
    }
  }
}
