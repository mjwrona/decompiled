// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.ResultUpdateRequestModel
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class ResultUpdateRequestModel
  {
    public ResultUpdateRequestModel()
    {
    }

    public ResultUpdateRequestModel(
      TestCaseResultUpdateModel resultUpdateModel,
      TestActionResultModel[] actionResults = null,
      TestActionResultModel[] deletedActionResults = null,
      TestResultParameterModel[] parameters = null,
      TestResultParameterModel[] deletedParameters = null)
    {
      this.TestCaseResult = resultUpdateModel;
      this.ActionResults = actionResults == null ? Array.Empty<TestActionResultModel>() : actionResults;
      this.ActionResultDeletes = deletedActionResults == null ? Array.Empty<TestActionResultModel>() : deletedActionResults;
      this.Parameters = parameters == null ? Array.Empty<TestResultParameterModel>() : parameters;
      if (deletedParameters != null)
        this.ParameterDeletes = deletedParameters;
      else
        this.ParameterDeletes = Array.Empty<TestResultParameterModel>();
    }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public TestCaseResultUpdateModel TestCaseResult { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public TestActionResultModel[] ActionResults { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public TestActionResultModel[] ActionResultDeletes { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public TestResultParameterModel[] Parameters { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public TestResultParameterModel[] ParameterDeletes { get; set; }
  }
}
