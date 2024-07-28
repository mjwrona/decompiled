// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTrackingAggregateException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [Serializable]
  public sealed class WorkItemTrackingAggregateException : WorkItemTrackingServiceException
  {
    private List<TeamFoundationServiceException> m_exceptions;

    public WorkItemTrackingAggregateException(
      IEnumerable<TeamFoundationServiceException> exceptions)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) exceptions, nameof (exceptions));
      this.m_exceptions = new List<TeamFoundationServiceException>(exceptions);
    }

    public WorkItemTrackingAggregateException(TeamFoundationServiceException exception)
    {
      ArgumentUtility.CheckForNull<TeamFoundationServiceException>(exception, nameof (exception));
      this.m_exceptions = new List<TeamFoundationServiceException>();
      this.m_exceptions.Add(exception);
    }

    public void AddException(TeamFoundationServiceException e)
    {
      if (e == null)
        return;
      if (e.GetType() == typeof (WorkItemTrackingAggregateException))
      {
        WorkItemTrackingAggregateException aggregateException = e as WorkItemTrackingAggregateException;
        foreach (TeamFoundationServiceException allException in aggregateException.AllExceptions)
          this.m_exceptions.Add(allException);
        this.LeadingException = aggregateException.LeadingException;
      }
      else
      {
        this.m_exceptions.Add(e);
        this.LeadingException = e;
      }
    }

    public override string Message => this.LeadingException.Message;

    public IEnumerable<TeamFoundationServiceException> AllExceptions => (IEnumerable<TeamFoundationServiceException>) this.m_exceptions;

    public TeamFoundationServiceException LeadingException { get; set; }
  }
}
