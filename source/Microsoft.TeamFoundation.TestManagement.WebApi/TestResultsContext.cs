// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestResultsContext
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class TestResultsContext : TestManagementBaseSecuredObject
  {
    public TestResultsContext()
    {
    }

    public TestResultsContext(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public TestResultsContextType ContextType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public BuildReference Build { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public ReleaseReference Release { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public PipelineReference PipelineReference { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      this.Build?.InitializeSecureObject(securedObject);
      this.Release?.InitializeSecureObject(securedObject);
      this.PipelineReference?.InitializeSecureObject(securedObject);
    }
  }
}
