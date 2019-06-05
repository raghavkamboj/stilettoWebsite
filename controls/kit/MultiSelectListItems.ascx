﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MultiSelectListItems.ascx.cs" Inherits="AspDotNetStorefront.Kit.MultiSelectListItems" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<%@ Import Namespace="AspDotNetStorefrontCore" %>

 <asp:Repeater ID="rptItems" runat="server" DataSource='<%# KitGroup.SelectableItems %>' onitemdatabound="rptItems_ItemDataBound" >
         
    <ItemTemplate>
        <asp:HiddenField ID="hdfKitItemId" runat="server" Value='<%# Container.DataItemAs<KitItemData>().Id %>' />
            
        <asp:CheckBox 
            ID="chkItem"
            runat="server"             
            AutoPostBack="true"
            GroupName="Hello"
            Checked='<%# Container.DataItemAs<KitItemData>().IsSelected %>'
            Enabled='<%# Container.DataItemAs<KitItemData>().CanBeSelected %>' />
        
        <asp:Label
			ID="lblItemName"
			runat="server"
            CssClass='<%# this.CssClassIf(Container.DataItemAs<KitItemData>().HasImage, "kit_groupItem_nameLabel") %>' 
            Text='<%# KitItemDisplayText(Container.DataItemAs<KitItemData>()) %>'></asp:Label>
            
        <asp:Literal ID="litStockHint" runat="server" Text='<%# StockHint(Container.DataItemAs<KitItemData>()) %>' />
          
        <asp:Panel ID="pnlCollapsible" runat="server">
            <asp:Panel ID="pnlKitItemImage" runat="server" Visible='<%# Container.DataItemAs<KitItemData>().HasImage %>' >        
                <asp:Image ID="imgKitItem" runat="server" CssClass="kit_groupitem_image" ImageUrl='<%# Container.DataItemAs<KitItemData>().ImagePath %>' />            
            </asp:Panel>
            <asp:Panel ID="pnlKitItemDescription" runat="server" Visible='<%# Container.DataItemAs<KitItemData>().HasDescription %>'>
                <asp:Literal ID="ltKitItemDescription" runat="server" Text='<%# Container.DataItemAs<KitItemData>().Description %>' />
            </asp:Panel>
        </asp:Panel>
        
        <ajax:CollapsiblePanelExtender 
			ID="extCollapseImage" 
			runat="server" 
            TargetControlID="pnlCollapsible" 
            CollapseControlID="lblItemName" 
            ExpandControlID="lblItemName"
            Collapsed="true" />
        
        <br />
    </ItemTemplate>
    
</asp:Repeater>
