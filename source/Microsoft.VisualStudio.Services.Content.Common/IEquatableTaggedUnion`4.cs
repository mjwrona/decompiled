// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.IEquatableTaggedUnion`4
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public interface IEquatableTaggedUnion<T1, T2, T3, T4> : 
    ITaggedUnion<T1, T2, T3, T4>,
    IEquatable<IEquatableTaggedUnion<T1, T2, T3, T4>>
    where T1 : IEquatable<T1>
    where T2 : IEquatable<T2>
    where T3 : IEquatable<T3>
    where T4 : IEquatable<T4>
  {
  }
}
