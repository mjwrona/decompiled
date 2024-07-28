// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression.TermsExpressionQueryBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Board;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Setting;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.Expression
{
  internal class TermsExpressionQueryBuilder : IPlatformQueryBuilder
  {
    [StaticSafe]
    private static readonly IDictionary<DocumentContractType, AbstractSearchDocumentContract> s_documentContractMapping = (IDictionary<DocumentContractType, AbstractSearchDocumentContract>) new FriendlyDictionary<DocumentContractType, AbstractSearchDocumentContract>()
    {
      [DocumentContractType.DedupeFileContractV3] = (AbstractSearchDocumentContract) new DedupeFileContractV3(),
      [DocumentContractType.DedupeFileContractV4] = (AbstractSearchDocumentContract) new DedupeFileContractV4(),
      [DocumentContractType.DedupeFileContractV5] = (AbstractSearchDocumentContract) new DedupeFileContractV5(),
      [DocumentContractType.SourceNoDedupeFileContractV3] = (AbstractSearchDocumentContract) new SourceNoDedupeFileContractV3(),
      [DocumentContractType.SourceNoDedupeFileContractV4] = (AbstractSearchDocumentContract) new SourceNoDedupeFileContractV4(),
      [DocumentContractType.SourceNoDedupeFileContractV5] = (AbstractSearchDocumentContract) new SourceNoDedupeFileContractV5(),
      [DocumentContractType.ProjectContract] = (AbstractSearchDocumentContract) new ProjectContract(),
      [DocumentContractType.RepositoryContract] = (AbstractSearchDocumentContract) new RepositoryContract(),
      [DocumentContractType.WorkItemContract] = (AbstractSearchDocumentContract) new WorkItemContract(),
      [DocumentContractType.WikiContract] = (AbstractSearchDocumentContract) new WikiContract(),
      [DocumentContractType.PackageVersionContract] = (AbstractSearchDocumentContract) new PackageVersionContract(),
      [DocumentContractType.BoardContract] = (AbstractSearchDocumentContract) new BoardVersionContract(),
      [DocumentContractType.SettingContract] = (AbstractSearchDocumentContract) new SettingContract()
    };

    public string Build(
      IVssRequestContext requestContext,
      IExpression expression,
      IEntityType type,
      DocumentContractType contractType,
      bool enableRanking,
      bool allowSpellingErrors,
      string requestId,
      ResultsCountPlatformRequest request)
    {
      if (!(expression is TermsExpression termsExpression))
        throw new ArgumentNullException(nameof (expression));
      AbstractSearchDocumentContract documentContract;
      if (!TermsExpressionQueryBuilder.s_documentContractMapping.TryGetValue(contractType, out documentContract))
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Unhandled document contract [{0}] encountered.", (object) contractType.ToString())));
      string str = string.Empty;
      if (termsExpression.Terms != null)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\r\n                        \"{0}\": {{\r\n                            \"{1}\": [{2}]\r\n                        }}\r\n                    }}", (object) "terms", (object) documentContract.GetSearchFieldForType(termsExpression.Type), (object) string.Join(", ", termsExpression.Terms.Select<string, string>(TermsExpressionQueryBuilder.\u003C\u003EO.\u003C0\u003E__SerializeObject ?? (TermsExpressionQueryBuilder.\u003C\u003EO.\u003C0\u003E__SerializeObject = new Func<string, string>(JsonConvert.SerializeObject)))));
      }
      return str;
    }
  }
}
