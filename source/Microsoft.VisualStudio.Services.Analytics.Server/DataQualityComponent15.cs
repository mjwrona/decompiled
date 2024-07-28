// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DataQualityComponent15
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class DataQualityComponent15 : DataQualityComponent11
  {
    public override void InitializeModelReady()
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_InitializeModelReady");
      this.ExecuteNonQuery();
    }
  }
}
