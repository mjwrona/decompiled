// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.CustomArtifactTriggerEvent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class CustomArtifactTriggerEvent
  {
    public CustomArtifactTriggerEvent() => this.SourceData = new SerializableDictionary<string, string>();

    public string Definition { get; set; }

    public string Version { get; set; }

    public string ArtifactType { get; set; }

    public string UniqueSourceIdentifierTemplate { get; set; }

    [XmlArray("UniqueSourceIdentifierArray")]
    [XmlArrayItem("UniqueSourceIdentifier")]
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Required for serialization")]
    public List<string> UniqueSourceIdentifiers { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    public SerializableDictionary<string, string> SourceData { get; set; }
  }
}
