// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.CodeRepositoryReference
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class CodeRepositoryReference : ReleaseManagementSecuredObject
  {
    [DataMember]
    public PullRequestSystemType SystemType { get; set; }

    [DataMember]
    public IDictionary<string, ReleaseManagementInputValue> RepositoryReference { get; set; }

    internal override void SetSecuredObject(string token, int requiredPermissions)
    {
      base.SetSecuredObject(token, requiredPermissions);
      IDictionary<string, ReleaseManagementInputValue> repositoryReference = this.RepositoryReference;
      if (repositoryReference == null)
        return;
      repositoryReference.ForEach<KeyValuePair<string, ReleaseManagementInputValue>>((Action<KeyValuePair<string, ReleaseManagementInputValue>>) (r => r.Value.SetSecuredObject(token, requiredPermissions)));
    }
  }
}
