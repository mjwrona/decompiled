// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CollectionAvatarColorUtil
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Drawing;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class CollectionAvatarColorUtil
  {
    private static readonly Color c_colorBlue = Color.FromArgb(37, 116, 204);
    private static readonly Color[] c_colorPalette = new Color[14]
    {
      Color.FromArgb(104, 123, 33),
      Color.FromArgb(76, 119, 168),
      Color.FromArgb(177, 87, 144),
      Color.FromArgb(0, 133, 0),
      Color.FromArgb(30, 113, 69),
      Color.FromArgb(217, 0, 128),
      Color.FromArgb(126, 56, 120),
      Color.FromArgb(96, 60, 186),
      Color.FromArgb(98, 113, 171),
      Color.FromArgb(0, 130, 129),
      CollectionAvatarColorUtil.c_colorBlue,
      Color.FromArgb(43, 87, 151),
      Color.FromArgb(196, 74, 38),
      Color.FromArgb(185, 29, 71)
    };

    public static Color GetColorFromName(string displayName)
    {
      if (displayName == null)
        return CollectionAvatarColorUtil.c_colorBlue;
      int num1 = 0;
      byte[] bytes = Encoding.UTF8.GetBytes(displayName);
      for (int index = bytes.Length - 1; index >= 0; --index)
      {
        int num2 = index % 8;
        byte num3 = bytes[index];
        num1 ^= ((int) num3 << num2) + ((int) num3 >> 8 - num2);
      }
      return CollectionAvatarColorUtil.c_colorPalette[num1 % CollectionAvatarColorUtil.c_colorPalette.Length];
    }
  }
}
