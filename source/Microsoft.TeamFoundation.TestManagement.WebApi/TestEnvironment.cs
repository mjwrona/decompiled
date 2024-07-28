// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestEnvironment
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  [ClientInternalUseOnly(false)]
  public class TestEnvironment : TestManagementBaseSecuredObject
  {
    public TestEnvironment()
    {
    }

    public TestEnvironment(string environmentIdString)
    {
      Guid result;
      if (!Guid.TryParse(environmentIdString, out result))
        throw new ArgumentException("environmentId");
      this.EnvironmentId = result;
    }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public Guid EnvironmentId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string EnvironmentName { get; set; }
  }
}
