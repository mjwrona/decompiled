// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.ICommonStructureService4
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.TeamFoundation.Server
{
  public interface ICommonStructureService4 : ICommonStructureService3, ICommonStructureService
  {
    string CreateNode(
      string nodeName,
      string parentNodeUri,
      DateTime? startDate,
      DateTime? finishDate);

    void SetIterationDates(string nodeUri, DateTime? startDate, DateTime? finishDate);

    ProjectProperty GetProjectProperty(string projectUri, string name);

    void SetProjectProperty(string projectUri, string name, string value);
  }
}
