// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Types.Directories
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Types
{
  [DataContract]
  public class Directories
  {
    [DataMember(Name = "lib", EmitDefaultValue = false)]
    public string LibraryFolder { get; set; }

    [DataMember(Name = "bin", EmitDefaultValue = false)]
    public string BinariesFolder { get; set; }

    [DataMember(Name = "man", EmitDefaultValue = false)]
    public string ManualPagesFolder { get; set; }

    [DataMember(Name = "doc", EmitDefaultValue = false)]
    public string DocumentationFolder { get; set; }

    [DataMember(Name = "examples", EmitDefaultValue = false)]
    public string ExamplesFolder { get; set; }
  }
}
