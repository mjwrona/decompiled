// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.VstestCoverageInput
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
  [XmlRoot("VstestCoverageInput")]
  public class VstestCoverageInput : CoverageToolInput
  {
    [XmlIgnore]
    public const string ToolName = "VstestCoverageInput";
    private IAttachmentFilter _filter;

    public IAttachmentFilter AttachmentFilter
    {
      get
      {
        if (this._filter == null)
          this._filter = (IAttachmentFilter) new Microsoft.TeamFoundation.TestManagement.Server.AttachmentFilter(AttachmentType.IntermediateCollectorData, TestLogType.Intermediate, TestLogScope.Build, (ISet<string>) new HashSet<string>()
          {
            ".coverage",
            ".coveragebuffer",
            ".covx",
            ".covb"
          });
        return this._filter;
      }
    }

    public VstestCoverageInput() => this.Files = new HashSet<CodeCoverageFile>();

    [XmlAttribute("ModuleName")]
    public string ModuleName { get; set; }

    public override bool IsValid() => this.IsCoverageFilePresent() && this.AreBufferFilesPresent();

    private bool AreBufferFilesPresent() => this.Files != null && this.Files.Count != 0 && this.Files.Where<CodeCoverageFile>((Func<CodeCoverageFile, bool>) (x => Path.GetExtension(x.FileName).Equals(".coveragebuffer", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(x.FileName).Equals(".covb", StringComparison.OrdinalIgnoreCase))).Any<CodeCoverageFile>();

    private bool IsCoverageFilePresent() => this.Files != null && this.Files.Count != 0 && this.Files.Where<CodeCoverageFile>((Func<CodeCoverageFile, bool>) (x => Path.GetExtension(x.FileName).Equals(".coverage", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(x.FileName).Equals(".covx", StringComparison.OrdinalIgnoreCase))).Any<CodeCoverageFile>();
  }
}
