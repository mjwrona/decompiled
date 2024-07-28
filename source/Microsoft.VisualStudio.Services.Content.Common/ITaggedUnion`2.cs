// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ITaggedUnion`2
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public interface ITaggedUnion<out T1, out T2>
  {
    void Match(Action<T1> onT1, Action<T2> onT2);

    T Match<T>(Func<T1, T> onT1, Func<T2, T> onT2);
  }
}
