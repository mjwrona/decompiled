// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.CustomSqlError
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

namespace Microsoft.Azure.Pipelines.Deployment
{
  internal static class CustomSqlError
  {
    public const int GenericDatabaseUserMessage = 50000;
    public const int GenericDatabaseUpdateFailure = 2000002;
    public const int MetadataNoteNotFound = 2000021;
    public const int MetadataOccurrenceNotFound = 2000022;
    public const int MetadataOccurrenceTagAlreadyExists = 2000023;
    public const int MetadataNoteAlreadyExists = 2000024;
    public const int TransactionRequired = 2000025;
  }
}
