// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.HttpHeaderValueElement
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData
{
  public sealed class HttpHeaderValueElement
  {
    public HttpHeaderValueElement(
      string name,
      string value,
      IEnumerable<KeyValuePair<string, string>> parameters)
    {
      ExceptionUtils.CheckArgumentStringNotNullOrEmpty(name, nameof (name));
      ExceptionUtils.CheckArgumentNotNull<IEnumerable<KeyValuePair<string, string>>>(parameters, nameof (parameters));
      this.Name = name;
      this.Value = value;
      this.Parameters = parameters;
    }

    public string Name { get; private set; }

    public string Value { get; private set; }

    public IEnumerable<KeyValuePair<string, string>> Parameters { get; private set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      HttpHeaderValueElement.AppendNameValuePair(stringBuilder, this.Name, this.Value);
      foreach (KeyValuePair<string, string> parameter in this.Parameters)
      {
        stringBuilder.Append(";");
        HttpHeaderValueElement.AppendNameValuePair(stringBuilder, parameter.Key, parameter.Value);
      }
      return stringBuilder.ToString();
    }

    private static void AppendNameValuePair(StringBuilder stringBuilder, string name, string value)
    {
      stringBuilder.Append(name);
      if (value == null)
        return;
      stringBuilder.Append("=");
      stringBuilder.Append(value);
    }
  }
}
