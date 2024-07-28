// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionQueryController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension;
using Microsoft.VisualStudio.Services.Gallery.Server.SoapService;
using Microsoft.VisualStudio.Services.Gallery.Server.Utility;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "extensionquery")]
  public class ExtensionQueryController : GalleryController
  {
    [HttpPost]
    [ClientHeaderParameter("X-Market-AccountToken", typeof (string), "accountTokenHeader", "Header to pass the account token", true, true)]
    public ExtensionQueryResult QueryExtensions(ExtensionQuery extensionQuery, string accountToken = null)
    {
      int minValue = int.MinValue;
      ExtensionQuery searchQuery = (ExtensionQuery) null;
      if (extensionQuery == null)
        throw new ArgumentNullException(nameof (extensionQuery)).Expected("Gallery");
      if (extensionQuery.Filters == null)
        throw new ArgumentNullException("extensionQuery.Filters").Expected("Gallery");
      accountToken = GalleryServerUtil.TryUseAccountTokenFromHttpHeader(this.TfsRequestContext, this.Request?.Headers, "ExtensionQueryController.QueryExtensions", accountToken);
      try
      {
        if (this.TfsRequestContext.UserAgent != null)
        {
          if (this.TfsRequestContext.UserAgent.StartsWith("VSIDE", StringComparison.OrdinalIgnoreCase))
          {
            if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVsGzipTracing") && this.ControllerContext.Request.Headers.AcceptEncoding.Count == 0)
              this.TfsRequestContext.TraceAlways(10262027, TraceLevel.Error, "gallery", "ExtensionQueryController.QueryExtensions", "Call from VS IDE received without accept-encoding header");
            IDictionary<string, BackConsolidationMappingEntry> backConsolidatedVsixIds = (IDictionary<string, BackConsolidationMappingEntry>) null;
            if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableBackConsolidation"))
            {
              List<string> vsixIds = this.ExtractVsixIds(extensionQuery);
              if (vsixIds != null && vsixIds.Count != 0)
              {
                backConsolidatedVsixIds = this.FilterVsixIdsFromCache(vsixIds);
                if (backConsolidatedVsixIds != null && backConsolidatedVsixIds.Count > 0)
                  this.ReplaceVsixIdsInExtensionQuery(extensionQuery, backConsolidatedVsixIds);
              }
            }
            ExtensionQueryResult finalResult = this.TfsRequestContext.GetService<IVisualStudioApiService>().SearchForVsIde(this.TfsRequestContext, extensionQuery);
            if (finalResult != null && finalResult.Results.Any<ExtensionFilterResult>() && finalResult.Results[0] != null)
            {
              finalResult.Results[0].Extensions = GalleryServerUtil.SetEffectiveDisplayedStarRating(this.TfsRequestContext, finalResult.Results[0].Extensions);
              if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableBackConsolidation") && backConsolidatedVsixIds != null && backConsolidatedVsixIds.Count > 0)
                this.AddRequestedVsixIdsReferenceInResponseProperties(finalResult, backConsolidatedVsixIds.Values.ToList<BackConsolidationMappingEntry>());
            }
            return finalResult;
          }
        }
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceAlways(12062044, TraceLevel.Error, "gallery", "ExtensionQueryController.QueryExtensions", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ExtensionQuery {0}", (object) extensionQuery.Serialize<ExtensionQuery>()));
        throw;
      }
      this.SplitQueryForSearch(ref extensionQuery, ref searchQuery, ref minValue);
      ExtensionQueryResult extensionQueryResult = this.CallAndMergeResults(extensionQuery, searchQuery, minValue, accountToken);
      if (extensionQueryResult != null && extensionQueryResult.Results.Any<ExtensionFilterResult>() && extensionQueryResult.Results[0] != null)
        extensionQueryResult.Results[0].Extensions = GalleryServerUtil.SetEffectiveDisplayedStarRating(this.TfsRequestContext, extensionQueryResult.Results[0].Extensions);
      return extensionQueryResult;
    }

    private IDictionary<string, BackConsolidationMappingEntry> FilterVsixIdsFromCache(
      List<string> vsixIds)
    {
      IReadOnlyDictionary<string, BackConsolidationMappingEntry> consolidationList = this.TfsRequestContext.GetService<IPublishedExtensionService>().GetBackConsolidationList(this.TfsRequestContext);
      IDictionary<string, BackConsolidationMappingEntry> dictionary = (IDictionary<string, BackConsolidationMappingEntry>) new Dictionary<string, BackConsolidationMappingEntry>();
      for (int index = 0; index < vsixIds.Count; ++index)
      {
        BackConsolidationMappingEntry consolidationMappingEntry;
        consolidationList.TryGetValue(vsixIds[index], out consolidationMappingEntry);
        if (consolidationMappingEntry != null)
          dictionary.TryAdd<string, BackConsolidationMappingEntry>(vsixIds[index], consolidationMappingEntry);
      }
      return dictionary;
    }

    private void AddRequestedVsixIdsReferenceInResponseProperties(
      ExtensionQueryResult finalResult,
      List<BackConsolidationMappingEntry> backConsolidatedVsixIds)
    {
      foreach (ExtensionFilterResult result in finalResult.Results)
      {
        foreach (PublishedExtension extension in result.Extensions)
        {
          Guid extensionId = extension.ExtensionId;
          foreach (BackConsolidationMappingEntry consolidatedVsixId in backConsolidatedVsixIds)
          {
            if (consolidatedVsixId.TargetExtensionId.Equals(extensionId))
              extension.Versions[0].Properties?.Add(new KeyValuePair<string, string>("PreConsolidationVsixId", consolidatedVsixId.SourceExtensionVsixId));
          }
        }
      }
    }

    private void ReplaceVsixIdsInExtensionQuery(
      ExtensionQuery extensionQuery,
      IDictionary<string, BackConsolidationMappingEntry> backConsolidatedVsixIds)
    {
      foreach (QueryFilter filter in extensionQuery.Filters)
      {
        foreach (FilterCriteria criterion in filter.Criteria)
        {
          if (criterion.FilterType == 17)
          {
            string key = criterion.Value.Substring(criterion.Value.IndexOf("=") + 1);
            BackConsolidationMappingEntry consolidationMappingEntry;
            backConsolidatedVsixIds.TryGetValue(key, out consolidationMappingEntry);
            if (consolidationMappingEntry != null)
              criterion.Value = string.Join("=", new string[2]
              {
                "VsixId",
                consolidationMappingEntry.TargetExtensionVsixId
              });
          }
        }
      }
    }

    private List<string> ExtractVsixIds(ExtensionQuery extensionQuery)
    {
      List<string> vsixIds = new List<string>();
      foreach (QueryFilter filter in extensionQuery.Filters)
      {
        foreach (FilterCriteria criterion in filter.Criteria)
        {
          if (criterion.FilterType == 17 && criterion.Value.StartsWith("VsixId", StringComparison.OrdinalIgnoreCase))
            vsixIds.Add(criterion.Value.Substring(criterion.Value.IndexOf("=") + 1));
        }
      }
      return vsixIds;
    }

    private void SplitQueryForSearch(
      ref ExtensionQuery extensionQuery,
      ref ExtensionQuery searchQuery,
      ref int searchFilterIdx)
    {
      for (int index = 0; index < extensionQuery.Filters.Count; ++index)
      {
        QueryFilter filter = extensionQuery.Filters[index];
        if (filter.Criteria == null)
          throw new ArgumentNullException("extensionQuery.Filters[" + index.ToString() + "].Criteria");
        foreach (FilterCriteria criterion in filter.Criteria)
        {
          if (criterion.FilterType == 10)
          {
            if (searchFilterIdx != int.MinValue)
              throw new NotSupportedException("More than one filters with search text criteria not supported");
            searchFilterIdx = index;
          }
        }
      }
      if (searchFilterIdx == int.MinValue)
        return;
      if (extensionQuery.Filters.Count == 1)
      {
        searchQuery = extensionQuery;
        extensionQuery = (ExtensionQuery) null;
      }
      else
      {
        searchQuery = new ExtensionQuery();
        searchQuery.Flags = extensionQuery.Flags;
        searchQuery.Filters = new List<QueryFilter>();
        QueryFilter filter = extensionQuery.Filters[searchFilterIdx];
        extensionQuery.Filters.RemoveAt(searchFilterIdx);
        searchQuery.Filters.Add(filter);
      }
    }

    private ExtensionQueryResult CallAndMergeResults(
      ExtensionQuery extensionQuery,
      ExtensionQuery searchQuery,
      int searchFilterIdx,
      string accountToken)
    {
      try
      {
        IPublishedExtensionService service = this.TfsRequestContext.GetService<IPublishedExtensionService>();
        ExtensionQueryResult queryResult = (ExtensionQueryResult) null;
        ExtensionQueryResult extensionQueryResult = (ExtensionQueryResult) null;
        bool flag = false;
        if (extensionQuery != null)
        {
          queryResult = service.QueryExtensions(this.TfsRequestContext, extensionQuery, accountToken, this.Request?.Headers?.Referrer);
          if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.MaskCertifiedPublisherFlagForOnPremQuery"))
            this.UnsetPublisherCertifiedFlag(ref queryResult);
          if (extensionQuery.Flags.HasFlag((Enum) ExtensionQueryFlags.IncludeNameConflictInfo))
            flag = true;
        }
        if (searchQuery != null)
        {
          extensionQueryResult = service.SearchExtensions(this.TfsRequestContext, searchQuery, accountToken);
          if (searchQuery.Flags.HasFlag((Enum) ExtensionQueryFlags.IncludeNameConflictInfo))
            flag = true;
        }
        ExtensionQueryResult result;
        if (queryResult == null)
          result = extensionQueryResult;
        else if (extensionQueryResult == null)
        {
          result = queryResult;
        }
        else
        {
          result = queryResult;
          result.Results.Insert(searchFilterIdx, extensionQueryResult.Results[0]);
        }
        if (flag)
          this.AddConflictNameInformation(result);
        return result;
      }
      catch (Exception ex)
      {
        if (extensionQuery != null)
        {
          this.ReplaceFilterValueInExtensionQuery(extensionQuery, ExtensionQueryFilterType.Private, "JWT token is hidden in trace for security reasons");
          this.TfsRequestContext.TraceAlways(10262028, TraceLevel.Error, "gallery", "ExtensionQueryController.QueryExtensions", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ExtensionQuery {0}", (object) extensionQuery.Serialize<ExtensionQuery>()));
        }
        if (searchQuery != null)
        {
          this.ReplaceFilterValueInExtensionQuery(searchQuery, ExtensionQueryFilterType.Private, "JWT token is hidden in trace for security reasons");
          this.TfsRequestContext.TraceAlways(10262028, TraceLevel.Error, "gallery", "ExtensionQueryController.QueryExtensions", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SearchQuery {0}", (object) searchQuery.Serialize<ExtensionQuery>()));
        }
        throw;
      }
    }

    private void AddConflictNameInformation(ExtensionQueryResult result)
    {
      IExtensionNameConflictService service = this.TfsRequestContext.GetService<IExtensionNameConflictService>();
      foreach (ExtensionFilterResult result1 in result.Results)
      {
        foreach (PublishedExtension extension in result1.Extensions)
        {
          if (service.ExtensionNameInConflictList(extension.Publisher.PublisherName + "." + extension.ExtensionName))
            extension.PresentInConflictList = "True";
        }
      }
    }

    private void ReplaceFilterValueInExtensionQuery(
      ExtensionQuery extensionQuery,
      ExtensionQueryFilterType filterType,
      string newFilterValue)
    {
      if (extensionQuery?.Filters == null)
        return;
      foreach (QueryFilter filter in extensionQuery.Filters)
      {
        if (filter.Criteria != null)
        {
          foreach (FilterCriteria criterion in filter.Criteria)
          {
            if ((ExtensionQueryFilterType) criterion.FilterType == filterType)
              criterion.Value = newFilterValue;
          }
        }
      }
    }

    private void UnsetPublisherCertifiedFlag(ref ExtensionQueryResult queryResult)
    {
      MediaTypeWithQualityHeaderValue qualityHeaderValue = new MediaTypeWithQualityHeaderValue("application/json");
      qualityHeaderValue.Parameters.Add(new NameValueHeaderValue("api-version", "3.0-preview.1"));
      string enumerable1 = this.Request?.Headers?.UserAgent == null || this.Request.Headers.UserAgent.Count <= 0 ? "" : this.Request.Headers.UserAgent.ElementAt<ProductInfoHeaderValue>(0).ToString();
      string enumerable2 = this.Request?.Headers?.UserAgent == null || this.Request.Headers.UserAgent.Count <= 1 ? "" : this.Request.Headers.UserAgent.ElementAt<ProductInfoHeaderValue>(1).ToString();
      bool flag = false;
      if (!enumerable1.IsNullOrEmpty<char>() && !enumerable2.IsNullOrEmpty<char>())
      {
        string[] strArray = enumerable1.Split('/');
        if (strArray.Length == 2)
        {
          Version result = new Version();
          if (Version.TryParse(strArray[1], out result) && result.Major >= 16 && result.Minor >= 142 && strArray[0].Equals("VSServices", StringComparison.OrdinalIgnoreCase) && enumerable2.Equals("(TfsJobAgent.exe)", StringComparison.OrdinalIgnoreCase))
            flag = true;
        }
      }
      if (flag || this.Request?.Headers?.Accept == null || !this.Request.Headers.Accept.Contains(qualityHeaderValue) || queryResult?.Results == null)
        return;
      foreach (ExtensionFilterResult result in queryResult.Results)
      {
        foreach (PublishedExtension extension in result.Extensions)
        {
          if (extension?.Publisher != null)
            extension.Publisher.Flags &= ~PublisherFlags.Certified;
        }
      }
    }
  }
}
