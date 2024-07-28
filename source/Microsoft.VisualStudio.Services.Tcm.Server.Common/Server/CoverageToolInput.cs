// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageToolInput
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [XmlInclude(typeof (VstestCoverageInput))]
  [XmlInclude(typeof (VstestDotCoverageInput))]
  [XmlInclude(typeof (NativeCoverageInput))]
  [Serializable]
  public abstract class CoverageToolInput
  {
    [XmlAttribute("FriendlyName")]
    public string FriendlyName { get; set; }

    public string ToolType { get; set; }

    public abstract bool IsValid();

    [XmlArray("CodeCoverageFiles")]
    [XmlArrayItem("CodeCoverageFile", typeof (CodeCoverageFile))]
    public HashSet<CodeCoverageFile> Files { get; protected set; }

    [XmlIgnore]
    public virtual BatchType BatchType => BatchType.Default;
  }
}
