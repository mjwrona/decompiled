// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.PlanConcurrency
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public class PlanConcurrency
  {
    public string DataspaceCategory { get; set; }

    public Guid ScopeIdentifier { get; set; }

    public int DefinitionId { get; set; }

    public int MaxConcurrency { get; set; }

    public string Message { get; set; }

    public string IncidentId { get; set; }

    public static string GetIdentifier(
      string dataspaceCategory,
      Guid scopeIdentifier,
      int? definitionId)
    {
      return string.Format("{0}/{1}/{2}", (object) dataspaceCategory, (object) scopeIdentifier, (object) definitionId);
    }

    public string GetIdentifier() => PlanConcurrency.GetIdentifier(this.DataspaceCategory, this.ScopeIdentifier, new int?(this.DefinitionId));
  }
}
