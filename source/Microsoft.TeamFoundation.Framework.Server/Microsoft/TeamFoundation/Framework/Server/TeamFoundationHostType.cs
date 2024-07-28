// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationHostType
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Flags]
  public enum TeamFoundationHostType
  {
    [Obsolete] Application = 2,
    [Obsolete] Organization = Application, // 0x00000002
    Enterprise = Organization, // 0x00000002
    Parent = -1, // 0xFFFFFFFF
    Unknown = 0,
    Deployment = 1,
    ProjectCollection = 4,
    All = ProjectCollection | Deployment | Enterprise, // 0x00000007
  }
}
