// Decompiled with JetBrains decompiler
// Type: Pegasus.Common.IParseResult`1
// Assembly: Pegasus.Common, Version=4.0.14.0, Culture=neutral, PublicKeyToken=28c69b6c6d100f4a
// MVID: 081C50C0-D236-41F7-86F9-E3F2168B7118
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Pegasus.Common.dll

namespace Pegasus.Common
{
  public interface IParseResult<out T>
  {
    Cursor EndCursor { get; }

    Cursor StartCursor { get; }

    T Value { get; }
  }
}
