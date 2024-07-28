// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.UpdatedRunProperties
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class UpdatedRunProperties : UpdatedProperties
  {
    [XmlIgnore]
    public int TotalTests { get; set; }

    [XmlIgnore]
    public int PassedTests { get; set; }

    [XmlIgnore]
    public int FailedTests { get; set; }

    [XmlIgnore]
    public bool IsRunStarted { get; set; }

    [XmlIgnore]
    public bool IsRunCompleted { get; set; }

    [XmlIgnore]
    public DateTime CompleteDate { get; set; }
  }
}
