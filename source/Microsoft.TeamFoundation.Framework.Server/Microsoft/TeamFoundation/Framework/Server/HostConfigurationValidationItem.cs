// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostConfigurationValidationItem
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class HostConfigurationValidationItem
  {
    private HostConfigurationValidationItemType m_type;
    private string m_message;

    public HostConfigurationValidationItem(HostConfigurationValidationItemType type, string message)
    {
      this.m_type = type;
      this.m_message = message;
    }

    public virtual HostConfigurationValidationItemType Type
    {
      get => this.m_type;
      protected set => this.m_type = value;
    }

    public virtual string Message
    {
      get => this.m_message;
      protected set => this.m_message = value;
    }
  }
}
