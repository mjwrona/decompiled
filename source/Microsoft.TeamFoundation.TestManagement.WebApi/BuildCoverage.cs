// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public sealed class BuildCoverage : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public BuildConfiguration Configuration { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string State { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string LastError { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<ModuleCoverage> Modules { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string CodeCoverageFileUrl { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      if (this.Configuration != null)
        this.Configuration.InitializeSecureObject(securedObject);
      if (this.Modules == null || this.Modules.Count <= 0)
        return;
      foreach (TestManagementBaseSecuredObject module in this.Modules)
        module.InitializeSecureObject(securedObject);
    }
  }
}
