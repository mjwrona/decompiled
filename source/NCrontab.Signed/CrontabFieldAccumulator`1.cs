// Decompiled with JetBrains decompiler
// Type: NCrontab.CrontabFieldAccumulator`1
// Assembly: NCrontab.Signed, Version=3.3.2.0, Culture=neutral, PublicKeyToken=5247b4370afff365
// MVID: 57590294-74AB-400C-8AE3-EEC26A6094FB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\NCrontab.Signed.dll

using System;

namespace NCrontab
{
  internal delegate T CrontabFieldAccumulator<T>(
    int start,
    int end,
    int interval,
    T successs,
    Func<ExceptionProvider, T> onError);
}
