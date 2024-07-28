// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TfsRunsHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TfsRunsHelper : RunsHelper
  {
    internal TfsRunsHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    protected internal override DataContractConverter DataContractConverter
    {
      get
      {
        if (this.m_dataContractConverter == null)
          this.m_dataContractConverter = (DataContractConverter) new TfsTestRunDataContractConverter(this.TfsTestManagementRequestContext);
        return this.m_dataContractConverter;
      }
    }

    protected internal virtual TfsTestManagementRequestContext TfsTestManagementRequestContext => this.TestManagementRequestContext as TfsTestManagementRequestContext;
  }
}
