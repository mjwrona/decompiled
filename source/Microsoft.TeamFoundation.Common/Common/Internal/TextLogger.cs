// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.TextLogger
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.ComponentModel;
using System.IO;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TextLogger : TFLogger
  {
    private readonly object m_lock = new object();

    public TextLogger(TextWriter writer)
      : this(writer, false)
    {
    }

    public TextLogger(TextWriter writer, bool includeTimestamps)
    {
      this.Writer = writer;
      this.IncludeTimestamps = includeTimestamps;
    }

    public TextWriter Writer { get; private set; }

    public int ErrorCount { get; private set; }

    public bool IncludeTimestamps { get; private set; }

    public override void Info(string message)
    {
      lock (this.m_lock)
      {
        if (this.IncludeTimestamps)
          this.Writer.WriteLine(string.Format("[{0}] {1}", (object) DateTime.UtcNow, (object) TFCommonResources.TextLoggerInfoLine((object) message)));
        else
          this.Writer.WriteLine(TFCommonResources.TextLoggerInfoLine((object) message));
      }
    }

    public override void Warning(string message)
    {
      lock (this.m_lock)
      {
        if (this.IncludeTimestamps)
          this.Writer.WriteLine(string.Format("[{0}] {1}", (object) DateTime.UtcNow, (object) TFCommonResources.TextLoggerWarningLine((object) message)));
        else
          this.Writer.WriteLine(TFCommonResources.TextLoggerWarningLine((object) message));
      }
    }

    public override void Error(string message)
    {
      lock (this.m_lock)
      {
        if (this.IncludeTimestamps)
          this.Writer.WriteLine(string.Format("[{0}] {1}", (object) DateTime.UtcNow, (object) TFCommonResources.TextLoggerErrorLine((object) message)));
        else
          this.Writer.WriteLine(TFCommonResources.TextLoggerErrorLine((object) message));
        ++this.ErrorCount;
      }
    }
  }
}
