// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Query.Kqlm.ExecutionMessage
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Metrics;
using System;

namespace Microsoft.Cloud.Metrics.Client.Query.Kqlm
{
  internal sealed class ExecutionMessage : IExecutionMessage
  {
    public ExecutionMessage(
      MessageSeverity severity,
      string text,
      string documentationLink,
      StatementContextInformation statementContext)
    {
      if (string.IsNullOrWhiteSpace(text))
        throw new ArgumentNullException(nameof (text));
      if (string.IsNullOrWhiteSpace(documentationLink))
        throw new ArgumentNullException(nameof (documentationLink));
      this.Severity = severity;
      this.Text = text;
      this.DocumentationLink = documentationLink;
      this.StatementContext = (IStatementContextInformation) statementContext;
    }

    public MessageSeverity Severity { get; }

    public string Text { get; }

    public string DocumentationLink { get; }

    public IStatementContextInformation StatementContext { get; }
  }
}
