// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestActionResultModel
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class TestActionResultModel : TestResultModelBase
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ActionPath;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int IterationId;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public SharedStepModel SharedStepModel;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Url;

    public TestActionResultModel()
    {
    }

    public TestActionResultModel(
      string actionPath,
      string iterationId,
      SharedStepModel sharedStepModel,
      string outcome,
      string startedDate,
      string completedDate,
      string duration,
      string errorMessage = "",
      string comment = "")
      : base(outcome, startedDate, completedDate, duration, errorMessage, comment)
    {
      this.ActionPath = actionPath;
      if (!int.TryParse(iterationId, out this.IterationId))
        throw new ArgumentException(nameof (iterationId));
      if (sharedStepModel.Id == 0 && sharedStepModel.Revision == 0)
        return;
      this.SharedStepModel = sharedStepModel;
    }

    [DataMember(EmitDefaultValue = false)]
    public string StepIdentifier { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      this.SharedStepModel?.InitializeSecureObject(securedObject);
    }
  }
}
