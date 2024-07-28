// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.DataAccess.CustomSqlError
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

namespace Microsoft.VisualStudio.Services.Orchestration.Server.DataAccess
{
  internal static class CustomSqlError
  {
    public const int GenericDatabaseUserMessage = 50000;
    public const int TransactionRequired = 907102;
    public const int GenericDatabaseUpdateFailure = 907104;
    public const int HubExists = 907105;
    public const int HubNotFound = 907106;
    public const int OrchestrationSessionExists = 907107;
    public const int OrchestrationSessionNotFound = 907108;
    public const int OrchestrationDispatcherNotFound = 907109;
  }
}
