// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlAnalysisServiceConnectionStringBuilder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultProperty("DataSource")]
  public sealed class SqlAnalysisServiceConnectionStringBuilder : DbConnectionStringBuilder
  {
    public const string DataSourceKeyword = "Data Source";
    public const string InitialCatalogKeyword = "Initial Catalog";
    public const string IntegratedSecurityKeyword = "Integrated Security";
    public const string ConnectTimeoutKeyword = "Connect Timeout";
    public const string TimeoutKeyword = "Timeout";

    public SqlAnalysisServiceConnectionStringBuilder()
      : this((string) null)
    {
    }

    public SqlAnalysisServiceConnectionStringBuilder(string connectionString)
    {
      if (string.IsNullOrEmpty(connectionString))
        return;
      this.ConnectionString = connectionString;
    }

    public override object this[string keyword]
    {
      get => base[keyword];
      set => base[keyword] = this.FormattedValue(value, keyword);
    }

    public string DataSource
    {
      get => this.PullStringKeyword("Data Source");
      set => this["Data Source"] = (object) value;
    }

    public string InitialCatalog
    {
      get => this.PullStringKeyword("Initial Catalog");
      set => this["Initial Catalog"] = (object) value;
    }

    public SSASIntegratedSecurity? IntegratedSecurity
    {
      get
      {
        string str = this.PullStringKeyword("Integrated Security");
        if (string.IsNullOrEmpty(str))
          return new SSASIntegratedSecurity?();
        SSASIntegratedSecurity result;
        if (!Enum.TryParse<SSASIntegratedSecurity>(str, true, out result))
          throw new ArgumentException(FrameworkResources.UnknownSSASIntegratedSecurity((object) str));
        return new SSASIntegratedSecurity?(result);
      }
      set => this["Integrated Security"] = (object) value;
    }

    public int? ConnectTimeout
    {
      get => this.PullIntKeyword("Connect Timeout");
      set => this["Connect Timeout"] = (object) value;
    }

    public int? Timeout
    {
      get => this.PullIntKeyword(nameof (Timeout));
      set => this[nameof (Timeout)] = (object) value;
    }

    private string PullStringKeyword(string keyword)
    {
      object obj;
      this.TryGetValue(keyword, out obj);
      return obj == null ? (string) null : this.ToCultureInvariantString(obj);
    }

    private int? PullIntKeyword(string keyword)
    {
      string s = this.PullStringKeyword(keyword);
      if (string.IsNullOrEmpty(s))
        return new int?();
      int result;
      if (!int.TryParse(s, out result))
        throw new ArgumentException(FrameworkResources.ExpectedValueForKeywordToBeAnInteger((object) keyword));
      return new int?(result);
    }

    private object FormattedValue(object value, string keyword)
    {
      if (value == null)
        return (object) null;
      if (string.Equals("Integrated Security", keyword, StringComparison.OrdinalIgnoreCase))
      {
        if (value is SSASIntegratedSecurity integratedSecurity)
          return (object) integratedSecurity.ToString().ToUpperInvariant();
        SSASIntegratedSecurity result;
        if (!Enum.TryParse<SSASIntegratedSecurity>(value.ToString(), true, out result))
          throw new ArgumentException(FrameworkResources.UnknownSSASIntegratedSecurity(value));
        return (object) result.ToString().ToUpperInvariant();
      }
      if (!string.Equals("Connect Timeout", keyword, StringComparison.OrdinalIgnoreCase) && !string.Equals("Timeout", keyword, StringComparison.OrdinalIgnoreCase))
        return value;
      int result1;
      if (!int.TryParse(this.ToCultureInvariantString(value), out result1))
        throw new ArgumentException(FrameworkResources.ExpectedValueForKeywordToBeAnInteger((object) keyword));
      return (object) this.ToCultureInvariantString((object) result1);
    }

    private string ToCultureInvariantString(object value) => value is string ? (string) value : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", value);
  }
}
