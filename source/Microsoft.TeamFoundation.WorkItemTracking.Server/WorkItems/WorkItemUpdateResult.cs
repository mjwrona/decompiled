// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdateResult
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
  public class WorkItemUpdateResult
  {
    public WorkItemUpdateResult()
    {
      this.Fields = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      this.LinkUpdates = new List<WorkItemLinkUpdateResult>();
      this.ResourceLinkUpdates = new List<WorkItemResourceLinkUpdateResult>();
    }

    public virtual int Id { get; set; }

    public int UpdateId { get; set; }

    public int Rev { get; set; }

    public virtual TeamFoundationServiceException Exception { get; internal set; }

    public IDictionary<string, object> Fields { get; set; }

    public List<WorkItemLinkUpdateResult> LinkUpdates { get; private set; }

    public List<WorkItemResourceLinkUpdateResult> ResourceLinkUpdates { get; private set; }

    public IEnumerable<Guid> CurrentExtensions { get; internal set; }

    public IEnumerable<Guid> AttachedExtensions { get; internal set; }

    public IEnumerable<Guid> DetachedExtensions { get; internal set; }

    public IEnumerable<string> UpdatedBoardFields
    {
      get
      {
        List<string> updatedBoardFields = new List<string>();
        if (this.Fields != null)
        {
          foreach (KeyValuePair<string, object> field in (IEnumerable<KeyValuePair<string, object>>) this.Fields)
          {
            int result;
            if (int.TryParse(field.Key, out result))
            {
              switch (result)
              {
                case 90:
                  updatedBoardFields.Add("System.BoardColumn");
                  continue;
                case 92:
                  updatedBoardFields.Add("System.BoardLane");
                  continue;
                default:
                  continue;
              }
            }
          }
        }
        return (IEnumerable<string>) updatedBoardFields;
      }
    }

    public bool? IsCommentingAvailable { get; internal set; }

    internal void AddExceptions(
      IEnumerable<TeamFoundationServiceException> exceptions)
    {
      if (exceptions == null)
        return;
      foreach (TeamFoundationServiceException exception in exceptions)
        this.AddException(exception);
    }

    internal void AddException(TeamFoundationServiceException exception)
    {
      if (exception == null)
        return;
      if (this.Exception == null)
      {
        this.Exception = exception;
      }
      else
      {
        if (this.Exception.GetType() != typeof (WorkItemTrackingAggregateException))
          this.Exception = (TeamFoundationServiceException) new WorkItemTrackingAggregateException(this.Exception);
        ((WorkItemTrackingAggregateException) this.Exception).AddException(exception);
      }
    }

    protected virtual string DebuggerDisplay => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Id = {0}, UpdateId = {1}, Rev = {2}, HasLastError = {3}, HasFields = {4}, HasLinkUpdates = {5}, HasResourceLinkUpdates = {6}, HasCurrentExtensions = {7}, HasAttachedExtensions = {8}, HasDetachedExtensions = {9}", (object) this.Id, (object) this.UpdateId, (object) this.Rev, (object) (this.Exception != null), (object) (bool) (this.Fields == null ? 0 : (this.Fields.Any<KeyValuePair<string, object>>() ? 1 : 0)), (object) (bool) (this.LinkUpdates == null ? 0 : (this.LinkUpdates.Any<WorkItemLinkUpdateResult>() ? 1 : 0)), (object) (bool) (this.ResourceLinkUpdates == null ? 0 : (this.ResourceLinkUpdates.Any<WorkItemResourceLinkUpdateResult>() ? 1 : 0)), (object) (bool) (this.CurrentExtensions == null ? 0 : (this.CurrentExtensions.Any<Guid>() ? 1 : 0)), (object) (bool) (this.AttachedExtensions == null ? 0 : (this.AttachedExtensions.Any<Guid>() ? 1 : 0)), (object) (bool) (this.DetachedExtensions == null ? 0 : (this.DetachedExtensions.Any<Guid>() ? 1 : 0)));
  }
}
