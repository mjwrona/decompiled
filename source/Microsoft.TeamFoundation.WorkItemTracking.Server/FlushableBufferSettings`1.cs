// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FlushableBufferSettings`1
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class FlushableBufferSettings<T> where T : BufferableItem
  {
    public const string MaxQueueLengthRegistryKeySuffix = "/MaxQueueLength";
    public const string MaxAgeRegistryKeySuffix = "/MaxItemAge";
    public const int DefaultQueueLengthThreshold = 1000;
    public const int DefaultAgeThresholdMs = 60000;
    public const int MinAgeThresholdMs = 5000;
    public const int MaxAgeThresholdMs = 900000;
    private int m_maxRecordAgeInMs;

    public FlushableBufferSettings()
    {
      this.MaxRecordAgeMs = 60000;
      this.MaxQueuedLength = 1000;
      this.FlushBucketsCount = this.MaxRecordAgeMs / 1000;
    }

    public FlushableBufferSettings(int maxAge, int maxQueueLength)
    {
      this.MaxRecordAgeMs = maxAge;
      this.MaxQueuedLength = maxQueueLength;
      this.FlushBucketsCount = this.MaxRecordAgeMs / 1000;
    }

    public int MaxRecordAgeMs
    {
      get => this.m_maxRecordAgeInMs;
      set
      {
        this.m_maxRecordAgeInMs = value;
        if (this.m_maxRecordAgeInMs >= 5000 && this.m_maxRecordAgeInMs <= 900000)
          return;
        if (this.m_maxRecordAgeInMs < 5000)
        {
          this.m_maxRecordAgeInMs = 5000;
        }
        else
        {
          if (this.m_maxRecordAgeInMs <= 900000)
            return;
          this.m_maxRecordAgeInMs = 900000;
        }
      }
    }

    public int FlushBucketsCount { get; }

    public int MaxQueuedLength { get; set; }
  }
}
