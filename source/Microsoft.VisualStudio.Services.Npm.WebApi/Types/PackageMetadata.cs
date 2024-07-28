// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Types.PackageMetadata
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Microsoft.VisualStudio.Services.Npm.WebApi.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Types
{
  [DataContract]
  public class PackageMetadata
  {
    [DataMember(Name = "_id", IsRequired = true)]
    public string Id { get; set; }

    [DataMember(Name = "_rev")]
    public string Revision { get; set; }

    [DataMember(Name = "name", IsRequired = true)]
    public string Name { get; set; }

    [DataMember(Name = "dist-tags", EmitDefaultValue = false)]
    public IDictionary<string, string> DistributionTags { get; set; }

    [DataMember(Name = "_attachments", EmitDefaultValue = false)]
    public Dictionary<string, Attachment> Attachments { get; set; }

    [DataMember(Name = "isUpstreamCached", EmitDefaultValue = false)]
    public bool IsUpstreamCached { get; set; }

    [DataMember(Name = "versions", EmitDefaultValue = false)]
    public IDictionary<string, VersionMetadata> Versions { get; set; }

    [DataMember(Name = "time", EmitDefaultValue = false)]
    public IDictionary<string, object> Time { get; set; }

    [DataMember(Name = "readme", EmitDefaultValue = false)]
    public string Readme { get; set; }

    [DataMember(Name = "readmeFilename", EmitDefaultValue = false)]
    public string ReadmeFileName { get; set; }

    [DataMember(Name = "maintainers", EmitDefaultValue = false)]
    [JsonConverter(typeof (ArrayOrSingleItemConverter<Person>))]
    public Person[] Maintainers { get; set; }

    [DataMember(Name = "description", EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(Name = "homepage", EmitDefaultValue = false)]
    [JsonConverter(typeof (ForceStringConverter))]
    public string Homepage { get; set; }

    [DataMember(Name = "keywords", EmitDefaultValue = false)]
    [JsonConverter(typeof (LegacyKeywordJsonConverter))]
    public IEnumerable<string> Keywords { get; set; }

    [DataMember(Name = "repository", EmitDefaultValue = false)]
    [JsonConverter(typeof (RepositoryJsonConverter))]
    public Repository Repository { get; set; }

    [DataMember(Name = "contributors", EmitDefaultValue = false)]
    [JsonConverter(typeof (ArrayOrSingleItemConverter<Person>))]
    public Person[] Contributors { get; set; }

    [DataMember(Name = "author", EmitDefaultValue = false)]
    public Person Author { get; set; }

    [DataMember(Name = "bugs", EmitDefaultValue = false)]
    [JsonConverter(typeof (BugTrackerJsonConverter))]
    public BugTracker Bugs { get; set; }

    [DataMember(Name = "license", EmitDefaultValue = false)]
    [JsonConverter(typeof (LicenseJsonConverter))]
    public string License { get; set; }
  }
}
