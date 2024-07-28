// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.SecurityObject
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;

namespace Microsoft.TeamFoundation.Integration.Server
{
  internal class SecurityObject
  {
    private int m_instance;
    private string m_objectId;
    private string m_securityToken;
    private string m_classId;
    private Guid m_projectId;

    public SecurityObject(
      int instance,
      string objectId,
      string securityToken,
      string classId,
      Guid projectId)
    {
      this.m_instance = instance;
      this.m_objectId = objectId;
      this.m_securityToken = securityToken;
      this.m_classId = classId;
      this.m_projectId = projectId;
    }

    public int Instance => this.m_instance;

    public string ObjectId => this.m_objectId;

    public string SecurityToken => this.m_securityToken;

    public string ClassId => this.m_classId;

    public Guid ProjectId => this.m_projectId;
  }
}
