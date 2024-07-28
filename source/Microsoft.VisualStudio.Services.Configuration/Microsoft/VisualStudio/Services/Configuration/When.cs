// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.When
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public static class When
  {
    public static ICondition IsTokenTrue(string token) => (ICondition) new When.IfTokenTrue(token);

    public static bool TryParse(XElement node, out ICondition condition) => When.Condition.TryParse(node, out condition);

    private abstract class Condition : ICondition
    {
      public string Predicate { get; }

      public Condition(string predicate) => this.Predicate = predicate;

      public static bool TryParse(XElement node, out ICondition condition)
      {
        if (node.Name.LocalName == typeof (When.IfTokenTrue).Name)
        {
          condition = (ICondition) new When.IfTokenTrue(node);
          return true;
        }
        condition = (ICondition) null;
        return false;
      }

      public abstract void Materialize(ref XElement root);

      public abstract bool? Evaluate();
    }

    private class IfTokenTrue : When.Condition
    {
      private static readonly XName s_tokenName = XNamespace.None.GetName("token");

      internal IfTokenTrue(string token)
        : base(token)
      {
      }

      internal IfTokenTrue(XElement node)
        : base(node.Attribute(When.IfTokenTrue.s_tokenName).Value)
      {
      }

      public override void Materialize(ref XElement root)
      {
        XElement content = new XElement(XNamespace.None.GetName(this.GetType().Name));
        content.Add((object) new XAttribute(When.IfTokenTrue.s_tokenName, (object) this.Predicate));
        content.Add((object) root);
        root.ReplaceWith((object) content);
        root = content;
      }

      public override bool? Evaluate()
      {
        bool result;
        return bool.TryParse(this.Predicate, out result) ? new bool?(result) : new bool?();
      }
    }
  }
}
