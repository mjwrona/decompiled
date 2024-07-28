// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.LinkingUtilities
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class LinkingUtilities
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string VSTFS = "vstfs:///";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string URISEPARATOR = "/";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string TOOLARTIFACTMONIKER = ".aspx?artifactMoniker=";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string SERVICE = "/Service/";
    private static readonly char[] s_delimiters = new char[3]
    {
      '/',
      '\\',
      '.'
    };

    public static bool IsToolTypeWellFormed(string tool) => !string.IsNullOrEmpty(tool) && tool.IndexOfAny(LinkingUtilities.s_delimiters) < 0;

    public static bool IsUriWellFormed(string artifactUri) => LinkingUtilities.IsUriWellFormed(artifactUri, out string _, out string _, out string _, out string _);

    private static bool IsUriWellFormed(
      string artifactUri,
      out string serverName,
      out string tool,
      out string artifactType,
      out string artifactMoniker)
    {
      serverName = string.Empty;
      tool = (string) null;
      artifactType = (string) null;
      artifactMoniker = (string) null;
      if (artifactUri == null)
        return false;
      string str = artifactUri.Trim();
      if (!str.StartsWith(LinkingUtilities.VSTFS, StringComparison.OrdinalIgnoreCase))
        return false;
      string[] strArray = str.Substring(LinkingUtilities.VSTFS.Length, str.Length - LinkingUtilities.VSTFS.Length).Split(LinkingUtilities.URISEPARATOR.ToCharArray(), 3);
      if (strArray.Length != 3)
        return false;
      tool = strArray[0].Trim();
      if (!LinkingUtilities.IsToolTypeWellFormed(tool))
        return false;
      artifactType = strArray[1].Trim();
      if (!LinkingUtilities.IsArtifactTypeWellFormed(artifactType))
        return false;
      artifactMoniker = strArray[2].Trim();
      return LinkingUtilities.IsArtifactToolSpecificIdWellFormed(artifactMoniker);
    }

    public static bool IsArtifactTypeWellFormed(string artifactType) => !string.IsNullOrEmpty(artifactType) && artifactType.IndexOf('/') < 0;

    public static bool IsArtifactToolSpecificIdWellFormed(string toolSpecificId) => !string.IsNullOrEmpty(toolSpecificId);

    public static bool IsArtifactIdWellFormed(ArtifactId artifactId) => artifactId != null && LinkingUtilities.IsToolTypeWellFormed(artifactId.Tool) && LinkingUtilities.IsArtifactTypeWellFormed(artifactId.ArtifactType) && LinkingUtilities.IsArtifactToolSpecificIdWellFormed(artifactId.ToolSpecificId);

    public static string EncodeUri(ArtifactId artifactId)
    {
      if (!LinkingUtilities.IsArtifactIdWellFormed(artifactId))
      {
        if (artifactId == null)
          throw new ArgumentNullException(nameof (artifactId));
        throw new ArgumentException(CommonResources.MalformedArtifactId((object) artifactId.ToString()), nameof (artifactId));
      }
      string tool = artifactId.Tool;
      string artifactType = artifactId.ArtifactType;
      string toolSpecificId = artifactId.ToolSpecificId;
      return LinkingUtilities.VSTFS + UriUtility.UrlEncode(tool) + LinkingUtilities.URISEPARATOR + UriUtility.UrlEncode(artifactType) + LinkingUtilities.URISEPARATOR + UriUtility.UrlEncode(toolSpecificId);
    }

    public static ArtifactId DecodeUri(string uri)
    {
      string serverName = (string) null;
      string tool = (string) null;
      string artifactType = (string) null;
      string artifactMoniker = (string) null;
      if (LinkingUtilities.IsUriWellFormed(uri, out serverName, out tool, out artifactType, out artifactMoniker))
        return new ArtifactId()
        {
          VisualStudioServerNamespace = serverName,
          Tool = UriUtility.UrlDecode(tool),
          ArtifactType = UriUtility.UrlDecode(artifactType),
          ToolSpecificId = UriUtility.UrlDecode(artifactMoniker)
        };
      ArgumentUtility.CheckForNull<string>(uri, nameof (uri));
      throw new ArgumentException(CommonResources.MalformedUri((object) uri), nameof (uri));
    }

    public static ArrayList RemoveDuplicateArtifacts(ArrayList artifactList)
    {
      Hashtable hashtable = new Hashtable();
      foreach (Artifact artifact in artifactList)
        hashtable[(object) artifact.Uri] = (object) artifact;
      ArrayList arrayList = new ArrayList();
      foreach (Artifact artifact in (IEnumerable) hashtable.Values)
        arrayList.Add((object) artifact);
      return arrayList;
    }

    public static string GetArtifactUri(string artifactUrl)
    {
      string str1 = !string.IsNullOrEmpty(artifactUrl) ? artifactUrl.Trim() : throw new ArgumentNullException(CommonResources.NullArtifactUrl());
      int startIndex = str1.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || str1.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ? str1.IndexOf(LinkingUtilities.TOOLARTIFACTMONIKER, StringComparison.OrdinalIgnoreCase) : throw new ArgumentException(CommonResources.MalformedUrl((object) artifactUrl));
      if (startIndex == -1)
        throw new ArgumentException(CommonResources.MalformedUrl((object) artifactUrl));
      string str2 = str1.Substring(startIndex + LinkingUtilities.TOOLARTIFACTMONIKER.Length);
      if (string.IsNullOrEmpty(str2))
        throw new ArgumentException(CommonResources.MalformedUrl((object) artifactUrl));
      int num1 = str1.LastIndexOf(LinkingUtilities.URISEPARATOR, startIndex, StringComparison.Ordinal);
      if (num1 <= 0 || startIndex - num1 - 2 <= 0)
        throw new ArgumentException(CommonResources.MalformedUrl((object) artifactUrl));
      string str3 = str1.Substring(num1 + 1, startIndex - num1 - 1);
      int num2 = str1.LastIndexOf(LinkingUtilities.URISEPARATOR, num1 - 1, StringComparison.Ordinal);
      if (num2 <= 0 || num1 - num2 - 2 <= 0)
        throw new ArgumentException(CommonResources.MalformedUrl((object) artifactUrl));
      string str4 = str1.Substring(num2 + 1, num1 - num2 - 1);
      return LinkingUtilities.VSTFS + str4 + LinkingUtilities.URISEPARATOR + str3 + LinkingUtilities.URISEPARATOR + str2;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetArtifactUrl(
      string artifactDisplayUrl,
      ArtifactId artId,
      string serverUrl)
    {
      artifactDisplayUrl = !string.IsNullOrEmpty(artifactDisplayUrl) ? artifactDisplayUrl.Trim() : LinkingUtilities.URISEPARATOR + artId.Tool + LinkingUtilities.URISEPARATOR;
      if (!string.IsNullOrEmpty(serverUrl) && artifactDisplayUrl.StartsWith(LinkingUtilities.URISEPARATOR, StringComparison.OrdinalIgnoreCase))
        artifactDisplayUrl = !serverUrl.EndsWith(LinkingUtilities.URISEPARATOR, StringComparison.OrdinalIgnoreCase) ? serverUrl + artifactDisplayUrl : serverUrl + artifactDisplayUrl.Substring(1);
      if (!artifactDisplayUrl.EndsWith(LinkingUtilities.URISEPARATOR, StringComparison.OrdinalIgnoreCase))
        artifactDisplayUrl += LinkingUtilities.URISEPARATOR;
      return artifactDisplayUrl + artId.ArtifactType + LinkingUtilities.TOOLARTIFACTMONIKER + artId.ToolSpecificId;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetServerUrl(Uri serverUri)
    {
      string appSetting = ConfigurationManager.AppSettings["TFSUrlPublic"];
      return !string.IsNullOrEmpty(appSetting) ? appSetting : serverUri.ToString();
    }
  }
}
