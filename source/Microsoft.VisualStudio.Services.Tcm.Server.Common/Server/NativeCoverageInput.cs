// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.NativeCoverageInput
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [XmlRoot("NativeCoverageInput")]
  public class NativeCoverageInput : CoverageToolInput
  {
    [XmlIgnore]
    public const string ToolName = "NativeCoverageInput";
    private IAttachmentFilter _filter;

    public IAttachmentFilter AttachmentFilter
    {
      get
      {
        if (this._filter == null)
          this._filter = (IAttachmentFilter) new Microsoft.TeamFoundation.TestManagement.Server.AttachmentFilter(AttachmentType.IntermediateCollectorData, TestLogType.Intermediate, TestLogScope.Build, (ISet<string>) new HashSet<string>()
          {
            ".cjson"
          });
        return this._filter;
      }
    }

    public NativeCoverageInput() => this.Files = new HashSet<CodeCoverageFile>();

    [XmlAttribute("CoverageFile")]
    public string CoverageFile { get; set; }

    public override bool IsValid() => this.Files.Any<CodeCoverageFile>();
  }
}
