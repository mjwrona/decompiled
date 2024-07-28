// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.WikiEntityType
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
  public class WikiEntityType : EntityType
  {
    [StaticSafe]
    private static WikiEntityType s_wikiEntityTypeInstance;

    [DataMember(Order = 0)]
    public override string Name
    {
      get => "Wiki";
      set
      {
      }
    }

    [DataMember(Order = 1)]
    public override int ID
    {
      get => 6;
      set
      {
      }
    }

    protected WikiEntityType()
    {
    }

    public static WikiEntityType GetInstance()
    {
      if (WikiEntityType.s_wikiEntityTypeInstance == null)
        WikiEntityType.s_wikiEntityTypeInstance = new WikiEntityType();
      return WikiEntityType.s_wikiEntityTypeInstance;
    }
  }
}
