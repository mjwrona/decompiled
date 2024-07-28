// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestAttachmentModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public class TestAttachmentModel
  {
    public TestAttachmentModel(TestResultAttachment attachment)
    {
      this.ActionPath = attachment.ActionPath;
      this.FileName = attachment.FileName;
      this.Id = attachment.Id;
      this.IterationId = attachment.IterationId;
      this.Size = attachment.Length;
      this.TestRunId = attachment.TestRunId;
      this.TestResultId = attachment.TestResultId;
      this.Comment = attachment.Comment;
    }

    public TestAttachmentModel(
      int testRunId,
      int testResultId,
      TestCaseResultAttachmentModel attachment)
    {
      this.ActionPath = attachment.ActionPath;
      this.FileName = attachment.Name;
      this.Id = attachment.Id;
      this.IterationId = attachment.IterationId;
      this.Size = attachment.Size;
      this.TestRunId = testRunId;
      this.TestResultId = testResultId;
    }

    public TestAttachmentModel()
    {
    }

    [DataMember(Name = "actionPath")]
    public string ActionPath { get; set; }

    [DataMember(Name = "fileName")]
    public string FileName { get; set; }

    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "size")]
    public long Size { get; set; }

    [DataMember(Name = "iterationId")]
    public int IterationId { get; set; }

    [DataMember(Name = "testResultId")]
    public int TestResultId { get; set; }

    [DataMember(Name = "testRunId")]
    public int TestRunId { get; set; }

    [DataMember(Name = "comment")]
    public string Comment { get; set; }
  }
}
