// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.DataAccess.CheckSuiteBinder4
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using System.Data;

namespace Microsoft.Azure.Pipelines.Checks.Server.DataAccess
{
  internal class CheckSuiteBinder4 : CheckSuiteBinder3
  {
    protected override CheckRunStatus GetStatus() => (CheckRunStatus) this.m_status.GetInt16((IDataReader) this.Reader);
  }
}
