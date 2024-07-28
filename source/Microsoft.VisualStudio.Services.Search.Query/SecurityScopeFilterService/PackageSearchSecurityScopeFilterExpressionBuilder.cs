// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.SecurityScopeFilterService.PackageSearchSecurityScopeFilterExpressionBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Query.SecurityScopeFilterService
{
  public class PackageSearchSecurityScopeFilterExpressionBuilder : 
    ISearchSecurityScopeFilterExpressionBuilder
  {
    public IExpression GetScopeFilterExpression(
      IVssRequestContext requestContext,
      bool enableSecurityChecks,
      out bool noResultAccessible,
      ProjectInfo projectInfo = null)
    {
      noResultAccessible = true;
      IList<IExpression> source = (IList<IExpression>) new List<IExpression>();
      if (enableSecurityChecks)
      {
        bool allPackageContainersAccessible = false;
        IEnumerable<PackageContainer> packageContainers = this.GetUserAccessiblePackageContainers(requestContext, out allPackageContainersAccessible);
        noResultAccessible = !allPackageContainersAccessible;
        if (!allPackageContainersAccessible)
        {
          noResultAccessible = !packageContainers.Any<PackageContainer>();
          IEnumerable<string> strings1 = packageContainers.Where<PackageContainer>((Func<PackageContainer, bool>) (c => c.Type == PackageContainerType.Feed)).ToList<PackageContainer>().Select<PackageContainer, string>((Func<PackageContainer, string>) (f => f.ContainerId.ToString()));
          IEnumerable<string> strings2 = packageContainers.Where<PackageContainer>((Func<PackageContainer, bool>) (c => c.Type == PackageContainerType.View)).ToList<PackageContainer>().Select<PackageContainer, string>((Func<PackageContainer, string>) (f => f.ContainerId.ToString()));
          if (strings1.Any<string>() && strings2.Any<string>())
            source.Add((IExpression) new OrExpression(new IExpression[2]
            {
              this.GetFeedsScopeExpression(strings1),
              this.GetViewsScopeExpression(strings2)
            }));
          else if (strings1.Any<string>())
            source.Add(this.GetFeedsScopeExpression(strings1));
          else if (strings2.Any<string>())
            source.Add(this.GetViewsScopeExpression(strings2));
        }
      }
      source.Add(this.GetCollectionScopeExpression(requestContext));
      source.Add(this.GetIsListedScopeFilterExpression());
      return source.Aggregate<IExpression>((Func<IExpression, IExpression, IExpression>) ((current, filter) => (IExpression) new AndExpression(new IExpression[2]
      {
        current,
        filter
      })));
    }

    private IExpression GetFeedsScopeExpression(IEnumerable<string> accessiblefeeds)
    {
      if (accessiblefeeds == null || !accessiblefeeds.Any<string>())
        accessiblefeeds = (IEnumerable<string>) new List<string>();
      return (IExpression) new TermsExpression("feedId", Operator.In, accessiblefeeds);
    }

    private IExpression GetViewsScopeExpression(IEnumerable<string> accessibleViews)
    {
      if (accessibleViews == null || !accessibleViews.Any<string>())
        accessibleViews = (IEnumerable<string>) new List<string>();
      return (IExpression) new TermsExpression("views.viewId", Operator.In, accessibleViews);
    }

    private IExpression GetIsListedScopeFilterExpression() => (IExpression) new TermsExpression("isListed", Operator.In, (IEnumerable<string>) new string[1]
    {
      bool.TrueString.ToLowerInvariant()
    });

    private IExpression GetCollectionScopeExpression(IVssRequestContext requestContext) => (IExpression) new TermExpression("collectionId", Operator.Equals, requestContext.GetCollectionID().ToString().ToLowerInvariant());

    private IEnumerable<PackageContainer> GetUserAccessiblePackageContainers(
      IVssRequestContext requestContext,
      out bool allPackageContainersAccessible)
    {
      return requestContext.GetService<IPackageSecurityChecksService>().GetUserAccessiblePackageContainers(requestContext, out allPackageContainersAccessible);
    }
  }
}
