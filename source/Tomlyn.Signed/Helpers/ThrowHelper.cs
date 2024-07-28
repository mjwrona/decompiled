// Decompiled with JetBrains decompiler
// Type: Tomlyn.Helpers.ThrowHelper
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;


#nullable enable
namespace Tomlyn.Helpers
{
  internal static class ThrowHelper
  {
    public static ArgumentOutOfRangeException GetIndexNegativeArgumentOutOfRangeException() => new ArgumentOutOfRangeException("index", "Index must be positive");

    public static ArgumentOutOfRangeException GetIndexArgumentOutOfRangeException(int maxValue) => new ArgumentOutOfRangeException("index", string.Format("Index must be less than {0}", (object) maxValue));

    public static InvalidOperationException GetExpectingNoParentException() => new InvalidOperationException("The node is already attached to another parent");
  }
}
