using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace libCSV
{
    public class CSV
    {
        public CSV()
        {
        }

        private DataTable csvTable = new DataTable();


        private void addHeader(ArrayList tokens)
        {
            foreach (object obj in tokens)
            {
                DataColumn col = new DataColumn();
                col.DataType = System.Type.GetType("System.String");
                col.ColumnName = obj as string;
                csvTable.Columns.Add(col);
            }
        }

        private void addRow(ArrayList tokens)
        {
            DataColumnCollection dc = csvTable.Columns;

            DataRow row = csvTable.NewRow();
            int dcn = 0;
            foreach (object obj in tokens)
            {
                row[dc[dcn]] = obj as string;
            }
            csvTable.Rows.Add(row);
        }


        public DataTable toDataTable(string filename)
        {
            bool needHeader = true;
            TextReader csvfile = File.OpenText(filename);
            String theLine;

            csvTable.Reset();

            while ((theLine = csvfile.ReadLine()) != null)
            {
                if (theLine == "")
                    continue;
                if ((theLine[0] == ';') || (theLine[0] == '#'))
                    continue;

                ArrayList tokens;
                tokens = parseLine(theLine);
                if (needHeader)
                {
                    addHeader(tokens);
                    needHeader = false;
                } else
                {
                    addRow(tokens);
                }
            }
            return csvTable;
        }

        public ArrayList parseLine(String theLine)
        {
            // Preparse out any quoted double-quotes
            theLine = theLine.Replace("\"\"", "\001");
            String[] rawfields = theLine.Split(',');
            ArrayList token = new ArrayList();
            bool quoted = false;        // Quoted indicates a CSV record that contains field-delimiting characters
                                        // as part of the field
            String current = "";
            int i;

            for (i = 0; i < rawfields.Length; i++)
            {
                if (quoted)
                {
                    // If we are presently consolidating a CSV record that was split apart
                    // due to internal comma's, then just append this record
                    current += rawfields[i];
                }
                else
                {
                    // We are just reading a singleton CSV field
                    current = rawfields[i];
                    // But check to see if we are starting a quoted CSV sequence
                    if(current.Length>0)
                        quoted = current[0] == '"';
                }

                if (quoted)
                {
                    // If we are in the process of reading a quoted CSV, check to see
                    // if we hit the closing quote to end the sequence
                    quoted = current[current.Length - 1] != '"';
                }

                if(!quoted || (i == rawfields.Length) )
                {
                    // If we aren't in sequence of quoted CSV's or if we hit the last record
                    // write the list of CSV tokens.

                    // Unencode that double double-quote
                    if (current != "")
                    {
                        current = current.Replace("\001", "\"\"");
                        if (current[0] == '"')
                        {
                            current = current.Substring(1, current.Length - 2);
                        }
                    }
                    token.Add(current);
                    current = "";
                }
            }

            return token;
        }



    }

}
