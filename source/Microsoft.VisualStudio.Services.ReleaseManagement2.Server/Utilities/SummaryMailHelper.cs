// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.SummaryMailHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public sealed class SummaryMailHelper
  {
    private const int ReleaseInfoSectionRank = 5;
    private const int DetailsSectionRank = 10;
    private const int EnvironmentsSectionRank = 30;
    private const int IssuesSectionRank = 40;
    private const int WorkItemsSectionRank = 50;

    private SummaryMailHelper()
    {
    }

    public static Func<IVssRequestContext, ProjectInfo, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release, SummaryMailSection> GetSectionHtmlGenerator(
      MailSectionType sectionType)
    {
      switch (sectionType)
      {
        case MailSectionType.Details:
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return SummaryMailHelper.\u003C\u003EO.\u003C1\u003E__GetDetailsSection ?? (SummaryMailHelper.\u003C\u003EO.\u003C1\u003E__GetDetailsSection = new Func<IVssRequestContext, ProjectInfo, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release, SummaryMailSection>(SummaryMailHelper.GetDetailsSection));
        case MailSectionType.Environments:
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return SummaryMailHelper.\u003C\u003EO.\u003C2\u003E__GetEnvironmentsSection ?? (SummaryMailHelper.\u003C\u003EO.\u003C2\u003E__GetEnvironmentsSection = new Func<IVssRequestContext, ProjectInfo, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release, SummaryMailSection>(SummaryMailHelper.GetEnvironmentsSection));
        case MailSectionType.Issues:
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return SummaryMailHelper.\u003C\u003EO.\u003C3\u003E__GetIssuesSection ?? (SummaryMailHelper.\u003C\u003EO.\u003C3\u003E__GetIssuesSection = new Func<IVssRequestContext, ProjectInfo, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release, SummaryMailSection>(SummaryMailHelper.GetIssuesSection));
        case MailSectionType.WorkItems:
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return SummaryMailHelper.\u003C\u003EO.\u003C4\u003E__GetWorkItemsSection ?? (SummaryMailHelper.\u003C\u003EO.\u003C4\u003E__GetWorkItemsSection = new Func<IVssRequestContext, ProjectInfo, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release, SummaryMailSection>(SummaryMailHelper.GetWorkItemsSection));
        case MailSectionType.ReleaseInfo:
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return SummaryMailHelper.\u003C\u003EO.\u003C0\u003E__GetReleaseInfoSection ?? (SummaryMailHelper.\u003C\u003EO.\u003C0\u003E__GetReleaseInfoSection = new Func<IVssRequestContext, ProjectInfo, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release, SummaryMailSection>(SummaryMailHelper.GetReleaseInfoSection));
        default:
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidMailSectionType, (object) SummaryMailHelper.GetValidMailSectionTypes()));
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "It is not a property")]
    public static IList<MailSectionType> GetAllSummaryMailSections() => (IList<MailSectionType>) new List<MailSectionType>()
    {
      MailSectionType.ReleaseInfo,
      MailSectionType.Details,
      MailSectionType.Environments,
      MailSectionType.Issues,
      MailSectionType.WorkItems
    };

    public static string SafeConvertXElementToString(XNode element)
    {
      string empty = string.Empty;
      if (element != null)
      {
        using (StringWriter output = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
        {
          XmlWriterSettings settings = new XmlWriterSettings()
          {
            OmitXmlDeclaration = true,
            CheckCharacters = false
          };
          using (XmlWriter writer = XmlWriter.Create((TextWriter) output, settings))
            element.WriteTo(writer);
          empty = output.ToString();
        }
      }
      return empty;
    }

    public static XElement SafeConvertStringToXElement(string xmlContent)
    {
      XElement xelement = (XElement) null;
      if (!string.IsNullOrEmpty(xmlContent))
      {
        byte[] bytes = Encoding.UTF8.GetBytes(xmlContent);
        using (MemoryStream input = new MemoryStream(bytes, 0, bytes.Length))
        {
          XmlReaderSettings settings = new XmlReaderSettings()
          {
            CheckCharacters = false
          };
          settings.XmlResolver = (XmlResolver) null;
          using (XmlReader reader = XmlReader.Create((Stream) input, settings))
            xelement = XElement.Load(reader);
        }
      }
      return xelement;
    }

    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Cannot make it read-only")]
    public static void OrderSectionsByRank(List<SummaryMailSection> sections)
    {
      if (sections == null)
        throw new ArgumentNullException(nameof (sections));
      sections.Sort((Comparison<SummaryMailSection>) ((s1, s2) => s1.Rank.CompareTo(s2.Rank)));
    }

    private static SummaryMailSection GetReleaseInfoSection(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release)
    {
      string htmlContent = SummaryMailHelper.SafeConvertXElementToString((XNode) SummaryMailHtmlGenerator.GetReleaseInfo(requestContext, projectInfo.Name, release));
      return new SummaryMailSection(string.Empty, MailSectionType.ReleaseInfo, htmlContent, SummaryMailHelper.GetSectionRank(MailSectionType.ReleaseInfo));
    }

    private static SummaryMailSection GetDetailsSection(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release)
    {
      return new SummaryMailSection(Resources.MailDetailsSectionTitle, MailSectionType.Details, SummaryMailHelper.SafeConvertXElementToString((XNode) SummaryMailHtmlGenerator.GetDetailsSectionHtml(requestContext, projectInfo.Name, release)), SummaryMailHelper.GetSectionRank(MailSectionType.Details));
    }

    private static SummaryMailSection GetEnvironmentsSection(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release)
    {
      return new SummaryMailSection(Resources.MailEnvironmentsSectionTitle, MailSectionType.Environments, SummaryMailHelper.SafeConvertXElementToString((XNode) SummaryMailHtmlGenerator.GetEnvironmentsSectionHtml(release)), SummaryMailHelper.GetSectionRank(MailSectionType.Environments));
    }

    private static SummaryMailSection GetIssuesSection(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release)
    {
      return new SummaryMailSection(Resources.MailIssuesSectionTitle, MailSectionType.Issues, SummaryMailHelper.SafeConvertXElementToString((XNode) SummaryMailHtmlGenerator.GetIssuesSectionHtml(requestContext, projectInfo.Name, release)), SummaryMailHelper.GetSectionRank(MailSectionType.Issues));
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We need to catch all exceptions")]
    private static SummaryMailSection GetWorkItemsSection(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release)
    {
      int num = 0;
      string htmlContent;
      try
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release lastReleaseId = SummaryMailHelper.GetLastReleaseId(requestContext, projectInfo, release);
        int id = lastReleaseId == null ? 0 : lastReleaseId.Id;
        List<WorkItem> list = ReleaseWorkItemsCommitsHelper.GetWorkItems(requestContext, id, release.Id, projectInfo, int.MaxValue).ToList<WorkItem>();
        num = SummaryMailHelper.IsRollback(lastReleaseId, release) ? list.Count * -1 : list.Count;
        htmlContent = SummaryMailHelper.SafeConvertXElementToString((XNode) SummaryMailHtmlGenerator.GetWorkItemsSection(requestContext, projectInfo.Name, (IList<WorkItem>) list));
      }
      catch (Exception ex)
      {
        Trace.TraceError("Exception occurred while fetching work items for release id: {0}, Exception: {1}", (object) release.Id, (object) ex);
        htmlContent = SummaryMailHtmlGenerator.GetUnableToFetchWorkItems().ToString();
      }
      return new SummaryMailSection(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} ({1})", (object) Resources.MailWorkItemsSectionTitle, (object) num), MailSectionType.WorkItems, htmlContent, SummaryMailHelper.GetSectionRank(MailSectionType.WorkItems));
    }

    public static int GetSectionRank(MailSectionType sectionType)
    {
      switch (sectionType)
      {
        case MailSectionType.Details:
          return 10;
        case MailSectionType.Environments:
          return 30;
        case MailSectionType.Issues:
          return 40;
        case MailSectionType.WorkItems:
          return 50;
        default:
          return 0;
      }
    }

    private static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release GetLastReleaseId(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release)
    {
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release> lastTwoReleases = ReleaseWorkItemsCommitsHelper.GetLastTwoReleases(requestContext, release.Id, release.ReleaseDefinitionReference.Id, 0, projectInfo);
      return lastTwoReleases.Count <= 1 ? (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release) null : lastTwoReleases[1].ToContract(requestContext, projectInfo.Id, false);
    }

    private static bool IsRollback(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release previousRelease, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release currentRelease)
    {
      if (previousRelease == null || currentRelease == null || previousRelease.Id == currentRelease.Id)
        return false;
      Artifact artifact1 = currentRelease.Artifacts.SingleOrDefault<Artifact>((Func<Artifact, bool>) (a => a.IsPrimary));
      Artifact artifact2 = previousRelease.Artifacts.SingleOrDefault<Artifact>((Func<Artifact, bool>) (a => a.IsPrimary));
      if (artifact1 == null || artifact2 == null || artifact1.Type != "Build" || !SummaryMailHelper.ArtifactsAreOfSameType(artifact1, artifact2))
        return false;
      int result1 = 0;
      int result2 = 0;
      return artifact1.DefinitionReference.ContainsKey("version") && artifact2.DefinitionReference.ContainsKey("version") && int.TryParse(artifact1.DefinitionReference["version"].Id, out result1) && int.TryParse(artifact2.DefinitionReference["version"].Id, out result2) && result1 < result2;
    }

    private static bool ArtifactsAreOfSameType(Artifact artifact1, Artifact artifact2)
    {
      if (artifact1 == null || artifact2 == null || artifact1.Type != artifact2.Type)
        return false;
      string[] strArray = new string[3]
      {
        "project",
        "definition",
        "branch"
      };
      foreach (string key in strArray)
      {
        if (!artifact1.DefinitionReference.ContainsKey(key) || !artifact2.DefinitionReference.ContainsKey(key) || artifact1.DefinitionReference[key].Id != artifact2.DefinitionReference[key].Id)
          return false;
      }
      return true;
    }

    private static string GetValidMailSectionTypes() => string.Join(", ", ((IEnumerable<MailSectionType>) Enum.GetValues(typeof (MailSectionType))).Where<MailSectionType>((Func<MailSectionType, bool>) (t => t != MailSectionType.TestResults)).Select<MailSectionType, string>((Func<MailSectionType, string>) (s => s.ToString())));
  }
}
