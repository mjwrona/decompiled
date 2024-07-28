// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Metrics.IHistogram`1
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Telemetry.Metrics
{
  public interface IHistogram<T> : IInstrument where T : struct
  {
    void Record(T value);

    void Record(T value, KeyValuePair<string, object?> tag);

    void Record(T value, KeyValuePair<string, object?> tag1, KeyValuePair<string, object?> tag2);

    void Record(
      T value,
      KeyValuePair<string, object?> tag1,
      KeyValuePair<string, object?> tag2,
      KeyValuePair<string, object?> tag3);

    void Record(T value, params KeyValuePair<string, object>[] tags);

    void Record(T value, ReadOnlySpan<KeyValuePair<string, object?>> tags);
  }
}
