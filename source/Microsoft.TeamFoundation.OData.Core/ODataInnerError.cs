// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataInnerError
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Microsoft.OData
{
  [DebuggerDisplay("{Message}")]
  public sealed class ODataInnerError
  {
    public ODataInnerError()
    {
      this.Properties = (IDictionary<string, ODataValue>) new Dictionary<string, ODataValue>();
      this.Properties.Add("message", (ODataValue) new ODataNullValue());
      this.Properties.Add("type", (ODataValue) new ODataNullValue());
      this.Properties.Add("stacktrace", (ODataValue) new ODataNullValue());
    }

    public ODataInnerError(Exception exception)
    {
      ExceptionUtils.CheckArgumentNotNull<Exception>(exception, nameof (exception));
      if (exception.InnerException != null)
        this.InnerError = new ODataInnerError(exception.InnerException);
      this.Properties = (IDictionary<string, ODataValue>) new Dictionary<string, ODataValue>();
      this.Properties.Add("message", exception.Message.ToODataValue() ?? (ODataValue) new ODataNullValue());
      this.Properties.Add("type", exception.GetType().FullName.ToODataValue() ?? (ODataValue) new ODataNullValue());
      this.Properties.Add("stacktrace", exception.StackTrace.ToODataValue() ?? (ODataValue) new ODataNullValue());
    }

    public ODataInnerError(IDictionary<string, ODataValue> properties)
    {
      ExceptionUtils.CheckArgumentNotNull<IDictionary<string, ODataValue>>(properties, nameof (properties));
      this.Properties = (IDictionary<string, ODataValue>) new Dictionary<string, ODataValue>(properties);
    }

    public IDictionary<string, ODataValue> Properties { get; private set; }

    public string Message
    {
      get => this.GetStringValue("message");
      set => this.SetStringValue("message", value);
    }

    public string TypeName
    {
      get => this.GetStringValue("type");
      set => this.SetStringValue("type", value);
    }

    public string StackTrace
    {
      get => this.GetStringValue("stacktrace");
      set => this.SetStringValue("stacktrace", value);
    }

    public ODataInnerError InnerError { get; set; }

    internal string ToJson()
    {
      StringBuilder sb = new StringBuilder();
      foreach (KeyValuePair<string, ODataValue> property in (IEnumerable<KeyValuePair<string, ODataValue>>) this.Properties)
      {
        if (!(property.Key == "message") && !(property.Key == "stacktrace") && !(property.Key == "type") && !(property.Key == "internalexception"))
        {
          sb.Append(",");
          sb.Append("\"").Append(property.Key).Append("\"").Append(":");
          ODataJsonWriterUtils.ODataValueToString(sb, property.Value);
        }
      }
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\"message\":\"{0}\",\"type\":\"{1}\",\"stacktrace\":\"{2}\",\"innererror\":{3}{4}}}", this.Message == null ? (object) "" : (object) JsonValueUtils.GetEscapedJsonString(this.Message), this.TypeName == null ? (object) "" : (object) JsonValueUtils.GetEscapedJsonString(this.TypeName), this.StackTrace == null ? (object) "" : (object) JsonValueUtils.GetEscapedJsonString(this.StackTrace), this.InnerError == null ? (object) "{}" : (object) this.InnerError.ToJson(), (object) sb.ToString());
    }

    private string GetStringValue(string propertyKey)
    {
      if (!this.Properties.ContainsKey(propertyKey))
        return string.Empty;
      return this.Properties[propertyKey].FromODataValue()?.ToString();
    }

    private void SetStringValue(string propertyKey, string value)
    {
      ODataValue odataValue = value == null ? (ODataValue) new ODataNullValue() : value.ToODataValue();
      if (!this.Properties.ContainsKey(propertyKey))
        this.Properties.Add(propertyKey, odataValue);
      else
        this.Properties[propertyKey] = odataValue;
    }
  }
}
