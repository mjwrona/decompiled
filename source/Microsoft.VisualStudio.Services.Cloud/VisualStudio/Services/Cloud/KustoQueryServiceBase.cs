// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.KustoQueryServiceBase
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public abstract class KustoQueryServiceBase : IVssFrameworkService
  {
    private static readonly string s_area = "Kusto";

    public IDataReader Query(
      IVssRequestContext requestContext,
      string[] requestedTables,
      string query)
    {
      return this.ExecuteQuery<IDataReader>(requestContext, requestedTables, query, new Func<IVssRequestContext, string, IDataReader>(this.GetKustoQueryProviderService(requestContext, KustoQueryConfig.DefaultConfig).ExecuteQuery));
    }

    public IDataReader Query(
      IVssRequestContext requestContext,
      string[] requestedTables,
      string query,
      KustoQueryConfig kustoQueryConfig)
    {
      return this.ExecuteQuery<IDataReader>(requestContext, requestedTables, query, new Func<IVssRequestContext, string, IDataReader>(this.GetKustoQueryProviderService(requestContext, kustoQueryConfig).ExecuteQuery));
    }

    public IEnumerable<T> Query<T>(
      IVssRequestContext requestContext,
      string[] requestedTables,
      string query,
      KustoQueryConfig kustoQueryConfig)
      where T : class
    {
      return this.ExecuteQuery<IEnumerable<T>>(requestContext, requestedTables, query, new Func<IVssRequestContext, string, IEnumerable<T>>(this.GetKustoQueryProviderService(requestContext, kustoQueryConfig).ExecuteQuery<T>));
    }

    protected bool ReadDataReader(
      IVssRequestContext requestContext,
      KustoQueryConfig kustoQueryConfig,
      IDataReader dataReader)
    {
      return this.GetKustoQueryProviderService(requestContext, kustoQueryConfig).ReadDataReader(dataReader);
    }

    protected abstract string Layer { get; }

    public abstract void ServiceStart(IVssRequestContext systemRequestContext);

    public abstract void ServiceEnd(IVssRequestContext systemRequestContext);

    protected virtual KustoQueryRestriction GetQueryRestrictions(
      IVssRequestContext requestContext,
      string[] requestedTables)
    {
      return (KustoQueryRestriction) null;
    }

    private TResult ExecuteQuery<TResult>(
      IVssRequestContext requestContext,
      string[] requestedTables,
      string query,
      Func<IVssRequestContext, string, TResult> executeQuery)
    {
      try
      {
        this.ApplyQueryRestrictions(requestContext, requestedTables, ref query);
        return executeQuery(requestContext.To(TeamFoundationHostType.Deployment), query);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15103100, KustoQueryServiceBase.s_area, this.Layer, ex);
        throw;
      }
    }

    private void ApplyQueryRestrictions(
      IVssRequestContext requestContext,
      string[] requestedTables,
      ref string query)
    {
      KustoQueryRestriction queryRestrictions = this.GetQueryRestrictions(requestContext, requestedTables);
      if (queryRestrictions == null)
        return;
      query = queryRestrictions.Apply(query);
    }

    private IKustoQueryProviderService GetKustoQueryProviderService(
      IVssRequestContext requestContext,
      KustoQueryConfig kustoQueryConfig)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      switch (kustoQueryConfig.KustoClusterDestination)
      {
        case KustoClusterDestination.VSO:
          return context.GetService<IKustoQueryProviderService>();
        case KustoClusterDestination.UNION:
          return (IKustoQueryProviderService) context.GetService<KustoQueryProviderServiceUnion>();
        default:
          throw new InvalidOperationException(string.Format("Kusto cluster destination '{0}' is not supported", (object) kustoQueryConfig.KustoClusterDestination));
      }
    }
  }
}
