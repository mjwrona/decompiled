// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestLogReference
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class TestLogReference : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = true)]
    public TestLogScope Scope { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public int BuildId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int ReleaseId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int ReleaseEnvId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public int RunId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public int ResultId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public int SubResultId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public TestLogType Type { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string FilePath { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject) => base.InitializeSecureObject(securedObject);
  }
}
