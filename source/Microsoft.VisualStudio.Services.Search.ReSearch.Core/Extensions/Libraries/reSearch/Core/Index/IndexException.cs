// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Index.IndexException
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.IO;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Index
{
  [Serializable]
  public class IndexException : Exception
  {
    public IndexException()
    {
    }

    public IndexException(string message)
      : base(message)
    {
    }

    public IndexException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected IndexException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public static void Critical(bool condition)
    {
      if (!condition)
        throw new IndexException("Critial assertion triggered.");
    }

    public static void Assert(bool condition)
    {
      if (IoHelper.CheckedIoEnabled && !condition)
        throw new IndexException("Assertion triggered.");
    }

    public static void Assert(bool condition, string message, params object[] args)
    {
      if (!condition)
        throw new IndexException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Assertion triggered: {0}", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, args)));
    }

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "expected")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "args")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "actual")]
    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "message")]
    public static void AssertEqual<T>(T expected, T actual, string message, params object[] args)
    {
    }

    public static void AssertEqual<T>(T expected, T actual)
    {
      if (IoHelper.CheckedIoEnabled && !object.Equals((object) expected, (object) actual))
        throw new IndexException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Assertion: Expected='{0}' Actual='{1}'", (object) expected, (object) actual));
    }
  }
}
