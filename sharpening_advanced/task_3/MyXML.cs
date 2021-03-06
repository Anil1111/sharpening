﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml;
using System.Diagnostics;

namespace task_3
{
    /// <summary>
    /// Класс работы с телефонной книгой в формате XML.
    /// </summary>
    class MyXML
    {
        #region Properties
        /// <summary>
        /// Директория файла.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Наименование XML файла.
        /// </summary>
        string _fileName = "";
        public string FileName { get => _fileName + ".xml"; set => _fileName = value; }

        /// <summary>
        /// Словарь данных.
        /// </summary>
        public Dictionary<string, string> Dictionary = new Dictionary<string, string>();
        #endregion

        /// <summary>
        /// Преобразует считанные с .txt контактные данные в .xml формат.
        /// </summary>
        /// <param name="fileAddress"></param>
        public void LoadDictionaryFromTXT(string fileName)
        {
            string[] inputLines = File.ReadAllLines(this.FilePath+fileName);

            string pattern = @"\d{1}[(]\d{3}[)] \d{3}-\d{2}-\d{2}$";
            Regex regex = new Regex(pattern);

            // Записывем считанные с .txt файла данные в словарь.
            foreach (string line in inputLines)
            {
                Match match = regex.Match(line);
                if (match.Success)
                {
                    this.Dictionary.Add(line.Replace(match.Value, ""),match.Value);
                }
            }
        }

        /// <summary>
        /// Создает XML файл с корневым элементом <MyContacts>.
        /// </summary>
        /// <param name="fileName">Имя файла.</param>
        private void InitXML()
        {
            // Создание файла
            XmlTextWriter xmlWriter = new XmlTextWriter(this.FilePath + this.FileName, Encoding.UTF8);

            // Добавление корневого узла
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("MyContacts");                  // < MyContacts >
            xmlWriter.WriteEndElement();                                // </ MyContacts >

            xmlWriter.Close();
        }

        /// <summary>
        /// Добавляет данные из словаря в xml.
        /// </summary>
        public void WriteData()
        {
            // получаем xml документ
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(this.FilePath+this.FileName);

            // обращаемся к корневому узлу <MyContacts>
            XmlElement root = xDoc.DocumentElement;
            
            // переносим данные из словаря в XML файл.
            foreach (KeyValuePair<string, string> record in this.Dictionary)
            {
                XmlElement Contact = xDoc.CreateElement("Contact");
                Contact.SetAttribute("Name", record.Key);
                Contact.SetAttribute("TelNumber", record.Value);
                root.AppendChild(Contact);
            }
            xDoc.Save(this.FilePath+this.FileName);
        }

        /// <summary>
        /// Выводит содержимое xml файла.
        /// </summary>
        public void PrintFileData()
        {
            // получаем xml документ
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(this.FilePath + this.FileName);

            Console.WriteLine($"Содержимое файла {this.FileName}:\n");
            foreach (XmlNode node in xDoc.DocumentElement.ChildNodes)
            {
                // выводим содержимое атрибутов каждого узла
                string name = node.Attributes["Name"]?.InnerText;
                string telNum = node.Attributes["TelNumber"]?.InnerText;
                Console.WriteLine($"Name: {name} \n TelNumber: {telNum}");
            }
            Console.WriteLine();
        }

        // TODO: Доделать поиск по XML
        // ссылка на пример: https://stackoverflow.com/questions/5173062/what-is-a-fastest-way-to-do-search-through-xml
        public void SearchByTel(string PhoneNumber)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            XmlReader reader = XmlReader.Create(this.FilePath + this.FileName, settings);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            reader.MoveToContent();
            while (reader.Read())
            {
                
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="filePath">Путь до файла.</param>
        /// <param name="fileName">Имя XML файла.</param>
        /// <param name="dataFileName">Имя TXT файла.</param>
        public MyXML(string filePath, string fileName, string dataFileName)
        {
            this.FilePath = filePath;
            this.FileName = fileName;
            this.LoadDictionaryFromTXT(dataFileName);
            this.InitXML();
        }
    }
}
