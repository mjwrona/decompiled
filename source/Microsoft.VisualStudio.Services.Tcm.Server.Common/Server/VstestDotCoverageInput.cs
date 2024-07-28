// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.VstestDotCoverageInput
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [XmlRoot("VstestDotCoverageInput")]
  public class VstestDotCoverageInput : CoverageToolInput
  {
    [XmlIgnore]
    public const string ToolName = "VstestDotCoverageInput";
    [XmlIgnore]
    public const string UniqueFileName = "{F4006627-1F8D-47D9-AF5A-ADEC93EB76EB}";
    private IAttachmentFilter _filter;

    public IAttachmentFilter AttachmentFilter
    {
      get
      {
        if (this._filter == null)
          this._filter = (IAttachmentFilter) new Microsoft.TeamFoundation.TestManagement.Server.AttachmentFilter(AttachmentType.CodeCoverage, TestLogType.CodeCoverage, TestLogScope.Run, (ISet<string>) new HashSet<string>()
          {
            ".coverage"
          });
        return this._filter;
      }
    }

    public VstestDotCoverageInput() => this.Files = new HashSet<CodeCoverageFile>();

    public override bool IsValid() => this.IsCoverageFilePresent();

    public override BatchType BatchType => BatchType.NoBatch;

    private bool IsCoverageFilePresent() => this.Files != null && this.Files.Count != 0 && this.Files.Where<CodeCoverageFile>((Func<CodeCoverageFile, bool>) (x => Path.GetExtension(x.FileName).Equals(".coverage", StringComparison.OrdinalIgnoreCase))).Any<CodeCoverageFile>();
  }
}
