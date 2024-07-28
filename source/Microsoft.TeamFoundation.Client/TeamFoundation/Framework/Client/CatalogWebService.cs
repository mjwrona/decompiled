// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.CatalogWebService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Channels;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class CatalogWebService : TfsHttpClient
  {
    public CatalogWebService(TfsConfigurationServer connection)
      : base((TfsConnection) connection)
    {
    }

    protected override string ComponentName => "Framework";

    protected override Guid ConfigurationServiceIdentifier => new Guid("c2f9106f-127a-45b7-b0a3-e0ad8239a2a7");

    protected override string ServiceType => "CatalogService";

    protected override Exception ConvertException(SoapException exception) => TeamFoundationServiceException.ConvertException(exception);

    public CatalogData QueryDependents(string path, int queryOptions) => (CatalogData) this.Invoke((TfsClientOperation) new CatalogWebService.QueryDependentsClientOperation(), new object[2]
    {
      (object) path,
      (object) queryOptions
    });

    public CatalogData QueryNodes(
      string[] pathSpecs,
      Guid[] resourceTypeFilters,
      KeyValueOfStringString[] propertyFilters,
      int queryOptions)
    {
      return (CatalogData) this.Invoke((TfsClientOperation) new CatalogWebService.QueryNodesClientOperation(), new object[4]
      {
        (object) pathSpecs,
        (object) resourceTypeFilters,
        (object) propertyFilters,
        (object) queryOptions
      });
    }

    public CatalogData QueryParents(
      Guid resourceIdentifier,
      string[] pathFilters,
      Guid[] resourceTypeFilters,
      bool recurseToRoot,
      int queryOptions)
    {
      return (CatalogData) this.Invoke((TfsClientOperation) new CatalogWebService.QueryParentsClientOperation(), new object[5]
      {
        (object) resourceIdentifier,
        (object) pathFilters,
        (object) resourceTypeFilters,
        (object) recurseToRoot,
        (object) queryOptions
      });
    }

    public CatalogResourceType[] QueryResourceTypes(Guid[] resourceTypeIdentifiers) => (CatalogResourceType[]) this.Invoke((TfsClientOperation) new CatalogWebService.QueryResourceTypesClientOperation(), new object[1]
    {
      (object) resourceTypeIdentifiers
    });

    public CatalogData QueryResources(Guid[] resourceIdentifiers, int queryOptions) => (CatalogData) this.Invoke((TfsClientOperation) new CatalogWebService.QueryResourcesClientOperation(), new object[2]
    {
      (object) resourceIdentifiers,
      (object) queryOptions
    });

    public CatalogData QueryResourcesByType(
      Guid[] resourceTypes,
      KeyValueOfStringString[] propertyFilters,
      int queryOptions)
    {
      return (CatalogData) this.Invoke((TfsClientOperation) new CatalogWebService.QueryResourcesByTypeClientOperation(), new object[3]
      {
        (object) resourceTypes,
        (object) propertyFilters,
        (object) queryOptions
      });
    }

    public CatalogData SaveCatalogChanges(
      CatalogResource[] resources,
      CatalogNode[] nodes,
      KeyValueOfStringString[] nodeMoves,
      int queryOptions,
      bool preview)
    {
      return (CatalogData) this.Invoke((TfsClientOperation) new CatalogWebService.SaveCatalogChangesClientOperation(), new object[5]
      {
        (object) resources,
        (object) nodes,
        (object) nodeMoves,
        (object) queryOptions,
        (object) preview
      });
    }

    internal sealed class QueryDependentsClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryDependents";

      public override bool HasOutputs => true;

      public override string ResultName => "QueryDependentsResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueryDependents";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) CatalogData.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string parameter1 = (string) parameters[0];
        if (parameter1 != null)
          XmlUtility.ToXmlElement((XmlWriter) writer, "path", parameter1);
        int parameter2 = (int) parameters[1];
        if (parameter2 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "queryOptions", parameter2);
      }
    }

    internal sealed class QueryNodesClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryNodes";

      public override bool HasOutputs => true;

      public override string ResultName => "QueryNodesResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueryNodes";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) CatalogData.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        string[] parameter1 = (string[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "pathSpecs", parameter1, false, false);
        Guid[] parameter2 = (Guid[]) parameters[1];
        Helper.ToXml((XmlWriter) writer, "resourceTypeFilters", parameter2, false, false);
        KeyValueOfStringString[] parameter3 = (KeyValueOfStringString[]) parameters[2];
        Helper.ToXml((XmlWriter) writer, "propertyFilters", parameter3, false, false);
        int parameter4 = (int) parameters[3];
        if (parameter4 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "queryOptions", parameter4);
      }
    }

    internal sealed class QueryParentsClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryParents";

      public override bool HasOutputs => true;

      public override string ResultName => "QueryParentsResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueryParents";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) CatalogData.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid parameter1 = (Guid) parameters[0];
        if (parameter1 != Guid.Empty)
          XmlUtility.ToXmlElement((XmlWriter) writer, "resourceIdentifier", parameter1);
        string[] parameter2 = (string[]) parameters[1];
        Helper.ToXml((XmlWriter) writer, "pathFilters", parameter2, false, false);
        Guid[] parameter3 = (Guid[]) parameters[2];
        Helper.ToXml((XmlWriter) writer, "resourceTypeFilters", parameter3, false, false);
        bool parameter4 = (bool) parameters[3];
        if (parameter4)
          XmlUtility.ToXmlElement((XmlWriter) writer, "recurseToRoot", parameter4);
        int parameter5 = (int) parameters[4];
        if (parameter5 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "queryOptions", parameter5);
      }
    }

    internal sealed class QueryResourceTypesClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryResourceTypes";

      public override bool HasOutputs => true;

      public override string ResultName => "QueryResourceTypesResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueryResourceTypes";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) Helper.ZeroLengthArrayOfCatalogResourceType;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) Helper.ArrayOfCatalogResourceTypeFromXml(serviceProvider, reader, false);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid[] parameter = (Guid[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "resourceTypeIdentifiers", parameter, false, false);
      }
    }

    internal sealed class QueryResourcesByTypeClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryResourcesByType";

      public override bool HasOutputs => true;

      public override string ResultName => "QueryResourcesByTypeResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueryResourcesByType";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) CatalogData.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid[] parameter1 = (Guid[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "resourceTypes", parameter1, false, false);
        KeyValueOfStringString[] parameter2 = (KeyValueOfStringString[]) parameters[1];
        Helper.ToXml((XmlWriter) writer, "propertyFilters", parameter2, false, false);
        int parameter3 = (int) parameters[2];
        if (parameter3 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "queryOptions", parameter3);
      }
    }

    internal sealed class QueryResourcesClientOperation : TfsClientOperation
    {
      public override string BodyName => "QueryResources";

      public override bool HasOutputs => true;

      public override string ResultName => "QueryResourcesResult";

      public override string SoapAction => "http://microsoft.com/webservices/QueryResources";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) CatalogData.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        Guid[] parameter1 = (Guid[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "resourceIdentifiers", parameter1, false, false);
        int parameter2 = (int) parameters[1];
        if (parameter2 == 0)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "queryOptions", parameter2);
      }
    }

    internal sealed class SaveCatalogChangesClientOperation : TfsClientOperation
    {
      public override string BodyName => "SaveCatalogChanges";

      public override bool HasOutputs => true;

      public override string ResultName => "SaveCatalogChangesResult";

      public override string SoapAction => "http://microsoft.com/webservices/SaveCatalogChanges";

      public override string SoapNamespace => "http://microsoft.com/webservices/";

      public override object InitializeOutputs(out object[] outputs)
      {
        outputs = (object[]) null;
        return (object) null;
      }

      public override object ReadResult(IServiceProvider serviceProvider, XmlReader reader) => (object) CatalogData.FromXml(serviceProvider, reader);

      protected override void WriteParameters(XmlDictionaryWriter writer, object[] parameters)
      {
        CatalogResource[] parameter1 = (CatalogResource[]) parameters[0];
        Helper.ToXml((XmlWriter) writer, "resources", parameter1, false, false);
        CatalogNode[] parameter2 = (CatalogNode[]) parameters[1];
        Helper.ToXml((XmlWriter) writer, "nodes", parameter2, false, false);
        KeyValueOfStringString[] parameter3 = (KeyValueOfStringString[]) parameters[2];
        Helper.ToXml((XmlWriter) writer, "nodeMoves", parameter3, false, false);
        int parameter4 = (int) parameters[3];
        if (parameter4 != 0)
          XmlUtility.ToXmlElement((XmlWriter) writer, "queryOptions", parameter4);
        bool parameter5 = (bool) parameters[4];
        if (!parameter5)
          return;
        XmlUtility.ToXmlElement((XmlWriter) writer, "preview", parameter5);
      }
    }
  }
}
