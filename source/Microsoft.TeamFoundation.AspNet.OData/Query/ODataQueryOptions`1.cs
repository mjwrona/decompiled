// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.ODataQueryOptions`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.AspNet.OData.Query
{
  [ODataQueryParameterBinding]
  public class ODataQueryOptions<TEntity> : ODataQueryOptions
  {
    public ODataQueryOptions(ODataQueryContext context, HttpRequestMessage request)
      : base(context, request)
    {
      if (this.Context.ElementClrType == (Type) null)
        throw Error.Argument(nameof (context), SRResources.ElementClrTypeNull, (object) typeof (ODataQueryContext).Name);
      if (context.ElementClrType != typeof (TEntity))
        throw Error.Argument(nameof (context), SRResources.EntityTypeMismatch, (object) context.ElementClrType.FullName, (object) typeof (TEntity).FullName);
    }

    public ETag<TEntity> IfMatch => base.IfMatch as ETag<TEntity>;

    public ETag<TEntity> IfNoneMatch => base.IfNoneMatch as ETag<TEntity>;

    internal override ETag GetETag(EntityTagHeaderValue etagHeaderValue) => this.InternalRequest.GetETag<TEntity>(etagHeaderValue);

    public override IQueryable ApplyTo(IQueryable query)
    {
      ODataQueryOptions<TEntity>.ValidateQuery(query);
      return base.ApplyTo(query);
    }

    public override IQueryable ApplyTo(IQueryable query, ODataQuerySettings querySettings)
    {
      ODataQueryOptions<TEntity>.ValidateQuery(query);
      return base.ApplyTo(query, querySettings);
    }

    private static void ValidateQuery(IQueryable query)
    {
      if (query == null)
        throw Error.ArgumentNull(nameof (query));
      if (!TypeHelper.IsTypeAssignableFrom(typeof (TEntity), query.ElementType))
        throw Error.Argument(nameof (query), SRResources.CannotApplyODataQueryOptionsOfT, (object) typeof (ODataQueryOptions).Name, (object) typeof (TEntity).FullName, (object) typeof (IQueryable).Name, (object) query.ElementType.FullName);
    }
  }
}
