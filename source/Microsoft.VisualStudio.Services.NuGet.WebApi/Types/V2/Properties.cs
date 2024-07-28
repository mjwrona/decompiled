// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.Types.V2.Properties
// Assembly: Microsoft.VisualStudio.Services.NuGet.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D44F181-506D-4445-A06B-7AA7FD5D22D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.WebApi.dll

using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.NuGet.WebApi.Types.V2
{
  public class Properties
  {
    [XmlElement(Namespace = "http://schemas.microsoft.com/ado/2007/08/dataservices")]
    public string Id { get; set; }

    [XmlElement(Namespace = "http://schemas.microsoft.com/ado/2007/08/dataservices")]
    public string Version { get; set; }

    [XmlElement(Namespace = "http://schemas.microsoft.com/ado/2007/08/dataservices")]
    public string NormalizedVersion { get; set; }

    [XmlElement(Namespace = "http://schemas.microsoft.com/ado/2007/08/dataservices")]
    public string Authors { get; set; }

    [XmlElement(Namespace = "http://schemas.microsoft.com/ado/2007/08/dataservices")]
    public string Copyright { get; set; }

    [XmlElement]
    public DateTime Created { get; set; }

    [XmlElement]
    public string Dependencies { get; set; }

    [XmlElement]
    public string Description { get; set; }

    [XmlElement]
    public int DownloadCount { get; set; }

    [XmlElement]
    public string IconUrl { get; set; }

    [XmlElement]
    public bool IsLatestVersion { get; set; }

    [XmlElement]
    public bool IsAbsoluteLatestVersion { get; set; }

    [XmlElement]
    public bool IsPrerelease { get; set; }

    [XmlElement]
    public string Language { get; set; }

    [XmlElement]
    public DateTime LastUpdated { get; set; }

    [XmlElement]
    public DateTime Published { get; set; }

    [XmlElement]
    public long PackageSize { get; set; }

    [XmlElement]
    public string ProjectUrl { get; set; }

    [XmlElement]
    public string ProjectHash { get; set; }

    [XmlElement]
    public string PackageHashAlgorithm { get; set; }

    [XmlElement]
    public string ReleaseNotes { get; set; }

    [XmlElement]
    public bool RequireLicenseAcceptance { get; set; }

    [XmlElement]
    public string Summary { get; set; }

    [XmlElement]
    public string Tags { get; set; }

    [XmlElement]
    public string Title { get; set; }

    [XmlElement]
    public string MinClientVersion { get; set; }

    [XmlElement]
    public DateTime? LastEdited { get; set; }

    [XmlElement]
    public string LicenseUrl { get; set; }

    [XmlIgnore]
    [Obsolete("Use IsListed instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Listed { get; set; }

    [XmlElement("Listed")]
    public bool? ListedElement { get; set; }

    [XmlIgnore]
    public bool IsListed => this.ListedElement ?? this.Published > new DateTime(1990, 1, 1);
  }
}
