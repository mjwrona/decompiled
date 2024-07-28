// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.SubscriptionUpdate
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class SubscriptionUpdate
  {
    public SubscriptionUpdate(int subscriptionId) => this.Id = subscriptionId;

    public SubscriptionUpdate(string subscriptionId)
    {
      int result;
      if (int.TryParse(subscriptionId, out result))
      {
        this.Id = result;
      }
      else
      {
        this.Id = -1;
        this.ContributionId = subscriptionId;
      }
    }

    public int Id { get; set; }

    public string ContributionId { get; set; }

    public string EventTypeName { get; set; }

    public string Expression { get; set; }

    public string IndexedExpression { get; set; }

    public string Channel { get; set; }

    public string Address { get; set; }

    public string Classification { get; set; }

    public string Description { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public string MetaData { get; set; }

    public SubscriptionStatus? Status { get; set; }

    public string StatusMessage { get; set; }

    public SubscriptionFlags? Flags { get; set; }

    public SubscriptionAdminSettings AdminSettings { get; set; }

    public SubscriptionUserSettings UserSettings { get; set; }

    public string Matcher { get; set; }

    public Guid? ScopeId { get; set; }

    public SubscriptionDiagnostics Diagnostics { get; set; }

    public Guid? SubscriberId { get; set; }

    public bool IsFilterUpdate() => this.Expression != null || this.IndexedExpression != null || this.Matcher != null || this.EventTypeName != null;

    public bool IsNoNonStatusCoreFieldsUpdate() => this.Expression == null && this.EventTypeName == null && this.IndexedExpression == null && this.Channel == null && this.Address == null && this.Classification == null && this.Description == null && this.MetaData == null && !this.Flags.HasValue && !this.Flags.HasValue && this.Matcher == null && !this.ScopeId.HasValue && this.Diagnostics == null && !this.SubscriberId.HasValue;

    public bool IsNoStatusUpdate() => !this.Status.HasValue && this.StatusMessage == null;

    public bool IsNoCoreFieldsUpdate() => this.IsNoNonStatusCoreFieldsUpdate() && this.IsNoStatusUpdate();
  }
}
