// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataMediaType
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.OData
{
  [DebuggerDisplay("MediaType [{ToText()}]")]
  public sealed class ODataMediaType
  {
    private readonly IEnumerable<KeyValuePair<string, string>> parameters;
    private readonly string subType;
    private readonly string type;

    public ODataMediaType(string type, string subType)
      : this(type, subType, (IEnumerable<KeyValuePair<string, string>>) null)
    {
    }

    public ODataMediaType(
      string type,
      string subType,
      IEnumerable<KeyValuePair<string, string>> parameters)
    {
      this.type = type;
      this.subType = subType;
      this.parameters = parameters;
    }

    internal ODataMediaType(string type, string subType, KeyValuePair<string, string> parameter)
      : this(type, subType, (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[1]
      {
        parameter
      })
    {
    }

    public string SubType => this.subType;

    public string Type => this.type;

    public IEnumerable<KeyValuePair<string, string>> Parameters => this.parameters;

    internal string FullTypeName => this.type + "/" + this.subType;

    internal Encoding SelectEncoding()
    {
      if (this.parameters != null)
      {
        using (IEnumerator<string> enumerator = this.parameters.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (parameter => HttpUtils.CompareMediaTypeParameterNames("charset", parameter.Key))).Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (parameter => parameter.Value.Trim())).Where<string>((Func<string, bool>) (encodingName => encodingName.Length > 0)).GetEnumerator())
        {
          if (enumerator.MoveNext())
            return ODataMediaType.EncodingFromName(enumerator.Current);
        }
      }
      return HttpUtils.CompareMediaTypeNames("text", this.type) ? (!HttpUtils.CompareMediaTypeNames("xml", this.subType) ? MediaTypeUtils.MissingEncoding : (Encoding) null) : (HttpUtils.CompareMediaTypeNames("application", this.type) && HttpUtils.CompareMediaTypeNames("json", this.subType) ? MediaTypeUtils.FallbackEncoding : (Encoding) null);
    }

    internal string ToText() => this.ToText((Encoding) null);

    internal string ToText(Encoding encoding)
    {
      if (this.parameters == null || !this.parameters.Any<KeyValuePair<string, string>>())
      {
        string text = this.FullTypeName;
        if (encoding != null)
          text = text + ";" + "charset" + "=" + encoding.WebName;
        return text;
      }
      StringBuilder stringBuilder = new StringBuilder(this.FullTypeName);
      foreach (KeyValuePair<string, string> parameter in this.parameters)
      {
        if (!HttpUtils.CompareMediaTypeParameterNames("charset", parameter.Key))
        {
          stringBuilder.Append(";");
          stringBuilder.Append(parameter.Key);
          stringBuilder.Append("=");
          stringBuilder.Append(parameter.Value);
        }
      }
      if (encoding != null)
      {
        stringBuilder.Append(";");
        stringBuilder.Append("charset");
        stringBuilder.Append("=");
        stringBuilder.Append(encoding.WebName);
      }
      return stringBuilder.ToString();
    }

    private static Encoding EncodingFromName(string name) => HttpUtils.GetEncodingFromCharsetName(name) ?? throw new ODataException(Strings.MediaType_EncodingNotSupported((object) name));
  }
}
