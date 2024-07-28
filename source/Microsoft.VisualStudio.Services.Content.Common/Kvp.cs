// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Kvp
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class Kvp
  {
    public static KeyValuePair<T1, T2> Create<T1, T2>(T1 item1, T2 item2) => new KeyValuePair<T1, T2>(item1, item2);
  }
}
