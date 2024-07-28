// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.UriQueryBuilder
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Azure.Storage.Core
{
  public class UriQueryBuilder
  {
    public UriQueryBuilder()
      : this((UriQueryBuilder) null)
    {
    }

    public UriQueryBuilder(UriQueryBuilder builder) => this.Parameters = builder != null ? (IDictionary<string, string>) new Dictionary<string, string>(builder.Parameters) : (IDictionary<string, string>) new Dictionary<string, string>();

    protected IDictionary<string, string> Parameters { get; private set; }

    public string this[string name]
    {
      get
      {
        string str;
        if (this.Parameters.TryGetValue(name, out str))
          return str;
        throw new KeyNotFoundException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "'{0}' key not found in the query builder.", (object) name));
      }
    }

    public virtual void Add(string name, string value)
    {
      if (value != null)
        value = Uri.EscapeDataString(value);
      this.Parameters.Add(name, value);
    }

    public void AddRange(
      IEnumerable<KeyValuePair<string, string>> parameters)
    {
      CommonUtility.AssertNotNull(nameof (parameters), (object) parameters);
      foreach (KeyValuePair<string, string> parameter in parameters)
        this.Add(parameter.Key, parameter.Value);
    }

    public bool ContainsQueryStringName(string name) => this.Parameters.ContainsKey(name);

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = true;
      foreach (KeyValuePair<string, string> parameter in (IEnumerable<KeyValuePair<string, string>>) this.Parameters)
      {
        if (flag)
        {
          flag = false;
          stringBuilder.Append("?");
        }
        else
          stringBuilder.Append("&");
        stringBuilder.Append(parameter.Key);
        if (parameter.Value != null)
          stringBuilder.AppendFormat("={0}", (object) parameter.Value);
      }
      return stringBuilder.ToString();
    }

    public StorageUri AddToUri(StorageUri storageUri)
    {
      CommonUtility.AssertNotNull(nameof (storageUri), (object) storageUri);
      return new StorageUri(this.AddToUri(storageUri.PrimaryUri), this.AddToUri(storageUri.SecondaryUri));
    }

    public virtual Uri AddToUri(Uri uri) => this.AddToUriCore(uri);

    protected Uri AddToUriCore(Uri uri)
    {
      if (uri == (Uri) null)
        return (Uri) null;
      string str = this.ToString();
      if (str.Length > 1)
        str = str.Substring(1);
      UriBuilder uriBuilder = new UriBuilder(uri);
      uriBuilder.Query = uriBuilder.Query == null || uriBuilder.Query.Length <= 1 ? str : uriBuilder.Query.Substring(1) + "&" + str;
      return uriBuilder.Uri;
    }
  }
}
