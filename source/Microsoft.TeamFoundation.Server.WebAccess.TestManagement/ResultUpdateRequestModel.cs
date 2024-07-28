// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.ResultUpdateRequestModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public class ResultUpdateRequestModel
  {
    [DataMember(Name = "testRunId")]
    public int TestRunId { get; set; }

    [DataMember(Name = "testResultId")]
    public int TestResultId { get; set; }

    [DataMember(Name = "testCaseResult")]
    public TestCaseResultModel TestCaseResult { get; set; }

    [DataMember(Name = "actionResults")]
    public TestActionResultModel[] ActionResults { get; set; }

    [DataMember(Name = "actionResultDeletes")]
    public TestActionResultModel[] ActionResultDeletes { get; set; }

    [DataMember(Name = "parameters")]
    public TestResultParameterModel[] Parameters { get; set; }

    [DataMember(Name = "parameterDeletes")]
    public TestResultParameterModel[] ParameterDeletes { get; set; }

    internal ResultUpdateRequest CreateFromModel() => new ResultUpdateRequest()
    {
      TestRunId = this.TestRunId,
      TestResultId = this.TestResultId,
      TestCaseResult = this.TestCaseResult.CreateFromModel(),
      ActionResults = ((IEnumerable<TestActionResultModel>) this.ActionResults).Select<TestActionResultModel, TestActionResult>((Func<TestActionResultModel, TestActionResult>) (action => action.CreateFromModel())).ToArray<TestActionResult>(),
      ActionResultDeletes = ((IEnumerable<TestActionResultModel>) this.ActionResultDeletes).Select<TestActionResultModel, TestActionResult>((Func<TestActionResultModel, TestActionResult>) (action => action.CreateFromModel())).ToArray<TestActionResult>(),
      Parameters = ((IEnumerable<TestResultParameterModel>) this.Parameters).Select<TestResultParameterModel, TestResultParameter>((Func<TestResultParameterModel, TestResultParameter>) (parameter => parameter.CreateFromModel())).ToArray<TestResultParameter>(),
      ParameterDeletes = ((IEnumerable<TestResultParameterModel>) this.ParameterDeletes).Select<TestResultParameterModel, TestResultParameter>((Func<TestResultParameterModel, TestResultParameter>) (parameter => parameter.CreateFromModel())).ToArray<TestResultParameter>()
    };
  }
}
