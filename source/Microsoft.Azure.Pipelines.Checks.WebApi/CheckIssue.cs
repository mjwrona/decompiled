// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.WebApi.CheckIssue
// Assembly: Microsoft.Azure.Pipelines.Checks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 381241F9-9196-42AF-BB4C-5187E3EFE32E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Checks.WebApi
{
  [DataContract]
  public class CheckIssue
  {
    public CheckIssue()
    {
    }

    private CheckIssue(CheckIssue issueToBeCloned)
    {
      this.Description = issueToBeCloned.Description;
      this.Type = issueToBeCloned.Type;
      this.Message = issueToBeCloned.Message;
    }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public CheckIssueType Type { get; set; }

    [DataMember]
    public string Message { get; set; }

    [DataMember]
    public string DetailedMessage { get; set; }

    public CheckIssue Clone() => new CheckIssue(this);
  }
}
