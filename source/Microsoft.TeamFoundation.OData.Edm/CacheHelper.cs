// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.CacheHelper
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm
{
  internal static class CacheHelper
  {
    internal static readonly object Unknown = new object();
    internal static readonly object CycleSentinel = new object();
    internal static readonly object SecondPassCycleSentinel = new object();
    private static readonly object BoxedTrue = (object) true;
    private static readonly object BoxedFalse = (object) false;

    internal static object BoxedBool(bool value) => !value ? CacheHelper.BoxedFalse : CacheHelper.BoxedTrue;
  }
}
