using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



//crystal reports
using CrystalDecisions.Shared;
using CrystalDecisions.CrystalReports.Engine;


//database
using Microsoft.VisualBasic;
using System.IO;
using System.Data.Odbc;
using System.Data.SqlClient;


namespace vvt
{
    public partial class Form2 : Form
    {

        public Form1 LaunchOrigin { get; set; }

        string jobNumberUser = "";

        public Form2()
        {

            InitializeComponent();

            label3.Text = "";
            label5.Text = "";

        }


        #region mailing ticket
        //mailing ticket
        private void button3_Click_1(object sender, EventArgs e)
        {
            //here is where i will put all Form {button/label/etc} editing
            //Notes: can NOT edit crystal report stuff here; have not created crystal reprort object yet, that will be later
            #region label and buttons UI
            label3.Text = "Please wait report loading..";

            jobNumberUser = textBox1.Text;


            button1.FlatAppearance.BorderSize = 0;
            button1.FlatAppearance.BorderColor = Color.Black;

            button11.FlatAppearance.BorderSize = 0;
            button11.FlatAppearance.BorderColor = Color.Black;

            button8.FlatAppearance.BorderSize = 0;
            button8.FlatAppearance.BorderColor = Color.Black;

            button9.FlatAppearance.BorderSize = 0;
            button9.FlatAppearance.BorderColor = Color.Black;


            button2.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.BorderColor = Color.Black;


            button3.FlatAppearance.BorderSize = 5;
            button3.FlatAppearance.BorderColor = Color.Black;

            //timestamp when a button is clicked (report is ran), so user knows what time the current report on screen was ran
            label5.Text = DateTime.Now.ToString();

            #endregion label/buttons UI end




            //here is where i will put the creation and manipualtion of the crystal report object
            #region start crystal report config:

            #region connection and CR object properties/settings
            //rerport object



            CrystalReport2 cryrpt = new CrystalReport2();

            cryrpt.DataSourceConnections.Clear();  //clear the connections (will popualte with fresh sql query defined data

            //set the databse login info, use twice first one is to login into sql server

            //getting write only error


            cryrpt.SetDatabaseLogon("Bob", "Orchard", "monarch18", "gams1");
            cryrpt.SetDatabaseLogon("Bob", "Orchard"); //this one for that annoying prompt to login into database


            //this does not error out!?
            //this opens connection to DB with the login info down..
            ConnectionInfo crconnectioninfo = new ConnectionInfo();
            crconnectioninfo.ServerName = "monarch18";
            crconnectioninfo.DatabaseName = "gams1";
            crconnectioninfo.UserID = "Bob";
            crconnectioninfo.Password = "Orchard";


            //try our best to never have paramter panel pop-up, works like 95% of time
            //error here as well?
            //LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;

            #endregion connectiona and CR object properties/settings

            #region UI on CR object editing (non-DB dependant)

            //change text object, tell user what ticket the are curentlly running/looking at
            CrystalDecisions.CrystalReports.Engine.TextObject txtObj2;
            txtObj2 = cryrpt.ReportDefinition.ReportObjects["ticketName"] as TextObject;
            txtObj2.Text = "Mailing Ticket";


            #endregion UI on CR object editing (non-DB dependant)

            #endregion crystal report config end




            //here is where i need to clean up lots, make functions, sql queryies, DT and DS manipualtion, pass the job number, etc 
            //main code for program functionallity
            #region DB connection 


            //here is where i iwll be connecting to DB's (all queries will need access to basically my connection properties globals)
            #region global connection properties

            //connection string for DB
            string connectStr = "DSN=Progress11;uid=Bob;pwd=Orchard";

            //open th econnection and error check
            OdbcConnection dbConn = new OdbcConnection(connectStr);

            try
            {
                dbConn.Open();
            }
            catch (Exception ex)
            {

                string error = ex + " : DB error cannot connect";

                ErrorLog(error);

            }
            #endregion global connection propeties




            //here is where i can change the UI of the report based on database data
            //ex) show word nailing on report if job# has a 810 tag associated with it
            #region UI crystal report editing (DB dependant)


            #region Header

            //set the job numbers from iuser input
            CrystalDecisions.CrystalReports.Engine.TextObject jobNum1;
            jobNum1 = cryrpt.ReportDefinition.ReportObjects["jobNum"] as TextObject;
            jobNum1.Text = jobNumberUser;

            CrystalDecisions.CrystalReports.Engine.TextObject jobNum2;
            jobNum2 = cryrpt.ReportDefinition.ReportObjects["jobNum2"] as TextObject;
            jobNum2.Text = jobNumberUser;



            String headerJob = "SELECT \"Job-Desc\", \"Date-Promised\", \"Sales-Rep-ID\", \"CSR-ID\", \"" +
                "PO-Number\", \"Overs-Allowed\", \"Last-Estimate-ID\", \"Quantity-Ordered\", \"Contact-Name\", \"Date-Entered\", \"Cust-ID-Ordered-by\"" +
                " FROM PUB.JOB WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtHeader = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter header = new OdbcDataAdapter(headerJob, dbConn);
                header.Fill(dtHeader);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //set all text objects to the data from datatable

            //job descriptions
            CrystalDecisions.CrystalReports.Engine.TextObject jobDesc;
            jobDesc = cryrpt.ReportDefinition.ReportObjects["jobDesc"] as TextObject;
            jobDesc.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

            CrystalDecisions.CrystalReports.Engine.TextObject jobDesc2;
            jobDesc2 = cryrpt.ReportDefinition.ReportObjects["jobDesc2"] as TextObject;
            jobDesc2.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

            //date promised
            CrystalDecisions.CrystalReports.Engine.TextObject dateProm;
            dateProm = cryrpt.ReportDefinition.ReportObjects["dateProm"] as TextObject;
            dateProm.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

            CrystalDecisions.CrystalReports.Engine.TextObject dateProm2;
            dateProm2 = cryrpt.ReportDefinition.ReportObjects["dateProm2"] as TextObject;
            dateProm2.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

            //qty
            CrystalDecisions.CrystalReports.Engine.TextObject qty;
            qty = cryrpt.ReportDefinition.ReportObjects["qty"] as TextObject;
            qty.Text = dtHeader.Rows[0]["Quantity-Ordered"].ToString();

            CrystalDecisions.CrystalReports.Engine.TextObject qty2;
            qty2 = cryrpt.ReportDefinition.ReportObjects["qty2"] as TextObject;
            qty2.Text = dtHeader.Rows[0]["Quantity-Ordered"].ToString();

            //job contact
            CrystalDecisions.CrystalReports.Engine.TextObject jobContact;
            jobContact = cryrpt.ReportDefinition.ReportObjects["contactName"] as TextObject;
            jobContact.Text = dtHeader.Rows[0]["Contact-Name"].ToString();

            //job date entered
            CrystalDecisions.CrystalReports.Engine.TextObject jobDE;
            jobDE = cryrpt.ReportDefinition.ReportObjects["jobDE"] as TextObject;
            jobDE.Text = dtHeader.Rows[0]["Date-Entered"].ToString();

            //over allowed
            CrystalDecisions.CrystalReports.Engine.TextObject OA;
            OA = cryrpt.ReportDefinition.ReportObjects["jobOA"] as TextObject;
            OA.Text = dtHeader.Rows[0]["Overs-Allowed"].ToString();

            //po num
            CrystalDecisions.CrystalReports.Engine.TextObject PO;
            PO = cryrpt.ReportDefinition.ReportObjects["poNum"] as TextObject;
            PO.Text = dtHeader.Rows[0]["PO-Number"].ToString();

            //estimate
            string est = dtHeader.Rows[0]["Last-Estimate-ID"].ToString().Insert(6, "-");
            CrystalDecisions.CrystalReports.Engine.TextObject estimate;
            estimate = cryrpt.ReportDefinition.ReportObjects["estimate"] as TextObject;
            estimate.Text = est;

            //customer query and text objects
            String headerCust = "SELECT \"cust-name\", \"Address-1\", \"Address-2\", \"City\", \"" +
               "State\", \"Zip\", \"Phone\", \"Address-3\" FROM PUB.cust WHERE \"Cust-code\" = " + dtHeader.Rows[0]["Cust-ID-Ordered-by"].ToString();


            DataTable dtCust = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter cust = new OdbcDataAdapter(headerCust, dbConn);
                cust.Fill(dtCust);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //set the Customer info text objects
            //cust name
            CrystalDecisions.CrystalReports.Engine.TextObject custName;
            custName = cryrpt.ReportDefinition.ReportObjects["custName"] as TextObject;
            custName.Text = dtCust.Rows[0]["cust-name"].ToString();

            CrystalDecisions.CrystalReports.Engine.TextObject custName2;
            custName2 = cryrpt.ReportDefinition.ReportObjects["custName2"] as TextObject;
            custName2.Text = dtCust.Rows[0]["cust-name"].ToString();

            //address -> add 1 and 2 and 3 combined
            CrystalDecisions.CrystalReports.Engine.TextObject custAdd;
            custAdd = cryrpt.ReportDefinition.ReportObjects["custAddress"] as TextObject;
            custAdd.Text = dtCust.Rows[0]["Address-1"].ToString() + " " + dtCust.Rows[0]["Address-2"].ToString() + " " + dtCust.Rows[0]["Address-3"].ToString();

            //city state zip customer
            CrystalDecisions.CrystalReports.Engine.TextObject custCSZ;
            custCSZ = cryrpt.ReportDefinition.ReportObjects["custCSZ"] as TextObject;
            custCSZ.Text = dtCust.Rows[0]["City"].ToString() + " " + dtCust.Rows[0]["State"].ToString() + " " + dtCust.Rows[0]["Zip"].ToString();

            //customerPhone
            CrystalDecisions.CrystalReports.Engine.TextObject custPhone;
            custPhone = cryrpt.ReportDefinition.ReportObjects["custPhone"] as TextObject;
            custPhone.Text = dtCust.Rows[0]["Phone"].ToString();

            //sales agent query and txt obj change
            //why this does not work i have no fkin clue, making stack overflwo see what the brians can thunk up
            String headerSalesAgent = "SELECT \"Sales-Agent-Name\" FROM PUB.\"sales-agent\" WHERE \"Sales-agent-id\" = " + "'" + dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "'";

            // String headerSalesAgent = "SELECT \"Sales-agent-id\" , \"Sales-Agent-Name\" FROM PUB.\"sales-agent\"";

            DataTable dtSalesAgent = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter salesAgent = new OdbcDataAdapter(headerSalesAgent, dbConn);
                salesAgent.Fill(dtSalesAgent);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }


            //sales agent name
            CrystalDecisions.CrystalReports.Engine.TextObject salesRep;
            salesRep = cryrpt.ReportDefinition.ReportObjects["salesRep"] as TextObject;
            salesRep.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "-" + dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();

            //sales rep ID for billing
            CrystalDecisions.CrystalReports.Engine.TextObject salesRep2;
            salesRep2 = cryrpt.ReportDefinition.ReportObjects["salesRep2"] as TextObject;
            salesRep2.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString();// + "-" + dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();

            //csr query and txt obj change
            String headerCSR = "SELECT \"CSR-Name\" FROM PUB.CSR WHERE \"CSR-ID\"=" + "'" + dtHeader.Rows[0]["CSR-ID"].ToString() + "'";

            DataTable dtCsr = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter csrAdap = new OdbcDataAdapter(headerCSR, dbConn);
                csrAdap.Fill(dtCsr);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            CrystalDecisions.CrystalReports.Engine.TextObject csr;
            csr = cryrpt.ReportDefinition.ReportObjects["csr"] as TextObject;
            csr.Text = dtHeader.Rows[0]["CSR-ID"].ToString() + "-" + dtCsr.Rows[0]["CSR-Name"].ToString();

            #endregion Header


            #region 810 tag check - show MAILING
            //DataTable for all UI db-depenedant editing
            DataTable dtEdit = new DataTable();


            String query810Tag = "SELECT \"Work-Center-ID\", \"TagStatus-ID\" FROM PUB.ScheduleByJob WHERE \"Job-ID\"=" + jobNumberUser;

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adap810Tag = new OdbcDataAdapter(query810Tag, dbConn);
                adap810Tag.Fill(dtEdit);
            }
            catch (Exception ex)
            {


                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //here is needed for 810 chck
            bool check = false;

            foreach (DataRow dr in dtEdit.Rows)
            {
                //do nothing 
                if (dr["Work-Center-ID"].ToString().Contains("810"))
                {

                    check = true;
                }

            }//end foreach

            //no check to see if 810 tag is present
            if (!check)
            {

                CrystalDecisions.CrystalReports.Engine.TextObject txtObj;
                txtObj = cryrpt.ReportDefinition.ReportObjects["txtMail"] as TextObject;
                txtObj.Text = "";
            }//end check for 810
            #endregion 810 tag check - show MAILING

            //can use same data from above query and dataTable to get the the job status and description
            //ex) 50d status and it's description is ready to run on digital press
            #region tag status grab (no query, re-use data gathered from 810 check)

            try
            {
                //need to base of the 900 tag's -> tag status
                string tagStat = "";

                foreach (DataRow dr in dtEdit.Rows)
                {
                    //do nothing 
                    if (dr["Work-Center-ID"].ToString() == "900")
                    {

                        tagStat = dr["TagStatus-ID"].ToString();
                    }

                }//end foreach


                //grab first record's 'TagStatus-ID" and set status to it ex) 50d, 09, etc
                var statusObj = cryrpt.ReportDefinition.ReportObjects["status"] as TextObject;
                statusObj.Text = tagStat;

                var statusDescObj = cryrpt.ReportDefinition.ReportObjects["statusDesc"] as TextObject;

                if (tagStat == "01")
                {
                    statusDescObj.Text = "Outside Service";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }

                if (tagStat == "02")
                {
                    statusDescObj.Text = "On Hold";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "05")
                {
                    statusDescObj.Text = "Digital Mailing/Need Run Files";
                }
                if (tagStat == "07")
                {
                    statusDescObj.Text = "Long Term Project (In-House)";
                }
                if (tagStat == "08")
                {
                    statusDescObj.Text = "Long Term Project (out on proof)";
                }
                if (tagStat == "09")
                {
                    statusDescObj.Text = "In Pre Press Production";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "09-R")
                {
                    statusDescObj.Text = "Art corrections after proof";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "18")
                {
                    statusDescObj.Text = "Out on Random Proof";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "20")
                {
                    statusDescObj.Text = "Ready to Strip and Plate";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "20p")
                {
                    statusDescObj.Text = "Approved-Waiting for Mock up";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "21")
                {
                    statusDescObj.Text = "Rerun-Pill Plates";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "29")
                {
                    statusDescObj.Text = "Ready to Plate";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50")
                {
                    statusDescObj.Text = "Press-Running/Plated";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50d" || tagStat == "50D")
                {
                    statusDescObj.Text = "Ready to Run Digital Press";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50e" || tagStat == "50E")
                {
                    statusDescObj.Text = "Ready to Run Digital Press ENVELOPE";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "70")
                {
                    statusDescObj.Text = "Printed and in Bindery";
                    statusObj.Color = Color.Brown;
                    statusDescObj.Color = Color.Brown;
                }
                if (tagStat == "72")
                {
                    statusDescObj.Text = "Monthly DSF billing jobs";
                }
                if (tagStat == "75")
                {
                    statusDescObj.Text = "Waiting for Mailing Data";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "80")
                {
                    statusDescObj.Text = "Bindery Done-Ready for Mailing";
                    statusObj.Color = Color.Blue;
                    statusDescObj.Color = Color.Blue;
                }
                if (tagStat == "82")
                {
                    statusDescObj.Text = "Running on Netjet";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "85")
                {
                    statusDescObj.Text = "Mail Complete/Need Paperwork";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;

                }
                if (tagStat == "88")
                {
                    statusDescObj.Text = "Mail Complete - Ready to Mail";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;

                }
                if (tagStat == "88p")
                {
                    statusDescObj.Text = "PARTIAL mail/Ready to Deliver";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;
                }
                if (tagStat == "90")
                {
                    statusDescObj.Text = "Job Completed-Ready to Deliver";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;
                }
                if (tagStat == "92")
                {
                    statusDescObj.Text = "Being Delivered";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
                if (tagStat == "95")
                {
                    statusDescObj.Text = "Delivered";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
                if (tagStat == "97")
                {
                    statusDescObj.Text = "DSF Jobs To Be Billed";

                }
                if (tagStat == "97b" || tagStat == "97B")
                {
                    statusDescObj.Text = "DSF jobs Already Billed";
                }
                if (tagStat == "29")
                {
                    statusDescObj.Text = "Job Close Pending";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
            }//end try
            catch (Exception ex)
            {

                string error = ex + " : Tag Status update error check code";

                ErrorLog(error);

            }

            #endregion tag status

            #endregion UI


            //sub-reports section
            #region sub report creater

            //Press/Prepress sub rereports - Mailing Version, Mailing Free Feilds, Job Notes, Job Free Feilds, PO Req, PO line,
            //Forms, press, Stock

            #region Mailing Version subReport
            string queryMailVersion = "SELECT \"Free-Field-Char\" FROM PUB.MailingVersionFreeField WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtMailVersion = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapMailVersion = new OdbcDataAdapter(queryMailVersion, dbConn);
                adapMailVersion.Fill(dtMailVersion);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - Mailing Version FF report";

                ErrorLog(error);
            }

            //and change second column name to match CR report value "FFvalue"
            dtMailVersion.Columns["Free-Field-Char"].ColumnName = "FFvalue";

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtMailVersion.Rows.Count != 0)
            {

                cryrpt.Subreports[7].DataSourceConnections.Clear();
                cryrpt.Subreports[7].SetDataSource(dtMailVersion);

            }
            else
            {

                string subMailVersionFF = "subMailVersion";
                HideSubs(cryrpt, subMailVersionFF);

            }

            #endregion Mailing Version subReport

            #region Mailing Free Fields subReport
            string queryMailFF = "SELECT \"Free-Field-Char\" FROM PUB.MailingFreeField WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtMailFF = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapMailFF = new OdbcDataAdapter(queryMailFF, dbConn);
                adapMailFF.Fill(dtMailFF);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - Mailing FF report";

                ErrorLog(error);
            }

            //and change second column name to match CR report value "FFvalue"
            dtMailFF.Columns["Free-Field-Char"].ColumnName = "FFval";

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtMailFF.Rows.Count != 0)
            {
                cryrpt.Subreports[6].DataSourceConnections.Clear();
                cryrpt.Subreports[6].SetDataSource(dtMailFF);

            }
            else
            {

                string subMailFF = "subMailFreeFields";
                HideSubs(cryrpt, subMailFF);

            }

            #endregion Mailing Free Fields subReport

            #region Job Notes subReport
            string queryJobNotes = "SELECT \"SpecCategory-ID\", Description, \"Created-By\", \"Comment-Date\", \"Update-date\" FROM PUB.JobComments WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtJobNotes = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapJobNotes = new OdbcDataAdapter(queryJobNotes, dbConn);
                adapJobNotes.Fill(dtJobNotes);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Job Notes report";

                ErrorLog(error);
            }

            //change the names
            dtJobNotes.Columns["SpecCategory-ID"].ColumnName = "SpecID";
            dtJobNotes.Columns["Created-By"].ColumnName = "EnterBy";
            dtJobNotes.Columns["Comment-Date"].ColumnName = "DateEntered";
            dtJobNotes.Columns["Update-date"].ColumnName = "DateUpdated";

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtJobNotes.Rows.Count != 0)
            {
                cryrpt.Subreports[5].DataSourceConnections.Clear();
                cryrpt.Subreports[5].SetDataSource(dtJobNotes);

            }
            else
            {

                string subJobComments = "subJobNotes";
                HideSubs(cryrpt, subJobComments);

            }


            #endregion Job Notes subReports



            //hidden
            #region Alterations subRpt
            /*
            string queryAlt = "SELECT * FROM PUB.Alterations WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtAlt = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapAlt = new OdbcDataAdapter(queryAlt, dbConn);
                adapAlt.Fill(dtAlt);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Alterations report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtAlt.Rows.Count != 0)
            {
                cryrpt.Subreports[0].DataSourceConnections.Clear();
                cryrpt.Subreports[0].SetDataSource(dtAlt);

            }
            else
            {

                string subAlt = "subAlterations";
                HideSubs(cryrpt, subAlt);

            }
            */
            #endregion alterations subRpt

            #region Job free fields subRPt
            string queryFF = "SELECT \"Free-Field-Char\", \"Free-Field-Decimal\" FROM PUB.JobFreeField WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtFF = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapFF = new OdbcDataAdapter(queryFF, dbConn);
                adapFF.Fill(dtFF);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Free fields report";

                ErrorLog(error);
            }

