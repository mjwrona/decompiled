// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.AttachmentFilter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class AttachmentFilter : IAttachmentFilter
  {
    public readonly AttachmentType attachmentType;
    public readonly TestLogType testLogType;
    public readonly TestLogScope testLogScope;
    public readonly HashSet<string> coverageExtensions;
    public readonly string moduleName;

    public AttachmentFilter(
      AttachmentType attachmentType,
      TestLogType testLogType,
      TestLogScope testLogScope,
      ISet<string> coverageExtensions)
    {
      this.attachmentType = attachmentType;
      this.testLogType = testLogType;
      this.testLogScope = testLogScope;
      this.coverageExtensions = coverageExtensions != null && coverageExtensions.Any<string>() ? new HashSet<string>((IEnumerable<string>) coverageExtensions, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : throw new ArgumentException("coverageExtensions cannot be null or empty.");
    }

    public TestLogScope GetTestLogScope() => this.testLogScope;

    public TestLogType GetTestLogType() => this.testLogType;

    public bool IsMatch(TestAttachment attachment) => attachment.AttachmentType == this.attachmentType && this.coverageExtensions.Contains(Path.GetExtension(attachment.FileName));

    public bool IsMatch(TestLog testLog) => testLog.LogReference.Type == this.testLogType && this.coverageExtensions.Contains(Path.GetExtension(testLog.LogReference.FilePath));

    public ISet<string> GetKnownExtensions() => (ISet<string>) this.coverageExtensions;
  }
}
