// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.AllEntityType
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
  public class AllEntityType : EntityType
  {
    [StaticSafe]
    private static AllEntityType s_allEntityTypeInstance;

    [DataMember(Order = 0)]
    public override string Name
    {
      get => "All";
      set
      {
      }
    }

    [DataMember(Order = 1)]
    public override int ID
    {
      get => 0;
      set
      {
      }
    }

    protected AllEntityType()
    {
    }

    public static AllEntityType GetInstance()
    {
      if (AllEntityType.s_allEntityTypeInstance == null)
        AllEntityType.s_allEntityTypeInstance = new AllEntityType();
      return AllEntityType.s_allEntityTypeInstance;
    }
  }
}
