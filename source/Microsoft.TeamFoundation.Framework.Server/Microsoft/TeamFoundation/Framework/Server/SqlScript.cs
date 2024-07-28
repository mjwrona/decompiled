// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlScript
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class SqlScript
  {
    private static readonly char[] s_crlf = new char[2]
    {
      '\n',
      '\r'
    };
    private static readonly Dictionary<string, string[]> s_headers = new Dictionary<string, string[]>()
    {
      {
        "PROCEDURE",
        SqlScript.GetLines(FrameworkResources.StoredProcedureHeader())
      },
      {
        "FUNCTION",
        SqlScript.GetLines(FrameworkResources.FunctionHeader())
      },
      {
        "VIEW",
        SqlScript.GetLines(FrameworkResources.ViewHeader())
      }
    };
    private const string c_servicesHeader = "-->#(Services)";
    private List<SqlBatch> m_batches;
    private readonly string m_body;
    private readonly string m_objectName;

    public SqlScript(string name, string body)
    {
      this.Name = name;
      this.m_body = body;
    }

    public SqlScript(string name, string body, string objectName)
      : this(name, body)
    {
      this.m_objectName = objectName;
    }

    public SqlScript(string name, Stream body)
    {
      this.Name = name;
      using (StreamReader streamReader = new StreamReader(body))
        this.m_body = streamReader.ReadToEnd();
    }

    public SqlScript(string name, List<SqlBatch> batches)
    {
      this.Name = name;
      foreach (SqlBatch batch in batches)
      {
        batch.ScriptName = name;
        if (batch.Batch != null && batch.Batch.StartsWith("-->#(Services)", StringComparison.Ordinal))
          this.HasServiceVersionBlock = true;
      }
      this.m_batches = batches;
    }

    public string Name { get; private set; }

    public bool HasServiceVersionBlock { get; private set; }

    public List<SqlBatch> GetBatches() => this.GetBatches(false);

    public List<SqlBatch> GetBatches(bool replaceOnlineOn, ISqlInstrumentator sqlInstrumentator = null)
    {
      if (this.m_batches != null)
        return this.m_batches;
      List<SqlBatch> batches = new List<SqlBatch>();
      StringBuilder batchBuilder = new StringBuilder();
      bool flag1 = false;
      bool flag2 = false;
      string name = this.m_objectName;
      string type = string.Empty;
      StringReader stringReader = new StringReader(this.m_body);
      int num1 = 1;
      SqlBatch sqlBatch = new SqlBatch();
      sqlBatch.LineNumber = 1;
      sqlBatch.ScriptName = this.Name;
      int headerLinesCount = 0;
      string str1;
      while ((str1 = stringReader.ReadLine()) != null)
      {
        string header = str1.TrimEnd();
        int num2 = string.Compare(header.Trim(), "GO", StringComparison.OrdinalIgnoreCase) == 0 ? 1 : 0;
        ++num1;
        if (num2 != 0)
        {
          this.AddToBatches(batches, sqlBatch, batchBuilder, name, headerLinesCount, sqlInstrumentator);
          name = this.m_objectName;
          type = string.Empty;
          sqlBatch = new SqlBatch();
          sqlBatch.ScriptName = this.Name;
          sqlBatch.LineNumber = num1;
          sqlBatch.HeaderLinesCount = headerLinesCount;
          batchBuilder.Clear();
          headerLinesCount = 0;
        }
        else
        {
          if (flag2)
          {
            if (header.Contains("-->#END DBG"))
            {
              flag2 = false;
              header = "-- END NOCOMPARE (END DBG)";
            }
            if (!flag1)
              header = string.Empty;
          }
          else if (header.Contains("-->#BEGIN DBG"))
          {
            flag2 = true;
            header = !flag1 ? string.Empty : "-- BEGIN NOCOMPARE (BEGIN DBG)";
          }
          if (replaceOnlineOn)
            header = header.Replace("ONLINE=ON", "ONLINE=OFF");
          int length = header.IndexOf("-->#", StringComparison.Ordinal);
          if (length == 0)
          {
            sqlBatch.InSnapshot = SqlScript.GetInSnapshotAttr(header);
            if (SqlScript.TryGetTypeAttr(header, out type))
            {
              if (string.IsNullOrEmpty(this.m_objectName))
                SqlScript.TryGetNameAttr(header, out name);
              string[] strArray;
              if (SqlScript.s_headers.TryGetValue(type, out strArray))
              {
                headerLinesCount = strArray.Length;
                foreach (string str2 in strArray)
                  batchBuilder.AppendLine(str2);
                string hash;
                if (SqlScript.TryGetHashAttr(header, out hash))
                  batchBuilder.AppendFormat("-- Hash: {0}", (object) hash);
              }
            }
            else if (header.StartsWith("-->#(Services)", StringComparison.Ordinal))
              this.HasServiceVersionBlock = true;
            batchBuilder.AppendLine();
          }
          else
          {
            if (length > 0)
              header = header.Substring(0, length);
            batchBuilder.AppendLine(header);
          }
        }
      }
      this.AddToBatches(batches, sqlBatch, batchBuilder, name, headerLinesCount, sqlInstrumentator);
      this.m_batches = batches;
      return batches;
    }

    private void AddToBatches(
      List<SqlBatch> batches,
      SqlBatch sqlBatch,
      StringBuilder batchBuilder,
      string objectName,
      int headerLinesCount,
      ISqlInstrumentator sqlInstrumentator)
    {
      string extraBatchText = (string) null;
      sqlInstrumentator?.InstrumentSqlContent(objectName, batchBuilder, out extraBatchText);
      string str = batchBuilder.ToString().Trim();
      if (!string.IsNullOrEmpty(str))
      {
        sqlBatch.Batch = str;
        sqlBatch.HeaderLinesCount = headerLinesCount;
        batches.Add(sqlBatch);
      }
      extraBatchText = extraBatchText?.Trim();
      if (string.IsNullOrEmpty(extraBatchText))
        return;
      batches.Add(new SqlBatch()
      {
        Batch = extraBatchText,
        LineNumber = sqlBatch.LineNumber,
        HeaderLinesCount = 0,
        ScriptName = this.Name
      });
    }

    public static SqlBatch CreateAlterObjectBatch(string content)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(content, nameof (content));
      string[] strArray = content.Split(new string[1]
      {
        Environment.NewLine
      }, StringSplitOptions.None);
      string header = strArray[0];
      string type;
      string hash;
      if (!SqlScript.TryGetTypeAttr(header, out type) || !SqlScript.TryGetHashAttr(header, out hash))
        throw new ArgumentException("First line of the content must have object's metadata.", nameof (content));
      if (!type.Equals("PROCEDURE", StringComparison.Ordinal) && !type.Equals("FUNCTION", StringComparison.Ordinal))
        throw new ArgumentException("The object type must be either PROCEDURE or FUNCTION.", nameof (content));
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string str in SqlScript.s_headers[type])
        stringBuilder.AppendLine(str);
      stringBuilder.AppendFormat("-- Hash: {0}", (object) hash).AppendLine();
      bool flag1 = false;
      string str1 = "CREATE " + type;
      bool flag2 = false;
      bool flag3 = false;
      for (int index = 0; index < strArray.Length; ++index)
      {
        string str2 = strArray[index];
        if (!flag1 && str2.StartsWith(str1, StringComparison.Ordinal))
        {
          str2 = "ALTER " + type + str2.Substring(str1.Length);
          flag1 = true;
        }
        if (flag3)
        {
          if (str2.Contains("-->#END DBG"))
          {
            flag3 = false;
            str2 = "-- END NOCOMPARE (END DBG)";
          }
          if (!flag2)
            str2 = string.Empty;
        }
        else if (str2.Contains("-->#BEGIN DBG"))
        {
          flag3 = true;
          str2 = !flag2 ? string.Empty : "-- BEGIN NOCOMPARE (BEGIN DBG)";
        }
        if (index != strArray.Length - 1 || !string.IsNullOrEmpty(str2))
        {
          if (flag1)
            stringBuilder.AppendLine(str2);
        }
        else
          break;
      }
      return new SqlBatch()
      {
        Batch = stringBuilder.ToString()
      };
    }

    private static bool TryGetTypeAttr(string header, out string type) => SqlScript.TryGetAttributeValue(header, nameof (type), out type);

    private static bool TryGetHashAttr(string header, out string hash) => SqlScript.TryGetAttributeValue(header, nameof (hash), out hash);

    private static bool TryGetNameAttr(string header, out string name) => SqlScript.TryGetAttributeValue(header, nameof (name), out name);

    private static bool TryGetKindAttr(string header, out string kind) => SqlScript.TryGetAttributeValue(header, nameof (kind), out kind);

    private static bool GetInSnapshotAttr(string header)
    {
      string attributeValue;
      return SqlScript.TryGetAttributeValue(header, "inSnapshot", out attributeValue) && bool.Parse(attributeValue);
    }

    private static bool TryGetAttributeValue(
      string header,
      string attribute,
      out string attributeValue)
    {
      attributeValue = (string) null;
      string str = attribute + "=\"";
      int num1 = header.IndexOf(str);
      if (num1 > 0)
      {
        int startIndex = num1 + str.Length;
        int num2 = header.IndexOf("\"", startIndex);
        if (num2 > 0)
          attributeValue = header.Substring(startIndex, num2 - startIndex);
      }
      return attributeValue != null;
    }

    private static string[] GetLines(string text) => text.Split(SqlScript.s_crlf, StringSplitOptions.RemoveEmptyEntries);
  }
}
