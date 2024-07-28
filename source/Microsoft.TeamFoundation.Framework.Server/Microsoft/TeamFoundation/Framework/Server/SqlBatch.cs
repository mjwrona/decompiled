// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlBatch
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class SqlBatch
  {
    private string m_batch;

    public int Index { get; set; }

    public string ScriptName { get; set; }

    public int LineNumber { get; set; }

    public int HeaderLinesCount { get; set; }

    public bool InSnapshot { get; set; }

    public string Batch
    {
      get => this.m_batch;
      set
      {
        this.m_batch = value;
        int length = value != null ? value.Length : 0;
        bool flag = false;
        for (int index = 0; index < length; ++index)
        {
          if (value[index] >= '\u0080')
          {
            flag = true;
            break;
          }
        }
        this.HasNonAsciiCharacters = flag;
      }
    }

    public List<string> GetLines()
    {
      List<string> lines = new List<string>();
      using (StringReader stringReader = new StringReader(this.m_batch))
      {
        string str;
        while ((str = stringReader.ReadLine()) != null)
          lines.Add(str);
      }
      return lines;
    }

    public bool HasNonAsciiCharacters { get; private set; }
  }
}
