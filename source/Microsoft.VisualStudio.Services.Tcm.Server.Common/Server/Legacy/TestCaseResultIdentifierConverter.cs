// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.TestCaseResultIdentifierConverter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  internal static class TestCaseResultIdentifierConverter
  {
    public static TestCaseResultIdentifier Convert(LegacyTestCaseResultIdentifier identifier)
    {
      if (identifier == null)
        return (TestCaseResultIdentifier) null;
      return new TestCaseResultIdentifier()
      {
        TestRunId = identifier.TestRunId,
        TestResultId = identifier.TestResultId,
        AreaUri = identifier.AreaUri
      };
    }

    public static LegacyTestCaseResultIdentifier Convert(TestCaseResultIdentifier identifier)
    {
      if (identifier == null)
        return (LegacyTestCaseResultIdentifier) null;
      return new LegacyTestCaseResultIdentifier()
      {
        TestRunId = identifier.TestRunId,
        TestResultId = identifier.TestResultId,
        AreaUri = identifier.AreaUri
      };
    }

    public static IEnumerable<LegacyTestCaseResultIdentifier> Convert(
      IEnumerable<TestCaseResultIdentifier> identifiers)
    {
      return identifiers == null ? (IEnumerable<LegacyTestCaseResultIdentifier>) null : identifiers.Select<TestCaseResultIdentifier, LegacyTestCaseResultIdentifier>((Func<TestCaseResultIdentifier, LegacyTestCaseResultIdentifier>) (identifier => TestCaseResultIdentifierConverter.Convert(identifier)));
    }

    public static IEnumerable<TestCaseResultIdentifier> Convert(
      IEnumerable<LegacyTestCaseResultIdentifier> identifiers)
    {
      return identifiers == null ? (IEnumerable<TestCaseResultIdentifier>) null : identifiers.Select<LegacyTestCaseResultIdentifier, TestCaseResultIdentifier>((Func<LegacyTestCaseResultIdentifier, TestCaseResultIdentifier>) (identifier => TestCaseResultIdentifierConverter.Convert(identifier)));
    }
  }
}
