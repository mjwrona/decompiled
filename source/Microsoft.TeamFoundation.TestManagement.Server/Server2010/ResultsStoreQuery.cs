// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server2010.ResultsStoreQuery
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server2010
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class ResultsStoreQuery
  {
    private string m_queryText;
    private bool m_dayPrecision;
    private string m_timeZone;
    private string m_teamProjectName;

    public string QueryText
    {
      get => this.m_queryText;
      set => this.m_queryText = value;
    }

    public bool DayPrecision
    {
      get => this.m_dayPrecision;
      set => this.m_dayPrecision = value;
    }

    public string TimeZone
    {
      get => this.m_timeZone;
      set => this.m_timeZone = value;
    }

    public string TeamProjectName
    {
      get => this.m_teamProjectName;
      set => this.m_teamProjectName = value;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ResultsStoreQuery QueryText='{0}' ProjectName ='{1}'", (object) this.m_queryText, (object) this.m_teamProjectName);
  }
}
