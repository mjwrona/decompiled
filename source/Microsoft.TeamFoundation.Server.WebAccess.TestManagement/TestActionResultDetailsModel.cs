// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestActionResultDetailsModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public class TestActionResultDetailsModel
  {
    public TestActionResultDetailsModel(
      List<TestActionResultModel> actionResults,
      List<TestResultParameterModel> parameters,
      List<TestAttachmentModel> attachments,
      List<TestResultLinkedBugsModel> linkedBugs = null)
    {
      this.ActionResults = actionResults;
      this.Parameters = parameters;
      this.Attachments = attachments;
      this.AssociatedBugs = linkedBugs;
    }

    [DataMember(Name = "actionResults")]
    public List<TestActionResultModel> ActionResults { get; set; }

    [DataMember(Name = "parameters")]
    public List<TestResultParameterModel> Parameters { get; set; }

    [DataMember(Name = "attachments")]
    public List<TestAttachmentModel> Attachments { get; set; }

    [DataMember(Name = "associatedBugs")]
    public List<TestResultLinkedBugsModel> AssociatedBugs { get; set; }
  }
}
