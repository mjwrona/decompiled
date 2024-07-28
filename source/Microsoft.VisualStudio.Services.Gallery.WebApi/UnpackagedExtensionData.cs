// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.UnpackagedExtensionData
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  public class UnpackagedExtensionData
  {
    public Guid DraftId { get; set; }

    public string PublisherName { get; set; }

    public string Product { get; set; }

    public string ExtensionName { get; set; }

    public string Version { get; set; }

    public string DisplayName { get; set; }

    public string Description { get; set; }

    public List<string> Tags { get; set; }

    public List<string> Categories { get; set; }

    public List<InstallationTarget> InstallationTargets { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string ReferralUrl { get; set; }

    public string PricingCategory { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string VsixId { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string RepositoryUrl { get; set; }

    public bool QnAEnabled { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool IsConvertedToMarkdown { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public bool IsPreview { get; set; }

    internal List<KeyValuePair<string, string>> Metadata { get; set; }

    internal List<int> Lcids { get; set; }

    internal PublishedExtensionFlags Flags { get; set; }
  }
}
