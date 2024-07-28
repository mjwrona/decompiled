// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.DocumentContractTypeServiceHelper
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public static class DocumentContractTypeServiceHelper
  {
    public static DocumentContractType GetDefaultDocumentContractType(
      IVssRequestContext requestContext,
      IEntityType entityType,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      switch (entityType.Name)
      {
        case "Code":
          try
          {
            DocumentContractType indexContractType = (DocumentContractType) Enum.Parse(typeof (DocumentContractType), requestContext.GetConfigValue("/Service/ALMSearch/Settings/DefaultCodeDocumentContractType"));
            CollectionIndexingProperties properties = indexingUnit?.Properties as CollectionIndexingProperties;
            if (indexingUnit != null && properties != null)
              indexContractType = properties.IndexContractType;
            return DocumentContractTypeServiceHelper.GetLatestDocumentContractTypeForGivenDocumentContract(requestContext, indexContractType);
          }
          catch
          {
            return DocumentContractType.SourceNoDedupeFileContractV3;
          }
        case "ProjectRepo":
          try
          {
            return (DocumentContractType) Enum.Parse(typeof (DocumentContractType), requestContext.GetConfigValue("/Service/ALMSearch/Settings/ProjectDocumentContractType"));
          }
          catch
          {
            return DocumentContractType.ProjectContract;
          }
        case "WorkItem":
          try
          {
            return (DocumentContractType) Enum.Parse(typeof (DocumentContractType), requestContext.GetConfigValue("/Service/ALMSearch/Settings/WorkItemDocumentContractType"));
          }
          catch
          {
            return DocumentContractType.WorkItemContract;
          }
        case "Wiki":
          try
          {
            return (DocumentContractType) Enum.Parse(typeof (DocumentContractType), requestContext.GetConfigValue("/Service/ALMSearch/Settings/WikiDocumentContractType"));
          }
          catch
          {
            return DocumentContractType.WikiContract;
          }
        case "Package":
          try
          {
            return (DocumentContractType) Enum.Parse(typeof (DocumentContractType), requestContext.GetConfigValue("/Service/ALMSearch/Settings/PackageVersionDocumentContractType"));
          }
          catch
          {
            return DocumentContractType.PackageVersionContract;
          }
        case "Board":
          return DocumentContractType.BoardContract;
        default:
          throw new IndexerException(FormattableString.Invariant(FormattableStringFactory.Create("DocumentContractTypeExtension::GetLatestDocumentContractType - Unsupported entity type '{0}'", (object) entityType.Name)));
      }
    }

    internal static DocumentContractType GetLatestDocumentContractTypeForGivenDocumentContract(
      IVssRequestContext requestContext,
      DocumentContractType documentContractType)
    {
      string documentContract1 = documentContractType.GetRegistryKeyForDocumentContract();
      string configValue = requestContext.GetConfigValue(documentContract1);
      DocumentContractType documentContract2 = documentContractType;
      try
      {
        if (configValue != null)
          documentContract2 = (DocumentContractType) Enum.Parse(typeof (DocumentContractType), configValue);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(new TraceMetaData(1080227, "Indexing Pipeline", "IndexingOperation"), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Exception caught while parsing the latest DocumentContractType fetched from registry,so going with {0} which is taken from existing contract Type/Default Contract Type.Following exception occurred {1}", (object) documentContract2, (object) ex));
      }
      return documentContract2;
    }
  }
}
