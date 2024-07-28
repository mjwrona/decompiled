// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmPathExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
  public class EdmPathExpression : EdmElement, IEdmPathExpression, IEdmExpression, IEdmElement
  {
    private IEnumerable<string> pathSegments;
    private string path;

    public EdmPathExpression(string path)
    {
      EdmUtil.CheckArgumentNull<string>(path, nameof (path));
      this.path = path;
    }

    public EdmPathExpression(params string[] pathSegments)
      : this((IEnumerable<string>) pathSegments)
    {
    }

    public EdmPathExpression(IEnumerable<string> pathSegments)
    {
      EdmUtil.CheckArgumentNull<IEnumerable<string>>(pathSegments, nameof (pathSegments));
      this.pathSegments = !pathSegments.Any<string>((Func<string, bool>) (segment => segment.Contains("/"))) ? (IEnumerable<string>) pathSegments.ToList<string>() : throw new ArgumentException(Strings.PathSegmentMustNotContainSlash);
    }

    public IEnumerable<string> PathSegments
    {
      get
      {
        IEnumerable<string> pathSegments = this.pathSegments;
        if (pathSegments != null)
          return pathSegments;
        return this.pathSegments = (IEnumerable<string>) this.path.Split('/');
      }
    }

    public string Path => this.path ?? (this.path = string.Join("/", this.pathSegments.ToArray<string>()));

    public virtual EdmExpressionKind ExpressionKind => EdmExpressionKind.Path;
  }
}
