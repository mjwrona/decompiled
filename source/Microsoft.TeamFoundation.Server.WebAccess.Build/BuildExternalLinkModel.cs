// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildExternalLinkModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class BuildExternalLinkModel
  {
    public BuildExternalLinkType LinkType { get; set; }

    public string Url { get; set; }

    public string Text { get; set; }

    public string LocalPath { get; set; }

    public BuildExternalLinkModel(string url, string text)
    {
      this.Url = url;
      this.Text = text;
      this.ResolveLinkType(url);
    }

    private void ResolveLinkType(string url)
    {
      this.LinkType = BuildExternalLinkType.None;
      if (string.IsNullOrEmpty(url))
        return;
      if (url.StartsWith("$/", StringComparison.OrdinalIgnoreCase))
        this.LinkType = BuildExternalLinkType.VersionControlPath;
      else if (url.StartsWith("#", StringComparison.OrdinalIgnoreCase))
        this.LinkType = BuildExternalLinkType.BuildContainerPath;
      else if (LinkingUtilities.IsUriWellFormed(url))
      {
        this.LinkType = BuildExternalLinkType.ArtifactUri;
      }
      else
      {
        Uri uri = url.ToUri();
        if (uri != (Uri) null && (uri.IsUnc || uri.IsFile))
        {
          this.LinkType = BuildExternalLinkType.LocalPath;
          this.LocalPath = HttpUtility.UrlDecode(uri.ToBrowserSafeString());
        }
        else
          this.LinkType = BuildExternalLinkType.Url;
      }
    }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["url"] = (object) this.Url;
      json["text"] = (object) this.Text;
      json["type"] = (object) (int) this.LinkType;
      if (!string.IsNullOrWhiteSpace(this.LocalPath))
        json["localPath"] = (object) this.LocalPath;
      return json;
    }
  }
}
