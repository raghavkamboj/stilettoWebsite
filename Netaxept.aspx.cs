// --------------------------------------------------------------------------------
// Copyright AspDotNetStorefront.com. All Rights Reserved.
// http://www.aspdotnetstorefront.com
// For details on this license please visit the product homepage at the URL above.
// THE ABOVE NOTICE MUST REMAIN INTACT. 
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AspDotNetStorefrontCore;
using AspDotNetStorefrontGateways.Processors.NetaxeptAPI;
using System.Web.Services.Protocols;
using AspDotNetStorefrontGateways;

namespace AspDotNetStorefront
{
    /// <summary>
    /// Summary descripption for Netaxept.
    /// This page will process the transaction string that they have provided and redirect to the checkoutreview page
    /// </summary>
    public partial class Netaxept : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.CacheControl = "private";
            Response.Expires = 0;
            Response.AddHeader("pragma", "no-cache");

            // This is for one page checkout
			var checkoutController = CheckOutPageControllerFactory.CreateCheckOutPageController();
			bool UseOnePageCheckout = checkoutController.GetCheckoutType() == CheckOutType.BasicOPC; // SmartOPC doesn't support Netaxept
			string opcCheckoutPage = checkoutController.GetBasicOnePageCheckoutPage();

            bool UseFinalReviewOrderPage = AppLogic.AppConfigBool("Checkout.UseOnePageCheckout.UseFinalReviewOrderPage");

            ErrorMessage err;

            // We redirect the customer if he doesnt have customer record
            // this add a security, i8f try to access this page in the url
            if (!Customer.Current.HasCustomerRecord)
            {
                Response.Redirect("default.aspx");
            }
            
            // check if has customer record
            bool errorOccur = false;
            bool cancelButtonWasPressed = false;

            
            // The transaction string they provided.
            string bbsEpayTransactionString = CommonLogic.QueryStringCanBeDangerousContent("BBSePay_transaction");

            // Merchant account.
            string token = AppLogic.AppConfig("NETAXEPT.Merchant_Token");
            string merchantID = AppLogic.AppConfig("NETAXEPT.Merchant_Id");

            TokenService service = new TokenService();

            // The server url.
            string url = string.Empty;
            
            // Determine to go live or test.
            if (AppLogic.AppConfigBool("UseLiveTransactions"))
            {
                url = AppLogic.AppConfig("NETAXEPT.Live_Server"); // use live.
            }
            else
            {
                url = AppLogic.AppConfig("NETAXEPT.Test_Server"); // use test.
            }

            // Set the url.
            service.Url = url;

            string errorResult = string.Empty;

            try 
            {
                Result result;

                // try to process if it fails, it will redirect to the checkoutpayment page
                // this usually occur when you click cancel button on the BBS UI Interface.
                result = service.ProcessSetup(token, merchantID, bbsEpayTransactionString);
                
                // if succesful, add the transaction string to the customer session so we can access it in another page 
                // and redirect to the checkoutreview page.
                Customer.Current.ThisCustomerSession["Nextaxept_TransactionString"] = bbsEpayTransactionString;
            }
            catch(Exception ex)
            {
                SoapException se = ex as SoapException;

                errorResult = AppLogic.GetString("toc.aspx.6", Customer.Current.SkinID, Customer.Current.LocaleSetting) + ex.Message;
                
                if (se != null) // this is just oocur when pressing the cancel button, no need to output
                {
                    if (se.Detail["UserCancelledException"] != null)
                    {
                        errorResult = string.Empty;
                        cancelButtonWasPressed = true;
                    }
                }

                errorOccur = true;
            }

            if (errorOccur)
            {
                
                // We will not display the error causese by cancel button on BBS UI interface
                if (cancelButtonWasPressed == true)
                {
                    if (UseOnePageCheckout)
                    {
						Response.Redirect(opcCheckoutPage);
                    }

                    Response.Redirect("checkoutpayment.aspx");
                
                }
                err = new ErrorMessage(Server.HtmlEncode(errorResult));
                if (UseOnePageCheckout)
                {
                    Response.Redirect(opcCheckoutPage + "?errormsg=" + err.MessageId);
                }

                Response.Redirect("checkoutpayment.aspx?nexaxepterror=" + err.MessageId);
            }
            else
            {
                if (UseOnePageCheckout)
                {
                    if (!UseFinalReviewOrderPage)
                    {
                        string status = string.Empty;
                        int ordnum = 0;
                       
                        try
                        {
                            ShoppingCart cart = new ShoppingCart(Customer.Current.SkinID, Customer.Current, CartTypeEnum.ShoppingCart, 0, false);

                            bool result = int.TryParse(Customer.Current.ThisCustomerSession["Nextaxept_OrderNumber"], out ordnum);

                            status = Gateway.MakeOrder(String.Empty, AppLogic.TransactionMode(), cart, ordnum, String.Empty, String.Empty, String.Empty, String.Empty);

                        }
                        catch (Exception ex)
                        {
                            status = ex.Message;
                            err = new ErrorMessage(Server.HtmlEncode(status));
                            Response.Redirect(opcCheckoutPage + "?errormsg=" + err.MessageId);
                        }

                        if (status != AppLogic.ro_OK)
                        {
                            err = new ErrorMessage(Server.HtmlEncode(status));
							Response.Redirect(opcCheckoutPage + "errormsg=" + err.MessageId);
                        }
                     
                        Response.Redirect("orderconfirmation.aspx?ordernumber=" + ordnum.ToString() + "&paymentmethod=" + Server.UrlEncode(AppLogic.ro_PMCreditCard));
                    }
                }

                Response.Redirect("checkoutreview.aspx?paymentmethod=" + Server.UrlEncode(AppLogic.ro_PMCreditCard));
            }
           
        }
    }
}


