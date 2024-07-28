// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Component.StateMachine
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.Component
{
  internal class StateMachine
  {
    public static KeyValuePair<int, int> FindKeywords(string[] keywords, string text, int offset)
    {
      StateMachine.State state = StateMachine.State.Start;
      for (int index = offset; index < text.Length; ++index)
      {
        switch (state)
        {
          case StateMachine.State.Start:
            switch (text[index])
            {
              case 'C':
                state = StateMachine.State.State_K701952337_80_1;
                continue;
              case 'e':
                state = StateMachine.State.State_K1483067490_108_1;
                continue;
              case 'l':
                state = StateMachine.State.State_K403743348_111_1;
                continue;
              case 'p':
                state = StateMachine.State.State_K493357053_104_1;
                continue;
              case 'r':
                state = StateMachine.State.State_K1740371959_101_1;
                continue;
              default:
                continue;
            }
          case StateMachine.State.State_K701952337_80_1:
            state = text[index] != 'P' ? StateMachine.State.Start : StateMachine.State.State_K701952337_85_2;
            break;
          case StateMachine.State.State_K701952337_85_2:
            state = text[index] != 'U' ? StateMachine.State.Start : StateMachine.State.State_K701952337_32_3;
            break;
          case StateMachine.State.State_K701952337_32_3:
            state = text[index] != ' ' ? StateMachine.State.Start : StateMachine.State.State_K701952337_116_4;
            break;
          case StateMachine.State.State_K701952337_116_4:
            state = text[index] != 't' ? StateMachine.State.Start : StateMachine.State.State_K701952337_105_5;
            break;
          case StateMachine.State.State_K701952337_105_5:
            state = text[index] != 'i' ? StateMachine.State.Start : StateMachine.State.State_K701952337_109_6;
            break;
          case StateMachine.State.State_K701952337_109_6:
            state = text[index] != 'm' ? StateMachine.State.Start : StateMachine.State.State_K701952337_101_7;
            break;
          case StateMachine.State.State_K701952337_101_7:
            state = text[index] != 'e' ? StateMachine.State.Start : StateMachine.State.State_K701952337_32_8;
            break;
          case StateMachine.State.State_K701952337_32_8:
            state = text[index] != ' ' ? StateMachine.State.Start : StateMachine.State.State_K701952337_61_9;
            break;
          case StateMachine.State.State_K701952337_61_9:
            state = text[index] != '=' ? StateMachine.State.Start : StateMachine.State.State_K701952337_32_10;
            break;
          case StateMachine.State.State_K701952337_32_10:
            if (text[index] == ' ')
              return new KeyValuePair<int, int>(0, index);
            state = StateMachine.State.Start;
            break;
          case StateMachine.State.State_K403743348_111_1:
            state = text[index] != 'o' ? StateMachine.State.Start : StateMachine.State.State_K403743348_103_2;
            break;
          case StateMachine.State.State_K403743348_103_2:
            state = text[index] != 'g' ? StateMachine.State.Start : StateMachine.State.State_K403743348_105_3;
            break;
          case StateMachine.State.State_K403743348_105_3:
            state = text[index] != 'i' ? StateMachine.State.Start : StateMachine.State.State_K403743348_99_4;
            break;
          case StateMachine.State.State_K403743348_99_4:
            state = text[index] != 'c' ? StateMachine.State.Start : StateMachine.State.State_K403743348_97_5;
            break;
          case StateMachine.State.State_K403743348_97_5:
            state = text[index] != 'a' ? StateMachine.State.Start : StateMachine.State.State_K403743348_108_6;
            break;
          case StateMachine.State.State_K403743348_108_6:
            state = text[index] != 'l' ? StateMachine.State.Start : StateMachine.State.State_K403743348_32_7;
            break;
          case StateMachine.State.State_K403743348_32_7:
            state = text[index] != ' ' ? StateMachine.State.Start : StateMachine.State.State_K403743348_114_8;
            break;
          case StateMachine.State.State_K403743348_114_8:
            state = text[index] != 'r' ? StateMachine.State.Start : StateMachine.State.State_K403743348_101_9;
            break;
          case StateMachine.State.State_K403743348_101_9:
            state = text[index] != 'e' ? StateMachine.State.Start : StateMachine.State.State_K403743348_97_10;
            break;
          case StateMachine.State.State_K403743348_97_10:
            state = text[index] != 'a' ? StateMachine.State.Start : StateMachine.State.State_K403743348_100_11;
            break;
          case StateMachine.State.State_K403743348_100_11:
            state = text[index] != 'd' ? StateMachine.State.Start : StateMachine.State.State_K403743348_115_12;
            break;
          case StateMachine.State.State_K403743348_115_12:
            state = text[index] != 's' ? StateMachine.State.Start : StateMachine.State.State_K403743348_32_13;
            break;
          case StateMachine.State.State_K403743348_32_13:
            if (text[index] == ' ')
              return new KeyValuePair<int, int>(1, index);
            state = StateMachine.State.Start;
            break;
          case StateMachine.State.State_K493357053_104_1:
            state = text[index] != 'h' ? StateMachine.State.Start : StateMachine.State.State_K493357053_121_2;
            break;
          case StateMachine.State.State_K493357053_121_2:
            state = text[index] != 'y' ? StateMachine.State.Start : StateMachine.State.State_K493357053_115_3;
            break;
          case StateMachine.State.State_K493357053_115_3:
            state = text[index] != 's' ? StateMachine.State.Start : StateMachine.State.State_K493357053_105_4;
            break;
          case StateMachine.State.State_K493357053_105_4:
            state = text[index] != 'i' ? StateMachine.State.Start : StateMachine.State.State_K493357053_99_5;
            break;
          case StateMachine.State.State_K493357053_99_5:
            state = text[index] != 'c' ? StateMachine.State.Start : StateMachine.State.State_K493357053_97_6;
            break;
          case StateMachine.State.State_K493357053_97_6:
            state = text[index] != 'a' ? StateMachine.State.Start : StateMachine.State.State_K493357053_108_7;
            break;
          case StateMachine.State.State_K493357053_108_7:
            state = text[index] != 'l' ? StateMachine.State.Start : StateMachine.State.State_K493357053_32_8;
            break;
          case StateMachine.State.State_K493357053_32_8:
            state = text[index] != ' ' ? StateMachine.State.Start : StateMachine.State.State_K493357053_114_9;
            break;
          case StateMachine.State.State_K493357053_114_9:
            state = text[index] != 'r' ? StateMachine.State.Start : StateMachine.State.State_K493357053_101_10;
            break;
          case StateMachine.State.State_K493357053_101_10:
            state = text[index] != 'e' ? StateMachine.State.Start : StateMachine.State.State_K493357053_97_11;
            break;
          case StateMachine.State.State_K493357053_97_11:
            state = text[index] != 'a' ? StateMachine.State.Start : StateMachine.State.State_K493357053_100_12;
            break;
          case StateMachine.State.State_K493357053_100_12:
            state = text[index] != 'd' ? StateMachine.State.Start : StateMachine.State.State_K493357053_115_13;
            break;
          case StateMachine.State.State_K493357053_115_13:
            state = text[index] != 's' ? StateMachine.State.Start : StateMachine.State.State_K493357053_32_14;
            break;
          case StateMachine.State.State_K493357053_32_14:
            if (text[index] == ' ')
              return new KeyValuePair<int, int>(2, index);
            state = StateMachine.State.Start;
            break;
          case StateMachine.State.State_K1483067490_108_1:
            state = text[index] != 'l' ? StateMachine.State.Start : StateMachine.State.State_K1483067490_97_2;
            break;
          case StateMachine.State.State_K1483067490_97_2:
            state = text[index] != 'a' ? StateMachine.State.Start : StateMachine.State.State_K1483067490_112_3;
            break;
          case StateMachine.State.State_K1483067490_112_3:
            state = text[index] != 'p' ? StateMachine.State.Start : StateMachine.State.State_K1483067490_115_4;
            break;
          case StateMachine.State.State_K1483067490_115_4:
            state = text[index] != 's' ? StateMachine.State.Start : StateMachine.State.State_K1483067490_101_5;
            break;
          case StateMachine.State.State_K1483067490_101_5:
            state = text[index] != 'e' ? StateMachine.State.Start : StateMachine.State.State_K1483067490_100_6;
            break;
          case StateMachine.State.State_K1483067490_100_6:
            state = text[index] != 'd' ? StateMachine.State.Start : StateMachine.State.State_K1483067490_32_7;
            break;
          case StateMachine.State.State_K1483067490_32_7:
            state = text[index] != ' ' ? StateMachine.State.Start : StateMachine.State.State_K1483067490_116_8;
            break;
          case StateMachine.State.State_K1483067490_116_8:
            state = text[index] != 't' ? StateMachine.State.Start : StateMachine.State.State_K1483067490_105_9;
            break;
          case StateMachine.State.State_K1483067490_105_9:
            state = text[index] != 'i' ? StateMachine.State.Start : StateMachine.State.State_K1483067490_109_10;
            break;
          case StateMachine.State.State_K1483067490_109_10:
            state = text[index] != 'm' ? StateMachine.State.Start : StateMachine.State.State_K1483067490_101_11;
            break;
          case StateMachine.State.State_K1483067490_101_11:
            state = text[index] != 'e' ? StateMachine.State.Start : StateMachine.State.State_K1483067490_32_12;
            break;
          case StateMachine.State.State_K1483067490_32_12:
            state = text[index] != ' ' ? StateMachine.State.Start : StateMachine.State.State_K1483067490_61_13;
            break;
          case StateMachine.State.State_K1483067490_61_13:
            state = text[index] != '=' ? StateMachine.State.Start : StateMachine.State.State_K1483067490_32_14;
            break;
          case StateMachine.State.State_K1483067490_32_14:
            if (text[index] == ' ')
              return new KeyValuePair<int, int>(3, index);
            state = StateMachine.State.Start;
            break;
          case StateMachine.State.State_K1740371959_101_1:
            state = text[index] != 'e' ? StateMachine.State.Start : StateMachine.State.State_K1740371959_97_2;
            break;
          case StateMachine.State.State_K1740371959_97_2:
            state = text[index] != 'a' ? StateMachine.State.Start : StateMachine.State.State_K1740371959_100_3;
            break;
          case StateMachine.State.State_K1740371959_100_3:
            state = text[index] != 'd' ? StateMachine.State.Start : StateMachine.State.State_K1740371959_45_4;
            break;
          case StateMachine.State.State_K1740371959_45_4:
            state = text[index] != '-' ? StateMachine.State.Start : StateMachine.State.State_K1740371959_97_5;
            break;
          case StateMachine.State.State_K1740371959_97_5:
            state = text[index] != 'a' ? StateMachine.State.Start : StateMachine.State.State_K1740371959_104_6;
            break;
          case StateMachine.State.State_K1740371959_104_6:
            state = text[index] != 'h' ? StateMachine.State.Start : StateMachine.State.State_K1740371959_101_7;
            break;
          case StateMachine.State.State_K1740371959_101_7:
            state = text[index] != 'e' ? StateMachine.State.Start : StateMachine.State.State_K1740371959_97_8;
            break;
          case StateMachine.State.State_K1740371959_97_8:
            state = text[index] != 'a' ? StateMachine.State.Start : StateMachine.State.State_K1740371959_100_9;
            break;
          case StateMachine.State.State_K1740371959_100_9:
            state = text[index] != 'd' ? StateMachine.State.Start : StateMachine.State.State_K1740371959_32_10;
            break;
          case StateMachine.State.State_K1740371959_32_10:
            state = text[index] != ' ' ? StateMachine.State.Start : StateMachine.State.State_K1740371959_114_11;
            break;
          case StateMachine.State.State_K1740371959_114_11:
            state = text[index] != 'r' ? StateMachine.State.Start : StateMachine.State.State_K1740371959_101_12;
            break;
          case StateMachine.State.State_K1740371959_101_12:
            state = text[index] != 'e' ? StateMachine.State.Start : StateMachine.State.State_K1740371959_97_13;
            break;
          case StateMachine.State.State_K1740371959_97_13:
            state = text[index] != 'a' ? StateMachine.State.Start : StateMachine.State.State_K1740371959_100_14;
            break;
          case StateMachine.State.State_K1740371959_100_14:
            state = text[index] != 'd' ? StateMachine.State.Start : StateMachine.State.State_K1740371959_115_15;
            break;
          case StateMachine.State.State_K1740371959_115_15:
            state = text[index] != 's' ? StateMachine.State.Start : StateMachine.State.State_K1740371959_32_16;
            break;
          case StateMachine.State.State_K1740371959_32_16:
            if (text[index] == ' ')
              return new KeyValuePair<int, int>(4, index);
            state = StateMachine.State.Start;
            break;
        }
      }
      return new KeyValuePair<int, int>(-1, -1);
    }

    private enum State
    {
      Start,
      State_K701952337_67_0,
      State_K701952337_80_1,
      State_K701952337_85_2,
      State_K701952337_32_3,
      State_K701952337_116_4,
      State_K701952337_105_5,
      State_K701952337_109_6,
      State_K701952337_101_7,
      State_K701952337_32_8,
      State_K701952337_61_9,
      State_K701952337_32_10,
      State_K403743348_108_0,
      State_K403743348_111_1,
      State_K403743348_103_2,
      State_K403743348_105_3,
      State_K403743348_99_4,
      State_K403743348_97_5,
      State_K403743348_108_6,
      State_K403743348_32_7,
      State_K403743348_114_8,
      State_K403743348_101_9,
      State_K403743348_97_10,
      State_K403743348_100_11,
      State_K403743348_115_12,
      State_K403743348_32_13,
      State_K493357053_112_0,
      State_K493357053_104_1,
      State_K493357053_121_2,
      State_K493357053_115_3,
      State_K493357053_105_4,
      State_K493357053_99_5,
      State_K493357053_97_6,
      State_K493357053_108_7,
      State_K493357053_32_8,
      State_K493357053_114_9,
      State_K493357053_101_10,
      State_K493357053_97_11,
      State_K493357053_100_12,
      State_K493357053_115_13,
      State_K493357053_32_14,
      State_K1483067490_101_0,
      State_K1483067490_108_1,
      State_K1483067490_97_2,
      State_K1483067490_112_3,
      State_K1483067490_115_4,
      State_K1483067490_101_5,
      State_K1483067490_100_6,
      State_K1483067490_32_7,
      State_K1483067490_116_8,
      State_K1483067490_105_9,
      State_K1483067490_109_10,
      State_K1483067490_101_11,
      State_K1483067490_32_12,
      State_K1483067490_61_13,
      State_K1483067490_32_14,
      State_K1740371959_114_0,
      State_K1740371959_101_1,
      State_K1740371959_97_2,
      State_K1740371959_100_3,
      State_K1740371959_45_4,
      State_K1740371959_97_5,
      State_K1740371959_104_6,
      State_K1740371959_101_7,
      State_K1740371959_97_8,
      State_K1740371959_100_9,
      State_K1740371959_32_10,
      State_K1740371959_114_11,
      State_K1740371959_101_12,
      State_K1740371959_97_13,
      State_K1740371959_100_14,
      State_K1740371959_115_15,
      State_K1740371959_32_16,
    }
  }
}
