// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseManagementSecuredObject
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [DataContract]
  public class ReleaseManagementSecuredObject : SecuredObject
  {
    public ReleaseManagementSecuredObject()
    {
    }

    public ReleaseManagementSecuredObject(string token, int requiredPermissions)
      : base(SecurityConstants.ReleaseManagementSecurityNamespaceId, token, requiredPermissions)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual void SetSecuredObject(string token, int requiredPermissions) => this.SetSecuredObject(SecurityConstants.ReleaseManagementSecurityNamespaceId, token, requiredPermissions);
  }
}
