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

            #region label/button UI
            label3.Text = "Please wait report loading..";

            jobNumberUser = textBox1.Text;

            //reset all other buttons
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatAppearance.BorderColor = Color.Black;

            button2.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.BorderColor = Color.Black;

            button11.FlatAppearance.BorderSize = 0;
            button11.FlatAppearance.BorderColor = Color.Black;

            // button5.FlatAppearance.BorderSize = 0;
            // button5.FlatAppearance.BorderColor = Color.Black;

            button9.FlatAppearance.BorderSize = 0;
            button9.FlatAppearance.BorderColor = Color.Black;

            button8.FlatAppearance.BorderSize = 0;
            button8.FlatAppearance.BorderColor = Color.Black;



            button3.FlatAppearance.BorderSize = 5;
            button3.FlatAppearance.BorderColor = Color.Black;


            LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;


            //label5 is timestamp label update each button click with current time - so people can keep track
            //of when the ran the report: ex) before running the job at 10am they may have a un-updated ticket info
            //from running the reprort like at 9am
            //thsi will make sure they are clicking buttons to refresh the report
            label5.Text = DateTime.Now.ToString();

            #endregion label /button UI

            #region start
            CrystalReport1 cryrpt = new CrystalReport1();

            //path to report
            //cryrpt.Load("VVT.test.CrystalReport1.rpt");

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




            //uses he path above as refernce

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
            // LaunchOrigin.crystalReportViewer1.Refresh(); //here is where i get the prompt DB login

            //report loads fully first time then user can customize the report by supressio
            #endregion


            #region hide subs
            //hide stock (paper)
            string sub11 = "Subreport11";
            HideSubs(cryrpt, sub11);

            //hide bindery matts
            string sub5 = "Subreport5";
            HideSubs(cryrpt, sub5);

            //hide free fields
            // string sub6 = "Subreport6";
            //HideSubs(cryrpt, sub6);

            //hide mailing ticket
            // string sub24 = "Subreport24";
            // HideSubs(cryrpt, sub24);

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

            //make it say mailign ticket cuz tghis is the mailing ticket
            CrystalDecisions.CrystalReports.Engine.TextObject txtObj2;
            txtObj2 = cryrpt.ReportDefinition.ReportObjects["ticketName"] as TextObject;
            txtObj2.Text = "Mailing Ticket";

            //hide section (Orange billing) on all but intial load and print ticket button
            try
            {
                cryrpt.ReportDefinition.Sections["GroupHeaderSection2"].SectionFormat.EnableSuppress = true;
            }
            catch (System.ArgumentException)
            {
                MessageBox.Show("Oof please refresh or button click again, report did not load correctly.");
            }

            CrystalDecisions.CrystalReports.Engine.TextObject txtObj;
            txtObj = cryrpt.ReportDefinition.ReportObjects["ticketName"] as TextObject;
            txtObj.Text = "Mail Ticket";



            #endregion hide subs



            #region query DB 
            //query dtaabse for 810 tag*********************************************

            #region 810 tag

            string connectStr = "DSN=Progress11;uid=Bob;pwd=Orchard";

            //get 810 to display Mailing 
            DataTable dt = new DataTable();

            using (OdbcConnection dbConn = new OdbcConnection(connectStr))
            {

                //for mailing we want to grab the any 810 notes and display them 


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
                String query = "SELECT \"Work-Center-ID\", \"TagStatus-ID\", Notes FROM PUB.ScheduleByJob WHERE \"Job-ID\"=" + jobNumberUser;

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

                    //remove all tags that do not contain 810


                }//end foreach

                //no check to see if 810 tag is present
                if (!check)
                {

                    CrystalDecisions.CrystalReports.Engine.TextObject txtObj3;
                    txtObj3 = cryrpt.ReportDefinition.ReportObjects["txtMail"] as TextObject;
                    txtObj3.Text = "";

                }//end check for 810


                //mailing producution reprort add


                #endregion 810

                #region tag status grab

                try
                {
                    //need to base of the 900 tag's -> tag status
                    string tagStat = "";

                    foreach (DataRow dr in dt.Rows)
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

                    string error = ex + " : Error in job status txt Obj change : ";
                    ErrorLog(error);

                }


                //here is where i should display the datatable of 810 tags and their notes

                dt.Columns.Remove("TagStatus-ID");

                DataTable dt810 = new DataTable();
                dt810 = dt.Clone();

                for (int i = dt.Rows.Count - 1; i >= 0; i--)
                {

                    DataRow dr = dt.Rows[i];
                    if (!dr["Work-Center-ID"].ToString().Contains("810"))
                    {

                        dr.Delete();

                    }

                }

                dt.AcceptChanges();

                DataSet ds810Notes = new DataSet();
                //change columnn names?
                dt.Columns["Work-Center-ID"].ColumnName = "810Tag";
                dt.Columns["Notes"].ColumnName = "810Note";

                ds810Notes.Tables.Add(dt);

                //now make a subreprot to take "Work-Center-ID and Notes" 

                cryrpt.Subreports[23].DataSourceConnections.Clear();
                cryrpt.Subreports[23].SetDataSource(ds810Notes.Tables[0]);


                #endregion tag status                


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


                for (int i = dtComments.Rows.Count - 1; i >= 0; i--)
                {

                    DataRow dr2 = dtComments.Rows[i];


                    string catId = dr2["SpecCategory-ID"].ToString();

                    if (catId != "00" && catId != "" && catId != "00" && catId != "03")
                    {
                        //can make it required that each row has to have aspecCategory-ID
                        //will ask tell user that there is no tag and to isnert one 
                        //actually use SQL update the record at hand
                        dr2.Delete();

                    }//end if


                    dtComments.AcceptChanges();
                }//end loop

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


                #region mailing sub reports

                string queryMail = "SELECT \"Free-Field-Char\" FROM PUB.MailingVersionFreeField WHERE \"Job-ID\" = " + jobNumberUser;

                OdbcDataAdapter adapMail = new OdbcDataAdapter(queryMail, dbConn);

                DataTable dtMail = new DataTable();

                DataColumn newCol = new DataColumn("Names", typeof(String));

                dtMail.Columns.Add(newCol);

                adapMail.Fill(dtMail);

                //here is where i can take some of these free fields off
                dtMail.Rows.RemoveAt(8);
                dtMail.Rows.RemoveAt(7);
                dtMail.Rows.RemoveAt(6);
                dtMail.Rows.RemoveAt(5);

                //check if empty set, meaning there is no mail data skip it
                if (dtMail.Rows.Count != 0)
                {
                    DataSet dsMail = new DataSet();

                    dsMail.Tables.Add(dtMail);

                    //add values to the "Names"" basically free field names
                    dtMail.Rows[0]["Names"] = "Permit Type";


                    cryrpt.Subreports[7].DataSourceConnections.Clear();
                    cryrpt.Subreports[7].SetDataSource(dsMail.Tables[0]);

                }
                else
                {

                    string subMail = "Subreport30";
                    HideSubs(cryrpt, subMail);

                }
                #endregion mailing sub reports

                #region free fields
                string queryFF = "SELECT \"Free-Field-CHar\", \"Free-Field-Decimal\" FROM PUB.JobFreeField WHERE \"Job-ID\" = " + jobNumberUser;

                OdbcDataAdapter adapFF = new OdbcDataAdapter(queryFF, dbConn);

                DataTable dtFF = new DataTable();

                DataColumn newColFF = new DataColumn("Names", typeof(String));

                dtFF.Columns.Add(newColFF);


                adapFF.Fill(dtFF);

                //remove rows { 0,2,3,4,6}
                dtFF.Rows.RemoveAt(6);
                dtFF.Rows.RemoveAt(4);
                dtFF.Rows.RemoveAt(3);
                dtFF.Rows.RemoveAt(2);
                dtFF.Rows.RemoveAt(0);

                //check if empty set, meaning there is no mail data skip it
                if (dtFF.Rows.Count != 0)
                {

                    //loop thru to get the 2nd free field, it is a number and stored in Free-Field-Decimal
                    int x = 0;
                    foreach (DataRow dr in dtFF.Rows)
                    {

                        if (x == 1)
                        {


                            dr["Free-Field-Char"] = dr["Free-Field-Decimal"].ToString();

                        }

                        x++;
                    }


                    DataSet dsFF = new DataSet();

                    dsFF.Tables.Add(dtFF);

                    //add values to the "Names"" basically free field names
                    dtFF.Rows[0]["Names"] = "Permit Type";


                    //remove columns except for the two we need
                    dtFF.Columns.Remove("Free-Field-Decimal");


                    cryrpt.Subreports[16].DataSourceConnections.Clear();
                    cryrpt.Subreports[16].SetDataSource(dsFF.Tables[0]);

                }

                #endregion free fields


                #region mail FF

                string queryMailFF = "SELECT \"Free-Field-Char\" FROM PUB.MailingFreeField WHERE \"Job-ID\" = " + jobNumberUser;

                OdbcDataAdapter adapMailFF = new OdbcDataAdapter(queryMailFF, dbConn);

                DataTable dtMailFF = new DataTable();

                DataColumn newColMFF = new DataColumn("Names", typeof(String));

                dtMailFF.Columns.Add(newColMFF);

                adapMailFF.Fill(dtMailFF);

                dtMailFF.Rows.RemoveAt(4);

                //check if empty set, meaning there is no mail data skip it
                if (dtMailFF.Rows.Count != 0)
                {
                    DataSet dsMailFF = new DataSet();

                    dsMailFF.Tables.Add(dtMailFF);

                    //add values to the "Names"" basically free field names
                    dtMailFF.Rows[0]["Names"] = "Permit Type";

                    cryrpt.Subreports[6].DataSourceConnections.Clear();
                    cryrpt.Subreports[6].SetDataSource(dsMailFF.Tables[0]);

                }
                else
                {

                    string subMailFF = "Subreport31";
                    HideSubs(cryrpt, subMailFF);

                }

                #endregion mail FF

                dbConn.Close();
            }//end odbc connection
            #endregion DB query

            #region set param vals and error check
            //try set parameter panel values
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
            LaunchOrigin.crystalReportViewer1.ReuseParameterValuesOnRefresh = true;
            LaunchOrigin.crystalReportViewer1.ReportSource = cryrpt;

            LaunchOrigin.crystalReportViewer1.Refresh();

            label3.Text = "Report loaded.";
            #endregion set param vals and error check

        }//end mailing
        #endregion mailing ticket








        #region shipping ticket
        //shipping, combined with Bindery button
        private void button5_Click(object sender, EventArgs e)
        {
            #region label/button UI
            label3.Text = "Please wait report loading..";

            jobNumberUser = textBox1.Text;

            button8.FlatAppearance.BorderSize = 0;
            button8.FlatAppearance.BorderColor = Color.Black;

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


            // button5.FlatAppearance.BorderSize = 5;
            // button5.FlatAppearance.BorderColor = Color.Black;


            LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;

            //label5 is timestamp label update each button click with current time - so people can keep track
            //of when the ran the report: ex) before running the job at 10am they may have a un-updated ticket info
            //from running the reprort like at 9am
            //thsi will make sure they are clicking buttons to refresh the report
            label5.Text = DateTime.Now.ToString();

            #endregion label/button UI

            #region start
            CrystalReport1 cryrpt = new CrystalReport1();

            //path to report
            //cryrpt.Load("VVT.test.CrystalReport1.rpt");

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




            //uses he path above as refernce

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
            // LaunchOrigin.crystalReportViewer1.Refresh(); //here is where i get the prompt DB login

            //report loads fully first time then user can customize the report by supressio

            #endregion

            #region query DB 

            #region 810 tag
            //query dtaabse for 810 tag*********************************************

            string connectStr = "DSN=Progress11;uid=Bob;pwd=Orchard";

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

                #endregion 810 tag

                #region tag status grab

                try
                {

                    //need to base of the 900 tag's -> tag status
                    string tagStat = "";

                    foreach (DataRow dr in dt.Rows)
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


                for (int i = dtComments.Rows.Count - 1; i >= 0; i--)
                {


                    DataRow dr2 = dtComments.Rows[i];
                    string catId = dr2["SpecCategory-ID"].ToString();

                    if (catId != "00" && catId != "" && catId != "00" && catId != "09")
                    {
                        //can make it required that each row has to have aspecCategory-ID
                        //will ask tell user that there is no tag and to isnert one 
                        //actually use SQL update the record at hand
                        dr2.Delete();

                    }//end if
                    dtComments.AcceptChanges();
                }//end loop

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

                #region mailing subReport Freefields

                string queryMail = "SELECT \"Free-Field-Char\" FROM PUB.MailingVersionFreeField WHERE \"Job-ID\" = " + jobNumberUser;

                OdbcDataAdapter adapMail = new OdbcDataAdapter(queryMail, dbConn);

                DataTable dtMail = new DataTable();

                DataColumn newCol = new DataColumn("Names", typeof(String));

                dtMail.Columns.Add(newCol);

                adapMail.Fill(dtMail);

                //check if empty set, meaning there is no mail data skip it
                if (dtMail.Rows.Count != 0)
                {
                    DataSet dsMail = new DataSet();

                    dsMail.Tables.Add(dtMail);

                    //add values to the "Names"" basically free field names
                    dtMail.Rows[0]["Names"] = "Permit Type";

                    cryrpt.Subreports[7].DataSourceConnections.Clear();
                    cryrpt.Subreports[7].SetDataSource(dsMail.Tables[0]);

                }
                else
                {

                    string subMail = "Subreport30";
                    HideSubs(cryrpt, subMail);

                }
                #endregion mailing sub reports


                #region free fields
                string queryFF = "SELECT \"Free-Field-CHar\", \"Free-Field-Decimal\" FROM PUB.JobFreeField WHERE \"Job-ID\" = " + jobNumberUser;

                OdbcDataAdapter adapFF = new OdbcDataAdapter(queryFF, dbConn);

                DataTable dtFF = new DataTable();

                DataColumn newColFF = new DataColumn("Names", typeof(String));

                dtFF.Columns.Add(newColFF);

                adapFF.Fill(dtFF);

                //check if empty set, meaning there is no mail data skip it
                if (dtFF.Rows.Count != 0)
                {

                    //loop thru to get the 2nd free field, it is a number and stored in Free-Field-Decimal
                    int x = 0;
                    foreach (DataRow dr in dtFF.Rows)
                    {

                        if (x == 1)
                        {


                            dr["Free-Field-Char"] = dr["Free-Field-Decimal"].ToString();

                        }

                        x++;
                    }


                    DataSet dsFF = new DataSet();

                    dsFF.Tables.Add(dtFF);

                    //add values to the "Names"" basically free field names
                    dtFF.Rows[0]["Names"] = "Permit Type";


                    //remove columns except for the two we need
                    dtFF.Columns.Remove("Free-Field-Decimal");


                    cryrpt.Subreports[16].DataSourceConnections.Clear();
                    cryrpt.Subreports[16].SetDataSource(dsFF.Tables[0]);

                }

                #endregion free fields


                #region mail FF

                string queryMailFF = "SELECT \"Free-Field-Char\" FROM PUB.MailingFreeField WHERE \"Job-ID\" = " + jobNumberUser;

                OdbcDataAdapter adapMailFF = new OdbcDataAdapter(queryMailFF, dbConn);

                DataTable dtMailFF = new DataTable();

                DataColumn newColMFF = new DataColumn("Names", typeof(String));

                dtMailFF.Columns.Add(newColMFF);

                adapMailFF.Fill(dtMailFF);

                //check if empty set, meaning there is no mail data skip it
                if (dtMailFF.Rows.Count != 0)
                {
                    DataSet dsMailFF = new DataSet();

                    dsMailFF.Tables.Add(dtMailFF);

                    //add values to the "Names"" basically free field names
                    dtMailFF.Rows[0]["Names"] = "Permit Type";

                    cryrpt.Subreports[6].DataSourceConnections.Clear();
                    cryrpt.Subreports[6].SetDataSource(dsMailFF.Tables[0]);

                }
                else
                {

                    string subMailFF = "Subreport31";
                    HideSubs(cryrpt, subMailFF);

                }

                #endregion mail FF


                dbConn.Close();
            }//end odbc connection
            #endregion DB query

            #region hide subs
            /*
            * 
           //hide pre-press matts
           string sub2 = "Subreport2";
           HideSubs(cryrpt, sub2);

           //hide press (Plates)
           string sub16 = "Subreport16";
           HideSubs(cryrpt, sub16);

           //hide press (Ink)
           string sub10 = "Subreport10";
           HideSubs(cryrpt, sub10);
           */
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
            //string sub8 = "Subreport8";
            // HideSubs(cryrpt, sub8);

            //hide job specs
            string sub7 = "Subreport7";
            HideSubs(cryrpt, sub7);

            //hide form notes
            //string sub15 = "Subreport15";
            //HideSubs(cryrpt, sub15);

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

            //hide section (Orange billing) on all but intial load and print ticket button
            try
            {
                cryrpt.ReportDefinition.Sections["GroupHeaderSection2"].SectionFormat.EnableSuppress = true;
            }
            catch (System.ArgumentException)
            {
                MessageBox.Show("Oof please refresh or button click again, report did not load correctly.");
            }
            #endregion hide subs


            #region set param values and error check, supress sections
            CrystalDecisions.CrystalReports.Engine.TextObject txtObj2;
            txtObj2 = cryrpt.ReportDefinition.ReportObjects["ticketName"] as TextObject;
            txtObj2.Text = "Shipping Ticket";

            //try pre-set paramter panel
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
            LaunchOrigin.crystalReportViewer1.ReuseParameterValuesOnRefresh = true;
            LaunchOrigin.crystalReportViewer1.ReportSource = cryrpt;

            LaunchOrigin.crystalReportViewer1.Refresh();

            label3.Text = "Report loaded.";

            #endregion set param values and error check, supress sections

        }//end shipping

        #endregion shipping ticket





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



            CrystalReport1 cryrpt = new CrystalReport1();

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

            /*

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

                string subMailVersionFF = "subMailVersionFF";
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

                string subMailFF = "subMailFF";
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
                cryrpt.Subreports[0].DataSourceConnections.Clear();
                cryrpt.Subreports[0].SetDataSource(dtJobNotes);

            }
            else
            {

                string subJobComments = "subJobComments";
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
                cryrpt.Subreports[11].DataSourceConnections.Clear();
                cryrpt.Subreports[11].SetDataSource(dtAlt);

            }
            else
            {

                string subAlt = "subAlt";
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
                cryrpt.Subreports[15].DataSourceConnections.Clear();
                cryrpt.Subreports[15].SetDataSource(dtFF);

            }
            else
            {

                string subFF = "subFreeFields";
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
                cryrpt.Subreports[17].DataSourceConnections.Clear();
                cryrpt.Subreports[17].SetDataSource(dtPOreq);

            }
            else
            {

                string subPOreq = "subPOreq";
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
                cryrpt.Subreports[16].DataSourceConnections.Clear();
                cryrpt.Subreports[16].SetDataSource(dtPOLine);

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
                cryrpt.Subreports[20].DataSourceConnections.Clear();
                cryrpt.Subreports[20].SetDataSource(dtShipTo);

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
                cryrpt.Subreports[14].DataSourceConnections.Clear();
                cryrpt.Subreports[14].SetDataSource(dtSpecForm);

            }
            else
            {

                string subShipTo = "subShipTo";
                HideSubs(cryrpt, subShipTo);

            }


            #endregion form spec subRpt

            */

            #endregion sub report creation


            dbConn.Close();
            #endregion DB close connection



            #region set param vals and error check
            //try set paramter panel values
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
            LaunchOrigin.crystalReportViewer1.ReuseParameterValuesOnRefresh = true;
            LaunchOrigin.crystalReportViewer1.ReportSource = cryrpt;

            LaunchOrigin.crystalReportViewer1.Refresh(); //here is where i get the prompt DB login

            label3.Text = "Report loaded.";
            #endregion set param valeus and error check

        }//end press/prepress

        #endregion press/prepress ticket







        #region bindery ticket
        //bindery
        private void button9_Click(object sender, EventArgs e)
        {
            #region labels/buttons UI
            label3.Text = "Please wait report loading..";

            jobNumberUser = textBox1.Text;

            button8.FlatAppearance.BorderSize = 0;
            button8.FlatAppearance.BorderColor = Color.Black;

            button1.FlatAppearance.BorderSize = 0;
            button1.FlatAppearance.BorderColor = Color.Black;

            button11.FlatAppearance.BorderSize = 0;
            button11.FlatAppearance.BorderColor = Color.Black;

            button3.FlatAppearance.BorderSize = 0;
            button3.FlatAppearance.BorderColor = Color.Black;

            //  button5.FlatAppearance.BorderSize = 0;
            // button5.FlatAppearance.BorderColor = Color.Black;


            button2.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.BorderColor = Color.Black;


            button9.FlatAppearance.BorderSize = 5;
            button9.FlatAppearance.BorderColor = Color.Black;

            LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;


            //label5 is timestamp label update each button click with current time - so people can keep track
            //of when the ran the report: ex) before running the job at 10am they may have a un-updated ticket info
            //from running the reprort like at 9am
            //thsi will make sure they are clicking buttons to refresh the report
            label5.Text = DateTime.Now.ToString();

            #endregion labels/buttons UI

            #region start
            CrystalReport1 cryrpt = new CrystalReport1();

            //path to report
            //cryrpt.Load("VVT.test.CrystalReport1.rpt");

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
            // LaunchOrigin.crystalReportViewer1.Refresh(); //here is where i get the prompt DB login

            //report loads fully first time then user can customize the report by supressio

            #endregion

            #region query DB 

            #region 810 tag
            //query dtaabse for 810 tag*********************************************

            string connectStr = "DSN=Progress11;uid=Bob;pwd=Orchard";


            CrystalDecisions.CrystalReports.Engine.TextObject txtObj2;
            txtObj2 = cryrpt.ReportDefinition.ReportObjects["ticketName"] as TextObject;
            txtObj2.Text = "Bindery Ticket";


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

                #endregion 810 tag

                #region tag status grab

                try
                {
                    //need to base of the 900 tag's -> tag status
                    string tagStat = "";

                    foreach (DataRow dr in dt.Rows)
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


                for (int i = dtComments.Rows.Count - 1; i >= 0; i--)
                {


                    DataRow dr2 = dtComments.Rows[i];
                    string catId = dr2["SpecCategory-ID"].ToString();

                    if (catId != "00" && catId != "" && catId != "00" && catId != "10")
                    {
                        //can make it required that each row has to have aspecCategory-ID
                        //will ask tell user that there is no tag and to isnert one 
                        //actually use SQL update the record at hand
                        dr2.Delete();

                    }//end if
                    dtComments.AcceptChanges();
                }//end loop

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

                #region mailing sub reports

                string queryMail = "SELECT \"Free-Field-Char\" FROM PUB.MailingVersionFreeField WHERE \"Job-ID\" = " + jobNumberUser;

                OdbcDataAdapter adapMail = new OdbcDataAdapter(queryMail, dbConn);

                DataTable dtMail = new DataTable();

                DataColumn newCol = new DataColumn("Names", typeof(String));

                dtMail.Columns.Add(newCol);

                adapMail.Fill(dtMail);

                //check if empty set, meaning there is no mail data skip it
                if (dtMail.Rows.Count != 0)
                {
                    DataSet dsMail = new DataSet();

                    dsMail.Tables.Add(dtMail);

                    //add values to the "Names"" basically free field names
                    dtMail.Rows[0]["Names"] = "Permit Type";

                    cryrpt.Subreports[7].DataSourceConnections.Clear();
                    cryrpt.Subreports[7].SetDataSource(dsMail.Tables[0]);

                }
                else
                {

                    string subMail = "Subreport30";
                    HideSubs(cryrpt, subMail);

                }
                #endregion mailing sub reports

                dbConn.Close();
            }//end odbc connection
            #endregion DB 810 check


            #region hide subs
            /*

          //hide pre-press matts
          string sub2 = "Subreport2";
          HideSubs(cryrpt, sub2);

          //hide press (Plates)
          string sub16 = "Subreport16";
          HideSubs(cryrpt, sub16);

          //hide press (Ink)
          string sub10 = "Subreport10";
          HideSubs(cryrpt, sub10);
          */
            //hide stock (paper)
            string sub11 = "Subreport11";
            HideSubs(cryrpt, sub11);

            //hide bindery matts
            //string sub5 = "Subreport5";
            //HideSubs(cryrpt, sub5);

            //hide 810 tag notes
            string sub12 = "Subreport12";
            HideSubs(cryrpt, sub12);

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
            //string sub8 = "Subreport8";
            // HideSubs(cryrpt, sub8);

            //hide job specs
            string sub7 = "Subreport7";
            HideSubs(cryrpt, sub7);


            //hide form notes
            // string sub15 = "Subreport15";
            // HideSubs(cryrpt, sub15);

            //hide pre-press
            string sub9 = "Subreport9";
            HideSubs(cryrpt, sub9);

            //hide press
            string sub3 = "Subreport3";
            HideSubs(cryrpt, sub3);

            //hide bindery
            // string sub4 = "Subreport4";
            // HideSubs(cryrpt, sub4);

            //hide the PO sub-reprort sas well
            string sub10 = "Subreport10";
            HideSubs(cryrpt, sub10);

            //hide mailing subreprort free freilds
            string sub30 = "Subreport30";
            HideSubs(cryrpt, sub30);

            //hide mailing subreprort free freilds
            string sub31 = "Subreport31";
            HideSubs(cryrpt, sub31);

            //hide section (Orange billing) on all but intial load and print ticket button
            try
            {
                cryrpt.ReportDefinition.Sections["GroupHeaderSection2"].SectionFormat.EnableSuppress = true;
            }
            catch (System.ArgumentException)
            {
                MessageBox.Show("Oof please refresh or button click again, report did not load correctly.");
            }
            #endregion hide subs


            //try pre-set param panel
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
            LaunchOrigin.crystalReportViewer1.ReuseParameterValuesOnRefresh = true;
            LaunchOrigin.crystalReportViewer1.ReportSource = cryrpt;

            LaunchOrigin.crystalReportViewer1.Refresh();

            label3.Text = "Report loaded.";

        }//end bidnery
        #endregion bindery ticket






        #region full report
        //full report
        private void button11_Click_1(object sender, EventArgs e)
        {
            #region label/button UI
            label3.Text = "Please wait report loading..";

            jobNumberUser = textBox1.Text;

            button8.FlatAppearance.BorderSize = 0;
            button8.FlatAppearance.BorderColor = Color.Black;

            button1.FlatAppearance.BorderSize = 0;
            button1.FlatAppearance.BorderColor = Color.Black;

            //button5.FlatAppearance.BorderSize = 0;
            //button5.FlatAppearance.BorderColor = Color.Black;

            button3.FlatAppearance.BorderSize = 0;
            button3.FlatAppearance.BorderColor = Color.Black;

            button9.FlatAppearance.BorderSize = 0;
            button9.FlatAppearance.BorderColor = Color.Black;


            button2.FlatAppearance.BorderSize = 0;
            button2.FlatAppearance.BorderColor = Color.Black;


            button11.FlatAppearance.BorderSize = 5;
            button11.FlatAppearance.BorderColor = Color.Black;


            LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;

            //label5 is timestamp label update each button click with current time - so people can keep track
            //of when the ran the report: ex) before running the job at 10am they may have a un-updated ticket info
            //from running the reprort like at 9am
            //thsi will make sure they are clicking buttons to refresh the report
            label5.Text = DateTime.Now.ToString();

            #endregion label/button UI

            #region start
            CrystalReport1 cryrpt = new CrystalReport1();

            //path to report
            //cryrpt.Load("VVT.test.CrystalReport1.rpt");

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

            #region query DB 

            #region 810 tag
            //query dtaabse for 810 tag*********************************************

            string connectStr = "DSN=Progress11;uid=Bob;pwd=Orchard";

            CrystalDecisions.CrystalReports.Engine.TextObject txtObj2;
            txtObj2 = cryrpt.ReportDefinition.ReportObjects["ticketName"] as TextObject;
            txtObj2.Text = "Full Ticket";


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

                #endregion 810 tag

                #region tag status grab

                try
                {
                    //need to base of the 900 tag's -> tag status
                    string tagStat = "";

                    foreach (DataRow dr in dt.Rows)
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

                #endregion jobNotes

                #region PO

                string POquery = "SELECT * FROM Pub.POLine WHERE \"Job-ID\" = " + jobNumberUser;

                OdbcDataAdapter adapPO = new OdbcDataAdapter(POquery, dbConn);

                //define a dataset to hold onto data
                DataTable dtPO = new DataTable();

                //here fill the datatset then manipulate, then fill again to fill Data Table "jobComments"
                adapPO.Fill(dtPO);


                dtPO.AcceptChanges();

                DataSet sendDsPO = new DataSet();

                sendDsPO.Tables.Add(dtPO);

                //HERE very importnat to spell name of datatable correctly AS IT IS IN SOLUTION EXPLORER
                //NOT CRYSTAL REPORTS
                adapPO.Fill(sendDsPO, "POline");

                //ALSO BIG NOTE: KNOW WHAT SUBREPORT IT INDEXED AT "5", i was going after the wrong subreprot
                //pretty much causing crystal reprorts to re-query DB for the data and grabbing all records
                //just running  a simle SELECT * from
                //bind the dataset (returned from SQL query to our reprort object
                //also clear the connections and then set the subreprrot


                cryrpt.Subreports[17].DataSourceConnections.Clear();
                cryrpt.Subreports[17].SetDataSource(sendDsPO.Tables[0]);


                #endregion PO

                #region mailing version FF

                string queryMail = "SELECT \"Free-Field-Char\" FROM PUB.MailingVersionFreeField WHERE \"Job-ID\" = " + jobNumberUser;

                OdbcDataAdapter adapMail = new OdbcDataAdapter(queryMail, dbConn);

                DataTable dtMail = new DataTable();

                DataColumn newCol = new DataColumn("Names", typeof(String));

                dtMail.Columns.Add(newCol);

                adapMail.Fill(dtMail);

                //check if empty set, meaning there is no mail data skip it
                if (dtMail.Rows.Count != 0)
                {
                    DataSet dsMail = new DataSet();

                    dsMail.Tables.Add(dtMail);

                    //add values to the "Names"" basically free field names
                    dtMail.Rows[0]["Names"] = "Permit Type";

                    cryrpt.Subreports[7].DataSourceConnections.Clear();
                    cryrpt.Subreports[7].SetDataSource(dsMail.Tables[0]);

                }
                else
                {

                    string subMail = "Subreport30";
                    HideSubs(cryrpt, subMail);

                }
                #endregion mailing sub reports

                #region free fields
                string queryFF = "SELECT \"Free-Field-CHar\", \"Free-Field-Decimal\" FROM PUB.JobFreeField WHERE \"Job-ID\" = " + jobNumberUser;

                OdbcDataAdapter adapFF = new OdbcDataAdapter(queryFF, dbConn);

                DataTable dtFF = new DataTable();

                DataColumn newColFF = new DataColumn("Names", typeof(String));

                dtFF.Columns.Add(newColFF);

                adapFF.Fill(dtFF);

                //check if empty set, meaning there is no mail data skip it
                if (dtFF.Rows.Count != 0)
                {

                    //loop thru to get the 2nd free field, it is a number and stored in Free-Field-Decimal
                    int x = 0;
                    foreach (DataRow dr in dtFF.Rows)
                    {

                        if (x == 1)
                        {


                            dr["Free-Field-Char"] = dr["Free-Field-Decimal"].ToString();

                        }

                        x++;
                    }


                    DataSet dsFF = new DataSet();

                    dsFF.Tables.Add(dtFF);

                    //add values to the "Names"" basically free field names
                    dtFF.Rows[0]["Names"] = "Permit Type";


                    //remove columns except for the two we need
                    dtFF.Columns.Remove("Free-Field-Decimal");


                    cryrpt.Subreports[16].DataSourceConnections.Clear();
                    cryrpt.Subreports[16].SetDataSource(dsFF.Tables[0]);

                }

                #endregion free fields


                #region mail FF

                string queryMailFF = "SELECT \"Free-Field-Char\" FROM PUB.MailingFreeField WHERE \"Job-ID\" = " + jobNumberUser;

                OdbcDataAdapter adapMailFF = new OdbcDataAdapter(queryMailFF, dbConn);

                DataTable dtMailFF = new DataTable();

                DataColumn newColMFF = new DataColumn("Names", typeof(String));

                dtMailFF.Columns.Add(newColMFF);

                adapMailFF.Fill(dtMailFF);

                //check if empty set, meaning there is no mail data skip it
                if (dtMailFF.Rows.Count != 0)
                {
                    DataSet dsMailFF = new DataSet();

                    dsMailFF.Tables.Add(dtMailFF);

                    //add values to the "Names"" basically free field names
                    dtMailFF.Rows[0]["Names"] = "Permit Type";

                    cryrpt.Subreports[6].DataSourceConnections.Clear();
                    cryrpt.Subreports[6].SetDataSource(dsMailFF.Tables[0]);

                }
                else
                {

                    string subMailFF = "Subreport31";
                    HideSubs(cryrpt, subMailFF);

                }

                #endregion mail FF


                dbConn.Close();
            }//end odbc connection
            #endregion DB 810 check

            #region surpress, set param valuess and erroc check

            //hide the mailing sub reports free fields

            //hide mailing subreprort free freilds
            string sub30 = "Subreport30";
            HideSubs(cryrpt, sub30);

            //hide mailing subreprort free freilds
            string sub31 = "Subreport31";
            HideSubs(cryrpt, sub31);

            //hide the press subrerprot -> this one has the "BlueLine" line information
            //not sure if hiding based on that or just hide subreport no matter what
            string sub3 = "Subreport3";
            HideSubs(cryrpt, sub3);

            //hide section (Orange billing) on all but intial load and print ticket button
            try
            {
                cryrpt.ReportDefinition.Sections["GroupHeaderSection2"].SectionFormat.EnableSuppress = true;
            }
            catch (System.ArgumentException)
            {
                MessageBox.Show("Oof please refresh or button click again, report did not load correctly.");
            }
            //try pre set param panel
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
            LaunchOrigin.crystalReportViewer1.ReuseParameterValuesOnRefresh = true;
            LaunchOrigin.crystalReportViewer1.ReportSource = cryrpt;

            LaunchOrigin.crystalReportViewer1.Refresh();
            label3.Text = "Report loaded.";

            #endregion surpress, param vals, error check

            //report loads fully first time then user can customize the report by supressio
            #endregion

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

            //button5.FlatAppearance.BorderSize = 0;
            // button5.FlatAppearance.BorderColor = Color.Black;

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

            CrystalReport1 cryrpt = new CrystalReport1();

            //path to report
            //cryrpt.Load("VVT.test.CrystalReport1.rpt");

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
            #region label/button UI
            label3.Text = "Please wait report loading..";

            jobNumberUser = textBox1.Text;

            // button5.FlatAppearance.BorderSize = 0;
            // button5.FlatAppearance.BorderColor = Color.Black;

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

            LaunchOrigin.crystalReportViewer1.ShowParameterPanelButton = false;

            //label5 is timestamp label update each button click with current time - so people can keep track
            //of when the ran the report: ex) before running the job at 10am they may have a un-updated ticket info
            //from running the reprort like at 9am
            //thsi will make sure they are clicking buttons to refresh the report
            label5.Text = DateTime.Now.ToString();

            #endregion label/button UI

            #region start
            CrystalReport1 cryrpt = new CrystalReport1();

            //path to report
            //cryrpt.Load("VVT.test.CrystalReport1.rpt");

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
            //LaunchOrigin.crystalReportViewer1.Refresh(); //here is where i get the prompt DB login

            //report loads fully first time then user can customize the report by supressio

            #endregion

            #region query DB 

            #region 810 tag
            //query dtaabse for 810 tag*********************************************

            string connectStr = "DSN=Progress11;uid=Bob;pwd=Orchard";


            CrystalDecisions.CrystalReports.Engine.TextObject txtObj2;
            txtObj2 = cryrpt.ReportDefinition.ReportObjects["ticketName"] as TextObject;
            txtObj2.Text = "PO Ticket";


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

                #endregion 810 tag

                #region tag status grab

                try
                {
                    //need to base of the 900 tag's -> tag status
                    string tagStat = "";

                    foreach (DataRow dr in dt.Rows)
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

                #region job notes
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


                for (int i = dtComments.Rows.Count - 1; i >= 0; i--)
                {


                    DataRow dr2 = dtComments.Rows[i];
                    string catId = dr2["SpecCategory-ID"].ToString();

                    if (catId != "00" && catId != "" && catId != "01" && catId != "05")
                    {
                        //can make it required that each row has to have aspecCategory-ID
                        //will ask tell user that there is no tag and to isnert one 
                        //actually use SQL update the record at hand
                        dr2.Delete();

                    }//end if
                    dtComments.AcceptChanges();
                }//end loop

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


                #region PO

                string POquery = "SELECT * FROM Pub.POLine WHERE \"Job-ID\" = " + jobNumberUser;

                OdbcDataAdapter adapPO = new OdbcDataAdapter(POquery, dbConn);

                //define a dataset to hold onto data
                DataTable dtPO = new DataTable();

                //here fill the datatset then manipulate, then fill again to fill Data Table "jobComments"
                adapPO.Fill(dtPO);


                dtPO.AcceptChanges();

                DataSet sendDsPO = new DataSet();

                sendDsPO.Tables.Add(dtPO);

                //HERE very importnat to spell name of datatable correctly AS IT IS IN SOLUTION EXPLORER
                //NOT CRYSTAL REPORTS
                adapPO.Fill(sendDsPO, "POline");

                //ALSO BIG NOTE: KNOW WHAT SUBREPORT IT INDEXED AT "5", i was going after the wrong subreprot
                //pretty much causing crystal reprorts to re-query DB for the data and grabbing all records
                //just running  a simle SELECT * from
                //bind the dataset (returned from SQL query to our reprort object
                //also clear the connections and then set the subreprro


                cryrpt.Subreports[17].DataSourceConnections.Clear();
                cryrpt.Subreports[17].SetDataSource(sendDsPO.Tables[0]);

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

                #endregion PO

                #region mailing sub reports

                string queryMail = "SELECT \"Free-Field-Char\" FROM PUB.MailingVersionFreeField WHERE \"Job-ID\" = " + jobNumberUser;

                OdbcDataAdapter adapMail = new OdbcDataAdapter(queryMail, dbConn);

                DataTable dtMail = new DataTable();

                DataColumn newCol = new DataColumn("Names", typeof(String));

                dtMail.Columns.Add(newCol);

                adapMail.Fill(dtMail);

                //check if empty set, meaning there is no mail data skip it
                if (dtMail.Rows.Count != 0)
                {
                    DataSet dsMail = new DataSet();

                    dsMail.Tables.Add(dtMail);

                    //add values to the "Names"" basically free field names
                    dtMail.Rows[0]["Names"] = "Permit Type";

                    cryrpt.Subreports[7].DataSourceConnections.Clear();
                    cryrpt.Subreports[7].SetDataSource(dsMail.Tables[0]);

                }
                else
                {

                    string subMail = "Subreport30";
                    HideSubs(cryrpt, subMail);

                }
                #endregion mailing sub reports

                #region free fields
                string queryFF = "SELECT \"Free-Field-CHar\", \"Free-Field-Decimal\" FROM PUB.JobFreeField WHERE \"Job-ID\" = " + jobNumberUser;

                OdbcDataAdapter adapFF = new OdbcDataAdapter(queryFF, dbConn);

                DataTable dtFF = new DataTable();

                DataColumn newColFF = new DataColumn("Names", typeof(String));

                dtFF.Columns.Add(newColFF);

                adapFF.Fill(dtFF);

                //check if empty set, meaning there is no mail data skip it
                if (dtFF.Rows.Count != 0)
                {

                    //loop thru to get the 2nd free field, it is a number and stored in Free-Field-Decimal
                    int x = 0;
                    foreach (DataRow dr in dtFF.Rows)
                    {

                        if (x == 1)
                        {


                            dr["Free-Field-Char"] = dr["Free-Field-Decimal"].ToString();

                        }

                        x++;
                    }


                    DataSet dsFF = new DataSet();

                    dsFF.Tables.Add(dtFF);

                    //add values to the "Names"" basically free field names
                    dtFF.Rows[0]["Names"] = "Permit Type";


                    //remove columns except for the two we need
                    dtFF.Columns.Remove("Free-Field-Decimal");


                    cryrpt.Subreports[16].DataSourceConnections.Clear();
                    cryrpt.Subreports[16].SetDataSource(dsFF.Tables[0]);

                }

                #endregion free fields


                #region mail FF

                string queryMailFF = "SELECT \"Free-Field-Char\" FROM PUB.MailingFreeField WHERE \"Job-ID\" = " + jobNumberUser;

                OdbcDataAdapter adapMailFF = new OdbcDataAdapter(queryMailFF, dbConn);

                DataTable dtMailFF = new DataTable();

                DataColumn newColMFF = new DataColumn("Names", typeof(String));

                dtMailFF.Columns.Add(newColMFF);

                adapMailFF.Fill(dtMailFF);

                //check if empty set, meaning there is no mail data skip it
                if (dtMailFF.Rows.Count != 0)
                {
                    DataSet dsMailFF = new DataSet();

                    dsMailFF.Tables.Add(dtMailFF);

                    //add values to the "Names"" basically free field names
                    dtMailFF.Rows[0]["Names"] = "Permit Type";

                    cryrpt.Subreports[6].DataSourceConnections.Clear();
                    cryrpt.Subreports[6].SetDataSource(dsMailFF.Tables[0]);

                }
                else
                {

                    string subMailFF = "Subreport31";
                    HideSubs(cryrpt, subMailFF);

                }

                #endregion mail FF


                dbConn.Close();
            }//end odbc connection
            #endregion DB 810 check

            #region hide subs
            /*

          //hide pre-press matts
          string sub2 = "Subreport2";
          HideSubs(cryrpt, sub2);

          //hide press (Plates)
          string sub16 = "Subreport16";
          HideSubs(cryrpt, sub16);

          //hide press (Ink)
          string sub10 = "Subreport10";
          HideSubs(cryrpt, sub10);
          */

            //hide 810 tag notes
            string sub12 = "Subreport12";
            HideSubs(cryrpt, sub12);

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
            //  string sub13 = "Subreport13";
            // HideSubs(cryrpt, sub13);

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

            //hide mailing subreprort free freilds
            string sub30 = "Subreport30";
            HideSubs(cryrpt, sub30);

            //hide mailing subreprort free freilds
            string sub31 = "Subreport31";
            HideSubs(cryrpt, sub31);


            //hide section (Orange billing) on all but intial load and print ticket button
            try
            {
                cryrpt.ReportDefinition.Sections["GroupHeaderSection2"].SectionFormat.EnableSuppress = true;
            }
            catch (System.ArgumentException)
            {
                MessageBox.Show("Oof please refresh or button click again, report did not load correctly.");
            }



            #endregion hide subs

            #region set param vals, error check

            //try set paramter panel values
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
            LaunchOrigin.crystalReportViewer1.ReuseParameterValuesOnRefresh = true;
            LaunchOrigin.crystalReportViewer1.ReportSource = cryrpt;

            LaunchOrigin.crystalReportViewer1.Refresh(); //here is where i get the prompt DB login

            label3.Text = "Report loaded.";

            #endregion set param vals and error check

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
        public void HideSubs(CrystalReport1 crObj, string str)
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
        public void ObjectIndexCheck(CrystalReport1 crObj)
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
