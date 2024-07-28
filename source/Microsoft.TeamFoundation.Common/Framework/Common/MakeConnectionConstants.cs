// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.MakeConnectionConstants
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class MakeConnectionConstants
  {
    public const string Action = "http://docs.oasis-open.org/ws-rx/wsmc/200702/MakeConnection";
    public const string Address = "Address";
    public const string Name = "MakeConnection";
    public const string Namespace = "http://docs.oasis-open.org/ws-rx/wsmc/200702";
    public const string NamespacePrefix = "wsmc";
    public const string MessagePendingHeader = "MessagePending";
    public const string MessagePendingAttribute = "pending";
  }
}
