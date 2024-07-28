// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.ExceptionExtensionMethods
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Reflection;

namespace Microsoft.Azure.NotificationHubs.Common
{
  internal static class ExceptionExtensionMethods
  {
    private static MethodInfo prepForRemotingMethodInfo;

    public static Exception PrepareForRethrow(this Exception exception)
    {
      if (!ExceptionExtensionMethods.ShouldPrepareForRethrow(exception) || !PartialTrustHelpers.UnsafeIsInFullTrust())
        return exception;
      if (ExceptionExtensionMethods.prepForRemotingMethodInfo == (MethodInfo) null)
        ExceptionExtensionMethods.prepForRemotingMethodInfo = typeof (Exception).GetMethod("PrepForRemoting", BindingFlags.Instance | BindingFlags.NonPublic, (Binder) null, new Type[0], new ParameterModifier[0]);
      if (ExceptionExtensionMethods.prepForRemotingMethodInfo != (MethodInfo) null)
        ExceptionExtensionMethods.prepForRemotingMethodInfo.Invoke((object) exception, new object[0]);
      return exception;
    }

    public static Exception DisablePrepareForRethrow(this Exception exception)
    {
      exception.Data[(object) nameof (DisablePrepareForRethrow)] = (object) string.Empty;
      return exception;
    }

    private static bool ShouldPrepareForRethrow(Exception exception)
    {
      for (; exception != null; exception = exception.InnerException)
      {
        if (exception.Data != null && exception.Data.Contains((object) "DisablePrepareForRethrow"))
          return false;
      }
      return true;
    }
  }
}
