// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.CountQueryOption
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Query.Validators;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Query
{
  public class CountQueryOption
  {
    private bool? _value;
    private ODataQueryOptionParser _queryOptionParser;

    public CountQueryOption(
      string rawValue,
      ODataQueryContext context,
      ODataQueryOptionParser queryOptionParser)
    {
      if (string.IsNullOrEmpty(rawValue))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (rawValue));
      if (context == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (context));
      if (queryOptionParser == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (queryOptionParser));
      this.Context = context;
      this.RawValue = rawValue;
      this.Validator = CountQueryValidator.GetCountQueryValidator(context);
      this._queryOptionParser = queryOptionParser;
    }

    internal CountQueryOption(string rawValue, ODataQueryContext context)
    {
      if (string.IsNullOrEmpty(rawValue))
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNullOrEmpty(nameof (rawValue));
      this.Context = context != null ? context : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (context));
      this.RawValue = rawValue;
      this.Validator = CountQueryValidator.GetCountQueryValidator(context);
      IEdmModel model = context.Model;
      IEdmType elementType = context.ElementType;
      IEdmNavigationSource navigationSource = context.NavigationSource;
      Dictionary<string, string> queryOptions = new Dictionary<string, string>();
      queryOptions.Add("$count", rawValue);
      IServiceProvider requestContainer = context.RequestContainer;
      this._queryOptionParser = new ODataQueryOptionParser(model, elementType, navigationSource, (IDictionary<string, string>) queryOptions, requestContainer);
    }

    public ODataQueryContext Context { get; private set; }

    public string RawValue { get; private set; }

    public bool Value
    {
      get
      {
        if (!this._value.HasValue)
          this._value = this._queryOptionParser.ParseCount();
        return this._value.Value;
      }
    }

    public CountQueryValidator Validator { get; set; }

    public void Validate(ODataValidationSettings validationSettings)
    {
      if (validationSettings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (validationSettings));
      if (this.Validator == null)
        return;
      this.Validator.Validate(this, validationSettings);
    }

    public long? GetEntityCount(IQueryable query)
    {
      if (this.Context.ElementClrType == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.ApplyToOnUntypedQueryOption, (object) nameof (GetEntityCount));
      return this.Value ? new long?(ExpressionHelpers.Count(query, this.Context.ElementClrType)()) : new long?();
    }

    internal Func<long> GetEntityCountFunc(IQueryable query)
    {
      if (this.Context.ElementClrType == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.NotSupported(SRResources.ApplyToOnUntypedQueryOption, (object) "GetEntityCount");
      return this.Value ? ExpressionHelpers.Count(query, this.Context.ElementClrType) : (Func<long>) null;
    }
  }
}
