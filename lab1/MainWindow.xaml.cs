using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Windows;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Win32;


namespace lab1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GenerationLink_Click(object sender, RoutedEventArgs e)
        {
            txtKey.Text = Polibian.GetKey();
        }

        private void LoadLink_Click(object sender, RoutedEventArgs e)
        {
            string key = GetFileText();

            if (Polibian.IsStringValid(key))
                txtKey.Text = key;
            else
                MessageBox.Show("Ключ поврежден");

        }

        private string GetFileText()
        {
            var dlg = new OpenFileDialog();
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                return File.ReadAllText(filename);
            }
            return String.Empty;
        }

        private void SaveFileText(string text)
        {
            var dlg = new SaveFileDialog();
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                File.WriteAllText(filename, text);
            }
        }

        private void SaveLink_Click(object sender, RoutedEventArgs e)
        {
            SaveFileText(txtKey.Text);
        }

        private void LoadFileLink_Click(object sender, RoutedEventArgs e)
        {
            txtInputInformation.Text = GetFileText();
        }

        private void SaveFileLink_Click(object sender, RoutedEventArgs e)
        {
            SaveFileText(txtOutputInformation.Text);
        }

        private void btnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            if (IsInputValid())
            {
                var polibian = new Polibian(txtKey.Text);
                txtOutputInformation.Text = polibian.EncryptText(txtInputInformation.Text);
            }

        }

        private void btnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            if (IsInputValid())
            {
                var polibian = new Polibian(txtKey.Text);
                txtOutputInformation.Text = polibian.DecryptText(txtInputInformation.Text);
            }
        }

        public bool IsInputValid()
        {
            if (!String.IsNullOrEmpty(txtKey.Text))
            {
                var lines = txtKey.Text.Split('\r', '\n');

                foreach (var item in lines)
                {
                    if (item.Length != 0)
                    {
                        if (!Polibian.IsStringValid(item))
                        {
                            MessageBox.Show("Допустимы буквы русского алфавита и символы .,!?;\"-'");
                            return false;
                        }
                            
                    }
                }
                return true;
            }
            else
            {
                MessageBox.Show("Введите ключ");
                return false;
            }
        }

        private void CryptoAnalysis_Click(object sender, RoutedEventArgs e)
        {
            textByAnalysis.Text = txtOutputInformation.Text;
            tabControl.SelectedIndex = 1;
        }

        private void btnDecrypt_Analisys_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(textByAnalysis.Text)) { MessageBox.Show("Введите текст для дешифрования"); return;}
            if (!MyDictionary.IsInit()) { MessageBox.Show("Загрузите словарь"); return;}


            var timer = DateTime.Now;

            var decryption = new Decryption();
            AnalisysResult.Text = decryption.DecryptText(textByAnalysis.Text, txtInputInformation.Text.ToLower()); //TODO Debug

            var workTime = timer -  DateTime.Now;
            MessageBox.Show("Процесс дешифрации завершен. Время работы " + workTime.TotalSeconds + " секунды");
        }

        private void dictionaryLoad_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                var arr = File.ReadLines(filename).ToArray();

                MyDictionary.Init(arr);
            } 
        }
    }
}
