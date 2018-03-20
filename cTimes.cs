using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace WorksheetLog
{
    [Serializable()]
    public class cTimes
    {


        public bool connectOnWLAN = false;
        public String WLANID = "";
        public Hashtable Dates = new Hashtable();

        public cTimes()
        {
        }

        public void Export(DateTime von, DateTime bis)
        {
            Hashtable exportDates = new Hashtable();
            ArrayList sortedDate = new ArrayList();
            foreach (String date in Dates.Keys)
            {
                if (toDateTime(date) >= von && toDateTime(date) <= bis)
                {
                    sortedDate.Add(toDateTime(date));
                }
            }
            sortedDate = sort(sortedDate);
            output(sortedDate);
            //File.Move(".\\data.dat", ".\\data.dat." + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToShortTimeString().Replace(":","."));
        }

        private void output(ArrayList sortedDate)
        {
            String Ret = "Time Sheet\r\n--------------------------------------------------\r\n\r\n";

            foreach (DateTime date in sortedDate)
            {
                String sDate = date.ToShortDateString();
                String von = ((String)((Hashtable)Dates[sDate])["Start"]);
                String bis = ((String)((Hashtable)Dates[sDate])["End"]);
                Ret += sDate + "\t" + von + "-" + bis + "\r\n";
            }
            File.WriteAllText(".\\output.txt", Ret);
            Process.Start("notepad.exe", ".\\output.txt");
        }

        private ArrayList sort(ArrayList sortedDate)
        {
            for (int x = 0; x < sortedDate.Count; x++)
            {
                for (int i = 0; i < sortedDate.Count; i++)
                {
                    if ((DateTime)sortedDate[x] > (DateTime)sortedDate[i])
                    {
                        DateTime temp = (DateTime)sortedDate[x];
                        sortedDate[x] = sortedDate[i];
                        sortedDate[i] = temp;
                    }
                }
            }
            return sortedDate;
        }

        private DateTime toDateTime(String date)
        {
            DateTime myDate;
            try
            {
                myDate = DateTime.ParseExact(date, "dd.MM.yyyy",
                                           System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {            
                myDate = DateTime.ParseExact(date, "MM/dd/yyyy",
                                        System.Globalization.CultureInfo.InvariantCulture);
            }
            return myDate;
        }

        //Serializing the List
        public void Save(cTimes emps, String filename)
        {
            //Create the stream to add object into it.
            System.IO.Stream ms = File.OpenWrite(filename);
            //Format the object as Binary

            BinaryFormatter formatter = new BinaryFormatter();
            //It serialize the employee object
            formatter.Serialize(ms, emps);
            ms.Flush();
            ms.Close();
            ms.Dispose();
        }

        public cTimes Load(String filename)
        {
            //Format the object as Binary
            BinaryFormatter formatter = new BinaryFormatter();

            //Reading the file from the server
            FileStream fs = File.Open(filename, FileMode.Open);

            object obj = formatter.Deserialize(fs);
            cTimes emps = (cTimes)obj;
            fs.Flush();
            fs.Close();
            fs.Dispose();
            return emps;

        }


        public void addTimestamp()
        {
            if (!Dates.ContainsKey(DateTime.Now.Date.ToShortDateString()))
            {
                Hashtable start = new Hashtable();
                start.Add("Start", DateTime.Now.TimeOfDay.ToString().Split('.')[0]);
                Dates.Add(DateTime.Now.Date.ToShortDateString(), start);
            }
            else
            {
                if (((Hashtable)Dates[DateTime.Now.Date.ToShortDateString()]).Count == 1)
                {
                    ((Hashtable)Dates[DateTime.Now.Date.ToShortDateString()]).Add("End", DateTime.Now.TimeOfDay.ToString().Split('.')[0]);
                }
                else
                {
                    ((Hashtable)Dates[DateTime.Now.Date.ToShortDateString()])["End"] = DateTime.Now.TimeOfDay.ToString().Split('.')[0];
                }
            }
        }
    }
}
