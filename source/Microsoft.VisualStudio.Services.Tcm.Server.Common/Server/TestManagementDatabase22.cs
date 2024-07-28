// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase22
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase22 : TestManagementDatabase21
  {
    internal TestManagementDatabase22(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase22()
    {
    }

    public override List<TestConfiguration> QueryTestConfigurations2(
      Guid projectId,
      Dictionary<string, List<object>> parametersMap,
      int planId,
      out List<KeyValuePair<string, TestConfiguration>> areaUris)
    {
      string[] names = (string[]) null;
      int[] ids = (int[]) null;
      byte[] states = (byte[]) null;
      if (parametersMap.ContainsKey("Name"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        names = Array.ConvertAll<object, string>(parametersMap["Name"].ToArray(), TestManagementDatabase22.\u003C\u003EO.\u003C0\u003E__ToString ?? (TestManagementDatabase22.\u003C\u003EO.\u003C0\u003E__ToString = new Converter<object, string>(Convert.ToString)));
      }
      if (parametersMap.ContainsKey("Id"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        ids = Array.ConvertAll<object, int>(parametersMap["Id"].ToArray(), TestManagementDatabase22.\u003C\u003EO.\u003C1\u003E__ToInt32 ?? (TestManagementDatabase22.\u003C\u003EO.\u003C1\u003E__ToInt32 = new Converter<object, int>(Convert.ToInt32)));
      }
      if (parametersMap.ContainsKey("State"))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        states = Array.ConvertAll<object, byte>(parametersMap["State"].ToArray(), TestManagementDatabase22.\u003C\u003EO.\u003C2\u003E__ToByte ?? (TestManagementDatabase22.\u003C\u003EO.\u003C2\u003E__ToByte = new Converter<object, byte>(Convert.ToByte)));
      }
      this.PrepareStoredProcedure("TestManagement.prc_QueryConfigurations2");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindNameTypeTable("@configNames", (IEnumerable<string>) names);
      this.BindIdTypeTable("@configIds", (IEnumerable<int>) ids);
      this.BindTestManagement_TinyIntTypeTable("@states", (IEnumerable<byte>) states);
      if (parametersMap.ContainsKey("IsDefault"))
        this.BindBoolean("@isDefault", Convert.ToBoolean(parametersMap["IsDefault"].First<object>()));
      this.BindInt("@planId", planId);
      areaUris = new List<KeyValuePair<string, TestConfiguration>>();
      return this.GetTestConfigurationsFromReader(this.ExecuteReader(), areaUris);
    }
  }
}
