// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating.AvatarService
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.Users.Server;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating
{
  internal class AvatarService : 
    VssMemoryCacheService<Guid, string>,
    IAvatarService,
    IVssFrameworkService
  {
    private static readonly TimeSpan s_maxCacheLife = TimeSpan.FromHours(12.0);
    private static readonly TimeSpan s_maxCacheInactivityAge = TimeSpan.FromHours(1.0);
    private static readonly TimeSpan s_cleanupInterval = TimeSpan.FromMinutes(5.0);
    public static readonly string c_defaultContentType = "image/png";
    public static readonly string c_defaultEncodedAvatar = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACwAAAAsCAYAAAAehFoBAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuNvyMY98AAAJRSURBVFhH7Ze7qxpBFIfzz/osfGCjtYVaqGAjghba+CcI2lgpiBY2YhJIjGTvVbO5FyN7972bMzCBi/eMzu7s5sX+4CuEnXM+h3HO+i4Wi7n/EpFw2ETCYROYcCKRcJrN5tfJZHKczWY2YI5GI6lSqRzi8biDrfFDIMLFYvFlu90+GoZhOY5juzQ2RNd1Yz6ff8nlcjq21ivCwrVa7RnyjToys9/vj6VS6QWr4QUhYdg1bbfbPVGnm4GddxaLhZRMJm2sFi9Cwt1uVwYPkzrdDTkh1WpVxmrxIiQsSdJH6sKd9Xr9HqvFi5Dw+XxWqAd34AgpWC1ehIQty6Ia/JFlGa3Fi5Cwqqp3b4frwDE6YbV4ERJeLpcfqAd3YJj8uTNcLpc1GAwqdbkbRVF+5PN5E6vFi5BwKpWyptPpw+vpxoppmvpgMDiJjmkhYUI6nbZXq9UnGMvofQxfxtU0zRiPx59haAi/UwgLE0DE6nQ6581m8why32FAkMFmwY/yCOf81Gg0nrB1fghE+BdwRMxsNmsVCgUXcDKZjEm+DPasXwIV/h3838KswHWl1Ov1Z+wGIEei1+vJMBU1+vibXK+5RSDCJJfLRYUf1+F6Tb/fP5Bbgj6G5nrNLQITJoEhorVaLZnsNNlZIgs3xk1ZEqwXi0CFSWCntXa7/TAcDo+ws1zvylgvFoELk8BOm+T/Hf14N1gvFqEIew3Wi0Uk7CdYLxaRsJ9gvVhEwn6C9WIRCfsJ1otFJOwnWC8WnoThvTYUsF4sPAn/DUTCYRMJh0vM/QnsePNT7GbEdwAAAABJRU5ErkJggg==";

    public string GetEncodedAvatar(IVssRequestContext requestContext, Guid userId)
    {
      string encodedAvatar = (string) null;
      if (!this.TryGetValue(requestContext, userId, out encodedAvatar))
      {
        try
        {
          Avatar avatar = requestContext.GetService<IUserService>().GetAvatar(requestContext, userId);
          if (avatar != null)
          {
            encodedAvatar = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "data:{0};base64,{1}", (object) AvatarService.c_defaultContentType, (object) Convert.ToBase64String(avatar.Image));
            this.Set(requestContext, userId, encodedAvatar);
          }
        }
        catch (Exception ex)
        {
        }
      }
      if (encodedAvatar == null)
      {
        encodedAvatar = AvatarService.c_defaultEncodedAvatar;
        this.Set(requestContext, userId, encodedAvatar);
      }
      return encodedAvatar;
    }
  }
}
