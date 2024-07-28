// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestAttachmentRequestModel
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class TestAttachmentRequestModel
  {
    public TestAttachmentRequestModel()
    {
    }

    public TestAttachmentRequestModel(
      string stream = null,
      string fileName = "",
      string comment = "",
      string attachmentType = "")
    {
      this.Stream = stream;
      this.FileName = fileName;
      this.Comment = comment;
      this.AttachmentType = attachmentType;
    }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Stream { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string FileName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Comment { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AttachmentType { get; set; }
  }
}
