// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.UtcClock
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public sealed class UtcClock : IClock
  {
    public static readonly IClock Instance = (IClock) new UtcClock();

    private UtcClock()
    {
    }

    public DateTimeOffset Now => DateTimeOffset.UtcNow;
  }
}
