// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingContextExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class ServicingContextExtensions
  {
    public static TValue GetTokenOrDefault<TValue>(
      this IServicingContext servicingContext,
      string tokenName,
      TValue defaultValue = null)
    {
      string str;
      return !servicingContext.TryGetToken(tokenName, out str) ? defaultValue : RegistryUtility.FromString<TValue>(str);
    }

    internal static IInternalServicingContext AsInternalServicingContext(
      this IServicingContext servicingContext,
      [CallerMemberName] string callerMemberName = "Unknown")
    {
      if (servicingContext == null)
        throw new ArgumentNullException(nameof (servicingContext));
      return servicingContext is IInternalServicingContext servicingContext1 ? servicingContext1 : throw new ArgumentException("The servicing context passed was not an instance of IInternalServicingCOntext, which is required by " + callerMemberName);
    }
  }
}
