// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.Column
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class Column
  {
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("isnullable")]
    public bool IsNullable { get; set; }

    [XmlAttribute("isreplicated")]
    public bool IsReplicated { get; set; }

    [XmlAttribute("isnotforreplication")]
    public bool IsNotForReplication { get; set; }

    [XmlAttribute("isansipadded")]
    public bool IsAnsiPadded { get; set; }

    [XmlAttribute("isrowguid")]
    public bool IsRowGuid { get; set; }

    [XmlAttribute("default")]
    public string Default { get; set; }

    [XmlAttribute("datatype")]
    public string DataType { get; set; }

    [XmlAttribute("maximumlength")]
    public int MaximumLength { get; set; }

    [XmlAttribute("issparse")]
    public bool IsSparse { get; set; }

    public string CollationName { get; set; }

    public byte Precision { get; set; }

    public byte Scale { get; set; }

    public bool IsIdentity { get; set; }

    public long SeedValue { get; set; }

    public long IncrementValue { get; set; }

    public bool IsComputed { get; set; }

    public string ComputedDefinition { get; set; }

    public bool IsPersisted { get; set; }

    public bool IsUserDefined { get; set; }

    public bool IsTimestamp => "timestamp".Equals(this.DataType, StringComparison.OrdinalIgnoreCase);

    public string ScriptDdl(int offset)
    {
      string str = new string(' ', offset);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("{0}{1}", (object) str, (object) StringUtil.QuoteName(this.Name));
      if (this.IsComputed)
      {
        stringBuilder.Append(" AS ").Append(this.ComputedDefinition);
        if (this.IsPersisted)
        {
          stringBuilder.Append(" PERSISTED");
          if (!this.IsNullable)
            stringBuilder.Append(" NOT NULL");
        }
      }
      else
      {
        stringBuilder.Append(this.GetDataType());
        if (this.IsIdentity)
        {
          stringBuilder.AppendFormat(" IDENTITY({0}, {1})", (object) this.SeedValue, (object) this.IncrementValue);
          if (this.IsNotForReplication)
            stringBuilder.Append(" NOT FOR REPLICATION");
        }
        if (this.IsRowGuid)
          stringBuilder.Append(" ROWGUIDCOL");
        if (!string.IsNullOrEmpty(this.CollationName) && !this.IsUserDefined)
          stringBuilder.AppendFormat(" COLLATE {0}", (object) this.CollationName);
        if (this.IsNullable)
        {
          if (this.IsSparse)
            stringBuilder.Append(" SPARSE");
          stringBuilder.Append(" NULL");
        }
        else
          stringBuilder.Append(" NOT NULL");
        if (this.Default != null)
          stringBuilder.Append(" DEFAULT").Append(this.Default);
      }
      return stringBuilder.ToString();
    }

    private string GetDataType()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat(" {0}", (object) StringUtil.QuoteName(this.DataType));
      string lower = this.DataType.ToLower(CultureInfo.InvariantCulture);
      if (lower != null)
      {
        switch (lower.Length)
        {
          case 3:
            switch (lower[0])
            {
              case 'b':
                if (lower == "bit")
                  goto label_48;
                else
                  goto label_48;
              case 'i':
                if (lower == "int")
                  goto label_48;
                else
                  goto label_48;
              case 'x':
                if (lower == "xml")
                  goto label_48;
                else
                  goto label_48;
              default:
                goto label_48;
            }
          case 4:
            switch (lower[0])
            {
              case 'c':
                if (lower == "char")
                  goto label_45;
                else
                  goto label_48;
              case 'd':
                if (lower == "date")
                  goto label_48;
                else
                  goto label_48;
              case 'r':
                if (lower == "real")
                  goto label_48;
                else
                  goto label_48;
              case 't':
                if (lower == "time" || lower == "text")
                  goto label_48;
                else
                  goto label_48;
              default:
                goto label_48;
            }
          case 5:
            switch (lower[1])
            {
              case 'c':
                if (lower == "nchar")
                  break;
                goto label_48;
              case 'l':
                if (lower == "float")
                  goto label_48;
                else
                  goto label_48;
              case 'm':
                if (lower == "image")
                  goto label_48;
                else
                  goto label_48;
              case 'o':
                if (lower == "money")
                  goto label_48;
                else
                  goto label_48;
              case 't':
                if (lower == "ntext")
                  goto label_48;
                else
                  goto label_48;
              default:
                goto label_48;
            }
            break;
          case 6:
            switch (lower[2])
            {
              case 'g':
                if (lower == "bigint")
                  goto label_48;
                else
                  goto label_48;
              case 'n':
                if (lower == "binary")
                  goto label_45;
                else
                  goto label_48;
              default:
                goto label_48;
            }
          case 7:
            switch (lower[0])
            {
              case 'd':
                if (lower == "decimal")
                  break;
                goto label_48;
              case 'n':
                if (lower == "numeric")
                  break;
                goto label_48;
              case 's':
                if (lower == "sysname")
                  goto label_48;
                else
                  goto label_48;
              case 't':
                if (lower == "tinyint")
                  goto label_48;
                else
                  goto label_48;
              case 'v':
                if (lower == "varchar")
                  goto label_45;
                else
                  goto label_48;
              default:
                goto label_48;
            }
            stringBuilder.AppendFormat("({0}, {1})", (object) this.Precision, (object) this.Scale);
            goto label_48;
          case 8:
            switch (lower[0])
            {
              case 'd':
                if (lower == "datetime")
                  goto label_48;
                else
                  goto label_48;
              case 'g':
                if (lower == "geometry")
                  goto label_48;
                else
                  goto label_48;
              case 'n':
                if (lower == "nvarchar")
                  break;
                goto label_48;
              case 's':
                if (lower == "smallint")
                  goto label_48;
                else
                  goto label_48;
              default:
                goto label_48;
            }
            break;
          case 9:
            switch (lower[0])
            {
              case 'd':
                if (lower == "datetime2")
                  goto label_48;
                else
                  goto label_48;
              case 'g':
                if (lower == "geography")
                  goto label_48;
                else
                  goto label_48;
              case 't':
                if (lower == "timestamp")
                  goto label_48;
                else
                  goto label_48;
              case 'v':
                if (lower == "varbinary")
                  goto label_45;
                else
                  goto label_48;
              default:
                goto label_48;
            }
          case 10:
            if (lower == "smallmoney")
              goto label_48;
            else
              goto label_48;
          case 11:
            if (lower == "sql_variant")
              goto label_48;
            else
              goto label_48;
          case 13:
            if (lower == "smalldatetime")
              goto label_48;
            else
              goto label_48;
          case 14:
            if (lower == "datetimeoffset")
              goto label_48;
            else
              goto label_48;
          case 16:
            if (lower == "uniqueidentifier")
              goto label_48;
            else
              goto label_48;
          default:
            goto label_48;
        }
        if (this.MaximumLength == -1)
        {
          stringBuilder.Append("(MAX)");
          goto label_48;
        }
        else
        {
          stringBuilder.AppendFormat("({0})", (object) (this.MaximumLength / 2));
          goto label_48;
        }
label_45:
        if (this.MaximumLength == -1)
          stringBuilder.Append("(MAX)");
        else
          stringBuilder.AppendFormat("({0})", (object) this.MaximumLength);
      }
label_48:
      return stringBuilder.ToString();
    }
  }
}
