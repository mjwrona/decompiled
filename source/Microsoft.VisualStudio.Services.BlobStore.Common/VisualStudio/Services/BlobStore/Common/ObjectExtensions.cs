// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.ObjectExtensions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public static class ObjectExtensions
  {
    public const string NullDescriptionString = "{null}";

    public static string ToStringWithNullCheck<T>(this T obj) => (object) obj != null ? obj.ToString() : "{null}";

    public static string PropertyToStringWithNullCheck<X, Y>(this X x, Func<X, Y> propertyGetter)
    {
      if ((object) x == null)
        return "{null}";
      Y y = propertyGetter(x);
      return (object) y != null ? y.ToString() : "{null}";
    }
  }
}
