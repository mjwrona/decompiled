// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.SkipTokenQueryOption
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Query.Validators;
using Microsoft.OData.UriParser;
using System.Linq;

namespace Microsoft.AspNet.OData.Query
{
  public class SkipTokenQueryOption
  {
    private SkipTokenHandler skipTokenHandler;

    public SkipTokenQueryOption(
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
      this.RawValue = rawValue;
      this.Validator = context.GetSkipTokenQueryValidator();
      this.skipTokenHandler = context.GetSkipTokenHandler();
      this.Context = context;
    }

    public string RawValue { get; private set; }

    public ODataQueryContext Context { get; private set; }

    public SkipTokenQueryValidator Validator { get; }

    public ODataQuerySettings QuerySettings { get; private set; }

    public ODataQueryOptions QueryOptions { get; private set; }

    public virtual IQueryable<T> ApplyTo<T>(
      IQueryable<T> query,
      ODataQuerySettings querySettings,
      ODataQueryOptions queryOptions)
    {
      this.QuerySettings = querySettings;
      this.QueryOptions = queryOptions;
      return (IQueryable<T>) (this.skipTokenHandler.ApplyTo<T>(query, this) as IOrderedQueryable<T>);
    }

    public virtual IQueryable ApplyTo(
      IQueryable query,
      ODataQuerySettings querySettings,
      ODataQueryOptions queryOptions)
    {
      this.QuerySettings = querySettings;
      this.QueryOptions = queryOptions;
      return this.skipTokenHandler.ApplyTo(query, this);
    }

    public void Validate(ODataValidationSettings validationSettings)
    {
      if (validationSettings == null)
        throw Error.ArgumentNull(nameof (validationSettings));
      if (this.Validator == null)
        return;
      this.Validator.Validate(this, validationSettings);
    }
  }
}
