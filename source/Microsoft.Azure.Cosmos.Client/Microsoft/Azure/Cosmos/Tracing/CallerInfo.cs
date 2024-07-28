// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tracing.CallerInfo
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos.Tracing
{
  internal readonly struct CallerInfo
  {
    public CallerInfo(string memberName, string filePath, int lineNumber)
    {
      this.MemberName = memberName ?? throw new ArgumentNullException(nameof (memberName));
      this.FilePath = filePath ?? throw new ArgumentNullException(nameof (filePath));
      this.LineNumber = lineNumber >= 0 ? lineNumber : throw new ArgumentOutOfRangeException(nameof (lineNumber));
    }

    public string MemberName { get; }

    public string FilePath { get; }

    public int LineNumber { get; }
  }
}
