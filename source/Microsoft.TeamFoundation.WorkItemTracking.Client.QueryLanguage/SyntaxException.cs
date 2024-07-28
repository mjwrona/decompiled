// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.SyntaxException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public class SyntaxException : ApplicationException
  {
    private Node m_node;
    private SyntaxError m_code;

    public SyntaxException()
    {
    }

    protected SyntaxException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public SyntaxException(Node node, SyntaxError err)
      : base(SyntaxException.GetDetailMessage(node, err))
    {
      this.m_node = node;
      this.m_code = err;
    }

    public SyntaxException(string message)
      : base(message)
    {
    }

    public Node Node => this.m_node;

    public SyntaxError SyntaxError => this.m_code;

    public string Details => this.Message;

    private static string GetDetailMessage(Node node, SyntaxError code)
    {
      string detailMessage = ResourceStrings.Get(code.ToString());
      if (node != null)
        detailMessage = ResourceStrings.Format("DetailedMessage", (object) detailMessage, (object) node.ToString());
      return detailMessage;
    }
  }
}
