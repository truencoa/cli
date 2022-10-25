using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Net;
using System.Text;

namespace truencoa_cli
{
    class Program
    {
        static void Main(string[] args)
        {
            int batchsize = 100;
            File file = null;

            try
            {
                if (args.Length < 3) { Console.WriteLine("Missing arguments: filename, id, key"); return; };

                string filename = args[0];
                string id = args[1];
                string key = args[2];
                string url = "https://api.truencoa.com/";
                bool download = false;
                string tmpfilename = null;

                bool suppress = false;
                bool charge = false;

                if (args.Length > 3)
                {
                    if (args[3].Contains("http"))
                    {
                        url = args[3];
                    }
                    else
                    {
                        Boolean.TryParse(args[3], out download);
                    }
                }

                if (args.Length > 4)
                {
                    if (!args[4].Contains("http"))
                    {
                        Boolean.TryParse(args[4], out download);
                    }
                }

                if (args.Length > 5)
                {
                    Boolean.TryParse(args[5], out suppress);
                }

                if (args.Length > 6)
                {
                    Boolean.TryParse(args[5], out charge);
                }

                // the filename could be a file and path to upload, or an existing file to process
                FileInfo fi = new FileInfo(filename);
                if (fi.Exists == false)
                {
                    tmpfilename = filename;
                }
                else
                {
                    // make a random file name
                    tmpfilename = fi.Name.Replace(System.IO.Path.GetExtension(fi.Name), "") + "_" + DateTime.Now.Ticks;

                    // import records from file
                    StringBuilder data = new StringBuilder();
                    using (StreamReader sr = new StreamReader(filename))
                    {
                        string[] headers = sr.ReadLine().Split('\t');
                        int i = 0;
                        while (sr.EndOfStream == false)
                        {
                            string[] line = sr.ReadLine().Split('\t');

                            int h = 0;
                            foreach (string header in headers)
                            {
                                data.AppendFormat("{0}={1}&", header, line[h]);
                                h++;
                            }
                            if (i % batchsize == 0 || sr.EndOfStream == true)
                            {
                                using (WebClient wc = new WebClient())
                                {
                                    //wc.Headers["api_id"] = id;
                                    //wc.Headers["api_key"] = key;
                                    wc.Headers["user_name"] = id;
                                    wc.Headers["password"] = key;
                                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                                    wc.UploadString(url + $"files/{tmpfilename}/records", data.ToString());
                                    data = new StringBuilder();
                                }
                            }
                            i++;
                        }
                    }
                }

                // check to see if the file is ready to process
                using (WebClient wc = new WebClient())
                {
                    //wc.Headers["api_id"] = id;
                    //wc.Headers["api_key"] = key;
                    wc.Headers["user_name"] = id;
                    wc.Headers["password"] = key;
                    try
                    {
                        string json = wc.DownloadString(url + $"files/{tmpfilename}");
                        file =  JsonConvert.DeserializeObject<File>(json);
                        if (file.Status != "Mapped" && file.RecordCount < 100)
                        {
                            Console.WriteLine($"The filename: {tmpfilename} is not in the correct status or does not contain at least 100 records");
                            return;
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Invalid filename: {tmpfilename}");
                        return;
                    }
                }

                // submit for processing
                using (WebClient wc = new WebClient())
                {
                    //wc.Headers["api_id"] = id;
                    //wc.Headers["api_key"] = key;
                    wc.Headers["user_name"] = id;
                    wc.Headers["password"] = key;
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    wc.UploadString(url + $"files/{tmpfilename}", "PATCH", "status=submit");
                }

                // wait for processing to complete
                bool processing = true;
                while (processing)
                {
                    Thread.Sleep(1000);
                    using (WebClient wc = new WebClient())
                    {
                        //wc.Headers["api_id"] = id;
                        //wc.Headers["api_key"] = key;
                        wc.Headers["user_name"] = id;
                        wc.Headers["password"] = key;
                        string json = wc.DownloadString(url + $"files/{tmpfilename}");
                        file = JsonConvert.DeserializeObject<File>(json);
                        processing = (file.Status == "Import" || file.Status == "Importing" || file.Status == "Parse" || file.Status == "Parsing" || file.Status == "Validate" || file.Status == "Validating" || file.Status == "Report" || file.Status == "Reporting" || file.Status == "Process" || file.Status == "Processing");
                    }
                }

                string exportfileid = null;
                // submit for exporting
                using (WebClient wc = new WebClient())
                {
                    //wc.Headers["api_id"] = id;
                    //wc.Headers["api_key"] = key;
                    wc.Headers["user_name"] = id;
                    wc.Headers["password"] = key;
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    string json = wc.UploadString(url + $"files/{tmpfilename}", "PATCH", $"status=export&suppress={suppress}");
                    file = JsonConvert.DeserializeObject<File>(json);
                    exportfileid = file.Id;
                }

                // wait for exporting to complete
                bool exporting = true;
                while (exporting)
                {
                    Thread.Sleep(1000);
                    using (WebClient wc = new WebClient())
                    {
                        //wc.Headers["api_id"] = id;
                        //wc.Headers["api_key"] = key;
                        wc.Headers["user_name"] = id;
                        wc.Headers["password"] = key;
                        string json = wc.DownloadString(url + $"files/{exportfileid}");
                        file = JsonConvert.DeserializeObject<File>(json);
                        exporting = (file.Status == "Export" || file.Status == "Exporting");
                    }
                }

                if (download)
                {

                    int page = 1;
                    int recordcount = 0;

                    filename = $"{filename}.export.csv";

                    if (System.IO.File.Exists(filename))
                    {
                        System.IO.File.Delete(filename);
                    }

                    using (StreamWriter sw = new StreamWriter(filename))
                    {
                        while (page == 1 || (recordcount > 0))
                        {
                            using (WebClient wc = new WebClient())
                            {
                                //wc.Headers["api_id"] = id;
                                //wc.Headers["api_key"] = key;
                                wc.Headers["user_name"] = id;
                                wc.Headers["password"] = key;
                                string json = wc.DownloadString(url + $"files/{exportfileid}/records?page={page}&charge={charge}");
                                var obj = JObject.Parse(json);
                                var recordsjson = (string)obj["Records"].ToString();
                                DataTable records = (DataTable)JsonConvert.DeserializeObject(recordsjson, (typeof(DataTable)));
                                recordcount = records.Rows.Count;
                                if (page == 1)
                                {
                                    IEnumerable<string> columnNames = records.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                                    sw.WriteLine(string.Join(",", columnNames));
                                }
                                foreach (DataRow row in records.Rows)
                                {
                                    IEnumerable<string> fields = row.ItemArray.Select(field => string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                                    sw.WriteLine(string.Join(",", fields));
                                }
                            }
                            page++;
                        }
                    }

                    Thread.Sleep(5000);
                    using (WebClient wc = new WebClient())
                    {
                        List<string> reports = new List<string>() { "ncoa", "cass" };
                        foreach (string report in reports)
                        {
                            wc.Headers["user_name"] = id;
                            wc.Headers["password"] = key;
                            byte[] pdf = wc.DownloadData(url + $"files/{tmpfilename}/reports?report_name={report}&format=pdf");
                            System.IO.File.WriteAllBytes($"{filename}.{report}.pdf", pdf);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured while processing your file:");
                Console.WriteLine(((System.Net.HttpWebResponse)((System.Net.WebException)ex).Response).StatusDescription);
            }
        }
    }

    public class File
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string Id { get; set; }
        public int RecordCount { get; set; }
        // you can uncomment this if you are not using the datatable, saves memory on deserialization
        // file = JsonConvert.DeserializeObject<File>(json);
        // public List<Record> Records { get; set; }
    }

    public class Record
    {
        public string input_individual_id { get; set; }
        public string input_individual_first_name { get; set; }
        public string input_individual_last_name { get; set; }
        public string input_address_line_1 { get; set; }
        public object input_address_line_2 { get; set; }
        public string input_address_city { get; set; }
        public string input_address_state_code { get; set; }
        public string input_address_postal_code { get; set; }
        public string input_address_country_code { get; set; }
        public int global_id { get; set; }
        public int record_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public object company_name { get; set; }
        public string street_number { get; set; }
        public string street_pre_direction { get; set; }
        public string street_name { get; set; }
        public string street_post_direction { get; set; }
        public string street_suffix { get; set; }
        public string unit_type { get; set; }
        public string unit_number { get; set; }
        public string box_number { get; set; }
        public string city_name { get; set; }
        public string state_code { get; set; }
        public string postal_code { get; set; }
        public string postal_code_extension { get; set; }
        public string carrier_route { get; set; }
        public string address_status { get; set; }
        public string error_number { get; set; }
        public string address_type { get; set; }
        public string delivery_point { get; set; }
        public string check_digit { get; set; }
        public string delivery_point_verification { get; set; }
        public string delivery_point_verification_notes { get; set; }
        public string vacant { get; set; }
        public string congressional_district_code { get; set; }
        public string area_code { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string time_zone { get; set; }
        public string county_name { get; set; }
        public string county_fips { get; set; }
        public string state_fips { get; set; }
        public string barcode { get; set; }
        public object lacs { get; set; }
        public string line_of_travel { get; set; }
        public string ascending_descending { get; set; }
        public string move_applied { get; set; }
        public string move_type { get; set; }
        public string move_date { get; set; }
        public double? move_distance { get; set; }
        public string match_flag { get; set; }
        public string nxi { get; set; }
        public object ank { get; set; }
        public string residential_delivery_indicator { get; set; }
        public string record_type { get; set; }
        public string record_source { get; set; }
        public string country_code { get; set; }
        public string address_line_1 { get; set; }
        public string address_line_2 { get; set; }
        public int address_id { get; set; }
        public int household_id { get; set; }
        public int individual_id { get; set; }
    }
}