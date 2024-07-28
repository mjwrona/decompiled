// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Utility.TyposquattingStringNormalization
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Utility
{
  public static class TyposquattingStringNormalization
  {
    private static readonly IReadOnlyDictionary<string, string> SimilarCharacterDictionary = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>()
    {
      {
        "a",
        "AΑАαаÀÁÂÃÄÅàáâãäåĀāĂăĄąǍǎǞǟǠǡǺǻȀȁȂȃȦȧȺΆάἀἁἂἃἄἅἆἇἈἉἊἋἌΆἍἎἏӐӑӒӓὰάᾀᾁᾂᾃᾄᾅᾆᾇᾈᾊᾋᾌᾍᾎᾏᾰᾱᾲᾳᾴᾶᾷᾸᾹᾺᾼДд"
      },
      {
        "b",
        "BΒВЪЬƀƁƂƃƄƅɃḂḃϦЂБвъьѢѣҌҍႦႪხҔҕӃӄ"
      },
      {
        "c",
        "CСсϹϲÇçĆćĈĉĊċČčƇƈȻȼҪҫ\uD801\uDCA8"
      },
      {
        "d",
        "DƊԁÐĎďĐđƉƋƌǷḊḋԀԂԃ"
      },
      {
        "e",
        "EΕЕеÈÉÊËèéêëĒēĔĕĖėĘęĚěȄȅȆȇȨȩɆɇΈЀЁЄѐёҼҽҾҿӖӗἘἙἚἛἜἝῈΈ"
      },
      {
        "f",
        "FϜƑƒḞḟϝҒғӺӻ"
      },
      {
        "g",
        "GǤԌĜĝĞğĠġĢģƓǥǦǧǴǵԍ"
      },
      {
        "h",
        "HΗНһհҺĤĥħǶȞȟΉἨἩἪἫἬἭἮἯᾘᾙᾚᾛᾜᾝᾞᾟῊΉῌЋнћҢңҤҥӇӈӉӊԊԋԦԧԨԩႬႹ\uD801\uDC85\uD801\uDC8C\uD801\uDC8E\uD801\uDCA3"
      },
      {
        "i",
        "IΙІӀ¡ìíîïǐȉȋΐίιϊіїὶίῐῑῒΐῖῗΊΪȊȈἰἱἲἳἴἵἶἷἸἹἺἻἼἽἾἿῘῙῚΊЇӏÌÍÎÏĨĩĪīĬĭĮįİǏ"
      },
      {
        "j",
        "JЈͿϳĴĵǰȷ"
      },
      {
        "k",
        "KΚКKĶķĸƘƙǨǩκϏЌкќҚқҜҝҞҟҠҡԞԟ"
      },
      {
        "l",
        "LĹĺĻļĽľĿŀŁłſƖƪȴẛ"
      },
      {
        "m",
        "MΜМṀṁϺϻмӍӎ\uD801\uDC84"
      },
      {
        "n",
        "NΝпÑñŃńŅņŇňŉƝǸǹᾐᾑᾒᾓᾔᾕᾖᾗῂῃῄῆῇԤԥԮԯ\uD801\uDC90"
      },
      {
        "o",
        "OΟОՕჿоοÒÓÔÕÖðòóôõöøŌōŎŏŐőƠơǑǒǪǫǬǭȌȍȎȏȪȫȬȭȮȯȰȱΌδόϘϙὀὁὂὃὄὅὈὉὊὋὌὍὸόῸΌӦӧჾ\uD801\uDC86\uD801\uDCA00"
      },
      {
        "p",
        "PΡРрρÞþƤƥƿṖṗϷϸῤῥῬҎҏႲႼ"
      },
      {
        "q",
        "QգԛȡɊɋԚႭႳ"
      },
      {
        "r",
        "RгŔŕŖŗŘřƦȐȑȒȓɌɼѓ"
      },
      {
        "s",
        "SЅѕՏႽჽŚśŜŝŞşŠšȘșȿṠṡ\uD801\uDC96\uD801\uDCA1"
      },
      {
        "t",
        "TΤТͲͳŢţŤťŦŧƬƭƮȚțȾṪṫτтҬҭէ"
      },
      {
        "u",
        "UՍႮÙÚÛÜùúûüŨũŪūŬŭŮůŰűŲųƯưǓǔǕǖǗǘǙǚǛǜȔȕȖȗμυϋύὐὑὒὓὔὕὖὗὺύῠῡῢΰῦῧ\uD801\uDCA9"
      },
      {
        "v",
        "VνѴѵƔƲѶѷ"
      },
      {
        "w",
        "WωшԜԝŴŵƜẀẁẂẃẄẅώШЩщѡѿὠὡὢὣὤὥὦὧὼώᾠᾡᾢᾣᾤᾥᾦᾧῲῳῴῶῷ"
      },
      {
        "x",
        "XХΧх×χҲҳӼӽӾӿჯ"
      },
      {
        "y",
        "YΥҮƳуУÝýÿŶŷŸƴȲȳɎɏỲỳΎΫγϒϓϔЎЧўүҶҷҸҹӋӌӮӯӰӱӲӳӴӵὙὛὝὟῨῩῪΎႯႸ\uD801\uDC8B\uD801\uDCA6"
      },
      {
        "z",
        "ZΖჍŹźŻżŽžƵƶȤȥ"
      },
      {
        "3",
        "ƷЗʒӡჳǮǯȜȝзэӞӟӠ"
      },
      {
        "8",
        "Ȣȣ"
      },
      {
        "_",
        ".-"
      }
    };
    private static readonly IReadOnlyDictionary<string, string> NormalizedMappingDictionary = (IReadOnlyDictionary<string, string>) TyposquattingStringNormalization.GetNormalizedMappingDictionary(TyposquattingStringNormalization.SimilarCharacterDictionary);

    public static string NormalizeString(string str)
    {
      StringBuilder stringBuilder = new StringBuilder();
      TextElementEnumerator elementEnumerator = StringInfo.GetTextElementEnumerator(str);
      while (elementEnumerator.MoveNext())
      {
        string textElement = elementEnumerator.GetTextElement();
        string str1;
        if (TyposquattingStringNormalization.NormalizedMappingDictionary.TryGetValue(textElement, out str1))
          stringBuilder.Append(str1);
        else
          stringBuilder.Append(textElement);
      }
      return stringBuilder.ToString();
    }

    private static Dictionary<string, string> GetNormalizedMappingDictionary(
      IReadOnlyDictionary<string, string> similarCharacterDictionary)
    {
      Dictionary<string, string> mappingDictionary = new Dictionary<string, string>();
      foreach (KeyValuePair<string, string> similarCharacter in (IEnumerable<KeyValuePair<string, string>>) similarCharacterDictionary)
      {
        TextElementEnumerator elementEnumerator = StringInfo.GetTextElementEnumerator(similarCharacter.Value);
        while (elementEnumerator.MoveNext())
          mappingDictionary[elementEnumerator.GetTextElement()] = similarCharacter.Key;
      }
      return mappingDictionary;
    }
  }
}
