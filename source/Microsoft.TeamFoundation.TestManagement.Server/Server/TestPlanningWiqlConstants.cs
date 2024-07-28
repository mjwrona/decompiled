// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanningWiqlConstants
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class TestPlanningWiqlConstants
  {
    internal static List<Tuple<Type, string, string>> TestPlanningTablesForWiql = new List<Tuple<Type, string, string>>()
    {
      new Tuple<Type, string, string>(typeof (Session), "vw_Session", "Session"),
      new Tuple<Type, string, string>(typeof (ServerTestSuite), "vw_Suite", "TestSuite"),
      new Tuple<Type, string, string>(typeof (TestPlan), "vw_Plan", "TestPlan"),
      new Tuple<Type, string, string>(typeof (TestPoint), "vw_Point", "TestPoint")
    };
  }
}
