// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.FileContainer.FileContainerItem
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.FileContainer
{
  [DataContract]
  public class FileContainerItem
  {
    private string m_path;

    [DataMember(IsRequired = true)]
    public long ContainerId { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false)]
    public Guid ScopeIdentifier { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = true)]
    public string Path
    {
      get => this.m_path;
      [EditorBrowsable(EditorBrowsableState.Never)] set => this.m_path = FileContainerItem.EnsurePathFormat(value);
    }

    [DataMember(IsRequired = true)]
    public ContainerItemType ItemType { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = true)]
    public ContainerItemStatus Status { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public long FileLength { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public byte[] FileHash { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int FileEncoding { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int FileType { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime DateCreated { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime DateLastModified { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid CreatedBy { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid LastModifiedBy { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ItemLocation { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ContentLocation { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public int FileId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public long? ArtifactId { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public byte[] ContentId { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Ticket { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ContainerItemBlobReference BlobMetadata { get; set; }

    public static string EnsurePathFormat(string path)
    {
      if (string.IsNullOrWhiteSpace(path))
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      string[] strArray = path.Split(new char[2]
      {
        '\\',
        '/'
      }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length == 0)
        return string.Empty;
      for (int index = 0; index < strArray.Length; ++index)
        stringBuilder.AppendFormat("{0}{1}", (object) strArray[index], index == strArray.Length - 1 ? (object) string.Empty : (object) "/");
      return stringBuilder.ToString();
    }

    public override bool Equals(object obj) => obj is FileContainerItem fileContainerItem && this.ContainerId == fileContainerItem.ContainerId && this.ScopeIdentifier == fileContainerItem.ScopeIdentifier && this.Path == fileContainerItem.Path && this.ItemType == fileContainerItem.ItemType;

    public override int GetHashCode() => this.Path.GetHashCode();
  }
}
