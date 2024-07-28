// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdateTestRunRequest
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi.Legacy
{
  [DataContract]
  public sealed class UpdateTestRunRequest
  {
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string ProjectName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public LegacyTestRun TestRun { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public TestResultAttachment[] AttachmentsToAdd { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public TestResultAttachmentIdentity[] AttachmentsToDelete { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool ShouldHyderate { get; set; }
  }
}
