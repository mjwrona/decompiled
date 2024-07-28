// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.DataProviderMetadata
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public abstract class DataProviderMetadata : ISecuredObject
  {
    private string m_securityToken;

    public DataProviderMetadata(string dataspaceId = null) => this.m_securityToken = "/DataProviderMetadata/" + (dataspaceId ?? Guid.Empty.ToString());

    [IgnoreDataMember]
    public Guid NamespaceId => DataProviderSecurityConstants.NamespaceId;

    [IgnoreDataMember]
    public int RequiredPermissions => 1;

    public string GetToken() => this.m_securityToken;
  }
}
