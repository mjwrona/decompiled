// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.NonConfiguredSiteHelper
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class NonConfiguredSiteHelper
  {
    private static Dictionary<NonConfiguredSiteHelper.SiteType, string> m_createdPage = new Dictionary<NonConfiguredSiteHelper.SiteType, string>();
    private static object m_sync = new object();

    public static void ShowNonConfiguredSitePage(NonConfiguredSiteHelper.SiteType type) => Process.Start(NonConfiguredSiteHelper.GenerateNotFoundUri(type));

    public static string GenerateNotFoundUri(NonConfiguredSiteHelper.SiteType type)
    {
      lock (NonConfiguredSiteHelper.m_sync)
      {
        if (NonConfiguredSiteHelper.m_createdPage.ContainsKey(type))
          return NonConfiguredSiteHelper.m_createdPage[type];
        string str = Path.Combine(Path.GetTempPath(), type.ToString() + "_missing_17.0_" + CultureInfo.CurrentUICulture.Name + ".htm");
        if (File.Exists(str))
        {
          try
          {
            File.Delete(str);
          }
          catch (Exception ex)
          {
          }
        }
        if (!File.Exists(str))
        {
          string configuredSiteHtml = NonConfiguredSiteHelper.GetNonConfiguredSiteHtml(type, true);
          try
          {
            File.WriteAllText(str, configuredSiteHtml, Encoding.UTF8);
          }
          catch (Exception ex)
          {
            if (!File.Exists(str))
              throw;
          }
        }
        Uri uri = new Uri(str);
        NonConfiguredSiteHelper.m_createdPage[type] = uri.AbsoluteUri;
        return NonConfiguredSiteHelper.m_createdPage[type];
      }
    }

    public static string GetNonConfiguredSiteHtml(
      NonConfiguredSiteHelper.SiteType type,
      bool imagesFromAssembly)
    {
      string location = "Images";
      if (imagesFromAssembly)
        location = "res://" + Assembly.GetAssembly(typeof (NonConfiguredSiteHelper)).Location + "/Image";
      return NonConfiguredSiteHelper.GetNonConfiguredSiteHtml(type, location);
    }

    public static string GetNonConfiguredSiteHtml(
      NonConfiguredSiteHelper.SiteType type,
      string location)
    {
      string newValue = string.Empty;
      string str = string.Empty;
      int linkId = 0;
      switch (type)
      {
        case NonConfiguredSiteHelper.SiteType.ProcessGuidance:
          newValue = TFCommonResources.EntityModel_ProcessGuidanceNotConfigured();
          str = TFCommonResources.EntityModel_ProcessGuidanceNotConfiguredSubTitle();
          linkId = ExternalHelpLinks.ConfigureProcessGuidance;
          break;
        case NonConfiguredSiteHelper.SiteType.Portal:
          newValue = TFCommonResources.EntityModel_PortalSiteNotConfigured();
          str = TFCommonResources.EntityModel_PortalSiteNotConfiguredSubTitle();
          linkId = ExternalHelpLinks.ConfigurePortal;
          break;
        case NonConfiguredSiteHelper.SiteType.DefaultSiteCreationLocation:
          newValue = TFCommonResources.EntityModel_DefaultSiteCreationLocationNotConfigured();
          str = TFCommonResources.EntityModel_DefaultSiteCreationLocationNotConfiguredSubTitle();
          linkId = ExternalHelpLinks.ConfigureDefaultSiteCreationLocation;
          break;
        case NonConfiguredSiteHelper.SiteType.Reporting:
          newValue = TFCommonResources.EntityModel_ReportingNotConfigured();
          str = TFCommonResources.EntityModel_ReportingNotConfiguredSubTitle();
          linkId = ExternalHelpLinks.ConfigureReporting;
          break;
        case NonConfiguredSiteHelper.SiteType.WebAccess:
          newValue = TFCommonResources.EntityModel_WebAccessNotConfigured();
          str = TFCommonResources.EntityModel_WebAccessNotConfiguredSubTitle();
          linkId = ExternalHelpLinks.ConfigureWebAccess;
          break;
      }
      return NonConfiguredSiteHelper.LoadResource("NotConfigured.html").Replace("[AssemblyPath]", location).Replace("[MainTitle]", newValue).Replace("[Steps]", "<h2>" + UriUtility.HtmlEncode(str) + "</h2>" + "<h2>" + TFCommonResources.EntityModel_AdministratorsMessage((object) ("<a href=\"" + ExternalHelpLinks.GetUri(linkId)?.ToString() + "\">" + TFCommonResources.EntityModel_HelpLink() + "</a>")) + "</h2>");
    }

    private static string LoadResource(string resourceName)
    {
      using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
      {
        using (StreamReader streamReader = new StreamReader(manifestResourceStream, Encoding.UTF8))
          return streamReader.ReadToEnd();
      }
    }

    public enum SiteType
    {
      ProcessGuidance,
      Portal,
      DefaultSiteCreationLocation,
      Reporting,
      WebAccess,
    }
  }
}
