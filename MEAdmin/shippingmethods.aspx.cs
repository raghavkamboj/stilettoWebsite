// --------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com. All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// --------------------------------------------------------------------------------
using System;
using System.Data;
using System.Globalization;
using System.Text;
using AspDotNetStorefrontCore;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AspDotNetStorefrontAdmin
{
    /// <summary>
    /// Summary description for ShippingMethods.
    /// </summary>
    public partial class ShippingMethods : AdminPageBase
    {
        bool IsShippingMethod = false;

        private List<Store> m_stores;
        /// <summary>
        /// Gets or sets the available stores
        /// </summary>
        public List<Store> Stores
        {
            get { return m_stores; }
            set { m_stores = value; }
        }

        private bool m_multistorefilteringenabled;
        /// <summary>
        /// Gets or sets whether multi-store filtering is enabled
        /// </summary>
        public bool MultiStoreFilteringEnabled
        {
            get { return m_multistorefilteringenabled; }
            set { m_multistorefilteringenabled = value; }
        }

        /// <summary>
        /// Initializes the stores collection and determines the default store filter
        /// </summary>
        private void InitializeStores()
        {
            Stores = Store.GetStoreList();

            if (!IsPostBack || Stores.Count == 1)
            {
                int qStoreId = Request.QueryStringNativeInt("StoreId");
                if (Stores.Any(store => store.StoreID == qStoreId))
                {
                    StoreFilter = qStoreId;
                }
                else
                {
                    // default to All
                    //StoreFilter = Shipping.DONT_FILTER_PER_STORE;
                    var defStore = Stores.FirstOrDefault(store => store.IsDefault);
                    StoreFilter = defStore.StoreID;
                }
            }
            else
            {
                StoreFilter = Request.Form["StoreFilter"].ToNativeInt();
            }
        }

        /// <summary>
        /// Gets whether the postback was caused by the stores dropdown
        /// </summary>
        /// <returns></returns>
        private bool IsStoreFilterChangePostBack()
        {
            return IsPostBack && Request["__EVENTTARGET"].EqualsIgnoreCase("cboStores");
        }

        private int m_storefilter;
        /// <summary>
        /// Gets or sets the store id for filtering
        /// </summary>
        public int StoreFilter
        {
            get { return m_storefilter; }
            set { m_storefilter = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            DetermineMultiStoreShippingFiltering();
            InitializeStores();            
            base.OnInit(e);
        }

        private void DetermineMultiStoreShippingFiltering()
        {
            MultiStoreFilteringEnabled = AppLogic.GlobalConfigBool("AllowShippingFiltering");
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            SectionTitle = "Manage Shipping Methods";
            // if this is a postback and the one triggered the postback
            // is the store combobox
            if (IsStoreFilterChangePostBack())
            {
                StoreFilter = Request.Form["cboStores"].ToNativeInt();
            }
            else
            {
                if (CommonLogic.FormBool("IsSubmit"))
                {
                    for (int i = 0; i <= Request.Form.Count - 1; i++)
                    {
                        if (Request.Form.Keys[i].IndexOf("DisplayOrder_") != -1)
                        {
                            String[] keys = Request.Form.Keys[i].Split('_');
                            int ShippingMethodID = Localization.ParseUSInt(keys[1]);
                            int DispOrd = 1;
                            try
                            {
                                DispOrd = Localization.ParseUSInt(Request.Form[Request.Form.Keys[i]]);
                            }
                            catch { }
                            DB.ExecuteSQL("update ShippingMethod set DisplayOrder=" + DispOrd.ToString() + " where ShippingMethodID=" + ShippingMethodID.ToString());
                            IsShippingMethod = true;
                        }
                    }

                    // for the store mapping
                    if (Stores.Count > 1 && StoreFilter != Shipping.DONT_FILTER_PER_STORE)
                    {
                        DB.ExecuteSQL(string.Format("DELETE ShippingMethodStore WHERE StoreId = {0}", StoreFilter));
                        var chkStoreMapElementNames = Request.Form.AllKeys.Where(elem => elem.StartsWith("chkStoreMap_"));
                        foreach (string chkMap in chkStoreMapElementNames)
                        {
                            int shippingMethodId = chkMap.Split('_')[1].ToNativeInt();
                            DB.ExecuteSQL(string.Format("INSERT INTO ShippingMethodStore(StoreId, ShippingMethodId) Values({0}, {1})", StoreFilter, shippingMethodId));
                        }
                    }
                }
            }
            Render();
        }

        private void RenderBulkMapOtions(StringBuilder html)
        {
            //html.Append("<div style=\"padding-top:5px;padding-bottom:5px;padding-left:10px;\">");
            //html.Append("Select : ");
            //html.Append("<a href=\"javascript:void(0);\" onclick=\"javascript:selectMap(true);\">All</a>");
            //html.Append("&nbsp;<a href=\"javascript:void(0);\" onclick=\"javascript:selectMap(false);\">, None</a>");
            //html.Append("</div>");
        }

        private void Render()
        {
            if (CommonLogic.QueryStringCanBeDangerousContent("DeleteID").Length != 0)
            {
                // delete the method:
                int DeleteID = CommonLogic.QueryStringUSInt("DeleteID");
                DB.ExecuteSQL("delete from ShippingByTotal where ShippingMethodID=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ShippingByWeight where ShippingMethodID=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ShippingWeightByZone where ShippingMethodID=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ShippingTotalByZone where ShippingMethodID=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ShippingMethod where ShippingMethodID=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ShippingMethodToStateMap where ShippingMethodID=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ShippingMethodToCountryMap where ShippingMethodID=" + DeleteID.ToString());
                DB.ExecuteSQL("delete from ShippingMethodToZoneMap where ShippingMethodID=" + DeleteID.ToString());
                if (Stores.Count > 1)
                {
                    DB.ExecuteSQL("delete from ShippingMethodStore where ShippingMethodID=" + DeleteID.ToString());
                }
                DB.ExecuteSQL("update shoppingcart set ShippingMethodID=0, ShippingMethod=NULL where ShippingMethodID=" + DeleteID.ToString());
            }

            StringBuilder html = new StringBuilder();

            if (IsShippingMethod && !IsStoreFilterChangePostBack())
            {
                html.Append("<b>NOTICE:</b>&nbsp;&nbsp;&nbsp;Item updated");
                html.Append("<br />");
            }

            html.Append("<form method=\"POST\" id=\"frmShippingMethod\" name=\"frmShippingMethod\" action=\"" + AppLogic.AdminLinkUrl("shippingmethods.aspx") + "\">\n");

            if (Stores.Count > 1)
            {
                html.AppendFormat("<br/>");
                html.AppendFormat("<input type=\"hidden\" name=\"StoreFilter\" id=\"StoreFilter\" value=\"{0}\" />\n", StoreFilter);

                html.AppendLine();

                var aspnetPostbackScript = @"<script type=""text/javascript"">
                                            //<![CDATA[
                                            var theForm = document.forms[""aspnetForm""];
                                            function __doPostBack(eventTarget, eventArgument) {
                                                if (!theForm.onsubmit || (theForm.onsubmit() != false)) {
                                                    theForm.__EVENTTARGET.value = eventTarget;
                                                    theForm.__EVENTARGUMENT.value = eventArgument;
                                                    theForm.submit();
                                               }
                                            }
                                            //]]>
                                            </script>";

                html.Append(aspnetPostbackScript);

                html.AppendLine();
                html.AppendFormat("Store: <select id=\"cboStores\" name=\"cboStores\" onchange=\"javascript:setTimeout('__doPostBack(\\'cboStores\\',\\'\\')', 0)\" >\n");

                // add default All Option
                //html.AppendFormat("    <option value=\"{0}\" >All</option>\n", Shipping.DONT_FILTER_PER_STORE);

                foreach (var store in Stores)
                {
                    bool shouldBeSelected = false;
                    shouldBeSelected = (store.StoreID == StoreFilter);
                    html.AppendFormat("    <option value=\"{0}\" {2}>{1}</option>\n", store.StoreID, store.Name, shouldBeSelected ? "selected=\"selected\"" : string.Empty);
                }
                html.AppendFormat("</select>\n");

                html.AppendFormat("&nbsp;<span>Multi-Store Filtering: {0}</span><br/><br/>", MultiStoreFilteringEnabled ? "On" : "Off");

                html.AppendLine();
            }



            html.Append("<input class=\"normalButtons\" type=\"button\" value=\"Add New Shipping Method\" name=\"AddNew\" onClick=\"self.location='" + AppLogic.AdminLinkUrl("editShippingMethod.aspx") + "?storeid=" + StoreFilter.ToString() + "';\">\n");
            html.Append("&nbsp;<a href=\"" + AppLogic.AdminLinkUrl("shipping.aspx") + "?storeid=" + StoreFilter.ToString() + "\">Configure rates</a>\n");
            html.Append("<br/>\n");

            html.Append("<input type=\"hidden\" name=\"IsSubmit\" value=\"true\">\n");
            html.Append("<br>\n");

            if (Stores.Count > 1 && StoreFilter != Shipping.DONT_FILTER_PER_STORE)
            {
                RenderBulkMapOtions(html);
            }

            html.Append("<table border=\"0\" cellpadding=\"0\" border=\"0\" cellspacing=\"0\" width=\"100%\">\n");
            html.Append("<tr class=\"table-header\">\n");
            html.Append("<td width=\"5%\" align=\"left\" valign=\"middle\">ID</td>\n");
            if (Stores.Count > 1 && StoreFilter != Shipping.DONT_FILTER_PER_STORE)
            {
                html.Append("<td width=\"5%\" align=\"left\" valign=\"middle\">Map</td>\n");
            }
            html.Append("<td align=\"left\" valign=\"middle\">Method</td>\n");
            int ColSpan = 4;
            if (AppLogic.AppConfigBool("ShipRush.Enabled"))
            {
                ColSpan++;
                html.Append("<td align=\"left\" valign=\"middle\">ShipRush Template</td>\n");
            }
            html.Append("<td align=\"left\" valign=\"middle\">Display Order</td>\n");
            html.Append("<td align=\"left\" valign=\"middle\">Edit</td>\n");
            html.Append("<td align=\"left\" valign=\"middle\">Allowed States</td>\n");
            html.Append("<td align=\"left\" valign=\"middle\">Allowed Countries</td>\n");

            if (!AppLogic.ProductIsMLExpress() && !AppLogic.ProductIsMLX()) //not supported in Incartia and express
            {
                html.Append("<td align=\"center\"><b>Allowed Zones</b></td>\n");
            }

            if (AppLogic.AppConfigBool("UseMappingShipToPayment"))
            {
                ColSpan++;
                html.Append("<td align=\"left\" valign=\"middle\">Allowed Payment Methods</td>\n");
            }
            html.Append("<td align=\"left\" valign=\"middle\">Delete</td>\n");
            html.Append("</tr>\n");            
            string dtShipping = string.Format("exec aspdnsf_GetStoreShippingMethodMapping @StoreId={0}", StoreFilter);
            using (SqlConnection con = new SqlConnection(DB.GetDBConn()))
            {
                con.Open();
                using (IDataReader rs = DB.GetRS(dtShipping, con))
                {
                    int i = 0;

                    while (rs.Read())
                    {

                        int ThisID = DB.RSFieldInt(rs, "ShippingMethodID");

                        if (i % 2 == 0)
                        {
                            html.Append("<tr class=\"table-row2\">\n");
                        }
                        else
                        {
                            html.Append("<tr class=\"table-alternatingrow2\">\n");
                        }
                        html.Append("<td width=\"5%\"  align=\"left\" valign=\"middle\">" + ThisID.ToString() + "</td>\n");
                        if (Stores.Count > 1 && StoreFilter != Shipping.DONT_FILTER_PER_STORE)
                        {
                            html.AppendFormat("<td align=\"left\" valign=\"middle\"><input type=\"checkbox\" name=\"chkStoreMap_{0}\" class=\"storeMap\" value=\"{1}\" {2}></td>\n", ThisID, rs.FieldByLocale("Name", LocaleSetting), rs.FieldBool("Mapped") ? "checked" : string.Empty);
                        }
                        html.Append("<td align=\"left\" valign=\"middle\"><a href=\"" + AppLogic.AdminLinkUrl("editShippingMethod.aspx") + "?ShippingMethodID=" + ThisID.ToString() + "&StoreId=" + StoreFilter.ToString() + "\">" + DB.RSFieldByLocale(rs, "Name", LocaleSetting) + "</a></td>\n");
                        html.Append("<td align=\"left\" valign=\"middle\"><input size=\"2\" type=\"text\" name=\"DisplayOrder_" + ThisID.ToString() + "\" value=\"" + DB.RSFieldInt(rs, "DisplayOrder").ToString() + "\"></td>\n");
                        if (AppLogic.AppConfigBool("ShipRush.Enabled"))
                        {
                            html.Append("<td align=\"left\">" + DB.RSField(rs, "ShipRushTemplate") + "</td>\n");
                        }
                        html.Append("<td align=\"left\" valign=\"middle\"><input class=\"normalButtons\" type=\"button\" value=\"Edit\" name=\"Edit_" + ThisID.ToString() + "\" onClick=\"self.location='" + AppLogic.AdminLinkUrl("editShippingMethod.aspx") + "?ShippingMethodID=" + ThisID.ToString() + "&storeid=" + StoreFilter.ToString() + "';\" ></td>\n");
                        html.Append("<td align=\"left\" valign=\"middle\"><input class=\"normalButtons\" type=\"button\" value=\"Set Allowed States\" name=\"SetStates_" + ThisID.ToString() + "\" onClick=\"self.location='" + AppLogic.AdminLinkUrl("ShippingMethodStates.aspx") + "?ShippingMethodID=" + ThisID.ToString() + "'\"></td>\n");
                        html.Append("<td align=\"left\" valign=\"middle\"><input class=\"normalButtons\" type=\"button\" value=\"Set Allowed Countries\" name=\"SetCountries_" + ThisID.ToString() + "\" onClick=\"self.location='" + AppLogic.AdminLinkUrl("ShippingMethodCountries.aspx") + "?ShippingMethodID=" + ThisID.ToString() + "'\"></td>\n");

                        if (!AppLogic.ProductIsMLExpress() && !AppLogic.ProductIsMLX()) //not supported in Incartia and express
                        {
                            html.Append("<td align=\"center\"><input class=\"normalButtons\" type=\"button\" value=\"Set Allowed Zones\" name=\"SetZones_" + ThisID.ToString() + "\" onClick=\"self.location='" + AppLogic.AdminLinkUrl("ShippingMethodZones.aspx") + "?ShippingMethodID=" + ThisID.ToString() + "'\"></td>\n");
                        }

                        if (AppLogic.AppConfigBool("UseMappingShipToPayment"))
                        {
                            html.Append("<td align=\"left\" valign=\"middle\"><input class=\"normalButtons\" type=\"button\" value=\"Set Allowed Payment Methods\" name=\"SetPaymentMethods_" + ThisID.ToString() + "\" onClick=\"self.location='" + AppLogic.AdminLinkUrl("MapShippingMethodToPaymentMethod.aspx") + "?ShippingMethodID=" + ThisID.ToString() + "'\"></td>\n");
                        }
                        html.Append("<td align=\"left\" valign=\"middle\"><input class=\"normalButtons\" type=\"button\" value=\"Delete\" name=\"Delete_" + ThisID.ToString() + "\" onClick=\"DeleteShippingMethod(" + ThisID.ToString() + ")\"></td>\n");
                        html.Append("</tr>\n");

                        i++;

                    }
                }
            }

            html.Append("<tr>\n");

            int colspanOffset = 2;
            if (AppLogic.AppConfigBool("ShipRush.Enabled"))
            {
                colspanOffset = 3;
            }
            else
            {
                colspanOffset = 2;
            }

            if (Stores.Count > 1 && StoreFilter != Shipping.DONT_FILTER_PER_STORE)
            {
                colspanOffset += 1;
            }

            html.Append("<td align=\"left\" valign=\"top\" colspan=\"" + colspanOffset.ToString() + "\" >");

            if (Stores.Count > 1 && StoreFilter != Shipping.DONT_FILTER_PER_STORE)
            {
                RenderBulkMapOtions(html);
            }

            html.Append("</td>\n");
            html.Append("<td align=\"left\" valign=\"middle\" height=\"25px\"><input class=\"normalButtons\" type=\"submit\" value=\"Update\" name=\"Submit\"></td>\n");
            html.Append("<td align=\"left\" valign=\"middle\" colspan=\"" + ColSpan.ToString() + "\"></td>\n");
            html.Append("</tr>\n");
            html.Append("</table>\n");            
            html.Append("<input class=\"normalButtons\" type=\"button\" value=\"Add New Shipping Method\" name=\"AddNew\" onClick=\"self.location='" + AppLogic.AdminLinkUrl("editShippingMethod.aspx") + "?storeid=" + StoreFilter.ToString() + "';\">\n");
            html.Append("&nbsp;<a href=\"" + AppLogic.AdminLinkUrl("shipping.aspx") + "?storeid=" + StoreFilter.ToString() + "\">Configure rates</a>\n");
            html.Append("</form>\n");

            // ---------------------------------------------------------
            // REAL TIME RATES ADDED AUTOMATICALLY BY STOREFRONT:
            // ---------------------------------------------------------
            html.Append("<hr size=1>");
            html.Append("<p>The following Real Time Shipping Methods have been added automatically by the storefront, based on the rates returned for various customers. They should also be automatically mapped to allowed states & countries. You should only ever need to delete these shipping methods (and that should not be very often).<br/><br/>How were these mapped to states & countries? We assume that the carriers only return rates valid for the customer who requested them, so we analyzed that and just add the rate to the state and country that the customer was in when they requested the rates.<br/><br/>NOTE: It should be unusually rare to have to delete one of these methods! If you want to exclude rates from being used by customers, set the AppConfig:RTShipping.ShippingMethodsToPrevent parameter!</p>");

            html.Append("  <table border=\"0\" cellpadding=\"0\" border=\"0\" cellspacing=\"0\" width=\"100%\">\n");
            html.Append("    <tr class=\"table-header\">\n");
            html.Append("      <td width=\"5%\" align=\"left\" valign=\"middle\">ID</td>\n");
            html.Append("      <td align=\"left\" valign=\"middle\">Method</td>\n");
            if (AppLogic.AppConfigBool("ShipRush.Enabled"))
            {
                html.Append("      <td align=\"left\" valign=\"middle\">ShipRush Template</td>\n");
                html.Append("      <td align=\"left\" valign=\"middle\">Edit</td>\n");
            }
            html.Append("      <td align=\"left\" valign=\"middle\">Delete</td>\n");
            html.Append("    </tr>\n");            
            string sqlRTShipping = string.Format("exec aspdnsf_GetStoreShippingMethodMapping @StoreId={0}, @IsRTShipping = 1", StoreFilter);
            using (SqlConnection con = new SqlConnection(DB.GetDBConn()))
            {
                con.Open();
                using (IDataReader rs = DB.GetRS(sqlRTShipping, con))
                {
                    int i = 0;

                    while (rs.Read())
                    {

                        int ThisID = DB.RSFieldInt(rs, "ShippingMethodID");

                        if (i % 2 == 0)
                        {
                            html.Append("    <tr class=\"table-row2\">\n");
                        }
                        else
                        {
                            html.Append("    <tr class=\"table-alternatingrow2\">\n");
                        }
                        html.Append("      <td width=\"5%\"  align=\"left\" valign=\"middle\">" + ThisID.ToString() + "</td>\n");
                        html.Append("      <td align=\"left\" valign=\"middle\">" + DB.RSFieldByLocale(rs, "Name", LocaleSetting) + "</td>\n");
                        if (AppLogic.AppConfigBool("ShipRush.Enabled"))
                        {
                            html.Append("      <td align=\"left\" valign=\"middle\">" + DB.RSField(rs, "ShipRushTemplate") + "</td>\n");
                            html.Append("      <td align=\"left\" valign=\"middle\"><input class=\"class=\"normalButtons\"\" type=\"button\" value=\"Edit\" name=\"Edit_" + ThisID.ToString() + "\" onClick=\"self.location='" + AppLogic.AdminLinkUrl("editShippingMethod.aspx") + "?ShippingMethodID=" + ThisID.ToString() + "'\"></td>\n");
                        }
                        html.Append("      <td align=\"left\" valign=\"middle\"><input class=\"normalButtons\" type=\"button\" value=\"Delete\" name=\"Delete_" + ThisID.ToString() + "\" onClick=\"DeleteShippingMethod(" + ThisID.ToString() + ")\"></td>\n");
                        html.Append("    </tr>\n");

                        i++;

                    }
                }
            }
            html.Append("</table>\n");

            html.Append("<script type=\"text/javascript\">\n");
            html.Append("function DeleteShippingMethod(id)\n");
            html.Append("{\n");
            html.Append("if(confirm('Are you sure you want to delete Shipping Method: ' + id + '. This action cannot be undone!'))\n");
            html.Append("{\n");
            html.Append("self.location = '" + AppLogic.AdminLinkUrl("ShippingMethods.aspx") + "?deleteid=' + id;\n");
            html.Append("}\n");
            html.Append("}\n");
            html.Append("</SCRIPT>\n");
            ltContent.Text = html.ToString();
        }
    }
}
