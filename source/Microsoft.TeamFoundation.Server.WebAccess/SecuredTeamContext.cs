// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.SecuredTeamContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class SecuredTeamContext : ISecuredObject
  {
    private ISecuredObject m_securedObject;

    public SecuredTeamContext(ISecuredObject securedObject) => this.m_securedObject = securedObject;

    [DataMember(EmitDefaultValue = false, Order = 10)]
    public Guid Id { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 20)]
    public string Name { get; set; }

    [IgnoreDataMember]
    public Guid NamespaceId => this.m_securedObject.NamespaceId;

    [IgnoreDataMember]
    public int RequiredPermissions => this.m_securedObject.RequiredPermissions;

    public string GetToken() => this.m_securedObject.GetToken();
  }
}
