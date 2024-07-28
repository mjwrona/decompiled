// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Types.PackageJson
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Npm.WebApi.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Types
{
  [DataContract]
  public class PackageJson
  {
    public PackageJson()
    {
    }

    protected PackageJson(PackageJson packageJson)
    {
      ArgumentUtility.CheckForNull<PackageJson>(packageJson, nameof (packageJson));
      this.Name = packageJson.Name;
      this.Description = packageJson.Description;
      this.Version = packageJson.Version;
      this.Keywords = packageJson.Keywords;
      this.Homepage = packageJson.Homepage;
      this.Author = packageJson.Author;
      this.Contributors = packageJson.Contributors;
      this.Repository = packageJson.Repository;
      this.Bugs = packageJson.Bugs;
      this.License = packageJson.License;
      this.Scripts = packageJson.Scripts;
      this.Directories = packageJson.Directories;
      this.DevDependencies = packageJson.DevDependencies;
      this.PeerDependencies = packageJson.PeerDependencies;
      this.Dependencies = packageJson.Dependencies;
      this.OptionalDependencies = packageJson.OptionalDependencies;
      this.BundleDependencies = packageJson.BundleDependencies;
      this.BundledDependencies = packageJson.BundledDependencies;
      this.Engines = packageJson.Engines;
      this.Files = packageJson.Files;
      this.Binaries = packageJson.Binaries;
      this.ManualPages = packageJson.ManualPages;
      this.OperatingSystem = packageJson.OperatingSystem;
      this.ProcessorArchitecture = packageJson.ProcessorArchitecture;
      this.Main = packageJson.Main;
      this.EngineStrict = packageJson.EngineStrict;
      this.Config = packageJson.Config;
      this.PublishConfig = packageJson.PublishConfig;
      this.PreferGlobal = packageJson.PreferGlobal;
      this.PrivatePackage = packageJson.PrivatePackage;
      this.Deprecated = packageJson.Deprecated;
      this.AdditionalData = packageJson.AdditionalData;
    }

    [DataMember(Name = "name", IsRequired = true)]
    public string Name { get; set; }

    [DataMember(Name = "description", EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(Name = "version", IsRequired = true)]
    public string Version { get; set; }

    [DataMember(Name = "keywords", EmitDefaultValue = false)]
    [JsonConverter(typeof (LegacyKeywordJsonConverter))]
    public string[] Keywords { get; set; }

    [DataMember(Name = "homepage", EmitDefaultValue = false)]
    [JsonConverter(typeof (ForceStringConverter))]
    public string Homepage { get; set; }

    [DataMember(Name = "author", EmitDefaultValue = false)]
    public Person Author { get; set; }

    [DataMember(Name = "contributors", EmitDefaultValue = false)]
    [JsonConverter(typeof (ArrayOrSingleItemConverter<Person>))]
    public Person[] Contributors { get; set; }

    [DataMember(Name = "repository", EmitDefaultValue = false)]
    [JsonConverter(typeof (RepositoryJsonConverter))]
    public Repository Repository { get; set; }

    [DataMember(Name = "bugs", EmitDefaultValue = false)]
    [JsonConverter(typeof (BugTrackerJsonConverter))]
    public BugTracker Bugs { get; set; }

    [DataMember(Name = "license", EmitDefaultValue = false)]
    [JsonConverter(typeof (LicenseJsonConverter))]
    public string License { get; set; }

    [DataMember(Name = "scripts", EmitDefaultValue = false)]
    [JsonConverter(typeof (KeyValueJsonConverter))]
    public Dictionary<string, string> Scripts { get; set; }

    [DataMember(Name = "directories", EmitDefaultValue = true)]
    public Directories Directories { get; set; }

    [DataMember(Name = "devDependencies", EmitDefaultValue = false)]
    [JsonConverter(typeof (DependencyConverter))]
    public Dictionary<string, string> DevDependencies { get; set; }

    [DataMember(Name = "peerDependencies", EmitDefaultValue = false)]
    [JsonConverter(typeof (DependencyConverter))]
    public Dictionary<string, string> PeerDependencies { get; set; }

    [DataMember(Name = "dependencies", EmitDefaultValue = false)]
    [JsonConverter(typeof (DependencyConverter))]
    public Dictionary<string, string> Dependencies { get; set; }

    [DataMember(Name = "optionalDependencies", EmitDefaultValue = false)]
    [JsonConverter(typeof (DependencyConverter))]
    public Dictionary<string, string> OptionalDependencies { get; set; }

    [DataMember(Name = "bundleDependencies", EmitDefaultValue = false)]
    public BundledDependencies BundleDependencies { get; set; }

    [DataMember(Name = "bundledDependencies", EmitDefaultValue = false)]
    public BundledDependencies BundledDependencies { get; set; }

    [DataMember(Name = "engines", EmitDefaultValue = false)]
    public JRaw Engines { get; set; }

    [DataMember(Name = "files", EmitDefaultValue = false)]
    public List<string> Files { get; set; }

    [DataMember(Name = "bin", EmitDefaultValue = false)]
    [JsonConverter(typeof (BinaryJsonConverter))]
    public Dictionary<string, string> Binaries { get; set; }

    [DataMember(Name = "man", EmitDefaultValue = false)]
    public JRaw ManualPages { get; set; }

    [DataMember(Name = "os", EmitDefaultValue = false)]
    public List<string> OperatingSystem { get; set; }

    [DataMember(Name = "cpu", EmitDefaultValue = false)]
    public List<string> ProcessorArchitecture { get; set; }

    [DataMember(Name = "main", EmitDefaultValue = false)]
    [JsonConverter(typeof (ForceStringConverter))]
    public string Main { get; set; }

    [DataMember(Name = "engineStrict", EmitDefaultValue = false)]
    public JRaw EngineStrict { get; set; }

    [DataMember(Name = "config", EmitDefaultValue = false)]
    [JsonConverter(typeof (KeyValueJsonConverter))]
    public Dictionary<string, string> Config { get; set; }

    [DataMember(Name = "publishConfig", EmitDefaultValue = false)]
    [JsonConverter(typeof (KeyValueJsonConverter))]
    public Dictionary<string, string> PublishConfig { get; set; }

    [DataMember(Name = "preferGlobal", EmitDefaultValue = false)]
    public bool PreferGlobal { get; set; }

    [DataMember(Name = "private", EmitDefaultValue = false)]
    public bool PrivatePackage { get; set; }

    [DataMember(Name = "exports", EmitDefaultValue = false)]
    [Obsolete("This member name is reserved for future use.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public JToken Exports { get; set; }

    [JsonExtensionData]
    public IDictionary<string, JToken> AdditionalData { get; set; }

    [DataMember(Name = "deprecated", EmitDefaultValue = false)]
    public string Deprecated { get; set; }

    [DataMember(Name = "dist", EmitDefaultValue = false)]
    public Distribution Distribution { get; set; }
  }
}
