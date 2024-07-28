// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultsStoreQuery
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Internal)]
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

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Query={0};TeamProjectName={1};TimeZone={2};DayPrecision={3}", (object) this.m_queryText, (object) this.m_teamProjectName, (object) this.m_timeZone, (object) this.m_dayPrecision);
  }
}
