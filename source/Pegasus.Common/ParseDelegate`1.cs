// Decompiled with JetBrains decompiler
// Type: Pegasus.Common.ParseDelegate`1
// Assembly: Pegasus.Common, Version=4.0.14.0, Culture=neutral, PublicKeyToken=28c69b6c6d100f4a
// MVID: 081C50C0-D236-41F7-86F9-E3F2168B7118
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Pegasus.Common.dll

using System.Diagnostics.CodeAnalysis;

namespace Pegasus.Common
{
  [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Since it would be a breaking change, this will not be renamed.")]
  [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", Justification = "Necessary for best performance.")]
  public delegate IParseResult<T> ParseDelegate<T>(ref Cursor cursor);
}
