// Decompiled with JetBrains decompiler
// Type: YamlDotNet.RepresentationModel.YamlStream
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.RepresentationModel
{
  [Serializable]
  public class YamlStream : IEnumerable<YamlDocument>, IEnumerable
  {
    private readonly IList<YamlDocument> documents = (IList<YamlDocument>) new List<YamlDocument>();

    public IList<YamlDocument> Documents => this.documents;

    public YamlStream()
    {
    }

    public YamlStream(params YamlDocument[] documents)
      : this((IEnumerable<YamlDocument>) documents)
    {
    }

    public YamlStream(IEnumerable<YamlDocument> documents)
    {
      foreach (YamlDocument document in documents)
        this.documents.Add(document);
    }

    public void Add(YamlDocument document) => this.documents.Add(document);

    public void Load(TextReader input) => this.Load((IParser) new Parser(input));

    public void Load(IParser parser)
    {
      this.documents.Clear();
      parser.Expect<StreamStart>();
      while (!parser.Accept<StreamEnd>())
        this.documents.Add(new YamlDocument(parser));
      parser.Expect<StreamEnd>();
    }

    public void Save(TextWriter output) => this.Save(output, true);

    public void Save(TextWriter output, bool assignAnchors)
    {
      IEmitter emitter = (IEmitter) new Emitter(output);
      emitter.Emit((ParsingEvent) new StreamStart());
      foreach (YamlDocument document in (IEnumerable<YamlDocument>) this.documents)
        document.Save(emitter, assignAnchors);
      emitter.Emit((ParsingEvent) new StreamEnd());
    }

    public void Accept(IYamlVisitor visitor) => visitor.Visit(this);

    public IEnumerator<YamlDocument> GetEnumerator() => this.documents.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
