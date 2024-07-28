// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure.SqlResourceComponentInterceptor
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure
{
  public class SqlResourceComponentInterceptor : 
    DbCommandInterceptor,
    IDbConnectionInterceptor,
    IDbInterceptor
  {
    public static readonly SqlResourceComponentInterceptor Instance = new SqlResourceComponentInterceptor();

    public override void ReaderExecuting(
      DbCommand command,
      DbCommandInterceptionContext<DbDataReader> interceptionContext)
    {
      if (!(interceptionContext.DbContexts.FirstOrDefault<DbContext>() is IComponentContext componentContext))
        throw new NotSupportedException(AnalyticsResources.SQL_COMMAND_NOT_BOUND_TO_CONTEXT((object) command.CommandText));
      if (componentContext.Component != null)
      {
        SqlDataReader sqlDataReader = componentContext.Component.ExecuteContextCommand(command as SqlCommand);
        interceptionContext.Result = (DbDataReader) sqlDataReader;
      }
      else
      {
        if (command.Connection.State == ConnectionState.Closed)
          command.Connection.Open();
        base.ReaderExecuting(command, interceptionContext);
      }
    }

    public void BeganTransaction(
      DbConnection connection,
      BeginTransactionInterceptionContext interceptionContext)
    {
    }

    public void BeginningTransaction(
      DbConnection connection,
      BeginTransactionInterceptionContext interceptionContext)
    {
    }

    public void Closed(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext)
    {
    }

    public void Closing(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext)
    {
    }

    public void ConnectionStringGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    public void ConnectionStringGot(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    public void ConnectionStringSet(
      DbConnection connection,
      DbConnectionPropertyInterceptionContext<string> interceptionContext)
    {
    }

    public void ConnectionStringSetting(
      DbConnection connection,
      DbConnectionPropertyInterceptionContext<string> interceptionContext)
    {
      if (interceptionContext.Value != null && !interceptionContext.Value.Contains("AnalyticsContext"))
        return;
      interceptionContext.SuppressExecution();
    }

    public void ConnectionTimeoutGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<int> interceptionContext)
    {
    }

    public void ConnectionTimeoutGot(
      DbConnection connection,
      DbConnectionInterceptionContext<int> interceptionContext)
    {
    }

    public void DatabaseGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    public void DatabaseGot(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    public void DataSourceGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    public void DataSourceGot(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    public void Disposed(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext)
    {
    }

    public void Disposing(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext)
    {
    }

    public void EnlistedTransaction(
      DbConnection connection,
      EnlistTransactionInterceptionContext interceptionContext)
    {
    }

    public void EnlistingTransaction(
      DbConnection connection,
      EnlistTransactionInterceptionContext interceptionContext)
    {
    }

    public void Opened(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext)
    {
    }

    public void Opening(
      DbConnection connection,
      DbConnectionInterceptionContext interceptionContext)
    {
      interceptionContext.SuppressExecution();
    }

    public void ServerVersionGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
      interceptionContext.Result = "2012";
    }

    public void ServerVersionGot(
      DbConnection connection,
      DbConnectionInterceptionContext<string> interceptionContext)
    {
    }

    public void StateGetting(
      DbConnection connection,
      DbConnectionInterceptionContext<ConnectionState> interceptionContext)
    {
      interceptionContext.Result = ConnectionState.Open;
    }

    public void StateGot(
      DbConnection connection,
      DbConnectionInterceptionContext<ConnectionState> interceptionContext)
    {
    }
  }
}
