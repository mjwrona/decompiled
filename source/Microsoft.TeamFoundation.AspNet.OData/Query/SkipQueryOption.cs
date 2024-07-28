// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.SkipQueryOption
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Query.Validators;
using Microsoft.OData;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Query
{
  public class SkipQueryOption
  {
    private int? _value;
    private ODataQueryOptionParser _queryOptionParser;

    public SkipQueryOption(
      string rawValue,
      ODataQueryContext context,
      ODataQueryOptionParser queryOptionParser)
    {
      if (context == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (context));
      if (string.IsNullOrEmpty(rawValue))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (rawValue));
      if (queryOptionParser == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (queryOptionParser));
      this.Context = context;
      this.RawValue = rawValue;
      this.Validator = SkipQueryValidator.GetSkipQueryValidator(context);
      this._queryOptionParser = queryOptionParser;
    }

    internal SkipQueryOption(string rawValue, ODataQueryContext context)
    {
      if (context == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (context));
      if (string.IsNullOrEmpty(rawValue))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (rawValue));
      this.Context = context;
      this.RawValue = rawValue;
      this.Validator = SkipQueryValidator.GetSkipQueryValidator(context);
      this._queryOptionParser = new ODataQueryOptionParser(context.Model, context.ElementType, context.NavigationSource, (IDictionary<string, string>) new Dictionary<string, string>()
      {
        {
          "$skip",
          rawValue
        }
      });
    }

    public ODataQueryContext Context { get; private set; }

    public string RawValue { get; private set; }

    public int Value
    {
      get
      {
        if (!this._value.HasValue)
        {
          long? skip = this._queryOptionParser.ParseSkip();
          if (skip.HasValue)
          {
            long? nullable = skip;
            long maxValue = (long) int.MaxValue;
            if (nullable.GetValueOrDefault() > maxValue & nullable.HasValue)
              throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.SkipTopLimitExceeded, (object) int.MaxValue, (object) AllowedQueryOptions.Skip, (object) this.RawValue));
          }
          long? nullable1 = skip;
          this._value = nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?();
        }
        return this._value.Value;
      }
    }

    public SkipQueryValidator Validator { get; set; }

    public IQueryable<T> ApplyTo<T>(IQueryable<T> query, ODataQuerySettings querySettings) => (IQueryable<T>) (this.ApplyToCore((IQueryable) query, querySettings) as IOrderedQueryable<T>);

    public IQueryable ApplyTo(IQueryable query, ODataQuerySettings querySettings) => this.ApplyToCore(query, querySettings);

    public void Validate(ODataValidationSettings validationSettings)
    {
      if (validationSettings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (validationSettings));
      if (this.Validator == null)
        return;
      this.Validator.Validate(this, validationSettings);
    }

    private IQueryable ApplyToCore(IQueryable query, ODataQuerySettings querySettings)
    {
      if (this.Context.ElementClrType == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.ApplyToOnUntypedQueryOption, (object) "ApplyTo");
      return ExpressionHelpers.Skip(query, this.Value, query.ElementType, querySettings.EnableConstantParameterization);
    }
  }
}
