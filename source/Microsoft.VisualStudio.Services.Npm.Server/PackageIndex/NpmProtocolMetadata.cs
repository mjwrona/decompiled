// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.PackageIndex.NpmProtocolMetadata
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Npm.Server.PackageIndex
{
  [DataContract]
  public class NpmProtocolMetadata
  {
    [DataMember(EmitDefaultValue = false, Name = "author")]
    public Person Author { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "homepage")]
    public string Homepage { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "contributors")]
    public Person[] Contributors { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "repository")]
    public Repository Repository { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "bugs")]
    public BugTracker Bugs { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "license")]
    public string License { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "scripts")]
    public Dictionary<string, string> Scripts { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "directories")]
    public Directories Directories { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "engines")]
    public JRaw Engines { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "files")]
    public List<string> Files { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "binaries")]
    public Dictionary<string, string> Binaries { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "manualPages")]
    public JRaw ManualPages { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "operatingSystem")]
    public List<string> OperatingSystem { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "processorArchitecture")]
    public List<string> ProcessorArchitecture { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "main")]
    public string Main { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "engineStrict")]
    public JRaw EngineStrict { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "config")]
    public Dictionary<string, string> Config { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "publishConfig")]
    public Dictionary<string, string> PublishConfig { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "preferGlobal")]
    public bool PreferGlobal { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "privatePackage")]
    public bool PrivatePackage { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "deprecated")]
    public string Deprecated { get; set; }
  }
}