            //here is where i need to convert decimal -> free-fieldchar
            string lastJob = dtFF.Rows[1]["Free-Field-Decimal"].ToString();
            dtFF.Rows[1]["Free-Field-Char"] = lastJob;

            dtFF.Columns.Remove("Free-Field-Decimal");

            //also check if empty exists it is empty hideSubs it
            if (dtFF.Rows.Count != 0)
            {
                cryrpt.Subreports[4].DataSourceConnections.Clear();
                cryrpt.Subreports[4].SetDataSource(dtFF);

            }
            else
            {

                string subFF = "subJobFreeFields";
                HideSubs(cryrpt, subFF);

            }


            #endregion job free fields subRpt

            //hidden
            #region PO req subRpt
            /*
            string queryPOreq = "SELECT \"Req-Number\", \"Supplier-ID\", \"Supplier-Name\", \"Inventory-Item-ID\", \"Item-Desc\", \"Qty-Requisitioned\", \"Unit-Meas-Purchase\", \"Unit-Cost-Purchase\", \"Req-Status\" FROM PUB.PORequisition WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtPOreq = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPOreq = new OdbcDataAdapter(queryPOreq, dbConn);
                adapPOreq.Fill(dtPOreq);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -PO Req report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPOreq.Rows.Count != 0)
            {
                cryrpt.Subreports[9].DataSourceConnections.Clear();
                cryrpt.Subreports[9].SetDataSource(dtPOreq);

            }
            else
            {

                string subPOreq = "subPOReq";
                HideSubs(cryrpt, subPOreq);

            }
            */
            #endregion PO req subRpt

            //hidden
            #region PO line info subRpt
            /*
            string queryPOLine = "SELECT \"PO-Number\", \"Line-Status\", \"Date-PO\", \"Date-Last-Receipt\", \"Item-Desc\", \"Qty-Received-Purchase\" FROM PUB.POLine WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtPOLine = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPOLine = new OdbcDataAdapter(queryPOLine, dbConn);
                adapPOLine.Fill(dtPOLine);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -PO Line report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPOLine.Rows.Count != 0)
            {
                cryrpt.Subreports[8].DataSourceConnections.Clear();
                cryrpt.Subreports[8].SetDataSource(dtPOLine);

            }
            else
            {

                string subPOLine = "subPOLine";
                HideSubs(cryrpt, subPOLine);

            }
            */
            #endregion Po Line info Subrpt

            //hidden
            #region ship to data subRpt
            /*
            string queryShipTo = "SELECT \"Ship-To-Name\", \"Ship-To-Address1\", \"Ship-To-Address2\", \"Ship-To-Address3\", \"Ship-To-City\", \"Ship-To-State\",\"Ship-To-Zip\",\"Ship-To-Attention\"" +
                ",\"Instructions\", \"Requested-Ship-Date\" ,\"Requested-Quantity\", \"Requested-Number-Packages\", FAX, Phone, \"Ship-To-Country\", \"Shipment-Method-ID\"  FROM PUB.JobShipTo WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtShipTo = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapShipTo = new OdbcDataAdapter(queryShipTo, dbConn);
                adapShipTo.Fill(dtShipTo);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -ship to report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtShipTo.Rows.Count != 0)
            {
                cryrpt.Subreports[12].DataSourceConnections.Clear();
                cryrpt.Subreports[12].SetDataSource(dtShipTo);

            }
            else
            {

                string subShipTo = "subShipTo";
                HideSubs(cryrpt, subShipTo);

            }
            */
            #endregion ship to data subRpt

            //hidden
            #region form spec subRpt
            /*
            string querySpecForm = "SELECT \"Form-ID\",  \"Form-Desc\", \"Number-Pages\", Width, Length, \"Number-forms\", \"Number-Out\", Imposition, Notes FROM PUB.JobSpecForm WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtSpecForm = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapSpecForm = new OdbcDataAdapter(querySpecForm, dbConn);
                adapSpecForm.Fill(dtSpecForm);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Spec Form report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtSpecForm.Rows.Count != 0)
            {
                cryrpt.Subreports[3].DataSourceConnections.Clear();
                cryrpt.Subreports[3].SetDataSource(dtSpecForm);

            }
            else
            {

                string subFormSpec = "subFormSpecs";
                HideSubs(cryrpt, subFormSpec);

            }

    */
            #endregion form spec subRpt

            //hidden
            #region form Notes subrpt
            /*
            string queryFormNotes = "SELECT \"Form-ID\", Description FROM PUB.JobComments WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtFormNotes = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapFormNotes = new OdbcDataAdapter(queryFormNotes, dbConn);
                adapFormNotes.Fill(dtFormNotes);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Form Notes report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtFormNotes.Rows.Count != 0)
            {
                cryrpt.Subreports[2].DataSourceConnections.Clear();
                cryrpt.Subreports[2].SetDataSource(dtFormNotes);

            }
            else
            {

                string subFormNotes = "subFormNotes";
                HideSubs(cryrpt, subFormNotes);

            }

               */
            #endregion form notes subRpt

            //hidden
            #region prepress rpt
            /*
            string queryPrepress = "SELECT \"Form-ID\", \"Spec-Desc\", \"Work-Center-ID\", \"Operation-ID\", Quantity, Hours FROM PUB.JobSpecPrepress WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtPrepress = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPrepress = new OdbcDataAdapter(queryPrepress, dbConn);
                adapPrepress.Fill(dtPrepress);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Prepress report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPrepress.Rows.Count != 0)
            {
                cryrpt.Subreports[10].DataSourceConnections.Clear();
                cryrpt.Subreports[10].SetDataSource(dtPrepress);

            }
            else
            {

                string subPrepress = "subPrepress";
                HideSubs(cryrpt, subPrepress);

            }
            */
            #endregion prepress rpt

            //hidden
            #region press rpt
            /*
            string queryPress = "SELECT \"Form-ID\", \"Operation-Type\",  \"Spec-Desc\", \"Qty-Makeready\", \"Qty-Net\", \"Qty-Spoilage\", Width, Length, \"Hours-Makeready\", \"Hours-Run\", \"Hours-Cleanup\" FROM PUB.JobSpecOperation WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtPress = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPress = new OdbcDataAdapter(queryPress, dbConn);
                adapPress.Fill(dtPress);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Press report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPress.Rows.Count != 0)
            {
                cryrpt.Subreports[11].DataSourceConnections.Clear();
                cryrpt.Subreports[11].SetDataSource(dtPress);

            }
            else
            {

                string subPress = "subPress";
                HideSubs(cryrpt, subPress);

            }
            */
            #endregion press rpt

            //hidden
            #region stock rpt
            /*
            string queryStock = "SELECT \"Form-ID\", \"Gross-Sheets\", \"Gross-Weight\",  Notes,  \"Inventory-Item-ID\", \"Inventory-Item-Desc\", \"Material-Type\", Length, Width FROM PUB.JobSpecMaterial WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtStock = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapStock = new OdbcDataAdapter(queryStock, dbConn);
                adapStock.Fill(dtStock);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Stock report";

                ErrorLog(error);
            }

            //need to get page-grain new query
            string queryPageGrain = "SELECT \"Page-Grain\" FROM PUB.JobSpecForm WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtPG = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPG = new OdbcDataAdapter(queryPageGrain, dbConn);
                adapPG.Fill(dtPG);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Stock report -PG";

                ErrorLog(error);
            }

            dtStock.Columns.Add("Page-Grain");
            string val = dtPG.Rows[0]["Page-Grain"].ToString();

            foreach (DataRow dr in dtStock.Rows)
            {

                dr["Page-Grain"] = val;

            }

            //also check if empty exists it is empty hideSubs it
            if (dtStock.Rows.Count != 0)
            {
                cryrpt.Subreports[13].DataSourceConnections.Clear();
                cryrpt.Subreports[13].SetDataSource(dtStock);

            }
            else
            {

                string subStock = "subStock";
                HideSubs(cryrpt, subStock);

            }
            */
            #endregion stock rpt

            //hidden
            #region bindery rpt
            /*
            string queryBind = "SELECT \"Form-ID\", \"Operation-Type\",  \"Spec-Desc\", \"Qty-Makeready\", \"Qty-Net\", \"Qty-Spoilage\", Width, Length, \"Hours-Makeready\", \"Hours-Run\", \"Hours-Cleanup\", \"Number-Out-for-Oper\" FROM PUB.JobSpecOperation WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtBind = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapBind = new OdbcDataAdapter(queryBind, dbConn);
                adapBind.Fill(dtBind);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Bindery report";

                ErrorLog(error);
            }


            //also check if empty exists it is empty hideSubs it
            if (dtBind.Rows.Count != 0)
            {
                cryrpt.Subreports[1].DataSourceConnections.Clear();
                cryrpt.Subreports[1].SetDataSource(dtBind);

            }
            else
            {

                string subBindery = "subBindery";
                HideSubs(cryrpt, subBindery);

            }

            */
            #endregion bindery rpt 


            #endregion sub report creation


            dbConn.Close();
            #endregion DB close connection



            #region display rpt

            LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;
            LaunchOrigin.crystalReportViewer1.ReportSource = cryrpt;

            LaunchOrigin.crystalReportViewer1.Refresh(); //here is where i get the prompt DB login

