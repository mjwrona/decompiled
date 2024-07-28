// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.ServiceDefinitionComparer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  internal class ServiceDefinitionComparer : IEqualityComparer<ServiceDefinition>
  {
    private static readonly ServiceDefinitionComparer m_instance = new ServiceDefinitionComparer();

    private ServiceDefinitionComparer()
    {
    }

    public bool Equals(ServiceDefinition x, ServiceDefinition y)
    {
      if (x == null && y == null)
        return true;
      return x != null && y != null && x.Identifier.Equals(y.Identifier) && x.ServiceType.Equals(y.ServiceType, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(ServiceDefinition obj) => obj.Identifier.GetHashCode() ^ obj.ServiceType.GetHashCode();

    public static ServiceDefinitionComparer Instance => ServiceDefinitionComparer.m_instance;
  }
}
