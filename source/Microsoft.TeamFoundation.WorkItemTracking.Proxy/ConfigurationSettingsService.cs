// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.ConfigurationSettingsService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  internal class ConfigurationSettingsService : TfsHttpClient
  {
    protected override string ComponentName => nameof (ConfigurationSettingsService);

    public ConfigurationSettingsService(TfsTeamProjectCollection connection)
      : base((TfsConnection) connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("1e9d1b48-775a-49c8-af8f-d41a06e0cdb0");

    protected override string ServiceType => "ConfigurationSettingsUrl";

    public bool GetInProcBuildCompletionNotificationAvailability() => (bool) this.Invoke((TfsClientOperation) new ConfigurationSettingsService.GetInProcBuildCompletionNotificationAvailabilityClientOperation(), Array.Empty<object>());

    public long GetMaxAttachmentSize() => (long) this.Invoke((TfsClientOperation) new ConfigurationSettingsService.GetMaxAttachmentSizeClientOperation(), Array.Empty<object>());

    public int GetMaxBuildListSize() => (int) this.Invoke((TfsClientOperation) new ConfigurationSettingsService.GetMaxBuildListSizeClientOperation(), Array.Empty<object>());

    public int GetWorkItemQueryTimeout() => (int) this.Invoke((TfsClientOperation) new ConfigurationSettingsService.GetWorkItemQueryTimeoutClientOperation(), Array.Empty<object>());

    public string GetWorkitemTrackingVersion() => (string) this.Invoke((TfsClientOperation) new ConfigurationSettingsService.GetWorkitemTrackingVersionClientOperation(), Array.Empty<object>());

    public void SetInProcBuildCompletionNotificationAvailability(bool isEnabled) => this.Invoke((TfsClientOperation) new ConfigurationSettingsService.SetInProcBuildCompletionNotificationAvailabilityClientOperation(), new object[1]
    {
      (object) isEnabled
    });

    public void SetMaxAttachmentSize(long maxSize) => this.Invoke((TfsClientOperation) new ConfigurationSettingsService.SetMaxAttachmentSizeClientOperation(), new object[1]
    {
      (object) maxSize
    });

    public void SetMaxBuildListSize(int maxBuildListSize) => this.Invoke((TfsClientOperation) new ConfigurationSettingsService.SetMaxBuildListSizeClientOperation(), new object[1]
    {
      (object) maxBuildListSize
    });

    public void SetWorkItemQueryTimeout(int workItemQueryTimeout) => this.Invoke((TfsClientOperation) new ConfigurationSettingsService.SetWorkItemQueryTimeoutClientOperation(), new object[1]
    {
      (object) workItemQueryTimeout
    });

    internal sealed class GetInProcBuildCompletionNotificationAvailabilityClientOperation : 
      TfsClientOperation
    {
      public override string BodyName => "GetInProcBuildCompletionNotificationAvailability";

      public override bool HasOutputs => true;

      public override string ResultName => "GetInProcBuildCompletionNotificationAvailabilityResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/configurationSettingsService/03/GetInProcBuildCompletionNotificationAvailability";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/configurationSettingsService/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) false;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.BooleanFromXmlElement(reader);
    }

    internal sealed class GetMaxAttachmentSizeClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetMaxAttachmentSize";

      public override bool HasOutputs => true;

      public override string ResultName => "GetMaxAttachmentSizeResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/configurationSettingsService/03/GetMaxAttachmentSize";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/configurationSettingsService/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) 0L;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.Int64FromXmlElement(reader);
    }

    internal sealed class GetMaxBuildListSizeClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetMaxBuildListSize";

      public override bool HasOutputs => true;

      public override string ResultName => "GetMaxBuildListSizeResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/configurationSettingsService/03/GetMaxBuildListSize";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/configurationSettingsService/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) 0;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.Int32FromXmlElement(reader);
    }

    internal sealed class GetWorkItemQueryTimeoutClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetWorkItemQueryTimeout";

      public override bool HasOutputs => true;

      public override string ResultName => "GetWorkItemQueryTimeoutResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/configurationSettingsService/03/GetWorkItemQueryTimeout";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/configurationSettingsService/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) 0;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.Int32FromXmlElement(reader);
    }

    internal sealed class GetWorkitemTrackingVersionClientOperation : TfsClientOperation
    {
      public override string BodyName => "GetWorkitemTrackingVersion";

      public override bool HasOutputs => true;

      public override string ResultName => "GetWorkitemTrackingVersionResult";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/configurationSettingsService/03/GetWorkitemTrackingVersion";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/configurationSettingsService/03";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.StringFromXmlElement(reader);
    }

    internal sealed class SetInProcBuildCompletionNotificationAvailabilityClientOperation : 
      TfsClientOperation
    {
      public override string BodyName => "SetInProcBuildCompletionNotificationAvailability";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/configurationSettingsService/03/SetInProcBuildCompletionNotificationAvailability";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/configurationSettingsService/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        bool parameter = (bool) parameters[0];
        if (!parameter)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "isEnabled", parameter);
      }
    }

    internal sealed class SetMaxAttachmentSizeClientOperation : TfsClientOperation
    {
      public override string BodyName => "SetMaxAttachmentSize";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/configurationSettingsService/03/SetMaxAttachmentSize";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/configurationSettingsService/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        long parameter = (long) parameters[0];
        if (parameter == 0L)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "maxSize", parameter);
      }
    }

    internal sealed class SetMaxBuildListSizeClientOperation : TfsClientOperation
    {
      public override string BodyName => "SetMaxBuildListSize";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/configurationSettingsService/03/SetMaxBuildListSize";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/configurationSettingsService/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        int parameter = (int) parameters[0];
        if (parameter == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "maxBuildListSize", parameter);
      }
    }

    internal sealed class SetWorkItemQueryTimeoutClientOperation : TfsClientOperation
    {
      public override string BodyName => "SetWorkItemQueryTimeout";

      public override string SoapAction => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/configurationSettingsService/03/SetWorkItemQueryTimeout";

      public override string SoapNamespace => "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/configurationSettingsService/03";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        int parameter = (int) parameters[0];
        if (parameter == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "workItemQueryTimeout", parameter);
      }
    }
  }
}
