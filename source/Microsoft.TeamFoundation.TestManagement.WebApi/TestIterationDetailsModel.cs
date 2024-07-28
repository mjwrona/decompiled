// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestIterationDetailsModel
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class TestIterationDetailsModel : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int Id;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Outcome;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Comment;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ErrorMessage;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime StartedDate;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime CompletedDate;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public double DurationInMs;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<TestActionResultModel> ActionResults;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<TestResultParameterModel> Parameters;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<TestCaseResultAttachmentModel> Attachments;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Url;

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      if (this.ActionResults != null)
      {
        foreach (TestManagementBaseSecuredObject actionResult in this.ActionResults)
          actionResult.InitializeSecureObject(securedObject);
      }
      if (this.Parameters != null)
      {
        foreach (TestManagementBaseSecuredObject parameter in this.Parameters)
          parameter.InitializeSecureObject(securedObject);
      }
      if (this.Attachments == null)
        return;
      foreach (TestManagementBaseSecuredObject attachment in this.Attachments)
        attachment.InitializeSecureObject(securedObject);
    }
  }
}
