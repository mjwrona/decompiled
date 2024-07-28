// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.Events.IParsingEventVisitor
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

namespace YamlDotNet.Core.Events
{
  public interface IParsingEventVisitor
  {
    void Visit(AnchorAlias e);

    void Visit(StreamStart e);

    void Visit(StreamEnd e);

    void Visit(DocumentStart e);

    void Visit(DocumentEnd e);

    void Visit(Scalar e);

    void Visit(SequenceStart e);

    void Visit(SequenceEnd e);

    void Visit(MappingStart e);

    void Visit(MappingEnd e);

    void Visit(Comment e);
  }
}
