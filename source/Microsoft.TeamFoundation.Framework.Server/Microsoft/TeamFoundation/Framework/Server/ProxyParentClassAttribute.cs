// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ProxyParentClassAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public sealed class ProxyParentClassAttribute : Attribute
  {
    private string m_parentClassName;
    private bool m_ignoreInheritedMethods;

    public ProxyParentClassAttribute(string parentClassName) => this.m_parentClassName = parentClassName;

    public string ParentClassName => this.m_parentClassName;

    public bool IgnoreInheritedMethods
    {
      get => this.m_ignoreInheritedMethods;
      set => this.m_ignoreInheritedMethods = value;
    }
  }
}
