// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ExceptionUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData
{
  internal static class ExceptionUtils
  {
    private static readonly Type OutOfMemoryType = typeof (OutOfMemoryException);

    internal static bool IsCatchableExceptionType(Exception e) => e.GetType() != ExceptionUtils.OutOfMemoryType;

    internal static T CheckArgumentNotNull<T>([ExceptionUtils.ValidatedNotNull] T value, string parameterName) where T : class => (object) value != null ? value : throw Error.ArgumentNull(parameterName);

    internal static void CheckArgumentStringNotEmpty(string value, string parameterName)
    {
      switch (value)
      {
        case "":
          throw new ArgumentException(Strings.ExceptionUtils_ArgumentStringEmpty, parameterName);
      }
    }

    internal static void CheckArgumentStringNotNullOrEmpty([ExceptionUtils.ValidatedNotNull] string value, string parameterName)
    {
      if (string.IsNullOrEmpty(value))
        throw new ArgumentNullException(parameterName, Strings.ExceptionUtils_ArgumentStringNullOrEmpty);
    }

    internal static void CheckIntegerNotNegative(int value, string parameterName)
    {
      if (value < 0)
        throw new ArgumentOutOfRangeException(parameterName, Strings.ExceptionUtils_CheckIntegerNotNegative((object) value));
    }

    internal static void CheckIntegerPositive(int value, string parameterName)
    {
      if (value <= 0)
        throw new ArgumentOutOfRangeException(parameterName, Strings.ExceptionUtils_CheckIntegerPositive((object) value));
    }

    internal static void CheckLongPositive(long value, string parameterName)
    {
      if (value <= 0L)
        throw new ArgumentOutOfRangeException(parameterName, Strings.ExceptionUtils_CheckLongPositive((object) value));
    }

    internal static void CheckArgumentCollectionNotNullOrEmpty<T>(
      ICollection<T> value,
      string parameterName)
    {
      if (value == null)
        throw Error.ArgumentNull(parameterName);
      if (value.Count == 0)
        throw new ArgumentException(Strings.ExceptionUtils_ArgumentStringEmpty, parameterName);
    }

    private sealed class ValidatedNotNullAttribute : Attribute
    {
    }
  }
}
