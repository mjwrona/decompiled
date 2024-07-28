// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Store.StoreUtility
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Store, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 88C5F689-5CBE-419A-B234-7228E63B94DF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Store.dll

using Microsoft.VisualStudio.Services.Search.Common;
using System;

namespace Microsoft.VisualStudio.Services.Search.Server.Store
{
  internal static class StoreUtility
  {
    public static bool IsNonWrappedException(Exception e)
    {
      switch (e)
      {
        case StoreException _:
        case NullReferenceException _:
          return true;
        default:
          return e is ArgumentException;
      }
    }

    public static void WrapAndThrowException(Exception e, string msg) => throw new StoreException(msg, e);
  }
}
