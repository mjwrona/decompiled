// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ILogStorePathFormatter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public interface ILogStorePathFormatter
  {
    string GetBlobReferenceName(
      TestLogReference logReference,
      TestLogType type,
      string destFilePath,
      bool isDuplicate);

    string GetBlobReferenceNameForTagFile(TestLogReference logReference, string tagName);

    string GetDirectoryPrefixForTag(TestLogReference logReference);

    string GetNameWhenDuplicate(string name);

    bool ValidateFileName(string fileName);

    string SanitizeFilePath(string filePath);

    string GetDestinationBlobName(string fileName, string destDirectoryPath);

    TestLogReference GetTestLogReferenceFromBlobName(
      TestLogScope scope,
      int containerId,
      string blobName);

    TestTagReference GetTestTagReferenceFromBlobName(
      TestLogScope scope,
      int containerId,
      string blobName,
      int releaseEnvIdToMatch);

    string GetTestTagNameFromPrefix(TestLogScope scope, string directoryPath);

    string GetFilePathForAttachmentTestLog(AttachmentTestLogReference testLogReference);

    AttachmentTestLogReference GetAttachmentTestLogReferenceFromBlobName(
      int containerId,
      string blobName);

    bool ValidateRunTagName(string tagName, string allowedSpecialChars);
  }
}
