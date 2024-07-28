// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.ProcessTemplateValidatorResult
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public class ProcessTemplateValidatorResult
  {
    private List<object> m_details = new List<object>();
    private List<ProcessTemplateValidatorMessage> m_errors = new List<ProcessTemplateValidatorMessage>();
    private List<ProcessTemplateValidatorMessage> m_confirmationsNeeded = new List<ProcessTemplateValidatorMessage>();

    public ProcessTemplateValidatorResult(
      IEnumerable<ProcessTemplateValidatorMessage> errors = null,
      IEnumerable<ProcessTemplateValidatorMessage> confirmationsNeeded = null,
      IEnumerable<object> details = null)
    {
      if (errors != null)
        this.m_errors.AddRange(errors);
      if (confirmationsNeeded != null)
        this.m_confirmationsNeeded.AddRange(confirmationsNeeded);
      if (details == null)
        return;
      this.AddDetails(details);
    }

    public IList<ProcessTemplateValidatorMessage> Errors => (IList<ProcessTemplateValidatorMessage>) this.m_errors;

    public IList<ProcessTemplateValidatorMessage> ConfirmationsNeeded => (IList<ProcessTemplateValidatorMessage>) this.m_confirmationsNeeded;

    public IList<object> Details => (IList<object>) this.m_details;

    public bool IsSuccessful => !this.Errors.Any<ProcessTemplateValidatorMessage>();

    public void AddDetails(IEnumerable<object> details)
    {
      if (details == null)
        return;
      this.m_details.AddRange(details);
    }

    public void AddErrors(
      IEnumerable<ProcessTemplateValidatorMessage> errors)
    {
      if (errors == null)
        return;
      this.m_errors.AddRange(errors);
    }
  }
}
