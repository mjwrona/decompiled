// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ContextType
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Flags]
  public enum ContextType : byte
  {
    None = 0,
    Activity = 1,
    Job = 2,
    Task = 4,
    Notification = 8,
    Pipeline = 16, // 0x10
    Anonymous = 32, // 0x20
    Public = 64, // 0x40
    Other = 128, // 0x80
  }
}
