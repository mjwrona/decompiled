// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.SubscriptionFieldProvider
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class SubscriptionFieldProvider
  {
    private Dictionary<string, NotificationEventField> m_fieldsByName;
    private Dictionary<string, NotificationEventField> m_fieldsByPath;

    public string EventType { get; private set; }

    public SubscriptionScope Scope { get; private set; }

    public SubscriptionFieldProvider(
      IVssRequestContext requestContext,
      string eventType,
      SubscriptionScope scope)
    {
      this.EventType = eventType;
      this.Scope = scope;
      this.GetFields(requestContext);
    }

    private void GetFields(IVssRequestContext requestContext)
    {
      this.m_fieldsByName = new Dictionary<string, NotificationEventField>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_fieldsByPath = new Dictionary<string, NotificationEventField>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      INotificationEventService service = requestContext.GetService<INotificationEventService>();
      FieldValuesQuery fieldValuesQuery = new FieldValuesQuery();
      SubscriptionScope scope = this.Scope;
      int num;
      if (scope == null)
      {
        num = 0;
      }
      else
      {
        Guid id = scope.Id;
        num = 1;
      }
      if (num != 0 && this.Scope.Id != Guid.Empty)
        fieldValuesQuery.Scope = this.Scope.Id.ToString();
      IVssRequestContext requestContext1 = requestContext;
      string eventType = this.EventType;
      FieldValuesQuery query = fieldValuesQuery;
      IList<NotificationEventField> inputValues = service.GetInputValues(requestContext1, eventType, query);
      if (inputValues == null)
        return;
      foreach (NotificationEventField notificationEventField in (IEnumerable<NotificationEventField>) inputValues)
      {
        this.m_fieldsByPath[notificationEventField.Path] = notificationEventField;
        this.m_fieldsByName[notificationEventField.Name] = notificationEventField;
      }
    }

    public NotificationEventField GetFieldByName(string fieldName, bool throwIfNotFound)
    {
      NotificationEventField fieldByName;
      if (this.m_fieldsByName.TryGetValue(fieldName, out fieldByName))
        return fieldByName;
      if (throwIfNotFound)
        throw new InvalidFieldValueException("Field " + fieldName + " is not valid");
      return (NotificationEventField) null;
    }
  }
}
