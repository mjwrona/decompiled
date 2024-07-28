// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LogStorePathFormatter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class LogStorePathFormatter : ILogStorePathFormatter
  {
    private const string ReleaseEnvDirectoryPrefix = "ReleaseEnv";
    private const string RunDirectoryPrefix = "Run";
    private const string ResultDirectoryPrefix = "Result";
    private const string SubResultDirectoryPrefix = "SubResult";
    private const string AttachmentIdDirectoryPrefix = "Attachment";
    private const string TestTagJsonFileName = "Info.json";
    private const string TestTagName = "TestTag";
    private const string PathSeparator = "/";
    private const string ResultIdSeparator = "-";
    private const string AttachmentLogReferencePrefix = "Virtual";

    public string GetBlobReferenceName(
      TestLogReference logReference,
      TestLogType type,
      string destFilePath,
      bool isDuplicate)
    {
      string blobReferenceName = (string) null;
      if (isDuplicate)
        destFilePath = this.GetNameWhenDuplicate(destFilePath);
      string prefix = (string) null;
      if (logReference.Scope == TestLogScope.Release)
        prefix = this._getBlobNamePrefixForEnvironment(logReference.ReleaseEnvId);
      if (logReference.Scope == TestLogScope.Build || logReference.Scope == TestLogScope.Release)
        blobReferenceName = this._getBlobNameAtRootLevel(type, destFilePath, prefix);
      else if (logReference.Scope == TestLogScope.Run)
        blobReferenceName = logReference.ResultId != 0 || logReference.SubResultId != 0 ? (logReference.SubResultId != 0 ? this._getBlobNameAtSubResultLevel(logReference.ResultId, logReference.SubResultId, type, destFilePath) : this._getBlobNameAtResultLevel(logReference.ResultId, type, destFilePath)) : this._getBlobNameAtRootLevel(type, destFilePath, prefix);
      return blobReferenceName;
    }

    public string GetBlobReferenceNameForTagFile(TestLogReference logReference, string tagName)
    {
      string referenceNameForTagFile = (string) null;
      string prefix = (string) null;
      if (logReference.Scope == TestLogScope.Release)
        prefix = this._getBlobNamePrefixForEnvironment(logReference.ReleaseEnvId);
      if ((logReference.Scope == TestLogScope.Build || logReference.Scope == TestLogScope.Release) && !string.IsNullOrEmpty(tagName) && logReference.RunId > 0)
        referenceNameForTagFile = this._getRunLevelTagBlobName(logReference.RunId, tagName, prefix);
      return referenceNameForTagFile;
    }

    public string GetDirectoryPrefixForTag(TestLogReference logReference)
    {
      if (logReference.Scope == TestLogScope.Release || logReference.Scope == TestLogScope.Build)
      {
        if (logReference.Scope == TestLogScope.Release && logReference.ReleaseEnvId > 0)
          return this._getBlobNamePrefixForEnvironment(logReference.ReleaseEnvId) + "TestTag";
        if (logReference.Scope == TestLogScope.Build)
          return "TestTag";
      }
      return (string) null;
    }

    public string GetNameWhenDuplicate(string name)
    {
      string fileName = Path.GetFileName(name);
      string directoryName = Path.GetDirectoryName(name);
      string nameWhenDuplicate;
      if (!string.IsNullOrEmpty(directoryName))
        nameWhenDuplicate = directoryName + "/" + Path.GetFileNameWithoutExtension(fileName) + Guid.NewGuid().ToString() + Path.GetExtension(fileName);
      else
        nameWhenDuplicate = Path.GetFileNameWithoutExtension(fileName) + Guid.NewGuid().ToString() + Path.GetExtension(fileName);
      return nameWhenDuplicate;
    }

    public bool ValidateFileName(string fileName) => !string.IsNullOrEmpty(fileName) && fileName.IndexOf('/') != fileName.Length && fileName.IndexOf('.') != fileName.Length;

    public string SanitizeFilePath(string filePath)
    {
      if (!string.IsNullOrEmpty(filePath))
      {
        filePath = filePath.Replace("\\", "/");
        filePath = filePath.TrimStart('/', '\\');
        filePath = filePath.TrimEnd('/', '\\');
      }
      return filePath;
    }

    public string GetDestinationBlobName(string fileName, string destDirectoryPath)
    {
      if (!string.IsNullOrEmpty(destDirectoryPath))
      {
        destDirectoryPath = destDirectoryPath.Replace("\\", "/");
        destDirectoryPath = destDirectoryPath.TrimStart('/', '\\');
        destDirectoryPath = destDirectoryPath.TrimEnd('/', '\\');
        fileName = destDirectoryPath + "/" + fileName;
      }
      return fileName;
    }

    public TestLogReference GetTestLogReferenceFromBlobName(
      TestLogScope scope,
      int containerId,
      string blobName)
    {
      TestLogReference referenceFromBlobName = new TestLogReference();
      referenceFromBlobName.Scope = scope;
      string[] blobName1 = this._parseBlobName(blobName);
      if (blobName1 == null)
      {
        switch (scope)
        {
          case TestLogScope.Build:
            referenceFromBlobName.BuildId = containerId;
            break;
          case TestLogScope.Release:
            referenceFromBlobName.ReleaseId = containerId;
            break;
          default:
            referenceFromBlobName.RunId = containerId;
            break;
        }
        return referenceFromBlobName;
      }
      int index1 = 0;
      if (scope == TestLogScope.Build || scope == TestLogScope.Release)
      {
        if (scope == TestLogScope.Release)
        {
          referenceFromBlobName.ReleaseId = containerId;
          int id = this._parseId("ReleaseEnv", blobName1[index1]);
          if (id == 0)
            return referenceFromBlobName;
          referenceFromBlobName.ReleaseEnvId = id;
          ++index1;
        }
        else
          referenceFromBlobName.BuildId = containerId;
        TestLogType result;
        if (Enum.TryParse<TestLogType>(blobName1[index1], out result))
        {
          int index2 = index1 + 1;
          referenceFromBlobName.Type = result;
          referenceFromBlobName.FilePath = this._getFileName(blobName1, index2);
          return referenceFromBlobName;
        }
        if (!blobName1[index1].Equals("TestTag", StringComparison.OrdinalIgnoreCase))
          ;
      }
      else
      {
        referenceFromBlobName.RunId = containerId;
        TestLogType result;
        if (Enum.TryParse<TestLogType>(blobName1[0], out result))
        {
          referenceFromBlobName.Type = result;
          referenceFromBlobName.FilePath = this._getFileName(blobName1, 1);
          return referenceFromBlobName;
        }
        if (blobName1.Length > 2 && Enum.TryParse<TestLogType>(blobName1[1], out result))
        {
          referenceFromBlobName.ResultId = this._parseId("Result", blobName1[0]);
          referenceFromBlobName.Type = result;
          referenceFromBlobName.FilePath = this._getFileName(blobName1, 2);
          return referenceFromBlobName;
        }
        if (blobName1.Length > 3 && Enum.TryParse<TestLogType>(blobName1[2], out result))
        {
          referenceFromBlobName.ResultId = this._parseId("Result", blobName1[0]);
          referenceFromBlobName.SubResultId = this._parseId("SubResult", blobName1[1]);
          referenceFromBlobName.Type = result;
          referenceFromBlobName.FilePath = this._getFileName(blobName1, 3);
          return referenceFromBlobName;
        }
      }
      return referenceFromBlobName;
    }

    public TestTagReference GetTestTagReferenceFromBlobName(
      TestLogScope scope,
      int containerId,
      string blobName,
      int releaseEnvIdToMatch)
    {
      TestTagReference referenceFromBlobName = new TestTagReference();
      referenceFromBlobName.Scope = scope;
      string[] blobName1 = this._parseBlobName(blobName);
      int index = 0;
      if (blobName1 != null && (scope == TestLogScope.Build || scope == TestLogScope.Release))
      {
        if (scope == TestLogScope.Release)
        {
          referenceFromBlobName.ReleaseId = containerId;
          int id = this._parseId("ReleaseEnv", blobName1[index]);
          if (id == 0 || id != releaseEnvIdToMatch)
            return referenceFromBlobName;
          referenceFromBlobName.ReleaseEnvId = id;
          ++index;
        }
        else
          referenceFromBlobName.BuildId = containerId;
        if (blobName1[index].Equals("TestTag", StringComparison.OrdinalIgnoreCase) && blobName1.Length >= index + 2)
        {
          string str = blobName1[index + 1];
          int id = this._parseId("Run", blobName1[index + 2]);
          if (id > 0 && !string.IsNullOrEmpty(str))
          {
            referenceFromBlobName.RunId = id;
            referenceFromBlobName.TagName = str;
          }
        }
      }
      return referenceFromBlobName;
    }

    public string GetTestTagNameFromPrefix(TestLogScope scope, string directoryPath)
    {
      string[] blobName = this._parseBlobName(directoryPath);
      if (blobName != null && (scope == TestLogScope.Build || scope == TestLogScope.Release))
      {
        if (scope == TestLogScope.Release)
        {
          if (blobName.Length >= 3)
            return blobName[2];
        }
        else if (scope == TestLogScope.Build && blobName.Length >= 2)
          return blobName[1];
      }
      return (string) null;
    }

    public string GetFilePathForAttachmentTestLog(AttachmentTestLogReference testLogReference)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this._getAttachmentLogPrefix(testLogReference));
      if (testLogReference.AttachmentId > 0)
      {
        stringBuilder.Append(this._getAttachmentIdPrefix(testLogReference.AttachmentId));
        stringBuilder.Append("/");
        stringBuilder.Append(testLogReference.FilePath);
      }
      return stringBuilder.ToString();
    }

    public AttachmentTestLogReference GetAttachmentTestLogReferenceFromBlobName(
      int containerId,
      string blobName)
    {
      AttachmentTestLogReference referenceFromBlobName = new AttachmentTestLogReference();
      referenceFromBlobName.Scope = TestLogScope.Run;
      string[] blobName1 = this._parseBlobName(blobName);
      referenceFromBlobName.RunId = containerId;
      if (blobName1.Length >= 3 && this._checkIfItHasVirtualInPath(blobName1[0]))
      {
        referenceFromBlobName.AttachmentId = this._parseId("Attachment", blobName1[1]);
        referenceFromBlobName.FilePath = this._getFileName(blobName1, 2);
      }
      else if (blobName1.Length >= 4 && this._checkIfItHasVirtualInPath(blobName1[1]))
      {
        referenceFromBlobName.ResultId = this._parseId("Result", blobName1[0]);
        referenceFromBlobName.AttachmentId = this._parseId("Attachment", blobName1[2]);
        referenceFromBlobName.FilePath = this._getFileName(blobName1, 3);
      }
      else if (blobName1.Length >= 5 && this._checkIfItHasVirtualInPath(blobName1[2]))
      {
        referenceFromBlobName.ResultId = this._parseId("Result", blobName1[0]);
        referenceFromBlobName.SubResultId = this._parseId("SubResult", blobName1[1]);
        referenceFromBlobName.AttachmentId = this._parseId("Attachment", blobName1[3]);
        referenceFromBlobName.FilePath = this._getFileName(blobName1, 4);
      }
      return referenceFromBlobName;
    }

    public bool ValidateRunTagName(string tagName, string allowedSpecialChars) => !string.IsNullOrEmpty(tagName) && ((IEnumerable<char>) tagName.ToCharArray()).All<char>((Func<char, bool>) (c => char.IsLetter(c) || char.IsNumber(c) || allowedSpecialChars.Contains<char>(c)));

    private bool _checkIfItHasVirtualInPath(string split) => split.Equals("Virtual");

    private string _getAttachmentLogPrefix(
      AttachmentTestLogReference attachmentTestLogReference)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (attachmentTestLogReference.ResultId > 0)
      {
        stringBuilder.Append("Result");
        stringBuilder.Append("-");
        stringBuilder.Append(attachmentTestLogReference.ResultId);
        stringBuilder.Append("/");
      }
      if (attachmentTestLogReference.SubResultId > 0)
      {
        stringBuilder.Append("SubResult");
        stringBuilder.Append("-");
        stringBuilder.Append(attachmentTestLogReference.SubResultId);
        stringBuilder.Append("/");
      }
      stringBuilder.Append("Virtual");
      stringBuilder.Append("/");
      return stringBuilder.ToString();
    }

    private string _getAttachmentIdPrefix(int attachmentId) => "Attachment-" + attachmentId.ToString();

    private string _getFileName(string[] directory, int index)
    {
      StringBuilder stringBuilder = new StringBuilder();
      int length = directory.Length;
      for (int index1 = index; index1 < directory.Length - 1; ++index1)
      {
        stringBuilder.Append(directory[index1]);
        stringBuilder.Append("/");
      }
      stringBuilder.Append(directory[directory.Length - 1]);
      return stringBuilder.ToString();
    }

    private int _parseId(string name, string value)
    {
      string[] strArray = value.Split("-"[0]);
      int result;
      return strArray.Length == 2 && strArray[0].Equals(name, StringComparison.OrdinalIgnoreCase) && int.TryParse(strArray[1], out result) ? result : 0;
    }

    private string _getBlobNameAtRootLevel(TestLogType logType, string fileName, string prefix)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (!string.IsNullOrEmpty(prefix))
        stringBuilder.Append(prefix);
      stringBuilder.Append(this._getTestLogType(logType));
      stringBuilder.Append(fileName);
      return stringBuilder.ToString();
    }

    private string _getBlobNamePrefixForEnvironment(int releaseEnvId)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("ReleaseEnv-" + releaseEnvId.ToString());
      stringBuilder.Append("/");
      return stringBuilder.ToString();
    }

    private string _getBlobNameAtResultLevel(
      int testResultId,
      TestLogType logType,
      string fileName)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("Result-" + testResultId.ToString());
      stringBuilder.Append("/");
      stringBuilder.Append(this._getTestLogType(logType));
      stringBuilder.Append(fileName);
      return stringBuilder.ToString();
    }

    private string _getBlobNameAtSubResultLevel(
      int testResultId,
      int testSubResultId,
      TestLogType logType,
      string fileName)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("Result-" + testResultId.ToString());
      stringBuilder.Append("/");
      stringBuilder.Append("SubResult-" + testSubResultId.ToString());
      stringBuilder.Append("/");
      stringBuilder.Append(this._getTestLogType(logType));
      stringBuilder.Append(fileName);
      return stringBuilder.ToString();
    }

    private string _getTestLogType(TestLogType type) => type == (TestLogType) 0 ? (string) null : type.ToString() + "/";

    private string _getRunLevelTagBlobName(int runId, string tag, string prefix)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (!string.IsNullOrEmpty(prefix))
        stringBuilder.Append(prefix);
      stringBuilder.Append("TestTag");
      stringBuilder.Append("/");
      stringBuilder.Append(tag);
      stringBuilder.Append("/");
      stringBuilder.Append("Run-" + runId.ToString());
      stringBuilder.Append("/");
      stringBuilder.Append("Info.json");
      return stringBuilder.ToString();
    }

    private string[] _parseBlobName(string blobName)
    {
      if (string.IsNullOrEmpty(blobName))
        return (string[]) null;
      string[] strArray = blobName.Split('/', '\\');
      return strArray.Length < 2 ? (string[]) null : strArray;
    }
  }
}
