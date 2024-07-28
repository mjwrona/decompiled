// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CsvConfiguration`1
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Common
{
  public class CsvConfiguration<T>
  {
    private readonly HashSet<string> m_ignoredProperties;
    private const char c_comma = ',';
    private const char c_quote = '"';
    private const char c_newLine = '\n';
    private const char c_return = '\r';
    private const string c_doubleQuoteString = "\"\"";
    private const string c_CRLF = "\r\n";

    public bool UseHeader { get; set; } = true;

    public bool QuoteAllFields { get; set; }

    public bool Trim { get; set; }

    public bool UseInvariantCulture { get; set; }

    public char Delimiter => ',';

    public string DelimiterString => ','.ToString();

    public string QuoteString => '"'.ToString();

    public string DoubleQuoteString => "\"\"";

    public string NewLineString => '\n'.ToString();

    public string CRLFString => "\r\n";

    public char[] QuoteRequiredChars => new char[4]
    {
      '\r',
      '\n',
      '"',
      ','
    };

    public IReadOnlyDictionary<int, PropertyInfo> IndexToProperty { get; private set; }

    public IReadOnlyDictionary<int, string> IndexToColumnName { get; private set; }

    public int ColumnCount { get; private set; }

    public BindingFlags IncludedColumnTypes { get; set; } = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

    public CsvConfiguration(HashSet<string> ignoredProperties = null)
    {
      this.m_ignoredProperties = ignoredProperties ?? new HashSet<string>();
      this.ValidateTypeConfiguration();
    }

    private bool IsPropertyIgnored(string propertyName) => this.m_ignoredProperties.Contains(propertyName);

    private void ValidateTypeConfiguration()
    {
      Dictionary<int, PropertyInfo> dictionary1 = new Dictionary<int, PropertyInfo>();
      Dictionary<int, string> dictionary2 = new Dictionary<int, string>();
      foreach (PropertyInfo property in typeof (T).GetProperties(this.IncludedColumnTypes))
      {
        DataMemberAttribute customAttribute = property.GetCustomAttribute<DataMemberAttribute>();
        if (customAttribute != null && !this.IsPropertyIgnored(property.Name))
        {
          string str = customAttribute.Name ?? property.Name;
          dictionary2.Add(this.ColumnCount, str);
          dictionary1.Add(this.ColumnCount, property);
          ++this.ColumnCount;
        }
      }
      this.IndexToProperty = (IReadOnlyDictionary<int, PropertyInfo>) dictionary1;
      this.IndexToColumnName = (IReadOnlyDictionary<int, string>) dictionary2;
    }
  }
}
