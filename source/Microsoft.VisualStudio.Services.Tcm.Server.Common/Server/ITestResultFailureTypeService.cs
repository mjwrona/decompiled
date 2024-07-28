// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITestResultFailureTypeService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [DefaultServiceImplementation(typeof (TestResultFailureTypeHelper))]
  public interface ITestResultFailureTypeService : IVssFrameworkService
  {
    List<Microsoft.TeamFoundation.TestManagement.WebApi.TestResultFailureType> GetTestResultFailureType(
      TestManagementRequestContext context,
      string ProjectName);

    Microsoft.TeamFoundation.TestManagement.WebApi.TestResultFailureType CreateTestResultFailureType(
      TestManagementRequestContext context,
      string failureTypeName,
      string ProjectName);

    bool DeleteTestResultFailureType(
      TestManagementRequestContext context,
      int failureTypeId,
      string ProjectName);
  }
}
