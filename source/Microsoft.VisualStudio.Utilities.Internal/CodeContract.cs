// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Utilities.Internal.CodeContract
// Assembly: Microsoft.VisualStudio.Utilities.Internal, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E02F1555-E063-4795-BC05-853CA7424F51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Utilities.Internal.dll

using System;

namespace Microsoft.VisualStudio.Utilities.Internal
{
  public static class CodeContract
  {
    public static void RequiresArgumentNotNull<T>(this T value, string argumentName) where T : class
    {
      if ((object) value == null)
        throw new ArgumentNullException(argumentName);
    }

    public static void RequiresArgumentNotEmptyOrWhitespace(this string value, string argumentName)
    {
      if (value != null && value.IsNullOrWhiteSpace())
        throw new ArgumentException(argumentName + " can't be empty or contains only whitespace characters");
    }

    public static void RequiresArgumentNotNullAndNotEmpty(this string value, string argumentName)
    {
      if (string.IsNullOrEmpty(value))
        throw new ArgumentNullException(argumentName);
    }

    public static void RequiresArgumentNotNullAndNotWhiteSpace(
      this string value,
      string argumentName)
    {
      if (value.IsNullOrWhiteSpace())
        throw new ArgumentException(argumentName + " can't be null, empty or contains only whitespace characters");
    }

    public static void RequiresArgumentNotEmpty(this Guid guid, string argumentName)
    {
      if (guid == Guid.Empty)
        throw new ArgumentException(argumentName + " can't be empty");
    }
  }
}
