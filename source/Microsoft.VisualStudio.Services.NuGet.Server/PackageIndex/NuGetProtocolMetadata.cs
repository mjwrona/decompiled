// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageIndex.NuGetProtocolMetadata
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageIndex
{
  [DataContract]
  public class NuGetProtocolMetadata
  {
    [DataMember(EmitDefaultValue = false, Name = "copyright")]
    public string Copyright { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "iconUrl")]
    public string IconUrl { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "language")]
    public string Language { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "licenseUrl")]
    public string LicenseUrl { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "minClientVersion")]
    public string MinClientVersion { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "projectUrl")]
    public string ProjectUrl { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "releaseNotes")]
    public string ReleaseNotes { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "requireLicenseAcceptance")]
    public bool? RequireLicenseAcceptance { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "title")]
    public string Title { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "iconFile")]
    public string IconFile { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "licenseExpression")]
    public string LicenseExpression { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "licenseFile")]
    public string LicenseFile { get; set; }
  }
}
