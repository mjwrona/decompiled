// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.ModuleCoverage
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
  public sealed class ModuleCoverage : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int BlockCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public byte[] BlockData { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid Signature { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int SignatureAge { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public CoverageStatistics Statistics { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<FunctionCoverage> Functions { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string FileUrl { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      if (this.Statistics != null)
        this.Statistics.InitializeSecureObject(securedObject);
      if (this.Functions == null || this.Functions.Count <= 0)
        return;
      foreach (TestManagementBaseSecuredObject function in this.Functions)
        function.InitializeSecureObject(securedObject);
    }
  }
}
