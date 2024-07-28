// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Common.Error
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.AspNet.OData.Common
{
  internal static class Error
  {
    private const string HttpScheme = "http";
    private const string HttpsScheme = "https";

    internal static string Format(string format, params object[] args) => string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);

    internal static ArgumentException Argument(string messageFormat, params object[] messageArgs) => new ArgumentException(Error.Format(messageFormat, messageArgs));

    internal static ArgumentException Argument(
      string parameterName,
      string messageFormat,
      params object[] messageArgs)
    {
      return new ArgumentException(Error.Format(messageFormat, messageArgs), parameterName);
    }

    internal static ArgumentException ArgumentUriNotHttpOrHttpsScheme(
      string parameterName,
      Uri actualValue)
    {
      return new ArgumentException(Error.Format(CommonWebApiResources.ArgumentInvalidHttpUriScheme, (object) actualValue, (object) "http", (object) "https"), parameterName);
    }

    internal static ArgumentException ArgumentUriNotAbsolute(string parameterName, Uri actualValue) => new ArgumentException(Error.Format(CommonWebApiResources.ArgumentInvalidAbsoluteUri, (object) actualValue), parameterName);

    internal static ArgumentException ArgumentUriHasQueryOrFragment(
      string parameterName,
      Uri actualValue)
    {
      return new ArgumentException(Error.Format(CommonWebApiResources.ArgumentUriHasQueryOrFragment, (object) actualValue), parameterName);
    }

    internal static ArgumentNullException PropertyNull() => new ArgumentNullException("value");

    internal static ArgumentNullException ArgumentNull(string parameterName) => new ArgumentNullException(parameterName);

    internal static ArgumentNullException ArgumentNull(
      string parameterName,
      string messageFormat,
      params object[] messageArgs)
    {
      return new ArgumentNullException(parameterName, Error.Format(messageFormat, messageArgs));
    }

    internal static ArgumentException ArgumentNullOrEmpty(string parameterName) => Error.Argument(parameterName, CommonWebApiResources.ArgumentNullOrEmpty, (object) parameterName);

    internal static ArgumentOutOfRangeException ArgumentOutOfRange(
      string parameterName,
      object actualValue,
      string messageFormat,
      params object[] messageArgs)
    {
      return new ArgumentOutOfRangeException(parameterName, actualValue, Error.Format(messageFormat, messageArgs));
    }

    internal static ArgumentOutOfRangeException ArgumentMustBeGreaterThanOrEqualTo(
      string parameterName,
      object actualValue,
      object minValue)
    {
      return new ArgumentOutOfRangeException(parameterName, actualValue, Error.Format(CommonWebApiResources.ArgumentMustBeGreaterThanOrEqualTo, minValue));
    }

    internal static ArgumentOutOfRangeException ArgumentMustBeLessThanOrEqualTo(
      string parameterName,
      object actualValue,
      object maxValue)
    {
      return new ArgumentOutOfRangeException(parameterName, actualValue, Error.Format(CommonWebApiResources.ArgumentMustBeLessThanOrEqualTo, maxValue));
    }

    internal static KeyNotFoundException KeyNotFound() => new KeyNotFoundException();

    internal static KeyNotFoundException KeyNotFound(
      string messageFormat,
      params object[] messageArgs)
    {
      return new KeyNotFoundException(Error.Format(messageFormat, messageArgs));
    }

    internal static ObjectDisposedException ObjectDisposed(
      string messageFormat,
      params object[] messageArgs)
    {
      return new ObjectDisposedException((string) null, Error.Format(messageFormat, messageArgs));
    }

    internal static OperationCanceledException OperationCanceled() => new OperationCanceledException();

    internal static OperationCanceledException OperationCanceled(
      string messageFormat,
      params object[] messageArgs)
    {
      return new OperationCanceledException(Error.Format(messageFormat, messageArgs));
    }

    internal static ArgumentException InvalidEnumArgument(
      string parameterName,
      int invalidValue,
      Type enumClass)
    {
      return (ArgumentException) new InvalidEnumArgumentException(parameterName, invalidValue, enumClass);
    }

    internal static InvalidOperationException InvalidOperation(
      string messageFormat,
      params object[] messageArgs)
    {
      return new InvalidOperationException(Error.Format(messageFormat, messageArgs));
    }

    internal static InvalidOperationException InvalidOperation(
      Exception innerException,
      string messageFormat,
      params object[] messageArgs)
    {
      return new InvalidOperationException(Error.Format(messageFormat, messageArgs), innerException);
    }

    internal static NotSupportedException NotSupported(
      string messageFormat,
      params object[] messageArgs)
    {
      return new NotSupportedException(Error.Format(messageFormat, messageArgs));
    }
  }
}
