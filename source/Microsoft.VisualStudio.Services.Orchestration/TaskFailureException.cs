// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.TaskFailureException
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  internal class TaskFailureException : Exception
  {
    public TaskFailureException()
    {
    }

    public TaskFailureException(string reason)
      : base(reason)
    {
    }

    public TaskFailureException(string reason, Exception innerException)
      : base(reason, innerException)
    {
    }

    public TaskFailureException(string reason, string details)
      : base(reason)
    {
      this.Details = details;
    }

    public string Details { get; set; }
  }
}
