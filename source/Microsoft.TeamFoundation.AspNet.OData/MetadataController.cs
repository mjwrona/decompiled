// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.MetadataController
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData
{
  public class MetadataController : ODataController
  {
    private static readonly Version _defaultEdmxVersion = new Version(4, 0);

    public IEdmModel GetMetadata() => this.GetModel();

    public ODataServiceDocument GetServiceDocument()
    {
      IEdmModel model = this.GetModel();
      ODataServiceDocument serviceDocument = new ODataServiceDocument();
      IEdmEntityContainer entityContainer = model.EntityContainer;
      serviceDocument.EntitySets = entityContainer.EntitySets().Select<IEdmEntitySet, ODataEntitySetInfo>((Func<IEdmEntitySet, ODataEntitySetInfo>) (e => MetadataController.GetODataEntitySetInfo(model.GetNavigationSourceUrl((IEdmNavigationSource) e).ToString(), e.Name)));
      serviceDocument.Singletons = entityContainer.Elements.OfType<IEdmSingleton>().Select<IEdmSingleton, ODataSingletonInfo>((Func<IEdmSingleton, ODataSingletonInfo>) (e => MetadataController.GetODataSingletonInfo(model.GetNavigationSourceUrl((IEdmNavigationSource) e).ToString(), e.Name)));
      serviceDocument.FunctionImports = entityContainer.Elements.OfType<IEdmFunctionImport>().Where<IEdmFunctionImport>((Func<IEdmFunctionImport, bool>) (f => !f.Function.Parameters.Any<IEdmOperationParameter>() && f.IncludeInServiceDocument)).Distinct<IEdmFunctionImport>((IEqualityComparer<IEdmFunctionImport>) new FunctionImportComparer()).Select<IEdmFunctionImport, ODataFunctionImportInfo>((Func<IEdmFunctionImport, ODataFunctionImportInfo>) (f => MetadataController.GetODataFunctionImportInfo(f.Name)));
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
      if (model == null)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.RequestMustHaveModel);
      model.SetEdmxVersion(MetadataController._defaultEdmxVersion);
      return model;
    }
  }
}
