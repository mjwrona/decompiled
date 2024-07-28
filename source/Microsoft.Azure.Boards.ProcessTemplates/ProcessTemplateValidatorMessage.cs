// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessTemplateValidatorMessage
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using System;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public class ProcessTemplateValidatorMessage
  {
    public const string HostedDefaultHelpLink = "https://go.microsoft.com/fwlink?LinkID=613784";
    public const string OnPremDefaultHelpLink = "https://go.microsoft.com/fwlink?LinkID=852639";

    public ProcessTemplateValidatorMessage()
    {
    }

    public ProcessTemplateValidatorMessage(
      string message,
      string file,
      int? lineNumber = null,
      string helpLink = null)
    {
      this.Message = message;
      this.File = file;
      this.LineNumber = lineNumber;
      this.HelpLink = helpLink;
    }

    public string Message { get; set; }

    public string File { get; set; }

    public int? LineNumber { get; set; }

    public string HelpLink { get; set; }

    public string GetErrorCode() => this.Message == null || !this.Message.Contains(":") ? (string) null : this.Message.Substring(0, this.Message.IndexOf(":", StringComparison.Ordinal)).Trim();
  }
}
