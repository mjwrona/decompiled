// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestStepModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  internal class TestStepModel : TestActionModel
  {
    private const string c_stepActionType = "Step";
    private const string c_TestStepMetaTag = "[teststep={0}]";

    [DataMember(Name = "stepType")]
    public string StepType { get; set; }

    [DataMember(Name = "expectedResult")]
    public string ExpectedResult { get; set; }

    [DataMember(Name = "attachments")]
    public List<WorkItemResourceLinkInfo> Attachments { get; set; }

    public static TestStepModel CreateStep(
      int id,
      string stepType,
      string action,
      string expectedResult,
      bool isFormatted = false)
    {
      TestStepModel step = new TestStepModel();
      step.Id = id;
      step.StepType = stepType;
      step.Action = action;
      step.ExpectedResult = expectedResult;
      step.ActionType = "Step";
      return step;
    }

    public override bool ProcessTestAttachments(List<WorkItemResourceLinkInfo> attachmentList)
    {
      this.Attachments = new List<WorkItemResourceLinkInfo>();
      string x = string.Format("[teststep={0}]", (object) this.Id);
      if (attachmentList != null && attachmentList.Count > 0)
      {
        for (int index = 0; index < attachmentList.Count; ++index)
        {
          WorkItemResourceLinkInfo attachment = attachmentList[index];
          string comment = attachment.Comment;
          if (comment != null && comment.Length > x.Length)
          {
            string y = comment.Substring(0, x.Length);
            if (StringComparer.OrdinalIgnoreCase.Equals(x, y))
              this.Attachments.Add(attachment);
          }
        }
      }
      return false;
    }
  }
}
