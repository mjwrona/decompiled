// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.Util
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Security;
using System.Threading;

namespace Microsoft.Spatial
{
  internal class Util
  {
    private static readonly Type StackOverflowType = typeof (StackOverflowException);
    private static readonly Type ThreadAbortType = typeof (ThreadAbortException);
    private static readonly Type AccessViolationType = typeof (AccessViolationException);
    private static readonly Type OutOfMemoryType = typeof (OutOfMemoryException);
    private static readonly Type NullReferenceType = typeof (NullReferenceException);
    private static readonly Type SecurityType = typeof (SecurityException);

    internal static void CheckArgumentNull([Util.ValidatedNotNull] object arg, string errorMessage)
    {
      if (arg == null)
        throw new ArgumentNullException(errorMessage);
    }

    internal static bool IsCatchableExceptionType(Exception e)
    {
      Type type = e.GetType();
      return type != Util.OutOfMemoryType && type != Util.StackOverflowType && type != Util.ThreadAbortType && type != Util.AccessViolationType && type != Util.NullReferenceType && !Util.SecurityType.IsAssignableFrom(type);
    }

    private sealed class ValidatedNotNullAttribute : Attribute
    {
    }
  }
}
