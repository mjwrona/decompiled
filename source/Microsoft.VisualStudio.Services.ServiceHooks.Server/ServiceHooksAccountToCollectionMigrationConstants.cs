// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.ServiceHooksAccountToCollectionMigrationConstants
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server
{
  public static class ServiceHooksAccountToCollectionMigrationConstants
  {
    public const string NotStarted = "not started";
    public const string InProgress = "in progress";
    public const string Completed = "completed";
    public const string RegistryPathSql = "#\\Service\\ServiceHooks\\AccountMigrationState\\";
    public const string RegistryPathCSharp = "/Service/ServiceHooks/AccountMigrationState";
  }
}
