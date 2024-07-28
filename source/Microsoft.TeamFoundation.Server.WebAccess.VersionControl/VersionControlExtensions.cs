// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.VersionControlExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Server.WebAccess.Controls;
using Microsoft.TeamFoundation.Server.WebAccess.Html;
using Microsoft.TeamFoundation.SourceControl.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl
{
  public static class VersionControlExtensions
  {
    public static MvcHtmlString VCViewOptions(
      this HtmlHelper htmlHelper,
      VersionControlViewModel model)
    {
      return htmlHelper.RestApiJsonIsland((object) model, (object) new
      {
        @class = "options"
      });
    }

    public static MvcHtmlString VCAdminViewOptions(
      this HtmlHelper htmlHelper,
      VersionControlAdminViewModel model)
    {
      return htmlHelper.RestApiJsonIsland((object) model, (object) new
      {
        @class = "options"
      });
    }

    public static MvcHtmlString ChangeListInfo(this HtmlHelper htmlHelper, TfsWebContext webContext)
    {
      HtmlHelper htmlHelper1 = htmlHelper;
      // ISSUE: reference to a compiler-generated field
      if (VersionControlExtensions.\u003C\u003Eo__2.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        VersionControlExtensions.\u003C\u003Eo__2.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "ChangeList", typeof (VersionControlExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      ChangeList data = VersionControlExtensions.\u003C\u003Eo__2.\u003C\u003Ep__0.Target((CallSite) VersionControlExtensions.\u003C\u003Eo__2.\u003C\u003Ep__0, htmlHelper.ViewBag) as ChangeList;
      var htmlAttributes = new
      {
        @class = "vc-change-list-data"
      };
      return htmlHelper1.DataContractJsonIsland<ChangeList>(data, (object) htmlAttributes);
    }

    public static MvcHtmlString ChangeListPivotViews(
      this HtmlHelper htmlHelper,
      UrlHelper urlHelper)
    {
      List<PivotView> views = new List<PivotView>();
      PivotView pivotView = new PivotView(VCResources.Summary)
      {
        Id = "summary",
        Link = urlHelper.FragmentAction("summary"),
        Disabled = true
      };
      views.Add(pivotView);
      views.Add(new PivotView(VCResources.Contents)
      {
        Id = "contents",
        Link = urlHelper.FragmentAction("contents"),
        Disabled = true
      });
      views.Add(new PivotView(VCResources.History)
      {
        Id = "history",
        Link = urlHelper.FragmentAction("history"),
        Disabled = true
      });
      views.Add(new PivotView(VCResources.Compare)
      {
        Id = "compare",
        Link = urlHelper.FragmentAction("compare"),
        Disabled = true
      });
      views.Add(new PivotView(VCResources.Annotate)
      {
        Id = "annotate",
        Link = urlHelper.FragmentAction("annotate"),
        Disabled = true
      });
      // ISSUE: reference to a compiler-generated field
      if (VersionControlExtensions.\u003C\u003Eo__3.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        VersionControlExtensions.\u003C\u003Eo__3.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "ChangeList", typeof (VersionControlExtensions), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (VersionControlExtensions.\u003C\u003Eo__3.\u003C\u003Ep__0.Target((CallSite) VersionControlExtensions.\u003C\u003Eo__3.\u003C\u003Ep__0, htmlHelper.ViewBag) is GitCommit gitCommit && gitCommit.Parents != null)
      {
        List<PivotView> collection = new List<PivotView>();
        int num = 1;
        foreach (GitObjectReference parent in gitCommit.Parents)
        {
          string action = "diffparent" + num.ToString();
          string text = string.Format(VCServerResources.DiffParentCommitFormat, (object) num);
          string str = string.Format(VCResources.DiffParentCommitTitleFormat, (object) num, (object) parent.ObjectId.Short);
          collection.Add(new PivotView(text)
          {
            Id = action,
            Title = str,
            Link = urlHelper.FragmentAction(action),
            Disabled = true
          });
          ++num;
        }
        if (num > 1)
        {
          pivotView.Text = VCResources.MergeCommit;
          pivotView.Title = string.Format(VCServerResources.MergeCommitTitleFormat, (object) gitCommit.CommitId.Short);
          views.AddRange((IEnumerable<PivotView>) collection);
        }
      }
      return htmlHelper.PivotViews((IEnumerable<PivotView>) views, (object) new
      {
        @class = "vc-explorer-tabs"
      });
    }

    public static MvcHtmlString PullRequestsDateFilter(this HtmlHelper htmlHelper)
    {
      PivotFilter controlType = new PivotFilter(VCServerResources.PullRequest_Pivot_DateFilter_Title, (IEnumerable<PivotFilterItem>) new PivotFilterItem[2]
      {
        new PivotFilterItem(VCServerResources.PullRequest_Pivot_Sort_MostRecentlyCreated, (object) "mostRecentlyCreated")
        {
          Selected = true
        },
        new PivotFilterItem(VCServerResources.PullRequest_Pivot_Sort_LastRecentlyCreated, (object) "lastRecentlyCreated")
        {
          Selected = false
        }
      })
      {
        Behavior = PivotFilterBehavior.Dropdown
      };
      return htmlHelper.Control<PivotFilter>(controlType, (object) new
      {
        @class = "pullRequests-pane-filter"
      });
    }

    public static MvcHtmlString RenderSearchBoxInVCPageTitleArea(
      this HtmlHelper htmlHelper,
      string cssClass)
    {
      string cssClass1 = htmlHelper.ViewData["SearchBoxCssClass"] as string;
      string empty = string.Empty;
      return MvcHtmlString.Create(new TagBuilder("div").AddClass("search-box bowtie").AddClass(cssClass).AddClass(cssClass1).ToString());
    }

    public static bool IsGitProject(this HtmlHelper htmlHelper) => string.Equals(htmlHelper.ViewContext.TfsWebContext().NavigationContext.CurrentController, "Git", StringComparison.OrdinalIgnoreCase);

    public static bool IsReviewMode(this HtmlHelper htmlHelper) => (bool) htmlHelper.ViewData["ReviewMode"];
  }
}
