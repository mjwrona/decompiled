// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.PublishedExtension
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  public class PublishedExtension
  {
    public PublisherFacts Publisher { get; set; }

    public Guid ExtensionId { get; set; }

    public string ExtensionName { get; set; }

    public string DisplayName { get; set; }

    public PublishedExtensionFlags Flags { get; set; }

    public DateTime LastUpdated { get; set; }

    public DateTime PublishedDate { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public DateTime ReleaseDate { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string PresentInConflictList { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string ShortDescription { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string LongDescription { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<ExtensionVersion> Versions { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string> Categories { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<string> Tags { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<ExtensionShare> SharedWith { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<ExtensionStatistic> Statistics { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<InstallationTarget> InstallationTargets { get; set; }

    internal List<ExtensionMetadata> Metadata { get; set; }

    internal List<int> Lcids { get; set; }

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public ExtensionDeploymentTechnology DeploymentType { get; set; }

    public string GetProperty(string version, string propertyName)
    {
      if (!string.IsNullOrEmpty(version) && this.Versions != null && this.Versions.Count > 0)
      {
        ExtensionVersion extensionVersion = (ExtensionVersion) null;
        if (version.Equals("latest"))
        {
          extensionVersion = this.Versions[0];
        }
        else
        {
          foreach (ExtensionVersion version1 in this.Versions)
          {
            if (version1.Version.Equals(version))
            {
              extensionVersion = version1;
              break;
            }
          }
        }
        if (extensionVersion != null && extensionVersion.Properties != null)
        {
          foreach (KeyValuePair<string, string> property in extensionVersion.Properties)
          {
            if (property.Key.Equals(propertyName))
              return property.Value;
          }
        }
      }
      return (string) null;
    }

    internal PublishedExtension ShallowCopy() => (PublishedExtension) this.MemberwiseClone();
  }
}
