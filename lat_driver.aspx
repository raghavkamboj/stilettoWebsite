<%@ Page language="c#" Inherits="AspDotNetStorefront.lat_driver" CodeFile="lat_driver.aspx.cs" MasterPageFile="~/App_Templates/Skin_1/template.master"%>
<%@ Register TagPrefix="aspdnsf" TagName="Topic" Src="~/Controls/TopicControl.ascx" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="PageContent">
    <asp:Panel ID="pnlContent" runat="server" >
    
        <table id="Table1" runat="server" visible="true" cellspacing="5" cellpadding="5" border="0" width="100%">
            <tr>
                <td valign="top" align="left">
                    <asp:Panel ID="pnlBeforeSignup" runat="server" Visible="true">
                        <table cellspacing="0" cellpadding="1" width="171" border="0">
                            <tr>
                                <td class="AffiliateHeaderTd">&nbsp;<asp:Label ID="AppConfigAffiliateProgramName" runat="server"></asp:Label></td>
                            </tr>

                            <tr valign="middle" align="center">
                                <td class="AffiliateTextCell">
                                    <table cellspacing="0" cellpadding="8" width="100%" border="0">
                                        <tr>
                                            <td valign="top">
                                                <center>
                                                    <a href="lat_signout.aspx"><asp:Image ID="imgLogOut" AlternateText="Affiliate Logout" runat="server" /></a>
                                                </center>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>

                        <p>&nbsp;</p>

                        <table cellspacing="0" cellpadding="1" width="171" border="0" style="">
                            <tr>
                                <td class="AffiliateHeaderTd" height="18">&nbsp;Need Help?</td>
                            </tr>

                            <tr valign="middle" align="center">
                                <td class="AffiliateTextCell">
                                    <table cellspacing="0" cellpadding="4" width="100%" border="0">
                                        <tr>
                                            <td valign="top" align="left" width="100%" class="AffiliateLinks">
                                                 &bull; <a href="lat_account.aspx">Account Home</a><br />
                                                 &bull; <a href="lat_driver.aspx?topic=affiliate_linking">Web Linking Instructions</a><br/>
                                                 &bull; <a href="lat_driver.aspx?topic=affiliate_faq">FAQs</a><br />
                                                 &bull; <asp:HyperLink ID="AskAQuestion" runat="server" Text="Ask A Question"></asp:HyperLink><br />
                                                 &bull; <a href="lat_driver.aspx?topic=affiliate_terms">Terms &amp; Conditions</a>
                                             </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>


                    <asp:Panel ID="pnlAfterSignup" runat="server" Visible="false">
                        <table cellSpacing="0" cellPadding="1" width="171" bgColor="#AAAAAA" border="0">
                            <tr>
                              <td bgcolor="#AAAAAA" height="18"><b style="color: #ffffff" class="small">&nbsp;Program Links</b></td>
                            </tr>
                            <tr valign="middle" align="center">
                                <td>
                                    <table cellSpacing="0" cellPadding="4" width="100%" bgColor="#AAAAAA" border="0">
                                        <tr>
                                            <td vAlign="top" align="left" width="100%" bgColor="#ffffff">
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>

                    <br/><br/>
                    <asp:Label ID="lblErrorMsg" runat="server" Font-Bold="true" ForeColor="red" ></asp:Label>
                    
                </td>


                <td width="100%" align="left" valign="top">
                    <table cellpadding="0" cellspacing="0" align="center" width="400">
                        <tr><td><asp:Image id="affiliateheader_small_gif" runat="server" /><br /><br /></td></tr>
                        <tr>
                            <td>
                                <asp:Literal ID="PageTopic" runat="server" Mode="PassThrough"></asp:Literal>
                            </td>
                        </tr>
                    </table>

                </td>

            </tr>
        </table>
        
    </asp:Panel>
</asp:Content>



