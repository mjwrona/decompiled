// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ReleaseInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [XmlRoot("Release")]
  [DebuggerDisplay("Version: {Version}, Milestone: {Milestone}, CurrentLevel: {CurrentServiceLevel}, IsRtm: {IsRtm}, IsGoLive: {IsGoLive}, IsEvaluation: {IsEvaluation}")]
  public class ReleaseInfo
  {
    private List<UpdatePackage> m_updatePackages = new List<UpdatePackage>();

    [XmlAttribute("version")]
    public string Version { get; set; }

    [XmlAttribute("milestone")]
    public string Milestone { get; set; }

    [XmlAttribute("isRtm")]
    public bool IsRtm { get; set; }

    [XmlAttribute("isGoLive")]
    public bool IsGoLive { get; set; }

    [XmlAttribute("isEvaluation")]
    public bool IsEvaluation { get; set; }

    [XmlArray("UpdatePackages")]
    [XmlArrayItem("UpdatePackage")]
    public List<UpdatePackage> UpdatePackages => this.m_updatePackages;

    [XmlIgnore]
    internal ServiceLevel CurrentServiceLevel
    {
      get
      {
        UpdatePackage last = this.UpdatePackages.FindLast((Predicate<UpdatePackage>) (update => !update.Removed));
        int patchNumber = 0;
        bool isTestPatch = false;
        if (last != null)
        {
          patchNumber = last.PatchNumber;
          isTestPatch = last.TestPatch;
        }
        return new ServiceLevel(this.Version, this.Milestone, patchNumber, isTestPatch);
      }
    }
  }
}
