﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
public partial class StudentReportPage : System.Web.UI.Page
{
    DataTable Table;
    protected void Page_Load(object sender, EventArgs e)
    {
        // Fetch data from the database using a select query
        Table = DataForReport();

        // Generate the report HTML dynamically
        string report = ReportGeneration(Table);

        // Save the report HTML to a file
        string Path = Server.MapPath("~/Reports/StudentSectionReport.html");
        SaveReport(report, Path);

        // Display the report in the browser
        Response.Clear();
        Response.ContentType = "text/html";
        Response.WriteFile(Path);
        Response.End();
    }

    private DataTable DataForReport()
    {
        string connection = "Data Source=DESKTOP-DGFGQKN\\SQLEXPRESS;Initial Catalog=flex;Integrated Security=True";
        using (SqlConnection con = new SqlConnection(connection))
        {
            string query = "Select enrollment.secId as 'Section', flexuser.id as 'Registration Number', flexuser.username as 'Student Name' From enrollment Inner Join flexuser ON enrollment.stuId = flexuser.id Where enrollment.secId = 'A' AND enrollment.cId = 'CS-2001';";
            SqlCommand cmd = new SqlCommand(query, con);
            con.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            return dataTable;
        }
    }

    private string ReportGeneration(DataTable table)
    {
        StringWriter stringWriter = new StringWriter();
        using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
        {

            writer.AddStyleAttribute("border-collapse", "collapse");
            writer.AddStyleAttribute("font-family", "Arial, sans-serif");
            writer.AddStyleAttribute("margin", "auto");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);



            // Generate the table header row
            writer.RenderBeginTag(HtmlTextWriterTag.Thead);
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            for (int i = 0; i < Table.Columns.Count; i++)
            {
                DataColumn col = Table.Columns[i];
                writer.AddStyleAttribute("background-color", "dodgerblue");
                writer.AddStyleAttribute("padding", "8px");
                writer.AddStyleAttribute("color", "White");
                writer.RenderBeginTag(HtmlTextWriterTag.Th);
                writer.Write(col.ColumnName);
                writer.RenderEndTag();
            }
            writer.RenderEndTag(); // </tr>
            writer.RenderEndTag(); // </thead>

            // Generate the table body rows
            writer.RenderBeginTag(HtmlTextWriterTag.Tbody);
            for (int i = 0; i < Table.Rows.Count; i++)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                DataRow row = Table.Rows[i];
                for (int j = 0; j < row.ItemArray.Length; j++)
                {
                    object item = row.ItemArray[j];
                    writer.AddStyleAttribute("border", "1px solid #ccc");
                    writer.AddStyleAttribute("padding", "8px");
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.Write(item.ToString());
                    writer.RenderEndTag();
                }
                writer.RenderEndTag();
            }
            writer.RenderEndTag();

            writer.RenderEndTag();

        }
        return stringWriter.ToString();
    }

    private void SaveReport(string rep, string path)
    {
        using (StreamWriter writer = new StreamWriter(path))
        {
            writer.Write(rep);
        }
    }
}
