// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.ITestBase
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  public interface ITestBase
  {
    ITestStep CreateTestStep();

    ISharedStep CreateSharedStepReference();

    TestActionCollection Actions { get; }

    JsonPatchDocument SaveActions(JsonPatchDocument json);

    void LoadActions(string xmlString, IList<TestAttachmentLink> links);

    string GenerateXmlFromActions();
  }
}
