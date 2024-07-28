// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Enums.DocumentContractTypeExtension
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common.Enums
{
  public static class DocumentContractTypeExtension
  {
    private static readonly object s_hashSetLock = new object();
    private static HashSet<DocumentContractType> s_coexistingDocumentContractTypes;

    public static string GetMappingName(this DocumentContractType contractType)
    {
      switch (contractType)
      {
        case DocumentContractType.Unsupported:
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("[{0}] does not have a mapping name.", (object) DocumentContractType.Unsupported)));
        case DocumentContractType.ProjectContract:
        case DocumentContractType.RepositoryContract:
          return "projectRepoContract";
        case DocumentContractType.WorkItemContract:
          return "workItemContract";
        case DocumentContractType.PackageVersionContract:
          return contractType.ToString().ToLowerInvariant();
        default:
          return contractType.ToString();
      }
    }

    public static bool IsValidCodeDocumentContractType(DocumentContractType contractType) => contractType == DocumentContractType.SourceNoDedupeFileContractV3 || contractType == DocumentContractType.SourceNoDedupeFileContractV4 || contractType == DocumentContractType.SourceNoDedupeFileContractV5 || contractType == DocumentContractType.DedupeFileContractV3 || contractType == DocumentContractType.DedupeFileContractV4 || contractType == DocumentContractType.DedupeFileContractV5;

    public static bool IsSourceOnAndApplicable(this DocumentContractType contractType) => !DocumentContractTypeExtension.IsValidCodeDocumentContractType(contractType);

    public static bool IsNoPayloadContract(this DocumentContractType contractType) => contractType == DocumentContractType.SourceNoDedupeFileContractV4 || contractType == DocumentContractType.SourceNoDedupeFileContractV5 || contractType == DocumentContractType.DedupeFileContractV4 || contractType == DocumentContractType.DedupeFileContractV5;

    public static bool IsStoredFieldsOn(this DocumentContractType contractType) => DocumentContractTypeExtension.IsValidCodeDocumentContractType(contractType);

    public static int GetShardDensity(
      this DocumentContractType contractType,
      IVssRequestContext requestContext)
    {
      switch (contractType)
      {
        case DocumentContractType.SourceNoDedupeFileContractV3:
          return requestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/Routing/Code/ShardDensity", true, 40000);
        case DocumentContractType.DedupeFileContractV3:
        case DocumentContractType.DedupeFileContractV4:
        case DocumentContractType.DedupeFileContractV5:
          return requestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/Routing/Code/DedupeFileContractShardDensity", true, 10000);
        case DocumentContractType.WikiContract:
        case DocumentContractType.BoardContract:
          return -1;
        case DocumentContractType.SourceNoDedupeFileContractV4:
        case DocumentContractType.SourceNoDedupeFileContractV5:
          return requestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/Routing/Code/CodeEntityShardDensityForSourceNoDedupeFileContractV4", true, 32000);
        default:
          throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("{0} not supported to fetch shard density.", (object) contractType)));
      }
    }

    public static int GetMaxNumberOfDocumentsInAShard(
      this DocumentContractType contractType,
      IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (DocumentContractTypeExtension.IsValidCodeDocumentContractType(contractType))
      {
        long configValueOrDefault = (long) requestContext.To(TeamFoundationHostType.Deployment).GetConfigValueOrDefault("/Service/ALMSearch/Settings/Routing/Code/MaxShardSizeInBytes", 42949672960.0);
        return contractType.GetEstimatedDocumentCountFromEstimatedSize(requestContext, configValueOrDefault);
      }
      if (contractType == DocumentContractType.WikiContract)
        return -1;
      throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("{0} not supported to fetch max number of documents in a shard.", (object) contractType)));
    }

    public static int GetEstimatedDocumentCountFromEstimatedSize(
      this DocumentContractType contractType,
      IVssRequestContext requestContext,
      long sizeInBytes)
    {
      int shardDensity = contractType.GetShardDensity(requestContext);
      return (int) ((double) checked (sizeInBytes * (long) shardDensity) / 1073741824.0);
    }

    public static double GetFileCountFactorForMultibranch(
      this DocumentContractType contractType,
      IVssRequestContext requestContext,
      IndexingUnit indexingUnit,
      int branchCount)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (branchCount <= 0)
        throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Branch count can not be {0} for any repository.", (object) branchCount.ToString((IFormatProvider) CultureInfo.InvariantCulture))));
      switch (contractType)
      {
        case DocumentContractType.SourceNoDedupeFileContractV3:
        case DocumentContractType.SourceNoDedupeFileContractV4:
        case DocumentContractType.SourceNoDedupeFileContractV5:
          return 1.0;
        case DocumentContractType.DedupeFileContractV3:
        case DocumentContractType.DedupeFileContractV4:
        case DocumentContractType.DedupeFileContractV5:
          double currentHostConfigValue = requestContext.GetCurrentHostConfigValue<double>("/Service/ALMSearch/Settings/Routing/Code/FileCountIncFactorOnDeduplication", true, 0.10000000149011612);
          if (indexingUnit.IsLargeRepository(requestContext) && indexingUnit.IndexingUnitType == "Git_Repository")
            currentHostConfigValue /= 10.0;
          return (1.0 + currentHostConfigValue * (double) (branchCount - 1)) / (double) branchCount;
        default:
          throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("{0} not supported to get the file count factor for multi branch", (object) contractType)));
      }
    }

    public static string GetDocumentContractTypeCategory(this DocumentContractType contractType)
    {
      switch (contractType)
      {
        case DocumentContractType.SourceNoDedupeFileContractV3:
        case DocumentContractType.SourceNoDedupeFileContractV4:
        case DocumentContractType.SourceNoDedupeFileContractV5:
          return "SourceNoDedupeFileContract";
        case DocumentContractType.DedupeFileContractV3:
        case DocumentContractType.DedupeFileContractV4:
        case DocumentContractType.DedupeFileContractV5:
          return "DedupeFileContract";
        default:
          return "Unknown";
      }
    }

    public static bool IsDedupeFileContract(this DocumentContractType contractType) => contractType.GetDocumentContractTypeCategory() == "DedupeFileContract";

    public static bool IsSourceNoDedupeFileContract(this DocumentContractType contractType) => contractType.GetDocumentContractTypeCategory() == "SourceNoDedupeFileContract";

    public static string GetRegistryKeyForDocumentContract(this DocumentContractType contractType)
    {
      if (contractType.GetDocumentContractTypeCategory() == "Unknown")
        throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("{0} not supported by GetRegistryKeyForDocumentContract", (object) contractType)));
      return "/Service/ALMSearch/Settings/" + contractType.GetDocumentContractTypeCategory() + "Mapping";
    }

    public static IEntityType GetEntityTypeForContractType(
      this DocumentContractType documentContractType)
    {
      switch (documentContractType)
      {
        case DocumentContractType.ProjectContract:
        case DocumentContractType.RepositoryContract:
          return (IEntityType) ProjectRepoEntityType.GetInstance();
        case DocumentContractType.WorkItemContract:
          return (IEntityType) WorkItemEntityType.GetInstance();
        case DocumentContractType.SourceNoDedupeFileContractV3:
        case DocumentContractType.DedupeFileContractV3:
        case DocumentContractType.SourceNoDedupeFileContractV4:
        case DocumentContractType.DedupeFileContractV4:
        case DocumentContractType.SourceNoDedupeFileContractV5:
        case DocumentContractType.DedupeFileContractV5:
          return (IEntityType) CodeEntityType.GetInstance();
        case DocumentContractType.WikiContract:
          return (IEntityType) WikiEntityType.GetInstance();
        case DocumentContractType.PackageVersionContract:
          return (IEntityType) PackageEntityType.GetInstance();
        case DocumentContractType.BoardContract:
          return (IEntityType) BoardEntityType.GetInstance();
        case DocumentContractType.SettingContract:
          return (IEntityType) SettingEntityType.GetInstance();
        default:
          throw new SearchServiceException("Unsupported document contract type - " + documentContractType.ToString());
      }
    }

    public static IEnumerable<DocumentContractType> GetSupportedContractTypes(
      this IEntityType entityType)
    {
      switch (entityType)
      {
        case CodeEntityType _:
          return (IEnumerable<DocumentContractType>) new List<DocumentContractType>()
          {
            DocumentContractType.DedupeFileContractV3,
            DocumentContractType.DedupeFileContractV4,
            DocumentContractType.DedupeFileContractV5,
            DocumentContractType.SourceNoDedupeFileContractV3,
            DocumentContractType.SourceNoDedupeFileContractV4,
            DocumentContractType.SourceNoDedupeFileContractV5
          };
        case ProjectRepoEntityType _:
          return (IEnumerable<DocumentContractType>) new List<DocumentContractType>()
          {
            DocumentContractType.ProjectContract,
            DocumentContractType.RepositoryContract
          };
        case WorkItemEntityType _:
          return (IEnumerable<DocumentContractType>) new List<DocumentContractType>()
          {
            DocumentContractType.WorkItemContract
          };
        case WikiEntityType _:
          return (IEnumerable<DocumentContractType>) new List<DocumentContractType>()
          {
            DocumentContractType.WikiContract
          };
        case PackageEntityType _:
          return (IEnumerable<DocumentContractType>) new List<DocumentContractType>()
          {
            DocumentContractType.PackageVersionContract
          };
        case BoardEntityType _:
          return (IEnumerable<DocumentContractType>) new List<DocumentContractType>()
          {
            DocumentContractType.BoardContract
          };
        case SettingEntityType _:
          return (IEnumerable<DocumentContractType>) new List<DocumentContractType>()
          {
            DocumentContractType.SettingContract
          };
        default:
          throw new SearchServiceException("Unsupported entity type - " + entityType?.ToString());
      }
    }

    public static DocumentContractType GetChildContractType(
      this DocumentContractType documentContractType)
    {
      return documentContractType == DocumentContractType.ProjectContract ? DocumentContractType.RepositoryContract : DocumentContractType.Unsupported;
    }

    public static IList<DocumentContractType> GetDocumentContractTypes(
      this string mappingName,
      bool ignoreCase = true)
    {
      List<DocumentContractType> documentContractTypes = new List<DocumentContractType>();
      foreach (DocumentContractType contractType in Enum.GetValues(typeof (DocumentContractType)).Cast<DocumentContractType>())
      {
        if (contractType != DocumentContractType.Unsupported && contractType.GetMappingName().Equals(mappingName, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
          documentContractTypes.Add(contractType);
      }
      return (IList<DocumentContractType>) documentContractTypes;
    }

    public static bool CoExistsWithOtherContractTypes(this DocumentContractType contractType)
    {
      if (DocumentContractTypeExtension.s_coexistingDocumentContractTypes == null)
      {
        lock (DocumentContractTypeExtension.s_hashSetLock)
        {
          if (DocumentContractTypeExtension.s_coexistingDocumentContractTypes == null)
          {
            DocumentContractTypeExtension.s_coexistingDocumentContractTypes = new HashSet<DocumentContractType>();
            FriendlyDictionary<string, List<DocumentContractType>> friendlyDictionary = new FriendlyDictionary<string, List<DocumentContractType>>();
            foreach (DocumentContractType contractType1 in Enum.GetValues(typeof (DocumentContractType)).Cast<DocumentContractType>())
            {
              if (contractType1 != DocumentContractType.Unsupported)
              {
                string mappingName = contractType1.GetMappingName();
                List<DocumentContractType> values;
                if (friendlyDictionary.TryGetValue(mappingName, out values))
                {
                  values.Add(contractType1);
                  if (values.Count > 1)
                    DocumentContractTypeExtension.s_coexistingDocumentContractTypes.AddRange<DocumentContractType, HashSet<DocumentContractType>>((IEnumerable<DocumentContractType>) values);
                }
                else
                  friendlyDictionary.Add(mappingName, new List<DocumentContractType>()
                  {
                    contractType1
                  });
              }
            }
          }
        }
      }
      return DocumentContractTypeExtension.s_coexistingDocumentContractTypes.Contains(contractType);
    }
  }
}
