// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ApiExceptionMapping
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ApiExceptionMapping
  {
    private readonly IDictionary<string, HttpStatusCode> m_statusCodeMapping;
    private readonly IDictionary<string, Type> m_translatedExceptions;

    public ApiExceptionMapping()
    {
      this.m_statusCodeMapping = (IDictionary<string, HttpStatusCode>) new Dictionary<string, HttpStatusCode>();
      this.m_translatedExceptions = (IDictionary<string, Type>) new Dictionary<string, Type>();
    }

    public void AddStatusCode<TException>(HttpStatusCode responseStatus) where TException : Exception => this.AddStatusCode(typeof (TException), responseStatus);

    public void AddStatusCode(Type exceptionType, HttpStatusCode responseStatus) => this.AddStatusCode(exceptionType.FullName, responseStatus);

    public void AddStatusCode(string exceptionTypeName, HttpStatusCode responseStatus) => this.m_statusCodeMapping[exceptionTypeName] = responseStatus;

    public void AddTranslation<TSourceException, TTargetException>() => this.AddTranslation(typeof (TSourceException), typeof (TTargetException));

    public void AddTranslation(Type sourceExceptionType, Type targetExceptionType) => this.AddTranslation(sourceExceptionType.FullName, targetExceptionType);

    public void AddTranslation(string sourceExceptionType, Type targetExceptionType) => this.m_translatedExceptions[sourceExceptionType] = targetExceptionType;

    public HttpStatusCode? GetStatusCode(Type exceptionType)
    {
      HttpStatusCode httpStatusCode;
      return this.m_statusCodeMapping.TryGetValue(exceptionType.FullName, out httpStatusCode) ? new HttpStatusCode?(httpStatusCode) : new HttpStatusCode?();
    }

    public Exception TranslateException(Exception exception)
    {
      Type type;
      if (exception == null || !this.m_translatedExceptions.TryGetValue(exception.GetType().FullName, out type))
        return exception;
      return Activator.CreateInstance(type, (object) exception.Message) as Exception;
    }
  }
}
