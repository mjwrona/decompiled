// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestLog
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
  public class TestLog : TestManagementBaseSecuredObject
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public TestLogReference LogReference { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime ModifiedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public long Size { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Dictionary<string, string> MetaData { get; set; }

    internal override void InitializeSecureObject(ISecuredObject securedObject)
    {
      base.InitializeSecureObject(securedObject);
      if (this.LogReference == null)
        return;
      this.LogReference.InitializeSecureObject(securedObject);
    }
  }
}
