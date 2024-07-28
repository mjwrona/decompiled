// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessInvalidStateTransitionException
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using System;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  [Serializable]
  public class ProcessInvalidStateTransitionException : ProcessServiceException
  {
    public ProcessInvalidStateTransitionException()
      : base(Resources.ProcessTemplateInvalidStateTransition(), 402482)
    {
    }

    protected ProcessInvalidStateTransitionException(string message, int errorCode)
      : base(message, errorCode)
    {
    }

    public ProcessInvalidStateTransitionException(
      Guid type,
      ProcessStatus fromState,
      ProcessStatus toState)
      : base(Resources.ProcessTemplateInformedStateTransition((object) type, (object) fromState, (object) toState), 402483)
    {
      this.Type = type;
      this.FromState = fromState;
      this.ToState = toState;
    }

    public Guid Type { get; set; }

    public ProcessStatus FromState { get; set; }

    public ProcessStatus ToState { get; set; }
  }
}
