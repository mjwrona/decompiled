// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.PageMetadataHelperUtilities
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  internal class PageMetadataHelperUtilities
  {
    public string GetProductNameForExtension(PublishedExtension extension)
    {
      string nameForExtension = string.Empty;
      if (extension != null && !extension.InstallationTargets.IsNullOrEmpty<InstallationTarget>())
      {
        foreach (InstallationTarget installationTarget in extension.InstallationTargets)
        {
          if (installationTarget != null && !installationTarget.Target.IsNullOrEmpty<char>())
          {
            if (installationTarget.Target.Equals("Microsoft.VisualStudio.Services", StringComparison.InvariantCultureIgnoreCase) || installationTarget.Target.Equals("Microsoft.VisualStudio.Services.Cloud", StringComparison.InvariantCultureIgnoreCase) || installationTarget.Target.Equals("Microsoft.TeamFoundation.Server", StringComparison.InvariantCultureIgnoreCase) || installationTarget.Target.Equals("Microsoft.VisualStudio.Services.Integration", StringComparison.InvariantCultureIgnoreCase) || installationTarget.Target.Equals("Microsoft.TeamFoundation.Server.Integration", StringComparison.InvariantCultureIgnoreCase) || installationTarget.Target.Equals("Microsoft.VisualStudio.Services.Cloud.Integration", StringComparison.InvariantCultureIgnoreCase))
              nameForExtension = GalleryResources.VSO_Header;
            else if (installationTarget.Target.Equals("Microsoft.VisualStudio.Code", StringComparison.InvariantCultureIgnoreCase))
              nameForExtension = GalleryResources.VSCode;
            else if (installationTarget.Target.Equals("Microsoft.VisualStudio.Offer", StringComparison.InvariantCultureIgnoreCase))
              nameForExtension = GalleryCommonResources.Subs_Header;
            else if (installationTarget.Target.Equals("Microsoft.VisualStudio.Ide", StringComparison.InvariantCultureIgnoreCase))
              nameForExtension = GalleryCommonResources.VS_Header;
            else if (installationTarget.Target.Equals("Microsoft.VisualStudio.Mac", StringComparison.InvariantCultureIgnoreCase))
              nameForExtension = GalleryCommonResources.VSForMac_Header;
          }
        }
      }
      return nameForExtension;
    }

    public string GetExtensionIconUrlFromExtension(PublishedExtension extension)
    {
      string urlFromExtension = string.Empty;
      if (extension != null && !extension.Versions.IsNullOrEmpty<ExtensionVersion>())
      {
        ExtensionVersion version = extension.Versions[0];
        if (!version.Files.IsNullOrEmpty<ExtensionFile>())
        {
          foreach (ExtensionFile file in version.Files)
          {
            if (file.AssetType.Equals("Microsoft.VisualStudio.Services.Icons.Default", StringComparison.InvariantCultureIgnoreCase))
              urlFromExtension = file.Source;
          }
        }
      }
      return urlFromExtension;
    }

    public string GetCommaSeparatedTagsFromExtension(PublishedExtension extension)
    {
      string tagsFromExtension = string.Empty;
      if (extension != null && extension.Tags != null && extension.Tags.Count > 0)
      {
        for (int index = 0; index < extension.Tags.Count; ++index)
        {
          if (!GalleryUtil.IsSystemTag(extension.Tags[index]))
            tagsFromExtension = tagsFromExtension + extension.Tags[index] + ",";
        }
        tagsFromExtension = tagsFromExtension.TrimEnd(',');
      }
      return tagsFromExtension;
    }

    public string GetHtmlStringFromHtmlGenericControls(
      List<HtmlSelfClosingGenericControl> htmlGenericControls)
    {
      string empty = string.Empty;
      if (htmlGenericControls != null)
      {
        foreach (HtmlSelfClosingGenericControl htmlGenericControl in htmlGenericControls)
        {
          if (htmlGenericControl != null)
            empty += this.GetHtmlStringFromHtmlGenericControl(htmlGenericControl);
        }
      }
      return empty;
    }

    public string GetHtmlStringFromHtmlGenericControl(
      HtmlSelfClosingGenericControl htmlGenericControl)
    {
      string empty = string.Empty;
      if (htmlGenericControl != null)
      {
        StringBuilder sb = new StringBuilder();
        using (StringWriter writer1 = new StringWriter(sb))
        {
          using (HtmlTextWriter writer2 = new HtmlTextWriter((TextWriter) writer1))
          {
            htmlGenericControl.RenderControl(writer2);
            empty = sb.ToString();
          }
        }
      }
      return empty;
    }
  }
}
