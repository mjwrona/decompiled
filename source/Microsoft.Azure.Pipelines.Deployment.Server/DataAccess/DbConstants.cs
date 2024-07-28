// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.DbConstants
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  public static class DbConstants
  {
    public const long TriggerAliasColumnLength = 256;
    public const long ArtifactTypeColumnLength = 256;
    public const long TriggerContentColumnLength = 4000;
    public const long UniqueResourceIdentifierColumnLength = 2048;
    public const long MaxValue = -1;
    public const long PropertiesColumnLength = 4000;
  }
}
