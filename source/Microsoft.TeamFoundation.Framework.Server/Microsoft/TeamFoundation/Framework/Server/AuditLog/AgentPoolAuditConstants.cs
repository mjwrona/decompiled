// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.AgentPoolAuditConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public static class AgentPoolAuditConstants
  {
    private static readonly string LibraryArea = "Library.";
    public static readonly string AgentPoolCreated = AgentPoolAuditConstants.LibraryArea + nameof (AgentPoolCreated);
    public static readonly string AgentPoolDeleted = AgentPoolAuditConstants.LibraryArea + nameof (AgentPoolDeleted);
    public static readonly string AgentAdded = AgentPoolAuditConstants.LibraryArea + nameof (AgentAdded);
    public static readonly string AgentDeleted = AgentPoolAuditConstants.LibraryArea + nameof (AgentDeleted);
    public static readonly string AgentsDeleted = AgentPoolAuditConstants.LibraryArea + nameof (AgentsDeleted);
  }
}
