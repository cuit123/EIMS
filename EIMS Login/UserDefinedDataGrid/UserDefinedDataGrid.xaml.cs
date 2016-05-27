﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;

namespace EIMS_Login.UserDefinedDataGrid
{
    /// <summary>
    /// UserDefinedDataGrid.xaml 的交互逻辑
    /// </summary>
    public partial class UserDefinedDataGrid : UserControl
    {
        Connection Temp = new Connection();
        bool UpDate = true;
        bool Save = false;
        public UserDefinedDataGrid()
        {
            InitializeComponent();

            
        }
        public void InitTableHeightWidth(int Height,int Width)
        {
            dataGrid.Height = Height;
            dataGrid.Width = Width;
            dataGrid.FontSize = 16;
            
        }
        //增加列
        public void AddColumns(string Binding,string Header,int Width)
        {
            dataGrid.AutoGenerateColumns = false;
            System.Windows.Data.Binding binding = new System.Windows.Data.Binding(Binding);
            binding.Mode = System.Windows.Data.BindingMode.OneWay;
            DataGridTextColumn Temp = new DataGridTextColumn();
            Temp.Header = Header;
            Temp.Width = Width;
            Temp.Binding = binding;
            Temp.ElementStyle = Resources["MyDataGrid"] as Style;
            dataGrid.Columns.Add(Temp);
        }
        //数据库数据绑定
        public DataSet DataTableSelect(string SQL, string SaveSlecte)
        {
            DataTable Table0 = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                SqlDataAdapter sda = new SqlDataAdapter(SQL, Temp.GetConnStr());
                
                ds.Clear();
                sda.Fill(Table0);
                if (SaveSlecte == "更新")
                    dataGrid.ItemsSource = Table0.DefaultView;
                else if (SaveSlecte == "保存")
                    ds.Tables.Add(Table0);
                else
                {
                    MessageBox.Show("异常！");
                }
            }
            catch
            {
                MessageBox.Show("出现异常！");
            }
            return ds;
            
        }
        //增加行号及自动加1
        private void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        public string ExportExcel(string SQL,string []StrCloumns, string saveFileName)
        {
            try
            {
                DataSet ds = DataTableSelect(SQL,"保存");
                ChangeColumnName(ds, StrCloumns);
                if (ds == null)
                    return "数据库为空";

                bool fileSaved = false;
                Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                if (xlApp == null)
                {
                    return "无法创建Excel对象，可能您的机子未安装Excel";
                }
                Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
                Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
                Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];//取得sheet1
                                                                                                                                      //写入字段
                for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1] = ds.Tables[0].Columns[i].ColumnName;
                }
                //写入数值
                for (int r = 0; r < ds.Tables[0].Rows.Count; r++)
                {
                    for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                    {
                        worksheet.Cells[r + 2, i + 1] = ds.Tables[0].Rows[r][i];
                    }
                    System.Windows.Forms.Application.DoEvents();
                }
                worksheet.Columns.EntireColumn.AutoFit();//列宽自适应。
                if (saveFileName != "")
                {
                    try
                    {
                        workbook.Saved = true;
                        workbook.SaveCopyAs(saveFileName);
                        fileSaved = true;
                    }
                    catch (Exception ex)
                    {
                        fileSaved = false;
                        MessageBox.Show("导出文件时出错,文件可能正被打开！\n" + ex.Message);
                    }
                }
                else
                {
                    fileSaved = false;
                }
                xlApp.Quit();
                GC.Collect();//强行销毁
                if (fileSaved && System.IO.File.Exists(saveFileName)) System.Diagnostics.Process.Start(saveFileName); //打开EXCEL
                return "成功保存到Excel";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public void ChangeColumnName(DataSet ds,string []StrCloumns)
        {
            for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
            {
                ds.Tables[0].Columns[i].ColumnName = StrCloumns[i];
            }
            
        }
    }
}
