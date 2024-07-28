// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.BuildConfiguration
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public sealed class BuildConfiguration : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Number { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Uri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Flavor { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Platform { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int BuildDefinitionId { get; set; }

    [Obsolete("Use RepositoryGuid instead")]
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int RepositoryId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string RepositoryGuid { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string BranchName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string SourceVersion { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime CreationDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string BuildSystem { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string RepositoryType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ShallowReference Project { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string TargetBranchName { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      this.Project?.InitializeSecureObject(securedObject);
    }
  }
}
