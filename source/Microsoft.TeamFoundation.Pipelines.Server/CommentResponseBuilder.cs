// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.CommentResponseBuilder
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using System.Text;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class CommentResponseBuilder
  {
    private StringBuilder m_body = new StringBuilder();

    public static string Build(string singleLine) => new CommentResponseBuilder().AppendLine(singleLine).ToString();

    public CommentResponseBuilder AppendLine(string line = null)
    {
      this.AppendText(line);
      this.m_body.AppendLine("<br>");
      return this;
    }

    public CommentResponseBuilder AppendText(string text, bool isBold = false)
    {
      if (isBold)
        this.m_body.Append("<b>" + text + "</b>");
      else
        this.m_body.Append(text);
      return this;
    }

    public CommentResponseBuilder StartList(string type = "none")
    {
      this.m_body.Append("<ul type=\"" + type + "\">");
      return this;
    }

    public CommentResponseBuilder StartListItem()
    {
      this.m_body.Append("<li>");
      return this;
    }

    public CommentResponseBuilder EndListItem()
    {
      this.m_body.Append("</li>");
      return this;
    }

    public CommentResponseBuilder EndList()
    {
      this.m_body.Append("</ul>");
      return this;
    }

    public CommentResponseBuilder AppendListItem(string item, bool isBold = false)
    {
      this.StartListItem();
      this.AppendText(item, isBold);
      this.EndListItem();
      return this;
    }

    public CommentResponseBuilder AppendLink(string link, string caption, bool isBold = false)
    {
      this.m_body.Append("<a href=\"" + link + "\">");
      this.AppendText(caption, isBold);
      this.m_body.Append("</a>");
      return this;
    }

    public override string ToString() => this.m_body.Length == 0 ? string.Empty : "<samp>\n" + this.m_body.ToString() + "\n</samp>";
  }
}
