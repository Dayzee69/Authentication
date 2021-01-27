using System.Windows;
using System.Text.RegularExpressions;
using System;

namespace Authentication
{
    /// <summary>
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        public OrderWindow()
        {
            InitializeComponent();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            Subject subject = new Subject();

            try 
            {
                string regex = @"\d{1,10}";
                if (Regex.IsMatch(subIdTb.Text, regex))
                {
                    subject.id = int.Parse(subIdTb.Text);
                }
                else 
                {
                    throw new Exception("Поле 'SubId' не должно быть пустым и должно содержать данные от 1 до 10 цифр");
                }

                regex = @"\D{1}";
                if (Regex.IsMatch(first_nameTb.Text, regex))
                {
                    subject.name = first_nameTb.Text;
                }
                else
                {
                    throw new Exception("Поле 'Имя' не должно быть пустым");
                }

                regex = @"\D{1}";
                if (Regex.IsMatch(last_nameTb.Text, regex))
                {
                    subject.last_name = last_nameTb.Text;
                }
                else
                {
                    throw new Exception("Поле 'Фамилия' не должно быть пустым");
                }
                regex = @"\D{1}";
                if (Regex.IsMatch(second_nameTb.Text, regex))
                {
                    subject.second_name = second_nameTb.Text;
                }
                else
                {
                    throw new Exception("Поле 'Отчество' не должно быть пустым");
                }

                regex = @"\d{4,4}";
                if (Regex.IsMatch(p_serieTb.Text, regex))
                {
                    subject.p_serie = p_serieTb.Text;
                }
                else
                {
                    throw new Exception("Поле 'Серия' не должно быть пустым и должно содержать данные из 4 цифр");
                }
                regex = @"\d{6,6}";
                if (Regex.IsMatch(p_numberTb.Text, regex))
                {
                    subject.p_number = p_numberTb.Text;
                }
                else
                {
                    throw new Exception("Поле 'Номер' не должно быть пустым и должно содержать данные из 6 цифр");
                }
                subject.doc_type = doc_typeCb.Text;
                if (subject.doc_type == "СНИЛС")
                {
                    regex = @"\d{11,11}";
                    if (Regex.IsMatch(doc_numbTb.Text, regex))
                    {
                        String str = doc_numbTb.Text;
                        str = str.Insert(3, "-");
                        str = str.Insert(7, "-");
                        str = str.Insert(11, " ");
                        subject.doc_num = str;
                    }
                    else 
                    {
                        throw new Exception("Поле 'Док№' не должно быть пустым и должно содержать данные из 11 цифр ");
                    }
                }
                else 
                {
                    regex = @"\d{12,12}";
                    if (Regex.IsMatch(doc_numbTb.Text, regex ))
                    {
                        subject.doc_num = doc_numbTb.Text;
                    }
                    else
                    {

                        throw new Exception("Поле 'Док№' не должно быть пустым и должно содержать данные из 12 цифр ");
                    }
                }


                ((MainWindow)Application.Current.MainWindow).AddSubject(subject);
                Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
