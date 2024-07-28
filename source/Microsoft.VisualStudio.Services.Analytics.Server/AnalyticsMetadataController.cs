// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsMetadataController
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using Microsoft.VisualStudio.Services.Analytics.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http.Routing;

namespace Microsoft.VisualStudio.Services.Analytics
{
  [AnalyticsODataFormatting]
  [ODataRouting]
  [ValidateViewAnalyticsPermission]
  public class AnalyticsMetadataController : TfsProjectApiController
  {
    private static readonly Version _defaultEdmxVersion = new Version(4, 0);

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<AnalyticsAccessCheckException>(HttpStatusCode.Forbidden);
    }

    [PublicProjectRequestRestrictions]
    public IEdmModel GetMetadata()
    {
      this.Request.GetRequestContext().Url = (UrlHelper) new AnalyticsUrlHelper(this.Request);
      MetadataEngagementPublisher.PublishGetMetadataEvent(this.TfsRequestContext);
      return this.GetModel();
    }

    [PublicProjectRequestRestrictions]
    public ODataServiceDocument GetServiceDocument()
    {
      this.Request.GetRequestContext().Url = (UrlHelper) new AnalyticsUrlHelper(this.Request);
      MetadataEngagementPublisher.PublishGetServiceDocumentEvent(this.TfsRequestContext);
      IEdmModel model = this.GetModel();
      ODataServiceDocument serviceDocument = new ODataServiceDocument();
      IEdmEntityContainer entityContainer = model.EntityContainer;
      serviceDocument.EntitySets = entityContainer.EntitySets().Select<IEdmEntitySet, ODataEntitySetInfo>((Func<IEdmEntitySet, ODataEntitySetInfo>) (e => AnalyticsMetadataController.GetODataEntitySetInfo(e.Name, e.Name)));
      IEnumerable<IEdmSingleton> source1 = entityContainer.Elements.OfType<IEdmSingleton>();
      serviceDocument.Singletons = source1.Select<IEdmSingleton, ODataSingletonInfo>((Func<IEdmSingleton, ODataSingletonInfo>) (e => AnalyticsMetadataController.GetODataSingletonInfo(e.Name, e.Name)));
      IEnumerable<IEdmFunctionImport> source2 = entityContainer.Elements.OfType<IEdmFunctionImport>().Where<IEdmFunctionImport>((Func<IEdmFunctionImport, bool>) (f => !f.Function.Parameters.Any<IEdmOperationParameter>() && f.IncludeInServiceDocument));
      serviceDocument.FunctionImports = source2.Select<IEdmFunctionImport, string>((Func<IEdmFunctionImport, string>) (f => f.Name)).Distinct<string>().Select<string, ODataFunctionImportInfo>((Func<string, ODataFunctionImportInfo>) (f => AnalyticsMetadataController.GetODataFunctionImportInfo(f)));
      return serviceDocument;
    }

    private static ODataEntitySetInfo GetODataEntitySetInfo(string url, string name)
    {
      ODataEntitySetInfo odataEntitySetInfo = new ODataEntitySetInfo();
      odataEntitySetInfo.Name = name;
      odataEntitySetInfo.Url = new Uri(url, UriKind.Relative);
      return odataEntitySetInfo;
    }

    private static ODataSingletonInfo GetODataSingletonInfo(string url, string name)
    {
      ODataSingletonInfo odataSingletonInfo = new ODataSingletonInfo();
      odataSingletonInfo.Name = name;
      odataSingletonInfo.Url = new Uri(url, UriKind.Relative);
      return odataSingletonInfo;
    }

    private static ODataFunctionImportInfo GetODataFunctionImportInfo(string name)
    {
      ODataFunctionImportInfo functionImportInfo = new ODataFunctionImportInfo();
      functionImportInfo.Name = name;
      functionImportInfo.Url = new Uri(name, UriKind.Relative);
      return functionImportInfo;
    }

    private IEdmModel GetModel()
    {
      IEdmModel model = this.Request.GetModel();
      model.SetEdmxVersion(AnalyticsMetadataController._defaultEdmxVersion);
      return model;
    }
  }
}
