using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace HttpParser
{
    public class HttpRequestLogParser
    {
        private const string ID = "Id";
        private const string IP = "IPAddress";
        private const string URL = "URL";
        private const string CNT = "Count";

        private DataTable _urls;
        private DataTable _ipAddress;
        private DataView _dataView;
        private DataTable _dataTable;
        private string[] _logLines;

        public HttpRequestLogParser(string logFilePath)
        {
            try
            {
                // Read all lines from the log file
                _logLines = File.ReadAllLines(logFilePath);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("HttpRequestLogParser class cannot be created due an exception {0}.", ex.Message));
            }
        }

        public void ProcessLogFile()
        {
            try
            {
                // Create data tables for the entire data, ip addresses and urls
                _dataTable = new DataTable();
                _dataTable.Columns.Add(ID, typeof(int));
                _dataTable.Columns.Add(IP, typeof(string));
                _dataTable.Columns.Add(URL, typeof(string));
                _dataTable.PrimaryKey = new DataColumn[] { _dataTable.Columns[ID] };

                _ipAddress = new DataTable();
                _ipAddress.Columns.Add(IP, typeof(string));
                _ipAddress.Columns.Add(CNT, typeof(int));
                _ipAddress.PrimaryKey = new DataColumn[] { _ipAddress.Columns[IP] };

                _urls = new DataTable();
                _urls.Columns.Add(URL, typeof(string));
                _urls.Columns.Add(CNT, typeof(int));
                _urls.PrimaryKey = new DataColumn[] { _urls.Columns[URL] };

                for (int i = 0; i < _logLines.Length; i++)
                {
                    string[] lineArray = _logLines[i].Split(' ');

                    if (lineArray[5].Contains("GET"))
                    {
                        var dr = _dataTable.NewRow();
                        dr[ID] = i + 1;
                        dr[IP] = lineArray[0];
                        dr[URL] = lineArray[6];
                        _dataTable.Rows.Add(dr);

                        CreateRow(_ipAddress, lineArray[0], IP);

                        CreateRow(_urls, lineArray[6], URL);

                        _dataTable.AcceptChanges();
                        _ipAddress.AcceptChanges();
                        _urls.AcceptChanges();
                    }
                }

                _dataView = _dataTable.DefaultView;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetNumberOfUniqueIpAddresses()
        {
            var distinctIps = _dataView.ToTable(true, IP);
            return distinctIps.Rows.Count;

            // NOTE: Can also use the _ipAddress table but wanted to have
            // a separate table for the entire data for extensibility ;-)
        }
        
        public StringBuilder GetTopThreeMostActiveIpAddresses()
        {
            return GetTopThree(_ipAddress, IP);
        }

        public StringBuilder GetTopThreeMostVisitedUrls()
        {
            return GetTopThree(_urls, URL);
        }

        private StringBuilder GetTopThree(DataTable dataTable, string columnName)
        {
            var defaultView = dataTable.DefaultView;
            defaultView.Sort = "Count desc";
            var sortedIpAddressTable = defaultView.ToTable();
            var topThreeRows = sortedIpAddressTable.Rows.Cast<DataRow>().Take(3);

            var top3 = new StringBuilder();
            foreach (var row in topThreeRows)
            {
                if (topThreeRows.Last() != row)
                {
                    top3.Append(string.Format("{0}, ", row[columnName]));
                }
                else
                {
                    top3.Append(row[columnName]);
                }
            }
            return top3;
        }

        private void CreateRow(DataTable dataTable, string value, string columnName)
        {
            var row = dataTable.NewRow();
            row[columnName] = value;
            row[CNT] = 1;

            var searchExpression = columnName + " = '" + value.ToString() + "'";
            var duplicateRow = dataTable.Select(searchExpression);

            if (duplicateRow.Length > 0)
            {
                int duplicateIndex = dataTable.Rows.IndexOf(duplicateRow[0]);
                dataTable.Rows[duplicateIndex][CNT] = int.Parse(dataTable.Rows[duplicateIndex][CNT].ToString()) + 1;
            }
            else
            {
                dataTable.Rows.Add(row);
            }
        }
    }
}
