// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.CleanupStreamResult
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class CleanupStreamResult
  {
    public bool Complete { get; set; }

    public int DeletedRows { get; set; }

    public int DeletedStageBatches { get; set; }

    public int DeletedStreams { get; set; }

    public int DeletedProcessBatches { get; set; }

    public bool Unsupported { get; set; }

    public void Add(CleanupStreamResult operand)
    {
      this.Complete = this.Complete && operand.Complete;
      this.DeletedRows += operand.DeletedRows;
      this.DeletedStageBatches += operand.DeletedStageBatches;
      this.DeletedStreams += operand.DeletedStreams;
      this.DeletedProcessBatches += operand.DeletedProcessBatches;
      this.Unsupported = this.Unsupported && operand.Unsupported;
    }

    public CleanupStreamResult() => this.Complete = true;
  }
}
