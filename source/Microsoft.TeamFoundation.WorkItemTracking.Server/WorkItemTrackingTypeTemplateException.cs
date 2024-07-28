// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTrackingTypeTemplateException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [Serializable]
  public class WorkItemTrackingTypeTemplateException : WorkItemTrackingServiceException
  {
    private string m_message;

    public WorkItemTrackingTypeTemplateException(Exception exception)
      : base(exception.Message, exception)
    {
      ArgumentUtility.CheckForNull<Exception>(exception, nameof (exception));
    }

    public WorkItemTrackingTypeTemplateException(
      Exception exception,
      IEnumerable<string> provisioningEvents)
      : base(exception.Message, exception)
    {
      ArgumentUtility.CheckForNull<Exception>(exception, nameof (exception));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(provisioningEvents, nameof (provisioningEvents));
      this.m_message = string.Join(Environment.NewLine, provisioningEvents.Concat<string>((IEnumerable<string>) new string[1]
      {
        exception.Message
      }));
    }

    public override string Message => this.m_message;
  }
}
