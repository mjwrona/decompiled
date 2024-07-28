// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestCaseModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  internal class TestCaseModel
  {
    private bool m_attachmentProcessed;

    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "title")]
    public string Title { get; set; }

    [DataMember(Name = "areaPath")]
    public string AreaPath { get; set; }

    [DataMember(Name = "testers")]
    public string TestStepsXml { get; set; }

    [DataMember(Name = "workitem")]
    public WorkItem WorkItem { get; private set; }

    [DataMember(Name = "testSteps")]
    public List<TestActionModel> TestSteps { get; set; }

    public TestCaseModel()
    {
    }

    public TestCaseModel(TestManagerRequestContext testContext, WorkItem workItem)
    {
      IVssRequestContext tfsRequestContext = testContext.TfsRequestContext;
      object fieldValue1 = workItem.GetFieldValue(tfsRequestContext, "System.Title");
      object fieldValue2 = workItem.GetFieldValue(tfsRequestContext, WorkItemFieldNames.Actions);
      object fieldValue3 = workItem.GetFieldValue(tfsRequestContext, "System.AreaPath");
      this.Id = workItem.Id;
      this.Title = fieldValue1 == null ? string.Empty : fieldValue1.ToString();
      this.AreaPath = fieldValue3 == null ? string.Empty : fieldValue3.ToString();
      this.TestStepsXml = fieldValue2 == null ? string.Empty : fieldValue2.ToString();
      this.WorkItem = workItem;
    }

    public List<TestActionModel> GetTestSteps()
    {
      if (this.TestSteps == null)
        this.TestSteps = TestStepXmlParserHelper.GetTestStepsArray(this.TestStepsXml == null ? string.Empty : this.TestStepsXml);
      return this.TestSteps;
    }

    public void ProcessTestStepAttachments(TestManagerRequestContext testContext)
    {
      if (this.TestSteps == null)
        this.GetTestSteps();
      if (this.m_attachmentProcessed)
        return;
      for (int index = 0; index < this.TestSteps.Count; ++index)
      {
        TestActionModel testStep = this.TestSteps[index];
        this.TestSteps[index].ProcessTestAttachments(this.WorkItem.ResourceLinks.ToList<WorkItemResourceLinkInfo>());
      }
      this.m_attachmentProcessed = true;
    }
  }
}
