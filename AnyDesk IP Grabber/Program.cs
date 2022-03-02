using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace AnyDesk_IP_Grabber
{
    class Program
    {

        static void Main()
        {
            Console.WriteLine("Getting connections");
            //Get all connections and convert them to the Connection Object (created by me)
            Connection.GetConnections();

            Console.WriteLine("Done, searching for anydesk connection.");

            foreach (var connection in Connection.connections)
            {
                if (connection.name.ToLower().Contains("anydesk"))
                {

                    //Filters:
                    if(connection.remoteport.ToString() != "443")
                    {
                        Console.WriteLine("---\nGot Connection\n" + "process: " + connection.pid + "\nremote address: " + connection.remoteaddy + "\nport: " + connection.remoteport + "\nProcess Name: " + connection.name+ "\n---");

                    }






                }

            }




        }
    }









    public class Connection
    {
        public static List<Connection> connections = new List<Connection>();

        public string remoteaddy;
        public string remoteport;

        public string localaddy;
        public string localport;

        public string pid;
        public string name;


        static readonly Regex trimmer = new Regex(@"  +");

        public static void GetConnections()
        {
            Process p = new Process();
            StreamReader sr;

            ProcessStartInfo startInfo = new ProcessStartInfo("netstat");
            startInfo.Arguments = "-n -a -o";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            p.StartInfo = startInfo;
            p.StartInfo.Verb = "runas";
            p.Start();

            sr = p.StandardOutput;

            using (sr)
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {

                    line = trimmer.Replace(line, " ");
                    if (line != "" && !line.Contains("UDP") && line.Contains("ESTABLISHED") || line.Contains("SYN_SENT"))
                    {
                        if (line.Contains("["))
                        {
                            return;
                        }
                        line = line.Remove(0, 1);





                        string[] args = line.Split(' ');

                        Connection connection = new Connection();

                        string[] local = args[1].ToString().Split(':');

                        string[] remote = args[2].ToString().Split(':');

                        connection.localaddy = local[0];
                        connection.localport = local[1];

                        connection.remoteaddy = remote[0];
                        connection.remoteport = remote[1];

                        connection.pid = args[4];

                        Process process = Process.GetProcessById(Int32.Parse(args[4]));

                        connection.name = process.ProcessName;

                        connections.Add(connection);




                    }

                }
            }
        }



    }


}