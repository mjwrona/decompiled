// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.CustomSqlError
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  public static class CustomSqlError
  {
    public const int SqlServerDefaultUserMessage = 50000;
    public const int GenericDatabaseFailure = 1760000;
    public const int LanguageMetadataExists = 1760001;
    public const int LanguageMetadataNotFound = 1760002;
    public const int MAX_SQL_ERROR = 1760002;
  }
}
