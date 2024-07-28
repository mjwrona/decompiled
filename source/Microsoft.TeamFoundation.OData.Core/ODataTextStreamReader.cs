// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataTextStreamReader
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.IO;

namespace Microsoft.OData
{
  internal sealed class ODataTextStreamReader : TextReader
  {
    private Func<char[], int, int, int> reader;

    internal ODataTextStreamReader(Func<char[], int, int, int> reader) => this.reader = reader;

    public override int Read(char[] buffer, int offset, int count) => this.reader(buffer, offset, count);
  }
}
