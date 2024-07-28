// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.EventWebService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class EventWebService : TfsHttpClient
  {
    public EventWebService(TfsConnection connection)
      : base(connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("ccb8c273-dacb-4f09-b9aa-8ec0e42bada2");

    protected override string ComponentName => "Framework";

    protected override Guid ConfigurationServiceIdentifier => new Guid("c424ae04-8c6f-4516-8b2d-238fffca3081");

    protected override string ServiceType => "Eventing";

    protected override Exception ConvertException(SoapException exception) => TeamFoundationServiceException.ConvertException(exception);

    public Subscription[] EventSubscriptions(string userId, string projectName = null) => (Subscription[]) this.Invoke((TfsClientOperation) new EventWebService.EventSubscriptionsClientOperation(), new object[2]
    {
      (object) userId,
      (object) projectName
    });

    public Subscription[] EventSubscriptionsByClassification(
      string userId,
      string classification,
      string projectName = null)
    {
      return (Subscription[]) this.Invoke((TfsClientOperation) new EventWebService.EventSubscriptionsByClassificationClientOperation(), new object[3]
      {
        (object) userId,
        (object) classification,
        (object) projectName
      });
    }

    public void FireAsyncEvent(bool guaranteed, string theEvent) => this.Invoke((TfsClientOperation) new EventWebService.FireAsyncEventClientOperation(), new object[2]
    {
      (object) guaranteed,
      (object) theEvent
    });

    public void FireBulkAsyncEvents(bool guaranteed, IEnumerable<string> theEvents) => this.Invoke((TfsClientOperation) new EventWebService.FireBulkAsyncEventsClientOperation(), new object[2]
    {
      (object) guaranteed,
      (object) theEvents
    });

    public int SubscribeEvent(
      string userId,
      string eventType,
      string filterExpression,
      DeliveryPreference preferences,
      string projectName = null)
    {
      return (int) this.Invoke((TfsClientOperation) new EventWebService.SubscribeEventClientOperation(), new object[5]
      {
        (object) userId,
        (object) eventType,
        (object) filterExpression,
        (object) preferences,
        (object) projectName
      });
    }

    public int SubscribeEventWithClassification(
      string userId,
      string eventType,
      string filterExpression,
      DeliveryPreference preferences,
      string classification,
      string projectName = null)
    {
      return (int) this.Invoke((TfsClientOperation) new EventWebService.SubscribeEventWithClassificationClientOperation(), new object[6]
      {
        (object) userId,
        (object) eventType,
        (object) filterExpression,
        (object) preferences,
        (object) classification,
        (object) projectName
      });
    }

    public void UnsubscribeEvent(int subscriptionId, string projectName = null) => this.Invoke((TfsClientOperation) new EventWebService.UnsubscribeEventClientOperation(), new object[2]
    {
      (object) subscriptionId,
      (object) projectName
    });

    internal sealed class EventSubscriptionsByClassificationClientOperation : TfsClientOperation
    {
      public override string BodyName => "EventSubscriptionsByClassification";

      public override bool HasOutputs => true;

      public override string ResultName => "EventSubscriptionsByClassificationResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Events/03/EventSubscriptionsByClassification";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Events/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfSubscription;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfSubscriptionFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "userId", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "classification", parameter2);
        string parameter3 = (string) parameters[2];
        if (parameter3 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "projectName", parameter3);
      }
    }

    internal sealed class EventSubscriptionsClientOperation : TfsClientOperation
    {
      public override string BodyName => "EventSubscriptions";

      public override bool HasOutputs => true;

      public override string ResultName => "EventSubscriptionsResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Events/03/EventSubscriptions";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Events/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfSubscription;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfSubscriptionFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "userId", parameter1);
        string parameter2 = (string) parameters[1];
        if (string.IsNullOrEmpty(parameter2))
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "projectName", parameter2);
      }
    }

    internal sealed class FireAsyncEventClientOperation : TfsClientOperation
    {
      public override string BodyName => "FireAsyncEvent";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Events/03/FireAsyncEvent";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Events/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        bool parameter1 = (bool) parameters[0];
        if (parameter1)
          XmlUtility.ToXmlElement((XmlWriter) writer, "guaranteed", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 == null)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "theEvent", parameter2);
      }
    }

    internal sealed class FireBulkAsyncEventsClientOperation : TfsClientOperation
    {
      public override string BodyName => "FireBulkAsyncEvents";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Events/03/FireBulkAsyncEvents";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Events/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        bool parameter1 = (bool) parameters[0];
        if (parameter1)
          XmlUtility.ToXmlElement((XmlWriter) writer, "guaranteed", parameter1);
        IEnumerable<string> parameter2 = (IEnumerable<string>) parameters[1];
        Helper.ToXml((XmlWriter) writer, "theEvents", parameter2, false, false);
      }
    }

    internal sealed class SubscribeEventClientOperation : TfsClientOperation
    {
      public override string BodyName => "SubscribeEvent";

      public override bool HasOutputs => true;

      public override string ResultName => "SubscribeEventResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Events/03/SubscribeEvent";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Events/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) 0;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.Int32FromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "userId", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "eventType", parameter2);
        string parameter3 = (string) parameters[2];
        if (parameter3 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "filterExpression", parameter3);
        DeliveryPreference parameter4 = (DeliveryPreference) parameters[3];
        if (parameter4 != null)
          DeliveryPreference.ToXml((XmlWriter) writer, "preferences", parameter4);
        string parameter5 = (string) parameters[4];
        if (string.IsNullOrEmpty(parameter5))
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "projectName", parameter5);
      }
    }

    internal sealed class SubscribeEventWithClassificationClientOperation : TfsClientOperation
    {
      public override string BodyName => "SubscribeEventWithClassification";

      public override bool HasOutputs => true;

      public override string ResultName => "SubscribeEventWithClassificationResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Events/03/SubscribeEventWithClassification";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Events/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) 0;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.Int32FromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "userId", parameter1);
        string parameter2 = (string) parameters[1];
        if (parameter2 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "eventType", parameter2);
        string parameter3 = (string) parameters[2];
        if (parameter3 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "filterExpression", parameter3);
        DeliveryPreference parameter4 = (DeliveryPreference) parameters[3];
        if (parameter4 != null)
          DeliveryPreference.ToXml((XmlWriter) writer, "preferences", parameter4);
        string parameter5 = (string) parameters[4];
        if (parameter5 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "classification", parameter5);
        string parameter6 = (string) parameters[5];
        if (string.IsNullOrEmpty(parameter6))
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "projectName", parameter6);
      }
    }

    internal sealed class UnsubscribeEventClientOperation : TfsClientOperation
    {
      public override string BodyName => "UnsubscribeEvent";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Events/03/UnsubscribeEvent";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Events/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        int parameter1 = (int) parameters[0];
        if (parameter1 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "subscriptionId", parameter1);
        string parameter2 = (string) parameters[1];
        if (string.IsNullOrEmpty(parameter2))
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "projectName", parameter2);
      }
    }
  }
}
