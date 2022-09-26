using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Test1
{
    
    public partial class MainWindow : Window
    {
        public class UsersDay
        {
            public int rank { get; set; }
            public string user { get; set; }
            public string status { get; set; }
            public int steps { get; set; }
            public int day { get; set; }
        }

        public class UsersData
        {
            public string userName { get; set; }
            public int avg { get; set; }
            public int max { get; set; }
            public int min { get; set; }
        }

        public class UserPerDay
        {
            public int day { get; set; }
            public int rank { get; set; }
            public string status { get; set; }
            public int steps { get; set; }
        }

        public class UserExport
        {
            public string user { get; set; }
            public int avg { get; set; }
            public int max { get; set; }
            public int min { get; set; }
            public List<UserPerDay> usersDays { get; set; }
        }

        List<UsersDay> result = new List<UsersDay>();
        List<string> userNames = new List<string>();
        List<UsersData> tableData = new List<UsersData>();


        public MainWindow()
         {
            
            InitializeComponent();
           
            loadJson();
            var usersNames = result.GroupBy(l => l.user);
            usersNames.Distinct();
            foreach (var username in usersNames)
            {
                userNames.Add(username.Key);
            }
            
            foreach(var username in userNames)
            {
                UsersData item = new UsersData();
                item.userName = username;
                item.avg = (int)result.Where(l => l.user == username).Select(l => l.steps).Average();
                item.min = result.Where(l => l.user == username).Select(l => l.steps).Min();
                item.max = result.Where(l => l.user == username).Select(l => l.steps).Max();
                tableData.Add(item);
            }
            for (int i = 0; i < tableData.Count; i++)
            {
                dgUsers.Items.Add(tableData[i]);
            }

        }


        public void loadJson() 
        {
            for(int i = 1; i <= 30; i++)
            {
                using (StreamReader r = new StreamReader("D:\\myJson\\day" + i.ToString() + ".json"))
                {
                    string json = r.ReadToEnd();
                    result.AddRange(JsonConvert.DeserializeObject<List<UsersDay>>(json));
                }
            }
            for (int i = 1; i <= 30; i++)
            {
                for (int a = 0; a < 100; a++)
                {
                    result[a + (i - 1) * 99].day = i;
                }
            }

        }

        private void dgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataGridRow rowSelected = (DataGridRow)gd.ItemContainerGenerator.ContainerFromIndex(gd.SelectedIndex);
            DataGridCell RowColumn = gd.Columns[0].GetCellContent(rowSelected).Parent as DataGridCell;
            string CellValue = ((TextBlock)RowColumn.Content).Text;
            if (rowSelected != null)
            {
                foreach (var username in userNames)
                {
                    if (username == CellValue)
                    {
                        WpfPlot1.Plot.Clear();
                        List<double> dataX = new List<double>();
                        List<double> dataY = new List<double>();
                        dataX.AddRange(result.Where(l => l.user == username).Select(l => Convert.ToDouble(l.day)));
                        dataY.AddRange(result.Where(l => l.user == username).Select(l => Convert.ToDouble(l.steps)));
                        WpfPlot1.Plot.AddScatter(dataX.ToArray(), dataY.ToArray());
                        WpfPlot1.Plot.Title("");
                        WpfPlot1.Plot.YLabel("Шаги");
                        WpfPlot1.Plot.XLabel("Дни");
                        var max = result.Where(l => l.user == username).Select(l => l.steps).Max();
                        var day = result.Where(l => l.steps == max).Select(l => l.day).Max();
                        WpfPlot1.Plot.AddPoint(day, max);
                        WpfPlot1.Refresh();
                    }
                }
            }
        }

        private void dgUsers_Loaded(object sender, RoutedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            for (int a = 0; a < 15; a++)
            {

                DataGridRow rowSelected = (DataGridRow)gd.ItemContainerGenerator.ContainerFromIndex(a);
                DataGridCell RowColumn = gd.Columns[0].GetCellContent(rowSelected).Parent as DataGridCell;
                string CellValue = ((TextBlock)RowColumn.Content).Text;
                if (rowSelected != null)
                {
                    foreach (var username in userNames)
                    {
                        if (username == CellValue)
                        {
                            if((int)tableData.Where(l=>l.userName == username).Select(l=>l.min).Min() < (int)tableData.Where(l => l.userName == username).Select(l => l.avg).Min() * 0.8)
                            {
                                rowSelected.Background = Brushes.Aqua;
                            }
                        }
                        if (username == CellValue)
                        {
                            if ((int)tableData.Where(l => l.userName == username).Select(l => l.max).Max() > (int)tableData.Where(l => l.userName == username).Select(l => l.avg).Min() * 1.2)
                            {
                                rowSelected.Background = Brushes.Aqua;
                            }
                        }
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Export();
        }

        public void Export()
        {
            List<UserExport> usExp = new List<UserExport>();
            foreach (var username in userNames)
            {
                UserExport us = new UserExport();
                us.user = username;
                foreach(var userdata in tableData)
                {
                    if(userdata.userName == username)
                    {
                        us.min = userdata.min;
                        us.max = userdata.max;
                        us.avg = userdata.avg;
                    }
                }
                List<UserPerDay> usersDays = new List<UserPerDay>();
                for(int i = 1; i <= 30; i++)
                {
                    UserPerDay usP = new UserPerDay();
                    usP.day = i;
                    usP.status = result.Where(l => l.user == username).Where(l => l.day == i).Select(l => l.status).FirstOrDefault();
                    usP.rank = result.Where(l => l.user == username).Where(l => l.day == i).Select(l => l.rank).FirstOrDefault();
                    usP.steps = result.Where(l => l.user == username).Where(l => l.day == i).Select(l => l.steps).FirstOrDefault();
                    usersDays.Add(usP);
                }
                us.usersDays = usersDays;
                usExp.Add(us);
                
            }

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter sw = new StreamWriter(@"D:\result.json"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, usExp);
                MessageBox.Show("successfull export!.");
            }
        }
    }
}
