// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.At
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public static class At
  {
    public static IPosition Top() => (IPosition) new At.TopPosition();

    public static IPosition Before(string name) => (IPosition) new At.BeforePosition(name);

    public static IPosition After(string name) => (IPosition) new At.AfterPosition(name);

    private abstract class Position : IPosition
    {
      protected string Name { get; private set; }

      public Position(string name) => this.Name = name;

      public abstract void Insert(XElement root, XElement item, Func<XElement, string, bool> match);
    }

    private class TopPosition : At.Position
    {
      internal TopPosition()
        : base((string) null)
      {
      }

      public override void Insert(XElement root, XElement item, Func<XElement, string, bool> match) => root.AddFirst((object) item);
    }

    private class BeforePosition : At.Position
    {
      public BeforePosition(string name)
        : base(name)
      {
      }

      public override void Insert(XElement root, XElement item, Func<XElement, string, bool> match) => root.Elements().Single<XElement>((Func<XElement, bool>) (x => match(x, this.Name))).AddBeforeSelf((object) item);
    }

    private class AfterPosition : At.Position
    {
      public AfterPosition(string name)
        : base(name)
      {
      }

      public override void Insert(XElement root, XElement item, Func<XElement, string, bool> match) => root.Elements().Single<XElement>((Func<XElement, bool>) (x => match(x, this.Name))).AddAfterSelf((object) item);
    }
  }
}
