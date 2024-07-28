// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.FileContainer.FileContainer
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.FileContainer
{
  [DataContract]
  public class FileContainer
  {
    [DataMember(IsRequired = true)]
    public long Id { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false)]
    public Guid ScopeIdentifier { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = true)]
    public Uri ArtifactUri { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string SecurityToken { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Name { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Description { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public long Size { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ContainerOptions Options { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid SigningKeyId { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid CreatedBy { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime DateCreated { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ItemLocation { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ContentLocation { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string LocatorPath { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    public override bool Equals(object obj) => obj is Microsoft.VisualStudio.Services.FileContainer.FileContainer fileContainer && this.ArtifactUri == fileContainer.ArtifactUri && this.Description == fileContainer.Description && this.Id == fileContainer.Id && this.Name == fileContainer.Name && this.ScopeIdentifier == fileContainer.ScopeIdentifier;

    public override int GetHashCode() => this.Id.GetHashCode();
  }
}
