// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.RegistryWebService
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
  internal class RegistryWebService : TfsHttpClient
  {
    public RegistryWebService(TfsConnection connection)
      : base(connection)
    {
    }

    protected override Guid CollectionServiceIdentifier => new Guid("9ebadf24-f99a-42f5-a615-7598045b47dd");

    protected override string ComponentName => "Framework";

    protected override Guid ConfigurationServiceIdentifier => new Guid("48df7c8f-5554-4a80-80b8-7c205ce39b7a");

    protected override string ServiceType => "RegistryService";

    protected override Exception ConvertException(SoapException exception) => TeamFoundationServiceException.ConvertException(exception);

    public RegistryAuditEntry[] QueryAuditLog(int changeIndex, bool returnOlder) => (RegistryAuditEntry[]) this.Invoke((TfsClientOperation) new RegistryWebService.QueryAuditLogClientOperation(), new object[2]
    {
      (object) changeIndex,
      (object) returnOlder
    });

    public RegistryEntry[] QueryRegistryEntries(string registryPathPattern, bool includeFolders) => (RegistryEntry[]) this.Invoke((TfsClientOperation) new RegistryWebService.QueryRegistryEntriesClientOperation(), new object[2]
    {
      (object) registryPathPattern,
      (object) includeFolders
    });

    public RegistryEntry[] QueryUserEntries(string registryPathPattern, bool includeFolders) => (RegistryEntry[]) this.Invoke((TfsClientOperation) new RegistryWebService.QueryUserEntriesClientOperation(), new object[2]
    {
      (object) registryPathPattern,
      (object) includeFolders
    });

    public int RemoveRegistryEntries(string[] registryPathPatterns) => (int) this.Invoke((TfsClientOperation) new RegistryWebService.RemoveRegistryEntriesClientOperation(), new object[1]
    {
      (object) registryPathPatterns
    });

    public int RemoveUserEntries(string[] registryPathPatterns) => (int) this.Invoke((TfsClientOperation) new RegistryWebService.RemoveUserEntriesClientOperation(), new object[1]
    {
      (object) registryPathPatterns
    });

    public void UpdateRegistryEntries(IEnumerable<RegistryEntry> registryEntries) => this.Invoke((TfsClientOperation) new RegistryWebService.UpdateRegistryEntriesClientOperation(), new object[1]
    {
      (object) registryEntries
    });

    public void UpdateUserEntries(IEnumerable<RegistryEntry> registryEntries) => this.Invoke((TfsClientOperation) new RegistryWebService.UpdateUserEntriesClientOperation(), new object[1]
    {
      (object) registryEntries
    });

    internal sealed class QueryAuditLogClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryAuditLog";

      public override bool HasOutputs => true;

      public override string ResultName => "QueryAuditLogResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueryAuditLog";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfRegistryAuditEntry;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfRegistryAuditEntryFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        int parameter1 = (int) parameters[0];
        if (parameter1 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "changeIndex", parameter1);
        bool parameter2 = (bool) parameters[1];
        if (!parameter2)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "returnOlder", parameter2);
      }
    }

    internal sealed class QueryRegistryEntriesClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryRegistryEntries";

      public override bool HasOutputs => true;

      public override string ResultName => "QueryRegistryEntriesResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueryRegistryEntries";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfRegistryEntry;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfRegistryEntryFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "registryPathPattern", parameter1);
        bool parameter2 = (bool) parameters[1];
        if (!parameter2)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "includeFolders", parameter2);
      }
    }

    internal sealed class QueryUserEntriesClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryUserEntries";

      public override bool HasOutputs => true;

      public override string ResultName => "QueryUserEntriesResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueryUserEntries";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfRegistryEntry;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfRegistryEntryFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "registryPathPattern", parameter1);
        bool parameter2 = (bool) parameters[1];
        if (!parameter2)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "includeFolders", parameter2);
      }
    }

    internal sealed class RemoveRegistryEntriesClientOperation : TfsClientOperation
    {
      public override string BodyName => "RemoveRegistryEntries";

      public override bool HasOutputs => true;

      public override string ResultName => "RemoveRegistryEntriesResult";

      public override string SoapAction => "http://microsoft.com/webservices/RemoveRegistryEntries";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) 0;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.Int32FromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string[] parameter = (string[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "registryPathPatterns", parameter, false, false);
      }
    }

    internal sealed class RemoveUserEntriesClientOperation : TfsClientOperation
    {
      public override string BodyName => "RemoveUserEntries";

      public override bool HasOutputs => true;

      public override string ResultName => "RemoveUserEntriesResult";

      public override string SoapAction => "http://microsoft.com/webservices/RemoveUserEntries";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) 0;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) XmlUtility.Int32FromXmlElement(reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string[] parameter = (string[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "registryPathPatterns", parameter, false, false);
      }
    }

    internal sealed class UpdateRegistryEntriesClientOperation : TfsClientOperation
    {
      public override string BodyName => "UpdateRegistryEntries";

      public override string SoapAction => "http://microsoft.com/webservices/UpdateRegistryEntries";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        IEnumerable<RegistryEntry> parameter = (IEnumerable<RegistryEntry>) parameters[0];
        Helper.ToXml((XmlWriter) writer, "registryEntries", parameter, false, false);
      }
    }

    internal sealed class UpdateUserEntriesClientOperation : TfsClientOperation
    {
      public override string BodyName => "UpdateUserEntries";

      public override string SoapAction => "http://microsoft.com/webservices/UpdateUserEntries";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        IEnumerable<RegistryEntry> parameter = (IEnumerable<RegistryEntry>) parameters[0];
        Helper.ToXml((XmlWriter) writer, "registryEntries", parameter, false, false);
      }
    }
  }
}
