// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi.Legacy
{
  [DataContract]
  public sealed class ResultUpdateRequest
  {
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestRunId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestResultId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public LegacyTestCaseResult TestCaseResult { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public TestActionResult[] ActionResults { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public TestActionResult[] ActionResultDeletes { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public TestResultParameter[] Parameters { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public TestResultParameter[] ParameterDeletes { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public TestResultAttachment[] Attachments { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public TestResultAttachmentIdentity[] AttachmentDeletes { get; set; }
  }
}
