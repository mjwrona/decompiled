// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.FileSpecBottomUpComparer
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class FileSpecBottomUpComparer : IComparer, IComparer<string>
  {
    int IComparer.Compare(object x, object y) => FileSpec.CompareBottomUpUI((string) x, (string) y);

    int IComparer<string>.Compare(string x, string y) => FileSpec.CompareBottomUpUI(x, y);
  }
}
