// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Common.Internal.ArtifactHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1401105B-6771-499A-8DF3-F3CBE1BB3AE4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ArtifactHelper
  {
    private static int[] s_emptyIdArray = Array.Empty<int>();

    public static NameValueCollection ExtractQueryParameters(
      Uri artifactUri,
      out string baseArtifactUri)
    {
      ArtifactId artifactId = artifactUri != (Uri) null ? LinkingUtilities.DecodeUri(artifactUri.ToString()) : (ArtifactId) null;
      if (artifactId != null && VssStringComparer.ArtifactTool.Equals(artifactId.Tool, "TestManagement") && VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "TcmResultAttachment"))
      {
        baseArtifactUri = artifactUri.ToString();
        if (!string.IsNullOrEmpty(artifactUri.Query))
          baseArtifactUri = baseArtifactUri.Substring(0, artifactUri.ToString().IndexOf('?'));
        return UriUtility.ParseQueryString(artifactUri.Query);
      }
      baseArtifactUri = artifactUri.ToString();
      return new NameValueCollection();
    }

    public static bool ParseAttachmentId(Uri artifactUri, out int attachmentId)
    {
      ArtifactId artifactId = artifactUri != (Uri) null ? LinkingUtilities.DecodeUri(artifactUri.ToString()) : (ArtifactId) null;
      if (artifactId != null && VssStringComparer.ArtifactTool.Equals(artifactId.Tool, "TestManagement") && VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "TcmResultAttachment") && ArtifactHelper.ParseAttachmentId(artifactId.ToolSpecificId, out attachmentId))
        return true;
      attachmentId = 0;
      return false;
    }

    public static bool ParseTestCaseResultId(
      Uri artifactUri,
      out int testRunId,
      out int testResultId)
    {
      ArtifactId artifactId = artifactUri != (Uri) null ? LinkingUtilities.DecodeUri(artifactUri.ToString()) : (ArtifactId) null;
      if (artifactId != null && VssStringComparer.ArtifactTool.Equals(artifactId.Tool, "TestManagement") && VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "TcmResult") && ArtifactHelper.ParseTestCaseResultId(artifactId.ToolSpecificId, out testRunId, out testResultId))
        return true;
      testRunId = 0;
      testResultId = 0;
      return false;
    }

    public static bool ParseTestCaseResultAttachmentId(
      Uri artifactUri,
      out int testRunId,
      out int testResultId,
      out int attachmentId,
      out int sessionId)
    {
      ArtifactId artifactId = artifactUri != (Uri) null ? LinkingUtilities.DecodeUri(artifactUri.ToString()) : (ArtifactId) null;
      if (artifactId != null && VssStringComparer.ArtifactTool.Equals(artifactId.Tool, "TestManagement") && VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "TcmResultAttachment") && ArtifactHelper.ParseTestCaseResultAttachmentId(artifactId.ToolSpecificId, out testRunId, out testResultId, out attachmentId, out sessionId))
        return true;
      testRunId = 0;
      testResultId = 0;
      attachmentId = 0;
      sessionId = 0;
      return false;
    }

    public static bool ParseTestCaseResultId(
      string toolSpecificId,
      out int testRunId,
      out int testResultId)
    {
      int[] toolSpecificId1 = ArtifactHelper.ParseToolSpecificId(toolSpecificId);
      if (toolSpecificId1 != null && toolSpecificId1.Length == 2)
      {
        testRunId = toolSpecificId1[0];
        testResultId = toolSpecificId1[1];
        return true;
      }
      testRunId = 0;
      testResultId = 0;
      return false;
    }

    public static bool ParseTestCaseResultAttachmentId(
      string toolSpecificId,
      out int testRunId,
      out int testResultId,
      out int attachmentId,
      out int sessionId)
    {
      int[] toolSpecificId1 = ArtifactHelper.ParseToolSpecificId(toolSpecificId);
      testRunId = 0;
      testResultId = 0;
      attachmentId = 0;
      sessionId = 0;
      if (toolSpecificId1 != null)
      {
        if (toolSpecificId1.Length == 3)
        {
          testRunId = toolSpecificId1[0];
          testResultId = toolSpecificId1[1];
          attachmentId = toolSpecificId1[2];
          sessionId = 0;
          return true;
        }
        if (toolSpecificId1.Length == 4)
        {
          testRunId = toolSpecificId1[0];
          testResultId = toolSpecificId1[1];
          attachmentId = toolSpecificId1[2];
          sessionId = toolSpecificId1[3];
          return true;
        }
      }
      return false;
    }

    public static bool ParseSessionId(Uri artifactUri, out int sessionId)
    {
      ArtifactId artifactId = artifactUri != (Uri) null ? LinkingUtilities.DecodeUri(artifactUri.ToString()) : (ArtifactId) null;
      if (artifactId != null && VssStringComparer.ArtifactTool.Equals(artifactId.Tool, "TestManagement") && VssStringComparer.ArtifactType.Equals(artifactId.ArtifactType, "TcmResult"))
      {
        int[] toolSpecificId = ArtifactHelper.ParseToolSpecificId(artifactId.ToolSpecificId);
        if (toolSpecificId != null && toolSpecificId.Length == 1)
        {
          sessionId = toolSpecificId[0];
          return true;
        }
      }
      sessionId = 0;
      return false;
    }

    public static bool ParseAttachmentId(string toolSpecificId, out int attachmentId)
    {
      int[] toolSpecificId1 = ArtifactHelper.ParseToolSpecificId(toolSpecificId);
      attachmentId = 0;
      if (toolSpecificId1 == null)
        return false;
      if (toolSpecificId1.Length == 1)
      {
        attachmentId = toolSpecificId1[0];
        return true;
      }
      if (toolSpecificId1.Length < 3)
        return false;
      attachmentId = toolSpecificId1[2];
      return true;
    }

    private static int[] ParseToolSpecificId(string toolSpecificId)
    {
      int length = toolSpecificId.IndexOf('?');
      if (length != -1)
        toolSpecificId = toolSpecificId.Substring(0, length);
      if (string.IsNullOrEmpty(toolSpecificId))
        return ArtifactHelper.s_emptyIdArray;
      string[] strArray = toolSpecificId.Split('.');
      int[] toolSpecificId1 = new int[strArray.Length];
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (!int.TryParse(strArray[index], out toolSpecificId1[index]) || toolSpecificId1[index] < 0)
          return ArtifactHelper.s_emptyIdArray;
      }
      return toolSpecificId1;
    }

    public static List<int> ConvertWorkItemUrisToIds(List<string> uris)
    {
      List<int> ids = new List<int>(uris.Count);
      foreach (string uri in uris)
      {
        int result;
        if (int.TryParse(LinkingUtilities.DecodeUri(uri).ToolSpecificId, out result))
          ids.Add(result);
      }
      return ids;
    }

    public static string GetArtifactUrlFromHttpUrl(string httpUrl)
    {
      Uri result;
      int runId;
      int resultId;
      int attachmentId;
      return string.IsNullOrEmpty(httpUrl) || !Uri.TryCreate(httpUrl, UriKind.Absolute, out result) || !string.Equals(result.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) && !string.Equals(result.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) || !ArtifactHelper.TryGetArtifactIds(result, out runId, out resultId, out attachmentId) ? string.Empty : LinkingUtilities.EncodeUri(new ArtifactId("TestManagement", "TcmResultAttachment", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.{2}", (object) runId, (object) resultId, (object) attachmentId)));
    }

    private static bool TryGetArtifactIds(
      Uri uri,
      out int runId,
      out int resultId,
      out int attachmentId)
    {
      string query = uri.Query;
      runId = resultId = attachmentId = 0;
      if (string.IsNullOrEmpty(query))
        return false;
      NameValueCollection queryString = UriUtility.ParseQueryString(uri.Query);
      string s1 = queryString[AttachmentDownloadFields.AttachmentId];
      string s2 = queryString[AttachmentDownloadFields.TestRunId];
      string s3 = queryString[AttachmentDownloadFields.TestResultId];
      return !string.IsNullOrEmpty(s1) && !string.IsNullOrEmpty(s2) && !string.IsNullOrEmpty(s3) && int.TryParse(s1, out attachmentId) && attachmentId > 0 && int.TryParse(s2, out runId) && runId >= 0 && int.TryParse(s3, out resultId) && resultId >= 0;
    }
  }
}
