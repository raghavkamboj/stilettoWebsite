<%@ Page language="c#" Inherits="AspDotNetStorefront.manufacturers" CodeFile="manufacturers.aspx.cs" EnableEventValidation="false" MasterPageFile="~/App_Templates/Skin_1/template.master" %>
<%@ Register TagPrefix="aspdnsf" TagName="XmlPackage" Src="~/Controls/XmlPackageControl.ascx" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="PageContent">
    <asp:Panel ID="pnlContent" runat="server" >
        <aspdnsf:XmlPackage id="Package1" PackageName="entity.manufacturers.xml.config" runat="server" EnforceDisclaimer="true" EnforcePassword="true" EnforceSubscription="true" AllowSEPropogation="true" RuntimeParams="entity=Manufacturer"/>
    </asp:Panel>
</asp:Content>




