// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryEntityIdentifierV1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.VisualStudio.Services.Directories
{
  internal class DirectoryEntityIdentifierV1 : DirectoryEntityIdentifier
  {
    private readonly string source;
    private readonly string type;
    private readonly string id;

    internal DirectoryEntityIdentifierV1(string source, string type, string id)
      : base(1)
    {
      this.source = source;
      this.type = type;
      this.id = id;
    }

    internal string Source => this.source;

    internal string Type => this.type;

    internal string Id => this.id;

    internal override string Encode() => string.Format("vss.ds.v{0}.{1}.{2}.{3}", (object) this.version, (object) this.source, (object) this.type, (object) this.id);
  }
}
