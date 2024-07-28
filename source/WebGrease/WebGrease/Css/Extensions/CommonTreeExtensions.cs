// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Extensions.CommonTreeExtensions
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebGrease.Css.Extensions
{
  public static class CommonTreeExtensions
  {
    public static IEnumerable<CommonTree> Children(
      this CommonTree commonTree,
      string childFilterText = null)
    {
      if (commonTree != null && commonTree.Children != null)
      {
        if (!string.IsNullOrWhiteSpace(childFilterText))
        {
          foreach (CommonTree child in commonTree.Children.OfType<CommonTree>().Where<CommonTree>((Func<CommonTree, bool>) (_ => _.Text == childFilterText)))
            yield return child;
        }
        else
        {
          foreach (CommonTree child in commonTree.Children.OfType<CommonTree>())
            yield return child;
        }
      }
    }

    public static IEnumerable<CommonTree> GrandChildren(
      this CommonTree commonTree,
      string childFilterText)
    {
      if (commonTree != null && commonTree.Children != null)
      {
        foreach (CommonTree granchChild in commonTree.Children(childFilterText).SelectMany<CommonTree, CommonTree>((Func<CommonTree, IEnumerable<CommonTree>>) (_ => _.Children())))
          yield return granchChild;
      }
    }

    public static string TextOrDefault(this CommonTree commonTree, string defaultText = null) => commonTree == null ? defaultText : commonTree.ToString();

    public static string FirstChildText(this CommonTree commonTree) => commonTree.FirstChildTextOrDefault();

    public static string FirstChildTextOrDefault(this CommonTree commonTree, string defaultText = null)
    {
      if (commonTree != null)
      {
        CommonTree commonTree1 = commonTree.Children().FirstOrDefault<CommonTree>();
        if (commonTree1 != null)
          return commonTree1.TextOrDefault(defaultText);
      }
      return defaultText;
    }

    public static string FirstChildText(this IEnumerable<CommonTree> commonTree) => commonTree.FirstChildTextOrDefault();

    public static string FirstChildTextOrDefault(
      this IEnumerable<CommonTree> commonTree,
      string defaultText = null)
    {
      if (commonTree != null)
      {
        CommonTree commonTree1 = commonTree.FirstOrDefault<CommonTree>();
        if (commonTree1 != null)
        {
          CommonTree commonTree2 = commonTree1.Children().FirstOrDefault<CommonTree>();
          if (commonTree2 != null)
            return commonTree2.TextOrDefault(defaultText);
        }
      }
      return defaultText;
    }
  }
}
