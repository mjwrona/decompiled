// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.StreamUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class StreamUtility
  {
    public static void Copy(
      IDataReader reader,
      TextWriter output,
      string delimiter = ",",
      bool includeHeader = true,
      string headerPrefix = null,
      string rowPrefix = null,
      int? max = null)
    {
      ArgumentUtility.CheckForNull<IDataReader>(reader, nameof (reader));
      ArgumentUtility.CheckForNull<TextWriter>(output, nameof (output));
      bool flag = !string.IsNullOrEmpty(headerPrefix) || !string.IsNullOrEmpty(rowPrefix);
      List<string> values = new List<string>();
      if (includeHeader)
      {
        if (flag)
          values.Add(headerPrefix);
        for (int i = 0; i < reader.FieldCount; ++i)
          values.Add(reader.GetName(i));
        output.WriteLine(string.Join(delimiter, (IEnumerable<string>) values));
      }
      int num = 0;
      while (reader.Read())
      {
        ++num;
        if (max.HasValue && num > max.Value)
        {
          output.WriteLine(string.Format("There were more than {0} results, the rest have been omitted", (object) max));
          break;
        }
        values.Clear();
        if (flag)
          values.Add(rowPrefix);
        for (int i = 0; i < reader.FieldCount; ++i)
        {
          object obj = reader[i];
          string str;
          switch (obj)
          {
            case null:
            case DBNull _:
              str = "<null>";
              break;
            case byte[] _:
              byte[] numArray = (byte[]) obj;
              StringBuilder stringBuilder = new StringBuilder("0x", numArray.Length * 2 + 2);
              for (int index = 0; index < numArray.Length; ++index)
                stringBuilder.Append(numArray[index].ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture));
              str = stringBuilder.ToString();
              break;
            default:
              str = obj.ToString();
              break;
          }
          values.Add(str);
        }
        output.WriteLine(string.Join(delimiter, (IEnumerable<string>) values));
      }
    }
  }
}
