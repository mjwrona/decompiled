// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.StatementContextInformation
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

namespace Microsoft.Cloud.Metrics.Client.Metrics
{
  public sealed class StatementContextInformation : IStatementContextInformation
  {
    public StatementContextInformation(
      int lineNumber,
      int charPositionInLine,
      int charPositionAbsolute,
      int errorSectionLength)
    {
      this.LineNumber = lineNumber;
      this.CharPositionInLine = charPositionInLine;
      this.CharPositionAbsolute = charPositionAbsolute;
      this.ErrorSectionLength = errorSectionLength;
    }

    public static StatementContextInformation Empty { get; } = new StatementContextInformation(0, 0, 0, 0);

    public int LineNumber { get; }

    public int CharPositionInLine { get; }

    public int CharPositionAbsolute { get; }

    public int ErrorSectionLength { get; }
  }
}
