// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestBase
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  internal class TestBase : ITestActionOwner, ITestBase
  {
    private TestActionRootGroup m_actions;

    public TestActionCollection Actions => this.RootGroup.Actions;

    public ITestStep CreateTestStep() => (ITestStep) new TestStep((ITestActionOwner) this);

    public ISharedStep CreateSharedStepReference() => (ISharedStep) new SharedStep((ITestActionOwner) this);

    public JsonPatchDocument SaveActions(JsonPatchDocument json)
    {
      if (json != null && this.m_actions != null)
      {
        this.UpdateTestStepsInJson(json);
        this.UpdateTestStepAttachmentsInJson(json);
      }
      return json;
    }

    public void LoadActions(string xmlString, IList<TestAttachmentLink> links)
    {
      if (string.IsNullOrEmpty(xmlString))
        return;
      this.LoadActionsFromXml(xmlString);
      this.LoadTestStepAttachmentsFromLinks(links);
    }

    public string GenerateXmlFromActions() => XmlStorageHelper.ToXml((IXmlStorage) this.m_actions);

    public void LoadActionsFromXml(string xmlString)
    {
      if (this.m_actions == null)
        this.m_actions = new TestActionRootGroup((ITestActionOwner) this);
      XmlStorageHelper.FromXml((IXmlStorage) this.m_actions, xmlString);
    }

    public int GetNextAvailableActionId() => this.RootGroup.GetNextAvailableId();

    internal TestActionRootGroup RootGroup
    {
      get
      {
        if (this.m_actions == null)
          this.m_actions = new TestActionRootGroup((ITestActionOwner) this);
        return this.m_actions;
      }
    }

    private void UpdateTestStepsInJson(JsonPatchDocument json)
    {
      string xmlFromActions = this.GenerateXmlFromActions();
      json.Add(new JsonPatchOperation()
      {
        Operation = Operation.Add,
        Path = "/fields/Microsoft.VSTS.TCM.Steps",
        Value = (object) xmlFromActions
      });
    }

    private void UpdateTestStepAttachmentsInJson(JsonPatchDocument json)
    {
      foreach (ITestAction action in (Collection<ITestAction>) this.m_actions.Actions)
      {
        if (action is TestStep testStep)
          testStep.UpdateAttachmentsInJson(json);
      }
    }

    private void LoadTestStepAttachmentsFromLinks(IList<TestAttachmentLink> links)
    {
      if (links == null)
        return;
      foreach (ITestAction action in (Collection<ITestAction>) this.m_actions.Actions)
      {
        if (action is TestStep testStep)
          testStep.LoadAttachementFromLinks(links);
      }
    }
  }
}
