// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestResultsGroupsForRelease
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  public class TestResultsGroupsForRelease : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int ReleaseId;
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int ReleaseEnvId;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IList<FieldDetailsForTestResults> Fields { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      if (this.Fields == null)
        return;
      foreach (TestManagementBaseSecuredObject field in (IEnumerable<FieldDetailsForTestResults>) this.Fields)
        field.InitializeSecureObject(securedObject);
    }
  }
}
