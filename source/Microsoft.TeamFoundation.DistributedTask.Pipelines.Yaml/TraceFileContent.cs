// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.TraceFileContent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  internal struct TraceFileContent
  {
    private readonly string m_header;
    private readonly string m_value;

    public TraceFileContent(string header, string value)
    {
      this.m_header = header;
      this.m_value = value;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine();
      stringBuilder.AppendLine(string.Empty.PadRight(80, '*'));
      stringBuilder.AppendLine("* " + this.m_header);
      stringBuilder.AppendLine(string.Empty.PadRight(80, '*'));
      stringBuilder.AppendLine();
      using (StringReader stringReader = new StringReader(this.m_value))
      {
        int num = 1;
        string str = stringReader.ReadLine();
        while (str != null)
        {
          stringBuilder.AppendLine(num.ToString().PadLeft(4) + ": " + str);
          str = stringReader.ReadLine();
          ++num;
        }
      }
      return stringBuilder.ToString();
    }
  }
}
