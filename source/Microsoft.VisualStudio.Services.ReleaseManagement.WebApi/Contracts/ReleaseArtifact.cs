// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseArtifact
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class ReleaseArtifact
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public int ReleaseId { get; set; }

    [DataMember]
    public string DefinitionData { get; set; }

    [DataMember]
    public string ArtifactType { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public int DefinitionId { get; set; }

    [DataMember]
    public ArtifactProvider ArtifactProvider { get; set; }
  }
}
