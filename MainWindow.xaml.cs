using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using System.IO;
using System.Xml;
using Microsoft.Win32;
using System.Threading;
using System.Windows.Threading;

namespace Authentication
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Subject> subjects = new List<Subject>();
        string fileName = "";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void checkSPIDRequest(Subject sub)
        {
            WebRequest request = WebRequest.Create("http://host?wsdl");
            request.ContentType = "text/xml;charset=\"utf-8\"";

            request.Method = "POST";
            System.Threading.Thread.Sleep(1000);
            sub.messageId = DateTime.UtcNow.ToString();

            sub.messageId = sub.messageId.Replace(".", "");
            sub.messageId = sub.messageId.Replace(" ", "");
            sub.messageId = sub.messageId.Replace(":", "");

            string postData = "&";
            Console.WriteLine(sub.doc_type);
            if (sub.doc_type == "СНИЛС")
            {
                postData =
                $@"<s11:Envelope xmlns:s11='http://schemas.xmlsoap.org/soap/envelope/'>
                      <s11:Body>
                        <ns1:ws_ss_checkSPID xmlns:ns1='http://rsbank.softlab.ru/'>
                    <!-- optional -->
                          <SSSource>
                    <!-- optional -->
                            <messageID>{sub.messageId}</messageID>
                    <!-- optional -->
                            <senderID>{sub.senderId}</senderID>
                    <!-- optional -->
                            <senderPointID></senderPointID>
                          </SSSource>
                    <!-- optional -->
                          <SSASRequest>
                    <!-- optional -->
                            <firstName>{sub.name}</firstName>
                    <!-- optional -->
                            <INN></INN>
                    <!-- optional -->
                            <lastName>{sub.last_name}</lastName>
                    <!-- optional -->
                            <passportNumber>{sub.p_number}</passportNumber>
                    <!-- optional -->
                            <passportSeries>{sub.p_serie}</passportSeries>
                    <!-- optional -->
                            <patronymic>{sub.second_name}</patronymic>
                    <!-- optional -->
                            <SNILS>{sub.doc_num}</SNILS>
                    <!-- optional -->
                            <subjID>{sub.id}</subjID>
                          </SSASRequest>
                        </ns1:ws_ss_checkSPID>
                      </s11:Body>
                    </s11:Envelope>";
            }
            else if (sub.doc_type == "ИНН")
            {
                postData =
                $@"<s11:Envelope xmlns:s11='http://schemas.xmlsoap.org/soap/envelope/'>
                      <s11:Body>
                        <ns1:ws_ss_checkSPID xmlns:ns1='http://rsbank.softlab.ru/'>
                    <!-- optional -->
                          <SSSource>
                    <!-- optional -->
                            <messageID>{sub.messageId}</messageID>
                    <!-- optional -->
                            <senderID>{sub.senderId}</senderID>
                    <!-- optional -->
                            <senderPointID></senderPointID>
                          </SSSource>
                    <!-- optional -->
                          <SSASRequest>
                    <!-- optional -->
                            <firstName>{sub.name}</firstName>
                    <!-- optional -->
                            <INN>{sub.doc_num}</INN>
                    <!-- optional -->
                            <lastName>{sub.last_name}</lastName>
                    <!-- optional -->
                            <passportNumber>{sub.p_number}</passportNumber>
                    <!-- optional -->
                            <passportSeries>{sub.p_serie}</passportSeries>
                    <!-- optional -->
                            <patronymic>{sub.second_name}</patronymic>
                    <!-- optional -->
                            <SNILS></SNILS>
                    <!-- optional -->
                            <subjID>{sub.id}</subjID>
                          </SSASRequest>
                        </ns1:ws_ss_checkSPID>
                      </s11:Body>
                    </s11:Envelope>";

            }
            else
            {
                throw new Exception("Поле doc_type должно содержать либо 'СНИЛС', либо 'ИНН'");
            }

            StreamWriter sw = new StreamWriter(request.GetRequestStream());
            sw.WriteLine(postData);
            sw.Close();

            Thread myThread = new Thread(() => checkSPIDResponce(request, sub));
            myThread.Start();


        }

        private void checkSPIDResponce(WebRequest request, Subject sub)
        {
            try
            {
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());

                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(reader);
                XmlElement xRoot = xDoc.DocumentElement;

                XmlNode node = xRoot.SelectSingleNode("//SSDescription/objectID");
                sub.objectId = int.Parse(node.InnerText);

                node = xRoot.SelectSingleNode("//SSDescription/senderPointID");
                if (node.InnerText != "")
                    sub.senderPointID = int.Parse(node.InnerText);
                reader.Close();

                dataGrid.Dispatcher.Invoke(DispatcherPriority.Background, new
                 Action(() =>
                 {
                     DataGridUpdate();
                 ;
                 }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void getSPIDRequest(Subject sub)
        {
            WebRequest request = WebRequest.Create("http://host?wsdl");
            request.ContentType = "text/xml;charset=\"utf-8\"";
            request.Method = "POST";

            string postData =
            $@"<s11:Envelope xmlns:s11='http://schemas.xmlsoap.org/soap/envelope/'>
                   <s11:Body>
                     <ns1:ws_ss_getSPID xmlns:ns1='http://rsbank.softlab.ru/'>
                 <!-- optional -->
                       <SSSource>
                 <!-- optional -->
                         <messageID>{sub.messageId}</messageID>
                 <!-- optional -->
                         <senderID>{sub.senderId}</senderID>
                 <!-- optional -->
                         <senderPointID>{sub.senderPointID}</senderPointID>
                       </SSSource>
                 <!-- optional -->
                       <ObjectID>{sub.objectId}</ObjectID>
                 <!-- optional -->
                       <MessageInitID>{sub.messageInitID}</MessageInitID>
                     </ns1:ws_ss_getSPID>
                   </s11:Body>
                 </s11:Envelope>";

            StreamWriter sw = new StreamWriter(request.GetRequestStream());
            sw.WriteLine(postData);
            sw.Close();

            Thread myThread = new Thread(() => getSPIDResponce(request, sub));
            myThread.Start();

        }

        private void getSPIDResponce(WebRequest request, Subject sub)
        {
            try
            {
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());

                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(reader);
                XmlElement xRoot = xDoc.DocumentElement;

                XmlNode node = xRoot.SelectSingleNode("//SSExtError/returnCode");
                sub.returnCode = node.InnerText;

                node = xRoot.SelectSingleNode("//SSDescription/statusName");
                sub.statusName = node.InnerText;

                if (sub.returnCode.Length > 7)
                {
                    node = xRoot.SelectSingleNode("//SSExtError/returnText");
                    sub.returnText = node.InnerText;
                }
                reader.Close();
                dataGrid.Dispatcher.Invoke(DispatcherPriority.Background, new
                 Action(() =>
                 {
                     DataGridUpdate();
                     ;
                 }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void checkSPIDStart(List<Subject> sub) 
        {
            
            try
            {
                foreach (var item in sub)
                {
                    checkSPIDRequest(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void getSPIDStart(List<Subject> sub)
        {
            try
            {
                foreach (var item in sub)
                {
                    getSPIDRequest(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void openFile_Button_Click(object sender, RoutedEventArgs e)
        {

            subjects.Clear();

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Text files(*.csv)|*.csv|All files(*.*)|*.*";
            openFile.ShowDialog();

            if (openFile.FileName == "")
               return;
            fileName = openFile.FileName;

            string[] fileText = File.ReadAllLines(fileName, Encoding.Default);

            try
            {
                for (int i = 1; i < fileText.Length; i++)
                {

                    Subject subject = new Subject(fileText[i]);
                    //if (subject.id == 0 || subject.name == "" || subject.last_name == "" || subject.second_name == "" || subject.p_serie == "" || subject.p_number == "" || subject.doc_num == "")
                    if (subject.id == 0 || subject.name == "" || subject.last_name == "" || subject.second_name == "" || subject.p_serie == "" || subject.p_number == "")
                            throw new Exception("Поля id, last_name, name, second_name, p_serie, p_number, doc_num обязательны для заполнения");
                    subjects.Add(subject);
                }

                DataGridUpdate();

                senderId.Text = "";
                senderId.IsReadOnly = false;
                get_Button.IsEnabled = false;
                check_Button.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //MessageBox.Show(ex.Message);
                filePath.Text = "";

            }

        }

        private void Check_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 0; i < subjects.Count(); i++)
                    { 
                        subjects[i].senderId = senderId.Text;
                    }
                DataGridUpdate();

                Thread myThread = new Thread(() => checkSPIDStart(subjects));
                    myThread.Start();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void Get_Button_Click(object sender, RoutedEventArgs e)
        {

            Thread myThread = new Thread(() => getSPIDStart(subjects));
            myThread.Start();

            saveFile();

        }

        private void SenderId_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (senderId.Text != "")
            {
                check_Button.IsEnabled = true;
                get_Button.IsEnabled = true;
            }
            else 
            {
                check_Button.IsEnabled = false;
                get_Button.IsEnabled = false;
            }
        }

        private void DataGridUpdate() 
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = subjects;
            saveFile();
        }

        private void saveFile()
        {

            if (fileName == "") 
            {
                string strDate = DateTime.UtcNow.ToString();
                strDate = strDate.Replace(".", "");
                strDate = strDate.Replace(" ", "");
                strDate = strDate.Replace(":", "");
                fileName = subjects[0].senderId + "_" + strDate + ".csv";
            }

            string headers = "id,email,personal_mobile,last_name,name,second_name,p_serie,p_number,personal_birthday,p_date,p_who,personal_zip,doc_type," +
            "doc_num,resident,identification,senderId,messageId,objectId,senderPointID,messageInitID,statusName,returnCode,returnText";
            headers = headers.Replace(',', ';');
            string strFile = headers;
            for (int i = 0; i < subjects.Count(); i++)
            {
                strFile += "\r\n";
                strFile += subjects[i].id.ToString() + ";";
                strFile += subjects[i].email + ";";
                strFile += subjects[i].personal_mobile + ";";
                strFile += subjects[i].last_name + ";";
                strFile += subjects[i].name + ";";
                strFile += subjects[i].second_name + ";";
                strFile += subjects[i].p_serie + ";";
                strFile += subjects[i].p_number + ";";
                strFile += subjects[i].personal_birthday + ";";
                strFile += subjects[i].p_date + ";";
                strFile += subjects[i].p_who + ";";
                strFile += subjects[i].personal_zip + ";";
                strFile += subjects[i].doc_type + ";";
                strFile += subjects[i].doc_num + ";";
                strFile += subjects[i].resident + ";";
                strFile += subjects[i].identification + ";";
                strFile += subjects[i].senderId + ";";
                strFile += subjects[i].messageId + ";";
                strFile += subjects[i].objectId + ";";
                strFile += subjects[i].senderPointID + ";";
                strFile += subjects[i].messageInitID + ";";
                strFile += subjects[i].statusName + ";";
                strFile += subjects[i].returnCode + ";";
                strFile += subjects[i].returnText + ";";

            }

            File.WriteAllText(fileName, strFile,Encoding.GetEncoding(1251));
        }

        public void AddSubject(Subject sub) 
        {
            subjects.Add(sub);
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = subjects;
            senderId.IsReadOnly = false;
        }

        private void orderItem_Click(object sender, RoutedEventArgs e)
        {
            OrderWindow orderWindow = new OrderWindow();
            orderWindow.Owner = this;
            orderWindow.ShowDialog();
        }
    }
}
