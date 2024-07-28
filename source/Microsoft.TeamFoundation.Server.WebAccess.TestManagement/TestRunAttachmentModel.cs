// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestRunAttachmentModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  internal class TestRunAttachmentModel
  {
    public TestRunAttachmentModel(
      string Name,
      long Size,
      DateTime date,
      string Comment,
      int Id,
      string attachmentType)
    {
      this.AttachmentName = Name;
      this.AttachmentSize = Size;
      this.AttachmentComment = Comment;
      this.AttachmentId = Id;
      this.AttachmentCreationDate = date;
      this.AttachmentType = attachmentType;
    }

    [DataMember(Name = "attachmentType")]
    public string AttachmentType { get; set; }

    [DataMember(Name = "attachmentName")]
    public string AttachmentName { get; set; }

    [DataMember(Name = "attachmentSize")]
    public long AttachmentSize { get; set; }

    [DataMember(Name = "attachmentComment")]
    public string AttachmentComment { get; set; }

    [DataMember(Name = "attachmentCreationDate")]
    public DateTime AttachmentCreationDate { get; set; }

    [DataMember(Name = "attachmentId")]
    public int AttachmentId { get; set; }
  }
}
