// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.SummaryMailHtmlGenerator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public sealed class SummaryMailHtmlGenerator
  {
    private const string HtmlDocumentLabel = "html";
    private const string HtmlHeaderLabel = "head";
    private const string HtmlBodyLabel = "body";
    private const string HtmlLineBreakLabel = "br";
    private const string HtmlStyleLabel = "style";
    private const string HtmlAnchorLabel = "a";
    private const string HtmlHrefLabel = "href";
    private const string HtmlBoldLabel = "b";
    private const string HtmlTextcolorLabel = "text";
    private const string HtmlSpanLabel = "span";
    private const string HtmlDivLabel = "div";
    private const string HtmlPreLabel = "pre";
    private const string HtmlParagraphTextLabel = "p";
    private const string HtmlTableLabel = "table";
    private const string HtmlTHeadLabel = "thead";
    private const string HtmlTBodyLabel = "tbody";
    private const string HtmlTableRowLabel = "tr";
    private const string HtmlTableData = "td";
    private const string HtmlTableHeader = "th";
    private const string DefaultFontStyle = "font-family: Segoe UI,Arial,sans-serif; font-size: 12px; white-space:nowrap;";
    private const string DefaultAnchorStyle = "color: #007acc; text-decoration: none;";
    private const string GreyColour = "#555555;";
    private const string BlueColour = "#007acc;";
    private const string RedColour = "#e51400;";
    private const string GreenColour = "#339933;";
    private const string BlackColour = "#000000;";
    private const string OrangeColour = "#FF8000;";
    private const string EmptyStartTime = "-";

    private SummaryMailHtmlGenerator()
    {
    }

    public static XElement GetSummaryMailHtml(IList<SummaryMailSection> sections, string note)
    {
      if (sections == null)
        throw new ArgumentNullException(nameof (sections));
      XElement summaryMailHtml = new XElement((XName) "div");
      if (!string.IsNullOrEmpty(note))
      {
        XElement element = new XElement((XName) "div", new object[2]
        {
          (object) note,
          (object) new XAttribute((XName) "style", (object) "font-family: Segoe UI,Arial,sans-serif; font-size: 12px; white-space:nowrap;")
        });
        summaryMailHtml.Add((object) SummaryMailHtmlGenerator.GetSummaryMailSectionWithOutHeader(SummaryMailHelper.SafeConvertXElementToString((XNode) element)));
      }
      SummaryMailSection summaryMailSection = sections.FirstOrDefault<SummaryMailSection>((Func<SummaryMailSection, bool>) (sec => sec.SectionType == MailSectionType.ReleaseInfo));
      if (summaryMailSection != null)
      {
        summaryMailHtml.Add((object) SummaryMailHtmlGenerator.GetSummaryMailSectionWithOutHeader(summaryMailSection.HtmlContent));
        sections.Remove(summaryMailSection);
      }
      foreach (SummaryMailSection section in (IEnumerable<SummaryMailSection>) sections)
        summaryMailHtml.Add((object) SummaryMailHtmlGenerator.GetSummaryMailSectionHtml(section));
      return summaryMailHtml;
    }

    public static XElement GetReleaseInfo(
      IVssRequestContext requestContext,
      string projectName,
      Release release)
    {
      XElement releaseInfo = new XElement((XName) "div");
      if (release == null)
        return releaseInfo;
      string releaseDefinitionUrl = SummaryMailHtmlGenerator.GetReleaseDefinitionUrl(requestContext, projectName, release.ReleaseDefinitionReference.Id);
      string releaseUrl = SummaryMailHtmlGenerator.GetReleaseUrl(requestContext, projectName, release.Id);
      releaseInfo.Add((object) new XElement((XName) "a", new object[5]
      {
        (object) release.ReleaseDefinitionReference.Name,
        (object) new XAttribute((XName) "href", (object) releaseDefinitionUrl),
        (object) new XAttribute((XName) "target", (object) "_blank"),
        (object) new XAttribute((XName) "title", (object) releaseDefinitionUrl),
        (object) new XAttribute((XName) "style", (object) "color: #007acc; text-decoration: none;")
      }));
      releaseInfo.Add((object) new XElement((XName) "span", (object) "/"));
      releaseInfo.Add((object) new XElement((XName) "a", new object[4]
      {
        (object) release.Name,
        (object) new XAttribute((XName) "href", (object) releaseUrl),
        (object) new XAttribute((XName) "target", (object) "_blank"),
        (object) new XAttribute((XName) "style", (object) "color: #007acc; text-decoration: none;")
      }));
      return releaseInfo;
    }

    public static XElement GetDetailsSectionHtml(
      IVssRequestContext requestContext,
      string projectName,
      Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      XElement detailsSectionHtml = new XElement((XName) "div", (object) new XAttribute((XName) "style", (object) "font-family: Segoe UI,Arial,sans-serif; font-size: 12px; white-space:nowrap;"));
      if (!string.IsNullOrEmpty(release.Description))
      {
        XElement content = new XElement((XName) "div", new object[2]
        {
          (object) release.Description,
          (object) new XAttribute((XName) "style", (object) "color: #8C8C8C; font-style: italic")
        });
        detailsSectionHtml.Add((object) content);
      }
      XElement content1 = new XElement((XName) "div", (object) SummaryMailHtmlGenerator.GetCreatedByText(release));
      detailsSectionHtml.Add((object) content1);
      XElement content2 = new XElement((XName) "table", (object) new XAttribute((XName) "style", (object) "font-family: Segoe UI,Arial,sans-serif; font-size: 12px; white-space:nowrap;"));
      XElement content3 = new XElement((XName) "td", (object) Resources.MailArtifactsSectionTitle);
      content2.Add((object) new XElement((XName) "tr", (object) content3));
      content2.Add((object) new XElement((XName) "tr", (object) new XElement((XName) "td", (object) SummaryMailHtmlGenerator.GetArtifactsSection(requestContext, projectName, release))));
      detailsSectionHtml.Add((object) content2);
      return detailsSectionHtml;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)", Justification = "Is culture invariant")]
    public static XElement GetEnvironmentsSectionHtml(Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      XElement environmentsSectionHtml = new XElement((XName) "div");
      XElement content1 = new XElement((XName) "table", (object) new XAttribute((XName) "style", (object) "font-family: Segoe UI,Arial,sans-serif; font-size: 12px; white-space:nowrap;"));
      environmentsSectionHtml.Add((object) content1);
      XElement content2 = new XElement((XName) "tr", (object) new XAttribute((XName) "style", (object) "color: #666666;"));
      content2.Add((object) new XElement((XName) "td", new object[3]
      {
        (object) Resources.MailEnvironmentColumnHeader,
        (object) new XAttribute((XName) "align", (object) "left"),
        (object) new XAttribute((XName) "style", (object) "padding: 2px; width: 30%; min-width: 150px;")
      }));
      content2.Add((object) new XElement((XName) "td", new object[3]
      {
        (object) Resources.MailEnvironmentStatusHeader,
        (object) new XAttribute((XName) "align", (object) "left"),
        (object) new XAttribute((XName) "style", (object) "padding: 2px; padding-left: 25px; min-width: 150px;")
      }));
      content2.Add((object) new XElement((XName) "td", new object[3]
      {
        (object) Resources.MailEnvironmentStartTimeColumnHeader,
        (object) new XAttribute((XName) "align", (object) "left"),
        (object) new XAttribute((XName) "style", (object) "padding: 2px; padding-left: 25px; min-width: 150px;")
      }));
      content2.Add((object) new XElement((XName) "td", new object[3]
      {
        (object) Resources.MailEnvironmentDurationColumnHeader,
        (object) new XAttribute((XName) "align", (object) "left"),
        (object) new XAttribute((XName) "style", (object) "padding: 2px; padding-left: 25px; min-width: 150px;")
      }));
      content1.Add((object) content2);
      foreach (ReleaseEnvironment environment in (IEnumerable<ReleaseEnvironment>) release.Environments)
      {
        XElement environmentTableRow = SummaryMailHtmlGenerator.GetEnvironmentTableRow(environment);
        content1.Add((object) environmentTableRow);
      }
      return environmentsSectionHtml;
    }

    public static XElement GetIssuesSectionHtml(
      IVssRequestContext requestContext,
      string projectName,
      Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      XElement issuesSectionHtml = new XElement((XName) "div");
      List<string> errors = new List<string>();
      List<string> warnings = new List<string>();
      foreach (ReleaseEnvironment environment in (IEnumerable<ReleaseEnvironment>) release.Environments)
        SummaryMailHtmlGenerator.PopulateEnvironmentIssues(environment, ref errors, ref warnings);
      string releaseLogsUrl = SummaryMailHtmlGenerator.GetReleaseLogsUrl(requestContext, projectName, release.Id);
      if (errors.Count > 0)
      {
        XElement tableSectionHtml = SummaryMailHtmlGenerator.GetIssuesTableSectionHtml(errors, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MailErrorsTableHeader, (object) errors.Count), releaseLogsUrl, "#e51400;");
        issuesSectionHtml.Add((object) tableSectionHtml);
      }
      if (warnings.Count > 0)
      {
        XElement tableSectionHtml = SummaryMailHtmlGenerator.GetIssuesTableSectionHtml(warnings, string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MailWarningsTableHeader, (object) warnings.Count), releaseLogsUrl, "#000000;");
        issuesSectionHtml.Add((object) tableSectionHtml);
      }
      if (errors.Count == 0 && warnings.Count == 0)
      {
        XElement content = new XElement((XName) "div", (object) Resources.MailNoIssuesMessage);
        issuesSectionHtml.Add((object) content);
      }
      return issuesSectionHtml;
    }

    public static XElement GetWorkItemsSection(
      IVssRequestContext requestContext,
      string projectName,
      IList<WorkItem> workItems)
    {
      XElement workItemsSection = new XElement((XName) "div");
      if (workItems == null || workItems.Count == 0)
      {
        workItemsSection.Add((object) new XElement((XName) "div", new object[2]
        {
          (object) Resources.MailNoAssociatedWorkItems,
          (object) new XAttribute((XName) "style", (object) "font-family: Segoe UI,Arial,sans-serif; font-size: 12px; white-space:nowrap;")
        }));
      }
      else
      {
        XElement content = new XElement((XName) "table", (object) new XAttribute((XName) "style", (object) "font-family: Segoe UI,Arial,sans-serif; font-size: 12px; white-space:nowrap;"));
        foreach (WorkItem workItem in (IEnumerable<WorkItem>) workItems)
          content.Add((object) SummaryMailHtmlGenerator.GetWorkItemRowHtml(requestContext, projectName, workItem));
        workItemsSection.Add((object) content);
      }
      return workItemsSection;
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not a property")]
    public static XElement GetUnableToFetchWorkItems()
    {
      XElement toFetchWorkItems = new XElement((XName) "div");
      XElement content = new XElement((XName) "span", new object[2]
      {
        (object) new XAttribute((XName) "style", (object) "font-family: Segoe UI,Arial,sans-serif; font-size: 12px; white-space:nowrap;"),
        (object) Resources.MailUnableToFetchWorkItems
      });
      toFetchWorkItems.Add((object) content);
      return toFetchWorkItems;
    }

    private static XElement GetArtifactsSection(
      IVssRequestContext requestContext,
      string projectName,
      Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      XElement artifactsSection = new XElement((XName) "div");
      if (!release.Artifacts.Any<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>())
      {
        artifactsSection.Add((object) new XElement((XName) "div", (object) Resources.NoArtifactsMessage));
        return artifactsSection;
      }
      XElement xelement = new XElement((XName) "table", (object) new XAttribute((XName) "style", (object) "font-family: Segoe UI,Arial,sans-serif; font-size: 12px; white-space:nowrap;"));
      List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact> list = release.Artifacts.ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>();
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact artifact1 = list.FirstOrDefault<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact, bool>) (a => a.IsPrimary));
      if (artifact1 != null)
      {
        SummaryMailHtmlGenerator.AddArtifactDetailsRow(requestContext, projectName, xelement, artifact1);
        list.Remove(artifact1);
      }
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact artifact2 in list)
        SummaryMailHtmlGenerator.AddArtifactDetailsRow(requestContext, projectName, xelement, artifact2);
      artifactsSection.Add((object) xelement);
      return artifactsSection;
    }

    private static void AddArtifactDetailsRow(
      IVssRequestContext requestContext,
      string projectName,
      XElement table,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact artifact)
    {
      XElement artifactRowHtml = SummaryMailHtmlGenerator.GetArtifactRowHtml(requestContext, projectName, artifact);
      if (artifactRowHtml == null)
        return;
      table.Add((object) artifactRowHtml);
    }

    private static XElement GetWorkItemRowHtml(
      IVssRequestContext requestContext,
      string projectName,
      WorkItem workItem)
    {
      XElement workItemRowHtml = new XElement((XName) "tr");
      XElement xelement = new XElement((XName) "div", new object[2]
      {
        (object) "!",
        (object) new XAttribute((XName) "style", (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "width: 4.0pt; height: 15px; color:{0} background-color:{0}", (object) SummaryMailHtmlGenerator.GetWorkItemColor(workItem.Fields["System.WorkItemType"].ToString())))
      });
      workItemRowHtml.Add((object) new XElement((XName) "td", new object[2]
      {
        (object) xelement,
        (object) new XAttribute((XName) "style", (object) "padding-bottom: 5px; width: 4.0pt;")
      }));
      XElement content = new XElement((XName) "a", new object[4]
      {
        (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", workItem.Fields["System.WorkItemType"], (object) workItem.Id),
        (object) new XAttribute((XName) "href", (object) SummaryMailHtmlGenerator.GetWorkItemUrl(requestContext, projectName, workItem.Id)),
        (object) new XAttribute((XName) "target", (object) "_blank"),
        (object) new XAttribute((XName) "style", (object) "color: #007acc; text-decoration: none;")
      });
      workItemRowHtml.Add((object) new XElement((XName) "td", (object) content));
      workItemRowHtml.Add((object) new XElement((XName) "td", new object[2]
      {
        workItem.Fields["System.Title"],
        (object) new XAttribute((XName) "style", (object) "padding: 2px; padding-left: 10px;")
      }));
      workItemRowHtml.Add((object) new XElement((XName) "td", new object[2]
      {
        workItem.Fields["System.State"],
        (object) new XAttribute((XName) "style", (object) "padding: 2px; padding-left: 10px;")
      }));
      return workItemRowHtml;
    }

    private static string GetWorkItemColor(string workItemType)
    {
      if (workItemType != null)
      {
        switch (workItemType.Length)
        {
          case 3:
            if (workItemType == "Bug")
              return "#cc293d;";
            break;
          case 4:
            switch (workItemType[0])
            {
              case 'E':
                if (workItemType == "Epic")
                  return "#ff7b00;";
                break;
              case 'T':
                if (workItemType == "Task")
                  return "#f2cb1d;";
                break;
            }
            break;
          case 7:
            if (workItemType == "Feature")
              return "#773b93;";
            break;
          case 10:
            if (workItemType == "User Story")
              return "#009ccc;";
            break;
          case 11:
            if (workItemType == "Requirement")
              return "#009ccc;";
            break;
          case 20:
            if (workItemType == "Product Backlog Item")
              return "#009ccc;";
            break;
        }
      }
      return "#ff9d00;";
    }

    private static XElement GetArtifactRowHtml(
      IVssRequestContext requestContext,
      string projectName,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact artifact)
    {
      XElement artifactRowHtml = new XElement((XName) "tr");
      XElement content1 = new XElement((XName) "td");
      XElement content2 = new XElement((XName) "td", (object) new XAttribute((XName) "style", (object) "padding-left: 10px;"));
      artifactRowHtml.Add((object) content1);
      artifactRowHtml.Add((object) content2);
      if (artifact.Type == "Build")
      {
        int result1;
        if (!artifact.DefinitionReference.ContainsKey("definition") || artifact.DefinitionReference["definition"].Id.IsNullOrEmpty<char>() || !int.TryParse(artifact.DefinitionReference["definition"].Id, out result1))
          return (XElement) null;
        string buildDefinitionUrl = SummaryMailHtmlGenerator.GetBuildDefinitionUrl(requestContext, projectName, result1);
        content1.Add((object) new XElement((XName) "a", new object[4]
        {
          (object) artifact.DefinitionReference["definition"].Name,
          (object) new XAttribute((XName) "style", (object) "color: #007acc; text-decoration: none;"),
          (object) new XAttribute((XName) "href", (object) buildDefinitionUrl),
          (object) new XAttribute((XName) "target", (object) "_blank")
        }));
        int result2;
        if (artifact.DefinitionReference.ContainsKey("version") && !artifact.DefinitionReference["version"].Id.IsNullOrEmpty<char>() && int.TryParse(artifact.DefinitionReference["version"].Id, out result2))
        {
          content1.Add((object) new XElement((XName) "span", (object) "/"));
          string buildUrl = SummaryMailHtmlGenerator.GetBuildUrl(requestContext, projectName, result2);
          content1.Add((object) new XElement((XName) "a", new object[4]
          {
            (object) artifact.DefinitionReference["version"].Name,
            (object) new XAttribute((XName) "style", (object) "color: #007acc; text-decoration: none;"),
            (object) new XAttribute((XName) "href", (object) buildUrl),
            (object) new XAttribute((XName) "target", (object) "_blank")
          }));
        }
        content1.Add((object) new XElement((XName) "span", (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, "({0})", (object) artifact.Type)));
        if (artifact.DefinitionReference.ContainsKey("branch") && !artifact.DefinitionReference["branch"].Name.IsNullOrEmpty<char>())
        {
          content2.Add((object) new XElement((XName) "span", new object[2]
          {
            (object) Resources.MailArtifactBranchLabel,
            (object) new XAttribute((XName) "style", (object) "color: #8C8C8C;")
          }));
          content2.Add((object) new XElement((XName) "span", (object) artifact.DefinitionReference["branch"].Name));
        }
      }
      else
      {
        if (!artifact.DefinitionReference.ContainsKey("definition") || artifact.DefinitionReference["definition"].Name.IsNullOrEmpty<char>())
          return (XElement) null;
        string content3 = !artifact.DefinitionReference.ContainsKey("version") || artifact.DefinitionReference["version"].Name.IsNullOrEmpty<char>() ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} ({1})", (object) artifact.DefinitionReference["definition"].Name, (object) artifact.Type) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} / {1} ({2})", (object) artifact.DefinitionReference["definition"].Name, (object) artifact.DefinitionReference["version"].Name, (object) artifact.Type);
        content1.Add((object) new XElement((XName) "span", (object) content3));
      }
      return artifactRowHtml;
    }

    private static XElement GetSummaryMailSectionHtml(SummaryMailSection section)
    {
      XElement summaryMailSectionHtml = new XElement((XName) "table", new object[2]
      {
        (object) new XAttribute((XName) "align", (object) "left"),
        (object) new XAttribute((XName) "style", (object) ("width: 100%;" + "font-family: Segoe UI,Arial,sans-serif; font-size: 12px; white-space:nowrap;"))
      });
      XElement content1 = new XElement((XName) "thead");
      XElement content2 = new XElement((XName) "tr", new object[2]
      {
        (object) new XAttribute((XName) "align", (object) "left"),
        (object) new XAttribute((XName) "style", (object) "width: 100%; height:25px; background-color: #DEE8F2;")
      });
      XElement content3 = new XElement((XName) "th", new object[3]
      {
        (object) new XElement((XName) "span", (object) section.Title),
        (object) new XAttribute((XName) "align", (object) "left"),
        (object) new XAttribute((XName) "style", (object) "padding: 2px 2px 2px 10px;")
      });
      content2.Add((object) content3);
      content1.Add((object) content2);
      summaryMailSectionHtml.Add((object) content1);
      XElement content4 = new XElement((XName) "tbody");
      XElement content5 = new XElement((XName) "tr");
      XElement content6 = new XElement((XName) "td", new object[2]
      {
        (object) new XElement((XName) "div", (object) SummaryMailHelper.SafeConvertStringToXElement(section.HtmlContent)),
        (object) new XAttribute((XName) "style", (object) "padding: 5px 2px 5px 2px;")
      });
      content5.Add((object) content6);
      content4.Add((object) content5);
      summaryMailSectionHtml.Add((object) content4);
      return summaryMailSectionHtml;
    }

    private static XElement GetSummaryMailSectionWithOutHeader(string htmlContent)
    {
      XElement sectionWithOutHeader = new XElement((XName) "table", new object[2]
      {
        (object) new XAttribute((XName) "align", (object) "left"),
        (object) new XAttribute((XName) "style", (object) ("width: 100%;" + "font-family: Segoe UI,Arial,sans-serif; font-size: 12px; white-space:nowrap;"))
      });
      XElement content1 = new XElement((XName) "tbody");
      XElement content2 = new XElement((XName) "tr");
      XElement content3 = new XElement((XName) "td", new object[2]
      {
        (object) new XElement((XName) "div", (object) SummaryMailHelper.SafeConvertStringToXElement(htmlContent)),
        (object) new XAttribute((XName) "style", (object) "padding: 5px 2px 5px 2px;")
      });
      content2.Add((object) content3);
      content1.Add((object) content2);
      sectionWithOutHeader.Add((object) content1);
      return sectionWithOutHeader;
    }

    private static XElement GetIssuesTableSectionHtml(
      List<string> issues,
      string title,
      string logsUrl,
      string indexColorCode)
    {
      XElement tableSectionHtml = new XElement((XName) "div");
      XElement content1 = new XElement((XName) "table", new object[2]
      {
        (object) new XAttribute((XName) "align", (object) "left"),
        (object) new XAttribute((XName) "style", (object) ("width: 100%;" + "font-family: Segoe UI,Arial,sans-serif; font-size: 12px; white-space:nowrap;"))
      });
      XElement content2 = new XElement((XName) "tr", (object) new XElement((XName) "td", (object) new XElement((XName) "div", (object) title)));
      content1.Add((object) content2);
      foreach (string issue in issues)
      {
        XElement content3 = new XElement((XName) "tr", (object) new XAttribute((XName) "align", (object) "left"));
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "text-decoration: none; color:{0} {1}", (object) indexColorCode, (object) "font-family: Segoe UI,Arial,sans-serif; font-size: 12px; white-space:nowrap;");
        XElement content4 = new XElement((XName) "a", new object[4]
        {
          (object) issue,
          (object) new XAttribute((XName) "href", (object) logsUrl),
          (object) new XAttribute((XName) "target", (object) "_blank"),
          (object) new XAttribute((XName) "style", (object) str)
        });
        content3.Add((object) new XElement((XName) "td", (object) content4));
        content1.Add((object) content3);
      }
      tableSectionHtml.Add((object) content1);
      return tableSectionHtml;
    }

    private static void PopulateEnvironmentIssues(
      ReleaseEnvironment environment,
      ref List<string> errors,
      ref List<string> warnings)
    {
      int maxAttemptNumber = SummaryMailHtmlGenerator.GetMaxAttemptNumber(environment.PreDeployApprovals);
      List<ReleaseTask> tasksForAttempt = SummaryMailHtmlGenerator.GetTasksForAttempt(environment, maxAttemptNumber);
      tasksForAttempt.AddRange((IEnumerable<ReleaseTask>) SummaryMailHtmlGenerator.GetJobTasksForSpecificAttempt(environment, maxAttemptNumber));
      SummaryMailHtmlGenerator.PopulateIssues(tasksForAttempt, ref errors, ref warnings);
      SummaryMailHtmlGenerator.PopulateErrorsFromDeployStep(environment, ref errors);
    }

    private static void PopulateErrorsFromDeployStep(
      ReleaseEnvironment environment,
      ref List<string> errors)
    {
      DeploymentAttempt deploymentAttempt = SummaryMailHtmlGenerator.GetLatestDeploymentAttempt(environment.DeploySteps);
      if (deploymentAttempt == null || deploymentAttempt.ErrorLog.IsNullOrEmpty<char>())
        return;
      string[] strArray = deploymentAttempt.ErrorLog.Split('\n');
      strArray.AddRange<string, string[]>(deploymentAttempt.Issues.Select<Issue, string>((Func<Issue, string>) (e => e.Message)));
      IEnumerable<string> collection = ((IEnumerable<string>) strArray).ToList<string>().Select<string, string>((Func<string, string>) (log => log.Trim()));
      errors.AddRange(collection);
    }

    private static DeploymentAttempt GetLatestDeploymentAttempt(
      List<DeploymentAttempt> deploymentAttempts)
    {
      int num = 0;
      DeploymentAttempt deploymentAttempt1 = (DeploymentAttempt) null;
      if (deploymentAttempts != null)
      {
        foreach (DeploymentAttempt deploymentAttempt2 in deploymentAttempts)
        {
          if (num < deploymentAttempt2.Attempt)
          {
            num = deploymentAttempt2.Attempt;
            deploymentAttempt1 = deploymentAttempt2;
          }
        }
      }
      return deploymentAttempt1;
    }

    private static void PopulateIssues(
      List<ReleaseTask> tasks,
      ref List<string> errors,
      ref List<string> warnings)
    {
      string strB1 = "Error";
      string strB2 = "Warning";
      foreach (ReleaseTask task in tasks)
      {
        if (task.Issues != null && task.Issues.Count > 0)
        {
          foreach (Issue issue in task.Issues)
          {
            if (string.Compare(issue.IssueType, strB1, StringComparison.OrdinalIgnoreCase) == 0)
              errors.Add(issue.Message);
            else if (string.Compare(issue.IssueType, strB2, StringComparison.OrdinalIgnoreCase) == 0)
              warnings.Add(issue.Message);
          }
        }
      }
    }

    private static IList<ReleaseTask> GetJobTasksForSpecificAttempt(
      ReleaseEnvironment environment,
      int attempt)
    {
      List<ReleaseTask> forSpecificAttempt = new List<ReleaseTask>();
      if (environment.DeploySteps != null && environment.DeploySteps.Count > 0)
      {
        DeploymentAttempt deploymentAttempt = environment.DeploySteps.FirstOrDefault<DeploymentAttempt>((Func<DeploymentAttempt, bool>) (s => s.Attempt == attempt));
        if (deploymentAttempt != null)
          forSpecificAttempt.AddRange(deploymentAttempt.ReleaseDeployPhases.GetAllJobs());
      }
      return (IList<ReleaseTask>) forSpecificAttempt;
    }

    private static List<ReleaseTask> GetTasksForAttempt(ReleaseEnvironment environment, int attempt)
    {
      List<ReleaseTask> tasksForAttempt = new List<ReleaseTask>();
      if (environment.DeploySteps != null && environment.DeploySteps.Count > 0)
      {
        DeploymentAttempt deploymentAttempt = environment.DeploySteps.FirstOrDefault<DeploymentAttempt>((Func<DeploymentAttempt, bool>) (step => step.Attempt == attempt));
        if (deploymentAttempt != null)
          tasksForAttempt.AddRange(deploymentAttempt.ReleaseDeployPhases.GetAllTasks());
      }
      return tasksForAttempt;
    }

    private static XElement GetEnvironmentTableRow(ReleaseEnvironment environment)
    {
      int maxAttemptNumber = SummaryMailHtmlGenerator.GetMaxAttemptNumber(environment.PreDeployApprovals);
      XElement environmentTableRow = new XElement((XName) "tr");
      environmentTableRow.Add((object) new XElement((XName) "td", new object[3]
      {
        (object) environment.Name,
        (object) new XAttribute((XName) "align", (object) "left"),
        (object) new XAttribute((XName) "style", (object) "padding: 2px; width: 30%;")
      }));
      environmentTableRow.Add((object) new XElement((XName) "td", new object[3]
      {
        (object) SummaryMailHtmlGenerator.GetReleaseEnvironmentStatusElement(environment),
        (object) new XAttribute((XName) "align", (object) "center"),
        (object) new XAttribute((XName) "style", (object) "padding: 2px; padding-left: 25px;")
      }));
      environmentTableRow.Add((object) new XElement((XName) "td", new object[3]
      {
        (object) SummaryMailHtmlGenerator.GetEnvironmentStartTimeAsString(environment, maxAttemptNumber),
        (object) new XAttribute((XName) "align", (object) "left"),
        (object) new XAttribute((XName) "style", (object) "padding: 2px;padding-left: 25px;")
      }));
      environmentTableRow.Add((object) new XElement((XName) "td", new object[3]
      {
        (object) SummaryMailHtmlGenerator.GetEnvironmentDuration(environment, maxAttemptNumber),
        (object) new XAttribute((XName) "align", (object) "left"),
        (object) new XAttribute((XName) "style", (object) "padding: 2px; padding-left: 25px;")
      }));
      return environmentTableRow;
    }

    private static string GetEnvironmentStartTimeAsString(
      ReleaseEnvironment environment,
      int maxTrial)
    {
      DateTime? environmentStartTime = SummaryMailHtmlGenerator.GetEnvironmentStartTime(environment, maxTrial);
      return !environmentStartTime.HasValue ? "-" : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} (UTC)", (object) environmentStartTime);
    }

    private static DateTime? GetEnvironmentStartTime(ReleaseEnvironment environment, int maxTrial) => environment.PreDeployApprovals.FirstOrDefault<ReleaseApproval>((Func<ReleaseApproval, bool>) (a => a.Attempt == maxTrial))?.CreatedOn;

    private static DateTime? GetEnvironmentEndTime(ReleaseEnvironment environment, int maxTrial)
    {
      List<ReleaseApproval> all1 = environment.PostDeployApprovals.FindAll((Predicate<ReleaseApproval>) (a => a.Attempt == maxTrial));
      if (all1.Count > 0)
        return new DateTime?(all1.Max<ReleaseApproval, DateTime>((Func<ReleaseApproval, DateTime>) (a => a.ModifiedOn)));
      List<ReleaseTask> tasksForAttempt = SummaryMailHtmlGenerator.GetTasksForAttempt(environment, maxTrial);
      if (tasksForAttempt.Count > 0)
        return tasksForAttempt.Max<ReleaseTask, DateTime?>((Func<ReleaseTask, DateTime?>) (t => t.FinishTime));
      List<ReleaseApproval> all2 = environment.PreDeployApprovals.FindAll((Predicate<ReleaseApproval>) (a => a.Attempt == maxTrial));
      return all2.Count > 0 ? new DateTime?(all2.Max<ReleaseApproval, DateTime>((Func<ReleaseApproval, DateTime>) (a => a.ModifiedOn))) : new DateTime?();
    }

    private static string GetEnvironmentDuration(ReleaseEnvironment environment, int maxTrial)
    {
      DateTime? environmentStartTime = SummaryMailHtmlGenerator.GetEnvironmentStartTime(environment, maxTrial);
      if (!environmentStartTime.HasValue)
        return string.Empty;
      switch (environment.Status)
      {
        case EnvironmentStatus.Succeeded:
        case EnvironmentStatus.Canceled:
        case EnvironmentStatus.Rejected:
        case EnvironmentStatus.PartiallySucceeded:
          DateTime? environmentEndTime = SummaryMailHtmlGenerator.GetEnvironmentEndTime(environment, maxTrial);
          if (!environmentEndTime.HasValue)
            return string.Empty;
          DateTime? nullable1 = environmentEndTime;
          DateTime? nullable2 = environmentStartTime;
          return (nullable1.HasValue & nullable2.HasValue ? new TimeSpan?(nullable1.GetValueOrDefault() - nullable2.GetValueOrDefault()) : new TimeSpan?()).Value.ToString("hh\\:mm\\:ss", (IFormatProvider) CultureInfo.InvariantCulture);
        default:
          return string.Empty;
      }
    }

    private static int GetMaxAttemptNumber(List<ReleaseApproval> approvals)
    {
      int maxAttemptNumber = 1;
      if (approvals != null && approvals.Count > 0)
        approvals.ForEach((Action<ReleaseApproval>) (approval => maxAttemptNumber = Math.Max(maxAttemptNumber, approval.Attempt)));
      return maxAttemptNumber;
    }

    private static string GetCreatedByText(Release release) => release.Reason.IsAutoTriggeredRelease() ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MailCreatedByContinuousIntegrationForFormat, (object) release.CreatedBy.DisplayName, (object) release.CreatedOn.ToString("G", (IFormatProvider) CultureInfo.InvariantCulture)) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.MailCreatedByManuallyTriggerdFormat, (object) release.CreatedBy.DisplayName, (object) release.CreatedOn.ToString("G", (IFormatProvider) CultureInfo.InvariantCulture));

    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)", Justification = "Is culture invariant")]
    private static XElement GetReleaseEnvironmentStatusElement(ReleaseEnvironment environment)
    {
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      string str1;
      string str2;
      if (environment.DeploySteps == null || environment.DeploySteps.Count == 0)
      {
        str1 = Resources.EnvironmentNotDeployedStatusText;
        str2 = "#555555;";
      }
      else
      {
        DeploymentAttempt deployment = environment.DeploySteps.OrderByDescending<DeploymentAttempt, int>((Func<DeploymentAttempt, int>) (dep => dep.Attempt)).First<DeploymentAttempt>();
        str1 = SummaryMailHtmlGenerator.GetStatusTextFromDeployment(deployment);
        str2 = SummaryMailHtmlGenerator.GetStatusColorFromDeployment(deployment);
      }
      string str3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "width: 100%; height: 20px; padding: 1px; color: #FFFFFF; background-color: {0}", (object) str2);
      return new XElement((XName) "div", new object[2]
      {
        (object) str1.ToUpper(CultureInfo.CurrentCulture),
        (object) new XAttribute((XName) "style", (object) str3)
      });
    }

    private static string GetStatusTextFromDeployment(DeploymentAttempt deployment)
    {
      if (deployment == null)
        return Resources.EnvironmentNotDeployedStatusText;
      switch (deployment.Status)
      {
        case DeploymentStatus.Undefined:
        case DeploymentStatus.NotDeployed:
          switch (deployment.OperationStatus)
          {
            case DeploymentOperationStatus.Rejected:
              return Resources.EnvironmentRejectedStatusText;
            case DeploymentOperationStatus.Canceled:
              return Resources.EnvironmentCanceledStatusText;
            default:
              return Resources.EnvironmentNotDeployedStatusText;
          }
        case DeploymentStatus.InProgress:
          return Resources.EnvironmentInProgressStatusText;
        case DeploymentStatus.Succeeded:
          return Resources.EnvironmentSucceededStatusText;
        case DeploymentStatus.PartiallySucceeded:
          return Resources.EnvironmentPartiallySucceededStatusText;
        case DeploymentStatus.Failed:
          return Resources.EnvironmentFailedStatusText;
        default:
          return Resources.EnvironmentNotDeployedStatusText;
      }
    }

    private static string GetStatusColorFromDeployment(DeploymentAttempt deployment)
    {
      if (deployment == null)
        return "#555555;";
      switch (deployment.Status)
      {
        case DeploymentStatus.NotDeployed:
          return "#555555;";
        case DeploymentStatus.InProgress:
          return "#007acc;";
        case DeploymentStatus.Succeeded:
          return "#339933;";
        case DeploymentStatus.PartiallySucceeded:
          return "#FF8000;";
        case DeploymentStatus.Failed:
          return "#e51400;";
        default:
          return "#555555;";
      }
    }

    private static string GetReleaseLogsUrl(
      IVssRequestContext requestContext,
      string projectName,
      int releaseId)
    {
      return WebAccessUrlBuilder.GetReleaseLogsWebAccessUri(WebAccessUrlBuilder.GetCollectionUrl(requestContext), projectName, releaseId);
    }

    private static string GetReleaseUrl(
      IVssRequestContext requestContext,
      string projectName,
      int releaseId)
    {
      return WebAccessUrlBuilder.GetReleaseWebAccessUri(requestContext, projectName, releaseId);
    }

    private static string GetReleaseDefinitionUrl(
      IVssRequestContext requestContext,
      string projectName,
      int definitionId)
    {
      return WebAccessUrlBuilder.GetReleaseDefinitionWebAccessUri(requestContext, projectName, definitionId);
    }

    private static string GetBuildDefinitionUrl(
      IVssRequestContext requestContext,
      string projectName,
      int buildDefinitonId)
    {
      return WebAccessUrlBuilder.GetBuildDefinitionWebAccessUri(WebAccessUrlBuilder.GetCollectionUrl(requestContext), projectName, buildDefinitonId);
    }

    private static string GetBuildUrl(
      IVssRequestContext requestContext,
      string projectName,
      int buildId)
    {
      return WebAccessUrlBuilder.GetBuildWebAccessUri(WebAccessUrlBuilder.GetCollectionUrl(requestContext), projectName, buildId);
    }

    private static string GetWorkItemUrl(
      IVssRequestContext requestContext,
      string projectName,
      int? id)
    {
      return WebAccessUrlBuilder.GetWorkItemWebAccessUri(WebAccessUrlBuilder.GetCollectionUrl(requestContext), projectName, id);
    }
  }
}
