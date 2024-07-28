// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.BoardEntityType
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities
{
  [DataContract]
  [Export(typeof (IEntityType))]
  public class BoardEntityType : EntityType
  {
    [StaticSafe]
    private static BoardEntityType s_boardEntityTypeInstance;

    [DataMember(Order = 0)]
    public override string Name
    {
      get => "Board";
      set
      {
      }
    }

    [DataMember(Order = 1)]
    public override int ID
    {
      get => 8;
      set
      {
      }
    }

    protected BoardEntityType()
    {
    }

    public static BoardEntityType GetInstance()
    {
      if (BoardEntityType.s_boardEntityTypeInstance == null)
        BoardEntityType.s_boardEntityTypeInstance = new BoardEntityType();
      return BoardEntityType.s_boardEntityTypeInstance;
    }
  }
}
