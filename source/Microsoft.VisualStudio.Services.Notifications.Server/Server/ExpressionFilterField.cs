// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.ExpressionFilterField
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class ExpressionFilterField
  {
    private IEnumerable<string> m_values;
    private IDictionary<string, string> m_valuesLookup;
    private Func<IVssRequestContext, IDictionary<string, string>> m_getValuesLookupMethod;
    private Func<IVssRequestContext, IEnumerable<string>> m_getValuesMethod;

    public ExpressionFilterField(
      SubscriptionFieldType fieldType,
      string invariantFieldName,
      string localizedFieldName,
      IEnumerable<byte> rawOperators,
      Func<IVssRequestContext, IEnumerable<string>> getValuesMethod)
    {
      this.FieldType = fieldType;
      this.InvariantFieldName = invariantFieldName;
      this.LocalizedFieldName = localizedFieldName;
      object obj = (object) rawOperators;
      if (obj == null)
        obj = (object) new byte[2]{ (byte) 12, (byte) 13 };
      this.RawOperators = (IEnumerable<byte>) obj;
      this.m_getValuesMethod = getValuesMethod;
    }

    public ExpressionFilterField(
      SubscriptionFieldType fieldType,
      string invariantFieldName,
      string localizedFieldName,
      IEnumerable<byte> rawOperators,
      Func<IVssRequestContext, IDictionary<string, string>> getValuesLookupMethod)
    {
      this.FieldType = fieldType;
      this.InvariantFieldName = invariantFieldName;
      this.LocalizedFieldName = localizedFieldName;
      IEnumerable<byte> bytes;
      if (rawOperators != null)
        bytes = rawOperators;
      else
        bytes = (IEnumerable<byte>) new byte[2]
        {
          (byte) 12,
          (byte) 13
        };
      this.RawOperators = bytes;
      this.m_getValuesLookupMethod = getValuesLookupMethod;
    }

    public SubscriptionFieldType FieldType { get; private set; }

    public string InvariantFieldName { get; private set; }

    public string LocalizedFieldName { get; private set; }

    public List<string> SupportedScopes { get; set; }

    public IEnumerable<byte> RawOperators { get; private set; }

    public Func<IVssRequestContext, IDictionary<string, string>> GetValuesLookupMethod
    {
      get => this.m_getValuesLookupMethod;
      set => this.m_getValuesLookupMethod = value;
    }

    public IEnumerable<string> GetValues(IVssRequestContext requestContext)
    {
      if (this.m_values == null)
      {
        if (this.m_getValuesMethod != null)
          this.m_values = this.m_getValuesMethod(requestContext);
        else if (this.m_getValuesLookupMethod != null)
          this.m_values = (IEnumerable<string>) this.GetValuesLookup(requestContext).Values;
      }
      return this.m_values == null ? (IEnumerable<string>) Array.Empty<string>() : this.m_values;
    }

    private IDictionary<string, string> GetValuesLookup(IVssRequestContext requestContext)
    {
      if (this.m_valuesLookup == null && this.m_getValuesLookupMethod != null)
        this.m_valuesLookup = this.m_getValuesLookupMethod(requestContext);
      return this.m_valuesLookup;
    }

    public string GetLocalizedValue(IVssRequestContext requestContext, string invariantValue)
    {
      IDictionary<string, string> valuesLookup = this.GetValuesLookup(requestContext);
      string str;
      return valuesLookup != null && valuesLookup.TryGetValue(invariantValue, out str) ? str : this.PostProcessLocalizedInvariantValue(invariantValue);
    }

    public virtual string PostProcessLocalizedInvariantValue(string invariantValue) => invariantValue;

    public string GetInvariantValue(IVssRequestContext requestContext, string localizedValue)
    {
      IDictionary<string, string> valuesLookup = this.GetValuesLookup(requestContext);
      if (valuesLookup != null)
      {
        foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) valuesLookup)
        {
          if (string.Equals(keyValuePair.Value, localizedValue, StringComparison.CurrentCultureIgnoreCase))
            return keyValuePair.Key;
        }
      }
      else
      {
        string invariantValue = this.GetValues(requestContext).FirstOrDefault<string>((Func<string, bool>) (val => string.Equals(val, localizedValue, StringComparison.CurrentCultureIgnoreCase)));
        if (invariantValue != null)
          return invariantValue;
      }
      return localizedValue;
    }

    public Dictionary<string, object> ToJson(IVssRequestContext requestContext) => new Dictionary<string, object>()
    {
      {
        "invariantFieldName",
        (object) this.InvariantFieldName
      },
      {
        "fieldName",
        (object) this.LocalizedFieldName
      },
      {
        "fieldType",
        (object) this.FieldType
      },
      {
        "operators",
        (object) SubscriptionFilterOperators.GetLocalizedOperators(this.RawOperators)
      },
      {
        "values",
        (object) this.GetValues(requestContext)
      }
    };
  }
}