            label3.Text = "Report loaded.";
            #endregion display rpt


        }//end mailing
        #endregion mailing ticket






        #region press/press ticket
        //press/prepress
        private void button8_Click(object sender, EventArgs e)
        {

            //here is where i will put all Form {button/label/etc} editing
            //Notes: can NOT edit crystal report stuff here; have not created crystal reprort object yet, that will be later
            #region label and buttons UI
            label3.Text = "Please wait report loading..";

            jobNumberUser = textBox1.Text;


            button1.FlatAppearance.BorderSize = 0;
            button1.FlatAppearance.BorderColor = Color.Black;

            button11.FlatAppearance.BorderSize = 0;
            button11.FlatAppearance.BorderColor = Color.Black;

            button3.FlatAppearance.BorderSize = 0;
            button3.FlatAppearance.BorderColor = Color.Black;

            button9.FlatAppearance.BorderSize = 0;
            button9.FlatAppearance.BorderColor = Color.Black;


            button2.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.BorderColor = Color.Black;


            button8.FlatAppearance.BorderSize = 5;
            button8.FlatAppearance.BorderColor = Color.Black;

            //timestamp when a button is clicked (report is ran), so user knows what time the current report on screen was ran
            label5.Text = DateTime.Now.ToString();

            #endregion label/buttons UI end




            //here is where i will put the creation and manipualtion of the crystal report object
            #region start crystal report config:

            #region connection and CR object properties/settings
            //rerport object



            CrystalReport2 cryrpt = new CrystalReport2();

            cryrpt.DataSourceConnections.Clear();  //clear the connections (will popualte with fresh sql query defined data

            //set the databse login info, use twice first one is to login into sql server

            //getting write only error


            cryrpt.SetDatabaseLogon("Bob", "Orchard", "monarch18", "gams1");
            cryrpt.SetDatabaseLogon("Bob", "Orchard"); //this one for that annoying prompt to login into database


            //this does not error out!?
            //this opens connection to DB with the login info down..
            ConnectionInfo crconnectioninfo = new ConnectionInfo();
            crconnectioninfo.ServerName = "monarch18";
            crconnectioninfo.DatabaseName = "gams1";
            crconnectioninfo.UserID = "Bob";
            crconnectioninfo.Password = "Orchard";


            //try our best to never have paramter panel pop-up, works like 95% of time
            //error here as well?
            //LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;

            #endregion connectiona and CR object properties/settings

            #region UI on CR object editing (non-DB dependant)

            //change text object, tell user what ticket the are curentlly running/looking at
            CrystalDecisions.CrystalReports.Engine.TextObject txtObj2;
            txtObj2 = cryrpt.ReportDefinition.ReportObjects["ticketName"] as TextObject;
            txtObj2.Text = "Prepress/Press Ticket";


            #endregion UI on CR object editing (non-DB dependant)

            #endregion crystal report config end




            //here is where i need to clean up lots, make functions, sql queryies, DT and DS manipualtion, pass the job number, etc 
            //main code for program functionallity
            #region DB connection 


            //here is where i iwll be connecting to DB's (all queries will need access to basically my connection properties globals)
            #region global connection properties

            //connection string for DB
            string connectStr = "DSN=Progress11;uid=Bob;pwd=Orchard";

            //open th econnection and error check
            OdbcConnection dbConn = new OdbcConnection(connectStr);

            try
            {
                dbConn.Open();
            }
            catch (Exception ex)
            {

                string error = ex + " : DB error cannot connect";

                ErrorLog(error);

            }
            #endregion global connection propeties




            //here is where i can change the UI of the report based on database data
            //ex) show word nailing on report if job# has a 810 tag associated with it
            #region UI crystal report editing (DB dependant)


            #region Header

            //set the job numbers from iuser input
            CrystalDecisions.CrystalReports.Engine.TextObject jobNum1;
            jobNum1 = cryrpt.ReportDefinition.ReportObjects["jobNum"] as TextObject;
            jobNum1.Text = jobNumberUser;

            CrystalDecisions.CrystalReports.Engine.TextObject jobNum2;
            jobNum2 = cryrpt.ReportDefinition.ReportObjects["jobNum2"] as TextObject;
            jobNum2.Text = jobNumberUser;


            
            String headerJob = "SELECT \"Job-Desc\", \"Date-Promised\", \"Sales-Rep-ID\", \"CSR-ID\", \"" +
                "PO-Number\", \"Overs-Allowed\", \"Last-Estimate-ID\", \"Quantity-Ordered\", \"Contact-Name\", \"Date-Entered\", \"Cust-ID-Ordered-by\"" +
                " FROM PUB.JOB WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtHeader = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter header = new OdbcDataAdapter(headerJob, dbConn);
                header.Fill(dtHeader);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //set all text objects to the data from datatable

            //job descriptions
            CrystalDecisions.CrystalReports.Engine.TextObject jobDesc;
            jobDesc = cryrpt.ReportDefinition.ReportObjects["jobDesc"] as TextObject;
            jobDesc.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

            CrystalDecisions.CrystalReports.Engine.TextObject jobDesc2;
            jobDesc2 = cryrpt.ReportDefinition.ReportObjects["jobDesc2"] as TextObject;
            jobDesc2.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

            //date promised
            CrystalDecisions.CrystalReports.Engine.TextObject dateProm;
            dateProm = cryrpt.ReportDefinition.ReportObjects["dateProm"] as TextObject;
            dateProm.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

            CrystalDecisions.CrystalReports.Engine.TextObject dateProm2;
            dateProm2 = cryrpt.ReportDefinition.ReportObjects["dateProm2"] as TextObject;
            dateProm2.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

            //qty
            CrystalDecisions.CrystalReports.Engine.TextObject qty;
            qty = cryrpt.ReportDefinition.ReportObjects["qty"] as TextObject;
            qty.Text = dtHeader.Rows[0]["Quantity-Ordered"].ToString();

            CrystalDecisions.CrystalReports.Engine.TextObject qty2;
            qty2 = cryrpt.ReportDefinition.ReportObjects["qty2"] as TextObject;
            qty2.Text = dtHeader.Rows[0]["Quantity-Ordered"].ToString();

            //job contact
            CrystalDecisions.CrystalReports.Engine.TextObject jobContact;
            jobContact = cryrpt.ReportDefinition.ReportObjects["contactName"] as TextObject;
            jobContact.Text = dtHeader.Rows[0]["Contact-Name"].ToString();

            //job date entered
            CrystalDecisions.CrystalReports.Engine.TextObject jobDE;
            jobDE = cryrpt.ReportDefinition.ReportObjects["jobDE"] as TextObject;
            jobDE.Text = dtHeader.Rows[0]["Date-Entered"].ToString();

            //over allowed
            CrystalDecisions.CrystalReports.Engine.TextObject OA;
            OA = cryrpt.ReportDefinition.ReportObjects["jobOA"] as TextObject;
            OA.Text = dtHeader.Rows[0]["Overs-Allowed"].ToString();

            //po num
            CrystalDecisions.CrystalReports.Engine.TextObject PO;
            PO = cryrpt.ReportDefinition.ReportObjects["poNum"] as TextObject;
            PO.Text = dtHeader.Rows[0]["PO-Number"].ToString();

            //estimate
            string est = dtHeader.Rows[0]["Last-Estimate-ID"].ToString().Insert(6,"-");
            CrystalDecisions.CrystalReports.Engine.TextObject estimate;
            estimate = cryrpt.ReportDefinition.ReportObjects["estimate"] as TextObject;
            estimate.Text = est;

            //customer query and text objects
            String headerCust = "SELECT \"cust-name\", \"Address-1\", \"Address-2\", \"City\", \"" +
               "State\", \"Zip\", \"Phone\", \"Address-3\" FROM PUB.cust WHERE \"Cust-code\" = " + dtHeader.Rows[0]["Cust-ID-Ordered-by"].ToString();


            DataTable dtCust = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter cust = new OdbcDataAdapter(headerCust, dbConn);
                cust.Fill(dtCust);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //set the Customer info text objects
            //cust name
            CrystalDecisions.CrystalReports.Engine.TextObject custName;
            custName = cryrpt.ReportDefinition.ReportObjects["custName"] as TextObject;
            custName.Text = dtCust.Rows[0]["cust-name"].ToString();

            CrystalDecisions.CrystalReports.Engine.TextObject custName2;
            custName2 = cryrpt.ReportDefinition.ReportObjects["custName2"] as TextObject;
            custName2.Text = dtCust.Rows[0]["cust-name"].ToString();

            //address -> add 1 and 2 and 3 combined
            CrystalDecisions.CrystalReports.Engine.TextObject custAdd;
            custAdd = cryrpt.ReportDefinition.ReportObjects["custAddress"] as TextObject;
            custAdd.Text = dtCust.Rows[0]["Address-1"].ToString()+" "+ dtCust.Rows[0]["Address-2"].ToString()+" "+dtCust.Rows[0]["Address-3"].ToString();

            //city state zip customer
            CrystalDecisions.CrystalReports.Engine.TextObject custCSZ;
            custCSZ = cryrpt.ReportDefinition.ReportObjects["custCSZ"] as TextObject;
            custCSZ.Text = dtCust.Rows[0]["City"].ToString() + " " + dtCust.Rows[0]["State"].ToString() + " " + dtCust.Rows[0]["Zip"].ToString();

            //customerPhone
            CrystalDecisions.CrystalReports.Engine.TextObject custPhone;
            custPhone = cryrpt.ReportDefinition.ReportObjects["custPhone"] as TextObject;
            custPhone.Text = dtCust.Rows[0]["Phone"].ToString();

            //sales agent query and txt obj change
            //why this does not work i have no fkin clue, making stack overflwo see what the brians can thunk up
            String headerSalesAgent = "SELECT \"Sales-Agent-Name\" FROM PUB.\"sales-agent\" WHERE \"Sales-agent-id\" = " + "'" + dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "'";

           // String headerSalesAgent = "SELECT \"Sales-agent-id\" , \"Sales-Agent-Name\" FROM PUB.\"sales-agent\"";

            DataTable dtSalesAgent = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter salesAgent = new OdbcDataAdapter(headerSalesAgent, dbConn);
                salesAgent.Fill(dtSalesAgent);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

     
            //sales agent name
            CrystalDecisions.CrystalReports.Engine.TextObject salesRep;
            salesRep = cryrpt.ReportDefinition.ReportObjects["salesRep"] as TextObject;
            salesRep.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "-"+ dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();

            //sales rep ID for billing
            CrystalDecisions.CrystalReports.Engine.TextObject salesRep2;
            salesRep2 = cryrpt.ReportDefinition.ReportObjects["salesRep2"] as TextObject;
            salesRep2.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString();// + "-" + dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();

            //csr query and txt obj change
            String headerCSR = "SELECT \"CSR-Name\" FROM PUB.CSR WHERE \"CSR-ID\"="+ "'"+dtHeader.Rows[0]["CSR-ID"].ToString()+"'";

            DataTable dtCsr = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter csrAdap = new OdbcDataAdapter(headerCSR, dbConn);
                csrAdap.Fill(dtCsr);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            CrystalDecisions.CrystalReports.Engine.TextObject csr;
            csr = cryrpt.ReportDefinition.ReportObjects["csr"] as TextObject;
            csr.Text = dtHeader.Rows[0]["CSR-ID"].ToString() + "-" + dtCsr.Rows[0]["CSR-Name"].ToString();

            #endregion Header


            #region 810 tag check - show MAILING
            //DataTable for all UI db-depenedant editing
            DataTable dtEdit = new DataTable();


            String query810Tag = "SELECT \"Work-Center-ID\", \"TagStatus-ID\" FROM PUB.ScheduleByJob WHERE \"Job-ID\"=" + jobNumberUser;

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adap810Tag = new OdbcDataAdapter(query810Tag, dbConn);
                adap810Tag.Fill(dtEdit);
            }
            catch (Exception ex)
            {


                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //here is needed for 810 chck
            bool check = false;

            foreach (DataRow dr in dtEdit.Rows)
            {
                //do nothing 
                if (dr["Work-Center-ID"].ToString().Contains("810"))
                {

                    check = true;
                }

            }//end foreach

            //no check to see if 810 tag is present
            if (!check)
            {

                CrystalDecisions.CrystalReports.Engine.TextObject txtObj;
                txtObj = cryrpt.ReportDefinition.ReportObjects["txtMail"] as TextObject;
                txtObj.Text = "";
            }//end check for 810
            #endregion 810 tag check - show MAILING

            //can use same data from above query and dataTable to get the the job status and description
            //ex) 50d status and it's description is ready to run on digital press
            #region tag status grab (no query, re-use data gathered from 810 check)

            try
            {
                //need to base of the 900 tag's -> tag status
                string tagStat = "";

                foreach (DataRow dr in dtEdit.Rows)
                {
                    //do nothing 
                    if (dr["Work-Center-ID"].ToString() == "900")
                    {

                        tagStat = dr["TagStatus-ID"].ToString();
                    }

                }//end foreach


                //grab first record's 'TagStatus-ID" and set status to it ex) 50d, 09, etc
                var statusObj = cryrpt.ReportDefinition.ReportObjects["status"] as TextObject;
                statusObj.Text = tagStat;

                var statusDescObj = cryrpt.ReportDefinition.ReportObjects["statusDesc"] as TextObject;

                if (tagStat == "01")
                {
                    statusDescObj.Text = "Outside Service";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }

                if (tagStat == "02")
                {
                    statusDescObj.Text = "On Hold";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "05")
                {
                    statusDescObj.Text = "Digital Mailing/Need Run Files";
                }
                if (tagStat == "07")
                {
                    statusDescObj.Text = "Long Term Project (In-House)";
                }
                if (tagStat == "08")
                {
                    statusDescObj.Text = "Long Term Project (out on proof)";
                }
                if (tagStat == "09")
                {
                    statusDescObj.Text = "In Pre Press Production";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "09-R")
                {
                    statusDescObj.Text = "Art corrections after proof";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "18")
                {
                    statusDescObj.Text = "Out on Random Proof";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "20")
                {
                    statusDescObj.Text = "Ready to Strip and Plate";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "20p")
                {
                    statusDescObj.Text = "Approved-Waiting for Mock up";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "21")
                {
                    statusDescObj.Text = "Rerun-Pill Plates";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "29")
                {
                    statusDescObj.Text = "Ready to Plate";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50")
                {
                    statusDescObj.Text = "Press-Running/Plated";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50d" || tagStat == "50D")
                {
                    statusDescObj.Text = "Ready to Run Digital Press";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50e" || tagStat == "50E")
                {
                    statusDescObj.Text = "Ready to Run Digital Press ENVELOPE";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "70")
                {
                    statusDescObj.Text = "Printed and in Bindery";
                    statusObj.Color = Color.Brown;
                    statusDescObj.Color = Color.Brown;
                }
                if (tagStat == "72")
                {
                    statusDescObj.Text = "Monthly DSF billing jobs";
                }
                if (tagStat == "75")
                {
                    statusDescObj.Text = "Waiting for Mailing Data";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "80")
                {
                    statusDescObj.Text = "Bindery Done-Ready for Mailing";
                    statusObj.Color = Color.Blue;
                    statusDescObj.Color = Color.Blue;
                }
                if (tagStat == "82")
                {
                    statusDescObj.Text = "Running on Netjet";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "85")
                {
                    statusDescObj.Text = "Mail Complete/Need Paperwork";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;

                }
                if (tagStat == "88")
                {
                    statusDescObj.Text = "Mail Complete - Ready to Mail";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;

                }
                if (tagStat == "88p")
                {
                    statusDescObj.Text = "PARTIAL mail/Ready to Deliver";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;
                }
                if (tagStat == "90")
                {
                    statusDescObj.Text = "Job Completed-Ready to Deliver";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;
                }
                if (tagStat == "92")
                {
                    statusDescObj.Text = "Being Delivered";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
                if (tagStat == "95")
                {
                    statusDescObj.Text = "Delivered";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
                if (tagStat == "97")
                {
                    statusDescObj.Text = "DSF Jobs To Be Billed";

                }
                if (tagStat == "97b" || tagStat == "97B")
                {
                    statusDescObj.Text = "DSF jobs Already Billed";
                }
                if (tagStat == "29")
                {
                    statusDescObj.Text = "Job Close Pending";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
            }//end try
            catch (Exception ex)
            {

                string error = ex + " : Tag Status update error check code";

                ErrorLog(error);

            }

            #endregion tag status

            #endregion UI


            //sub-reports section
            #region sub report creater

            //ObjectIndexCheck(cryrpt);

            //Press/Prepress sub rereports - Mailing Version, Mailing Free Feilds, Job Notes, Job Free Feilds, PO Req, PO line,
            //Forms, press, Stock

            #region Mailing Version subReport
            string queryMailVersion = "SELECT \"Free-Field-Char\" FROM PUB.MailingVersionFreeField WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtMailVersion = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapMailVersion = new OdbcDataAdapter(queryMailVersion, dbConn);
                adapMailVersion.Fill(dtMailVersion);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - Mailing Version FF report";

                ErrorLog(error);
            }

            //and change second column name to match CR report value "FFvalue"
            dtMailVersion.Columns["Free-Field-Char"].ColumnName = "FFvalue";

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtMailVersion.Rows.Count != 0)
            {

                cryrpt.Subreports[7].DataSourceConnections.Clear();
                cryrpt.Subreports[7].SetDataSource(dtMailVersion);

            }
            else
            {

                string subMailVersionFF = "subMailVersion";
                HideSubs(cryrpt, subMailVersionFF);

            }

            #endregion Mailing Version subReport

            #region Mailing Free Fields subReport
            string queryMailFF = "SELECT \"Free-Field-Char\" FROM PUB.MailingFreeField WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtMailFF = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapMailFF = new OdbcDataAdapter(queryMailFF, dbConn);
                adapMailFF.Fill(dtMailFF);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - Mailing FF report";

                ErrorLog(error);
            }

            //and change second column name to match CR report value "FFvalue"
            dtMailFF.Columns["Free-Field-Char"].ColumnName = "FFval";

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtMailFF.Rows.Count != 0)
            {
                cryrpt.Subreports[6].DataSourceConnections.Clear();
                cryrpt.Subreports[6].SetDataSource(dtMailFF);

            }
            else
            {

                string subMailFF = "subMailFreeFields";
                HideSubs(cryrpt, subMailFF);

            }

            #endregion Mailing Free Fields subReport

            #region Job Notes subReport
            string queryJobNotes = "SELECT \"SpecCategory-ID\", Description, \"Created-By\", \"Comment-Date\", \"Update-date\" FROM PUB.JobComments WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtJobNotes = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapJobNotes = new OdbcDataAdapter(queryJobNotes, dbConn);
                adapJobNotes.Fill(dtJobNotes);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Job Notes report";

                ErrorLog(error);
            }

            //change the names
            dtJobNotes.Columns["SpecCategory-ID"].ColumnName = "SpecID";
            dtJobNotes.Columns["Created-By"].ColumnName = "EnterBy";
            dtJobNotes.Columns["Comment-Date"].ColumnName = "DateEntered";
            dtJobNotes.Columns["Update-date"].ColumnName = "DateUpdated";

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtJobNotes.Rows.Count != 0)
            {
                cryrpt.Subreports[5].DataSourceConnections.Clear();
                cryrpt.Subreports[5].SetDataSource(dtJobNotes);

            }
            else
            {

                string subJobComments = "subJobNotes";
                HideSubs(cryrpt, subJobComments);

            }


            #endregion Job Notes subReports


            

            #region Alterations subRpt
            string queryAlt = "SELECT * FROM PUB.Alterations WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtAlt = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapAlt = new OdbcDataAdapter(queryAlt, dbConn);
                adapAlt.Fill(dtAlt);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Alterations report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtAlt.Rows.Count != 0)
            {
                cryrpt.Subreports[0].DataSourceConnections.Clear();
                cryrpt.Subreports[0].SetDataSource(dtAlt);

            }
            else
            {

                string subAlt = "subAlterations";
                HideSubs(cryrpt, subAlt);

            }

            #endregion alterations subRpt

            #region Job free fields subRPt
            string queryFF = "SELECT \"Free-Field-Char\", \"Free-Field-Decimal\" FROM PUB.JobFreeField WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtFF = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapFF = new OdbcDataAdapter(queryFF, dbConn);
                adapFF.Fill(dtFF);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Free fields report";

                ErrorLog(error);
            }

            //here is where i need to convert decimal -> free-fieldchar
            string lastJob = dtFF.Rows[1]["Free-Field-Decimal"].ToString();
            dtFF.Rows[1]["Free-Field-Char"] = lastJob;

            dtFF.Columns.Remove("Free-Field-Decimal");

            //also check if empty exists it is empty hideSubs it
            if (dtFF.Rows.Count != 0)
            {
                cryrpt.Subreports[4].DataSourceConnections.Clear();
                cryrpt.Subreports[4].SetDataSource(dtFF);

            }
            else
            {

                string subFF = "subJobFreeFields";
                HideSubs(cryrpt, subFF);

            }


            #endregion job free fields subRpt

            
            #region PO req subRpt
            string queryPOreq = "SELECT \"Req-Number\", \"Supplier-ID\", \"Supplier-Name\", \"Inventory-Item-ID\", \"Item-Desc\", \"Qty-Requisitioned\", \"Unit-Meas-Purchase\", \"Unit-Cost-Purchase\", \"Req-Status\" FROM PUB.PORequisition WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtPOreq = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPOreq = new OdbcDataAdapter(queryPOreq, dbConn);
                adapPOreq.Fill(dtPOreq);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -PO Req report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPOreq.Rows.Count != 0)
            {
                cryrpt.Subreports[9].DataSourceConnections.Clear();
                cryrpt.Subreports[9].SetDataSource(dtPOreq);

            }
            else
            {

                string subPOreq = "subPOReq";
                HideSubs(cryrpt, subPOreq);

            }

            #endregion PO req subRpt


            #region PO line info subRpt
            string queryPOLine = "SELECT \"PO-Number\", \"Line-Status\", \"Date-PO\", \"Date-Last-Receipt\", \"Item-Desc\", \"Qty-Received-Purchase\" FROM PUB.POLine WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtPOLine = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPOLine = new OdbcDataAdapter(queryPOLine, dbConn);
                adapPOLine.Fill(dtPOLine);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -PO Line report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPOLine.Rows.Count != 0)
            {
                cryrpt.Subreports[8].DataSourceConnections.Clear();
                cryrpt.Subreports[8].SetDataSource(dtPOLine);

            }
            else
            {

                string subPOLine = "subPOLine";
                HideSubs(cryrpt, subPOLine);

            }
            #endregion Po Line info Subrpt

            #region ship to data subRpt
            string queryShipTo = "SELECT \"Ship-To-Name\", \"Ship-To-Address1\", \"Ship-To-Address2\", \"Ship-To-Address3\", \"Ship-To-City\", \"Ship-To-State\",\"Ship-To-Zip\",\"Ship-To-Attention\"" +
                ",\"Instructions\", \"Requested-Ship-Date\" ,\"Requested-Quantity\", \"Requested-Number-Packages\", FAX, Phone, \"Ship-To-Country\", \"Shipment-Method-ID\"  FROM PUB.JobShipTo WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtShipTo = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapShipTo = new OdbcDataAdapter(queryShipTo, dbConn);
                adapShipTo.Fill(dtShipTo);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -ship to report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtShipTo.Rows.Count != 0)
            {
                cryrpt.Subreports[12].DataSourceConnections.Clear();
                cryrpt.Subreports[12].SetDataSource(dtShipTo);

            }
            else
            {

                string subShipTo = "subShipTo";
                HideSubs(cryrpt, subShipTo);

            }

            #endregion ship to data subRpt

            #region form spec subRpt
            string querySpecForm = "SELECT \"Form-ID\",  \"Form-Desc\", \"Number-Pages\", Width, Length, \"Number-forms\", \"Number-Out\", Imposition, Notes FROM PUB.JobSpecForm WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtSpecForm = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapSpecForm = new OdbcDataAdapter(querySpecForm, dbConn);
                adapSpecForm.Fill(dtSpecForm);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Spec Form report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtSpecForm.Rows.Count != 0)
            {
                cryrpt.Subreports[3].DataSourceConnections.Clear();
                cryrpt.Subreports[3].SetDataSource(dtSpecForm);

            }
            else
            {

                string subFormSpec = "subFormSpecs";
                HideSubs(cryrpt, subFormSpec);

            }


            #endregion form spec subRpt

            #region form Notes subrpt
            string queryFormNotes = "SELECT \"Form-ID\", Description FROM PUB.JobComments WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtFormNotes = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapFormNotes = new OdbcDataAdapter(queryFormNotes, dbConn);
                adapFormNotes.Fill(dtFormNotes);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Form Notes report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtFormNotes.Rows.Count != 0)
            {
                cryrpt.Subreports[2].DataSourceConnections.Clear();
                cryrpt.Subreports[2].SetDataSource(dtFormNotes);

            }
            else
            {

                string subFormNotes = "subFormNotes";
                HideSubs(cryrpt, subFormNotes);

            }


            #endregion form notes subRpt

            #region prepress rpt
            string queryPrepress = "SELECT \"Form-ID\", \"Spec-Desc\", \"Work-Center-ID\", \"Operation-ID\", Quantity, Hours FROM PUB.JobSpecPrepress WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtPrepress = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPrepress = new OdbcDataAdapter(queryPrepress, dbConn);
                adapPrepress.Fill(dtPrepress);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Prepress report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPrepress.Rows.Count != 0)
            {
                cryrpt.Subreports[10].DataSourceConnections.Clear();
                cryrpt.Subreports[10].SetDataSource(dtPrepress);

            }
            else
            {

                string subPrepress = "subPrepress";
                HideSubs(cryrpt, subPrepress);

            }
            #endregion prepress rpt

            #region press rpt
            string queryPress = "SELECT \"Form-ID\", \"Operation-Type\",  \"Spec-Desc\", \"Qty-Makeready\", \"Qty-Net\", \"Qty-Spoilage\", Width, Length, \"Hours-Makeready\", \"Hours-Run\", \"Hours-Cleanup\" FROM PUB.JobSpecOperation WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtPress = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPress = new OdbcDataAdapter(queryPress, dbConn);
                adapPress.Fill(dtPress);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Press report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPress.Rows.Count != 0)
            {
                cryrpt.Subreports[11].DataSourceConnections.Clear();
                cryrpt.Subreports[11].SetDataSource(dtPress);

            }
            else
            {

                string subPress = "subPress";
                HideSubs(cryrpt, subPress);

            }

            #endregion press rpt

            #region stock rpt
            string queryStock = "SELECT \"Form-ID\", \"Gross-Sheets\", \"Gross-Weight\",  Notes,  \"Inventory-Item-ID\", \"Inventory-Item-Desc\", \"Material-Type\", Length, Width FROM PUB.JobSpecMaterial WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtStock = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapStock = new OdbcDataAdapter(queryStock, dbConn);
                adapStock.Fill(dtStock);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Stock report";

                ErrorLog(error);
            }

            //need to get page-grain new query
            string queryPageGrain = "SELECT \"Page-Grain\" FROM PUB.JobSpecForm WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtPG = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPG = new OdbcDataAdapter(queryPageGrain, dbConn);
                adapPG.Fill(dtPG);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Stock report -PG";

                ErrorLog(error);
            }

            dtStock.Columns.Add("Page-Grain");
            string val = dtPG.Rows[0]["Page-Grain"].ToString();

            foreach (DataRow dr in dtStock.Rows) {

                dr["Page-Grain"] = val;

            }

            //also check if empty exists it is empty hideSubs it
            if (dtStock.Rows.Count != 0)
            {
                cryrpt.Subreports[13].DataSourceConnections.Clear();
                cryrpt.Subreports[13].SetDataSource(dtStock);

            }
            else
            {

                string subStock = "subStock";
                HideSubs(cryrpt, subStock);

            }

            #endregion stock rpt

            #region bindery rpt
            string queryBind = "SELECT \"Form-ID\", \"Operation-Type\",  \"Spec-Desc\", \"Qty-Makeready\", \"Qty-Net\", \"Qty-Spoilage\", Width, Length, \"Hours-Makeready\", \"Hours-Run\", \"Hours-Cleanup\", \"Number-Out-for-Oper\" FROM PUB.JobSpecOperation WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtBind = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapBind = new OdbcDataAdapter(queryBind, dbConn);
                adapBind.Fill(dtBind);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Bindery report";

                ErrorLog(error);
            }

            ObjectIndexCheck(cryrpt);

            //also check if empty exists it is empty hideSubs it
            if (dtBind.Rows.Count != 0)
            {
                cryrpt.Subreports[1].DataSourceConnections.Clear();
                cryrpt.Subreports[1].SetDataSource(dtBind);

            }
            else
            {

                string subBindery = "subBindery";
                HideSubs(cryrpt, subBindery);

            }

            #endregion bindery rpt


            #endregion sub report creation


            dbConn.Close();
            #endregion DB close connection



            #region display rpt

            LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;
            LaunchOrigin.crystalReportViewer1.ReportSource = cryrpt;

            LaunchOrigin.crystalReportViewer1.Refresh(); //here is where i get the prompt DB login

            label3.Text = "Report loaded.";
            #endregion display rpt

        }//end press/prepress

        #endregion press/prepress ticket







        #region bindery ticket
        //bindery
        private void button9_Click(object sender, EventArgs e)
        {
            //here is where i will put all Form {button/label/etc} editing
            //Notes: can NOT edit crystal report stuff here; have not created crystal reprort object yet, that will be later
            #region label and buttons UI
            label3.Text = "Please wait report loading..";

            jobNumberUser = textBox1.Text;


            button1.FlatAppearance.BorderSize = 0;
            button1.FlatAppearance.BorderColor = Color.Black;

            button11.FlatAppearance.BorderSize = 0;
            button11.FlatAppearance.BorderColor = Color.Black;

            button3.FlatAppearance.BorderSize = 0;
            button3.FlatAppearance.BorderColor = Color.Black;

            button8.FlatAppearance.BorderSize = 0;
            button8.FlatAppearance.BorderColor = Color.Black;


            button2.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.BorderColor = Color.Black;


            button9.FlatAppearance.BorderSize = 5;
            button9.FlatAppearance.BorderColor = Color.Black;

            //timestamp when a button is clicked (report is ran), so user knows what time the current report on screen was ran
            label5.Text = DateTime.Now.ToString();

            #endregion label/buttons UI end




            //here is where i will put the creation and manipualtion of the crystal report object
            #region start crystal report config:

            #region connection and CR object properties/settings
            //rerport object



            CrystalReport2 cryrpt = new CrystalReport2();

            cryrpt.DataSourceConnections.Clear();  //clear the connections (will popualte with fresh sql query defined data

            //set the databse login info, use twice first one is to login into sql server

            //getting write only error


            cryrpt.SetDatabaseLogon("Bob", "Orchard", "monarch18", "gams1");
            cryrpt.SetDatabaseLogon("Bob", "Orchard"); //this one for that annoying prompt to login into database


            //this does not error out!?
            //this opens connection to DB with the login info down..
            ConnectionInfo crconnectioninfo = new ConnectionInfo();
            crconnectioninfo.ServerName = "monarch18";
            crconnectioninfo.DatabaseName = "gams1";
            crconnectioninfo.UserID = "Bob";
            crconnectioninfo.Password = "Orchard";


            //try our best to never have paramter panel pop-up, works like 95% of time
            //error here as well?
            //LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;

            #endregion connectiona and CR object properties/settings

            #region UI on CR object editing (non-DB dependant)

            //change text object, tell user what ticket the are curentlly running/looking at
            CrystalDecisions.CrystalReports.Engine.TextObject txtObj2;
            txtObj2 = cryrpt.ReportDefinition.ReportObjects["ticketName"] as TextObject;
            txtObj2.Text = "Bindery/Shipping Ticket";


            #endregion UI on CR object editing (non-DB dependant)

            #endregion crystal report config end




            //here is where i need to clean up lots, make functions, sql queryies, DT and DS manipualtion, pass the job number, etc 
            //main code for program functionallity
            #region DB connection 


            //here is where i iwll be connecting to DB's (all queries will need access to basically my connection properties globals)
            #region global connection properties

            //connection string for DB
            string connectStr = "DSN=Progress11;uid=Bob;pwd=Orchard";

            //open th econnection and error check
            OdbcConnection dbConn = new OdbcConnection(connectStr);

            try
            {
                dbConn.Open();
            }
            catch (Exception ex)
            {

                string error = ex + " : DB error cannot connect";

                ErrorLog(error);

            }
            #endregion global connection propeties




            //here is where i can change the UI of the report based on database data
            //ex) show word nailing on report if job# has a 810 tag associated with it
            #region UI crystal report editing (DB dependant)


            #region Header

            //set the job numbers from iuser input
            CrystalDecisions.CrystalReports.Engine.TextObject jobNum1;
            jobNum1 = cryrpt.ReportDefinition.ReportObjects["jobNum"] as TextObject;
            jobNum1.Text = jobNumberUser;

            CrystalDecisions.CrystalReports.Engine.TextObject jobNum2;
            jobNum2 = cryrpt.ReportDefinition.ReportObjects["jobNum2"] as TextObject;
            jobNum2.Text = jobNumberUser;



            String headerJob = "SELECT \"Job-Desc\", \"Date-Promised\", \"Sales-Rep-ID\", \"CSR-ID\", \"" +
                "PO-Number\", \"Overs-Allowed\", \"Last-Estimate-ID\", \"Quantity-Ordered\", \"Contact-Name\", \"Date-Entered\", \"Cust-ID-Ordered-by\"" +
                " FROM PUB.JOB WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtHeader = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter header = new OdbcDataAdapter(headerJob, dbConn);
                header.Fill(dtHeader);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //set all text objects to the data from datatable

            //job descriptions
            CrystalDecisions.CrystalReports.Engine.TextObject jobDesc;
            jobDesc = cryrpt.ReportDefinition.ReportObjects["jobDesc"] as TextObject;
            jobDesc.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

            CrystalDecisions.CrystalReports.Engine.TextObject jobDesc2;
            jobDesc2 = cryrpt.ReportDefinition.ReportObjects["jobDesc2"] as TextObject;
            jobDesc2.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

            //date promised
            CrystalDecisions.CrystalReports.Engine.TextObject dateProm;
            dateProm = cryrpt.ReportDefinition.ReportObjects["dateProm"] as TextObject;
            dateProm.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

            CrystalDecisions.CrystalReports.Engine.TextObject dateProm2;
            dateProm2 = cryrpt.ReportDefinition.ReportObjects["dateProm2"] as TextObject;
            dateProm2.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

            //qty
            CrystalDecisions.CrystalReports.Engine.TextObject qty;
            qty = cryrpt.ReportDefinition.ReportObjects["qty"] as TextObject;
            qty.Text = dtHeader.Rows[0]["Quantity-Ordered"].ToString();

            CrystalDecisions.CrystalReports.Engine.TextObject qty2;
            qty2 = cryrpt.ReportDefinition.ReportObjects["qty2"] as TextObject;
            qty2.Text = dtHeader.Rows[0]["Quantity-Ordered"].ToString();

            //job contact
            CrystalDecisions.CrystalReports.Engine.TextObject jobContact;
            jobContact = cryrpt.ReportDefinition.ReportObjects["contactName"] as TextObject;
            jobContact.Text = dtHeader.Rows[0]["Contact-Name"].ToString();

            //job date entered
            CrystalDecisions.CrystalReports.Engine.TextObject jobDE;
            jobDE = cryrpt.ReportDefinition.ReportObjects["jobDE"] as TextObject;
            jobDE.Text = dtHeader.Rows[0]["Date-Entered"].ToString();

            //over allowed
            CrystalDecisions.CrystalReports.Engine.TextObject OA;
            OA = cryrpt.ReportDefinition.ReportObjects["jobOA"] as TextObject;
            OA.Text = dtHeader.Rows[0]["Overs-Allowed"].ToString();

            //po num
            CrystalDecisions.CrystalReports.Engine.TextObject PO;
            PO = cryrpt.ReportDefinition.ReportObjects["poNum"] as TextObject;
            PO.Text = dtHeader.Rows[0]["PO-Number"].ToString();

            //estimate
            string est = dtHeader.Rows[0]["Last-Estimate-ID"].ToString().Insert(6, "-");
            CrystalDecisions.CrystalReports.Engine.TextObject estimate;
            estimate = cryrpt.ReportDefinition.ReportObjects["estimate"] as TextObject;
            estimate.Text = est;

            //customer query and text objects
            String headerCust = "SELECT \"cust-name\", \"Address-1\", \"Address-2\", \"City\", \"" +
               "State\", \"Zip\", \"Phone\", \"Address-3\" FROM PUB.cust WHERE \"Cust-code\" = " + dtHeader.Rows[0]["Cust-ID-Ordered-by"].ToString();


            DataTable dtCust = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter cust = new OdbcDataAdapter(headerCust, dbConn);
                cust.Fill(dtCust);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //set the Customer info text objects
            //cust name
            CrystalDecisions.CrystalReports.Engine.TextObject custName;
            custName = cryrpt.ReportDefinition.ReportObjects["custName"] as TextObject;
            custName.Text = dtCust.Rows[0]["cust-name"].ToString();

            CrystalDecisions.CrystalReports.Engine.TextObject custName2;
            custName2 = cryrpt.ReportDefinition.ReportObjects["custName2"] as TextObject;
            custName2.Text = dtCust.Rows[0]["cust-name"].ToString();

            //address -> add 1 and 2 and 3 combined
            CrystalDecisions.CrystalReports.Engine.TextObject custAdd;
            custAdd = cryrpt.ReportDefinition.ReportObjects["custAddress"] as TextObject;
            custAdd.Text = dtCust.Rows[0]["Address-1"].ToString() + " " + dtCust.Rows[0]["Address-2"].ToString() + " " + dtCust.Rows[0]["Address-3"].ToString();

            //city state zip customer
            CrystalDecisions.CrystalReports.Engine.TextObject custCSZ;
            custCSZ = cryrpt.ReportDefinition.ReportObjects["custCSZ"] as TextObject;
            custCSZ.Text = dtCust.Rows[0]["City"].ToString() + " " + dtCust.Rows[0]["State"].ToString() + " " + dtCust.Rows[0]["Zip"].ToString();

            //customerPhone
            CrystalDecisions.CrystalReports.Engine.TextObject custPhone;
            custPhone = cryrpt.ReportDefinition.ReportObjects["custPhone"] as TextObject;
            custPhone.Text = dtCust.Rows[0]["Phone"].ToString();

            //sales agent query and txt obj change
            //why this does not work i have no fkin clue, making stack overflwo see what the brians can thunk up
            String headerSalesAgent = "SELECT \"Sales-Agent-Name\" FROM PUB.\"sales-agent\" WHERE \"Sales-agent-id\" = " + "'" + dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "'";

            // String headerSalesAgent = "SELECT \"Sales-agent-id\" , \"Sales-Agent-Name\" FROM PUB.\"sales-agent\"";

            DataTable dtSalesAgent = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter salesAgent = new OdbcDataAdapter(headerSalesAgent, dbConn);
                salesAgent.Fill(dtSalesAgent);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }


            //sales agent name
            CrystalDecisions.CrystalReports.Engine.TextObject salesRep;
            salesRep = cryrpt.ReportDefinition.ReportObjects["salesRep"] as TextObject;
            salesRep.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "-" + dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();

            //sales rep ID for billing
            CrystalDecisions.CrystalReports.Engine.TextObject salesRep2;
            salesRep2 = cryrpt.ReportDefinition.ReportObjects["salesRep2"] as TextObject;
            salesRep2.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString();// + "-" + dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();

            //csr query and txt obj change
            String headerCSR = "SELECT \"CSR-Name\" FROM PUB.CSR WHERE \"CSR-ID\"=" + "'" + dtHeader.Rows[0]["CSR-ID"].ToString() + "'";

            DataTable dtCsr = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter csrAdap = new OdbcDataAdapter(headerCSR, dbConn);
                csrAdap.Fill(dtCsr);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            CrystalDecisions.CrystalReports.Engine.TextObject csr;
            csr = cryrpt.ReportDefinition.ReportObjects["csr"] as TextObject;
            csr.Text = dtHeader.Rows[0]["CSR-ID"].ToString() + "-" + dtCsr.Rows[0]["CSR-Name"].ToString();

            #endregion Header


            #region 810 tag check - show MAILING
            //DataTable for all UI db-depenedant editing
            DataTable dtEdit = new DataTable();


            String query810Tag = "SELECT \"Work-Center-ID\", \"TagStatus-ID\" FROM PUB.ScheduleByJob WHERE \"Job-ID\"=" + jobNumberUser;

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adap810Tag = new OdbcDataAdapter(query810Tag, dbConn);
                adap810Tag.Fill(dtEdit);
            }
            catch (Exception ex)
            {


                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //here is needed for 810 chck
            bool check = false;

            foreach (DataRow dr in dtEdit.Rows)
            {
                //do nothing 
                if (dr["Work-Center-ID"].ToString().Contains("810"))
                {

                    check = true;
                }

            }//end foreach

            //no check to see if 810 tag is present
            if (!check)
            {

                CrystalDecisions.CrystalReports.Engine.TextObject txtObj;
                txtObj = cryrpt.ReportDefinition.ReportObjects["txtMail"] as TextObject;
                txtObj.Text = "";
            }//end check for 810
            #endregion 810 tag check - show MAILING

            //can use same data from above query and dataTable to get the the job status and description
            //ex) 50d status and it's description is ready to run on digital press
            #region tag status grab (no query, re-use data gathered from 810 check)

            try
            {
                //need to base of the 900 tag's -> tag status
                string tagStat = "";

                foreach (DataRow dr in dtEdit.Rows)
                {
                    //do nothing 
                    if (dr["Work-Center-ID"].ToString() == "900")
                    {

                        tagStat = dr["TagStatus-ID"].ToString();
                    }

                }//end foreach


                //grab first record's 'TagStatus-ID" and set status to it ex) 50d, 09, etc
                var statusObj = cryrpt.ReportDefinition.ReportObjects["status"] as TextObject;
                statusObj.Text = tagStat;

                var statusDescObj = cryrpt.ReportDefinition.ReportObjects["statusDesc"] as TextObject;

                if (tagStat == "01")
                {
                    statusDescObj.Text = "Outside Service";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }

                if (tagStat == "02")
                {
                    statusDescObj.Text = "On Hold";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "05")
                {
                    statusDescObj.Text = "Digital Mailing/Need Run Files";
                }
                if (tagStat == "07")
                {
                    statusDescObj.Text = "Long Term Project (In-House)";
                }
                if (tagStat == "08")
                {
                    statusDescObj.Text = "Long Term Project (out on proof)";
                }
                if (tagStat == "09")
                {
                    statusDescObj.Text = "In Pre Press Production";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "09-R")
                {
                    statusDescObj.Text = "Art corrections after proof";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "18")
                {
                    statusDescObj.Text = "Out on Random Proof";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "20")
                {
                    statusDescObj.Text = "Ready to Strip and Plate";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "20p")
                {
                    statusDescObj.Text = "Approved-Waiting for Mock up";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "21")
                {
                    statusDescObj.Text = "Rerun-Pill Plates";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "29")
                {
                    statusDescObj.Text = "Ready to Plate";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50")
                {
                    statusDescObj.Text = "Press-Running/Plated";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50d" || tagStat == "50D")
                {
                    statusDescObj.Text = "Ready to Run Digital Press";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50e" || tagStat == "50E")
                {
                    statusDescObj.Text = "Ready to Run Digital Press ENVELOPE";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "70")
                {
                    statusDescObj.Text = "Printed and in Bindery";
                    statusObj.Color = Color.Brown;
                    statusDescObj.Color = Color.Brown;
                }
                if (tagStat == "72")
                {
                    statusDescObj.Text = "Monthly DSF billing jobs";
                }
                if (tagStat == "75")
                {
                    statusDescObj.Text = "Waiting for Mailing Data";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "80")
                {
                    statusDescObj.Text = "Bindery Done-Ready for Mailing";
                    statusObj.Color = Color.Blue;
                    statusDescObj.Color = Color.Blue;
                }
                if (tagStat == "82")
                {
                    statusDescObj.Text = "Running on Netjet";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "85")
                {
                    statusDescObj.Text = "Mail Complete/Need Paperwork";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;

                }
                if (tagStat == "88")
                {
                    statusDescObj.Text = "Mail Complete - Ready to Mail";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;

                }
                if (tagStat == "88p")
                {
                    statusDescObj.Text = "PARTIAL mail/Ready to Deliver";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;
                }
                if (tagStat == "90")
                {
                    statusDescObj.Text = "Job Completed-Ready to Deliver";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;
                }
                if (tagStat == "92")
                {
                    statusDescObj.Text = "Being Delivered";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
                if (tagStat == "95")
                {
                    statusDescObj.Text = "Delivered";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
                if (tagStat == "97")
                {
                    statusDescObj.Text = "DSF Jobs To Be Billed";

                }
                if (tagStat == "97b" || tagStat == "97B")
                {
                    statusDescObj.Text = "DSF jobs Already Billed";
                }
                if (tagStat == "29")
                {
                    statusDescObj.Text = "Job Close Pending";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
            }//end try
            catch (Exception ex)
            {

                string error = ex + " : Tag Status update error check code";

                ErrorLog(error);

            }

            #endregion tag status

            #endregion UI


            //sub-reports section
            #region sub report creater

            //ObjectIndexCheck(cryrpt);

            //Press/Prepress sub rereports - Mailing Version, Mailing Free Feilds, Job Notes, Job Free Feilds, PO Req, PO line,
            //Forms, press, Stock

            #region Mailing Version subReport
            string queryMailVersion = "SELECT \"Free-Field-Char\" FROM PUB.MailingVersionFreeField WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtMailVersion = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapMailVersion = new OdbcDataAdapter(queryMailVersion, dbConn);
                adapMailVersion.Fill(dtMailVersion);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - Mailing Version FF report";

                ErrorLog(error);
            }

            //and change second column name to match CR report value "FFvalue"
            dtMailVersion.Columns["Free-Field-Char"].ColumnName = "FFvalue";

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtMailVersion.Rows.Count != 0)
            {

                cryrpt.Subreports[7].DataSourceConnections.Clear();
                cryrpt.Subreports[7].SetDataSource(dtMailVersion);

            }
            else
            {

                string subMailVersionFF = "subMailVersion";
                HideSubs(cryrpt, subMailVersionFF);

            }

            #endregion Mailing Version subReport

            #region Mailing Free Fields subReport
            string queryMailFF = "SELECT \"Free-Field-Char\" FROM PUB.MailingFreeField WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtMailFF = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapMailFF = new OdbcDataAdapter(queryMailFF, dbConn);
                adapMailFF.Fill(dtMailFF);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - Mailing FF report";

                ErrorLog(error);
            }

            //and change second column name to match CR report value "FFvalue"
            dtMailFF.Columns["Free-Field-Char"].ColumnName = "FFval";

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtMailFF.Rows.Count != 0)
            {
                cryrpt.Subreports[6].DataSourceConnections.Clear();
                cryrpt.Subreports[6].SetDataSource(dtMailFF);

            }
            else
            {

                string subMailFF = "subMailFreeFields";
                HideSubs(cryrpt, subMailFF);

            }

            #endregion Mailing Free Fields subReport

            #region Job Notes subReport
            string queryJobNotes = "SELECT \"SpecCategory-ID\", Description, \"Created-By\", \"Comment-Date\", \"Update-date\" FROM PUB.JobComments WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtJobNotes = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapJobNotes = new OdbcDataAdapter(queryJobNotes, dbConn);
                adapJobNotes.Fill(dtJobNotes);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Job Notes report";

                ErrorLog(error);
            }

            //change the names
            dtJobNotes.Columns["SpecCategory-ID"].ColumnName = "SpecID";
            dtJobNotes.Columns["Created-By"].ColumnName = "EnterBy";
            dtJobNotes.Columns["Comment-Date"].ColumnName = "DateEntered";
            dtJobNotes.Columns["Update-date"].ColumnName = "DateUpdated";

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtJobNotes.Rows.Count != 0)
            {
                cryrpt.Subreports[5].DataSourceConnections.Clear();
                cryrpt.Subreports[5].SetDataSource(dtJobNotes);

            }
            else
            {

                string subJobComments = "subJobNotes";
                HideSubs(cryrpt, subJobComments);

            }


            #endregion Job Notes subReports




            #region Alterations subRpt
            string queryAlt = "SELECT * FROM PUB.Alterations WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtAlt = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapAlt = new OdbcDataAdapter(queryAlt, dbConn);
                adapAlt.Fill(dtAlt);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Alterations report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtAlt.Rows.Count != 0)
            {
                cryrpt.Subreports[0].DataSourceConnections.Clear();
                cryrpt.Subreports[0].SetDataSource(dtAlt);

            }
            else
            {

                string subAlt = "subAlterations";
                HideSubs(cryrpt, subAlt);

            }

            #endregion alterations subRpt

            #region Job free fields subRPt
            string queryFF = "SELECT \"Free-Field-Char\", \"Free-Field-Decimal\" FROM PUB.JobFreeField WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtFF = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapFF = new OdbcDataAdapter(queryFF, dbConn);
                adapFF.Fill(dtFF);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Free fields report";

                ErrorLog(error);
            }

            //here is where i need to convert decimal -> free-fieldchar
            string lastJob = dtFF.Rows[1]["Free-Field-Decimal"].ToString();
            dtFF.Rows[1]["Free-Field-Char"] = lastJob;

            dtFF.Columns.Remove("Free-Field-Decimal");

            //also check if empty exists it is empty hideSubs it
            if (dtFF.Rows.Count != 0)
            {
                cryrpt.Subreports[4].DataSourceConnections.Clear();
                cryrpt.Subreports[4].SetDataSource(dtFF);

            }
            else
            {

                string subFF = "subJobFreeFields";
                HideSubs(cryrpt, subFF);

            }


            #endregion job free fields subRpt

            #region PO req subRpt
            string queryPOreq = "SELECT \"Req-Number\", \"Supplier-ID\", \"Supplier-Name\", \"Inventory-Item-ID\", \"Item-Desc\", \"Qty-Requisitioned\", \"Unit-Meas-Purchase\", \"Unit-Cost-Purchase\", \"Req-Status\" FROM PUB.PORequisition WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtPOreq = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPOreq = new OdbcDataAdapter(queryPOreq, dbConn);
                adapPOreq.Fill(dtPOreq);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -PO Req report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPOreq.Rows.Count != 0)
            {
                cryrpt.Subreports[9].DataSourceConnections.Clear();
                cryrpt.Subreports[9].SetDataSource(dtPOreq);

            }
            else
            {

                string subPOreq = "subPOReq";
                HideSubs(cryrpt, subPOreq);

            }

            #endregion PO req subRpt


            #region PO line info subRpt
            string queryPOLine = "SELECT \"PO-Number\", \"Line-Status\", \"Date-PO\", \"Date-Last-Receipt\", \"Item-Desc\", \"Qty-Received-Purchase\" FROM PUB.POLine WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtPOLine = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPOLine = new OdbcDataAdapter(queryPOLine, dbConn);
                adapPOLine.Fill(dtPOLine);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -PO Line report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPOLine.Rows.Count != 0)
            {
                cryrpt.Subreports[8].DataSourceConnections.Clear();
                cryrpt.Subreports[8].SetDataSource(dtPOLine);

            }
            else
            {

                string subPOLine = "subPOLine";
                HideSubs(cryrpt, subPOLine);

            }
            #endregion Po Line info Subrpt

            #region ship to data subRpt
            string queryShipTo = "SELECT \"Ship-To-Name\", \"Ship-To-Address1\", \"Ship-To-Address2\", \"Ship-To-Address3\", \"Ship-To-City\", \"Ship-To-State\",\"Ship-To-Zip\",\"Ship-To-Attention\"" +
                ",\"Instructions\", \"Requested-Ship-Date\" ,\"Requested-Quantity\", \"Requested-Number-Packages\", FAX, Phone, \"Ship-To-Country\", \"Shipment-Method-ID\"  FROM PUB.JobShipTo WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtShipTo = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapShipTo = new OdbcDataAdapter(queryShipTo, dbConn);
                adapShipTo.Fill(dtShipTo);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -ship to report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtShipTo.Rows.Count != 0)
            {
                cryrpt.Subreports[12].DataSourceConnections.Clear();
                cryrpt.Subreports[12].SetDataSource(dtShipTo);

            }
            else
            {

                string subShipTo = "subShipTo";
                HideSubs(cryrpt, subShipTo);

            }

            #endregion ship to data subRpt

            #region form spec subRpt
            string querySpecForm = "SELECT \"Form-ID\",  \"Form-Desc\", \"Number-Pages\", Width, Length, \"Number-forms\", \"Number-Out\", Imposition, Notes FROM PUB.JobSpecForm WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtSpecForm = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapSpecForm = new OdbcDataAdapter(querySpecForm, dbConn);
                adapSpecForm.Fill(dtSpecForm);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Spec Form report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtSpecForm.Rows.Count != 0)
            {
                cryrpt.Subreports[3].DataSourceConnections.Clear();
                cryrpt.Subreports[3].SetDataSource(dtSpecForm);

            }
            else
            {

                string subFormSpec = "subFormSpecs";
                HideSubs(cryrpt, subFormSpec);

            }


            #endregion form spec subRpt

            #region form Notes subrpt
            string queryFormNotes = "SELECT \"Form-ID\", Description FROM PUB.JobComments WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtFormNotes = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapFormNotes = new OdbcDataAdapter(queryFormNotes, dbConn);
                adapFormNotes.Fill(dtFormNotes);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Form Notes report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtFormNotes.Rows.Count != 0)
            {
                cryrpt.Subreports[2].DataSourceConnections.Clear();
                cryrpt.Subreports[2].SetDataSource(dtFormNotes);

            }
            else
            {

                string subFormNotes = "subFormNotes";
                HideSubs(cryrpt, subFormNotes);

            }


            #endregion form notes subRpt

            #region prepress rpt
            string queryPrepress = "SELECT \"Form-ID\", \"Spec-Desc\", \"Work-Center-ID\", \"Operation-ID\", Quantity, Hours FROM PUB.JobSpecPrepress WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtPrepress = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPrepress = new OdbcDataAdapter(queryPrepress, dbConn);
                adapPrepress.Fill(dtPrepress);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Prepress report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPrepress.Rows.Count != 0)
            {
                cryrpt.Subreports[10].DataSourceConnections.Clear();
                cryrpt.Subreports[10].SetDataSource(dtPrepress);

            }
            else
            {

                string subPrepress = "subPrepress";
                HideSubs(cryrpt, subPrepress);

            }
            #endregion prepress rpt

            #region press rpt
            string queryPress = "SELECT \"Form-ID\", \"Operation-Type\",  \"Spec-Desc\", \"Qty-Makeready\", \"Qty-Net\", \"Qty-Spoilage\", Width, Length, \"Hours-Makeready\", \"Hours-Run\", \"Hours-Cleanup\" FROM PUB.JobSpecOperation WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtPress = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPress = new OdbcDataAdapter(queryPress, dbConn);
                adapPress.Fill(dtPress);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Press report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPress.Rows.Count != 0)
            {
                cryrpt.Subreports[11].DataSourceConnections.Clear();
                cryrpt.Subreports[11].SetDataSource(dtPress);

            }
            else
            {

                string subPress = "subPress";
                HideSubs(cryrpt, subPress);

            }

            #endregion press rpt

            #region stock rpt
            string queryStock = "SELECT \"Form-ID\", \"Gross-Sheets\", \"Gross-Weight\",  Notes,  \"Inventory-Item-ID\", \"Inventory-Item-Desc\", \"Material-Type\", Length, Width FROM PUB.JobSpecMaterial WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtStock = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapStock = new OdbcDataAdapter(queryStock, dbConn);
                adapStock.Fill(dtStock);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Stock report";

                ErrorLog(error);
            }

            //need to get page-grain new query
            string queryPageGrain = "SELECT \"Page-Grain\" FROM PUB.JobSpecForm WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtPG = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPG = new OdbcDataAdapter(queryPageGrain, dbConn);
                adapPG.Fill(dtPG);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Stock report -PG";

                ErrorLog(error);
            }

            dtStock.Columns.Add("Page-Grain");
            string val = dtPG.Rows[0]["Page-Grain"].ToString();

            foreach (DataRow dr in dtStock.Rows)
            {

                dr["Page-Grain"] = val;

            }

            //also check if empty exists it is empty hideSubs it
            if (dtStock.Rows.Count != 0)
            {
                cryrpt.Subreports[13].DataSourceConnections.Clear();
                cryrpt.Subreports[13].SetDataSource(dtStock);

            }
            else
            {

                string subStock = "subStock";
                HideSubs(cryrpt, subStock);

            }

            #endregion stock rpt

            #region bindery rpt
            string queryBind = "SELECT \"Form-ID\", \"Operation-Type\",  \"Spec-Desc\", \"Qty-Makeready\", \"Qty-Net\", \"Qty-Spoilage\", Width, Length, \"Hours-Makeready\", \"Hours-Run\", \"Hours-Cleanup\", \"Number-Out-for-Oper\" FROM PUB.JobSpecOperation WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtBind = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapBind = new OdbcDataAdapter(queryBind, dbConn);
                adapBind.Fill(dtBind);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Bindery report";

                ErrorLog(error);
            }

            ObjectIndexCheck(cryrpt);

            //also check if empty exists it is empty hideSubs it
            if (dtBind.Rows.Count != 0)
            {
                cryrpt.Subreports[1].DataSourceConnections.Clear();
                cryrpt.Subreports[1].SetDataSource(dtBind);

            }
            else
            {

                string subBindery = "subBindery";
                HideSubs(cryrpt, subBindery);

            }


            #endregion bindery rpt

            #endregion sub report creation


            dbConn.Close();
            #endregion DB close connection



            #region display rpt

            LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;
            LaunchOrigin.crystalReportViewer1.ReportSource = cryrpt;

            LaunchOrigin.crystalReportViewer1.Refresh(); //here is where i get the prompt DB login

            label3.Text = "Report loaded.";
            #endregion display rpt


        }//end bidnery
        #endregion bindery ticket






        #region full report
        //full report
        private void button11_Click_1(object sender, EventArgs e)
        {
            //here is where i will put all Form {button/label/etc} editing
            //Notes: can NOT edit crystal report stuff here; have not created crystal reprort object yet, that will be later
            #region label and buttons UI
            label3.Text = "Please wait report loading..";

            jobNumberUser = textBox1.Text;


            button1.FlatAppearance.BorderSize = 0;
            button1.FlatAppearance.BorderColor = Color.Black;

            button8.FlatAppearance.BorderSize = 0;
            button8.FlatAppearance.BorderColor = Color.Black;

            button3.FlatAppearance.BorderSize = 0;
            button3.FlatAppearance.BorderColor = Color.Black;

            button9.FlatAppearance.BorderSize = 0;
            button9.FlatAppearance.BorderColor = Color.Black;


            button2.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.BorderColor = Color.Black;


            button11.FlatAppearance.BorderSize = 5;
            button11.FlatAppearance.BorderColor = Color.Black;

            //timestamp when a button is clicked (report is ran), so user knows what time the current report on screen was ran
            label5.Text = DateTime.Now.ToString();

            #endregion label/buttons UI end




            //here is where i will put the creation and manipualtion of the crystal report object
            #region start crystal report config:

            #region connection and CR object properties/settings
            //rerport object



            CrystalReport2 cryrpt = new CrystalReport2();

            cryrpt.DataSourceConnections.Clear();  //clear the connections (will popualte with fresh sql query defined data

            //set the databse login info, use twice first one is to login into sql server

            //getting write only error


            cryrpt.SetDatabaseLogon("Bob", "Orchard", "monarch18", "gams1");
            cryrpt.SetDatabaseLogon("Bob", "Orchard"); //this one for that annoying prompt to login into database


            //this does not error out!?
            //this opens connection to DB with the login info down..
            ConnectionInfo crconnectioninfo = new ConnectionInfo();
            crconnectioninfo.ServerName = "monarch18";
            crconnectioninfo.DatabaseName = "gams1";
            crconnectioninfo.UserID = "Bob";
            crconnectioninfo.Password = "Orchard";


            //try our best to never have paramter panel pop-up, works like 95% of time
            //error here as well?
            //LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;

            #endregion connectiona and CR object properties/settings

            #region UI on CR object editing (non-DB dependant)

            //change text object, tell user what ticket the are curentlly running/looking at
            CrystalDecisions.CrystalReports.Engine.TextObject txtObj2;
            txtObj2 = cryrpt.ReportDefinition.ReportObjects["ticketName"] as TextObject;
            txtObj2.Text = "Full Ticket";


            #endregion UI on CR object editing (non-DB dependant)

            #endregion crystal report config end




            //here is where i need to clean up lots, make functions, sql queryies, DT and DS manipualtion, pass the job number, etc 
            //main code for program functionallity
            #region DB connection 


            //here is where i iwll be connecting to DB's (all queries will need access to basically my connection properties globals)
            #region global connection properties

            //connection string for DB
            string connectStr = "DSN=Progress11;uid=Bob;pwd=Orchard";

            //open th econnection and error check
            OdbcConnection dbConn = new OdbcConnection(connectStr);

            try
            {
                dbConn.Open();
            }
            catch (Exception ex)
            {

                string error = ex + " : DB error cannot connect";

                ErrorLog(error);

            }
            #endregion global connection propeties




            //here is where i can change the UI of the report based on database data
            //ex) show word nailing on report if job# has a 810 tag associated with it
            #region UI crystal report editing (DB dependant)


            #region Header

            //set the job numbers from iuser input
            CrystalDecisions.CrystalReports.Engine.TextObject jobNum1;
            jobNum1 = cryrpt.ReportDefinition.ReportObjects["jobNum"] as TextObject;
            jobNum1.Text = jobNumberUser;

            CrystalDecisions.CrystalReports.Engine.TextObject jobNum2;
            jobNum2 = cryrpt.ReportDefinition.ReportObjects["jobNum2"] as TextObject;
            jobNum2.Text = jobNumberUser;



            String headerJob = "SELECT \"Job-Desc\", \"Date-Promised\", \"Sales-Rep-ID\", \"CSR-ID\", \"" +
                "PO-Number\", \"Overs-Allowed\", \"Last-Estimate-ID\", \"Quantity-Ordered\", \"Contact-Name\", \"Date-Entered\", \"Cust-ID-Ordered-by\"" +
                " FROM PUB.JOB WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtHeader = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter header = new OdbcDataAdapter(headerJob, dbConn);
                header.Fill(dtHeader);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //set all text objects to the data from datatable

            //job descriptions
            CrystalDecisions.CrystalReports.Engine.TextObject jobDesc;
            jobDesc = cryrpt.ReportDefinition.ReportObjects["jobDesc"] as TextObject;
            jobDesc.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

            CrystalDecisions.CrystalReports.Engine.TextObject jobDesc2;
            jobDesc2 = cryrpt.ReportDefinition.ReportObjects["jobDesc2"] as TextObject;
            jobDesc2.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

            //date promised
            CrystalDecisions.CrystalReports.Engine.TextObject dateProm;
            dateProm = cryrpt.ReportDefinition.ReportObjects["dateProm"] as TextObject;
            dateProm.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

            CrystalDecisions.CrystalReports.Engine.TextObject dateProm2;
            dateProm2 = cryrpt.ReportDefinition.ReportObjects["dateProm2"] as TextObject;
            dateProm2.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

            //qty
            CrystalDecisions.CrystalReports.Engine.TextObject qty;
            qty = cryrpt.ReportDefinition.ReportObjects["qty"] as TextObject;
            qty.Text = dtHeader.Rows[0]["Quantity-Ordered"].ToString();

            CrystalDecisions.CrystalReports.Engine.TextObject qty2;
            qty2 = cryrpt.ReportDefinition.ReportObjects["qty2"] as TextObject;
            qty2.Text = dtHeader.Rows[0]["Quantity-Ordered"].ToString();

            //job contact
            CrystalDecisions.CrystalReports.Engine.TextObject jobContact;
            jobContact = cryrpt.ReportDefinition.ReportObjects["contactName"] as TextObject;
            jobContact.Text = dtHeader.Rows[0]["Contact-Name"].ToString();

            //job date entered
            CrystalDecisions.CrystalReports.Engine.TextObject jobDE;
            jobDE = cryrpt.ReportDefinition.ReportObjects["jobDE"] as TextObject;
            jobDE.Text = dtHeader.Rows[0]["Date-Entered"].ToString();

            //over allowed
            CrystalDecisions.CrystalReports.Engine.TextObject OA;
            OA = cryrpt.ReportDefinition.ReportObjects["jobOA"] as TextObject;
            OA.Text = dtHeader.Rows[0]["Overs-Allowed"].ToString();

            //po num
            CrystalDecisions.CrystalReports.Engine.TextObject PO;
            PO = cryrpt.ReportDefinition.ReportObjects["poNum"] as TextObject;
            PO.Text = dtHeader.Rows[0]["PO-Number"].ToString();

            //estimate
            string est = dtHeader.Rows[0]["Last-Estimate-ID"].ToString().Insert(6, "-");
            CrystalDecisions.CrystalReports.Engine.TextObject estimate;
            estimate = cryrpt.ReportDefinition.ReportObjects["estimate"] as TextObject;
            estimate.Text = est;

            //customer query and text objects
            String headerCust = "SELECT \"cust-name\", \"Address-1\", \"Address-2\", \"City\", \"" +
               "State\", \"Zip\", \"Phone\", \"Address-3\" FROM PUB.cust WHERE \"Cust-code\" = " + dtHeader.Rows[0]["Cust-ID-Ordered-by"].ToString();


            DataTable dtCust = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter cust = new OdbcDataAdapter(headerCust, dbConn);
                cust.Fill(dtCust);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //set the Customer info text objects
            //cust name
            CrystalDecisions.CrystalReports.Engine.TextObject custName;
            custName = cryrpt.ReportDefinition.ReportObjects["custName"] as TextObject;
            custName.Text = dtCust.Rows[0]["cust-name"].ToString();

            CrystalDecisions.CrystalReports.Engine.TextObject custName2;
            custName2 = cryrpt.ReportDefinition.ReportObjects["custName2"] as TextObject;
            custName2.Text = dtCust.Rows[0]["cust-name"].ToString();

            //address -> add 1 and 2 and 3 combined
            CrystalDecisions.CrystalReports.Engine.TextObject custAdd;
            custAdd = cryrpt.ReportDefinition.ReportObjects["custAddress"] as TextObject;
            custAdd.Text = dtCust.Rows[0]["Address-1"].ToString() + " " + dtCust.Rows[0]["Address-2"].ToString() + " " + dtCust.Rows[0]["Address-3"].ToString();

            //city state zip customer
            CrystalDecisions.CrystalReports.Engine.TextObject custCSZ;
            custCSZ = cryrpt.ReportDefinition.ReportObjects["custCSZ"] as TextObject;
            custCSZ.Text = dtCust.Rows[0]["City"].ToString() + " " + dtCust.Rows[0]["State"].ToString() + " " + dtCust.Rows[0]["Zip"].ToString();

            //customerPhone
            CrystalDecisions.CrystalReports.Engine.TextObject custPhone;
            custPhone = cryrpt.ReportDefinition.ReportObjects["custPhone"] as TextObject;
            custPhone.Text = dtCust.Rows[0]["Phone"].ToString();

            //sales agent query and txt obj change
            //why this does not work i have no fkin clue, making stack overflwo see what the brians can thunk up
            String headerSalesAgent = "SELECT \"Sales-Agent-Name\" FROM PUB.\"sales-agent\" WHERE \"Sales-agent-id\" = " + "'" + dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "'";

            // String headerSalesAgent = "SELECT \"Sales-agent-id\" , \"Sales-Agent-Name\" FROM PUB.\"sales-agent\"";

            DataTable dtSalesAgent = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter salesAgent = new OdbcDataAdapter(headerSalesAgent, dbConn);
                salesAgent.Fill(dtSalesAgent);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }


            //sales agent name
            CrystalDecisions.CrystalReports.Engine.TextObject salesRep;
            salesRep = cryrpt.ReportDefinition.ReportObjects["salesRep"] as TextObject;
            salesRep.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "-" + dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();

            //sales rep ID for billing
            CrystalDecisions.CrystalReports.Engine.TextObject salesRep2;
            salesRep2 = cryrpt.ReportDefinition.ReportObjects["salesRep2"] as TextObject;
            salesRep2.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString();// + "-" + dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();

            //csr query and txt obj change
            String headerCSR = "SELECT \"CSR-Name\" FROM PUB.CSR WHERE \"CSR-ID\"=" + "'" + dtHeader.Rows[0]["CSR-ID"].ToString() + "'";

            DataTable dtCsr = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter csrAdap = new OdbcDataAdapter(headerCSR, dbConn);
                csrAdap.Fill(dtCsr);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            CrystalDecisions.CrystalReports.Engine.TextObject csr;
            csr = cryrpt.ReportDefinition.ReportObjects["csr"] as TextObject;
            csr.Text = dtHeader.Rows[0]["CSR-ID"].ToString() + "-" + dtCsr.Rows[0]["CSR-Name"].ToString();

            #endregion Header


            #region 810 tag check - show MAILING
            //DataTable for all UI db-depenedant editing
            DataTable dtEdit = new DataTable();


            String query810Tag = "SELECT \"Work-Center-ID\", \"TagStatus-ID\" FROM PUB.ScheduleByJob WHERE \"Job-ID\"=" + jobNumberUser;

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adap810Tag = new OdbcDataAdapter(query810Tag, dbConn);
                adap810Tag.Fill(dtEdit);
            }
            catch (Exception ex)
            {


                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //here is needed for 810 chck
            bool check = false;

            foreach (DataRow dr in dtEdit.Rows)
            {
                //do nothing 
                if (dr["Work-Center-ID"].ToString().Contains("810"))
                {

                    check = true;
                }

            }//end foreach

            //no check to see if 810 tag is present
            if (!check)
            {

                CrystalDecisions.CrystalReports.Engine.TextObject txtObj;
                txtObj = cryrpt.ReportDefinition.ReportObjects["txtMail"] as TextObject;
                txtObj.Text = "";
            }//end check for 810
            #endregion 810 tag check - show MAILING

            //can use same data from above query and dataTable to get the the job status and description
            //ex) 50d status and it's description is ready to run on digital press
            #region tag status grab (no query, re-use data gathered from 810 check)

            try
            {
                //need to base of the 900 tag's -> tag status
                string tagStat = "";

                foreach (DataRow dr in dtEdit.Rows)
                {
                    //do nothing 
                    if (dr["Work-Center-ID"].ToString() == "900")
                    {

                        tagStat = dr["TagStatus-ID"].ToString();
                    }

                }//end foreach


                //grab first record's 'TagStatus-ID" and set status to it ex) 50d, 09, etc
                var statusObj = cryrpt.ReportDefinition.ReportObjects["status"] as TextObject;
                statusObj.Text = tagStat;

                var statusDescObj = cryrpt.ReportDefinition.ReportObjects["statusDesc"] as TextObject;

                if (tagStat == "01")
                {
                    statusDescObj.Text = "Outside Service";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }

                if (tagStat == "02")
                {
                    statusDescObj.Text = "On Hold";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "05")
                {
                    statusDescObj.Text = "Digital Mailing/Need Run Files";
                }
                if (tagStat == "07")
                {
                    statusDescObj.Text = "Long Term Project (In-House)";
                }
                if (tagStat == "08")
                {
                    statusDescObj.Text = "Long Term Project (out on proof)";
                }
                if (tagStat == "09")
                {
                    statusDescObj.Text = "In Pre Press Production";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "09-R")
                {
                    statusDescObj.Text = "Art corrections after proof";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "18")
                {
                    statusDescObj.Text = "Out on Random Proof";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "20")
                {
                    statusDescObj.Text = "Ready to Strip and Plate";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "20p")
                {
                    statusDescObj.Text = "Approved-Waiting for Mock up";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "21")
                {
                    statusDescObj.Text = "Rerun-Pill Plates";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "29")
                {
                    statusDescObj.Text = "Ready to Plate";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50")
                {
                    statusDescObj.Text = "Press-Running/Plated";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50d" || tagStat == "50D")
                {
                    statusDescObj.Text = "Ready to Run Digital Press";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50e" || tagStat == "50E")
                {
                    statusDescObj.Text = "Ready to Run Digital Press ENVELOPE";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "70")
                {
                    statusDescObj.Text = "Printed and in Bindery";
                    statusObj.Color = Color.Brown;
                    statusDescObj.Color = Color.Brown;
                }
                if (tagStat == "72")
                {
                    statusDescObj.Text = "Monthly DSF billing jobs";
                }
                if (tagStat == "75")
                {
                    statusDescObj.Text = "Waiting for Mailing Data";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "80")
                {
                    statusDescObj.Text = "Bindery Done-Ready for Mailing";
                    statusObj.Color = Color.Blue;
                    statusDescObj.Color = Color.Blue;
                }
                if (tagStat == "82")
                {
                    statusDescObj.Text = "Running on Netjet";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "85")
                {
                    statusDescObj.Text = "Mail Complete/Need Paperwork";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;

                }
                if (tagStat == "88")
                {
                    statusDescObj.Text = "Mail Complete - Ready to Mail";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;

                }
                if (tagStat == "88p")
                {
                    statusDescObj.Text = "PARTIAL mail/Ready to Deliver";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;
                }
                if (tagStat == "90")
                {
                    statusDescObj.Text = "Job Completed-Ready to Deliver";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;
                }
                if (tagStat == "92")
                {
                    statusDescObj.Text = "Being Delivered";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
                if (tagStat == "95")
                {
                    statusDescObj.Text = "Delivered";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
                if (tagStat == "97")
                {
                    statusDescObj.Text = "DSF Jobs To Be Billed";

                }
                if (tagStat == "97b" || tagStat == "97B")
                {
                    statusDescObj.Text = "DSF jobs Already Billed";
                }
                if (tagStat == "29")
                {
                    statusDescObj.Text = "Job Close Pending";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
            }//end try
            catch (Exception ex)
            {

                string error = ex + " : Tag Status update error check code";

                ErrorLog(error);

            }

            #endregion tag status

            #endregion UI


            //sub-reports section
            #region sub report creater

            //ObjectIndexCheck(cryrpt);

            //Press/Prepress sub rereports - Mailing Version, Mailing Free Feilds, Job Notes, Job Free Feilds, PO Req, PO line,
            //Forms, press, Stock

            #region Mailing Version subReport
            string queryMailVersion = "SELECT \"Free-Field-Char\" FROM PUB.MailingVersionFreeField WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtMailVersion = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapMailVersion = new OdbcDataAdapter(queryMailVersion, dbConn);
                adapMailVersion.Fill(dtMailVersion);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - Mailing Version FF report";

                ErrorLog(error);
            }

            //and change second column name to match CR report value "FFvalue"
            dtMailVersion.Columns["Free-Field-Char"].ColumnName = "FFvalue";

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtMailVersion.Rows.Count != 0)
            {

                cryrpt.Subreports[7].DataSourceConnections.Clear();
                cryrpt.Subreports[7].SetDataSource(dtMailVersion);

            }
            else
            {

                string subMailVersionFF = "subMailVersion";
                HideSubs(cryrpt, subMailVersionFF);

            }

            #endregion Mailing Version subReport

            #region Mailing Free Fields subReport
            string queryMailFF = "SELECT \"Free-Field-Char\" FROM PUB.MailingFreeField WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtMailFF = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapMailFF = new OdbcDataAdapter(queryMailFF, dbConn);
                adapMailFF.Fill(dtMailFF);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - Mailing FF report";

                ErrorLog(error);
            }

            //and change second column name to match CR report value "FFvalue"
            dtMailFF.Columns["Free-Field-Char"].ColumnName = "FFval";

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtMailFF.Rows.Count != 0)
            {
                cryrpt.Subreports[6].DataSourceConnections.Clear();
                cryrpt.Subreports[6].SetDataSource(dtMailFF);

            }
            else
            {

                string subMailFF = "subMailFreeFields";
                HideSubs(cryrpt, subMailFF);

            }

            #endregion Mailing Free Fields subReport

            #region Job Notes subReport
            string queryJobNotes = "SELECT \"SpecCategory-ID\", Description, \"Created-By\", \"Comment-Date\", \"Update-date\" FROM PUB.JobComments WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtJobNotes = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapJobNotes = new OdbcDataAdapter(queryJobNotes, dbConn);
                adapJobNotes.Fill(dtJobNotes);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Job Notes report";

                ErrorLog(error);
            }

            //change the names
            dtJobNotes.Columns["SpecCategory-ID"].ColumnName = "SpecID";
            dtJobNotes.Columns["Created-By"].ColumnName = "EnterBy";
            dtJobNotes.Columns["Comment-Date"].ColumnName = "DateEntered";
            dtJobNotes.Columns["Update-date"].ColumnName = "DateUpdated";

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtJobNotes.Rows.Count != 0)
            {
                cryrpt.Subreports[5].DataSourceConnections.Clear();
                cryrpt.Subreports[5].SetDataSource(dtJobNotes);

            }
            else
            {

                string subJobComments = "subJobNotes";
                HideSubs(cryrpt, subJobComments);

            }


            #endregion Job Notes subReports




            #region Alterations subRpt
            string queryAlt = "SELECT * FROM PUB.Alterations WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtAlt = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapAlt = new OdbcDataAdapter(queryAlt, dbConn);
                adapAlt.Fill(dtAlt);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Alterations report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtAlt.Rows.Count != 0)
            {
                cryrpt.Subreports[0].DataSourceConnections.Clear();
                cryrpt.Subreports[0].SetDataSource(dtAlt);

            }
            else
            {

                string subAlt = "subAlterations";
                HideSubs(cryrpt, subAlt);

            }

            #endregion alterations subRpt

            #region Job free fields subRPt
            string queryFF = "SELECT \"Free-Field-Char\", \"Free-Field-Decimal\" FROM PUB.JobFreeField WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtFF = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapFF = new OdbcDataAdapter(queryFF, dbConn);
                adapFF.Fill(dtFF);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Free fields report";

                ErrorLog(error);
            }

            //here is where i need to convert decimal -> free-fieldchar
            string lastJob = dtFF.Rows[1]["Free-Field-Decimal"].ToString();
            dtFF.Rows[1]["Free-Field-Char"] = lastJob;

            dtFF.Columns.Remove("Free-Field-Decimal");

            //also check if empty exists it is empty hideSubs it
            if (dtFF.Rows.Count != 0)
            {
                cryrpt.Subreports[4].DataSourceConnections.Clear();
                cryrpt.Subreports[4].SetDataSource(dtFF);

            }
            else
            {

                string subFF = "subJobFreeFields";
                HideSubs(cryrpt, subFF);

            }


            #endregion job free fields subRpt

            #region PO req subRpt
            string queryPOreq = "SELECT \"Req-Number\", \"Supplier-ID\", \"Supplier-Name\", \"Inventory-Item-ID\", \"Item-Desc\", \"Qty-Requisitioned\", \"Unit-Meas-Purchase\", \"Unit-Cost-Purchase\", \"Req-Status\" FROM PUB.PORequisition WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtPOreq = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPOreq = new OdbcDataAdapter(queryPOreq, dbConn);
                adapPOreq.Fill(dtPOreq);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -PO Req report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPOreq.Rows.Count != 0)
            {
                cryrpt.Subreports[9].DataSourceConnections.Clear();
                cryrpt.Subreports[9].SetDataSource(dtPOreq);

            }
            else
            {

                string subPOreq = "subPOReq";
                HideSubs(cryrpt, subPOreq);

            }

            #endregion PO req subRpt


            #region PO line info subRpt
            string queryPOLine = "SELECT \"PO-Number\", \"Line-Status\", \"Date-PO\", \"Date-Last-Receipt\", \"Item-Desc\", \"Qty-Received-Purchase\" FROM PUB.POLine WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtPOLine = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPOLine = new OdbcDataAdapter(queryPOLine, dbConn);
                adapPOLine.Fill(dtPOLine);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -PO Line report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPOLine.Rows.Count != 0)
            {
                cryrpt.Subreports[8].DataSourceConnections.Clear();
                cryrpt.Subreports[8].SetDataSource(dtPOLine);

            }
            else
            {

                string subPOLine = "subPOLine";
                HideSubs(cryrpt, subPOLine);

            }
            #endregion Po Line info Subrpt

            #region ship to data subRpt
            string queryShipTo = "SELECT \"Ship-To-Name\", \"Ship-To-Address1\", \"Ship-To-Address2\", \"Ship-To-Address3\", \"Ship-To-City\", \"Ship-To-State\",\"Ship-To-Zip\",\"Ship-To-Attention\"" +
                ",\"Instructions\", \"Requested-Ship-Date\" ,\"Requested-Quantity\", \"Requested-Number-Packages\", FAX, Phone, \"Ship-To-Country\", \"Shipment-Method-ID\"  FROM PUB.JobShipTo WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtShipTo = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapShipTo = new OdbcDataAdapter(queryShipTo, dbConn);
                adapShipTo.Fill(dtShipTo);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -ship to report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtShipTo.Rows.Count != 0)
            {
                cryrpt.Subreports[12].DataSourceConnections.Clear();
                cryrpt.Subreports[12].SetDataSource(dtShipTo);

            }
            else
            {

                string subShipTo = "subShipTo";
                HideSubs(cryrpt, subShipTo);

            }

            #endregion ship to data subRpt

            #region form spec subRpt
            string querySpecForm = "SELECT \"Form-ID\",  \"Form-Desc\", \"Number-Pages\", Width, Length, \"Number-forms\", \"Number-Out\", Imposition, Notes FROM PUB.JobSpecForm WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtSpecForm = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapSpecForm = new OdbcDataAdapter(querySpecForm, dbConn);
                adapSpecForm.Fill(dtSpecForm);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Spec Form report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtSpecForm.Rows.Count != 0)
            {
                cryrpt.Subreports[3].DataSourceConnections.Clear();
                cryrpt.Subreports[3].SetDataSource(dtSpecForm);

            }
            else
            {

                string subFormSpec = "subFormSpecs";
                HideSubs(cryrpt, subFormSpec);

            }


            #endregion form spec subRpt

            #region form Notes subrpt
            string queryFormNotes = "SELECT \"Form-ID\", Description FROM PUB.JobComments WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtFormNotes = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapFormNotes = new OdbcDataAdapter(queryFormNotes, dbConn);
                adapFormNotes.Fill(dtFormNotes);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Form Notes report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtFormNotes.Rows.Count != 0)
            {
                cryrpt.Subreports[2].DataSourceConnections.Clear();
                cryrpt.Subreports[2].SetDataSource(dtFormNotes);

            }
            else
            {

                string subFormNotes = "subFormNotes";
                HideSubs(cryrpt, subFormNotes);

            }


            #endregion form notes subRpt

            #region prepress rpt
            string queryPrepress = "SELECT \"Form-ID\", \"Spec-Desc\", \"Work-Center-ID\", \"Operation-ID\", Quantity, Hours FROM PUB.JobSpecPrepress WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtPrepress = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPrepress = new OdbcDataAdapter(queryPrepress, dbConn);
                adapPrepress.Fill(dtPrepress);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Prepress report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPrepress.Rows.Count != 0)
            {
                cryrpt.Subreports[10].DataSourceConnections.Clear();
                cryrpt.Subreports[10].SetDataSource(dtPrepress);

            }
            else
            {

                string subPrepress = "subPrepress";
                HideSubs(cryrpt, subPrepress);

            }
            #endregion prepress rpt

            #region press rpt
            string queryPress = "SELECT \"Form-ID\", \"Operation-Type\",  \"Spec-Desc\", \"Qty-Makeready\", \"Qty-Net\", \"Qty-Spoilage\", Width, Length, \"Hours-Makeready\", \"Hours-Run\", \"Hours-Cleanup\" FROM PUB.JobSpecOperation WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtPress = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPress = new OdbcDataAdapter(queryPress, dbConn);
                adapPress.Fill(dtPress);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Press report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPress.Rows.Count != 0)
            {
                cryrpt.Subreports[11].DataSourceConnections.Clear();
                cryrpt.Subreports[11].SetDataSource(dtPress);

            }
            else
            {

                string subPress = "subPress";
                HideSubs(cryrpt, subPress);

            }

            #endregion press rpt

            #region stock rpt
            string queryStock = "SELECT \"Form-ID\", \"Gross-Sheets\", \"Gross-Weight\",  Notes,  \"Inventory-Item-ID\", \"Inventory-Item-Desc\", \"Material-Type\", Length, Width FROM PUB.JobSpecMaterial WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtStock = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapStock = new OdbcDataAdapter(queryStock, dbConn);
                adapStock.Fill(dtStock);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Stock report";

                ErrorLog(error);
            }

            //need to get page-grain new query
            string queryPageGrain = "SELECT \"Page-Grain\" FROM PUB.JobSpecForm WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtPG = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPG = new OdbcDataAdapter(queryPageGrain, dbConn);
                adapPG.Fill(dtPG);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Stock report -PG";

                ErrorLog(error);
            }

            dtStock.Columns.Add("Page-Grain");
            string val = dtPG.Rows[0]["Page-Grain"].ToString();

            foreach (DataRow dr in dtStock.Rows)
            {

                dr["Page-Grain"] = val;

            }

            //also check if empty exists it is empty hideSubs it
            if (dtStock.Rows.Count != 0)
            {
                cryrpt.Subreports[13].DataSourceConnections.Clear();
                cryrpt.Subreports[13].SetDataSource(dtStock);

            }
            else
            {

                string subStock = "subStock";
                HideSubs(cryrpt, subStock);

            }

            #endregion stock rpt

            #region bindery rpt
            string queryBind = "SELECT \"Form-ID\", \"Operation-Type\",  \"Spec-Desc\", \"Qty-Makeready\", \"Qty-Net\", \"Qty-Spoilage\", Width, Length, \"Hours-Makeready\", \"Hours-Run\", \"Hours-Cleanup\", \"Number-Out-for-Oper\" FROM PUB.JobSpecOperation WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtBind = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapBind = new OdbcDataAdapter(queryBind, dbConn);
                adapBind.Fill(dtBind);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Bindery report";

                ErrorLog(error);
            }

            ObjectIndexCheck(cryrpt);

            //also check if empty exists it is empty hideSubs it
            if (dtBind.Rows.Count != 0)
            {
                cryrpt.Subreports[1].DataSourceConnections.Clear();
                cryrpt.Subreports[1].SetDataSource(dtBind);

            }
            else
            {

                string subBindery = "subBindery";
                HideSubs(cryrpt, subBindery);

            }


            #endregion bindery rpt


            #endregion sub report creation


            dbConn.Close();
            #endregion DB close connection



            #region display rpt

            LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;
            LaunchOrigin.crystalReportViewer1.ReportSource = cryrpt;

            LaunchOrigin.crystalReportViewer1.Refresh(); //here is where i get the prompt DB login

            label3.Text = "Report loaded.";
            #endregion display rpt


        }//end full reprort
        #endregion full report






        #region print job ticket (intial load)
        private void button1_Click_1(object sender, EventArgs e)
        {
            label3.Text = "Please wait report loading..";

            jobNumberUser = textBox1.Text;

            button2.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.BorderColor = Color.Black;

            button8.FlatAppearance.BorderSize = 0;
            button8.FlatAppearance.BorderColor = Color.Black;

            button11.FlatAppearance.BorderSize = 0;
            button11.FlatAppearance.BorderColor = Color.Black;

            button3.FlatAppearance.BorderSize = 0;
            button3.FlatAppearance.BorderColor = Color.Black;

            button9.FlatAppearance.BorderSize = 0;
            button9.FlatAppearance.BorderColor = Color.Black;


            button1.FlatAppearance.BorderSize = 5;
            button1.FlatAppearance.BorderColor = Color.Black;


            LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;
            #region start program here

            CrystalReport2 cryrpt = new CrystalReport2();

            //path to report
            //cryrpt.Load("VVT.test.CrystalReport2.rpt");

            cryrpt.DataSourceConnections.Clear();

            //set the databse login info, use twice first one is to login into sql server
            cryrpt.SetDatabaseLogon("Bob", "Orchard", "monarch18", "gams1");

            cryrpt.SetDatabaseLogon("Bob", "Orchard"); //this one for that annoying prompt to login into database


            //this opens connection to DB with the login info down..
            ConnectionInfo crconnectioninfo = new ConnectionInfo();
            crconnectioninfo.ServerName = "monarch18";
            crconnectioninfo.DatabaseName = "gams1";
            crconnectioninfo.UserID = "Bob";
            crconnectioninfo.Password = "Orchard";


            //how to hide subreprorts
            //seems like you cast our reprort to a subreportObject then can directly access the reprorts
            //subreprorts and setting the Surpress property with T/F
            //so know make another form with buttons like in word doc from senior
            //should be very easy click button hide some reports, show others
            //run full report 


            //go into form2 and set the buttons for the report


            //this runs th ereport so before running the report lets 
            //case statement to hide all other subreports
            //updates report to most current

            //make them status and status desc blank
            CrystalDecisions.CrystalReports.Engine.TextObject txtObj4;
            txtObj4 = cryrpt.ReportDefinition.ReportObjects["status"] as TextObject;
            txtObj4.Text = "";

            CrystalDecisions.CrystalReports.Engine.TextObject txtObj5;
            txtObj5 = cryrpt.ReportDefinition.ReportObjects["statusDesc"] as TextObject;
            txtObj5.Text = "";

            //surpress the maiong  sub reprorts adn clear the data source (load empty DS to the subreprrot)
            DataSet emptyDS = new DataSet();
            DataTable emptyDT = new DataTable();
            emptyDS.Tables.Add(emptyDT);
            cryrpt.Subreports[7].DataSourceConnections.Clear();
            cryrpt.Subreports[7].SetDataSource(emptyDS.Tables[0]);
            string subMail = "Subreport30";
            HideSubs(cryrpt, subMail);


            #endregion onto hiding subreprorts


            #region query DB 

            #region 810 tag
            //query dtaabse for 810 tag*********************************************

            string connectStr = "DSN=Progress11;uid=Bob;pwd=Orchard";

            CrystalDecisions.CrystalReports.Engine.TextObject txtObj2;
            txtObj2 = cryrpt.ReportDefinition.ReportObjects["ticketName"] as TextObject;
            txtObj2.Text = "Printed Job Ticket";

            //get 810 to display Mailing 
            DataTable dt = new DataTable();

            using (OdbcConnection dbConn = new OdbcConnection(connectStr))
            {
                try
                {
                    dbConn.Open();
                }
                catch (Exception)
                {
                    MessageBox.Show("Failed to connect to database, thru odbc connection");
                }

                //got-dam that fricken hyphen in the table name, the hyphen needed some extra string manipulation
                //needed to have it as "Job-ID" instead of just Job-ID, needed to wrap in ""
                String query = "SELECT \"Work-Center-ID\", \"TagStatus-ID\" FROM PUB.ScheduleByJob WHERE \"Job-ID\"=" + jobNumberUser;

                OdbcDataAdapter adap = new OdbcDataAdapter(query, dbConn);

                adap.Fill(dt);

                //here is needed for 810 chck

                bool check = false;

                foreach (DataRow dr in dt.Rows)
                {
                    //do nothing 
                    if (dr["Work-Center-ID"].ToString().Contains("810"))
                    {

                        check = true;
                    }

                }//end foreach

                //no check to see if 810 tag is present
                if (!check)
                {

                    CrystalDecisions.CrystalReports.Engine.TextObject txtObj;
                    txtObj = cryrpt.ReportDefinition.ReportObjects["txtMail"] as TextObject;
                    txtObj.Text = "";
                }//end check for 810

                #endregion 810

                #region job notes custome for departments

                //so we want to customize the job notes ex) shipping buttton clicks
                //shoipping only display shipping notes
                //online says all i have to do is bind this to the crystal report viewer and
                //it comes up as a table automagically we shall see

                string commentQry = "SELECT * FROM Pub.JobComments WHERE \"Job-ID\" = " + jobNumberUser;

                OdbcDataAdapter adapComments = new OdbcDataAdapter(commentQry, dbConn);

                //define a dataset to hold onto data
                DataTable dtComments = new DataTable();

                //here fill the datatset then manipulate, then fill again to fill Data Table "jobComments"
                adapComments.Fill(dtComments);

                dtComments.AcceptChanges();

                DataSet sendDs = new DataSet();

                sendDs.Tables.Add(dtComments);

                //HERE very importnat to spell name of datatable correctly AS IT IS IN SOLUTION EXPLORER
                //NOT CRYSTAL REPORTS
                adapComments.Fill(sendDs, "jobComments");

                //ALSO BIG NOTE: KNOW WHAT SUBREPORT IT INDEXED AT "5", i was going after the wrong subreprot
                //pretty much causing crystal reprorts to re-query DB for the data and grabbing all records
                //just running  a simle SELECT * from
                //bind the dataset (returned from SQL query to our reprort object
                //also clear the connections and then set the subreprrot
                cryrpt.Subreports[0].DataSourceConnections.Clear();
                cryrpt.Subreports[0].SetDataSource(sendDs.Tables[0]);

                //if there are no comments hide the subreport 
                if (dtComments.Rows.Count == 0)
                {

                    string sub2 = "Subreport2";
                    HideSubs(cryrpt, sub2);

                }

                #endregion jobNotes

                #region PO outside service check

                string POquery = "SELECT * FROM Pub.POLine WHERE \"Job-ID\" = " + jobNumberUser;

                OdbcDataAdapter adapPO = new OdbcDataAdapter(POquery, dbConn);

                //define a dataset to hold onto data
                DataTable dtPO = new DataTable();

                //here fill the datatset then manipulate, then fill again to fill Data Table "jobComments"
                adapPO.Fill(dtPO);


                dtPO.AcceptChanges();


                //check the item if it contains an "01" in it make the text object Outside Service show if nto do not show
                bool osCheck = false;

                foreach (DataRow dr in dtPO.Rows)
                {

                    if (dr["Inventory-Item-ID"].ToString().Contains("01"))
                    {

                        osCheck = true;

                    }

                }
                if (osCheck)
                {

                    CrystalDecisions.CrystalReports.Engine.TextObject txtObjOS;
                    txtObjOS = cryrpt.ReportDefinition.ReportObjects["OSservice"] as TextObject;
                    txtObjOS.Text = "Outside Service";

                }

                if (!osCheck)
                {

                    CrystalDecisions.CrystalReports.Engine.TextObject txtObjOS;
                    txtObjOS = cryrpt.ReportDefinition.ReportObjects["OSservice"] as TextObject;
                    txtObjOS.Text = "";

                }

                #endregion PO outside service check

                dbConn.Close();
            }//end odbc connection
            #endregion DB 810 check

            #region hide subs and objects
            //enetr code before running crystal reprort
            //hide stock (paper)
            string sub11 = "Subreport11";
            HideSubs(cryrpt, sub11);

            //hide bindery matts
            string sub5 = "Subreport5";
            HideSubs(cryrpt, sub5);

            //hide free fields
            string sub32 = "Subreport32";
            HideSubs(cryrpt, sub32);

            //hide mailing ticket
            string sub24 = "Subreport24";
            HideSubs(cryrpt, sub24);

            //hide PO
            string sub13 = "Subreport13";
            HideSubs(cryrpt, sub13);

            //hide shipping
            string sub8 = "Subreport8";
            HideSubs(cryrpt, sub8);

            //hide job specs
            string sub7 = "Subreport7";
            HideSubs(cryrpt, sub7);

            //hide form notes
            string sub15 = "Subreport15";
            HideSubs(cryrpt, sub15);

            //hide pre-press
            string sub9 = "Subreport9";
            HideSubs(cryrpt, sub9);

            //hide press
            string sub3 = "Subreport3";
            HideSubs(cryrpt, sub3);

            //hide bindery
            string sub4 = "Subreport4";
            HideSubs(cryrpt, sub4);

            //hide the PO sub-reprort sas well
            string sub10 = "Subreport10";
            HideSubs(cryrpt, sub10);

            //hide mailing subreprort free freilds
            string sub30 = "Subreport30";
            HideSubs(cryrpt, sub30);

            //hide mailing subreprort free freilds
            string sub31 = "Subreport31";
            HideSubs(cryrpt, sub31);


            //put code here to play aroudn with values

            #endregion hide subs

            //try to pre-set parameter panel
            try
            {
                cryrpt.SetParameterValue("Job-ID", jobNumberUser);

                cryrpt.SetParameterValue("System-ID", "Viso");


            }
            catch (CrystalDecisions.Shared.CrystalReportsException)
            {
                MessageBox.Show("Aw snap! X.X Error while loading report, please refresh! (If problem keeps occuring see Kyle in mailing please!)");
            }

            LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;
            LaunchOrigin.crystalReportViewer1.ReportSource = cryrpt;

            LaunchOrigin.crystalReportViewer1.Refresh();

            label3.Text = "Report loaded.";

        }
        #endregion pritn job ticket




        //exit button
        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }





        //PO button
        private void button2_Click(object sender, EventArgs e)
        {
            //here is where i will put all Form {button/label/etc} editing
            //Notes: can NOT edit crystal report stuff here; have not created crystal reprort object yet, that will be later
            #region label and buttons UI
            label3.Text = "Please wait report loading..";

            jobNumberUser = textBox1.Text;


            button1.FlatAppearance.BorderSize = 0;
            button1.FlatAppearance.BorderColor = Color.Black;

            button11.FlatAppearance.BorderSize = 0;
            button11.FlatAppearance.BorderColor = Color.Black;

            button3.FlatAppearance.BorderSize = 0;
            button3.FlatAppearance.BorderColor = Color.Black;

            button9.FlatAppearance.BorderSize = 0;
            button9.FlatAppearance.BorderColor = Color.Black;


            button8.FlatAppearance.BorderSize = 0;
            button8.FlatAppearance.BorderColor = Color.Black;


            button2.FlatAppearance.BorderSize = 5;
            button2.FlatAppearance.BorderColor = Color.Black;

            //timestamp when a button is clicked (report is ran), so user knows what time the current report on screen was ran
            label5.Text = DateTime.Now.ToString();

            #endregion label/buttons UI end




            //here is where i will put the creation and manipualtion of the crystal report object
            #region start crystal report config:

            #region connection and CR object properties/settings
            //rerport object



            CrystalReport2 cryrpt = new CrystalReport2();

            cryrpt.DataSourceConnections.Clear();  //clear the connections (will popualte with fresh sql query defined data

            //set the databse login info, use twice first one is to login into sql server

            //getting write only error


            cryrpt.SetDatabaseLogon("Bob", "Orchard", "monarch18", "gams1");
            cryrpt.SetDatabaseLogon("Bob", "Orchard"); //this one for that annoying prompt to login into database


            //this does not error out!?
            //this opens connection to DB with the login info down..
            ConnectionInfo crconnectioninfo = new ConnectionInfo();
            crconnectioninfo.ServerName = "monarch18";
            crconnectioninfo.DatabaseName = "gams1";
            crconnectioninfo.UserID = "Bob";
            crconnectioninfo.Password = "Orchard";


            //try our best to never have paramter panel pop-up, works like 95% of time
            //error here as well?
            //LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;

            #endregion connectiona and CR object properties/settings

            #region UI on CR object editing (non-DB dependant)

            //change text object, tell user what ticket the are curentlly running/looking at
            CrystalDecisions.CrystalReports.Engine.TextObject txtObj2;
            txtObj2 = cryrpt.ReportDefinition.ReportObjects["ticketName"] as TextObject;
            txtObj2.Text = "PO Ticket";


            #endregion UI on CR object editing (non-DB dependant)

            #endregion crystal report config end




            //here is where i need to clean up lots, make functions, sql queryies, DT and DS manipualtion, pass the job number, etc 
            //main code for program functionallity
            #region DB connection 


            //here is where i iwll be connecting to DB's (all queries will need access to basically my connection properties globals)
            #region global connection properties

            //connection string for DB
            string connectStr = "DSN=Progress11;uid=Bob;pwd=Orchard";

            //open th econnection and error check
            OdbcConnection dbConn = new OdbcConnection(connectStr);

            try
            {
                dbConn.Open();
            }
            catch (Exception ex)
            {

                string error = ex + " : DB error cannot connect";

                ErrorLog(error);

            }
            #endregion global connection propeties




            //here is where i can change the UI of the report based on database data
            //ex) show word nailing on report if job# has a 810 tag associated with it
            #region UI crystal report editing (DB dependant)


            #region Header

            //set the job numbers from iuser input
            CrystalDecisions.CrystalReports.Engine.TextObject jobNum1;
            jobNum1 = cryrpt.ReportDefinition.ReportObjects["jobNum"] as TextObject;
            jobNum1.Text = jobNumberUser;

            CrystalDecisions.CrystalReports.Engine.TextObject jobNum2;
            jobNum2 = cryrpt.ReportDefinition.ReportObjects["jobNum2"] as TextObject;
            jobNum2.Text = jobNumberUser;



            String headerJob = "SELECT \"Job-Desc\", \"Date-Promised\", \"Sales-Rep-ID\", \"CSR-ID\", \"" +
                "PO-Number\", \"Overs-Allowed\", \"Last-Estimate-ID\", \"Quantity-Ordered\", \"Contact-Name\", \"Date-Entered\", \"Cust-ID-Ordered-by\"" +
                " FROM PUB.JOB WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtHeader = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter header = new OdbcDataAdapter(headerJob, dbConn);
                header.Fill(dtHeader);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //set all text objects to the data from datatable

            //job descriptions
            CrystalDecisions.CrystalReports.Engine.TextObject jobDesc;
            jobDesc = cryrpt.ReportDefinition.ReportObjects["jobDesc"] as TextObject;
            jobDesc.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

            CrystalDecisions.CrystalReports.Engine.TextObject jobDesc2;
            jobDesc2 = cryrpt.ReportDefinition.ReportObjects["jobDesc2"] as TextObject;
            jobDesc2.Text = dtHeader.Rows[0]["Job-Desc"].ToString();

            //date promised
            CrystalDecisions.CrystalReports.Engine.TextObject dateProm;
            dateProm = cryrpt.ReportDefinition.ReportObjects["dateProm"] as TextObject;
            dateProm.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

            CrystalDecisions.CrystalReports.Engine.TextObject dateProm2;
            dateProm2 = cryrpt.ReportDefinition.ReportObjects["dateProm2"] as TextObject;
            dateProm2.Text = dtHeader.Rows[0]["Date-Promised"].ToString();

            //qty
            CrystalDecisions.CrystalReports.Engine.TextObject qty;
            qty = cryrpt.ReportDefinition.ReportObjects["qty"] as TextObject;
            qty.Text = dtHeader.Rows[0]["Quantity-Ordered"].ToString();

            CrystalDecisions.CrystalReports.Engine.TextObject qty2;
            qty2 = cryrpt.ReportDefinition.ReportObjects["qty2"] as TextObject;
            qty2.Text = dtHeader.Rows[0]["Quantity-Ordered"].ToString();

            //job contact
            CrystalDecisions.CrystalReports.Engine.TextObject jobContact;
            jobContact = cryrpt.ReportDefinition.ReportObjects["contactName"] as TextObject;
            jobContact.Text = dtHeader.Rows[0]["Contact-Name"].ToString();

            //job date entered
            CrystalDecisions.CrystalReports.Engine.TextObject jobDE;
            jobDE = cryrpt.ReportDefinition.ReportObjects["jobDE"] as TextObject;
            jobDE.Text = dtHeader.Rows[0]["Date-Entered"].ToString();

            //over allowed
            CrystalDecisions.CrystalReports.Engine.TextObject OA;
            OA = cryrpt.ReportDefinition.ReportObjects["jobOA"] as TextObject;
            OA.Text = dtHeader.Rows[0]["Overs-Allowed"].ToString();

            //po num
            CrystalDecisions.CrystalReports.Engine.TextObject PO;
            PO = cryrpt.ReportDefinition.ReportObjects["poNum"] as TextObject;
            PO.Text = dtHeader.Rows[0]["PO-Number"].ToString();

            //estimate
            string est = dtHeader.Rows[0]["Last-Estimate-ID"].ToString().Insert(6, "-");
            CrystalDecisions.CrystalReports.Engine.TextObject estimate;
            estimate = cryrpt.ReportDefinition.ReportObjects["estimate"] as TextObject;
            estimate.Text = est;

            //customer query and text objects
            String headerCust = "SELECT \"cust-name\", \"Address-1\", \"Address-2\", \"City\", \"" +
               "State\", \"Zip\", \"Phone\", \"Address-3\" FROM PUB.cust WHERE \"Cust-code\" = " + dtHeader.Rows[0]["Cust-ID-Ordered-by"].ToString();


            DataTable dtCust = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter cust = new OdbcDataAdapter(headerCust, dbConn);
                cust.Fill(dtCust);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //set the Customer info text objects
            //cust name
            CrystalDecisions.CrystalReports.Engine.TextObject custName;
            custName = cryrpt.ReportDefinition.ReportObjects["custName"] as TextObject;
            custName.Text = dtCust.Rows[0]["cust-name"].ToString();

            CrystalDecisions.CrystalReports.Engine.TextObject custName2;
            custName2 = cryrpt.ReportDefinition.ReportObjects["custName2"] as TextObject;
            custName2.Text = dtCust.Rows[0]["cust-name"].ToString();

            //address -> add 1 and 2 and 3 combined
            CrystalDecisions.CrystalReports.Engine.TextObject custAdd;
            custAdd = cryrpt.ReportDefinition.ReportObjects["custAddress"] as TextObject;
            custAdd.Text = dtCust.Rows[0]["Address-1"].ToString() + " " + dtCust.Rows[0]["Address-2"].ToString() + " " + dtCust.Rows[0]["Address-3"].ToString();

            //city state zip customer
            CrystalDecisions.CrystalReports.Engine.TextObject custCSZ;
            custCSZ = cryrpt.ReportDefinition.ReportObjects["custCSZ"] as TextObject;
            custCSZ.Text = dtCust.Rows[0]["City"].ToString() + " " + dtCust.Rows[0]["State"].ToString() + " " + dtCust.Rows[0]["Zip"].ToString();

            //customerPhone
            CrystalDecisions.CrystalReports.Engine.TextObject custPhone;
            custPhone = cryrpt.ReportDefinition.ReportObjects["custPhone"] as TextObject;
            custPhone.Text = dtCust.Rows[0]["Phone"].ToString();

            //sales agent query and txt obj change
            //why this does not work i have no fkin clue, making stack overflwo see what the brians can thunk up
            String headerSalesAgent = "SELECT \"Sales-Agent-Name\" FROM PUB.\"sales-agent\" WHERE \"Sales-agent-id\" = " + "'" + dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "'";

            // String headerSalesAgent = "SELECT \"Sales-agent-id\" , \"Sales-Agent-Name\" FROM PUB.\"sales-agent\"";

            DataTable dtSalesAgent = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter salesAgent = new OdbcDataAdapter(headerSalesAgent, dbConn);
                salesAgent.Fill(dtSalesAgent);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }


            //sales agent name
            CrystalDecisions.CrystalReports.Engine.TextObject salesRep;
            salesRep = cryrpt.ReportDefinition.ReportObjects["salesRep"] as TextObject;
            salesRep.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString() + "-" + dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();

            //sales rep ID for billing
            CrystalDecisions.CrystalReports.Engine.TextObject salesRep2;
            salesRep2 = cryrpt.ReportDefinition.ReportObjects["salesRep2"] as TextObject;
            salesRep2.Text = dtHeader.Rows[0]["Sales-Rep-ID"].ToString();// + "-" + dtSalesAgent.Rows[0]["Sales-Agent-Name"].ToString();

            //csr query and txt obj change
            String headerCSR = "SELECT \"CSR-Name\" FROM PUB.CSR WHERE \"CSR-ID\"=" + "'" + dtHeader.Rows[0]["CSR-ID"].ToString() + "'";

            DataTable dtCsr = new DataTable();
            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter csrAdap = new OdbcDataAdapter(headerCSR, dbConn);
                csrAdap.Fill(dtCsr);
            }
            catch (Exception ex)
            {
                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            CrystalDecisions.CrystalReports.Engine.TextObject csr;
            csr = cryrpt.ReportDefinition.ReportObjects["csr"] as TextObject;
            csr.Text = dtHeader.Rows[0]["CSR-ID"].ToString() + "-" + dtCsr.Rows[0]["CSR-Name"].ToString();

            #endregion Header


            #region 810 tag check - show MAILING
            //DataTable for all UI db-depenedant editing
            DataTable dtEdit = new DataTable();


            String query810Tag = "SELECT \"Work-Center-ID\", \"TagStatus-ID\" FROM PUB.ScheduleByJob WHERE \"Job-ID\"=" + jobNumberUser;

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adap810Tag = new OdbcDataAdapter(query810Tag, dbConn);
                adap810Tag.Fill(dtEdit);
            }
            catch (Exception ex)
            {


                string error = ex + " : SQL error cannot load OdbcDataAdapter - UI setter";

                ErrorLog(error);

            }

            //here is needed for 810 chck
            bool check = false;

            foreach (DataRow dr in dtEdit.Rows)
            {
                //do nothing 
                if (dr["Work-Center-ID"].ToString().Contains("810"))
                {

                    check = true;
                }

            }//end foreach

            //no check to see if 810 tag is present
            if (!check)
            {

                CrystalDecisions.CrystalReports.Engine.TextObject txtObj;
                txtObj = cryrpt.ReportDefinition.ReportObjects["txtMail"] as TextObject;
                txtObj.Text = "";
            }//end check for 810
            #endregion 810 tag check - show MAILING

            //can use same data from above query and dataTable to get the the job status and description
            //ex) 50d status and it's description is ready to run on digital press
            #region tag status grab (no query, re-use data gathered from 810 check)

            try
            {
                //need to base of the 900 tag's -> tag status
                string tagStat = "";

                foreach (DataRow dr in dtEdit.Rows)
                {
                    //do nothing 
                    if (dr["Work-Center-ID"].ToString() == "900")
                    {

                        tagStat = dr["TagStatus-ID"].ToString();
                    }

                }//end foreach


                //grab first record's 'TagStatus-ID" and set status to it ex) 50d, 09, etc
                var statusObj = cryrpt.ReportDefinition.ReportObjects["status"] as TextObject;
                statusObj.Text = tagStat;

                var statusDescObj = cryrpt.ReportDefinition.ReportObjects["statusDesc"] as TextObject;

                if (tagStat == "01")
                {
                    statusDescObj.Text = "Outside Service";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }

                if (tagStat == "02")
                {
                    statusDescObj.Text = "On Hold";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "05")
                {
                    statusDescObj.Text = "Digital Mailing/Need Run Files";
                }
                if (tagStat == "07")
                {
                    statusDescObj.Text = "Long Term Project (In-House)";
                }
                if (tagStat == "08")
                {
                    statusDescObj.Text = "Long Term Project (out on proof)";
                }
                if (tagStat == "09")
                {
                    statusDescObj.Text = "In Pre Press Production";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "09-R")
                {
                    statusDescObj.Text = "Art corrections after proof";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "18")
                {
                    statusDescObj.Text = "Out on Random Proof";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "20")
                {
                    statusDescObj.Text = "Ready to Strip and Plate";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "20p")
                {
                    statusDescObj.Text = "Approved-Waiting for Mock up";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "21")
                {
                    statusDescObj.Text = "Rerun-Pill Plates";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "29")
                {
                    statusDescObj.Text = "Ready to Plate";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50")
                {
                    statusDescObj.Text = "Press-Running/Plated";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50d" || tagStat == "50D")
                {
                    statusDescObj.Text = "Ready to Run Digital Press";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "50e" || tagStat == "50E")
                {
                    statusDescObj.Text = "Ready to Run Digital Press ENVELOPE";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "70")
                {
                    statusDescObj.Text = "Printed and in Bindery";
                    statusObj.Color = Color.Brown;
                    statusDescObj.Color = Color.Brown;
                }
                if (tagStat == "72")
                {
                    statusDescObj.Text = "Monthly DSF billing jobs";
                }
                if (tagStat == "75")
                {
                    statusDescObj.Text = "Waiting for Mailing Data";
                    statusObj.Color = Color.Red;
                    statusDescObj.Color = Color.Red;
                }
                if (tagStat == "80")
                {
                    statusDescObj.Text = "Bindery Done-Ready for Mailing";
                    statusObj.Color = Color.Blue;
                    statusDescObj.Color = Color.Blue;
                }
                if (tagStat == "82")
                {
                    statusDescObj.Text = "Running on Netjet";
                    statusObj.Color = Color.Green;
                    statusDescObj.Color = Color.Green;
                }
                if (tagStat == "85")
                {
                    statusDescObj.Text = "Mail Complete/Need Paperwork";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;

                }
                if (tagStat == "88")
                {
                    statusDescObj.Text = "Mail Complete - Ready to Mail";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;

                }
                if (tagStat == "88p")
                {
                    statusDescObj.Text = "PARTIAL mail/Ready to Deliver";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;
                }
                if (tagStat == "90")
                {
                    statusDescObj.Text = "Job Completed-Ready to Deliver";
                    statusObj.Color = Color.Orange;
                    statusDescObj.Color = Color.Orange;
                }
                if (tagStat == "92")
                {
                    statusDescObj.Text = "Being Delivered";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
                if (tagStat == "95")
                {
                    statusDescObj.Text = "Delivered";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
                if (tagStat == "97")
                {
                    statusDescObj.Text = "DSF Jobs To Be Billed";

                }
                if (tagStat == "97b" || tagStat == "97B")
                {
                    statusDescObj.Text = "DSF jobs Already Billed";
                }
                if (tagStat == "29")
                {
                    statusDescObj.Text = "Job Close Pending";
                    statusDescObj.Color = Color.Blue;
                    statusObj.Color = Color.Blue;
                }
            }//end try
            catch (Exception ex)
            {

                string error = ex + " : Tag Status update error check code";

                ErrorLog(error);

            }

            #endregion tag status

            #endregion UI


            //sub-reports section
            #region sub report creater

            //ObjectIndexCheck(cryrpt);

            //Press/Prepress sub rereports - Mailing Version, Mailing Free Feilds, Job Notes, Job Free Feilds, PO Req, PO line,
            //Forms, press, Stock

            #region Mailing Version subReport
            string queryMailVersion = "SELECT \"Free-Field-Char\" FROM PUB.MailingVersionFreeField WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtMailVersion = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapMailVersion = new OdbcDataAdapter(queryMailVersion, dbConn);
                adapMailVersion.Fill(dtMailVersion);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - Mailing Version FF report";

                ErrorLog(error);
            }

            //and change second column name to match CR report value "FFvalue"
            dtMailVersion.Columns["Free-Field-Char"].ColumnName = "FFvalue";

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtMailVersion.Rows.Count != 0)
            {

                cryrpt.Subreports[7].DataSourceConnections.Clear();
                cryrpt.Subreports[7].SetDataSource(dtMailVersion);

            }
            else
            {

                string subMailVersionFF = "subMailVersion";
                HideSubs(cryrpt, subMailVersionFF);

            }

            #endregion Mailing Version subReport

            #region Mailing Free Fields subReport
            string queryMailFF = "SELECT \"Free-Field-Char\" FROM PUB.MailingFreeField WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtMailFF = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapMailFF = new OdbcDataAdapter(queryMailFF, dbConn);
                adapMailFF.Fill(dtMailFF);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter - Mailing FF report";

                ErrorLog(error);
            }

            //and change second column name to match CR report value "FFvalue"
            dtMailFF.Columns["Free-Field-Char"].ColumnName = "FFval";

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtMailFF.Rows.Count != 0)
            {
                cryrpt.Subreports[6].DataSourceConnections.Clear();
                cryrpt.Subreports[6].SetDataSource(dtMailFF);

            }
            else
            {

                string subMailFF = "subMailFreeFields";
                HideSubs(cryrpt, subMailFF);

            }

            #endregion Mailing Free Fields subReport

            #region Job Notes subReport
            string queryJobNotes = "SELECT \"SpecCategory-ID\", Description, \"Created-By\", \"Comment-Date\", \"Update-date\" FROM PUB.JobComments WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtJobNotes = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapJobNotes = new OdbcDataAdapter(queryJobNotes, dbConn);
                adapJobNotes.Fill(dtJobNotes);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Job Notes report";

                ErrorLog(error);
            }

            //change the names
            dtJobNotes.Columns["SpecCategory-ID"].ColumnName = "SpecID";
            dtJobNotes.Columns["Created-By"].ColumnName = "EnterBy";
            dtJobNotes.Columns["Comment-Date"].ColumnName = "DateEntered";
            dtJobNotes.Columns["Update-date"].ColumnName = "DateUpdated";

            //now clear datasource connecctions and set them with dt
            //also check if empty exists it is empty hideSubs it
            if (dtJobNotes.Rows.Count != 0)
            {
                cryrpt.Subreports[5].DataSourceConnections.Clear();
                cryrpt.Subreports[5].SetDataSource(dtJobNotes);

            }
            else
            {

                string subJobComments = "subJobNotes";
                HideSubs(cryrpt, subJobComments);

            }


            #endregion Job Notes subReports




            #region Alterations subRpt
            string queryAlt = "SELECT * FROM PUB.Alterations WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtAlt = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapAlt = new OdbcDataAdapter(queryAlt, dbConn);
                adapAlt.Fill(dtAlt);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Alterations report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtAlt.Rows.Count != 0)
            {
                cryrpt.Subreports[0].DataSourceConnections.Clear();
                cryrpt.Subreports[0].SetDataSource(dtAlt);

            }
            else
            {

                string subAlt = "subAlterations";
                HideSubs(cryrpt, subAlt);

            }

            #endregion alterations subRpt

            #region Job free fields subRPt
            string queryFF = "SELECT \"Free-Field-Char\", \"Free-Field-Decimal\" FROM PUB.JobFreeField WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtFF = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapFF = new OdbcDataAdapter(queryFF, dbConn);
                adapFF.Fill(dtFF);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Free fields report";

                ErrorLog(error);
            }

            //here is where i need to convert decimal -> free-fieldchar
            string lastJob = dtFF.Rows[1]["Free-Field-Decimal"].ToString();
            dtFF.Rows[1]["Free-Field-Char"] = lastJob;

            dtFF.Columns.Remove("Free-Field-Decimal");

            //also check if empty exists it is empty hideSubs it
            if (dtFF.Rows.Count != 0)
            {
                cryrpt.Subreports[4].DataSourceConnections.Clear();
                cryrpt.Subreports[4].SetDataSource(dtFF);

            }
            else
            {

                string subFF = "subJobFreeFields";
                HideSubs(cryrpt, subFF);

            }


            #endregion job free fields subRpt

            #region PO req subRpt
            string queryPOreq = "SELECT \"Req-Number\", \"Supplier-ID\", \"Supplier-Name\", \"Inventory-Item-ID\", \"Item-Desc\", \"Qty-Requisitioned\", \"Unit-Meas-Purchase\", \"Unit-Cost-Purchase\", \"Req-Status\" FROM PUB.PORequisition WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtPOreq = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPOreq = new OdbcDataAdapter(queryPOreq, dbConn);
                adapPOreq.Fill(dtPOreq);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -PO Req report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPOreq.Rows.Count != 0)
            {
                cryrpt.Subreports[9].DataSourceConnections.Clear();
                cryrpt.Subreports[9].SetDataSource(dtPOreq);

            }
            else
            {

                string subPOreq = "subPOReq";
                HideSubs(cryrpt, subPOreq);

            }

            #endregion PO req subRpt


            #region PO line info subRpt
            string queryPOLine = "SELECT \"PO-Number\", \"Line-Status\", \"Date-PO\", \"Date-Last-Receipt\", \"Item-Desc\", \"Qty-Received-Purchase\" FROM PUB.POLine WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtPOLine = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPOLine = new OdbcDataAdapter(queryPOLine, dbConn);
                adapPOLine.Fill(dtPOLine);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -PO Line report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPOLine.Rows.Count != 0)
            {
                cryrpt.Subreports[8].DataSourceConnections.Clear();
                cryrpt.Subreports[8].SetDataSource(dtPOLine);

            }
            else
            {

                string subPOLine = "subPOLine";
                HideSubs(cryrpt, subPOLine);

            }
            #endregion Po Line info Subrpt

            #region ship to data subRpt
            string queryShipTo = "SELECT \"Ship-To-Name\", \"Ship-To-Address1\", \"Ship-To-Address2\", \"Ship-To-Address3\", \"Ship-To-City\", \"Ship-To-State\",\"Ship-To-Zip\",\"Ship-To-Attention\"" +
                ",\"Instructions\", \"Requested-Ship-Date\" ,\"Requested-Quantity\", \"Requested-Number-Packages\", FAX, Phone, \"Ship-To-Country\", \"Shipment-Method-ID\"  FROM PUB.JobShipTo WHERE \"Job-ID\" = " + jobNumberUser;

            DataTable dtShipTo = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapShipTo = new OdbcDataAdapter(queryShipTo, dbConn);
                adapShipTo.Fill(dtShipTo);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -ship to report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtShipTo.Rows.Count != 0)
            {
                cryrpt.Subreports[12].DataSourceConnections.Clear();
                cryrpt.Subreports[12].SetDataSource(dtShipTo);

            }
            else
            {

                string subShipTo = "subShipTo";
                HideSubs(cryrpt, subShipTo);

            }

            #endregion ship to data subRpt

            #region form spec subRpt
            string querySpecForm = "SELECT \"Form-ID\",  \"Form-Desc\", \"Number-Pages\", Width, Length, \"Number-forms\", \"Number-Out\", Imposition, Notes FROM PUB.JobSpecForm WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtSpecForm = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapSpecForm = new OdbcDataAdapter(querySpecForm, dbConn);
                adapSpecForm.Fill(dtSpecForm);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Spec Form report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtSpecForm.Rows.Count != 0)
            {
                cryrpt.Subreports[3].DataSourceConnections.Clear();
                cryrpt.Subreports[3].SetDataSource(dtSpecForm);

            }
            else
            {

                string subFormSpec = "subFormSpecs";
                HideSubs(cryrpt, subFormSpec);

            }


            #endregion form spec subRpt

            #region form Notes subrpt
            string queryFormNotes = "SELECT \"Form-ID\", Description FROM PUB.JobComments WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtFormNotes = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapFormNotes = new OdbcDataAdapter(queryFormNotes, dbConn);
                adapFormNotes.Fill(dtFormNotes);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Form Notes report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtFormNotes.Rows.Count != 0)
            {
                cryrpt.Subreports[2].DataSourceConnections.Clear();
                cryrpt.Subreports[2].SetDataSource(dtFormNotes);

            }
            else
            {

                string subFormNotes = "subFormNotes";
                HideSubs(cryrpt, subFormNotes);

            }


            #endregion form notes subRpt

            #region prepress rpt
            string queryPrepress = "SELECT \"Form-ID\", \"Spec-Desc\", \"Work-Center-ID\", \"Operation-ID\", Quantity, Hours FROM PUB.JobSpecPrepress WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtPrepress = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPrepress = new OdbcDataAdapter(queryPrepress, dbConn);
                adapPrepress.Fill(dtPrepress);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Prepress report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPrepress.Rows.Count != 0)
            {
                cryrpt.Subreports[10].DataSourceConnections.Clear();
                cryrpt.Subreports[10].SetDataSource(dtPrepress);

            }
            else
            {

                string subPrepress = "subPrepress";
                HideSubs(cryrpt, subPrepress);

            }
            #endregion prepress rpt

            #region press rpt
            string queryPress = "SELECT \"Form-ID\", \"Operation-Type\",  \"Spec-Desc\", \"Qty-Makeready\", \"Qty-Net\", \"Qty-Spoilage\", Width, Length, \"Hours-Makeready\", \"Hours-Run\", \"Hours-Cleanup\" FROM PUB.JobSpecOperation WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtPress = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPress = new OdbcDataAdapter(queryPress, dbConn);
                adapPress.Fill(dtPress);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Press report";

                ErrorLog(error);
            }

            //also check if empty exists it is empty hideSubs it
            if (dtPress.Rows.Count != 0)
            {
                cryrpt.Subreports[11].DataSourceConnections.Clear();
                cryrpt.Subreports[11].SetDataSource(dtPress);

            }
            else
            {

                string subPress = "subPress";
                HideSubs(cryrpt, subPress);

            }

            #endregion press rpt

            #region stock rpt
            string queryStock = "SELECT \"Form-ID\", \"Gross-Sheets\", \"Gross-Weight\",  Notes,  \"Inventory-Item-ID\", \"Inventory-Item-Desc\", \"Material-Type\", Length, Width FROM PUB.JobSpecMaterial WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtStock = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapStock = new OdbcDataAdapter(queryStock, dbConn);
                adapStock.Fill(dtStock);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Stock report";

                ErrorLog(error);
            }

            //need to get page-grain new query
            string queryPageGrain = "SELECT \"Page-Grain\" FROM PUB.JobSpecForm WHERE \"Job-ID\" =" + jobNumberUser;
            DataTable dtPG = new DataTable();

            try //to sql and fill adapter and DT
            {
                OdbcDataAdapter adapPG = new OdbcDataAdapter(queryPageGrain, dbConn);
                adapPG.Fill(dtPG);
            }
            catch (Exception ex)
            {

                string error = ex + " : SQL error cannot load OdbcDataAdapter -Stock report -PG";

                ErrorLog(error);
            }

            dtStock.Columns.Add("Page-Grain");
            string val = dtPG.Rows[0]["Page-Grain"].ToString();

            foreach (DataRow dr in dtStock.Rows)
            {

                dr["Page-Grain"] = val;

            }

            //also check if empty exists it is empty hideSubs it
            if (dtStock.Rows.Count != 0)
            {
                cryrpt.Subreports[13].DataSourceConnections.Clear();
                cryrpt.Subreports[13].SetDataSource(dtStock);

            }
            else
            {

                string subStock = "subStock";
                HideSubs(cryrpt, subStock);

            }

            #endregion stock rpt

            #region bindery rpt


            #endregion bindery rpt


            #endregion sub report creation


            dbConn.Close();
            #endregion DB close connection



            #region display rpt

            LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;
            LaunchOrigin.crystalReportViewer1.ReportSource = cryrpt;

            LaunchOrigin.crystalReportViewer1.Refresh(); //here is where i get the prompt DB login

            label3.Text = "Report loaded.";
            #endregion display rpt


        }//end Po button





        #region Functions

        public void ErrorLog(string logWrite)
        {

            //write to a public txt file (\\visonas\Public\Kyle\errorLog)
            string machineName = Environment.MachineName;

            logWrite += " : on computer name - " + machineName;

            MessageBox.Show("Please copy and paste the next pop-up box and save to txt file");
            MessageBox.Show(logWrite);

        }

        #region hide subs()
        //used to surpress sub reports
        public void HideSubs(CrystalReport2 crObj, string str)
        {

            try
            {
                var subRPT = (SubreportObject)crObj.ReportDefinition.ReportObjects[str];
                subRPT.ObjectFormat.EnableSuppress = true;

            }
            catch (Exception ex)
            {

                string error = ex + " : Error in HideSubs(); : ";

                ErrorLog(error);

            }
        }//end Hide subs
        #endregion hide subs

        #region object index checker
        public void ObjectIndexCheck(CrystalReport2 crObj)
        {


            Console.WriteLine("Main Report Objects and indexes");
            for (int x = 0; x < crObj.ReportDefinition.ReportObjects.Count; x++)
            {

                Console.WriteLine("Index: " + x + " Name: " + crObj.ReportDefinition.ReportObjects[x].Name);

            }//end reprort objects (main report)


            Console.WriteLine("\nSection Names and indexes");
            for (int x = 0; x < crObj.ReportDefinition.Sections.Count; x++)
            {

                Console.WriteLine("Index: " + x + " Name: " + crObj.ReportDefinition.Sections[x].Name);

            }//end section reprorts lsit


            Console.WriteLine("\nSubReport Names and indexes");
            for (int x = 0; x < crObj.Subreports.Count; x++)
            {

                Console.WriteLine("Index: " + x + " Name: " + crObj.Subreports[x].Name);

            }//end sub reprorts lsit


            //print out each sub reprort -> subreprort objects
            Console.WriteLine("\nSubReport Report Objects Names and indexes");
            for (int x = 0; x < crObj.Subreports.Count; x++)
            {

                Console.WriteLine("Index: " + x + " Name: " + crObj.Subreports[x].Name);

                for (int y = 0; y < crObj.Subreports[x].ReportDefinition.ReportObjects.Count; y++)
                {

                    Console.WriteLine("Index: " + y + " Name: " + crObj.Subreports[x].ReportDefinition.ReportObjects[y].Name);

                }

                Console.WriteLine("\n");

            }//end sub reprorts lsit



        }
        #endregion object index checker

        #endregion Functions




        #region accidental double clicks on forms
        //accidental double clciks in form*******
        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        #endregion accidental double clicks on forms


    }//end form2
}//end namespace
