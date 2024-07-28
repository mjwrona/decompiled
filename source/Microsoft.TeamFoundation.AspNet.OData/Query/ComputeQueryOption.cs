// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.ComputeQueryOption
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNet.OData.Query.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Query
{
  public class ComputeQueryOption
  {
    private ComputeClause _computeClause;
    private ODataQueryOptionParser _queryOptionParser;
    private readonly IWebApiAssembliesResolver _assembliesResolver;

    public ComputeQueryOption(
      string rawValue,
      ODataQueryContext context,
      ODataQueryOptionParser queryOptionParser)
    {
      if (context == null)
        throw Error.ArgumentNull(nameof (context));
      if (string.IsNullOrEmpty(rawValue))
        throw Error.ArgumentNullOrEmpty(nameof (rawValue));
      if (queryOptionParser == null)
        throw Error.ArgumentNull(nameof (queryOptionParser));
      this.Context = context;
      this.RawValue = rawValue;
      this.Validator = ComputeQueryValidator.GetComputeQueryValidator(context);
      this._queryOptionParser = queryOptionParser;
      IServiceProvider requestContainer = this.Context.RequestContainer;
      this._assembliesResolver = (requestContainer != null ? requestContainer.GetService<IWebApiAssembliesResolver>() : (IWebApiAssembliesResolver) null) ?? WebApiAssembliesResolver.Default;
    }

    internal ComputeQueryOption(string rawValue, ODataQueryContext context)
    {
      if (context == null)
        throw Error.ArgumentNull(nameof (context));
      if (string.IsNullOrEmpty(rawValue))
        throw Error.ArgumentNullOrEmpty(nameof (rawValue));
      this.Context = context;
      this.RawValue = rawValue;
      this.Validator = ComputeQueryValidator.GetComputeQueryValidator(context);
      this._queryOptionParser = new ODataQueryOptionParser(context.Model, context.ElementType, context.NavigationSource, (IDictionary<string, string>) new Dictionary<string, string>()
      {
        {
          "$compute",
          rawValue
        }
      });
    }

    public ODataQueryContext Context { get; private set; }

    public string RawValue { get; private set; }

    public ComputeQueryValidator Validator { get; set; }

    public ComputeClause ComputeClause
    {
      get
      {
        if (this._computeClause == null)
          this._computeClause = this._queryOptionParser.ParseCompute();
        return this._computeClause;
      }
    }

    internal IQueryable ApplyTo(IQueryable query, ODataQuerySettings querySettings)
    {
      if (query == null)
        throw Error.ArgumentNull(nameof (query));
      if (querySettings == null)
        throw Error.ArgumentNull(nameof (querySettings));
      if (this.Context.ElementClrType == (Type) null)
        throw Error.NotSupported(SRResources.ApplyToOnUntypedQueryOption, (object) nameof (ApplyTo));
      ComputeClause computeClause = this.ComputeClause;
      query = new Microsoft.AspNet.OData.Query.Expressions.ComputeBinder(this.Context.UpdateQuerySettings(querySettings, query), this._assembliesResolver, this.Context.ElementClrType, this.Context.Model, computeClause.ComputedItems).Bind(query);
      return query;
    }

    internal void Validate(ODataValidationSettings validationSettings)
    {
      if (validationSettings == null)
        throw Error.ArgumentNull(nameof (validationSettings));
      if (this.ComputeClause == null)
        throw Error.ArgumentNull("ComputeClause");
      if (this.Validator == null)
        return;
      this.Validator.Validate(this, validationSettings);
    }
  }
}
