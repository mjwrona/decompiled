// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CsvWriter`1
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Common
{
  public class CsvWriter<T>
  {
    private readonly TextWriter m_writer;
    private bool m_hasRowBeenWritten;
    private const string c_invalidStartingChars = "=@+-";

    public CsvConfiguration<T> Configuration { get; }

    public CsvWriter(TextWriter writer, CsvConfiguration<T> configuration = null)
    {
      this.m_writer = writer;
      this.Configuration = configuration ?? new CsvConfiguration<T>();
    }

    public void WriteRecords(IEnumerable<T> records)
    {
      if (this.Configuration.UseHeader)
        this.WriteHeader();
      foreach (T record in records)
        this.WriteRecord(record);
    }

    private void WriteHeader()
    {
      if (this.m_hasRowBeenWritten)
        return;
      string input = this.SerializeHeader();
      if (string.IsNullOrEmpty(input))
        return;
      this.BlockHarmfulStartingCharacters(ref input);
      this.m_writer.WriteLine(input);
      this.m_hasRowBeenWritten = true;
    }

    private void WriteRecord(T record)
    {
      string input = this.SerializeRecord(record);
      if (string.IsNullOrEmpty(input))
        return;
      this.BlockHarmfulStartingCharacters(ref input);
      this.m_writer.WriteLine(input);
      this.m_hasRowBeenWritten = true;
    }

    private string SerializeHeader()
    {
      string[] strArray = new string[this.Configuration.ColumnCount];
      for (int key = 0; key < this.Configuration.ColumnCount; ++key)
      {
        string field = this.Configuration.IndexToColumnName[key];
        strArray[key] = this.SerializeField(field);
      }
      return string.Join(this.Configuration.DelimiterString, strArray);
    }

    private string SerializeRecord(T record)
    {
      if ((object) record == null)
        return (string) null;
      string[] strArray = new string[this.Configuration.ColumnCount];
      for (int key = 0; key < this.Configuration.ColumnCount; ++key)
      {
        object obj = this.Configuration.IndexToProperty[key].GetValue((object) record) ?? (object) string.Empty;
        string field = !(obj is ICollection) ? (!this.Configuration.UseInvariantCulture ? obj.ToString() : Convert.ToString(obj, (IFormatProvider) CultureInfo.InvariantCulture)) : JsonConvert.SerializeObject(obj);
        strArray[key] = this.SerializeField(field);
      }
      return string.Join(this.Configuration.DelimiterString, strArray);
    }

    private string SerializeField(string field)
    {
      bool shouldQuote = this.Configuration.QuoteAllFields;
      if (!string.IsNullOrEmpty(field) && this.Configuration.Trim)
        field = field.Trim();
      if (!string.IsNullOrEmpty(field))
        shouldQuote = shouldQuote || field.IndexOfAny(this.Configuration.QuoteRequiredChars) != -1 || field[0] == ' ' || field[field.Length - 1] == ' ';
      return this.SerializeField(field, shouldQuote);
    }

    private string SerializeField(string field, bool shouldQuote)
    {
      if (shouldQuote && !string.IsNullOrEmpty(field))
      {
        field = field.Replace(this.Configuration.QuoteString, this.Configuration.DoubleQuoteString);
        field = field.Replace(this.Configuration.NewLineString, this.Configuration.CRLFString);
      }
      if (shouldQuote)
        field = this.Configuration.QuoteString + field + this.Configuration.QuoteString;
      return field;
    }

    private void BlockHarmfulStartingCharacters(ref string input)
    {
      if (string.IsNullOrEmpty(input) || !"=@+-".Contains<char>(input[0]))
        return;
      input = "\t" + input;
    }
  }
}
