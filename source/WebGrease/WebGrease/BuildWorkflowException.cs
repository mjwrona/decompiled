// Decompiled with JetBrains decompiler
// Type: WebGrease.BuildWorkflowException
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Runtime.Serialization;

namespace WebGrease
{
  [Serializable]
  internal class BuildWorkflowException : WorkflowException
  {
    public BuildWorkflowException()
    {
    }

    public BuildWorkflowException(string message)
      : base(message)
    {
    }

    public BuildWorkflowException(string message, Exception inner)
      : base(message, inner)
    {
    }

    public BuildWorkflowException(
      string message,
      string subcategory,
      string errorCode,
      string helpKeyword,
      string file,
      int lineNumber,
      int columnNumber,
      int endLineNumber,
      int endColumnNumber,
      Exception inner)
      : base(message, inner)
    {
      this.HasDetailedError = true;
      this.Subcategory = subcategory;
      this.ErrorCode = errorCode;
      this.HelpKeyword = helpKeyword;
      this.File = file;
      this.LineNumber = lineNumber;
      this.ColumnNumber = columnNumber;
      this.EndLineNumber = endLineNumber;
      this.EndColumnNumber = endColumnNumber;
    }

    protected BuildWorkflowException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public bool HasDetailedError { get; private set; }

    public string Subcategory { get; set; }

    public string ErrorCode { get; set; }

    public string HelpKeyword { get; set; }

    public string File { get; set; }

    public int LineNumber { get; set; }

    public int ColumnNumber { get; set; }

    public int EndLineNumber { get; set; }

    public int EndColumnNumber { get; set; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
        throw new ArgumentNullException(nameof (info));
      info.AddValue("HasDetailedError", this.HasDetailedError);
      info.AddValue("Subcategory", (object) this.Subcategory);
      info.AddValue("ErrorCode", (object) this.ErrorCode);
      info.AddValue("HelpKeyword", (object) this.HelpKeyword);
      info.AddValue("File", (object) this.File);
      info.AddValue("LineNumber", this.LineNumber);
      info.AddValue("ColumnNumber", this.ColumnNumber);
      info.AddValue("EndLineNumber", this.EndLineNumber);
      info.AddValue("EndColumnNumber", this.EndColumnNumber);
      base.GetObjectData(info, context);
    }
  }
}
