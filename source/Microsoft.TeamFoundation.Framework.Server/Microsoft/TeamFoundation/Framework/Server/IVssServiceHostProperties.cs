// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IVssServiceHostProperties
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public interface IVssServiceHostProperties
  {
    string Name { get; }

    TeamFoundationHostType HostType { get; }

    string PhysicalDirectory { get; }

    string PlugInDirectory { get; }

    string StaticContentDirectory { get; }

    TeamFoundationServiceHostStatus Status { get; }

    string StatusReason { get; }

    string Description { get; }
  }
}
