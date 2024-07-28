// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlExceptionFactory
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SqlExceptionFactory
  {
    private Type m_exceptionType;
    private Func<IVssRequestContext, int, SqlException, SqlError, Exception> m_factoryMethod;
    private ConstructorInfo m_constructorMethod;
    private SqlExceptionFactory.ConstructorType m_constructorType;
    private const string s_Area = "Sql";
    private const string s_Layer = "SqlExceptionFactory";

    public SqlExceptionFactory(Type exceptionType)
    {
      this.m_exceptionType = exceptionType;
      foreach (ConstructorInfo constructor in exceptionType.GetConstructors())
      {
        ParameterInfo[] parameters = constructor.GetParameters();
        if (parameters.Length == 1)
        {
          if (this.m_constructorType < SqlExceptionFactory.ConstructorType.Message && parameters[0].ParameterType.IsAssignableFrom(typeof (string)))
          {
            this.m_constructorType = SqlExceptionFactory.ConstructorType.Message;
            this.m_constructorMethod = constructor;
          }
        }
        else if (parameters.Length == 2)
        {
          if (this.m_constructorType < SqlExceptionFactory.ConstructorType.MessageInnerException && parameters[0].ParameterType.IsAssignableFrom(typeof (string)) && parameters[1].ParameterType.IsAssignableFrom(typeof (Exception)))
          {
            this.m_constructorType = SqlExceptionFactory.ConstructorType.MessageInnerException;
            this.m_constructorMethod = constructor;
          }
        }
        else if (parameters.Length == 3)
        {
          if (this.m_constructorType < SqlExceptionFactory.ConstructorType.MessageInnerExceptionErrorNum && parameters[0].ParameterType.IsAssignableFrom(typeof (string)) && parameters[1].ParameterType.IsAssignableFrom(typeof (Exception)) && parameters[2].ParameterType.IsAssignableFrom(typeof (int)))
          {
            this.m_constructorType = SqlExceptionFactory.ConstructorType.MessageInnerExceptionErrorNum;
            this.m_constructorMethod = constructor;
          }
          if (this.m_constructorType < SqlExceptionFactory.ConstructorType.ContextSqlExceptionSqlError && parameters[0].ParameterType.IsAssignableFrom(typeof (IVssRequestContext)) && parameters[1].ParameterType.IsAssignableFrom(typeof (SqlException)) && parameters[2].ParameterType.IsAssignableFrom(typeof (SqlError)))
          {
            this.m_constructorType = SqlExceptionFactory.ConstructorType.ContextSqlExceptionSqlError;
            this.m_constructorMethod = constructor;
          }
        }
        else if (parameters.Length == 4 && this.m_constructorType < SqlExceptionFactory.ConstructorType.ContextErrorNumSqlExceptionSqlError && parameters[0].ParameterType.IsAssignableFrom(typeof (IVssRequestContext)) && parameters[1].ParameterType.IsAssignableFrom(typeof (int)) && parameters[2].ParameterType.IsAssignableFrom(typeof (SqlException)) && parameters[3].ParameterType.IsAssignableFrom(typeof (SqlError)))
        {
          this.m_constructorType = SqlExceptionFactory.ConstructorType.ContextErrorNumSqlExceptionSqlError;
          this.m_constructorMethod = constructor;
        }
      }
    }

    public SqlExceptionFactory(
      Type exceptionType,
      Func<IVssRequestContext, int, SqlException, SqlError, Exception> factoryMethod)
    {
      this.m_exceptionType = exceptionType;
      this.m_factoryMethod = factoryMethod;
    }

    public Exception Create(
      IVssRequestContext requestContext,
      int errorNumber,
      SqlException sqlException,
      SqlError sqlError)
    {
      Exception exception = (Exception) null;
      if (this.m_factoryMethod != null)
        exception = this.m_factoryMethod(requestContext, errorNumber, sqlException, sqlError);
      else if (this.m_constructorType != SqlExceptionFactory.ConstructorType.None)
      {
        switch (this.m_constructorType)
        {
          case SqlExceptionFactory.ConstructorType.Message:
            exception = (Exception) this.m_constructorMethod.Invoke(new object[1]
            {
              (object) sqlException.Message
            });
            break;
          case SqlExceptionFactory.ConstructorType.MessageInnerException:
            exception = (Exception) this.m_constructorMethod.Invoke(new object[2]
            {
              (object) sqlException.Message,
              (object) sqlException
            });
            break;
          case SqlExceptionFactory.ConstructorType.MessageInnerExceptionErrorNum:
            exception = (Exception) this.m_constructorMethod.Invoke(new object[3]
            {
              (object) sqlException.Message,
              (object) sqlException,
              (object) errorNumber
            });
            break;
          case SqlExceptionFactory.ConstructorType.ContextSqlExceptionSqlError:
            exception = (Exception) this.m_constructorMethod.Invoke(new object[3]
            {
              (object) requestContext,
              (object) sqlException,
              (object) sqlError
            });
            break;
          case SqlExceptionFactory.ConstructorType.ContextErrorNumSqlExceptionSqlError:
            exception = (Exception) this.m_constructorMethod.Invoke(new object[4]
            {
              (object) requestContext,
              (object) errorNumber,
              (object) sqlException,
              (object) sqlError
            });
            break;
        }
      }
      else
        requestContext.Trace(0, TraceLevel.Error, "Sql", nameof (SqlExceptionFactory), "{0} is mapped to SQL error number {1} but does not have the proper constructor.", (object) this.m_exceptionType.AssemblyQualifiedName, (object) errorNumber);
      return exception;
    }

    private enum ConstructorType
    {
      None,
      Message,
      MessageInnerException,
      MessageInnerExceptionErrorNum,
      ContextSqlExceptionSqlError,
      ContextErrorNumSqlExceptionSqlError,
    }
  }
}
