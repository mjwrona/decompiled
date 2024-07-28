// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.ExceptionDetails
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External
{
  [GeneratedCode("gbc", "3.02")]
  internal class ExceptionDetails
  {
    public int id { get; set; }

    public int outerId { get; set; }

    public string typeName { get; set; }

    public string message { get; set; }

    public bool hasFullStack { get; set; }

    public string stack { get; set; }

    public IList<StackFrame> parsedStack { get; set; }

    public ExceptionDetails()
      : this("AI.ExceptionDetails", nameof (ExceptionDetails))
    {
    }

    protected ExceptionDetails(string fullName, string name)
    {
      this.typeName = string.Empty;
      this.message = string.Empty;
      this.hasFullStack = true;
      this.stack = string.Empty;
      this.parsedStack = (IList<StackFrame>) new List<StackFrame>();
    }

    internal static ExceptionDetails CreateWithoutStackInfo(
      Exception exception,
      ExceptionDetails parentExceptionDetails)
    {
      if (exception == null)
        throw new ArgumentNullException(nameof (exception));
      ExceptionDetails withoutStackInfo = new ExceptionDetails()
      {
        id = exception.GetHashCode(),
        typeName = exception.GetType().FullName,
        message = exception.Message
      };
      if (parentExceptionDetails != null)
        withoutStackInfo.outerId = parentExceptionDetails.id;
      return withoutStackInfo;
    }
  }
}
