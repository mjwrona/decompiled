// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.StageFieldMetaData
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.SqlServer.Server;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class StageFieldMetaData
  {
    public int MinStageVersion { get; }

    public int? MaxStageVersion { get; }

    public SqlMetaData SqlMetaData { get; }

    public StageFieldMetaData(int minStageVersion, int? maxStageVersion, SqlMetaData sqlMetaData)
    {
      this.MinStageVersion = minStageVersion;
      this.MaxStageVersion = maxStageVersion;
      this.SqlMetaData = sqlMetaData;
    }
  }
}
