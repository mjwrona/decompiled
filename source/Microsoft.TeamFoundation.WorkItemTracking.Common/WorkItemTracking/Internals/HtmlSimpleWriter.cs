// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.HtmlSimpleWriter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.Text;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  internal class HtmlSimpleWriter : IHtmlFilterWriter
  {
    private StringBuilder m_str = new StringBuilder();

    public override string ToString() => this.m_str.ToString();

    public StringBuilder Content => this.m_str;

    void IHtmlFilterWriter.WriteText(string s, int offs, int len) => this.m_str.Append(s, offs, len);

    void IHtmlFilterWriter.WriteTag(string s, int offs, int len, string tag, bool endTag) => this.m_str.Append(s, offs, len);

    void IHtmlFilterWriter.WriteEndOfTag(string s, int offs, int len, string tag) => this.m_str.Append(s, offs, len);

    void IHtmlFilterWriter.WriteAttribute(
      string s,
      int offs,
      int len,
      string tag,
      string attr,
      int i1,
      int i2)
    {
      this.m_str.Append(s, offs, len);
    }
  }
}
