// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.PublisherBase
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  public class PublisherBase
  {
    public Guid PublisherId { get; set; }

    public string PublisherName { get; set; }

    public string DisplayName { get; set; }

    public PublisherFlags Flags { get; set; }

    public DateTime LastUpdated { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string ShortDescription { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string LongDescription { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<PublishedExtension> Extensions { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string> EmailAddress { get; set; }

    [DefaultValue(PublisherState.None)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public PublisherState State { get; set; }
  }
}
