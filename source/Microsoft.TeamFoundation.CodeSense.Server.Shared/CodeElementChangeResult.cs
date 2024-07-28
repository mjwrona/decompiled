// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.CodeElementChangeResult
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public class CodeElementChangeResult
  {
    public const string ChangesCollectorId = "Microsoft.Changes";
    public const string ChangesCollectorVersion = "1.0";

    [JsonConstructor]
    public CodeElementChangeResult()
    {
    }

    public CodeElementChangeResult(
      ElementChangeKind kind,
      string changesId,
      string changesComment,
      DateTime createdDate,
      IEnumerable<WorkItemData> workItems,
      UserData author)
    {
      this.ChangeKind = kind;
      this.ChangesId = changesId;
      this.ChangesComment = changesComment;
      this.Date = createdDate;
      this.WorkItems = workItems == null || !workItems.Any<WorkItemData>() ? (IEnumerable<WorkItemData>) null : workItems;
      if (author != null)
      {
        this.AuthorDisplayName = author.DisplayName;
        this.AuthorUniqueName = author.UniqueName;
        this.AuthorEmail = author.Email;
      }
      else
        TracingExtensions.TraceRaw(1024601, TraceLevel.Warning, TraceLayer.Job, "Changes {0} does not have an author", (object) this.ChangesId);
    }

    [JsonProperty]
    public ElementChangeKind ChangeKind { get; private set; }

    [JsonProperty]
    public string AuthorDisplayName { get; private set; }

    [JsonProperty]
    public string AuthorUniqueName { get; private set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string AuthorEmail { get; private set; }

    [JsonProperty]
    public string ChangesId { get; private set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string ChangesComment { get; private set; }

    [JsonProperty]
    public DateTime Date { get; private set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<WorkItemData> WorkItems { get; private set; }
  }
}
