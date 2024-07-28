// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.SoapSerializableException
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Net;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class SoapSerializableException : TeamFoundationServiceException
  {
    public SoapSerializableException()
    {
    }

    public SoapSerializableException(int errorCode)
      : base(errorCode)
    {
    }

    public SoapSerializableException(int errorCode, bool logException)
      : base(errorCode, logException)
    {
    }

    public SoapSerializableException(string message)
      : base(message)
    {
    }

    public SoapSerializableException(string message, int errorCode)
      : base(message, errorCode)
    {
    }

    public SoapSerializableException(string message, HttpStatusCode httpStatusCode)
      : base(message, httpStatusCode)
    {
    }

    public SoapSerializableException(string message, int errorCode, bool logException)
      : base(message, errorCode, logException)
    {
    }

    public SoapSerializableException(
      string message,
      HttpStatusCode httpStatusCode,
      bool logException)
      : base(message, httpStatusCode, logException)
    {
    }

    public SoapSerializableException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public SoapSerializableException(string message, int errorCode, Exception innerException)
      : base(message, errorCode, innerException)
    {
    }

    public SoapSerializableException(
      string message,
      HttpStatusCode httpStatusCode,
      Exception innerException)
      : base(message, httpStatusCode, innerException)
    {
    }

    public SoapSerializableException(
      string message,
      int errorCode,
      bool logException,
      Exception innerException)
      : base(message, errorCode, logException, innerException)
    {
    }

    public SoapSerializableException(
      string message,
      HttpStatusCode httpStatusCode,
      bool logException,
      Exception innerException)
      : base(message, httpStatusCode, logException, innerException)
    {
    }

    public virtual SoapException ToSoapException() => TeamFoundationServiceExceptionExtensions.ToSoapException(this);
  }
}
