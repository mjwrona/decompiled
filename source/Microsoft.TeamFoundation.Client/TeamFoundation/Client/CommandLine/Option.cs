// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.CommandLine.Option
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.Client.CommandLine
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class Option
  {
    public List<string> Values;
    private Options.ID m_id;
    private string m_invokedAs;

    public Option(Options.ID id, string invokedAs)
    {
      this.m_id = id;
      this.m_invokedAs = invokedAs;
      this.Values = new List<string>();
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(100);
      stringBuilder.Append("Option instance " + this.GetHashCode().ToString() + "\n");
      int num = 0;
      foreach (string str in this.Values)
        stringBuilder.AppendFormat("  Value[{0}]: {1}\n", (object) num++, (object) str);
      return stringBuilder.ToString();
    }

    public Options.ID ID => this.m_id;

    public string InvokedAs => this.m_invokedAs;
  }
}
