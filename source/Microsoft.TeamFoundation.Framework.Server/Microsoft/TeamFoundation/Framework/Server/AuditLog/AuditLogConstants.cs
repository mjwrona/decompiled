// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.AuditLogConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public static class AuditLogConstants
  {
    public static readonly Guid AuditServiceType = new Guid("00000064-0000-8888-8000-000000000000");
    public static readonly Guid SecurityNamespaceId = new Guid("A6CC6381-A1CA-4B36-B3C1-4E65211E82B6");
    public const string AuditLog = "AuditLog";
  }
}
