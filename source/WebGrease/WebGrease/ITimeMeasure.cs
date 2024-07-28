// Decompiled with JetBrains decompiler
// Type: WebGrease.ITimeMeasure
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;

namespace WebGrease
{
  public interface ITimeMeasure
  {
    TimeMeasureResult[] GetResults();

    void End(bool isGroup, params string[] idParts);

    void Start(bool isGroup, params string[] idParts);

    void WriteResults(string filePathWithoutExtension, string title, DateTimeOffset utcStart);
  }
}
