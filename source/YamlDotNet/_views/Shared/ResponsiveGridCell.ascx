<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Microsoft.TeamFoundation.Server.WebAccess.Models.ResponsiveGridCellModel>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<div class="grid-view-item grid-item-hidden"
     data-grid-cell-section="<%=Model.SectionNumber%>"
     data-grid-cell-columns="<%=Model.Columns%>"
     data-grid-cell-rows="<%=Model.Rows%>" 
     data-grid-cell-adjustHeight="<%=Model.AdjustHeight%>">
    <%if (Model.ControlString != null)
      {%>
        <%=Model.ControlString %>  
      <%}
      else
      {%>
        <% Html.RenderPartial(Model.Area, Model.ViewName, Model.Model); %>   
      <%} %>
</div>