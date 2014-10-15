using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Net;
namespace Fonade
{
    public class Global : System.Web.HttpApplication
    {
         HttpApplication myApplication = new HttpApplication();
   
          
        
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
           
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

     
        }

        void Application_Error(object sender, EventArgs e)
        {


            
        
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
          
        }

        void Session_End(object sender, EventArgs e)
        {

            //myApplication.Response.TrySkipIisCustomErrors = true;
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }
    
}

 
}






    


