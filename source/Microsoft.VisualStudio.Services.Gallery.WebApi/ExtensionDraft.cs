// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionDraft
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  public class ExtensionDraft
  {
    public Guid Id { get; set; }

    public DateTime LastUpdated { get; set; }

    public DateTime CreatedDate { get; set; }

    public string PublisherName { get; set; }

    public string Product { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string ExtensionName { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public ExtensionPayload Payload { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<ExtensionDraftAsset> Assets { get; set; }

    internal Guid ExtensionId { get; set; }

    internal Guid UserId { get; set; }

    public DraftStateType DraftState { get; set; }

    internal DateTime EditReferenceDate { get; set; }

    public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

    public List<KeyValuePair<string, string>> ValidationWarnings { get; set; }
  }
}
