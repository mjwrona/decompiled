// Decompiled with JetBrains decompiler
// Type: Galleries.Domain.Model.ReleaseFile
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Galleries.Domain.Model
{
  [DataContract]
  public class ReleaseFile
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public ReleaseFileType Type { get; set; }

    [DataMember]
    public FileAttachment Attachment { get; set; }

    [DataMember]
    public int DownloadCount { get; set; }

    [DataMember]
    public IDictionary<string, string> Metadata { get; set; }

    [DataMember]
    public Release Parent { get; set; }
  }
}
