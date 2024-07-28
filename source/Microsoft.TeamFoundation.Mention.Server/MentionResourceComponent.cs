// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.MentionResourceComponent
// Assembly: Microsoft.TeamFoundation.Mention.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C680EDB7-9FDC-4722-A198-4B5BA1B43B52
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Mention.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Mention.Server
{
  public class MentionResourceComponent : TeamFoundationSqlResourceComponent
  {
    public virtual void SaveMentions(object requestContext, object mentions) => throw new NotImplementedException();

    public virtual void RetrieveMentions(object requestContect, object uri) => throw new NotImplementedException();
  }
}
