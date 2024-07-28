// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.ProjectRepoQueryFields
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders
{
  public static class ProjectRepoQueryFields
  {
    public static IDictionary<string, double> GetProjectQueryFieldsBoostValueMap(
      IVssRequestContext requestContext)
    {
      Dictionary<string, double> fieldsBoostValueMap = new Dictionary<string, double>();
      int configValueOrDefault1 = requestContext.GetConfigValueOrDefault("/Service/OrgSearch/Settings/ProjectRepoProjectNameBoost", 30);
      int configValueOrDefault2 = requestContext.GetConfigValueOrDefault("/Service/OrgSearch/Settings/ProjectRepoProjectDescriptionBoost", 5);
      int configValueOrDefault3 = requestContext.GetConfigValueOrDefault("/Service/OrgSearch/Settings/ProjectRepoProjectTagsBoost", 14);
      int configValueOrDefault4 = requestContext.GetConfigValueOrDefault("/Service/OrgSearch/Settings/ProjectRepoCollectioNameBoost", 1);
      fieldsBoostValueMap.Add("name", (double) configValueOrDefault1);
      fieldsBoostValueMap.Add("name.casechangeanalyzed", (double) configValueOrDefault1);
      fieldsBoostValueMap.Add("description", (double) configValueOrDefault2);
      fieldsBoostValueMap.Add("tags", (double) configValueOrDefault3);
      fieldsBoostValueMap.Add("tags.casechangeanalyzed", (double) configValueOrDefault3);
      fieldsBoostValueMap.Add("collectionNameAnalyzed", (double) configValueOrDefault4);
      fieldsBoostValueMap.Add("collectionNameAnalyzed.casechangeanalyzed", (double) configValueOrDefault4);
      return (IDictionary<string, double>) fieldsBoostValueMap;
    }

    public static IDictionary<string, double> GetRepositoryQueryFieldsBoostValueMap(
      IVssRequestContext requestContext)
    {
      IDictionary<string, double> fieldsBoostValueMap = ProjectRepoQueryFields.GetRepositoryBaseQueryFieldsBoostValueMap(requestContext);
      fieldsBoostValueMap.Add("projectName", 1.0);
      fieldsBoostValueMap.Add("projectName.casechangeanalyzed", 1.0);
      return fieldsBoostValueMap;
    }

    public static IDictionary<string, double> GetChildContractRepositoryQueryFieldsBoostValueMap(
      IVssRequestContext requestContext)
    {
      return ProjectRepoQueryFields.GetRepositoryBaseQueryFieldsBoostValueMap(requestContext);
    }

    public static IList<string> ConvertToRawString(IDictionary<string, double> fieldsBoostValueMap)
    {
      IList<string> rawString = (IList<string>) new List<string>();
      foreach (KeyValuePair<string, double> fieldsBoostValue in (IEnumerable<KeyValuePair<string, double>>) fieldsBoostValueMap)
        rawString.Add(FormattableString.Invariant(FormattableStringFactory.Create("{0}^{1}", (object) fieldsBoostValue.Key, (object) fieldsBoostValue.Value)));
      return rawString;
    }

    public static Fields ConvertToFields(IDictionary<string, double> fieldNameBoostValueMap)
    {
      IList<Field> source = (IList<Field>) new List<Field>();
      foreach (KeyValuePair<string, double> fieldNameBoostValue in (IEnumerable<KeyValuePair<string, double>>) fieldNameBoostValueMap)
        source.Add(new Field(fieldNameBoostValue.Key, new double?(fieldNameBoostValue.Value)));
      return (Fields) source.ToArray<Field>();
    }

    private static IDictionary<string, double> GetRepositoryBaseQueryFieldsBoostValueMap(
      IVssRequestContext requestContext)
    {
      Dictionary<string, double> fieldsBoostValueMap = new Dictionary<string, double>();
      int configValueOrDefault1 = requestContext.GetConfigValueOrDefault("/Service/OrgSearch/Settings/ProjectRepoRepositoryNameBoost", 20);
      int configValueOrDefault2 = requestContext.GetConfigValueOrDefault("/Service/OrgSearch/Settings/ProjectRepoRepositoryReadmeLinksBoost", 1);
      int configValueOrDefault3 = requestContext.GetConfigValueOrDefault("/Service/OrgSearch/Settings/ProjectRepoCollectioNameBoost", 1);
      fieldsBoostValueMap.Add("name", (double) configValueOrDefault1);
      fieldsBoostValueMap.Add("name.casechangeanalyzed", (double) configValueOrDefault1);
      fieldsBoostValueMap.Add("readme", 1.0);
      fieldsBoostValueMap.Add("readmeLinks", (double) configValueOrDefault2);
      fieldsBoostValueMap.Add("collectionNameAnalyzed", (double) configValueOrDefault3);
      fieldsBoostValueMap.Add("collectionNameAnalyzed.casechangeanalyzed", (double) configValueOrDefault3);
      return (IDictionary<string, double>) fieldsBoostValueMap;
    }
  }
}
