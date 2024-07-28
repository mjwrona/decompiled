// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationServiceException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Runtime.Serialization;
using System.Security;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class TeamFoundationServiceException : TeamFoundationServerException, ISerializable
  {
    private XmlQualifiedName m_faultCode = TeamFoundationServiceException.s_faultCode;
    private HttpStatusCode m_statusCode = HttpStatusCode.InternalServerError;
    private static readonly XmlQualifiedName s_faultCode = new XmlQualifiedName("Receiver", "http://www.w3.org/2003/05/soap-envelope");

    public TeamFoundationServiceException()
    {
    }

    public TeamFoundationServiceException(int errorCode)
      : this(errorCode, false)
    {
    }

    public TeamFoundationServiceException(int errorCode, bool logException)
    {
      this.ErrorCode = errorCode;
      this.LogException = logException;
    }

    public TeamFoundationServiceException(string message)
      : base(message)
    {
    }

    public TeamFoundationServiceException(string message, int errorCode)
      : this(message, errorCode, false)
    {
    }

    public TeamFoundationServiceException(string message, HttpStatusCode httpStatusCode)
      : this(message, httpStatusCode, false)
    {
    }

    public TeamFoundationServiceException(string message, int errorCode, bool logException)
      : this(message)
    {
      this.ErrorCode = errorCode;
      this.LogException = logException;
    }

    public TeamFoundationServiceException(
      string message,
      HttpStatusCode httpStatusCode,
      bool logException)
      : this(message)
    {
      this.HttpStatusCode = httpStatusCode;
      this.LogException = logException;
    }

    public TeamFoundationServiceException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public TeamFoundationServiceException(string message, int errorCode, Exception innerException)
      : this(message, errorCode, false, innerException)
    {
    }

    public TeamFoundationServiceException(
      string message,
      HttpStatusCode httpStatusCode,
      Exception innerException)
      : this(message, httpStatusCode, false, innerException)
    {
    }

    public TeamFoundationServiceException(
      string message,
      int errorCode,
      bool logException,
      Exception innerException)
      : this(message, innerException)
    {
      this.ErrorCode = errorCode;
      this.LogException = logException;
    }

    public TeamFoundationServiceException(
      string message,
      HttpStatusCode httpStatusCode,
      bool logException,
      Exception innerException)
      : this(message, innerException)
    {
      this.HttpStatusCode = httpStatusCode;
      this.LogException = logException;
    }

    public virtual void GetExceptionProperties(ExceptionPropertyCollection properties)
    {
    }

    protected TeamFoundationServiceException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.FaultCode = (XmlQualifiedName) info.GetValue(nameof (m_faultCode), typeof (XmlQualifiedName));
    }

    [SecurityCritical]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("m_faultCode", (object) this.FaultCode);
      info.AddValue("m_httpStatusCode", (object) this.HttpStatusCode);
    }

    public IVssRequestContext RequestContext { get; set; }

    public virtual string SerializedExceptionName => this.GetType().Name;

    public XmlQualifiedName FaultCode
    {
      get => this.m_faultCode;
      protected set => this.m_faultCode = value;
    }

    public HttpStatusCode HttpStatusCode
    {
      get => this.m_statusCode;
      protected set => this.m_statusCode = value;
    }

    public static string ExtractString(SqlError error, string key) => error.ExtractString(key);

    public static List<string> ExtractStrings(SqlError error, string key) => error.ExtractStrings(key);

    public static int ExtractInt(SqlError error, string key) => error.ExtractInt(key);

    public static List<int> ExtractInts(SqlError error, string key) => error.ExtractInts(key);

    public static long ExtractLong(SqlError error, string key) => error.ExtractLong(key);

    public static string ExtractEnumName(SqlError error, string key, Type enumerationType) => error.ExtractEnumName(key, enumerationType);
  }
}
