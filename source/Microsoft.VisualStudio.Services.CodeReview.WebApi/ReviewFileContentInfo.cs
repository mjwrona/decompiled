// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.ReviewFileContentInfo
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  [DataContract]
  [KnownType(typeof (ChangeEntryFileInfo))]
  public class ReviewFileContentInfo
  {
    [DataMember]
    internal int ReviewId { get; set; }

    internal Guid ContentId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Path { get; set; }

    internal int FileServiceFileId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string SHA1Hash { get; set; }

    internal bool NeedsCleanup { get; set; }

    [DataMember]
    public byte Flags { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    public bool FileUploadComplete => this.FileServiceFileId > 0;
  }
}
