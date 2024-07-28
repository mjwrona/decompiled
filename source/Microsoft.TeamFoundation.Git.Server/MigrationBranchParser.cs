// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.MigrationBranchParser
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class MigrationBranchParser
  {
    private const string c_targetNameDelimeter = "/-target-/";
    private const string c_parentNameDelimeter = "/-parent-/";

    internal static BranchMigrationData Parse(string migrationBranch, string migrationPrefix = null)
    {
      string[] strArray = (migrationPrefix != null ? migrationBranch.Replace(migrationPrefix, string.Empty) : migrationBranch).Split(new string[1]
      {
        "/-target-/"
      }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length == 1)
        return new BranchMigrationData(migrationBranch, migrationBranch, Array.Empty<string>());
      string[] array = strArray[1].Split(new string[1]
      {
        "/-parent-/"
      }, StringSplitOptions.RemoveEmptyEntries);
      return array.Length == 1 ? new BranchMigrationData(strArray[0], strArray[1], Array.Empty<string>()) : new BranchMigrationData(strArray[0], array[0], new ArraySegment<string>(array, 1, array.Length - 1).ToArray<string>());
    }
  }
}
