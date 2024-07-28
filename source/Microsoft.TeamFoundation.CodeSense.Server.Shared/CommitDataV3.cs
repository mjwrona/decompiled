// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.CommitDataV3
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public class CommitDataV3
  {
    private IEnumerable<int> _workItems;

    public CommitDataV3()
    {
    }

    public CommitDataV3(
      string changesId,
      string changesComment,
      string authorUniqueName,
      DateTime date,
      IEnumerable<int> workItems)
    {
      this.ChangesId = changesId;
      this.ChangesComment = changesComment;
      this.AuthorUniqueName = authorUniqueName;
      this.Date = date;
      this.WorkItems = workItems;
    }

    [JsonProperty]
    public string ChangesId { get; private set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string ChangesComment { get; set; }

    [JsonProperty]
    public DateTime Date { get; private set; }

    [JsonProperty]
    public string AuthorUniqueName { get; private set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<int> WorkItems
    {
      get => this._workItems == null || !this._workItems.Any<int>() ? (IEnumerable<int>) null : this._workItems;
      private set => this._workItems = value;
    }
  }
}
