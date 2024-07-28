// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.DefaultODataETagHandler
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Builder.Conventions;
using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;

namespace Microsoft.AspNet.OData.Formatter
{
  internal class DefaultODataETagHandler : IETagHandler
  {
    private const string NullLiteralInETag = "null";
    private const char Separator = ',';

    public EntityTagHeaderValue CreateETag(IDictionary<string, object> properties)
    {
      if (properties == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (properties));
      if (properties.Count == 0)
        return (EntityTagHeaderValue) null;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('"');
      bool flag = true;
      foreach (object obj in (IEnumerable<object>) properties.Values)
      {
        if (flag)
          flag = false;
        else
          stringBuilder.Append(',');
        string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(obj == null ? "null" : ConventionsHelpers.GetUriRepresentationForValue(obj)));
        stringBuilder.Append(base64String);
      }
      stringBuilder.Append('"');
      return new EntityTagHeaderValue(stringBuilder.ToString(), true);
    }

    public IDictionary<string, object> ParseETag(EntityTagHeaderValue etagHeaderValue)
    {
      string[] strArray = etagHeaderValue != null ? etagHeaderValue.Tag.Trim('"').Split(',') : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (etagHeaderValue));
      IDictionary<string, object> etag = (IDictionary<string, object>) new Dictionary<string, object>();
      for (int index = 0; index < strArray.Length; ++index)
      {
        object obj = ODataUriUtils.ConvertFromUriLiteral(Encoding.UTF8.GetString(Convert.FromBase64String(strArray[index])), ODataVersion.V4);
        if (obj is ODataNullValue)
          obj = (object) null;
        etag.Add(index.ToString((IFormatProvider) CultureInfo.InvariantCulture), obj);
      }
      return etag;
    }
  }
}
